using CIIC.HSR.TSP.WF.BizObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract
{
    /// <summary>
    /// 操作上下文
    /// </summary>
    public interface IRuntime
    {
        /// <summary>
        /// 数据
        /// </summary>
        ServiceContext Context { get; set; }
    }
}
