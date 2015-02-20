using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Archive
{
	/// <summary>
	/// 用户操作日志的归档操作
	/// </summary>
	public class UserOpLogArchiveOperation : IArchiveOperation
	{
		public static readonly UserOpLogArchiveOperation Instance = new UserOpLogArchiveOperation();

		private UserOpLogArchiveOperation()
		{
		}

		#region IArchiveOperation Members

		public void LoadOriginalData(ArchiveBasicInfo info)
		{
			info.Context["UserOperationLogs"] =
				UserOperationLogAdapter.Instance.LoadByResourceID(info.ResourceID);
		}

		public void SaveArchiveData(ArchiveBasicInfo info)
		{
			info.Context.DoAction<UserOperationLogCollection>("UserOperationLogs", logs =>
			{
				UserOperationLogAdapter.Instance.Delete(b => b.AppendItem("RESOURCE_ID", info.ResourceID));

				logs.ForEach(log => UserOperationLogAdapter.Instance.Update(log));
			});
		}

		public void DeleteOriginalData(ArchiveBasicInfo info)
		{
			info.Context.DoAction<UserOperationLogCollection>("UserOperationLogs", logs =>
			{
				UserOperationLogAdapter.Instance.Delete(b => b.AppendItem("RESOURCE_ID", info.ResourceID));
			});
		}

		#endregion
	}
}
