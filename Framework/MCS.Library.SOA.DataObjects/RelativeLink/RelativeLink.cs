
using System;
using System.Data.SqlTypes;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.Validation;

namespace MCS.Library.SOA.DataObjects
{   
		/// <summary>
    /// 相关链接
    /// </summary>
	[Serializable]
    [XElementSerializable]
    [ORTableMapping("KB.RELATIVE_LINK")]
    public class RelativeLink
    {  
		/// <summary>
		/// 相关链接ID
		/// </summary>
		[ORFieldMapping("RELATIVE_LINK_ID",PrimaryKey = true)]		
		public String RelativeLinkID
		{
			get; set;
		}
		/// <summary>
		/// 相关链接组ID
		/// </summary>
		[ORFieldMapping("RELATIVE_LINK_GROUP_ID")]
        [StringEmptyValidator(MessageTemplate = "相关链接组ID不能为空")]
		public String RelativeLinkGroupID
		{
			get; set;
		}
		/// <summary>
		/// 相关链接的名称
		/// </summary>
		[ORFieldMapping("CODE_NAME")]
        [StringEmptyValidator(MessageTemplate = "相关链接的名称不能为空")]
		public String CodeName
		{
			get; set;
		}
		/// <summary>
		/// RELATIVE_LINK_GROUP_CODE_NAME
		/// </summary>
		[ORFieldMapping("RELATIVE_LINK_GROUP_CODE_NAME")]
        [StringEmptyValidator(MessageTemplate = "相关链接的组名称不能为空")]
		public String RelativeLinkGroupCodeName
		{
			get; set;
		}
		/// <summary>
		/// 链接
		/// </summary>
		[ORFieldMapping("LINK")]
        [StringEmptyValidator(MessageTemplate = "相关链接不能为空")]
        [RegexValidator(@"https?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", MessageTemplate = "链接格式不正确")]
		public String Link
		{
			get; set;
		}
		/// <summary>
		/// 是否可用
		/// </summary>
		[ORFieldMapping("ENABLE")]		
		public String Enable
		{
			get; set;
		}
		/// <summary>
		/// 创建时间
		/// </summary>
		[ORFieldMapping("CREATE_TIME")]		
		public DateTime CreateTime
		{
			get; set;
		}
		public RelativeLink()
		{
            RelativeLinkID = string.Empty;
            RelativeLinkGroupID = string.Empty;
            CodeName = string.Empty;
            RelativeLinkGroupCodeName = string.Empty;
            Link = string.Empty;
            Enable = string.Empty;
            CreateTime = SqlDateTime.MinValue.Value;
		}
    }
    [Serializable]
    [XElementSerializable]
    public class RelativeLinkCollection : EditableDataObjectCollectionBase<RelativeLink>
    {
    }
}

