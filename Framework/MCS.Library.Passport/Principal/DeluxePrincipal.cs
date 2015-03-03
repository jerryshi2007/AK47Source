#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	DeluxePrincipal.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          ����ǿ      2008-12-2       ���ע��
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
    /// ͨ��DeluxeWorks��֤���ƣ����������û���ݼ�����Ȩ��Ϣ��ʵ����ϵͳ��IPrincipal�ӿڡ�
    /// </summary>
    public class DeluxePrincipal : IGenericTokenPrincipal
    {
        private DeluxeIdentity userIdentity = null;

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="identity"></param>
        public DeluxePrincipal(DeluxeIdentity identity)
        {
            this.userIdentity = identity;
        }

        /// <summary>
        /// �ӵ�ǰ�߳���ȡ���û��ĵ�¼��Ȩ����Ϣ(Principal)
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
        /// �Ƿ񾭹���֤
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

        #region IPrincipal ��Ա
        /// <summary>
        /// �û���ݱ�ʶ
        /// </summary>
        public IIdentity Identity
        {
            get
            {
                return this.userIdentity;
            }
        }

        /// <summary>
        /// �жϵ�ǰ�û��Ƿ�����ĳһ��ɫ
        /// </summary>
        /// <param name="role">��ɫ������(Ӧ�õ�����1:��ɫ����11,��ɫ����12,...;Ӧ������2:��ɫ����21,��ɫ����22,...)</param>
        /// <returns>�û��Ƿ�����ĳһ��ɫ</returns>
        public bool IsInRole(string role)
        {
            return IsInRole(this.userIdentity.User, role);
        }

        /// <summary>
        /// �ж�ĳ���û��Ƿ�����ĳһ��ɫ
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role">��ɫ������(Ӧ�õ�����1:��ɫ����11,��ɫ����12,...;Ӧ������2:��ɫ����21,��ɫ����22,...)</param>
        /// <returns>�û��Ƿ�����ĳһ��ɫ</returns>
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
        /// �����Web App(��HttpContext��)����ͨ������CookieʧЧ�ķ�ʽ����������е�Principal
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
        /// ���������õ�һ���ɫ����
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
        /// �õ�Ʊ����ص�Token����
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
