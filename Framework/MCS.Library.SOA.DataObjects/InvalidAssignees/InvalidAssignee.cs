using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[ORTableMapping("WF.INVALID_ASSIGNEES")]
	public class InvalidAssignee
	{
		private string _ID = string.Empty;
		[ORFieldMapping("ID", PrimaryKey = true, IsIdentity = true)]
		public string ID
		{
			get { return this._ID; }
			set { this._ID = value; }
		}

		private string _ProcessID = string.Empty;

		[ORFieldMapping("PROCESS_ID")]
		public string ProcessID
		{
			get { return this._ProcessID; }
			set { this._ProcessID = value; }
		}

		private string _ActivityID = string.Empty;

		[ORFieldMapping("ACTIVITY_ID")]
		public string ActivityID
		{
			get { return this._ActivityID; }
			set { this._ActivityID = value; }
		}

		private string _UserID = string.Empty;

		[ORFieldMapping("USER_ID")]
		public string UserID
		{
			get { return this._UserID; }
			set { this._UserID = value; }
		}

		private string _UserName = string.Empty;

		[ORFieldMapping("USER_NAME")]
		public string UserName
		{
			get { return this._UserName; }
			set { this._UserName = value; }
		}

		private string _UserPath = string.Empty;

		[ORFieldMapping("USER_PATH")]
		public string UserPath
		{
			get { return this._UserPath; }
			set { this._UserPath = value; }
		}

		private DateTime _ActivityStartTime = DateTime.MinValue;

		[ORFieldMapping("ACTIVITY_START_TIME")]
		public DateTime ActivityStartTime
		{
			get { return this._ActivityStartTime; }
			set { this._ActivityStartTime = value; }
		}

		[SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumString)]
		[ORFieldMapping("ACTIVITY_STATUS")]
		public WfActivityStatus Status { get; set; }
	}

	[Serializable]
	public class InvalidAssigneeCollection : EditableDataObjectCollectionBase<InvalidAssignee>
	{

	}
}
