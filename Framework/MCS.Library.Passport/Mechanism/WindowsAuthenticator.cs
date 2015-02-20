using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Runtime.InteropServices;

namespace MCS.Library.Passport
{
	/// <summary>
	/// 调用Windows的认证
	/// </summary>
	public class WindowsAuthenticator : IAuthenticator
	{
		[DllImport("advapi32.dll")]
		//映射函数LogonUser
		private static extern bool LogonUser( string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

		#region IAuthenticator Members

		/// <summary>
		/// 认证
		/// </summary>
		/// <param name="strUserID"></param>
		/// <param name="strPassword"></param>
		/// <returns></returns>
		public ISignInUserInfo Authenticate(string strUserID, string strPassword)
		{
			LogOnIdentity loi = new LogOnIdentity(strUserID, strPassword);

			DefaultSignInUserInfo signInUserInfo = new DefaultSignInUserInfo();

			if (AuthenticateUser(loi) == false)
				AuthenticateException.ThrowAuthenticateException(loi.LogOnNameWithoutDomain);

			signInUserInfo.UserID = loi.LogOnNameWithoutDomain;
			signInUserInfo.Domain = loi.Domain;

			return signInUserInfo;
		}

		/// <summary>
		/// 代码未实现，返回false;
		/// </summary>
		/// <param name="strUserID"></param>
		/// <returns></returns>
		public bool CheckUserExists(string strUserID)
		{
			return false;
		}
		#endregion

		private bool AuthenticateUser(LogOnIdentity loi)
		{
			const int LOGON32_PROVIDER_DEFAULT = 0; //使用默认的Windows 2000/NT NTLM验证方式
			const int LOGON32_LOGON_NETWORK = 3;
			IntPtr tokenHandle = new IntPtr(0);
			tokenHandle = IntPtr.Zero;

			return LogonUser(loi.LogOnNameWithoutDomain, loi.Domain, loi.Password, LOGON32_LOGON_NETWORK, LOGON32_PROVIDER_DEFAULT, ref tokenHandle);
		}
	}
}
