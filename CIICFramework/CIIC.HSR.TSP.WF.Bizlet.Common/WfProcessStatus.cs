using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Common
{
    /// <summary>

    /// 流程的状态

    /// </summary>

    public enum WfProcessStatusDetail
    {
        /// <summary>
        /// 运行中
        /// </summary>
        Running,

        /// <summary>
        /// 已完成
        /// </summary>
        Completed,

        /// <summary>
        /// 被终止
        /// </summary>
        Aborted,

        /// <summary>
        /// 未运行
        /// </summary>
        NotRunning,

        /// <summary>
        /// 已暂停
        /// </summary>
        Paused,

        /// <summary>
        /// 维护中
        /// </summary>
        Maintaining
    }


    public enum WfProcessStatus
    {
        All=0,
        /// <summary>
        /// 未运行
        /// </summary>
        NotRunning = 1,
        /// <summary>
        /// 运行中
        /// </summary>
        Running = 2,
        /// <summary>
        /// 已完成
        /// </summary>
        Completed = 3
    }
}
