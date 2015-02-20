//using CIIC.HSR.SSP.Tenants;
using CIIC.HSR.TSP.DataAccess;
using CIIC.HSR.TSP.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract
{
    /// <summary>
    /// 租户接口
    /// </summary>
    public interface IWfTenantUnitWork
    {
        /// <summary>
        /// 获取租户上下文
        /// </summary>
        /// <param name="tenantCode">租住编码</param>
        /// <returns>租户上下文</returns>
        IUnitOfWork GetUnitOfWork(string tenantCode);
    }
    /// <summary>
    /// 无租住的系统构建器
    /// </summary>
    public class DefaultWFTenantUOWFactory : IWfTenantUnitWork
    {
        /// <summary>
        /// 获取租户上线文
        /// </summary>
        /// <param name="tenantCode">租户编码</param>
        /// <returns>租户上下文</returns>
        public virtual IUnitOfWork GetUnitOfWork(string tenantCode)
        {
            return Containers.Default.CreateWFUnitOfWork();
        }
    }
    public class SSPWFTenantUOWFactory : IWfTenantUnitWork
    {
        public DataAccess.IUnitOfWork GetUnitOfWork(string tenantCode)
        {
			throw new NotImplementedException();
			//Tenant tenant = TenantsUtilities.GetTenantByTenantCode(tenantCode);
			//ITenantContext tenantContext = TenantsUtilities.GetTenantContext(tenant);
			//return tenantContext.GetTenantUnitOfWork(DataConnectionType.Business);
        }
    }
}
