using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Common
{
    /// <summary>
    /// 代办状态
    /// </summary>
    public enum TaskStatus
    {
        /// <summary>
        /// 所有
        /// </summary>
        All=1,
        /// <summary>
        /// 未处理
        /// </summary>
        Unprocessed=2,
        /// <summary>
        /// 已处理
        /// </summary>
        Processed=3
    }
}
