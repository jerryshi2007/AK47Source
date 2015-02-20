using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Workflow.Engine;

namespace MCS.Library.Workflow.OguObjects
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class WfOguUser : WfOguObject, IUser
    {
		private string logOnName = null;
		private string email = null;
		private string occupation = null;
		private UserRankType rank = UserRankType.Unspecified;
		//private bool? isSideline = null;

		internal WfOguUser() : base(SchemaType.Users)
		{
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public WfOguUser(string id)
            : base(id, SchemaType.Users)
        {
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="user"></param>
		public WfOguUser(IUser user)
			: base(user, SchemaType.Users)
		{
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected WfOguUser(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #region IUser ≥…‘±

        /// <summary>
        /// 
        /// </summary>
        public OguObjectCollection<IUser> AllRelativeUserInfo
        {
            get
            {
                return BaseUserObject.AllRelativeUserInfo;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public UserAttributesType Attributes
        {
            get
            {
                return BaseUserObject.Attributes;
            }
        }

        /// <summary>
        /// 
        /// </summary>
		public string Email
		{
			get
			{
				if (this.email == null)
					this.email = BaseUserObject.Email;

				return this.email;
			}
			set
			{
				this.email = value;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="includeSideline"></param>
        /// <returns></returns>
        public bool IsChildrenOf(IOrganization parent, bool includeSideline)
        {
            return BaseUserObject.IsChildrenOf(parent, includeSideline);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        public bool IsInGroups(params IGroup[] groups)
        {
            return BaseUserObject.IsInGroups(groups);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSideline
        {
            get
            {
                return BaseUserObject.IsSideline;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string LogOnName
        {
			get
			{
				if (this.logOnName == null)
					this.logOnName = BaseUserObject.LogOnName;

				return this.logOnName;
			}
			set
			{
				this.logOnName = value;
			}
        }

        /// <summary>
        /// 
        /// </summary>
        public OguObjectCollection<IGroup> MemberOf
        {
            get
            {
                return BaseUserObject.MemberOf;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Occupation
        {
			get
			{
				if (this.occupation == null)
					this.occupation = BaseUserObject.Occupation;

				return this.occupation;
			}
			set
			{
				this.occupation = value;
			}
        }

        /// <summary>
        /// 
        /// </summary>
        public UserPermissionCollection Permissions
        {
            get
            {
                return BaseUserObject.Permissions;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public UserRankType Rank
        {
			get
			{
				if (this.rank == UserRankType.Unspecified)
					this.rank = BaseUserObject.Rank;

				return this.rank;
			}
			set
			{
				this.rank = value;
			}
        }

		/// <summary>
		/// 
		/// </summary>
		public OguObjectCollection<IUser> Secretaries
		{
			get
			{
				return BaseUserObject.Secretaries;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public OguObjectCollection<IUser> SecretaryOf
		{
			get
			{
				return BaseUserObject.SecretaryOf;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        public UserRoleCollection Roles
        {
            get
            {
                return BaseUserObject.Roles;
            }
        }

        #endregion

        private IUser BaseUserObject
        {
            get
            {
                return (IUser)BaseObject;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WfOguUserCollection : WfKeyedCollectionBase<string, WfOguUser>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public void Add(WfOguUser user)
        {
            this.InnerAdd(user);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public WfOguUser this[int index]
        {
            get
            {
                return this.InnerGet(index);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public WfOguUser this[string userID]
        {
            get
            {
                return this.InnerGet(userID);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        protected override string GetKeyFromItem(WfOguUser user)
        {
            return user.ID;
        }
    }
}
