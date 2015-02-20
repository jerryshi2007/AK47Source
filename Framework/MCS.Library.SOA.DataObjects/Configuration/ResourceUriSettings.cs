using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using MCS.Web.Library;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// 应用所使用的全局的Url定义和窗口定义
    /// </summary>
    public sealed class ResourceUriSettings : ConfigurationSection
    {
        public static ResourceUriSettings GetConfig()
        {
            ResourceUriSettings settings = (ResourceUriSettings)ConfigurationBroker.GetSection("resourceUriSettings");

            if (settings == null)
                settings = new ResourceUriSettings();

            return settings;
        }

        private ResourceUriSettings()
        {
        }

        [ConfigurationProperty("urls", IsRequired = false)]
        public UriConfigurationCollection Paths
        {
            get
            {
                return (UriConfigurationCollection)this["urls"];
            }
        }

        [ConfigurationProperty("features", IsRequired = false)]
        public NameWindowFeatureElementCollection Features
        {
            get
            {
                return (NameWindowFeatureElementCollection)this["features"];
            }
        }
    }
}
