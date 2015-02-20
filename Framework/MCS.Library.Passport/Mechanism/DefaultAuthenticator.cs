#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	DefaultAuthenticator.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          ����ǿ      2008-12-2       ���ע��
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
	/// ���ڵ����¼������֤ʵ��
	/// </summary>
	public class DefaultAuthenticator : IAuthenticator, IAuthenticator2, IUserInfoUpdater, IUserIDConverter
	{
		private DefaultAuthenticator()
		{
		}

		#region IAuthenticator ��Ա
		/// <summary>
		/// ʵ��IAuthenticator�ӿڷ�����ʵ����֤���̡�
		/// </summary>
		/// <param name="strUserID">�û���</param>
		/// <param name="strPassword">����</param>
		/// <returns></returns>
		public ISignInUserInfo Authenticate(string strUserID, string strPassword)
		{
			return Authenticate(strUserID, strPassword, null);
		}

		/// <summary>
		/// ����δʵ�֣�����false
		/// </summary>
		/// <param name="strUserID">�û���ʶ��</param>
		/// <returns>false</returns>
		public bool CheckUserExists(string strUserID)
		{
			return CheckUserExists(strUserID, null);
		}

		/// <summary>
		/// �޸�����
		/// </summary>
		/// <param name="strUserID">�û���ʶ��ͨ���ǵ�¼��</param>
		/// <param name="oldPassword">������</param>
		/// <param name="newPassword">������</param>
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
		/// ��֤
		/// </summary>
		/// <param name="strUserID"></param>
		/// <param name="strPassword"></param>
		/// <param name="context">һЩ����Ĳ���</param>
		/// <returns></returns>
		public ISignInUserInfo Authenticate(string strUserID, string strPassword, Dictionary<string, object> context)
		{
			LogOnIdentity loi = new LogOnIdentity(strUserID, strPassword);

			DefaultSignInUserInfo signInUserInfo = new DefaultSignInUserInfo();

			string authCode = context.GetValue("AuthenticationCode", string.Empty);

			//�Ƿ�ʹ����֤����֤
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
		/// ����û��Ƿ����
		/// </summary>
		/// <param name="strUserID">�û���ʶ��ͨ���ǵ�¼��</param>
		/// <param name="context">һЩ����Ĳ���</param>
		/// <returns>�û��Ƿ����</returns>
		public bool CheckUserExists(string strUserID, Dictionary<string, object> context)
		{
			bool bResult = false;

			return bResult;
		}
		#endregion IAuthenticator2

		#region IUserIDConverter
		/// <summary>
		/// ���ݵ�¼���õ��û������ID
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
		/// ����ͯ���Ĳ���ID���õ��û��ĵ�¼��
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
		/// ��¼�û���ID
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
		/// ��¼�û�����
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
		/// ԭʼ�ĵ�¼ID
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
		/// ���Լ���
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
