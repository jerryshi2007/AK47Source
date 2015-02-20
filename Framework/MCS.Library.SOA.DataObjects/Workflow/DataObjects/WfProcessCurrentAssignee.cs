using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	[ORTableMapping("WF.PROCESS_CURRENT_ASSIGNEES")]
	public class WfProcessCurrentAssignee
	{
		public WfProcessCurrentAssignee()
		{
		}

		public WfProcessCurrentAssignee(WfAssignee assignee)
		{
			assignee.NullCheck("assignee");

			this.UserID = assignee.User.ID;
			this.UserName = assignee.User.DisplayName;
			this.UserPath = assignee.User.FullPath;
			this.AssigneeType = assignee.AssigneeType;
			this.Url = assignee.Url;
		}

		[ORFieldMapping("ID", IsIdentity = true, PrimaryKey = true)]
		public int ID
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

		[ORFieldMapping("ACTIVITY_ID")]
		public string ActivityID
		{
			get;
			set;
		}

		[ORFieldMapping("USER_PATH")]
		public string UserPath
		{
			get;
			set;
		}

		[ORFieldMapping("USER_ID")]
		public string UserID
		{
			get;
			set;
		}

		[ORFieldMapping("USER_NAME")]
		public string UserName
		{
			get;
			set;
		}

		[ORFieldMapping("ASSIGNEE_TYPE")]
		public WfAssigneeType AssigneeType
		{
			get;
			set;
		}

		[ORFieldMapping("URL")]
		public string Url
		{
			get;
			set;
		}
	}

	[Serializable]
	public class WfProcessCurrentAssigneeCollection : EditableDataObjectCollectionBase<WfProcessCurrentAssignee>
	{
	}
}
