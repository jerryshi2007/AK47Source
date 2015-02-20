using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using MCS.Library.Security;

namespace MCS.Library.SOA.DataObjects
{
	public sealed class TicketDESEncryptor : DESEncryptorBase
	{
		private static byte[] desKey = { 136, 183, 142, 217, 175, 71, 90, 239 };
		private static byte[] desIV = { 227, 105, 5, 40, 162, 158, 143, 156 };

		internal TicketDESEncryptor()
		{
		}

		protected override DES GetDesObject()
		{
			DES des = new DESCryptoServiceProvider();

			des.IV = TicketDESEncryptor.desIV;
			des.Key = TicketDESEncryptor.desKey;

			return des;
		}
	}
}
