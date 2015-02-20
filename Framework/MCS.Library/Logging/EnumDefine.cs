#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	EnumDefine.cs
// Remark	：	日志组件中枚举类型的定义
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\zhangtiejun    20070430		创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using MCS.Library.Core;

namespace MCS.Library.Logging
{
    /// <summary>
    /// 日志记录优先级枚举
    /// </summary>
    /// <remarks>
    /// 共分五级优先级
    /// </remarks>
    public enum LogPriority
    {
        /// <summary>
        /// 最低优先级
        /// </summary>
        Lowest,

        /// <summary>
        /// 低优先级
        /// </summary>
        BelowNormal,

        /// <summary>
        /// 普通
        /// </summary>
        Normal,

        /// <summary>
        /// 高优先级
        /// </summary>
        AboveNormal,

        /// <summary>
        /// 最高优先级
        /// </summary>
        Highest,
    }
}
