using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow.Actions;
namespace MCS.Library.SOA.DataObjects.Workflow
{
	public sealed class WfReplaceAssigneesExecutor : WfExecutorBase
	{
		public IEnumerable<IUser> TargetAssignees
		{
			get;
			private set;
		}

		private OguDataCollection<IUser> _OriginalAssignees = new OguDataCollection<IUser>();

		public IEnumerable<IUser> OriginalAssignees
		{
			get
			{
				return this._OriginalAssignees;
			}
		}

		public IWfActivity TargetActivity
		{
			get;
			private set;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="operatorActivity">操作人所在的Activity，可以为null</param>
		/// <param name="targetActivity"></param>
		/// <param name="originalAssignee"></param>
		/// <param name="targetAssignees"></param>
		public WfReplaceAssigneesExecutor(IWfActivity operatorActivity, IWfActivity targetActivity, IUser originalAssignee, IEnumerable<IUser> targetAssignees)
			: base(operatorActivity, WfControlOperationType.ReplaceAssignees)
		{
			targetActivity.NullCheck("targetActivity");
			targetAssignees.NullCheck("targetAssignees");

			if (originalAssignee != null)
				this._OriginalAssignees.Add(originalAssignee);
			else
			{
				this._OriginalAssignees.CopyFrom(targetActivity.Assignees.ToUsers());

				if (this._OriginalAssignees.Count == 0)
					this._OriginalAssignees.CopyFrom(targetActivity.Candidates.ToUsers());
			}

			this.TargetAssignees = targetAssignees;
			this.TargetActivity = targetActivity;

			if (OperatorActivity == null)
				OperatorActivity = this.TargetActivity;
		}

		/// <summary>
		/// 做后续的用户待办操作
		/// </summary>
		public static void DoUserTaskOperations()
		{
			WfRuntime.ProcessContext.MoveToUserTasks.DistinctByActivityUserAndStatus();
			UserTaskAdapter.Instance.SendUserTasks(WfRuntime.ProcessContext.MoveToUserTasks);
			WfRuntime.ProcessContext.MoveToUserTasks.Clear();

			UserTaskAdapter.Instance.DeleteUserTasks(WfRuntime.ProcessContext.DeletedUserTasks);
			WfRuntime.ProcessContext.DeletedUserTasks.Clear();
		}

		protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
		{
			ReplaceAssignees(this.TargetActivity.Assignees);
			ReplaceAssignees(this.TargetActivity.Candidates);
			ReplaceResources(this.TargetActivity.Assignees);

			WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(this.TargetActivity.Process);

			if (this.TargetActivity.Status == WfActivityStatus.Running)
				PrepareUserTasksAndAcl();
		}

		protected override IWfProcess OnGetCurrentProcess()
		{
			IWfProcess result = WfRuntime.ProcessContext.CurrentProcess;

			if (result == null)
			{
				result = this.TargetActivity.Process;
				WfRuntime.ProcessContext.OriginalActivity = this.TargetActivity;
			}

			return result;
		}

		protected override void OnPersistWorkflow(WfExecutorDataContext dataContext)
		{
			DoUserTaskOperations();

			base.OnPersistWorkflow(dataContext);
		}

		private void ReplaceResources(WfAssigneeCollection collection)
		{
			this.TargetActivity.Descriptor.Resources.Clear();

			foreach (IUser user in collection.ToUsers())
			{
				this.TargetActivity.Descriptor.Resources.Add(new WfUserResourceDescriptor(user));
			}
		}

		private void ReplaceAssignees(WfAssigneeCollection collection)
		{
			foreach (IUser user in this._OriginalAssignees)
			{
				collection.Remove(a => string.Compare(a.User.ID, user.ID, true) == 0);
			}

			collection.Add(this.TargetAssignees);
		}

		private string TargetAssigneesToString()
		{
			return UsersToString(this.TargetAssignees);
		}

		private string UsersToString(IEnumerable<IUser> users)
		{
			StringBuilder strB = new StringBuilder(32);

			foreach (var user in users)
			{
				if (strB.Length > 0)
					strB.Append(",");

				strB.Append(user.DisplayName);
			}

			return strB.ToString();
		}

		private void PrepareUserTasksAndAcl()
		{
			UserTaskCollection originalUserTasks = LoadOriginalUserTask();

			UserTaskCollection newTasks = null;

			if (originalUserTasks.Count > 0)
				newTasks = CreateUserTasksFromTemplate(originalUserTasks, this.TargetAssignees);
			else
				newTasks = CreateNewUserTasks(this.TargetActivity, this.TargetAssignees);

			WfRuntime.ProcessContext.DeletedUserTasks.CopyFrom(originalUserTasks);
			WfRuntime.ProcessContext.MoveToUserTasks.CopyFrom(newTasks);
			WfRuntime.ProcessContext.Acl.CopyFrom(CreateNewAcl(this.TargetActivity, this.TargetAssignees));
		}

		private UserTaskCollection LoadOriginalUserTask()
		{
			UserTaskCollection result = UserTaskAdapter.Instance.LoadUserTasksByActivity(this.TargetActivity.ID,
				build =>
				{
					build.DataField = "SEND_TO_USER";

					this.OriginalAssignees.ForEach(u => build.AppendItem("SEND_TO_USER", u.ID));
				}
			);

			return result;
		}

		private static UserTaskCollection CreateNewUserTasks(UserTaskCollection originalTasks, IEnumerable<IUser> targetUsers)
		{
			UserTaskCollection result = new UserTaskCollection();

			foreach (UserTask task in originalTasks)
			{
				foreach (IUser user in targetUsers)
				{
					UserTask newTask = task.Clone();

					newTask.TaskID = UuidHelper.NewUuidString();
					newTask.SendToUserID = user.ID;
					newTask.SendToUserName = user.DisplayName;
					newTask.TaskStartTime = DateTime.Now;
					result.Add(newTask);
				}
			}

			return result;
		}

		private static UserTaskCollection CreateNewUserTasks(IWfActivity activity, IEnumerable<IUser> targetUsers)
		{
			return UserTaskActionBase.BuildUserTasksFromActivity(activity, targetUsers, TaskStatus.Ban);
		}

		private static UserTaskCollection CreateUserTasksFromTemplate(UserTaskCollection originalTasks, IEnumerable<IUser> targetUsers)
		{
			UserTaskCollection result = new UserTaskCollection();

			foreach (UserTask task in originalTasks)
			{
				foreach (IUser user in targetUsers)
				{
					UserTask newTask = task.Clone();

					newTask.TaskID = UuidHelper.NewUuidString();
					newTask.SendToUserID = user.ID;
					newTask.SendToUserName = user.DisplayName;
					newTask.TaskStartTime = DateTime.Now;

					result.Add(newTask);
				}
			}

			return result;
		}

		private static WfAclItemCollection CreateNewAcl(IWfActivity activity, IEnumerable<IUser> targetUsers)
		{
			WfAclItemCollection result = new WfAclItemCollection();

			foreach (IUser user in targetUsers)
			{
				WfAclItem acl = new WfAclItem();

				acl.ObjectID = user.ID;
				acl.ObjectName = user.DisplayName;
				acl.ObjectType = "Users";
				acl.ResourceID = activity.Process.ResourceID;
				acl.Source = activity.ID;

				result.Add(acl);
			}

			return result;
		}

		protected override void OnPrepareUserOperationLog(WfExecutorDataContext dataContext)
		{
			if (OperatorActivity != null)
			{
				UserOperationLog log = UserOperationLog.FromActivity(OperatorActivity);

				log.OperationType = DataObjects.OperationType.Update;
				log.OperationName = EnumItemDescriptionAttribute.GetDescription(this.OperationType);

				dataContext.OperationLogs.Add(log);
			}

			FirePrepareUserOperationLog(dataContext, dataContext.OperationLogs);

			foreach (UserOperationLog log in dataContext.OperationLogs)
			{
				if (log.RealUser != null && string.IsNullOrEmpty(log.OperationDescription))
				{
					string originalUserName = UsersToString(this.OriginalAssignees);

					log.OperationDescription = string.Format("{0}:{1}->{2}>>'{3}' {4:yyyy-MM-dd HH:mm:ss}",
								log.OperationName, log.RealUser.DisplayName, originalUserName, TargetAssigneesToString(), DateTime.Now);
				}
			}
		}
	}
}
