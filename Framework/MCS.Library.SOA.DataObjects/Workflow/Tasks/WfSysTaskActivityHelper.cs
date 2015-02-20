using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow.Tasks
{
	/// <summary>
	/// 构造任务活动的辅助方法
	/// </summary>
	public static class WfSysTaskActivityHelper
	{
		/// <summary>
		/// 构造一个任务活动
		/// </summary>
		/// <param name="sysTaskProcess"></param>
		/// <param name="index"></param>
		/// <param name="name"></param>
		/// <param name="task"></param>
		/// <returns></returns>
		public static SysTaskActivity CreateActivity(this SysTaskProcess sysTaskProcess, int index, string name, SysTask task)
		{
			sysTaskProcess.NullCheck("sysTaskProcess");

			SysTaskActivity activity = new SysTaskActivity(task);

			activity.ID = UuidHelper.NewUuidString();
			activity.ProcessID = sysTaskProcess.ID;
			activity.Sequence = index;
			activity.Name = name;

			SysTaskProcessRuntime.ProcessContext.AffectedActivities.AddOrReplace(activity);

			return activity;
		}

		/// <summary>
		/// 构造一个推出维护模式的任务活动
		/// </summary>
		/// <param name="sysTaskProcess"></param>
		/// <param name="processID"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static SysTaskActivity CreateExitMaintainingActivity(this SysTaskProcess sysTaskProcess, string processID, int index)
		{
			SysTask task = ExitMaintainingStatusTask.CreateTask(string.Empty, processID, true);

			SysTaskActivity activity = CreateActivity(sysTaskProcess, index, processID, task);

			activity.Name = string.Format("退出ID为{0}流程的维护状态", processID);

			return activity;
		}

		/// <summary>
		/// 创建一个分发作废流程任务流程的任务活动
		/// </summary>
		/// <param name="sysTaskProcess"></param>
		/// <param name="ownerActivityID"></param>
		/// <param name="cancelAllBranchProcesses"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static SysTaskActivity CreateDispatchCancelBranchesProcessActivity(this SysTaskProcess sysTaskProcess, string ownerActivityID, bool cancelAllBranchProcesses, int index)
		{
			string activityID = UuidHelper.NewUuidString();

			DispatchCancelBranchProcessesTask task = DispatchCancelBranchProcessesTask.CreateTask(activityID, ownerActivityID, cancelAllBranchProcesses);

			SysTaskActivity activity = WfSysTaskActivityHelper.CreateActivity(
				sysTaskProcess,
				index,
				string.Format("分派作废活动ID为{0}的分支流程的任务流程活动", ownerActivityID, index),
				task);

			activity.ID = activityID;

			SysTaskProcessRuntime.ProcessContext.AffectedActivities.AddOrReplace(activity);

			return activity;
		}

		/// <summary>
		/// 创建一个包含了作废流程任务的任务流程活动
		/// </summary>
		/// <param name="sysTaskProcess"></param>
		/// <param name="processID"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static SysTaskActivity CreateCancelProcessActivity(this SysTaskProcess sysTaskProcess, string processID, int index)
		{
			CancelProcessTask task = CancelProcessTask.CreateTask(processID, false);

			SysTaskActivity activity = WfSysTaskActivityHelper.CreateActivity(
				sysTaskProcess,
				index,
				string.Format("作废ID为{0}的流程的任务流程活动",
				processID, index), task);

			SysTaskProcessRuntime.ProcessContext.AffectedActivities.AddOrReplace(activity);

			return activity;
		}

		/// <summary>
		/// 创建一个包含了撤回流程任务的任务流程活动
		/// </summary>
		/// <param name="sysTaskProcess"></param>
		/// <param name="processID"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static SysTaskActivity CreateWithdrawProcessActivity(this SysTaskProcess sysTaskProcess, string processID, int index)
		{
			string activityID = UuidHelper.NewUuidString();

			WithdrawProcessTask task = WithdrawProcessTask.CreateTask(processID, false);

			SysTaskActivity activity = WfSysTaskActivityHelper.CreateActivity(
				sysTaskProcess,
				index,
				string.Format("撤回ID为{0}的流程的任务流程活动",
				processID, index), task);

			SysTaskProcessRuntime.ProcessContext.AffectedActivities.AddOrReplace(activity);

			return activity;
		}
	}
}
