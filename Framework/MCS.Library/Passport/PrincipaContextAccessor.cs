using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MCS.Library.Passport
{
    /// <summary>
    /// Principal的上下文访问器
    /// </summary>
    public static class PrincipaContextAccessor
    {
        /// <summary>
        /// 在上下文中设置Principal
        /// </summary>
        /// <param name="principal"></param>
        public static void SetPrincipal(IPrincipal principal)
        {
            if (EnvironmentHelper.Mode == InstanceMode.Web)
                HttpContext.Current.User = principal;
            else
                Thread.CurrentPrincipal = principal;
        }

        /// <summary>
        /// 从上下文中读取指定类型的Principal，如果不存在，或者类型不匹配，则返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetPrincipal<T>() where T : class, IPrincipal
        {
            IPrincipal principal = default(T);

            if (EnvironmentHelper.Mode == InstanceMode.Web)
                principal = HttpContext.Current.User;
            else
                principal = Thread.CurrentPrincipal;

            return principal as T;
        }

        /// <summary>
        /// 从上下文中读取指定类型的Principal，如果不存在，或者类型不匹配，则抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetAndCheckPrincipal<T>() where T : class, IPrincipal
        {
            T principal = GetPrincipal<T>();

            if (principal == null)
                new InvalidOperationException(string.Format("不能在上下文中找到类型为{0}的Principal", typeof(T).FullName));

            return principal;
        }
    }
}
