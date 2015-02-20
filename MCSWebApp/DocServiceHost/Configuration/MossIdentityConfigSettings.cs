using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using MCS.Library.Configuration;

namespace DocServiceHost.Configuration
{
    public class MossIdentityConfigSettings :ConfigurationSection
    {
        private MossIdentityConfigSettings() { }

        public static MossIdentityConfigSettings GetConfig()
        {
            MossIdentityConfigSettings result = (MossIdentityConfigSettings)ConfigurationBroker.GetSection("mossIdentityConfigSettings");

            if (result == null)
                result = new MossIdentityConfigSettings();

            return result;

        }

        [ConfigurationProperty("identities")]
        public MossIdentityConfigrationElementCollection Identities
        {
            get
            {
                return (MossIdentityConfigrationElementCollection)this["identities"];
            }
        }

    }
}