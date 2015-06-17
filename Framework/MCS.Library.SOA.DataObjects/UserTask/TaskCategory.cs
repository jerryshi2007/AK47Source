#region �汾����
// -------------------------------------------------
// Assembly	��	HB.DataObjects
// FileName	��	TaskCategory.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    20070628		����
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
	/// �����䴦���
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
		/// ���ID
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
		/// �������
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
		/// �û�ID
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
		/// �������
		/// </summary>
		[ORFieldMapping("INNER_SORT_ID")]
		[StringLengthValidator(1, 5, MessageTemplate = "�����ŵĳ����������1��5������!")]
		[IntegerRangeValidator(1, 65535, MessageTemplate = "������Ӧ�ô���0��С�ڵ���65535!")]
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
		#region ��������
		/// <summary>
		/// ��DataView��ȡ���ݵ�TaskCategoryCollection��
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
