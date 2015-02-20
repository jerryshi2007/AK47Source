using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using MCS.Library.Core;

namespace MCS.Library.Security
{
	/// <summary>
	/// 和安全有关的扩展方法
	/// </summary>
	public static class SecurityExtension
	{
		/// <summary>
		/// 如果字符串是DES的key，那么就生成此DES的密钥
		/// </summary>
		/// <param name="desKey"></param>
		/// <returns></returns>
		public static DES ToDES(this string desKey)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(desKey, "desKey");

			string[] keyParts = desKey.Split(' ');

			ExceptionHelper.FalseThrow(keyParts.Length == 2, "EncryptionKey格式不合法，应该分为Key和IV两部分，由空格分开");
			ExceptionHelper.FalseThrow(keyParts[0].Length == 16, "EncryptionKey格式不合法，Key长度应该为16个字符(64位二进制数)");
			ExceptionHelper.FalseThrow(keyParts[1].Length == 16, "EncryptionKey格式不合法，IV长度应该为16个字符(64位二进制数)");

			DES des = DES.Create();

			des.Key = keyParts[0].ToBase16Bytes();
			des.IV = keyParts[1].ToBase16Bytes();

			return des;
		}
	}
}
