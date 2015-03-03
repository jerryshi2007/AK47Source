using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Passport;
using MCS.Library.Passport.Properties;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace MCS.Library.Principal
{
    /// <summary>
    /// 通过DeluxeWorks认证机制，所产生的用户身份对象，实现了系统的IIdentity接口。
    /// </summary>
    public class DeluxeIdentity : ITicketIdentity, ITicketTokenContainer<IUser>
    {
        private IUser user = null;
        private IUser realUser = null;
        private ITicket ticket = null;

        #region 构造方法
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logonUser">IUser</param>
        public DeluxeIdentity(IUser logonUser)
        {
            this.Init(logonUser, null);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logonUser">IUser</param>
        /// <param name="ticket">票据</param>
        public DeluxeIdentity(IUser logonUser, ITicket ticket)
        {
            this.Init(logonUser, ticket);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logonName">用户名</param>
        /// <param name="ticket">票据</param>
        public DeluxeIdentity(string logonName, ITicket ticket)
        {
            this.Init(GetUserInfoFromLogonName(logonName), ticket);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logonName">用户名</param>
        public DeluxeIdentity(string logonName)
        {
            this.Init(GetUserInfoFromLogonName(logonName), null);
        }

        /// <summary>
        /// 从Ticket token构造Identity
        /// </summary>
        /// <param name="tokenContainer"></param>
        /// <param name="ticket"></param>
        public DeluxeIdentity(GenericTicketTokenContainer tokenContainer, ITicket ticket)
        {
            tokenContainer.NullCheck("tokenContainer");
            tokenContainer.User.NullCheck("tokenContainer.User");
            tokenContainer.RealUser.NullCheck("tokenContainer.RealUser");

            OguObjectCollection<IUser> users = GetUsersInfoFromIDs(tokenContainer.User.ID, tokenContainer.RealUser.ID);

            this.user = users.Find(u => string.Compare(u.ID, tokenContainer.User.ID, true) == 0);
            this.realUser = users.Find(u => string.Compare(u.ID, tokenContainer.RealUser.ID, true) == 0);

            this.user.NullCheck("user");
            this.realUser.NullCheck("realUser");

            this.ticket = ticket;
        }

        #endregion 构造方法

        /// <summary>
        /// 用户身份对象
        /// </summary>
        public static DeluxeIdentity Current
        {
            get
            {
                return (DeluxeIdentity)DeluxePrincipal.Current.Identity;
            }
        }

        /// <summary>
        /// 取得当前用户信息
        /// </summary>
        public static IUser CurrentUser
        {
            get
            {
                return Current.User;
            }
        }

        /// <summary>
        /// 取得当前真实用户信息
        /// </summary>
        public static IUser CurrentRealUser
        {
            get
            {
                return Current.RealUser;
            }
        }

        #region 公共属性
        /// <summary>
        /// 用户的票据
        /// </summary>
        public ITicket Ticket
        {
            get
            {
                return this.ticket;
            }
        }

        /// <summary>
        /// 用户属性，如果是扮演用户，那么User和RealUser是不一样的
        /// </summary>
        public IUser User
        {
            get
            {
                return this.user;
            }
        }

        /// <summary>
        /// 真实的用户信息
        /// </summary>
        public IUser RealUser
        {
            get
            {
                return this.realUser;
            }
        }

        /// <summary>
        /// 当前身份是否是扮演过的
        /// </summary>
        public bool IsImpersonated
        {
            get
            {
                return string.Compare(this.user.ID, this.realUser.ID, true) != 0;
            }
        }

        #endregion 公共属性

        #region IIdentity 成员
        /// <summary>
        /// 认证类型
        /// </summary>
        public string AuthenticationType
        {
            get
            {
                return "DeluxeWorksPassport";
            }
        }
        /// <summary>
        /// 是否通过认证
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                return this.user != null;
            }
        }
        /// <summary>
        /// 用户名
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

        #region 私有成员
        private void Init(IUser user, ITicket ticket)
        {
            this.user = user;
            this.ticket = ticket;

            SetImpersonateInfo(this.user);
        }

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

        private static IUser GetUserInfoFromID(string userID)
        {
            OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, userID);

            ExceptionHelper.FalseThrow<AuthenticateException>(users.Count > 0, Resource.CanNotFindUser, userID);

            return users[0];
        }

        private static OguObjectCollection<IUser> GetUsersInfoFromIDs(params string[] userIDs)
        {
            return OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, userIDs);
        }

        private void SetImpersonateInfo(IUser originalUser)
        {
            this.realUser = originalUser;

            IUserImpersonatingInfoLoader loader = PrincipalSettings.GetConfig().GetImpersonatingInfoLoader(false);

            if (loader != null)
            {
                UserImpersonatingInfo info = loader.GetUserImpersonatingInfo(originalUser.ID);

                if (info != null && string.IsNullOrEmpty(info.ImpersonatingUserID) == false)
                    this.user = GetUserInfoFromID(info.ImpersonatingUserID);
            }
        }

        #endregion 私有成员
    }
}
