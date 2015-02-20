using CIIC.HSR.TSP.DataAccess;
using CIIC.HSR.TSP.IoC;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    /// <summary>
    /// 租户相关帮助类
    /// </summary>
    public class TenantHelper
    {
        public static IUnitOfWork GetUnitWOrk(string tenantCode)
        {
            IWfTenantUnitWork factory = Containers.Global.Singleton.Resolve<IWfTenantUnitWork>();
            return factory.GetUnitOfWork(tenantCode);
        }
    }
}
