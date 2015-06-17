using System;
using System.Data.SqlTypes;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.Validation;

namespace MCS.Library.SOA.DataObjects
{
  
	/// <summary>
    /// RELATIVE_LINK_GROUP
    /// </summary>
	[Serializable]
    [XElementSerializable]
    [ORTableMapping("KB.RELATIVE_LINK_GROUP")]
    [TenantRelativeObject]
    public class RelativeLinkGroup
    {  
		/// <summary>
		/// 相关链接组ID
		/// </summary>
        [ORFieldMapping("RELATIVE_LINK_GROUP_ID", PrimaryKey = true)]
        [StringEmptyValidator(MessageTemplate = "相关链接组ID不能为空")]
		public String RelativeLinkGroupID
		{
			get; set;
		}
		/// <summary>
		/// 相关链接分类
		/// </summary>
		[ORFieldMapping("CODE_NAME")]
		[StringEmptyValidator(MessageTemplate = "相关链接分类不能为空")]
		public String CodeName
		{
			get; set;
		}
		/// <summary>
		/// 描述   
		/// </summary>
		[ORFieldMapping("DESCRIPTION")]
		[StringEmptyValidator(MessageTemplate = "描述不能为空")]
		public String Description
		{
			get; set;
		}
		/// <summary>
		/// 是否可用
		/// </summary>
		[ORFieldMapping("ENABLE")]
		[StringEmptyValidator(MessageTemplate = "是否可用不能为空")]
		public String Enable
		{
			get; set;
		}
		/// <summary>
		/// 创建时间
		/// </summary>
		[ORFieldMapping("CREATE_TIME")]
		[DateTimeEmptyValidator(MessageTemplate = "创建时间不能为空")]
		public DateTime CreateTime
		{
			get; set;
		}
		public RelativeLinkGroup()
		{
            RelativeLinkGroupID = string.Empty;
            CodeName = string.Empty;
            Description = string.Empty;
            Enable = string.Empty;
            CreateTime = SqlDateTime.MinValue.Value;
		}
    }
    [Serializable]
    [XElementSerializable]
    public class RelativeLinkGroupCollection : EditableDataObjectCollectionBase<RelativeLinkGroup>
    {
    }
}
