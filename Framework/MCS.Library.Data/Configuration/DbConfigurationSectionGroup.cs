using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Configuration;
namespace MCS.Library.Data.Configuration
{
    sealed class DbConfigurationSectionGroup : ConfigurationSectionGroup
    {
        public DbConfigurationSectionGroup() : base() 
        {
        }

        [ConfigurationProperty("connectionManager")]
        public ConnectionManagerConfigurationSection ConnectionManager
        {
            get 
            {
                return base.Sections["connectionManager"] as ConnectionManagerConfigurationSection; 
            }
        }

    }
}
