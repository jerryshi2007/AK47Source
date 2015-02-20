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
	/// <summary>
	/// 任务分派人的数据实体
	/// </summary>
	[Serializable]
	[ORTableMapping("WF.TASK_ASSIGNEES")]
	public class TaskAssignee : TaskAssigneeBase<IUser>
	{
		[SubClassORFieldMapping("ID", "ASSIGNEE_ID", IsNullable = false)]
		[SubClassORFieldMapping("DisplayName", "ASSIGNEE_NAME", IsNullable = false)]
		[SubClassType(typeof(OguUser))]
		public override IUser Assignee
		{
			get
			{
				return base.Assignee;
			}
			set
			{
				base.Assignee = (IUser)OguUser.CreateWrapperObject(value);
			}
		}
	}

	/// <summary>
	/// 任务分派人的集合
	/// </summary>
	[Serializable]
	public class TaskAssigneeCollection : TaskAssigneeCollectionBase<TaskAssignee, IUser>
	{
	}
}
