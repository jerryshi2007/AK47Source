using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security
{
	[Serializable]
	public class HistoryEntry
	{
		public string Name { get; set; }

		public string DisplayName { get; set; }

		public DateTime VersionStartTime { get; set; }

		public DateTime VersionEndTime { get; set; }

		public SchemaObjectStatus Status { get; set; }

		public string CreatorID { get; set; }

		public string CreatorName { get; set; }
	}

	public class HistoryEntryCollection : EditableDataObjectCollectionBase<HistoryEntry>
	{

	}

	[Serializable]
	public class ReferenceHistoryEntry : HistoryEntry
	{
		public string ParentID { get; set; }

		public string ObjectID { get; set; }

		public bool IsDefault { get; set; }
	}

	public class ReferenceHistoryEntryCollection : EditableDataObjectCollectionBase<ReferenceHistoryEntry>
	{

	}

	[Serializable]
	public class MembershipHistoryEntry : HistoryEntry
	{
		public string MemberID { get; set; }

		public string ContainerID { get; set; }
	}

	public class MembershipHistoryEntryCollection : EditableDataObjectCollectionBase<MembershipHistoryEntry>
	{

	}
}
