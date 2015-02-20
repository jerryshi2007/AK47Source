using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DocServiceContract
{
    /// <summary>
    /// 文件版本
    /// </summary>
    [DataContract]
	[Serializable]
    public class DCTFileVersion
    {
        /// <summary>
        /// 签入的描述
        /// </summary>
		[DataMember]
		public string CheckInComment
		{
			get;
			set;
		}

        /// <summary>
        /// 创建日期
        /// </summary>
		[DataMember]
		public DateTime Created
		{
			get;
			set;
		}

        /// <summary>
        /// 创建人
        /// </summary>
		[DataMember]
		public DCTUser CreatedBy
		{
			get;
			set;
		}

        /// <summary>
        /// 编号
        /// </summary>
		[DataMember]
		public int ID
		{
			get;
			set;
		}

        /// <summary>
        /// 是否当前版本
        /// </summary>
		[DataMember]
		public bool IsCurrentVersion
		{
			get;
			set;
		}

        /// <summary>
        /// 地址
        /// </summary>
		[DataMember]
		public string Uri
		{
			get;
			set;
		}

		[DataMember]
		public string AbsoluteUri
		{
			get;
			set;
		}
    }
}