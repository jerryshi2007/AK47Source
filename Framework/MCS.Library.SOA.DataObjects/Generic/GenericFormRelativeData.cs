using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 通用表单数据的子对象的数据
	/// </summary>
	[Serializable]
	[XmlRootMapping("GenericFormRelativeData")]
	[XElementSerializable]
	[ORTableMapping("WF.GENERIC_FORM_RELATIVE_DATA")]
	[ObjectCompare("RESOURCE_ID")]
	public class GenericFormRelativeData
	{
		[Description("父表单的ID")]
		[XmlObjectMapping]
		[ORFieldMapping("RESOURCE_ID", PrimaryKey = true)]
		public string ResourceID { get; set; }

		[Description("子对象的类别")]
		[XmlObjectMapping]
		[ORFieldMapping("CLASS", PrimaryKey = true)]
		public string Class { get; set; }

		[Description("子对象的序号")]
		[XmlObjectMapping]
		[ORFieldMapping("SORT_ID", PrimaryKey = true)]
		public int SortID { get; set; }

		/// <summary>
		/// 数据部分
		/// </summary>
		[Description("XML")]
		[ORFieldMapping("XML_CONTENT")]
		public virtual string XmlContent { get; set; }

		/// <summary>
		/// 搜索内容
		/// </summary>
		[ORFieldMapping("SEARCH_CONTENT")]
		[Description("搜索内容")]
		public virtual string SearchContent { get; set; }
	}

	/// <summary>
	/// 通用表单数据的子对象的数据集合
	/// </summary>
	[Serializable]
	public class GenericFormRelativeDataCollection<T> : EditableDataObjectCollectionBase<T> where T : GenericFormRelativeData, new()
	{
	}
}
