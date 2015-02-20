using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Transactions;
using MCS.Library.Data;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 执行任务流程活动任务的执行器
	/// </summary>
	public class ExecuteSysTaskActivityTaskExecutor : SyncSysTaskExecutorBase
	{
		protected override void OnExecute(SysTask task)
		{
			ExecuteSysTaskActivityTask activityTask = new ExecuteSysTaskActivityTask(task);

			SysTaskProcess process = SysTaskProcessRuntime.GetProcessByActivityID(activityTask.ActivityID);

			process.CurrentActivity.ExecuteTask();

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				SysTaskProcessRuntime.Persist();

				//没有异常，则继续流转
				if (process.Status == SysTaskProcessStatus.Running)
				{
					process = SysTaskProcessRuntime.GetProcessByActivityID(activityTask.ActivityID);

					AutoMoveToNextActivity(process.CurrentActivity);

					if (process.OwnerActivity != null)
						AutoMoveToNextActivity(process.OwnerActivity);
				}

				scope.Complete();
			}

			if (process.Status == SysTaskProcessStatus.Aborted)
				throw new ApplicationException(process.CurrentActivity.StatusText);
		}

		/// <summary>
		/// 如果能够流转，自动流转到下一个活动
		/// </summary>
		/// <param name="activity"></param>
		private static void AutoMoveToNextActivity(SysTaskActivity activity)
		{
			if (activity.CanMoveTo())
			{
				SysTaskActivity nextActivity = activity.Process.MoveToNextActivity();

				SysTaskProcessRuntime.Persist();

				if (activity.Process.Status == SysTaskProcessStatus.Running)
					ExecuteSysTaskActivityTask.SendTask(nextActivity);
			}
		}
	}
}
