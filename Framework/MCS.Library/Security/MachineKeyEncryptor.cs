using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using MCS.Library.Core;

namespace MCS.Library.Security
{
	/// <summary>
	/// 使用Machine Key加密和解密字符串
	/// </summary>
	public static class MachineKeyEncryptor
	{
		/// <summary>
		/// 加密字符串
		/// </summary>
		/// <param name="strData"></param>
		/// <returns></returns>
		public static string Encrypt(this string strData)
		{
			string result = strData;

			if (strData.IsNotEmpty())
			{
				byte[] data = Encoding.UTF8.GetBytes(strData);

				result = Encode(data);
			}

			return result;
		}

		/// <summary>
		/// 解密字符串
		/// </summary>
		/// <param name="encryptedData"></param>
		/// <returns></returns>
		public static string Decrypt(this string encryptedData)
		{
			string result = encryptedData;

			if (encryptedData.IsNotEmpty())
			{
				byte[] data = Decode(encryptedData);

				result = Encoding.UTF8.GetString(data);
			}

			return result;
		}

		private static string Encode(byte[] data)
		{
#if GTNet40
			return Convert.ToBase64String(MachineKey.Protect(data));
#else
			return MachineKey.Encode(data, MachineKeyProtection.Encryption);
#endif
		}

		private static byte[] Decode(string encryptedData)
		{
#if GTNet40
			byte[] data = Convert.FromBase64String(encryptedData);
			return MachineKey.Unprotect(data);
#else
			return MachineKey.Decode(encryptedData, MachineKeyProtection.Encryption);
#endif
		}
	}
}
		