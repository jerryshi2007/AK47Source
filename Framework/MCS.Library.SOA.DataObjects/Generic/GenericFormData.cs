using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.Validation;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[XmlRootMapping("GenericFormData")]
	[XElementSerializable]
	[ORTableMapping("WF.GENERIC_FORM_DATA")]
	[ObjectCompare("ID")]
	public class GenericFormData : WorkflowObjectBase
	{
		[Description("编号")]
		[XmlObjectMapping]
		[ORFieldMapping("RESOURCE_ID", PrimaryKey = true)]
		public override string ID { get; set; }

		[Description("标题")]
		[XmlObjectMapping]
		[ORFieldMapping("SUBJECT")]
		[StringLengthValidator(1, 255, MessageTemplate = "请填写标题，且长度必须小于255个字符")]
		public override string Subject { get; set; }

		private IUser _Creator = null;

		[SubClassORFieldMapping("ID", "CREATOR_ID", IsNullable = false)]
		[SubClassORFieldMapping("DisplayName", "CREATOR_NAME", IsNullable = false)]
		[SubClassType(typeof(OguUser))]
		[XmlObjectMapping]
		public virtual IUser Creator
		{
			get
			{
				return this._Creator;
			}
			set
			{
				this._Creator = (IUser)OguBase.CreateWrapperObject(value);
			}
		}

		[Description("创建时间")]
		[XmlObjectMapping]
		[ORFieldMapping("CREATE_TIME")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.All, DefaultExpression = "getdate()")]
		public virtual DateTime CreateTime { get; set; }

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
	/// 通用表单数据
	/// </summary>
	[Serializable]
	public class GenericFormDataCollection : EditableDataObjectCollectionBase<GenericFormData>
	{
	}
}
