using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Web.Responsive.WebControls
{
    /// <summary>
    /// 页码显示模式
    /// </summary>
    /// <remarks>
    /// 页码显示模式
    /// </remarks>
    public enum PagerCodeShowMode
    {
        /// <summary>
        /// 总页码
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        RecordCount,

        /// <summary>
        /// 当前页/总页码
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        CurrentRecordCount,

        /// <summary>
        /// 自动显示（根据页面可视区域）
        /// </summary>
        Auto,

        /// <summary>
        /// 全部显示
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        All
    }
}
