using CIIC.HSR.TSP.WF.Bizlet.Contract.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Cache
{
    public class TaskCacheKeyGenerator : ITaskCacheKeyGenerator
    {
        public string Generate(string key, string userId)
        {
            return string.Format("CIIC.HSR.TSP.WF.Bizlet.Impl_{0}_{1}", key, userId);
        }
    }
}
