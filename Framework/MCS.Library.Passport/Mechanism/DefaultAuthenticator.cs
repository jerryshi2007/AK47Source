#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Passport
// FileName	：	DefaultAuthenticator.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          胡自强      2008-12-2       添加注释
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Passport.Properties;
using MCS.Library.Globalization;

namespace MCS.Library.Passport
{
	/// <summary>
	/// 用于单点登录服务认证实现
	/// </summary>
	public class DefaultAuthenticator : IAuthenticator, IAuthenticator2, IUserInfoUpdater, IUserIDConverter
	{
		private DefaultAuthenticator()
		{
		}

		#region IAuthenticator 成员
		/// <summary>
		/// 实现IAuthenticator接口方法，实现认证过程。
		/// </summary>
		/// <param name="strUserID">用户名</param>
		/// <param name="strPassword">密码</param>
		/// <returns></returns>
		public ISignInUserInfo Authenticate(string strUserID, string strPassword)
		{
			return Authenticate(strUserID, strPassword, null);
		}

		/// <summary>
		/// 代码未实现，返回false
		/// </summary>
		/// <param name="strUserID">用户标识。</param>
		/// <returns>false</returns>
		public bool CheckUserExists(string strUserID)
		{
			return CheckUserExists(strUserID, null);
		}

		/// <summary>
		/// 修改密码
		/// </summary>
		/// <param name="strUserID">用户标识。通常是登录名</param>
		/// <param name="oldPassword">旧密码</param>
		/// <param name="newPassword">新密码</param>
		public void ChangePassword(string strUserID, string oldPassword, string newPassword)
		{
			OguReaderServiceBroker.Instance.UpdateUserPwd(
				strUserID,
				(int)SearchOUIDType.LogOnName,
				oldPassword,
				newPassword,
				newPassword);
		}
		#endregion

		#region IAuthenticator2
		/// <summary>
		/// 认证
		/// </summary>
		/// <param name="strUserID"></param>
		/// <param name="strPassword"></param>
		/// <param name="context">一些额外的参数</param>
		/// <returns></returns>
		public ISignInUserInfo Authenticate(string strUserID, string strPassword, Dictionary<string, object> context)
		{
			LogOnIdentity loi = new LogOnIdentity(strUserID, strPassword);

			DefaultSignInUserInfo signInUserInfo = new DefaultSignInUserInfo();

			string authCode = context.GetValue("AuthenticationCode", string.Empty);

			//是否使用验证码认证
			if (authCode.IsNotEmpty())
			{
				AuthenticationCodeAdapter.Instance.CheckAuthenticationCode(authCode, strPassword);
			}
			else
			{
				if (AuthenticateUser(loi) == false)
				{
					IEnumerable<string> alternativeUserIDs = context.GetValue("AlternativeUserIDs", (IEnumerable<string>)null);

					if (AuthenticateAlternativeUserIDs(alternativeUserIDs, strPassword, context) == false)
						AuthenticateException.ThrowAuthenticateException(loi.LogOnNameWithoutDomain);
				}
			}

			signInUserInfo.UserID = loi.LogOnNameWithoutDomain;
			signInUserInfo.Domain = loi.Domain;

			return signInUserInfo;
		}

		/// <summary>
		/// 检查用户是否存在
		/// </summary>
		/// <param name="strUserID">用户标识，通常是登录名</param>
		/// <param name="context">一些额外的参数</param>
		/// <returns>用户是否存在</returns>
		public bool CheckUserExists(string strUserID, Dictionary<string, object> context)
		{
			bool bResult = false;

			return bResult;
		}
		#endregion IAuthenticator2

		#region IUserIDConverter
		/// <summary>
		/// 根据登录名得到用户不变的ID
		/// </summary>
		/// <param name="logonName"></param>
		/// <returns></returns>
		public string GetUserConsistentID(string logonName)
		{
			logonName.CheckStringIsNullOrEmpty("logonName");

			LogOnIdentity loi = new LogOnIdentity(logonName, string.Empty);

			OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, loi.LogOnNameWithoutDomain);

			string result = string.Empty;

			if (users.Count > 0)
				result = users[0].ID;

			return result;
		}

		/// <summary>
		/// 根据童虎的不变ID，得到用户的登录名
		/// </summary>
		/// <param name="consistentID"></param>
		/// <returns></returns>
		public string GetUserLogonName(string consistentID)
		{
			consistentID.CheckStringIsNullOrEmpty("consistentID");

			OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, consistentID);

			string result = string.Empty;

			if (users.Count > 0)
				result = users[0].LogOnName;

			return result;
		}
		#endregion

		private bool AuthenticateAlternativeUserIDs(IEnumerable<string> alternativeUserIDs, string password, Dictionary<string, object> context)
		{
			bool result = false;

			if (alternativeUserIDs != null)
			{
				foreach (string userID in alternativeUserIDs)
				{
					LogOnIdentity loi = new LogOnIdentity(userID, password);

					result = AuthenticateUser(loi);

					if (result)
						break;
				}
			}

			return result;
		}

		private bool AuthenticateUser(LogOnIdentity loi)
		{
			bool result = false;

			try
			{
				result = OguMechanismFactory.GetMechanism().AuthenticateUser(loi);
			}
			catch (System.Exception)
			{
			}

			return result;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class DefaultSignInUserInfo : ISignInUserInfo
	{
		private string userID = string.Empty;
		private string domain = string.Empty;
		private string originalUserID = string.Empty;
		private string consistentID = string.Empty;

		internal Dictionary<string, object> properties = new Dictionary<string, object>();

		/// <summary>
		/// 登录用户的ID
		/// </summary>
		public string UserID
		{
			get
			{
				return this.userID;
			}
			set
			{
				this.userID = value;
			}
		}

		/// <summary>
		/// 登录用户的域
		/// </summary>
		public string Domain
		{
			get
			{
				return this.domain;
			}
			set
			{
				this.domain = value;
			}
		}

		/// <summary>
		/// 原始的登录ID
		/// </summary>
		public string OriginalUserID
		{
			get
			{
				return this.originalUserID;
			}
			set
			{
				this.originalUserID = value;
			}
		}

		/// <summary>
		/// 属性集合
		/// </summary>
		public Dictionary<string, object> Properties
		{
			get
			{
				return properties;
			}
		}
	}
}
