using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	using MCS.Library.OGUPermission;

	public class TaskAssignedObjectAdapter : TaskAssigneeAdapterBase<TaskAssignedObject, TaskAssignedObjectCollection, IOguObject>
	{
		public static readonly TaskAssignedObjectAdapter Instance = new TaskAssignedObjectAdapter();

		private TaskAssignedObjectAdapter()
		{
		}

		public override TaskAssignedObject CreateNewData(DataRow row)
		{
			SchemaType type = (SchemaType)Enum.Parse(typeof(SchemaType), row["ASSIGNEE_TYPE"].ToString(), true);

			IOguObject obj = OguBase.CreateWrapperObject(row["ASSIGNEE_ID"].ToString(), type);

			TaskAssignedObject result = new TaskAssignedObject();

			result.Assignee = obj;

			return result;
		}
	}
}
