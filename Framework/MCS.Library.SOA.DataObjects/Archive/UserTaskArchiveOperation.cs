using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Archive
{
	/// <summary>
	/// UserTask的归档操作，它实际上只支持删除操作。
	/// </summary>
	public class UserTaskArchiveOperation : IArchiveOperation
	{
		public static readonly UserTaskArchiveOperation Instance = new UserTaskArchiveOperation();

		private UserTaskArchiveOperation()
		{
		}

		#region IArchiveOperation Members

		public void LoadOriginalData(ArchiveBasicInfo info)
		{
			info.Context["UserTasks"] = UserTaskAdapter.Instance.LoadUserTasks(b => b.AppendItem("RESOURCE_ID", info.ResourceID));
		}

		public void SaveArchiveData(ArchiveBasicInfo info)
		{
		}

		public void DeleteOriginalData(ArchiveBasicInfo info)
		{
			info.Context.DoAction<UserTaskCollection>("UserTasks", tasks =>
			{
				UserTaskAdapter.Instance.DeleteUserTasks(tasks);
			});
		}

		#endregion
	}
}
