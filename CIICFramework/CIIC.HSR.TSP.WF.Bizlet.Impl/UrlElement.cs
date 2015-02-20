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
    public class UrlElement : ConfigurationElement 
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return this["name"] as string; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("description", IsRequired = false)]
        public string Description
        {
            get { return this["description"] as string; }
            set { this["description"] = value; }
        }

        [ConfigurationProperty("url", IsRequired = true)]
        public string Url
        {
            get { return this["url"] as string; }
            set { this["url"] = value; }
        }

        [ConfigurationProperty("default", IsRequired = false)]
        public string Default
        {
            get { return this["default"] as string; }
            set { this["default"] = value; }
        }

    }
}
