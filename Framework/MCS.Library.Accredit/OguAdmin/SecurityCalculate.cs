using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace MCS.Library.Accredit.OguAdmin
{
	/// <summary>
	/// �����㷨ʵ��
	/// </summary>
	public class SecurityCalculate
	{
		/// <summary>
		/// ����һ���ļ����㷨����ת����ļ������ݣ���������ֵ���㣩
		/// </summary>
		/// <param name="strPwdType">ָ���ļ����㷨����</param>
		/// <param name="strPwd">ָ��Ҫ�󱻼��ܵ�����</param>
		/// <returns>����һ���ļ����㷨����ת����ļ������ݣ���������ֵ���㣩</returns>
		public static string PwdCalculate(string strPwdType, string strPwd)
		{
			string strResult = strPwd;

			MD5 md = new MD5CryptoServiceProvider();
			strResult = BitConverter.ToString(md.ComputeHash((new UnicodeEncoding()).GetBytes(strPwd)));

			return strResult;
		}
	}
}
