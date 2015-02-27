using MCS.Library.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Proxies.Configuration
{
    public class WfContractsProxySettings : DeluxeConfigurationSection
    {
        public static WfContractsProxySettings GetConfig()
        {
            WfContractsProxySettings settings = (WfContractsProxySettings)ConfigurationBroker.GetSection("wfContractsProxySettings");

            ConfigurationExceptionHelper.CheckSectionNotNull(settings, "wfContractsProxySettings");

            return settings;
        }

        public Uri ProcessDescriptorServiceUrl
        {
            get
            {
                return this.Paths.CheckAndGet("processDescriptorService").Uri;
            }
        }

        public Uri ProcessRuntimeServiceUrl
        {
            get
            {
                return this.Paths.CheckAndGet("processRuntimeService").Uri;
            }
        }

        public Uri DataSourceServiceUrl
        {
            get
            {
                return this.Paths.CheckAndGet("dataSourceService").Uri;
            }
        }

        [ConfigurationProperty("paths", IsRequired = true)]
        private UriConfigurationCollection Paths
        {
            get
            {
                return (UriConfigurationCollection)this["paths"];
            }
        }
    }
}
