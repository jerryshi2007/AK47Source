using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Services
{
    /// <summary>
    /// AppDomain的创建器
    /// </summary>
    public static class AppDomainBuilders
    {
        /// <summary>
        /// 默认的AppDomain的创建器
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static AppDomain CreateDefaultAppDomain(string appName)
        {
            AppDomainSetup setupInfo = new AppDomainSetup();

            setupInfo.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            setupInfo.PrivateBinPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            setupInfo.DisallowBindingRedirects = false;
            setupInfo.DisallowCodeDownload = true;
            setupInfo.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

            setupInfo.ApplicationName = appName;

            string domainName = string.Format("{0}-{1:yyyy-MM-dd HH:mm:ss}", appName, DateTime.Now);

            return AppDomain.CreateDomain(domainName, null, setupInfo);
        }
    }
}
