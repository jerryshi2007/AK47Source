using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MCS.Library.Core;

namespace MCS.Library.OGUPermission
{
	#region UserImpl
	/// <summary>
	/// 用户对象的实现类
	/// </summary>
	[Serializable]
	public class OguUserImpl : OguBaseImpl, IUser, MCS.Library.OGUPermission.IUserPropertyAccessible
	{
		private string logOnName = string.Empty;
		private string email = string.Empty;
		private string occupation = string.Empty;
		private UserRankType rank;
		private UserAttributesType attributes;
		private OguObjectCollection<IGroup> memberOf = null;
		private bool isSideline = false;
		private OguObjectCollection<IUser> secretaries = null;
		private OguObjectCollection<IUser> secretaryOf = null;
		private OguObjectCollection<IUser> allRelativeUserInfo = null;
		private UserRoleCollection roles = null;
		private UserPermissionCollection permissions = null;

		#region SyncObjects
		private object allRelativeUserInfoSycObj = new object();
		private object memberOfSyncObj = new object();
		private object secretariesSyncObj = new object();
		private object secretaryOfSyncObj = new object();
		private object rolesSyncObj = new object();
		private object permissionsSyncObj = new object();
		#endregion

		/// <summary>
		/// 构造方法
		/// </summary>
		public OguUserImpl()
		{
		}

		#region IUser 成员

		/// <summary>
		/// 登录名
		/// </summary>
		public string LogOnName
		{
			get { return this.logOnName; }
			internal set { this.logOnName = value; }
		}

		/// <summary>
		/// 邮件地址
		/// </summary>
		public string Email
		{
			get { return this.email; }
			internal set { this.email = value; }
		}

		/// <summary>
		/// 职务
		/// </summary>
		public string Occupation
		{
			get { return this.occupation; }
			internal set { this.occupation = value; }
		}

		/// <summary>
		/// 用户的级别
		/// </summary>
		public UserRankType Rank
		{
			get { return this.rank; }
			internal set { this.rank = value; }
		}

		/// <summary>
		/// 用户属性
		/// </summary>
		public UserAttributesType Attributes
		{
			get { return this.attributes; }
			internal set { this.attributes = value; }
		}

		/// <summary>
		/// 属于哪些组
		/// </summary>
		public OguObjectCollection<IGroup> MemberOf
		{
			get
			{
				if (this.memberOf == null)
				{
					lock (this.memberOfSyncObj)
					{
						if (this.memberOf == null)
							this.memberOf = OguPermissionSettings.GetConfig().OguObjectImpls.GetGroupsOfUser(this);
					}
				}

				return this.memberOf;
			}
		}

		/// <summary>
		/// 是否属于某些组
		/// </summary>
		/// <param name="groups"></param>
		/// <returns></returns>
		public bool IsInGroups(params IGroup[] groups)
		{
			if (groups == null)
				throw new ArgumentNullException("groups");

			bool bBelongTo = false;

			OguObjectCollection<IGroup> mof = MemberOf;

			for (int i = 0; i < groups.Length; i++)
			{
				foreach (IGroup group in mof)
				{
					if (group.ID == groups[i].ID)
					{
						bBelongTo = true;
						break;
					}
				}
			}

			return bBelongTo;
		}

		/// <summary>
		/// 是否是兼职的用户信息
		/// </summary>
		public bool IsSideline
		{
			get { return this.isSideline; }
			internal set { this.isSideline = value; }
		}

		/// <summary>
		/// 该用户的秘书
		/// </summary>
		public OguObjectCollection<IUser> Secretaries
		{
			get
			{
				if (this.secretaries == null)
				{
					lock (this.secretariesSyncObj)
					{
						if (this.secretaries == null)
							this.secretaries = OguPermissionSettings.GetConfig().OguObjectImpls.GetSecretaries(this);
					}
				}

				return this.secretaries;
			}
		}

		/// <summary>
		/// 是谁的秘书
		/// </summary>
		public OguObjectCollection<IUser> SecretaryOf
		{
			get
			{
				if (this.secretaryOf == null)
				{
					lock (this.secretaryOfSyncObj)
					{
						if (this.secretaryOf == null)
							this.secretaryOf = OguPermissionSettings.GetConfig().OguObjectImpls.GetSecretaryOf(this);
					}
				}

				return this.secretaryOf;
			}
		}

