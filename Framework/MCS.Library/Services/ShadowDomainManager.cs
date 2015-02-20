using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;

namespace MCS.Library.Services
{
    /// <summary>
    /// 
    /// </summary>
    public static class ShadowDomainManager
    {
        private readonly static Dictionary<string, AppDomain> _registeredDomains = new Dictionary<string, AppDomain>(StringComparer.OrdinalIgnoreCase);
        private const string _domainProxeKey = "AppDomainProxy";

        /// <summary>
        /// 加载某个应用的AppDomain，如果已经加载，则返回已经加载的AppDomain
        /// </summary>
        /// <param name="appName">应用名称</param>
        /// <param name="appDomainBuilder">AppDomain的创建器</param>
        /// <returns>应用程序域(AppDomain)</returns>
        public static AppDomain LoadAppDomain(string appName, Func<string, AppDomain> appDomainBuilder)
        {
            appName.CheckStringIsNullOrEmpty("appName");
            appDomainBuilder.NullCheck("appDomainBuilder");

            lock (_registeredDomains)
            {
                AppDomain domain = null;

                if (_registeredDomains.TryGetValue(appName, out domain) == false)
                {
                    domain = CreateAppDomain(appName, appDomainBuilder);
                    _registeredDomains.Add(appName, domain);
                }

                return domain;
            }
        }

        /// <summary>
        /// 通过某个应用对应的AppDomain来创建该域中的对象
        /// </summary>
        /// <param name="appDomain">应用名称</param>
        /// <param name="typeDesp">类型描述</param>
        /// <param name="args">对象的构造参数</param>
        /// <returns></returns>
        public static object CreateInstanceInApp(this AppDomain appDomain, string typeDesp, params object[] args)
        {
            appDomain.NullCheck("appDomain");

            AppDomainProxy proxy = (AppDomainProxy)appDomain.GetData(_domainProxeKey);

            if (proxy == null)
                throw new SystemSupportException(string.Format("不能再AppDomain中找到{0}的信息", _domainProxeKey));

            return proxy.CreateInstance(typeDesp, args);
        }

        /// <summary>
        /// 根据应用名称查找AppDomain，如果没有找到，则抛出异常
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static AppDomain GetAppDomain(string appName)
        {
            AppDomain appDomain = FindAppDomain(appName);

            if (appDomain == null)
                throw new SystemSupportException(string.Format("不能应用名称为{0}的AppDomain", appName));

            return appDomain;
        }

        /// <summary>
        /// 卸载一个AppDomain
        /// </summary>
        /// <param name="appName"></param>
        public static void UnloadAppDomain(string appName)
        {
            lock (_registeredDomains)
            {
                AppDomain domain = FindAppDomain(appName);

                if (domain != null)
                {
                    AppDomain.Unload(domain);
                    _registeredDomains.Remove(appName);
                }
            }
        }

        private static AppDomainProxy CreateProxy(AppDomain domain)
        {
            Type typeProxy = typeof(AppDomainProxy);

            return (AppDomainProxy)domain.CreateInstanceAndUnwrap(typeProxy.Assembly.FullName, typeProxy.FullName);
        }

        private static AppDomain CreateAppDomain(string appName, Func<string, AppDomain> appDomainBuilder)
        {
            AppDomain domain = appDomainBuilder(appName);

            domain.SetData(_domainProxeKey, CreateProxy(domain));

            return domain;
        }

        /// <summary>
        /// 根据应用的名称查找AppDomain
        /// </summary>
        /// <param name="appName">应用的名称</param>
        /// <returns>应用程序域</returns>
        private static AppDomain FindAppDomain(string appName)
        {
            appName.CheckStringIsNullOrEmpty("appName");

            lock (_registeredDomains)
            {
                AppDomain domain = null;

                _registeredDomains.TryGetValue(appName, out domain);

                return domain;
            }
        }
    }
}
