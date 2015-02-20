using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract.Cache
{
    public interface ITaskCacheKeyGenerator
    {
        /// <summary>
        /// 生成CacheKey
        /// </summary>
        /// <param name="key">业务key</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        string Generate(string key,string userId);
    }
}
