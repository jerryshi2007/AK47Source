using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MCS.Library.Office.OpenXml.Excel.Encryption
{
	/// <summary>
	/// 密报
	/// </summary>
	internal class EncryptionHeader
	{
		internal Flags Flags;
		internal int SizeExtra;             //MUST be 0x00000000.
		internal AlgorithmID AlgID;         //MUST be 0x0000660E (AES-128), 0x0000660F (AES-192), or 0x00006610 (AES-256).
		internal AlgorithmHashID AlgIDHash; //MUST be 0x00008004 (SHA-1).
		internal int KeySize;               //MUST be 0x00000080 (AES-128), 0x000000C0 (AES-192), or 0x00000100 (AES-256).
		internal ProviderType ProviderType; //SHOULD<10> be 0x00000018 (AES).
		internal int Reserved1;             //Undefined and MUST be ignored.
		internal int Reserved2;             //MUST be 0x00000000 and MUST be ignored.
		internal string CSPName;            //SHOULD<11> be set to either "Microsoft Enhanced RSA and AES Cryptographic Provider" or "Microsoft Enhanced RSA and AES Cryptographic Provider (Prototype)" as a null-terminated Unicode string.

		internal byte[] WriteBinary()
		{
			MemoryStream ms = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(ms);

			bw.Write((int)Flags);
			bw.Write(SizeExtra);
			bw.Write((int)AlgID);
			bw.Write((int)AlgIDHash);
			bw.Write((int)KeySize);
			bw.Write((int)ProviderType);
			bw.Write(Reserved1);
			bw.Write(Reserved2);
			bw.Write(Encoding.Unicode.GetBytes(CSPName));

			bw.Flush();
			return ms.ToArray();
		}
	}
}
