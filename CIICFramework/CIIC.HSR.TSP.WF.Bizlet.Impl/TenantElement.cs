using CIIC.HSR.TSP.DataAccess;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    public class TenantElement : ConfigurationElement 
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return this["name"] as string; }
            set { this["name"] = value; }
        }
        [ConfigurationProperty("enabled", IsRequired = true)]
        public bool Enabled
        {
            get { return this["enabled"].ToString().Equals("True",StringComparison.CurrentCultureIgnoreCase); }
            set { this["enabled"] = value.ToString(); }
        }
    }
}
