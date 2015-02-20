using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace MCS.Library.Accredit.OguAdmin
{
	/// <summary>
	/// 加密算法实现
	/// </summary>
	public class SecurityCalculate
	{
		/// <summary>
		/// 按照一定的加密算法生成转换后的加密数据（用于密码值计算）
		/// </summary>
		/// <param name="strPwdType">指定的加密算法类型</param>
		/// <param name="strPwd">指定要求被加密的数据</param>
		/// <returns>按照一定的加密算法生成转换后的加密数据（用于密码值计算）</returns>
		public static string PwdCalculate(string strPwdType, string strPwd)
		{
			string strResult = strPwd;

			MD5 md = new MD5CryptoServiceProvider();
			strResult = BitConverter.ToString(md.ComputeHash((new UnicodeEncoding()).GetBytes(strPwd)));

			return strResult;
		}
	}
}
