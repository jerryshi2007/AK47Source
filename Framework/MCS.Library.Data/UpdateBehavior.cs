using System;
using System.Collections.Generic;
using System.Text;
namespace MCS.Library.Data
{
    /// <summary>
    /// 指导批量更新过程的枚举
    /// </summary>
    /// <remarks>
    ///     面向批量处理增加的枚举
    ///     added by wangxiang . May 21, 2008
    /// </remarks>
    public enum UpdateBehavior
    {
        /// <summary>
        /// DataAdapter的标准过程，执行至出错为止
        /// </summary>
        Standard,

        /// <summary>
        /// 出错继续执行后续更新
        /// </summary>
        Continue,

        /// <summary>
        /// 整体作为一个事务提交
        /// </summary>
        Transactional
    }

}
