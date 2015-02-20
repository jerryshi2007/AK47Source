using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract.Cache
{
    /// <summary>
    /// 业务从数据库加载接口
    /// </summary>
    public interface ITaskDbLoader
    {
        /// <summary>
        /// 过期时间
        /// </summary>
        TimeSpan ExpiredTimeSpan { get; set; }
        /// <summary>
        /// 缓存Key
        /// </summary>
        string ChacheKey { get; set; }
        /// <summary>
        /// 在缓存中无数据时，需要加载的业务数据接口
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="userId">用户Id</param>
        /// <param name="tenantCode">租户编码</param>
        /// <param name="IsTenantMode">是否是多租户模式</param>
        /// <returns>预警信息</returns>
        T LoadDataFromDb<T>(string userId, string tenantCode, bool IsTenantMode) where T : class;
    }
}
