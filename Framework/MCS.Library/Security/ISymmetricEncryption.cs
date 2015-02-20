using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Security
{
	/// <summary>
	/// 对称加密算法的实现
	/// </summary>
	public interface ISymmetricEncryption
	{
		/// <summary>
		/// 加密字符串
		/// </summary>
		/// <param name="strData">字符串数据</param>
		/// <returns>加密后的二进制流</returns>
		byte[] EncryptString(string strData);

		/// <summary>
		/// 解密字符串
		/// </summary>
		/// <param name="encryptedData">加密过的数据</param>
		/// <returns>解密后的字符串</returns>
		string DecryptString(byte[] encryptedData);
	}
}
