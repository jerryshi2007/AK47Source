using CIIC.HSR.TSP.DataAccess;
using CIIC.HSR.TSP.IoC;
using CIIC.HSR.TSP.TA.Bizlet.Contract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    public class PermissionUnitWork : IPermissionUOWFactory
    {
        public DataAccess.IUnitOfWork GetUnitOfWork(string tenantCode)
        {
            TenantSection tenantSection = ConfigurationManager.GetSection("tenantSetting") as TenantSection;
            string registerKey = tenantSection.Tenants.GetEffectiveElement().Name;
            var unitWork = Containers.Global.Singleton.ResolveFactory<IUnitOfWork>(null, registerKey);
            return unitWork;
        }
    }
}