		/// <summary>
		/// 同一用户ID的所有相关信息（所有主职和兼职信息）
		/// </summary>
		public OguObjectCollection<IUser> AllRelativeUserInfo
		{
			get
			{
				if (this.allRelativeUserInfo == null)
				{
					lock (this.allRelativeUserInfoSycObj)
					{
						if (this.allRelativeUserInfo == null)
							this.allRelativeUserInfo = OguPermissionSettings.GetConfig().OguObjectImpls.GetAllRelativeUserInfo(this);
					}
				}

				return this.allRelativeUserInfo;
			}
		}

		/// <summary>
		/// 是否是某个机构的子成员
		/// </summary>
		/// <param name="parent">机构</param>
		/// <param name="includeSideline">是否判断兼职信息</param>
		/// <returns>是否是某个机构的子成员</returns>
		public bool IsChildrenOf(IOrganization parent, bool includeSideline)
		{
			bool result = false;

			ExceptionHelper.FalseThrow<ArgumentNullException>(parent != null, "parent");

			if (includeSideline == false)
				result = IsChildrenOf(parent);
			else
			{
				foreach (IUser user in AllRelativeUserInfo)
				{
					if (user.IsChildrenOf(parent))
					{
						result = true;
						break;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// 用户的角色
		/// </summary>
		public UserRoleCollection Roles
		{
			get
			{
				if (this.roles == null)
				{
					lock (this.rolesSyncObj)
					{
						if (this.roles == null)
							this.roles = new UserRoleCollection(this);
					}
				}

				return this.roles;
			}
		}

		/// <summary>
		/// 用户的功能
		/// </summary>
		public UserPermissionCollection Permissions
		{
			get
			{
				if (this.permissions == null)
				{
					lock (this.permissionsSyncObj)
					{
						if (this.permissions == null)
							this.permissions = new UserPermissionCollection(this);
					}
				}

				return this.permissions;
			}
		}

		#endregion

		/// <summary>
		/// 初始化属性
		/// </summary>
		/// <param name="row"></param>
		public override void InitProperties(DataRow row)
		{
			base.InitProperties(row);

			this.logOnName = Common.GetDataRowTextValue(row, "LOGON_NAME");
			this.email = Common.GetDataRowTextValue(row, "E_MAIL");
			this.occupation = Common.GetDataRowTextValue(row, "RANK_NAME");

			this.rank = Common.ConvertUserRankCode(Common.GetDataRowTextValue(row, "RANK_CODE"));

			ObjectType = SchemaType.Users;
			this.attributes = Common.ConvertUserAttribute(Common.GetDataRowValue(row, "ATTRIBUTES", 0));

			this.isSideline = Common.GetDataRowValue(row, "SIDELINE", 0) == 1;
		}

		#region 显示实现接口
		OguObjectCollection<IUser> IUserPropertyAccessible.AllRelativeUserInfo
		{
			get
			{
				return this.AllRelativeUserInfo;
			}
		}

		UserAttributesType IUserPropertyAccessible.Attributes
		{
			get
			{
				return this.Attributes;
			}
			set
			{
				this.Attributes = value;
			}
		}

		string IUserPropertyAccessible.Email
		{
			get
			{
				return this.Email;
			}
			set
			{
				this.Email = value;
			}
		}

		bool IUserPropertyAccessible.IsSideline
		{
			get
			{
				return this.IsSideline;
			}
			set
			{
				this.IsSideline = value;
			}
		}

		string IUserPropertyAccessible.LogOnName
		{
			get
			{
				return this.LogOnName;
			}
			set
			{
				this.LogOnName = value;
			}
		}

		OguObjectCollection<IGroup> IUserPropertyAccessible.MemberOf
		{
			get
			{
				return this.MemberOf;
			}
		
		}

		string IUserPropertyAccessible.Occupation
		{
			get
			{
				return this.Occupation;
			}
			set
			{
				this.Occupation = value;
			}
		}

		UserPermissionCollection IUserPropertyAccessible.Permissions
		{
			get
			{
				return this.Permissions;
			}
		}

		UserRankType IUserPropertyAccessible.Rank
		{
			get
			{
				return this.Rank;
			}
			set
			{
				this.Rank = value;
			}
		}

		UserRoleCollection IUserPropertyAccessible.Roles
		{
			get
			{
				return this.Roles;
			}
		}

		OguObjectCollection<IUser> IUserPropertyAccessible.Secretaries
		{
			get
			{
				return this.Secretaries;
			}
		}

		OguObjectCollection<IUser> IUserPropertyAccessible.SecretaryOf
		{
			get
			{
				return this.SecretaryOf;
			}
		}
		#endregion
	}

	#endregion OguUserImpl
}
