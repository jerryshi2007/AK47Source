using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using MCS.Web.Library;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// Ӧ����ʹ�õ�ȫ�ֵ�Url����ʹ��ڶ���
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
