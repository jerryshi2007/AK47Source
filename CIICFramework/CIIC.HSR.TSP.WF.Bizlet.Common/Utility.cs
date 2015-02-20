using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Common
{
    public static class Utility
    {
        public static void ExistsSkip(this IDictionary<string, string> dic, string key, string value)
        {
            if (!dic.ContainsKey(key))
            {
                dic.Add(key, value);
            }
        }
    }
}
