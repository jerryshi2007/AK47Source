using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    /// <summary>
    /// Url配置管理类
    /// </summary>
    public class UrlSection : ConfigurationSection 
    {
        [ConfigurationProperty("urls", IsDefaultCollection = false)]
        public UrlElementCollection Urls
        {
            get
            {
                return (UrlElementCollection)this["urls"];
            }
            set
            {
                this["urls"] = value;
            }
        }
    }
}
