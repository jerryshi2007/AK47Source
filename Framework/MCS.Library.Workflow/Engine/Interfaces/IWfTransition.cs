using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Workflow.Engine
{
    /// <summary>
    /// 连线的实例
    /// </summary>
    public interface IWfTransition
    {
        /// <summary>
        /// 线的ID
        /// </summary>
        string ID
        {
            get;
        }

        /// <summary>
        /// 起点
        /// </summary>
        IWfActivity FromActivity
        {
            get;
        }

        /// <summary>
        /// 终点
        /// </summary>
        IWfActivity ToActivity
        {
            get;
        }

        /// <summary>
        /// 起始时间 ???需要么
        /// </summary>
        DateTime StartTime
        {
            get;
        }

        /// <summary>
        /// 线是否被取消（删除）
        /// </summary>
        bool IsAborted
        {
            get;
        }
    }
}
