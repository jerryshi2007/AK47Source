using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Principal;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfSaveDataExecutor : WfActivityRelativeExecutorBase
	{
		private bool _SaveUserTasks = true;
		private bool _AutoCommit = true;

		public WfSaveDataExecutor(IWfActivity operatorActivity, IWfActivity targetActivity)
			: base(operatorActivity, targetActivity, WfControlOperationType.Save)
		{
		}

		public bool SaveUserTasks
		{
			get
			{
				return this._SaveUserTasks;
			}
			set
			{
				this._SaveUserTasks = value;
			}
		}

		/// <summary>
		/// 是否自动将流程状态设置为提交状态，默认是True，如果是False，则忽略流程原来的Committed属性
		/// </summary>
		public bool AutoCommit
		{
			get
			{
				return this._AutoCommit;
			}
			set
			{
				this._AutoCommit = value;
			}
		}

		protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
		{
			if (WfRuntime.ProcessContext != null)
				WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(TargetActivity.Process);

			if (TargetActivity != null && this._AutoCommit)
				TargetActivity.Process.Committed = true;
		}

		protected override void OnPrepareMoveToTasks(WfExecutorDataContext dataContext)
		{
			if (SaveUserTasks)
			{
				var userTasks = UserTaskAdapter.Instance.LoadUserTasks(builder => builder.AppendItem("ACTIVITY_ID",
								this.TargetActivity.ID));

				if (userTasks == null || userTasks.Count == 0)
					userTasks = (UserTaskCollection)TargetActivity.Context["UserTasks"];

				if (userTasks != null)
				{
					UserTaskCollection filteredTasks = FilterCurrentUserTasks(userTasks);

					WfRuntime.ProcessContext.DeletedUserTasks.CopyFrom(filteredTasks);
					WfRuntime.ProcessContext.MoveToUserTasks.CopyFrom(filteredTasks);
					WfRuntime.ProcessContext.MoveToUserTasks.DistinctByActivityUserAndStatus();

					base.OnPrepareMoveToTasks(dataContext);
				}
				else
					base.OnPrepareMoveToTasks(dataContext);
			}
		}

		protected override void OnPrepareNotifyTasks(WfExecutorDataContext dataContext)
		{
			UserTaskCollection notifyTasks = (UserTaskCollection)TargetActivity.Context["NotifyTasks"];

			if (notifyTasks != null)
			{
				UserTaskCollection filteredTasks = FilterCurrentUserTasks(notifyTasks);

				WfRuntime.ProcessContext.DeletedUserTasks.CopyFrom(filteredTasks);
				WfRuntime.ProcessContext.NotifyUserTasks.CopyFrom(filteredTasks);
				WfRuntime.ProcessContext.NormalizeTaskTitles();

				base.OnPrepareNotifyTasks(dataContext);
			}
			else
				base.OnPrepareNotifyTasks(dataContext);
		}

		protected override void OnSaveApplicationData(WfExecutorDataContext dataContext)
		{
			if (SaveUserTasks)
			{
				UserTaskAdapter.Instance.DeleteUserTasks(WfRuntime.ProcessContext.DeletedUserTasks);
				UserTaskAdapter.Instance.SendUserTasks(WfRuntime.ProcessContext.MoveToUserTasks);
				UserTaskAdapter.Instance.SendUserTasks(WfRuntime.ProcessContext.NotifyUserTasks);
			}

			base.OnSaveApplicationData(dataContext);
		}

		private static UserTaskCollection FilterCurrentUserTasks(UserTaskCollection sourceTasks)
		{
			UserTaskCollection result = new UserTaskCollection();

			foreach (UserTask task in sourceTasks)
			{
				if (string.Compare(task.SourceID, DeluxeIdentity.CurrentUser.ID, true) == 0)
				{
					result.Add(task);
				}
			}
			return result;
		}
	}
}
