using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.Security.ADSyncUtilities.Entity
{
	[ORTableMapping("SC.ADSynchronizeLog")]
	[Serializable]
	public class ADSynchronizeLog
	{
		[ORFieldMapping("LogID", PrimaryKey = true)]
		public string LogID { get; set; }
		public string SynchronizeID { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public string OperatorID { get; set; }
		public string OperatorName { get; set; }
		[SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumValue)]
		public ADSynchronizeResult SynchronizeResult { get; set; }
		public int ExceptionCount { get; set; }
		[SqlBehavior(ClauseBindingFlags.Select)]
		public DateTime CreateTime { get; set; }
		public int AddingItemCount { get; set; }
		public int DeletingItemCount { get; set; }
		public int ModifyingItemCount { get; set; }
		public int AddedItemCount { get; set; }
		public int DeletedItemCount { get; set; }
		public int ModifiedItemCount { get; set; }
	}

	[Serializable]
	public class ADSynchronizeLogCollection : EditableDataObjectCollectionBase<ADSynchronizeLog>
	{

	}
}