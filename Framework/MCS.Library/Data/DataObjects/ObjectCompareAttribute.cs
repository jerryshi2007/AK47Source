#region
// -------------------------------------------------
// Assembly	：	HB.DataObjects
// FileName	：	ObjectCompareAttribute.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    张曦	    2008-03-17		创建
// -------------------------------------------------
#endregion

using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// 定义了对象比较的相关信息
    /// </summary>
    /// <remarks>
    /// 这些信息包括：keyField，是否为List。
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ObjectCompareAttribute : Attribute, IObjectCompareInfo
    {
        /// <summary>
        /// 需要比较的字段，可以还是多个字段，由逗号或分号分隔
        /// </summary>
        public string KeyFields
        {
            get;
            set;
        }

        /// <summary>
        /// 是否为List
        /// </summary>
        public bool IsList
        {
            get;
            set;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="keyFieldsName"></param>
        public ObjectCompareAttribute(string keyFieldsName)
        {
            this.KeyFields = keyFieldsName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="list"></param>
        public ObjectCompareAttribute(bool list)
        {
            this.IsList = list;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="keyFieldName"></param>
        /// <param name="list"></param>
        public ObjectCompareAttribute(string keyFieldName, bool list)
        {
            this.KeyFields = keyFieldName;
            this.IsList = list;
        }
    }
}
