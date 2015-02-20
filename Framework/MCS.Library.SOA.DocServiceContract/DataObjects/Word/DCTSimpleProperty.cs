using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DocServiceContract
{
    /// <summary>
    /// Word数据描述
    /// </summary>
    [DataContract]
    [KnownType(typeof(int))]
    [KnownType(typeof(string))]
    [KnownType(typeof(DateTime))]
    [KnownType(typeof(double))]
    [KnownType(typeof(float))]
    [KnownType(typeof(decimal))]
    [KnownType(typeof(long))]
    public class DCTSimpleProperty : DCTDataProperty
    {
        /// <summary>
        /// 标签值
        /// </summary>
        [DataMember]
        public object Value { get; set; }

        /// <summary>
        /// 格式化字符串
        /// </summary>
        [DataMember]
        public string FormatString { get; set; }

        /// <summary>
        /// 值类型
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// 只读
        /// </summary>
        [DataMember]
        public bool IsReadOnly { get; set; }


    }
}
