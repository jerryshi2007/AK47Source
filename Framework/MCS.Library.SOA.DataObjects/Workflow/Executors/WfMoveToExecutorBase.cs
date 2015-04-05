using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 和流转相关的Executor的基类
	/// </summary>
	public abstract class WfMoveToExecutorBase : WfActivityRelativeExecutorBase
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="operatorActivity"></param>
		/// <param name="targetActivity"></param>
		/// <param name="operationType"></param>
		protected WfMoveToExecutorBase(IWfActivity operatorActivity, IWfActivity targetActivity, WfControlOperationType operationType)
			: base(operatorActivity, targetActivity, operationType)
		{
		}

		/// <summary>
		/// 重载准备待办的方法，调整所有待办中，涉及到的Activity的Assignee的Url
		/// </summary>
		/// <param name="dataContext"></param>
		/// <param name="tasks"></param>
		protected override void OnPrepareMoveToTasks(WfExecutorDataContext dataContext)
		{
			this.TargetActivity.Process.Committed = true;

			base.OnPrepareMoveToTasks(dataContext);

			SyncUrlsInAssigneesFromTasks(dataContext.MoveToTasks);
		}

		internal static void SyncUrlsInAssigneesFromTasks(UserTaskCollection tasks)
		{
			foreach (UserTask task in tasks)
			{
				IWfActivity activity = WfRuntime.GetProcessByActivityID(task.ActivityID).Activities[task.ActivityID];

				SyncActivityAssigneesUrl(task, activity.Assignees);
			}
		}

		private static void SyncActivityAssigneesUrl(UserTask task, WfAssigneeCollection assignees)
		{
			foreach (WfAssignee assignee in assignees)
			{
				if (assignee.User != null && string.Compare(assignee.User.ID, task.SendToUserID, true) == 0)
					assignee.Url = task.Url;
			}
		}
	}
}
