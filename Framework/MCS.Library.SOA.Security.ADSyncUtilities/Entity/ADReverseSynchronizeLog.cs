using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.Security.ADSyncUtilities.Entity
{
	[ORTableMapping("SC.ADReverseSynchronizeLog")]
	[Serializable]
	public class ADReverseSynchronizeLog
	{
		[ORFieldMapping("LogID", PrimaryKey = true)]
		public string LogID { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		[SqlBehavior(ClauseBindingFlags.Select)]
		public DateTime CreateTime { get; set; }
		public string OperatorID { get; set; }
		public string OperatorName { get; set; }
		public int NumberOfModifiedItems { get; set; }
		public int NumberOfExceptions { get; set; }
		[SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumValue)]
		public ADSynchronizeResult Status { get; set; }
	}


	[Serializable]
	public class ADReverseSynchronizeLogCollection : EditableDataObjectCollectionBase<ADReverseSynchronizeLog>
	{

	}
}
