
using System;
using System.Data.SqlTypes;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.Validation;

namespace MCS.Library.SOA.DataObjects
{   
	/// <summary>
    /// 提示表
    /// </summary>
	[Serializable]
    [XElementSerializable]
    [ORTableMapping("KB.TIP")]
    public class Tip
    {  
		/// <summary>
		/// 提示ID
		/// </summary>
        [ORFieldMapping("TIP_ID", PrimaryKey = true)]
        [StringEmptyValidator(MessageTemplate = "提示信息ID不能为空")]
		public String TipID
		{
			get; set;
		}
		/// <summary>
		/// 提示的名称
		/// </summary>
		[ORFieldMapping("CODE_NAME")]
        [StringEmptyValidator(MessageTemplate = "提示的名称不能为空")]
		public String CodeName
		{
			get; set;
		}
		/// <summary>
		/// 区域
		/// </summary>
		[ORFieldMapping("CULTURE")]
        [StringEmptyValidator(MessageTemplate = "区域不能为空")]
		public String Culture
		{
			get; set;
		}
		/// <summary>
		/// TIP的内容
		/// </summary>
		[ORFieldMapping("CONTENT")]
        [StringEmptyValidator(MessageTemplate = "提示的内容不能为空")]
		public String Content
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
		public Tip()
		{
            TipID = string.Empty;
            CodeName = string.Empty;
            Culture = string.Empty;
            Content = string.Empty;
            Enable = string.Empty;
            CreateTime = SqlDateTime.MinValue.Value;
		}
    }
    [Serializable]
    [XElementSerializable]
    public class TipCollection : EditableDataObjectCollectionBase<Tip>
    {
    }
}

