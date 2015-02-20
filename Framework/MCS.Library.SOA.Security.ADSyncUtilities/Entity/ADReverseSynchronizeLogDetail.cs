using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.Security.ADSyncUtilities.Entity
{
	[ORTableMapping("SC.ADReverseSynchronizeLogDetail")]
	public class ADReverseSynchronizeLogDetail
	{
		[ORFieldMapping("LogDetailID", PrimaryKey = true)]
		public string LogDetailID { get; set; }
		public string LogID { get; set; }
		public string SCObjectID { get; set; }
		public DateTime CreateTime { get; set; }
		public string ADObjectID { get; set; }
		public string ADObjectName { get; set; }
		public string Summary { get; set; }
		public string Detail { get; set; }
	}

	public class ADReverseSynchronizeLogDetailCollection : EditableDataObjectCollectionBase<ADReverseSynchronizeLogDetail>
	{

	}
}
