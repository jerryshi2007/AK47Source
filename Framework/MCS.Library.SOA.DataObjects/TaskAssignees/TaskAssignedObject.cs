using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[ORTableMapping("WF.TASK_ASSIGNED_OBJECTS")]
	public class TaskAssignedObject : TaskAssigneeBase<IOguObject>
	{
		[SubClassORFieldMapping("ID", "ASSIGNEE_ID", IsNullable = false)]
		[SubClassORFieldMapping("ObjectType", "ASSIGNEE_TYPE", IsNullable = false)]
		[SubClassORFieldMapping("DisplayName", "ASSIGNEE_NAME", IsNullable = false)]
        [SubClassType(typeof(OguBase))]
		public override IOguObject Assignee
		{
			get
			{
				return base.Assignee;
			}
			set
			{
				base.Assignee = OguUser.CreateWrapperObject(value);
			}
		}
	}

	[Serializable]
	public class TaskAssignedObjectCollection : TaskAssigneeCollectionBase<TaskAssignedObject, IOguObject>
	{
	}
}
