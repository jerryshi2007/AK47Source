#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	TicketEncryption.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          ����ǿ      2008-12-2       ���ע��
// -------------------------------------------------
#endregion
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using MCS.Library.Core;

namespace MCS.Library.Passport
{
	/// <summary>
	/// Ʊ�ݼ��ܴ�����
	/// </summary>
	public class TicketEncryption : ITicketEncryption
	{
		private const int C_DATA_BLOCK_SIZE = 100;
		private const int C_ENCRYPT_BLOCK_SIZE = 128;

		/// <summary>
		/// 
		/// </summary>
		public TicketEncryption()
		{
		}

		#region IEncryptTicket ��Ա
		/// <summary>
		/// ����Ʊ������
		/// </summary>
		/// <param name="ticket">Ʊ����Ϣ</param>
		/// <param name="oParam">���ܲ���</param>
		/// <returns>���ܺ��ֽ�</returns>
		/// <remarks>
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\EncryptorTest.cs" region="TicketEncryptorTest" lang="cs" title="����Ticket����" />
		/// </remarks>
		public byte[] EncryptTicket(ITicket ticket, object oParam)
		{
			string strKeyInfo = (string)oParam;

			CspParameters cspParams = new CspParameters();
			cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
			cspParams.KeyContainerName = "TicketContainer";

			RSACryptoServiceProvider rsa = CreateRSAProvider(cspParams);

			rsa.FromXmlString(strKeyInfo);

			byte[] dataToEncrypt = Encoding.UTF8.GetBytes(ticket.SaveToXml().InnerXml);

			return RSAEncryptData(dataToEncrypt, rsa, false);
		}

		/// <summary>
		/// ���ܺ�Ʊ����Ϣ
		/// </summary>
		/// <param name="encryptedData">���ܵ��ֽ���Ϣ</param>
		/// <param name="oParam">���ܲ���</param>
		/// <returns>���ܺ��Ʊ����Ϣ</returns>
		/// <remarks>
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\EncryptorTest.cs" region="TicketEncryptorTest" lang="cs" title="�����ֽ�����" />
		/// </remarks>
		public ITicket DecryptTicket(byte[] encryptedData, object oParam)
		{
			string strKeyInfo = (string)oParam;

			CspParameters cspParams = new CspParameters();
			cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
			cspParams.KeyContainerName = "TicketContainer";

			RSACryptoServiceProvider rsa = CreateRSAProvider(cspParams);

			rsa.FromXmlString(strKeyInfo);

			byte[] data = RSADecryptData(encryptedData, rsa, false);

			string strXML = Encoding.UTF8.GetString(data);

			return new Ticket(strXML);
		}
		#endregion

		private static RSACryptoServiceProvider CreateRSAProvider(CspParameters cspParams)
		{
			try
			{
				return new RSACryptoServiceProvider(cspParams);
			}
			catch (System.Security.Cryptography.CryptographicException ex)
			{
				throw new SystemSupportException(ex.Message, ex);
			}
		}

		private byte[] RSAEncryptData(byte[] dataToEncrypt, RSACryptoServiceProvider rsa, bool doOAEPPadding)
		{
			Stream stream = new MemoryStream();

			BinaryWriter bw = new BinaryWriter(stream);

			int nBlocks = (dataToEncrypt.Length - 1) / TicketEncryption.C_DATA_BLOCK_SIZE + 1;

			byte[] srcData = new byte[TicketEncryption.C_DATA_BLOCK_SIZE];

			for (int i = 0; i < nBlocks - 1; i++)
			{
				Array.Copy(dataToEncrypt, TicketEncryption.C_DATA_BLOCK_SIZE * i, srcData, 0, TicketEncryption.C_DATA_BLOCK_SIZE);
				byte[] encData = rsa.Encrypt(srcData, false);

				bw.Write((Int32)encData.Length);
				bw.Write(encData);
			}

			int nRemain = dataToEncrypt.Length - (nBlocks - 1) * TicketEncryption.C_DATA_BLOCK_SIZE;

			if (nRemain > 0)
			{
				srcData = new byte[nRemain];
				Array.Copy(dataToEncrypt, TicketEncryption.C_DATA_BLOCK_SIZE * (nBlocks - 1), srcData, 0, nRemain);
				byte[] encData = rsa.Encrypt(srcData, false);

				bw.Write((Int32)encData.Length);
				bw.Write(encData);
			}

			bw.Write((Int32)(-1));

			byte[] resultData = new byte[stream.Length];

			stream.Position = 0;
			stream.Read(resultData, 0, (int)stream.Length);

			return resultData;
		}

		private byte[] RSADecryptData(byte[] dataToDecrypt, RSACryptoServiceProvider rsa, bool doOAEPPadding)
		{
			Stream streamIn = new MemoryStream(dataToDecrypt);
			Stream streamOut = new MemoryStream();

			BinaryReader br = new BinaryReader(streamIn);
			BinaryWriter bw = new BinaryWriter(streamOut);

			byte[] encData = new byte[TicketEncryption.C_ENCRYPT_BLOCK_SIZE];

			int size = br.ReadInt32();

			while (size > 0)
			{
				br.Read(encData, 0, size);
				bw.Write(rsa.Decrypt(encData, false));
				size = br.ReadInt32();
			}

			streamOut.Position = 0;

			byte[] resultData = new byte[streamOut.Length];
			streamOut.Read(resultData, 0, (int)streamOut.Length);

			return resultData;
		}
	}
}
