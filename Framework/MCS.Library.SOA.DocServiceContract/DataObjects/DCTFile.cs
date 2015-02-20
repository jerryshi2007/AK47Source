using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DocServiceContract
{
	/// <summary>
	/// 文件
	/// </summary>
    [DataContract(IsReference = true)]
    [Serializable]
	public class DCTFile : DCTStorageObject
	{
		/// <summary>
		/// 作者
		/// </summary>
		[DataMember]
		public DCTUser Author
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
		/// 修改者
		/// </summary>
		[DataMember]
		public DCTUser ModifiedBy
		{
			get;
			set;
		}

		/// <summary>
		/// 修改日期
		/// </summary>
		[DataMember]
		public DateTime Modified
		{
			get;
			set;
		}

		/// <summary>
		/// 签出者
		/// </summary>
		[DataMember]
		public DCTUser CheckedOutBy
		{
			get;
			set;
		}

		/// <summary>
		/// 签出类型
		/// </summary>
		[DataMember]
		public DCTCheckOutType CheckOutType
		{
			get;
			set;
		}

		/// <summary>
		/// 主要版本
		/// </summary>
		[DataMember]
		public int MajorVersion
		{
			get;
			set;
		}

		/// <summary>
		/// 次要版本
		/// </summary>
		[DataMember]
		public int MinorVersion
		{
			get;
			set;
		}
	}
}