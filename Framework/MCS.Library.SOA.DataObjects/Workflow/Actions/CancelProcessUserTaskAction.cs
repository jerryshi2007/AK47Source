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
	/// 取消流程操作的UserTask生成器
	/// </summary>
	[Serializable]
	public class CancelProcessUserTaskAction : UserTaskActionBase
	{
		public override void PrepareAction(WfActionParams actionParams)
		{
			IWfActivity currentActivity = WfRuntime.ProcessContext.CurrentActivity;

			if (currentActivity != null)
			{
				//找到当前流程所有的待办，然后转为已办
				UserTaskCollection currentProcessTasks =
						UserTaskAdapter.Instance.LoadUserTasks(builder => builder.AppendItem("PROCESS_ID",
							currentActivity.Process.ID));

				WfRuntime.ProcessContext.DeletedUserTasks.CopyFrom(currentProcessTasks);

				IList<UserTask> toAccomplishedTasks = currentProcessTasks.FindAll(u => u.Status == TaskStatus.Ban);

				//将待办转到已办中，便于事后恢复
				toAccomplishedTasks.ForEach(u => LeaveActivityUserTaskAction.ChangeUserTaskToAccomplishedTasks(currentActivity, u));

				WfRuntime.ProcessContext.AccomplishedUserTasks.CopyFrom(toAccomplishedTasks);

				//暂时对当前活动的处理人发送通知
				UserTaskCollection notifyTasks = BuildUserNotifiesFromActivity(currentActivity);
				AppendResourcesToNotifiers(currentActivity, notifyTasks, currentActivity.Descriptor.Process.CancelEventReceivers);

				foreach (UserTask task in notifyTasks)
				{
					task.Status = TaskStatus.Yue;
					task.TaskTitle = Translator.Translate(Define.DefaultCulture,
						currentActivity.Process.Descriptor.Properties.GetValue("DefaultCancelTaskPrefix", "流程被取消:")) + task.TaskTitle;
				}

				WfRuntime.ProcessContext.NotifyUserTasks.CopyFrom(notifyTasks);

				WfRuntime.ProcessContext.Acl.CopyFrom(notifyTasks.ToAcl());
				WfRuntime.ProcessContext.AbortedProcesses.AddOrReplace(currentActivity.Process);
				WfRuntime.ProcessContext.FireCancelProcessPrepareAction();
			}
		}

		public override void PersistAction(WfActionParams actionParams)
		{
			WfRuntime.ProcessContext.FireCancelProcessPersistAction();

			WfRuntime.ProcessContext.NotifyUserTasks.DistinctByActivityUserAndStatus();
			UserTaskAdapter.Instance.SendUserTasks(WfRuntime.ProcessContext.NotifyUserTasks);
			UserTaskAdapter.Instance.SetUserTasksAccomplished(WfRuntime.ProcessContext.AccomplishedUserTasks);
			UserTaskAdapter.Instance.DeleteUserTasks(WfRuntime.ProcessContext.DeletedUserTasks);

			AppCommonInfoAdapter.Instance.UpdateProcessStatus(WfRuntime.ProcessContext.AffectedProcesses.FindAll(p => p.IsApprovalRootProcess));

			WfPendingActivityInfoAdapter.Instance.DeleteByProcesses(WfRuntime.ProcessContext.AbortedProcesses);
			WfPendingActivityInfoAdapter.Instance.DeleteByProcesses(WfRuntime.ProcessContext.ClosedProcesses);

			this.ClearCache();
		}

		public override void ClearCache()
		{
			WfRuntime.ProcessContext.AccomplishedUserTasks.Clear();
			WfRuntime.ProcessContext.NotifyUserTasks.Clear();
			WfRuntime.ProcessContext.DeletedUserTasks.Clear();
			WfRuntime.ProcessContext.AbortedProcesses.Clear();
			WfRuntime.ProcessContext.ClosedProcesses.Clear();
		}
	}
}
