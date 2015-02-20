#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Data
// FileName	：	EnumDefine.cs
// Remark	：	枚举类型定义。
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    龚文芳	    20070430		创建
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Data.Mapping
{
    #region Enumeration
    /// <summary>
    /// 为操作绑定标记
    /// </summary>
    [Flags]
    public enum ClauseBindingFlags
    {
        /// <summary>
        /// 任何情况下都不出现
        /// </summary>
        None = 0,

        /// <summary>
        /// 表示属性会出现在Insert中
        /// </summary>
        Insert = 1,

        /// <summary>
        /// 表示属性会出现在Update中
        /// </summary>
        Update = 2,

        /// <summary>
        /// 表示属性会出现在Where语句部分
        /// </summary>
        Where = 4,

        /// <summary>
        /// 表示属性会出现在查询的返回字段中
        /// </summary>
        Select = 8,

        /// <summary>
        /// 在所有情况下都会出现
        /// </summary>
        All = 15,
    }

    /// <summary>
    /// 枚举类型的使用方法
    /// </summary>
    public enum EnumUsageTypes
    {
        /// <summary>
        /// 使用枚举类型的值(整型)
        /// </summary>
        UseEnumValue = 0,

        /// <summary>
        /// 使用枚举类型的字符串
        /// </summary>
        UseEnumString
    }

    #endregion Enumeration
}
