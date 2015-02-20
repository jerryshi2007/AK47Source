#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Passport
// FileName	：	StringEncryption.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          胡自强      2008-12-2       添加注释
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
	/// 字符串加密，解密处理类
	/// </summary>
	public class StringEncryption : IStringEncryption
	{
		private static byte[] DesKeys = { 136, 183, 142, 217, 175, 71, 90, 239 };
		private static byte[] DesIVs = { 227, 105, 5, 40, 162, 158, 143, 156 };

		/// <summary>
		/// 
		/// </summary>
		public StringEncryption()
		{
		}

		#region IEncryptString 成员
		/// <summary>
		/// 加密字符串
		/// </summary>
		/// <param name="strData">待加密字符串</param>
		/// <returns>加密后字节</returns>
		/// <remarks>
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\EncryptorTest.cs" region="StringEncryptorTest" lang="cs" title="字符串加密" />
		/// </remarks>
		public byte[] EncryptString(string strData)
		{
			return EncryptString(strData, GetDesObject());
		}

		/// <summary>
		/// 解密字节
		/// </summary>
		/// <param name="encryptedData">待解密字节</param>
		/// <returns>解密后的字符串</returns>
		/// <remarks>
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\EncryptorTest.cs" region="StringEncryptorTest" lang="cs" title="解密字节数组" />
		/// </remarks>
		public string DecryptString(byte[] encryptedData)
		{
			return DecryptString(encryptedData, GetDesObject());
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strData"></param>
		/// <param name="des"></param>
		/// <returns></returns>
		public byte[] EncryptString(string strData, DES des)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(des != null, "des");

			byte[] bytes = Encoding.UTF8.GetBytes(strData);

			MemoryStream mStream = new MemoryStream();

			try
			{
				CryptoStream encStream = new CryptoStream(mStream, des.CreateEncryptor(), CryptoStreamMode.Write);

				try
				{
					encStream.Write(bytes, 0, bytes.Length);
				}
				finally
				{
					encStream.Close();
				}

				return mStream.ToArray();
			}
			finally
			{
				mStream.Close();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="encryptedData"></param>
		/// <param name="des"></param>
		/// <returns></returns>
		public string DecryptString(byte[] encryptedData, DES des)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(des != null, "des");

			string strResult = string.Empty;

			MemoryStream mStream = new MemoryStream();

			try
			{
				mStream.Write(encryptedData, 0, encryptedData.Length);
				mStream.Seek(0, SeekOrigin.Begin);

				CryptoStream cryptoStream = new CryptoStream(mStream,
					des.CreateDecryptor(),
					CryptoStreamMode.Read);

				try
				{
					strResult = (new StreamReader(cryptoStream, Encoding.UTF8)).ReadToEnd();
				}
				finally
				{
					cryptoStream.Close();
				}
			}
			finally
			{
				mStream.Close();
			}

			return strResult;
		}

		private static DES GetDesObject()
		{
			DES des = new DESCryptoServiceProvider();

			des.Key = StringEncryption.DesKeys;
			des.IV = StringEncryption.DesIVs;

			return des;
		}
	}
}
