using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MCS.Library.SOA.DocServiceContract
{
    /// <summary>
    /// 字段类型枚举
    /// </summary>
    /// 
     [Serializable]
    public enum DCTFieldType
    {
        /// <summary>
        /// 布尔型
        /// </summary>
        Boolean=0,
        /// <summary>
        /// 日期型
        /// </summary>
        DateTime=1,
        /// <summary>
        /// 数字
        /// </summary>
        Decimal=2,
        /// <summary>
        /// 整型
        /// </summary>
        Integer=4,
        /// <summary>
        /// 备注
        /// </summary>
        Note=5,
        /// <summary>
        /// 文本
        /// </summary>
        Text=6,
        /// <summary>
        /// 用户
        /// </summary>
        User=8,
    }
}