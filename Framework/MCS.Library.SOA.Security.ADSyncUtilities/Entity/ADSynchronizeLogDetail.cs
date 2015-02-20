using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.Security.ADSyncUtilities.Entity
{

	[ORTableMapping("SC.ADSynchronizeLogDetail")]
	public class ADSynchronizeLogDetail
	{
		[ORFieldMapping("LogDetailID", PrimaryKey = true)]
		public string LogDetailID { get; set; }
		public string SynchronizeID { get; set; }
		public string ActionName { get; set; }
		public string SCObjectID { get; set; }
		public string SCObjectName { get; set; }
		public string ADObjectID { get; set; }
		public string ADObjectName { get; set; }
		public string Detail { get; set; }
		[SqlBehavior(ClauseBindingFlags.Select)]
		public DateTime CreateTime { get; set; }
	}

	public class ADSynchronizeLogDetailCollection : EditableDataObjectCollectionBase<ADSynchronizeLogDetail>
	{

	}
}