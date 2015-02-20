#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	DeluxeIdentity.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          ����ǿ      2008-12-2       ���ע��
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Security.Principal;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Passport;
using MCS.Library.OGUPermission;
using MCS.Library.Passport.Properties;

namespace MCS.Library.Principal
{
	/// <summary>
	/// ͨ��DeluxeWorks��֤���ƣ����������û���ݶ���ʵ����ϵͳ��IIdentity�ӿڡ�
	/// </summary>
    public class DeluxeIdentity : ITicketIdentity
	{
		private IUser user = null;
		private IUser realUser = null;
		private ITicket ticket = null;

		/// <summary>
		/// �û���ݶ���
		/// </summary>
		public static DeluxeIdentity Current
		{
			get
			{
				return (DeluxeIdentity)DeluxePrincipal.Current.Identity;
			}
		}

		/// <summary>
		/// ȡ�õ�ǰ�û���Ϣ
		/// </summary>
		public static IUser CurrentUser
		{
			get
			{
				return Current.User;
			}
		}

		/// <summary>
		/// ȡ�õ�ǰ��ʵ�û���Ϣ
		/// </summary>
		public static IUser CurrentRealUser
		{
			get
			{
				return Current.RealUser;
			}
		}

		#region ���췽��
		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="logonUser">IUser</param>
		public DeluxeIdentity(IUser logonUser)
		{
			this.user = logonUser;
			SetImpersonateInfo(logonUser);
		}

		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="logonUser">IUser</param>
		/// <param name="ticket">Ʊ��</param>
		public DeluxeIdentity(IUser logonUser, ITicket ticket)
		{
			this.user = logonUser;
			this.ticket = ticket;

			SetImpersonateInfo(logonUser);
		}

		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="logonName">�û���</param>
		/// <param name="ticket">Ʊ��</param>
		public DeluxeIdentity(string logonName, ITicket ticket)
		{
			this.user = GetUserInfoFromLogonName(logonName);
			this.ticket = ticket;

			SetImpersonateInfo(this.user);
		}

		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="logonName">�û���</param>
		public DeluxeIdentity(string logonName)
		{
			this.user = GetUserInfoFromLogonName(logonName);

			SetImpersonateInfo(this.user);
		}
		#endregion ���췽��

		#region ��������
		/// <summary>
		/// �û���Ʊ��
		/// </summary>
		public ITicket Ticket
		{
			get
			{
				return this.ticket;
			}
		}

		/// <summary>
		/// �û����ԣ�����ǰ����û�����ôUser��RealUser�ǲ�һ����
		/// </summary>
		public IUser User
		{
			get
			{
				return this.user;
			}
		}

		/// <summary>
		/// ��ʵ���û���Ϣ
		/// </summary>
		public IUser RealUser
		{
			get
			{
				return this.realUser;
			}
		}

		/// <summary>
		/// ��ǰ����Ƿ��ǰ��ݹ���
		/// </summary>
		public bool IsImpersonated
		{
			get
			{
				return string.Compare(this.user.ID, this.realUser.ID, true) != 0;
			}
		}

		#endregion ��������

		#region IIdentity ��Ա
		/// <summary>
		/// ��֤����
		/// </summary>
		public string AuthenticationType
		{
			get
			{
				return "DeluxeWorksPassport";
			}
		}
		/// <summary>
		/// �Ƿ�ͨ����֤
		/// </summary>
		public bool IsAuthenticated
		{
			get
			{
				return this.user != null;
			}
		}
		/// <summary>
		/// �û���
		/// </summary>
		public string Name
		{
			get
			{
				string result = string.Empty;

				if (this.user != null)
					result = this.user.LogOnName;

				return result;
			}
		}
		#endregion

		#region ˽�г�Ա
		private IUser GetUserInfoFromLogonName(string logonName)
		{
			OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, logonName);

			ExceptionHelper.FalseThrow<AuthenticateException>(users.Count > 0, Resource.CanNotFindUser, logonName);

			IUser result = users[0];

			foreach (IUser user in users)
			{
				if (user.IsSideline == false)
				{
					result = user;
					break;
				}
			}

			return result;
		}

		private IUser GetUserInfoFromID(string userID)
		{
			OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, userID);

			ExceptionHelper.FalseThrow<AuthenticateException>(users.Count > 0, Resource.CanNotFindUser, userID);

			return users[0];
		}

		private void SetImpersonateInfo(IUser originalUser)
		{
			this.realUser = originalUser;

			IUserImpersonatingInfoLoader loader = AuthenticateDirSettings.GetConfig().ImpersonatingInfoLoader;

			if (loader != null)
			{
				UserImpersonatingInfo info = loader.GetUserImpersonatingInfo(originalUser.ID);

				if (info != null && string.IsNullOrEmpty(info.ImpersonatingUserID) == false)
					this.user = GetUserInfoFromID(info.ImpersonatingUserID);
			}
		}

		#endregion ˽�г�Ա
	}
}
