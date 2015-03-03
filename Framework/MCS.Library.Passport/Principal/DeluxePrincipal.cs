#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Passport
// FileName	：	DeluxePrincipal.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          胡自强      2008-12-2       添加注释
// -------------------------------------------------
#endregion
using System;
using System.Web;
using System.Text;
using System.Threading;
using System.Security.Principal;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Passport;
using MCS.Library.OGUPermission;
using MCS.Library.Passport.Properties;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.Principal
{
    /// <summary>
    /// 通过DeluxeWorks认证机制，所产生的用户身份及其授权信息，实现了系统的IPrincipal接口。
    /// </summary>
    public class DeluxePrincipal : IGenericTokenPrincipal
    {
        private DeluxeIdentity userIdentity = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="identity"></param>
        public DeluxePrincipal(DeluxeIdentity identity)
        {
            this.userIdentity = identity;
        }

        /// <summary>
        /// 从当前线程中取得用户的登录和权限信息(Principal)
        /// </summary>
        public static DeluxePrincipal Current
        {
            get
            {
                DeluxePrincipal result = GetPrincipal();

                ExceptionHelper.FalseThrow<AuthenticateException>(result != null,
                    Resource.CanNotGetPrincipal);

                return result;
            }
        }

        /// <summary>
        /// 是否经过认证
        /// </summary>
        public static bool IsAuthenticated
        {
            get
            {
                return GetPrincipal() != null;
            }
        }

        private static DeluxePrincipal GetPrincipal()
        {
            return PrincipaContextAccessor.GetPrincipal<DeluxePrincipal>();
        }

        #region IPrincipal 成员
        /// <summary>
        /// 用户身份标识
        /// </summary>
        public IIdentity Identity
        {
            get
            {
                return this.userIdentity;
            }
        }

        /// <summary>
        /// 判断当前用户是否属于某一角色
        /// </summary>
        /// <param name="role">角色的描述(应用的名称1:角色名称11,角色名称12,...;应用名称2:角色名称21,角色名称22,...)</param>
        /// <returns>用户是否属于某一角色</returns>
        public bool IsInRole(string role)
        {
            return IsInRole(this.userIdentity.User, role);
        }

        /// <summary>
        /// 判断某个用户是否属于某一角色
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role">角色的描述(应用的名称1:角色名称11,角色名称12,...;应用名称2:角色名称21,角色名称22,...)</param>
        /// <returns>用户是否属于某一角色</returns>
        public static bool IsInRole(IUser user, string role)
        {
            user.NullCheck("user");
            role.CheckStringIsNullOrEmpty("role");

            string[] appRoles = role.Split(';');

            for (int i = 0; i < appRoles.Length; i++)
            {
                string[] oneAppRoles = appRoles[i].Split(':');

                ExceptionHelper.FalseThrow<AuthenticateException>(oneAppRoles.Length == 2, Resource.InvalidAppRoleNameDescription);

                string appName = oneAppRoles[0].Trim();

                string[] roles = oneAppRoles[1].Split(',');

                for (int j = 0; j < roles.Length; j++)
                {
                    string roleName = roles[j].Trim();

                    if (user.Roles.ContainsApp(appName))
                    {
                        if (user.Roles[appName].ContainsKey(roleName))
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 如果是Web App(有HttpContext的)，则通过设置Cookie失效的方式来清除缓存中的Principal
        /// </summary>
        public void ClearCacheInWebApp()
        {
            if (EnvironmentHelper.Mode == InstanceMode.Web)
            {
                string cookieName = Common.C_SESSION_KEY_NAME;

                HttpCookie cookie = HttpContext.Current.Response.Cookies[cookieName];

                if (cookie != null)
                {
                    HttpContext.Current.Response.Cookies.Remove(cookieName);

                    cookie = new HttpCookie(cookieName, "Principal");
                    cookie.Expires = DateTime.Now.AddDays(-1);

                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
            }
        }

        /// <summary>
        /// 根据描述得到一组角色集合
        /// </summary>
        /// <param name="rolesDesp"></param>
        /// <returns></returns>
        public static IRole[] GetRoles(string rolesDesp)
        {
            List<IRole> result = new List<IRole>();

            if (string.IsNullOrEmpty(rolesDesp) == false)
            {
                AppRolesDespsCollection allAppRolesDesps = GetAppRolesDesps(rolesDesp);

                ApplicationCollection apps = PermissionMechanismFactory.GetMechanism().GetApplications(allAppRolesDesps.GetAppNames());

                foreach (AppRolesDesps appRolesDesps in allAppRolesDesps)
                {
                    if (apps.ContainsKey(appRolesDesps.AppName))
                    {
                        IApplication app = apps[appRolesDesps.AppName];

                        foreach (string roleName in appRolesDesps.RolesDesp)
                        {
                            if (app.Roles.ContainsKey(roleName))
                            {
                                result.Add(app.Roles[roleName]);
                            }
                        }
                    }
                }
            }

            return result.ToArray();
        }

        private static AppRolesDespsCollection GetAppRolesDesps(string rolesDesp)
        {
            AppRolesDespsCollection appRolesDesps = new AppRolesDespsCollection();

            string[] appRoles = rolesDesp.Split(';');

            for (int i = 0; i < appRoles.Length; i++)
            {
                string[] oneAppRoles = appRoles[i].Split(':');

                ExceptionHelper.FalseThrow<AuthenticateException>(oneAppRoles.Length == 2, Resource.InvalidAppRoleNameDescription);

                string appName = oneAppRoles[0].Trim();

                string[] roles = oneAppRoles[1].Split(',');

                appRolesDesps.Add(new AppRolesDesps { AppName = appName, RolesDesp = roles });
            }

            return appRolesDesps;
        }

        #endregion

        private class AppRolesDesps
        {
            public string AppName;
            public string[] RolesDesp;
        }

        private class AppRolesDespsCollection : EditableDataObjectCollectionBase<AppRolesDesps>
        {
            public string[] GetAppNames()
            {
                List<string> result = new List<string>();

                foreach (AppRolesDesps item in this)
                    result.Add(item.AppName);

                return result.ToArray();
            }
        }

        /// <summary>
        /// 得到票据相关的Token容器
        /// </summary>
        /// <returns></returns>
        public GenericTicketTokenContainer GetGenericTicketTokenContainer()
        {
            GenericTicketTokenContainer container = new GenericTicketTokenContainer();

            container.CopyFrom(this.Identity as DeluxeIdentity);

            return container;
        }
    }
}
