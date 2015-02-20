using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[ORTableMapping("WF.PENDING_ACTIVITIES")]
	[Serializable]
	public class WfPendingActivityInfo
	{
		public WfPendingActivityInfo()
		{

		}

		public WfPendingActivityInfo(IWfActivity activity)
		{
			activity.NullCheck("activity");

			this.ActivityID = activity.ID;
			this.ProcessID = activity.Process.ID;
			this.ApplicationName = activity.Descriptor.Process.ApplicationName;
			this.ProgramName = activity.Descriptor.Process.ProgramName;
		}

		[ORFieldMapping("ACTIVITY_ID", PrimaryKey = true)]
		public string ActivityID
		{
			get;
			set;
		}

		[ORFieldMapping("PROCESS_ID")]
		public string ProcessID
		{
			get;
			set;
		}

		[ORFieldMapping("APPLICATION_NAME")]
		public string ApplicationName
		{
			get;
			set;
		}

		[ORFieldMapping("PROGRAM_NAME")]
		public string ProgramName
		{
			get;
			set;
		}

		[ORFieldMapping("CREATE_TIME")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Select | ClauseBindingFlags.Where)]
		public DateTime CreateTime
		{
			get;
			set;
		}
	}

	public class WfPendingActivityInfoCollection : SerializableEditableKeyedDataObjectCollectionBase<string, WfPendingActivityInfo>
	{
		protected override string GetKeyForItem(WfPendingActivityInfo item)
		{
			return item.ActivityID;
		}
	}
}
