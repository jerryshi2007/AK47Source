#region
// -------------------------------------------------
// Assembly	：	HB.DataObjects
// FileName	：	OguUser.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    李苗	    20070628		创建
// -------------------------------------------------
#endregion

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[XElementSerializable]
	public class OguUser : OguBase, IUser
	{
		private string logOnName = null;
		private string email = null;
		private string occupation = null;
		private UserRankType rank = UserRankType.Unspecified;
		private bool? isSideline = null;

		public OguUser()
			: base(SchemaType.Users)
		{
		}

		public OguUser(IUser user)
			: base(user, SchemaType.Users)
		{
			if ((user is OguUser) == false)
			{
				this.logOnName = user.LogOnName;
			}
		}

		public OguUser(string id)
			: base(id, SchemaType.Users)
		{
		}

		public OguUser(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.logOnName = info.GetString("LogOnName");
		}

		/// <summary>
		/// 登陆名称
		/// </summary>
		public string LogOnName
		{
			get
			{
				if (this.logOnName == null && Ogu != null)
					this.logOnName = BaseUser.LogOnName;

				return this.logOnName;
			}
			set
			{
				this.logOnName = value;
			}
		}

		/// <summary>
		/// 机构类型
		/// </summary>
		public override SchemaType ObjectType
		{
			get
			{
				return SchemaType.Users;
			}
		}

		/// <summary>
		/// 邮件
		/// </summary>
		public string Email
		{
			get
			{
				if (this.email == null && Ogu != null)
					this.email = BaseUser.Email;

				return this.email;
			}
			set
			{
				this.email = value;
			}
		}

		/// <summary>
		/// 职位
		/// </summary>
		public string Occupation
		{
			get
			{
				if (this.occupation == null && Ogu != null)
					this.occupation = BaseUser.Occupation;

				return this.occupation;
			}
			set
			{
				this.occupation = value;
			}
		}

		/// <summary>
		/// 级别
		/// </summary>
		public UserRankType Rank
		{
			get
			{
				if (this.rank == UserRankType.Unspecified && Ogu != null)
					this.rank = BaseUser.Rank;

				return this.rank;
			}
			set
			{
				this.rank = value;
			}
		}

		/// <summary>
		/// 用户的一些特殊属性（党组成员1、署管干部2、交流干部4、借调干部8）
		/// </summary>
		public UserAttributesType Attributes
		{
			get
			{
				UserAttributesType result = UserAttributesType.Unspecified;

				if (Ogu != null)
					result = BaseUser.Attributes;

				return result;
			}
		}

		/// <summary>
		/// 是否兼职
		/// </summary>
		public bool IsSideline
		{
			get
			{
				if (this.isSideline == null && Ogu != null)
					this.isSideline = BaseUser.IsSideline;

				return (bool)this.isSideline;
			}
			set
			{
				this.isSideline = value;
			}
		}

		/// <summary>
		/// 用户属于那些组
		/// </summary>
		public OguObjectCollection<IGroup> MemberOf
		{
			get
			{
				OguObjectCollection<IGroup> result = null;

				if (Ogu != null)
					result = BaseUser.MemberOf;
				else
					result = new OguObjectCollection<IGroup>();

				return result;
			}
		}

		/// <summary>
		/// 得到某用户的所有相关用户信息，包括主职和兼职的
		/// </summary>
		public OguObjectCollection<IUser> AllRelativeUserInfo
		{
			get
			{
				OguObjectCollection<IUser> result = null;

				if (Ogu != null)
					result = BaseUser.AllRelativeUserInfo;
				else
					result = new OguObjectCollection<IUser>();

				return result;
			}
		}

		public UserRoleCollection Roles
		{
			get
			{
				UserRoleCollection result = null;

				if (Ogu != null)
					result = BaseUser.Roles;
				else
					result = new UserRoleCollection(this);

				return result;
			}
		}

		public UserPermissionCollection Permissions
		{
			get
			{
				UserPermissionCollection result = null;

				if (Ogu != null)
					result = BaseUser.Permissions;
				else
					result = new UserPermissionCollection(this);

				return result;
			}
		}

		/// <summary>
		/// 判断当前用户是否在这些组中
		/// </summary>
		/// <param name="groups"></param>
		/// <returns></returns>
		public bool IsInGroups(params IGroup[] groups)
		{

			bool result = false;

			if (Ogu != null)
				result = BaseUser.IsInGroups(groups);

			return result;
		}

		/// <summary>
		/// 判断当前用户是否在这个机构中
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="includeSideline"></param>
		/// <returns></returns>
		public bool IsChildrenOf(IOrganization parent, bool includeSideline)
		{
			bool result = false;

			if (Ogu != null)
				result = BaseUser.IsChildrenOf(parent, includeSideline);

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		public OguObjectCollection<IUser> Secretaries
		{
			get
			{
				OguObjectCollection<IUser> result = null;

				if (Ogu != null)
					result = BaseUser.Secretaries;
				else
					result = new OguObjectCollection<IUser>();

				return result;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public OguObjectCollection<IUser> SecretaryOf
		{
			get
			{
				OguObjectCollection<IUser> result = null;

				if (Ogu != null)
					result = BaseUser.SecretaryOf;
				else
					result = new OguObjectCollection<IUser>();

				return result;
			}
		}

		#region 基础类型
		private IUser BaseUser
		{
			get
			{
				return (IUser)Ogu;
			}
		}
		#endregion 基础类型

		#region ISerializable 成员

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("LogOnName", this.logOnName);
		}

		#endregion ISerializable 成员
	}
}
