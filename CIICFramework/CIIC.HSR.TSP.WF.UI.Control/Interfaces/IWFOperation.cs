using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.UI.Control.Interfaces
{
    /// <summary>
    /// 客户端执行流程操作接口
    /// </summary>
    /// <typeparam name="T">行为的操作结果类型</typeparam>
    public interface IWFOperation<T>
    {
        /// <summary>
        /// 执行流程行为
        /// </summary>
        /// <returns>行为的操作结果</returns>
        T Execute();
    }
}
