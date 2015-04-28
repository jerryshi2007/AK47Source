using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects.Workflow.Actions
{
	[Serializable]
	public class EnterActivityUserTaskAction : UserTaskActionBase
	{
		public override void PrepareAction(WfActionParams actionParams)
		{
			IWfActivity currentActivity = WfRuntime.ProcessContext.CurrentActivity;

			if (currentActivity != null)
			{
				if (WfRuntime.ProcessContext.TargetActivityCanMoveTo && currentActivity.Status == WfActivityStatus.Running)
				{
					UserTaskCollection userTasks = BuildUserTasksFromActivity(currentActivity);

					UserTaskCollection notifyTasks = new UserTaskCollection();

					AppendResourcesToNotifiers(currentActivity, notifyTasks, currentActivity.Descriptor.EnterEventReceivers);

					foreach (UserTask task in notifyTasks)
					{
						task.Status = TaskStatus.Yue;
						task.TaskTitle = task.TaskTitle;
					}

					WfRuntime.ProcessContext.Acl.CopyFrom(notifyTasks.ToAcl());

					//当从数据库中加载流程时，LoadingType为DataLoadingType.External。
					if (CanAutoSendUserTask(currentActivity))
					{
						WfRuntime.ProcessContext.MoveToUserTasks.CopyFrom(userTasks);
						WfRuntime.ProcessContext.NotifyUserTasks.CopyFrom(notifyTasks);
					}
					else
					{
						currentActivity.Context["UserTasks"] = userTasks;
						currentActivity.Context["NotifyTasks"] = notifyTasks;
					}

					if (currentActivity.Status == WfActivityStatus.Completed)
						WfRuntime.ProcessContext.ClosedProcesses.AddOrReplace(currentActivity.Process);
				}
				else
				{
					WfRuntime.ProcessContext.PendingActivities.Add(currentActivity);
				}

				WfRuntime.ProcessContext.FireEnterActivityPrepareAction();
			}
		}

		public override void PersistAction(WfActionParams actionParams)
		{
			WfRuntime.ProcessContext.FireEnterActivityPersistAction();

			WfRuntime.ProcessContext.MoveToUserTasks.DistinctByActivityUserAndStatus();
			UserTaskAdapter.Instance.SendUserTasks(WfRuntime.ProcessContext.MoveToUserTasks);

			WfRuntime.ProcessContext.NotifyUserTasks.DistinctByActivityUserAndStatus();
			UserTaskAdapter.Instance.SendUserTasks(WfRuntime.ProcessContext.NotifyUserTasks);

			WfPendingActivityInfoAdapter.Instance.DeleteByProcesses(WfRuntime.ProcessContext.AbortedProcesses);
			WfPendingActivityInfoAdapter.Instance.DeleteByProcesses(WfRuntime.ProcessContext.ClosedProcesses);

			AppCommonInfoAdapter.Instance.UpdateProcessStatus(WfRuntime.ProcessContext.AffectedProcesses.FindAll(p => p.IsApprovalRootProcess));

			WfRuntime.ProcessContext.PendingActivities.ForEach(act =>
			{
				WfPendingActivityInfo data = new WfPendingActivityInfo(act);

				WfPendingActivityInfoAdapter.Instance.Update(data);
			});

			this.ClearCache();
		}

		public override void ClearCache()
		{
			WfRuntime.ProcessContext.MoveToUserTasks.Clear();
			WfRuntime.ProcessContext.NotifyUserTasks.Clear();
			WfRuntime.ProcessContext.AbortedProcesses.Clear();
			WfRuntime.ProcessContext.ClosedProcesses.Clear();
			WfRuntime.ProcessContext.PendingActivities.Clear();
		}

		private static bool CanAutoSendUserTask(IWfActivity activity)
		{
			bool result = activity.Descriptor.Properties.GetValue("AutoSendUserTask", true);

			WfAutoSendUserTaskMode autoSendMode = activity.Descriptor.Properties.GetValue("AutoSendUserTaskMode", WfAutoSendUserTaskMode.ByDefault);

			switch (autoSendMode)
			{
				case WfAutoSendUserTaskMode.ByDefault:
					//当从数据库中加载流程时，LoadingType为DataLoadingType.External。
					//result = result ||
					//        activity.Process.LoadingType == DataLoadingType.External ||
					//        activity.Process.EntryInfo != null;
					result = result || activity.Process.Committed;
					break;
				case WfAutoSendUserTaskMode.ByAutoSendUserTaskProperty:
					//在不是Clone活动的情况下和AutoSendUserTask相同，否则返回true
					if (activity.Descriptor.ClonedKey.IsNotEmpty())
						result = true;

					break;
			}

			return result;
		}
	}
}
