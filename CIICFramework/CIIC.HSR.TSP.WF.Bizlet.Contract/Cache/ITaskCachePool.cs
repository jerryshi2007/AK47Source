using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract.Cache
{
    /// <summary>
    /// 待办缓存池管理
    /// </summary>
    public interface ITaskCachePool
    {
        /// <summary>
        /// 刷新所有缓存的数据
        /// </summary>
        /// <typeparam name="T">缓存中数据的类型</typeparam>
        /// <param name="userId">用户Id</param>
        /// <returns>返回缓存中的最新数据</returns>
        void RefreshCacheData(string userId, string tenantCode, bool isTenantMode);
        /// <summary>
        /// 刷新指定的数据
        /// </summary>
        /// <typeparam name="T">缓存中数据的类型</typeparam>
        /// <param name="key">缓存Key</param>
        /// <param name="userId">用户Id</param>
        /// <returns>返回缓存中的最新数据</returns>
        void RefreshCacheData(string key, string userId, string tenantCode, bool isTenantMode);
        /// <summary>
        /// 获取指定的缓存数据
        /// </summary>
        /// <typeparam name="T">缓存中数据的类型</typeparam>
        /// <param name="key">缓存Key</param>
        /// <param name="userId">用户Id</param>
        /// <returns>返回缓存中的数据</returns>
        T GetCacheData<T>(string key, string userId, string tenantCode, bool isTenantMode) where T : class;
    }
}
