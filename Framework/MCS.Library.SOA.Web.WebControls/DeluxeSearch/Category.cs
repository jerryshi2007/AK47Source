using System;
using System.Web.UI;
using MCS.Library.Data.DataObjects;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// DeluxeSearch涉及到的Category
	/// </summary>
	[Serializable]
	public class Category
	{
		/// <summary>
		/// 数据源控件的ID
		/// </summary>
		public string DataSourceID { get; set; }

		/// <summary>
		/// 数据源中的字段
		/// </summary>
		public string CategoryField { get; set; }

		/// <summary>
		/// 列表项提供文本内容的数据源字段
		/// </summary>
		public string DataTextField { get; set; }

		/// <summary>
		/// 各列表项提供值的数据源字段
		/// </summary>
		public string DataValueField { get; set; }

        /// <summary>
        /// 数据源中的值
        /// </summary>
        public string ConditionValue { get; set; }

        /// <summary>
        /// 数据源中的文本
        /// </summary>
        public string ConditionText { get; set; }
	}

	/// <summary>
	/// DeluxeSearchCategory集合
	/// </summary>
	[Serializable]
	public class DeluxeSearchCategoryCollection : SerializableEditableKeyedDataObjectCollectionBase<string, Category>
	{
		protected override string GetKeyForItem(Category item)
		{
			return item.CategoryField;
		}
	}
}
