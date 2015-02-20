using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DocServiceContract
{
	/// <summary>
	/// 字段信息
	/// </summary>
	[DataContract]
	[Serializable]
	public class DCTFieldInfo
	{
		/// <summary>
		/// id
		/// </summary>
		[DataMember]
		public string ID
		{
			get;
			set;
		}

		/// <summary>
		/// 默认值
		/// </summary>
		[DataMember]
		public string DefaultValue
		{
			get;
			set;
		}

		/// <summary>
		/// 内部名称
		/// </summary>
		[DataMember]
		public string InternalName
		{
			get;
			set;
		}

		/// <summary>
		/// 是否必填
		/// </summary>
		[DataMember]
		public bool Required
		{
			get;
			set;
		}

		/// <summary>
		/// 标题
		/// </summary>
		[DataMember]
		public string Title
		{
			get;
			set;
		}

		/// <summary>
		/// 值类型
		/// </summary>
		[DataMember]
		public DCTFieldType ValueType
		{
			get;
			set;
		}

		/// <summary>
		/// 验证公式
		/// </summary>
		[DataMember]
		public string ValidationFormula
		{
			get;
			set;
		}

		/// <summary>
		/// 验证不通过时给出的错误信息
		/// </summary>
		[DataMember]
		public string ValidationMessage
		{
			get;
			set;
		}
	}
}