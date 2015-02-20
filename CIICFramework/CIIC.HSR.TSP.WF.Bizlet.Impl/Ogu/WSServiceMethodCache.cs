using MCS.Library.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Ogu
{
    public class WSServiceMethodCache : ServiceMethodCache
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceName"></param>
        /// <param name="maxLength"></param>
        public WSServiceMethodCache(string instanceName, int maxLength) :
            base(instanceName, maxLength)
        {
        }
    }
}
