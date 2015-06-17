#region 版本作者
// -------------------------------------------------
// Assembly	：	HB.DataObjects
// FileName	：	TaskCategory.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    李苗	    20070628		创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Data.Mapping;
using MCS.Library;
using MCS.Library.Core;
using System.Collections;
using System.Data;
using MCS.Library.Validation;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 待办箱处理夹
	/// </summary>
	[Serializable]
	[ORTableMapping("WF.USER_TASK_CATEGORY")]
    [TenantRelativeObject]
	public class TaskCategory
	{
		private string categoryID = string.Empty;
		private string categoryName = string.Empty;
		private string userID = string.Empty;
		private int innerSortID = 0;

		/// <summary>
		/// 类别ID
		/// </summary>
		[ORFieldMapping("CATEGORY_GUID", PrimaryKey = true)]
		public string CategoryID
		{
			get
			{
				return this.categoryID;
			}
			set
			{
				this.categoryID = value;
			}
		}

		/// <summary>
		/// 类别名称
		/// </summary>
		[ORFieldMapping("CATEGORY_NAME")]
		public string CategoryName
		{
			get
			{
				return this.categoryName;
			}
			set
			{
				this.categoryName = value;
			}
		}

		/// <summary>
		/// 用户ID
		/// </summary>
		[ORFieldMapping("USER_ID")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Insert)]
		public string UserID
		{
			get
			{
				return this.userID;
			}
			set
			{
				this.userID = value;
			}
		}

		/// <summary>
		/// 类别排序
		/// </summary>
		[ORFieldMapping("INNER_SORT_ID")]
		[StringLengthValidator(1, 5, MessageTemplate = "类别序号的长度请控制在1到5个数字!")]
		[IntegerRangeValidator(1, 65535, MessageTemplate = "类别序号应该大于0且小于等于65535!")]
		public int InnerSortID
		{
			get
			{
				return this.innerSortID;
			}
			set
			{
				this.innerSortID = value;
			}
		}
	}

	public class TaskCategoryCollection : EditableDataObjectCollectionBase<TaskCategory>
	{
		#region 公共函数
		/// <summary>
		/// 从DataView获取数据到TaskCategoryCollection中
		/// </summary>
		/// <param name="dv"></param>
		public void LoadFromDataView(DataView dv)
		{
			this.Clear();

			ORMapping.DataViewToCollection(this, dv);
		}
		#endregion
	}
}
