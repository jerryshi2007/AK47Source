using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MCS.Library.Office.OpenXml.Excel.Encryption
{
	/// <summary>
	/// 处理EncryptionInfo流
	/// </summary>
	internal class EncryptionInfo
	{
		internal short MajorVersion;
		internal short MinorVersion;
		internal Flags Flags;
		internal uint HeaderSize;
		internal EncryptionHeader Header;
		internal EncryptionVerifier Verifier;
		internal void ReadBinary(byte[] data)
		{
			MajorVersion = BitConverter.ToInt16(data, 0);
			MinorVersion = BitConverter.ToInt16(data, 2);

			Flags = (Flags)BitConverter.ToInt32(data, 4);
			HeaderSize = (uint)BitConverter.ToInt32(data, 8);

			/**** EncryptionHeader ****/
			Header = new EncryptionHeader();
			Header.Flags = (Flags)BitConverter.ToInt32(data, 12);
			Header.SizeExtra = BitConverter.ToInt32(data, 16);
			Header.AlgID = (AlgorithmID)BitConverter.ToInt32(data, 20);
			Header.AlgIDHash = (AlgorithmHashID)BitConverter.ToInt32(data, 24);
			Header.KeySize = BitConverter.ToInt32(data, 28);
			Header.ProviderType = (ProviderType)BitConverter.ToInt32(data, 32);
			Header.Reserved1 = BitConverter.ToInt32(data, 36);
			Header.Reserved2 = BitConverter.ToInt32(data, 40);

			byte[] text = new byte[(int)HeaderSize - 34];
			Array.Copy(data, 44, text, 0, (int)HeaderSize - 34);
			Header.CSPName = UTF8Encoding.Unicode.GetString(text);

			int pos = (int)HeaderSize + 12;

			/**** EncryptionVerifier ****/
			Verifier = new EncryptionVerifier();
			Verifier.SaltSize = (uint)BitConverter.ToInt32(data, pos);
			Verifier.Salt = new byte[Verifier.SaltSize];

			Array.Copy(data, pos + 4, Verifier.Salt, 0, Verifier.SaltSize);

			Verifier.EncryptedVerifier = new byte[16];
			Array.Copy(data, pos + 20, Verifier.EncryptedVerifier, 0, 16);

			Verifier.VerifierHashSize = (uint)BitConverter.ToInt32(data, pos + 36);
			Verifier.EncryptedVerifierHash = new byte[Verifier.VerifierHashSize];
			Array.Copy(data, pos + 40, Verifier.EncryptedVerifierHash, 0, Verifier.VerifierHashSize);
		}

		internal byte[] WriteBinary()
		{
			MemoryStream ms = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(ms);

			bw.Write(MajorVersion);
			bw.Write(MinorVersion);
			bw.Write((int)Flags);
			byte[] header = Header.WriteBinary();
			bw.Write((uint)header.Length);
			bw.Write(header);
			bw.Write(Verifier.WriteBinary());

			bw.Flush();
			return ms.ToArray();
		}

	}
}
