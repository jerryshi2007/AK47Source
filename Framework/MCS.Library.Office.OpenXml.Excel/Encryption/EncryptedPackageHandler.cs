using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using comTypes = System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel.Encryption
{
	[Flags]
	internal enum Flags
	{
		Reserved1 = 1,   // (1 bit): MUST be set to zero, and MUST be ignored.
		Reserved2 = 2,   // (1 bit): MUST be set to zero, and MUST be ignored.
		fCryptoAPI = 4,   // (1 bit): A flag that specifies whether CryptoAPI RC4 or [ECMA-376] encryption is used. It MUST be set to 1 unless fExternal is 1. If fExternal is set to 1, it MUST be set to zero.        
		fDocProps = 8,   // (1 bit): MUST be set to zero if document properties are encrypted. Otherwise, it MUST be set to 1. Encryption of document properties is specified in section 2.3.5.4.
		fExternal = 16,  // (1 bit): If extensible encryption is used, it MUST be set to 1. Otherwise, it MUST be set to zero. If this field is set to 1, all other fields in this structure MUST be set to zero.
		fAES = 32   //(1 bit): If the protected content is an [ECMA-376] document, it MUST be set to 1. Otherwise, it MUST be set to zero. If the fAES bit is set to 1, the fCryptoAPI bit MUST also be set to 1
	}
	internal enum AlgorithmID
	{
		Flags = 0x00000000,   // Determined by Flags
		RC4 = 0x00006801,   // RC4
		AES128 = 0x0000660E,   // 128-bit AES
		AES192 = 0x0000660F,   // 192-bit AES
		AES256 = 0x00006610    // 256-bit AES
	}
	internal enum AlgorithmHashID
	{
		App = 0x00000000,
		SHA1 = 0x00008004,
	}
	internal enum ProviderType
	{
		Flags = 0x00000000,//Determined by Flags
		RC4 = 0x00000001,
		AES = 0x00000018,
	}

	/// <summary>
	/// 加密Excel文档
	/// </summary>
	internal class EncryptedPackageHandler
	{
		[DllImport("ole32.dll")]
		private static extern int StgIsStorageFile(
		[MarshalAs(UnmanagedType.LPWStr)] string pwcsName);
		[DllImport("ole32.dll")]
		private static extern int StgIsStorageILockBytes(ILockBytes plkbyt);


		[DllImport("ole32.dll")]
		static extern int StgOpenStorage(
			[MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
			IStorage pstgPriority,
			STGM grfMode,
			IntPtr snbExclude,
			uint reserved,
			out IStorage ppstgOpen);

		[DllImport("ole32.dll")]
		static extern int StgOpenStorageOnILockBytes(
			ILockBytes plkbyt,
			IStorage pStgPriority,
			STGM grfMode,
			IntPtr snbEnclude,
			uint reserved,
			out IStorage ppstgOpen);
		[DllImport("ole32.dll")]
		static extern int CreateILockBytesOnHGlobal(
			IntPtr hGlobal,
			bool fDeleteOnRelease,
			out ILockBytes ppLkbyt);

		[DllImport("ole32.dll")]
		static extern int StgCreateDocfileOnILockBytes(ILockBytes plkbyt, STGM grfMode, int reserved, out IStorage ppstgOpen);
		internal static int IsStorageFile(string Name)
		{
			return StgIsStorageFile(Name);
		}

		internal static int IsStorageILockBytes(ILockBytes lb)
		{
			return StgIsStorageILockBytes(lb);
		}

		/// <summary>
		/// 解密提取Package
		/// </summary>
		/// <param name="fi">文件</param>
		/// <param name="encryption"></param>
		/// <returns></returns>
		internal MemoryStream DecryptPackage(FileInfo fi, ExcelEncryption encryption)
		{
			ExceptionHelper.TrueThrow(StgIsStorageFile(fi.FullName) != 0, "文件{0}不是一个加密包", fi.FullName);
			MemoryStream ret = null;
			IStorage storage = null;
			if (StgOpenStorage(
				fi.FullName,
				null,
				STGM.DIRECT | STGM.READ | STGM.SHARE_EXCLUSIVE,
				IntPtr.Zero,
				0,
				out storage) == 0)
			{
				ret = GetStreamFromPackage(storage, encryption);
				Marshal.ReleaseComObject(storage);
			}

			return ret;
		}

		/// <summary>
		/// 解密提取package
		/// </summary>
		/// <param name="stream">内存流 </param>
		/// <param name="encryption">加密对象</param>
		/// <returns></returns>
		internal MemoryStream DecryptPackage(MemoryStream stream, ExcelEncryption encryption)
		{
			//创建lockBytes对象。
			ILockBytes lb = GetLockbyte(stream);

			ExceptionHelper.TrueThrow<Exception>(StgIsStorageILockBytes(lb) != 0, "不是加密文件");

			MemoryStream ret = null;

			IStorage storage = null;
			if (StgOpenStorageOnILockBytes(
					lb,
					null,
					STGM.DIRECT | STGM.READ | STGM.SHARE_EXCLUSIVE,
					IntPtr.Zero,
					0,
					out storage) == 0)
			{
				ret = GetStreamFromPackage(storage, encryption);
			}
			Marshal.ReleaseComObject(storage);

			Marshal.ReleaseComObject(lb);

			return ret;
		}

		internal ILockBytes GetLockbyte(MemoryStream stream)
		{
			ILockBytes lb;
			var iret = CreateILockBytesOnHGlobal(IntPtr.Zero, true, out lb);
			byte[] docArray = stream.GetBuffer();
			IntPtr buffer = Marshal.AllocHGlobal(docArray.Length);
			Marshal.Copy(docArray, 0, buffer, docArray.Length);
			UIntPtr readSize;
			lb.WriteAt(0, buffer, docArray.Length, out readSize);
			Marshal.FreeHGlobal(buffer);
			return lb;
		}

		/// <summary>
		/// 加密Package
		/// </summary>
		/// <param name="package">作为一个字节数组封装</param>
		/// <param name="encryption">加密信息</param>
		/// <returns></returns>
		internal MemoryStream EncryptPackage(byte[] package, ExcelEncryption encryption)
		{
			byte[] encryptionKey;
			//创建加密信息。这也返回了Encryptionkey
			var encryptionInfo = CreateEncryptionInfo(encryption.Password,
					encryption.Algorithm == EncryptionAlgorithm.AES128 ?
						AlgorithmID.AES128 :
					encryption.Algorithm == EncryptionAlgorithm.AES192 ?
						AlgorithmID.AES192 :
						AlgorithmID.AES256, out encryptionKey);

			ILockBytes lb;
			var iret = CreateILockBytesOnHGlobal(IntPtr.Zero, true, out lb);

			IStorage storage = null;
			MemoryStream ret = null;

			//创建内存中的文件
			if (StgCreateDocfileOnILockBytes(lb,
					STGM.CREATE | STGM.READWRITE | STGM.SHARE_EXCLUSIVE | STGM.TRANSACTED,
					0,
					out storage) == 0)
			{
				//创建数据空间存储
				CreateDataSpaces(storage);

				//创建加密信息流
				comTypes.IStream stream;
				storage.CreateStream("EncryptionInfo", (uint)(STGM.CREATE | STGM.WRITE | STGM.DIRECT | STGM.SHARE_EXCLUSIVE), (uint)0, (uint)0, out stream);
				byte[] ei = encryptionInfo.WriteBinary();
				stream.Write(ei, ei.Length, IntPtr.Zero);
				stream = null;

				//加密包
				byte[] encryptedPackage = EncryptData(encryptionKey, package, false);

				storage.CreateStream("EncryptedPackage", (uint)(STGM.CREATE | STGM.WRITE | STGM.DIRECT | STGM.SHARE_EXCLUSIVE), (uint)0, (uint)0, out stream);

				//文件大小
				MemoryStream ms = new MemoryStream();
				BinaryWriter bw = new BinaryWriter(ms);
				bw.Write((ulong)package.LongLength);
				bw.Flush();
				byte[] length = ms.ToArray();
				//先写加密的数据长度
				stream.Write(length, length.Length, IntPtr.Zero);
				//现在加密的数据
				stream.Write(encryptedPackage, encryptedPackage.Length, IntPtr.Zero);
				stream = null;
				storage.Commit(0);
				lb.Flush();

				//复制非托管的流为字节数组
				var statstg = new comTypes.STATSTG();
				lb.Stat(out statstg, 0);
				int size = (int)statstg.cbSize;
				IntPtr buffer = Marshal.AllocHGlobal(size);
				UIntPtr readSize;
				byte[] pack = new byte[size];
				lb.ReadAt(0, buffer, size, out readSize);
				Marshal.Copy(buffer, pack, 0, size);
				Marshal.FreeHGlobal(buffer);

				ret = new MemoryStream();
				ret.Write(pack, 0, size);
			}
			Marshal.ReleaseComObject(storage);
			Marshal.ReleaseComObject(lb);
			return ret;
		}
		#region "Dataspaces Stream methods"
		private void CreateDataSpaces(IStorage storage)
		{
			IStorage dataSpaces;
			storage.CreateStorage("\x06" + "DataSpaces", (uint)(STGM.CREATE | STGM.WRITE | STGM.DIRECT | STGM.SHARE_EXCLUSIVE), 0, 0, out dataSpaces);
			storage.Commit(0);

			//Version Stream
			comTypes.IStream versionStream;
			dataSpaces.CreateStream("Version", (uint)(STGM.CREATE | STGM.WRITE | STGM.DIRECT | STGM.SHARE_EXCLUSIVE), 0, 0, out versionStream);
			byte[] version = CreateVersionStream();
			versionStream.Write(version, version.Length, IntPtr.Zero);

			//DataSpaceMap
			comTypes.IStream dataSpaceMapStream;
			dataSpaces.CreateStream("DataSpaceMap", (uint)(STGM.CREATE | STGM.WRITE | STGM.DIRECT | STGM.SHARE_EXCLUSIVE), 0, 0, out dataSpaceMapStream);
			byte[] dataSpaceMap = CreateDataSpaceMap();
			dataSpaceMapStream.Write(dataSpaceMap, dataSpaceMap.Length, IntPtr.Zero);

			//DataSpaceInfo
			IStorage dataSpaceInfo;
			dataSpaces.CreateStorage("DataSpaceInfo", (uint)(STGM.CREATE | STGM.WRITE | STGM.DIRECT | STGM.SHARE_EXCLUSIVE), 0, 0, out dataSpaceInfo);

			comTypes.IStream strongEncryptionDataSpaceStream;
			dataSpaceInfo.CreateStream("StrongEncryptionDataSpace", (uint)(STGM.CREATE | STGM.WRITE | STGM.DIRECT | STGM.SHARE_EXCLUSIVE), 0, 0, out strongEncryptionDataSpaceStream);
			byte[] strongEncryptionDataSpace = CreateStrongEncryptionDataSpaceStream();
			strongEncryptionDataSpaceStream.Write(strongEncryptionDataSpace, strongEncryptionDataSpace.Length, IntPtr.Zero);
			dataSpaceInfo.Commit(0);

			//TransformInfo
			IStorage tranformInfo;
			dataSpaces.CreateStorage("TransformInfo", (uint)(STGM.CREATE | STGM.WRITE | STGM.DIRECT | STGM.SHARE_EXCLUSIVE), 0, 0, out tranformInfo);

			IStorage strongEncryptionTransform;
			tranformInfo.CreateStorage("StrongEncryptionTransform", (uint)(STGM.CREATE | STGM.WRITE | STGM.DIRECT | STGM.SHARE_EXCLUSIVE), 0, 0, out strongEncryptionTransform);

			comTypes.IStream primaryStream;
			strongEncryptionTransform.CreateStream("\x06Primary", (uint)(STGM.CREATE | STGM.WRITE | STGM.DIRECT | STGM.SHARE_EXCLUSIVE), 0, 0, out primaryStream);
			byte[] primary = CreateTransformInfoPrimary();
			primaryStream.Write(primary, primary.Length, IntPtr.Zero);
			tranformInfo.Commit(0);
			dataSpaces.Commit(0);
		}

		private byte[] CreateStrongEncryptionDataSpaceStream()
		{
			MemoryStream ms = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(ms);

			bw.Write((int)8);       //HeaderLength
			bw.Write((int)1);       //EntryCount

			string tr = "StrongEncryptionTransform";
			bw.Write((int)tr.Length);
			bw.Write(UTF8Encoding.Unicode.GetBytes(tr + "\0")); // end \0 is for padding

			bw.Flush();
			return ms.ToArray();
		}

		private byte[] CreateVersionStream()
		{
			MemoryStream ms = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(ms);

			bw.Write((short)0x3C);  //Major
			bw.Write((short)0);     //Minor
			bw.Write(UTF8Encoding.Unicode.GetBytes("Microsoft.Container.DataSpaces"));
			bw.Write((int)1);       //ReaderVersion
			bw.Write((int)1);       //UpdaterVersion
			bw.Write((int)1);       //WriterVersion

			bw.Flush();
			return ms.ToArray();
		}

		private byte[] CreateDataSpaceMap()
		{
			MemoryStream ms = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(ms);

			bw.Write((int)8);       //HeaderLength
			bw.Write((int)1);       //EntryCount
			string s1 = "EncryptedPackage";
			string s2 = "StrongEncryptionDataSpace";
			bw.Write((int)s1.Length + s2.Length + 0x14);
			bw.Write((int)1);       //ReferenceComponentCount
			bw.Write((int)0);       //Stream=0
			bw.Write((int)s1.Length * 2); //Length s1
			bw.Write(UTF8Encoding.Unicode.GetBytes(s1));
			bw.Write((int)(s2.Length - 1) * 2);   //Length s2
			bw.Write(UTF8Encoding.Unicode.GetBytes(s2 + "\0"));   // end \0 is for padding

			bw.Flush();
			return ms.ToArray();
		}

		private byte[] CreateTransformInfoPrimary()
		{
			MemoryStream ms = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(ms);
			string TransformID = "{FF9A3F03-56EF-4613-BDD5-5A41C1D07246}";
			string TransformName = "Microsoft.Container.EncryptionTransform";
			bw.Write(TransformID.Length * 2 + 12);
			bw.Write((int)1);
			bw.Write(TransformID.Length * 2);
			bw.Write(UTF8Encoding.Unicode.GetBytes(TransformID));
			bw.Write(TransformName.Length * 2);
			bw.Write(UTF8Encoding.Unicode.GetBytes(TransformName + "\0"));
			bw.Write((int)1);   //ReaderVersion
			bw.Write((int)1);   //UpdaterVersion
			bw.Write((int)1);   //WriterVersion

			bw.Write((int)0);
			bw.Write((int)0);
			bw.Write((int)0);       //CipherMode
			bw.Write((int)4);       //Reserved

			bw.Flush();
			return ms.ToArray();
		}

		#endregion

		/// <summary>
		/// Create an EncryptionInfo object to encrypt a workbook
		/// </summary>
		/// <param name="password">The password</param>
		/// <param name="algID"></param>
		/// <param name="key">The Encryption key</param>
		/// <returns></returns>
		private EncryptionInfo CreateEncryptionInfo(string password, AlgorithmID algID, out byte[] key)
		{
			ExceptionHelper.TrueThrow<ArgumentException>(algID == AlgorithmID.Flags || algID == AlgorithmID.RC4, "必须为aes128，AES192或AES256");

			var encryptionInfo = new EncryptionInfo();
			encryptionInfo.MajorVersion = 4;
			encryptionInfo.MinorVersion = 2;
			encryptionInfo.Flags = Flags.fAES | Flags.fCryptoAPI;

			//Header
			encryptionInfo.Header = new EncryptionHeader();
			encryptionInfo.Header.AlgID = algID;
			encryptionInfo.Header.AlgIDHash = AlgorithmHashID.SHA1;
			encryptionInfo.Header.Flags = encryptionInfo.Flags;
			encryptionInfo.Header.KeySize =
				(algID == AlgorithmID.AES128 ? 0x80 : algID == AlgorithmID.AES192 ? 0xC0 : 0x100);
			encryptionInfo.Header.ProviderType = ProviderType.AES;
			encryptionInfo.Header.CSPName = "Microsoft Enhanced RSA and AES Cryptographic Provider\0";
			encryptionInfo.Header.Reserved1 = 0;
			encryptionInfo.Header.Reserved2 = 0;
			encryptionInfo.Header.SizeExtra = 0;

			//Verifier
			encryptionInfo.Verifier = new EncryptionVerifier();
			encryptionInfo.Verifier.Salt = new byte[16];

			var rnd = RandomNumberGenerator.Create();
			rnd.GetBytes(encryptionInfo.Verifier.Salt);
			encryptionInfo.Verifier.SaltSize = 0x10;

			key = GetPasswordHash(password, encryptionInfo);

			var verifier = new byte[16];
			rnd.GetBytes(verifier);
			encryptionInfo.Verifier.EncryptedVerifier = EncryptData(key, verifier, true);

			//AES = 32 Bits
			encryptionInfo.Verifier.VerifierHashSize = 0x20;
			SHA1 sha = new SHA1Managed();
			var verifierHash = sha.ComputeHash(verifier);

			encryptionInfo.Verifier.EncryptedVerifierHash = EncryptData(key, verifierHash, false);

			return encryptionInfo;
		}

		private byte[] EncryptData(byte[] key, byte[] data, bool useDataSize)
		{
			RijndaelManaged aes = new RijndaelManaged();
			aes.KeySize = key.Length * 8;
			aes.Mode = CipherMode.ECB;
			aes.Padding = PaddingMode.Zeros;

			//Encrypt the data
			var crypt = aes.CreateEncryptor(key, null);
			var ms = new MemoryStream();
			var cs = new CryptoStream(ms, crypt, CryptoStreamMode.Write);
			cs.Write(data, 0, data.Length);

			cs.FlushFinalBlock();

			byte[] ret;
			if (useDataSize)
			{
				ret = new byte[data.Length];
				ms.Seek(0, SeekOrigin.Begin);
				ms.Read(ret, 0, data.Length);  //Truncate any padded Zeros
				return ret;
			}
			else
			{
				return ms.ToArray();
			}
		}

		private MemoryStream GetStreamFromPackage(IStorage storage, ExcelEncryption encryption)
		{
			MemoryStream ret = null;
			comTypes.STATSTG statstg;

			storage.Stat(out statstg, (uint)STATFLAG.STATFLAG_DEFAULT);

			IEnumSTATSTG pIEnumStatStg = null;
			storage.EnumElements(0, IntPtr.Zero, 0, out pIEnumStatStg);

			comTypes.STATSTG[] regelt = { statstg };
			uint fetched = 0;
			uint res = pIEnumStatStg.Next(1, regelt, out fetched);

			if (res == 0)
			{
				byte[] data;
				EncryptionInfo encryptionInfo = null;
				while (res != 1)
				{
					switch (statstg.pwcsName)
					{
						case "EncryptionInfo":
							data = GetOleStream(storage, statstg);

							encryptionInfo = new EncryptionInfo();
							encryptionInfo.ReadBinary(data);

							encryption.Algorithm = encryptionInfo.Header.AlgID == AlgorithmID.AES128 ?
								EncryptionAlgorithm.AES128 :
							encryptionInfo.Header.AlgID == AlgorithmID.AES192 ?
								EncryptionAlgorithm.AES192 :
								EncryptionAlgorithm.AES256;
							break;
						case "EncryptedPackage":
							data = GetOleStream(storage, statstg);
							ret = DecryptDocument(data, encryptionInfo, encryption.Password);
							break;
					}

					if ((res = pIEnumStatStg.Next(1, regelt, out fetched)) != 1)
					{
						statstg = regelt[0];
					}
				}
			}
			Marshal.ReleaseComObject(pIEnumStatStg);
			return ret;
		}
		// Help method to print a storage part binary to c:\temp
		//private void PrintStorage(IStorage storage, System.Runtime.InteropServices.ComTypes.STATSTG sTATSTG, string topName)
		//{
		//    IStorage ds;
		//    if (topName.Length > 0)
		//    {
		//        topName = topName[0] < 'A' ? topName.Substring(1, topName.Length - 1) : topName;
		//    }
		//    storage.OpenStorage(sTATSTG.pwcsName,
		//        null,
		//        (uint)(STGM.DIRECT | STGM.READ | STGM.SHARE_EXCLUSIVE),
		//        IntPtr.Zero,
		//        0,
		//        out ds);

		//    System.Runtime.InteropServices.ComTypes.STATSTG statstgSub;
		//    ds.Stat(out statstgSub, (uint)STATFLAG.STATFLAG_DEFAULT);

		//    IEnumSTATSTG pIEnumStatStgSub = null;
		//    System.Runtime.InteropServices.ComTypes.STATSTG[] regeltSub = { statstgSub };
		//    ds.EnumElements(0, IntPtr.Zero, 0, out pIEnumStatStgSub);

		//    uint fetched = 0;
		//    while (pIEnumStatStgSub.Next(1, regeltSub, out fetched) == 0)
		//    {
		//        string sName = regeltSub[0].pwcsName[0] < 'A' ? regeltSub[0].pwcsName.Substring(1, regeltSub[0].pwcsName.Length - 1) : regeltSub[0].pwcsName;
		//        if (regeltSub[0].type == 1)
		//        {
		//            PrintStorage(ds, regeltSub[0], topName + sName + "_");
		//        }
		//        else if(regeltSub[0].type==2)
		//        {
		//            File.WriteAllBytes(@"c:\temp\" + topName + sName + ".bin", GetOleStream(ds, regeltSub[0]));
		//        }
		//    }
		//}

		/// <summary>
		/// 解密文档
		/// </summary>
		/// <param name="data">加密的数据</param>
		/// <param name="encryptionInfo">加密信息对象</param>
		/// <param name="password">密码</param>
		/// <returns></returns>
		private MemoryStream DecryptDocument(byte[] data, EncryptionInfo encryptionInfo, string password)
		{
			ExceptionHelper.TrueThrow<Exception>(encryptionInfo == null, "无效的文件缺少EncryptionInfo。");

			long size = BitConverter.ToInt64(data, 0);

			var encryptedData = new byte[data.Length - 8];
			Array.Copy(data, 8, encryptedData, 0, encryptedData.Length);

			MemoryStream doc = new MemoryStream();

			ExceptionHelper.FalseThrow<UnauthorizedAccessException>(encryptionInfo.Header.AlgID == AlgorithmID.AES128
				|| (encryptionInfo.Header.AlgID == AlgorithmID.Flags && ((encryptionInfo.Flags & (Flags.fAES | Flags.fExternal | Flags.fCryptoAPI)) == (Flags.fAES | Flags.fCryptoAPI)))
				|| encryptionInfo.Header.AlgID == AlgorithmID.AES192
				|| encryptionInfo.Header.AlgID == AlgorithmID.AES256, "无效的密码");

			RijndaelManaged decryptKey = new RijndaelManaged();
			decryptKey.KeySize = encryptionInfo.Header.KeySize;
			decryptKey.Mode = CipherMode.ECB;
			decryptKey.Padding = PaddingMode.None;

			var key = GetPasswordHash(password, encryptionInfo);

			if (IsPasswordValid(key, encryptionInfo))
			{
				ICryptoTransform decryptor = decryptKey.CreateDecryptor(key, null);

				MemoryStream dataStream = new MemoryStream(encryptedData);

				CryptoStream cryptoStream = new CryptoStream(dataStream, decryptor, CryptoStreamMode.Read);

				byte[] decryptedData = new byte[size];
				cryptoStream.Read(decryptedData, 0, (int)size);
				doc.Write(decryptedData, 0, (int)size);
			}
			return doc;
		}

		/// <summary>
		///验证密码
		/// </summary>
		/// <param name="key">加密密钥</param>
		/// <param name="encryptionInfo">从ENCRYPTIOINFO流中提取里面的OLE文档的加密信息</param>
		/// <returns></returns>
		private bool IsPasswordValid(byte[] key, EncryptionInfo encryptionInfo)
		{
			RijndaelManaged decryptKey = new RijndaelManaged();
			decryptKey.KeySize = encryptionInfo.Header.KeySize;
			decryptKey.Mode = CipherMode.ECB;
			decryptKey.Padding = PaddingMode.None;

			ICryptoTransform decryptor = decryptKey.CreateDecryptor(key, null);

			//解密验证器
			MemoryStream dataStream = new MemoryStream(encryptionInfo.Verifier.EncryptedVerifier);
			CryptoStream cryptoStream = new CryptoStream(dataStream, decryptor, CryptoStreamMode.Read);

			byte[] decryptedVerifier = new byte[16];
			cryptoStream.Read(decryptedVerifier, 0, 16);

			dataStream = new MemoryStream(encryptionInfo.Verifier.EncryptedVerifierHash);

			cryptoStream = new CryptoStream(dataStream, decryptor, CryptoStreamMode.Read);

			//解密验证哈希
			byte[] decryptedVerifierHash = new byte[16];
			cryptoStream.Read(decryptedVerifierHash, 0, (int)16);

			//获取哈希解密验证
			var sha = new SHA1Managed();
			var hash = sha.ComputeHash(decryptedVerifier);

			//Equal?
			for (int i = 0; i < 16; i++)
			{
				if (hash[i] != decryptedVerifierHash[i])
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		///读取流，并返回一个字节数组
		/// </summary>
		/// <param name="storage"></param>
		/// <param name="statstg"></param>
		/// <returns></returns>
		private byte[] GetOleStream(IStorage storage, comTypes.STATSTG statstg)
		{
			comTypes.IStream pIStream;
			storage.OpenStream(statstg.pwcsName,
			   IntPtr.Zero,
			   (uint)(STGM.READ | STGM.SHARE_EXCLUSIVE),
			   0,
			   out pIStream);

			byte[] data = new byte[statstg.cbSize];
			pIStream.Read(data, (int)statstg.cbSize, IntPtr.Zero);
			Marshal.ReleaseComObject(pIStream);

			return data;
		}

		/// <summary>
		/// 创建哈希。
		/// </summary>
		/// <param name="password">密码</param>
		/// <param name="encryptionInfo">从ENCRYPTIOINFO流中提取里面的OLE文档的加密信息</param>
		/// <returns>哈希加密文件</returns>
		private byte[] GetPasswordHash(string password, EncryptionInfo encryptionInfo)
		{
			byte[] hash = null;
			byte[] tempHash = new byte[4 + 20];    //Iterator + prev. hash
			try
			{
				HashAlgorithm hashProvider;
				if (encryptionInfo.Header.AlgIDHash == AlgorithmHashID.SHA1 || encryptionInfo.Header.AlgIDHash == AlgorithmHashID.App && (encryptionInfo.Flags & Flags.fExternal) == 0)
				{
					hashProvider = new SHA1CryptoServiceProvider();
				}
				else if (encryptionInfo.Header.KeySize > 0 && encryptionInfo.Header.KeySize < 80)
				{
					throw new Exception("RC4 Hash provider is not supported. Must be SHA1(AlgIDHash == 0x8004)");
				}
				else
				{
					throw new Exception("Hash provider is invalid. Must be SHA1(AlgIDHash == 0x8004)");
				}

				hash = hashProvider.ComputeHash(CombinePassword(encryptionInfo.Verifier.Salt, password));

				//Iterate 50 000 times, inserting i in first 4 bytes and then the prev. hash in byte 5-24
				for (int i = 0; i < 50000; i++)
				{
					Array.Copy(BitConverter.GetBytes(i), tempHash, 4);
					Array.Copy(hash, 0, tempHash, 4, hash.Length);

					hash = hashProvider.ComputeHash(tempHash);
				}

				// Append "block" (0)
				Array.Copy(hash, tempHash, hash.Length);
				Array.Copy(System.BitConverter.GetBytes(0), 0, tempHash, hash.Length, 4);
				hash = hashProvider.ComputeHash(tempHash);

				/***** Now use the derived key algorithm *****/
				byte[] derivedKey = new byte[64];
				int keySizeBytes = encryptionInfo.Header.KeySize / 8;

				//First XOR hash bytes with 0x36 and fill the rest with 0x36
				for (int i = 0; i < derivedKey.Length; i++)
					derivedKey[i] = (byte)(i < hash.Length ? 0x36 ^ hash[i] : 0x36);


				byte[] X1 = hashProvider.ComputeHash(derivedKey);

				//if verifier size is bigger than the key size we can return X1
				if (encryptionInfo.Verifier.VerifierHashSize > keySizeBytes)
					return FixHashSize(X1, keySizeBytes);

				//Else XOR hash bytes with 0x5C and fill the rest with 0x5C
				for (int i = 0; i < derivedKey.Length; i++)
					derivedKey[i] = (byte)(i < hash.Length ? 0x5C ^ hash[i] : 0x5C);

				byte[] X2 = hashProvider.ComputeHash(derivedKey);

				//Join the two and return 
				byte[] join = new byte[X1.Length + X2.Length];

				Array.Copy(X1, 0, join, 0, X1.Length);
				Array.Copy(X2, 0, join, X1.Length, X2.Length);

				return FixHashSize(join, keySizeBytes);
			}
			catch (Exception ex)
			{
				throw (new Exception("创建encryptionkey时发生错误", ex));
			}
		}

		private byte[] FixHashSize(byte[] hash, int size)
		{
			byte[] buff = new byte[size];
			Array.Copy(hash, buff, size);
			return buff;
		}

		private byte[] CombinePassword(byte[] salt, string password)
		{
			if (password == "")
			{
				password = "VelvetSweatshop";   //Used if Password is blank
			}
			// Convert password to unicode...
			byte[] passwordBuf = UnicodeEncoding.Unicode.GetBytes(password);

			byte[] inputBuf = new byte[salt.Length + passwordBuf.Length];
			Array.Copy(salt, inputBuf, salt.Length);
			Array.Copy(passwordBuf, 0, inputBuf, salt.Length, passwordBuf.Length);
			return inputBuf;
		}

		internal static ushort CalculatePasswordHash(string Password)
		{
			//Calculate the hash
			//Thanks to Kohei Yoshida for the sample http://kohei.us/2008/01/18/excel-sheet-protection-password-hash/
			ushort hash = 0;
			for (int i = Password.Length - 1; i >= 0; i--)
			{
				hash ^= Password[i];
				hash = (ushort)(((ushort)((hash >> 14) & 0x01))
								|
								((ushort)((hash << 1) & 0x7FFF)));  //Shift 1 to the left. Overflowing bit 15 goes into bit 0
			}

			hash ^= (0x8000 | ('N' << 8) | 'K'); //Xor NK with high bit set(0xCE4B)
			hash ^= (ushort)Password.Length;

			return hash;
		}
	}

	[ComImport]
	[Guid("0000000d-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IEnumSTATSTG
	{
		// The user needs to allocate an STATSTG array whose size is celt. 
		[PreserveSig]
		uint Next(
			uint celt,
			[MarshalAs(UnmanagedType.LPArray), Out] 
            System.Runtime.InteropServices.ComTypes.STATSTG[] rgelt,
			out uint pceltFetched
		);

		void Skip(uint celt);

		void Reset();

		[return: MarshalAs(UnmanagedType.Interface)]
		IEnumSTATSTG Clone();
	}

	[ComImport]
	[Guid("0000000b-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	interface IStorage
	{
		void CreateStream(
			/* [string][in] */ string pwcsName,
			/* [in] */ uint grfMode,
			/* [in] */ uint reserved1,
			/* [in] */ uint reserved2,
			/* [out] */ out comTypes.IStream ppstm);

		void OpenStream(
			/* [string][in] */ string pwcsName,
			/* [unique][in] */ IntPtr reserved1,
			/* [in] */ uint grfMode,
			/* [in] */ uint reserved2,
			/* [out] */ out comTypes.IStream ppstm);

		void CreateStorage(
			/* [string][in] */ string pwcsName,
			/* [in] */ uint grfMode,
			/* [in] */ uint reserved1,
			/* [in] */ uint reserved2,
			/* [out] */ out IStorage ppstg);

		void OpenStorage(
			/* [string][unique][in] */ string pwcsName,
			/* [unique][in] */ IStorage pstgPriority,
			/* [in] */ uint grfMode,
			/* [unique][in] */ IntPtr snbExclude,
			/* [in] */ uint reserved,
			/* [out] */ out IStorage ppstg);

		void CopyTo(
			[InAttribute] uint ciidExclude,
			[InAttribute] Guid[] rgiidExclude,
			[InAttribute] IntPtr snbExclude,
			[InAttribute] IStorage pstgDest
		);

		void MoveElementTo(
			/* [string][in] */ string pwcsName,
			/* [unique][in] */ IStorage pstgDest,
			/* [string][in] */ string pwcsNewName,
			/* [in] */ uint grfFlags);

		void Commit(
			/* [in] */ uint grfCommitFlags);

		void Revert();

		void EnumElements(
			/* [in] */ uint reserved1,
			/* [size_is][unique][in] */ IntPtr reserved2,
			/* [in] */ uint reserved3,
			/* [out] */ out IEnumSTATSTG ppenum);

		void DestroyElement(
			/* [string][in] */ string pwcsName);

		void RenameElement(
			/* [string][in] */ string pwcsOldName,
			/* [string][in] */ string pwcsNewName);

		void SetElementTimes(
			/* [string][unique][in] */ string pwcsName,
			/* [unique][in] */ System.Runtime.InteropServices.ComTypes.FILETIME pctime,
			/* [unique][in] */ System.Runtime.InteropServices.ComTypes.FILETIME patime,
			/* [unique][in] */ System.Runtime.InteropServices.ComTypes.FILETIME pmtime);

		void SetClass(
			/* [in] */ Guid clsid);

		void SetStateBits(
			/* [in] */ uint grfStateBits,
			/* [in] */ uint grfMask);

		void Stat(
			/* [out] */ out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg,
			/* [in] */ uint grfStatFlag);

	}
	[ComVisible(false)]
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("0000000A-0000-0000-C000-000000000046")]
	internal interface ILockBytes
	{
		void ReadAt(long ulOffset, System.IntPtr pv, int cb, out UIntPtr pcbRead);
		void WriteAt(long ulOffset, System.IntPtr pv, int cb, out UIntPtr pcbWritten);
		void Flush();
		void SetSize(long cb);
		void LockRegion(long libOffset, long cb, int dwLockType);
		void UnlockRegion(long libOffset, long cb, int dwLockType);
		void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag);
	}
	[Flags]
	internal enum STGM : int
	{
		DIRECT = 0x00000000,
		TRANSACTED = 0x00010000,
		SIMPLE = 0x08000000,
		READ = 0x00000000,
		WRITE = 0x00000001,
		READWRITE = 0x00000002,
		SHARE_DENY_NONE = 0x00000040,
		SHARE_DENY_READ = 0x00000030,
		SHARE_DENY_WRITE = 0x00000020,
		SHARE_EXCLUSIVE = 0x00000010,
		PRIORITY = 0x00040000,
		DELETEONRELEASE = 0x04000000,
		NOSCRATCH = 0x00100000,
		CREATE = 0x00001000,
		CONVERT = 0x00020000,
		FAILIFTHERE = 0x00000000,
		NOSNAPSHOT = 0x00200000,
		DIRECT_SWMR = 0x00400000,
	}

	internal enum STATFLAG : uint
	{
		STATFLAG_DEFAULT = 0,
		STATFLAG_NONAME = 1,
		STATFLAG_NOOPEN = 2
	}

	internal enum STGTY : int
	{
		STGTY_STORAGE = 1,
		STGTY_STREAM = 2,
		STGTY_LOCKBYTES = 3,
		STGTY_PROPERTY = 4
	}
}
