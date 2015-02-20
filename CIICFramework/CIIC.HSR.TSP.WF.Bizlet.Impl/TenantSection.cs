using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    /// <summary>
    /// 租户配置管理类
    /// </summary>
    public class TenantSection : ConfigurationSection 
    {
        [ConfigurationProperty("tenants", IsDefaultCollection = false)]
        public TenantElementCollection Tenants
        {
            get
            {
                return (TenantElementCollection)this["tenants"];
            }
            set
            {
                this["tenants"] = value;
            }
        }
    }
}
