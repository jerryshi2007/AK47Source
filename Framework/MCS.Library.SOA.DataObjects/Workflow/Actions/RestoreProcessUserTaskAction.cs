using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow.Actions
{
	/// <summary>
	/// 恢复被取消的流程的待办事项
	/// </summary>
	[Serializable]
	public class RestoreProcessUserTaskAction : UserTaskActionBase
	{
		public override void PrepareAction(WfActionParams actionParams)
		{
			IWfActivity currentActivity = WfRuntime.ProcessContext.CurrentActivity;

			if (currentActivity != null)
			{
				//找到当前流程所有的待办，然后删除，主要是为了删除通知
				UserTaskCollection currentActivityTasks =
						UserTaskAdapter.Instance.LoadUserTasks(builder => builder.AppendItem("ACTIVITY_ID",
							currentActivity.ID));

				WfRuntime.ProcessContext.DeletedUserTasks.CopyFrom(currentActivityTasks);

				//找到当前流程所有的已办，然后转为待办
				UserTaskCollection currentActivityAccomplishedTasks =
					UserTaskAdapter.Instance.GetUserAccomplishedTasks(UserTaskIDType.ActivityID, UserTaskFieldDefine.All, false, currentActivity.ID);

				WfRuntime.ProcessContext.MoveToUserTasks.CopyFrom(currentActivityAccomplishedTasks);

				if (currentActivity.Status == WfActivityStatus.Pending)
					WfRuntime.ProcessContext.PendingActivities.Add(currentActivity);

				WfRuntime.ProcessContext.FireRestoreProcessPrepareAction();
			}
		}

		public override void PersistAction(WfActionParams actionParams)
		{
			WfRuntime.ProcessContext.FireRestoreProcessPersistAction();

			UserTaskAdapter.Instance.SendUserTasks(WfRuntime.ProcessContext.MoveToUserTasks);
			UserTaskAdapter.Instance.DeleteUserTasks(WfRuntime.ProcessContext.DeletedUserTasks);

			AppCommonInfoAdapter.Instance.UpdateProcessStatus(WfRuntime.ProcessContext.AffectedProcesses.FindAll(p => p.IsApprovalRootProcess));

			WfRuntime.ProcessContext.PendingActivities.ForEach(act =>
			{
				WfPendingActivityInfo data = new WfPendingActivityInfo(act);

				WfPendingActivityInfoAdapter.Instance.Update(data);
			});

			ClearCache();
		}

		public override void ClearCache()
		{
			WfRuntime.ProcessContext.MoveToUserTasks.Clear();
			WfRuntime.ProcessContext.DeletedUserTasks.Clear();
			WfRuntime.ProcessContext.PendingActivities.Clear();
		}
	}
}
