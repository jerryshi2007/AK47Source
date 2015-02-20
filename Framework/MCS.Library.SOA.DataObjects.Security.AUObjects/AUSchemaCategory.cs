using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	/// <summary>
	/// 表示Schema分类
	/// </summary>
	[ORTableMapping("SC.Categories")]
	public class AUSchemaCategory : ISCStatusObject, IVersionDataObject
	{
		/// <summary>
		/// 分类ID
		/// </summary>
		[ORFieldMapping("ID")]
		public string ID { get; set; }
		/// <summary>
		/// 分类名称
		/// </summary>
		[ORFieldMapping("Name")]
		public string Name { get; set; }
		/// <summary>
		/// 分类的版本开始时间
		/// </summary>
		[ORFieldMapping("VersionStartTime")]
		public DateTime VersionStartTime { get; set; }
		/// <summary>
		/// 分类的版本结束时间
		/// </summary>
		[ORFieldMapping("VersionEndTime")]
		public DateTime VersionEndTime { get; set; }
		/// <summary>
		/// 分类的状态
		/// </summary>
		[ORFieldMapping("Status")]
		public SchemaObjectStatus Status { get; set; }
		/// <summary>
		/// 分类的全路径
		/// </summary>
		[ORFieldMapping("FullPath")]
		public string FullPath { get; set; }
		/// <summary>
		/// 上级分类的ID
		/// </summary>
		[ORFieldMapping("ParentID")]
		public string ParentID { get; set; }

		[ORFieldMapping("Rank")]
		public int Rank { get; set; }
	}

	[Serializable]
	public class AUSchemaCategoryCollection : SerializableEditableKeyedDataObjectCollectionBase<string, AUSchemaCategory>
	{
		protected override string GetKeyForItem(AUSchemaCategory item)
		{
			return item.ID;
		}
	}
}
