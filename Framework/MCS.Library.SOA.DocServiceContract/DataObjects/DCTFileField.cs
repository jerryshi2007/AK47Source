using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DocServiceContract
{
    /// <summary>
    /// 文件字段
    /// </summary>
    [DataContract]
	[Serializable]
    public class DCTFileField
    {
        /// <summary>
        /// 字段
        /// </summary>
		[DataMember]
		public DCTFieldInfo Field
		{
			get;
			set;
		}

        /// <summary>
        /// 值 
        /// </summary>
		[DataMember]
		public string FieldValue
		{
			get;
			set;
		}
    }
}