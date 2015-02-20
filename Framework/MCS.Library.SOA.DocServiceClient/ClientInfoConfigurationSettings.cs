using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DocServiceClient
{
    public class DocClientConfigSettings : ConfigurationSection
    {
        private DocClientConfigSettings() { }

        public static DocClientConfigSettings GetConfig()
        {
            DocClientConfigSettings result = (DocClientConfigSettings)ConfigurationBroker.GetSection("docClientConfigSettings");

            if (result == null)
                result = new DocClientConfigSettings();

            return result;

        }

        [ConfigurationProperty("servers")]
        public ClientInfoConfigurationElementCollection Servers
        {
            get
            {
                return (ClientInfoConfigurationElementCollection)this["servers"];
            }
        }

    }
}
