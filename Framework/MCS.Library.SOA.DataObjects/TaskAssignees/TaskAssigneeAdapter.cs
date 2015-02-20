using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 数据适配器，访问TaskAssignee
	/// </summary>
	public class TaskAssigneeAdapter : TaskAssigneeAdapterBase<TaskAssignee, TaskAssigneeCollection, IUser>
	{
		public static readonly TaskAssigneeAdapter Instance = new TaskAssigneeAdapter();

		private TaskAssigneeAdapter()
		{
		}
	}
}
