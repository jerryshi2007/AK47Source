using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
    /// <summary>
    /// 组合类型类型定义：与 或 
    /// </summary>
    public enum CompositionType
    { 
        /// <summary>
        /// 与，表示附加在目标元素上的多个校验器全部校验正确，校验才可通过
        /// </summary>
        And,

        /// <summary>
        /// 或，表示附加在目标元素上的多个校验器只要有一个校验正确，校验即可通过
        /// </summary>
        Or
    }
}
