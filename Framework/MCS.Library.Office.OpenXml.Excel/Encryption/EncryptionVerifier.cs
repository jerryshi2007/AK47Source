using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MCS.Library.Office.OpenXml.Excel.Encryption
{
	/// <summary>
	/// 内EncryptionInfo流的加密验证
	/// </summary>
	internal class EncryptionVerifier
	{
		internal uint SaltSize;              // An unsigned integer that specifies the size of the Salt field. It MUST be 0x00000010.
		internal byte[] Salt;                //(16 bytes): An array of bytes that specifies the salt value used during password hash generation. It MUST NOT be the same data used for the verifier stored encrypted in the EncryptedVerifier field.
		internal byte[] EncryptedVerifier;   //(16 bytes): MUST be the randomly generated Verifier value encrypted using the algorithm chosen by the implementation.
		internal uint VerifierHashSize;      //(4 bytes): An unsigned integer that specifies the number of bytes needed to contain the hash of the data used to generate the EncryptedVerifier field.
		internal byte[] EncryptedVerifierHash; //(variable): An array of bytes that contains the encrypted form of the hash of the randomly generated Verifier value. The length of the array MUST be the size of the encryption block size multiplied by the number of blocks needed to encrypt the hash of the Verifier. If the encryption algorithm is RC4, the length MUST be 20 bytes. If the encryption algorithm is AES, the length MUST be 32 bytes.
		
		internal byte[] WriteBinary()
		{
			MemoryStream ms = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(ms);

			bw.Write(SaltSize);
			bw.Write(Salt);
			bw.Write(EncryptedVerifier);
			bw.Write(0x14);                 //Sha1 is 20 bytes  (Encrypted is 32)
			bw.Write(EncryptedVerifierHash);

			bw.Flush();
			return ms.ToArray();
		}
	}
}
