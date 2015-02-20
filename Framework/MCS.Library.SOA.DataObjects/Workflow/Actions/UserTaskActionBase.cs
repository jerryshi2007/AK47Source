using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;
using System.Collections.Specialized;

namespace MCS.Library.SOA.DataObjects.Workflow.Actions
{
	[Serializable]
	public abstract class UserTaskActionBase : IWfAction
	{
		#region IWfAction Members

		public virtual void PrepareAction(WfActionParams actionParams)
		{
		}

		public virtual void PersistAction(WfActionParams actionParams)
		{

		}

		public virtual void ClearCache()
		{

		}
		#endregion

		internal static UserTaskCollection BuildUserTasksFromActivity(IWfActivity activity, IEnumerable<IUser> users, TaskStatus status)
		{
			UserTaskCollection tasks = new UserTaskCollection();

			foreach (IUser user in users)
			{
				UserTask task = BuildOneUserTaskFromActivity(activity, status);

				task.SendToUserID = user.ID;
				task.SendToUserName = user.DisplayName;
				task.Emergency = WfRuntime.ProcessContext.Emergency;
				task.Purpose = WfRuntime.ProcessContext.Purpose;

				tasks.Add(task);
			}

			return tasks;
		}

		internal static UserTaskCollection BuildUserTasksFromActivity(IWfActivity activity, TaskStatus status)
		{
			return BuildUserTasksFromActivity(activity, activity.Assignees.ToUsers(), status);
		}

		internal static UserTaskCollection BuildUserTasksFromActivity(IWfActivity activity)
		{
			return BuildUserTasksFromActivity(activity, TaskStatus.Ban);
		}

		internal static UserTaskCollection BuildUserNotifiesFromActivity(IWfActivity activity)
		{
			UserTaskCollection tasks = BuildUserTasksFromActivity(activity, TaskStatus.Yue);

			foreach (UserTask task in tasks)
			{
				task.Level = TaskLevel.Low;
				task.Url = GenerateNotifyUrl(task, activity);
			}

			return tasks;
		}

		internal static UserTask BuildOneUserNotifyFromActivity(IWfActivity activity)
		{
			UserTask task = BuildOneUserTaskFromActivity(activity, TaskStatus.Yue);

			task.Url = GenerateNotifyUrl(task, activity);

			return task;
		}

		private static string GenerateNotifyUrl(UserTask task, IWfActivity activity)
		{
			NameValueCollection uriParams = UriHelper.GetUriParamsCollection(task.Url);

			uriParams.Clear();
			uriParams["resourceID"] = activity.Process.ResourceID;
			uriParams["processID"] = activity.Process.ID;
			uriParams["_op"] = "notifyDialog";
			uriParams["taskID"] = task.TaskID;

			return UriHelper.CombineUrlParams(task.Url, false, uriParams);
		}

		internal static UserTask BuildOneUserTaskFromActivity(IWfActivity activity)
		{
			return BuildOneUserTaskFromActivity(activity, TaskStatus.Ban);
		}

		internal static UserTask BuildOneUserTaskFromActivity(IWfActivity activity, TaskStatus status)
		{
			UserTask task = new UserTask();

			task.TaskID = UuidHelper.NewUuidString();

			task.ApplicationName = activity.Process.Descriptor.ApplicationName;
			task.ProgramName = activity.Process.Descriptor.ProgramName;
			task.ResourceID = activity.Process.ResourceID;
			task.ActivityID = activity.ID;
			task.ProcessID = activity.Process.ID;
			task.Status = status;
			task.TaskStartTime = activity.Process.StartTime;

			DateTime estimateEndTime = activity.Descriptor.Properties.GetValue("EstimateEndTime", DateTime.MinValue);

			if (estimateEndTime != DateTime.MinValue)
				task.ExpireTime = estimateEndTime;

			if (status == TaskStatus.Yue)
				task.TaskTitle = GetNotifyTitle(activity);

			if (string.IsNullOrEmpty(task.TaskTitle))
				task.TaskTitle = GetTaskTitle(activity);

			task.Level = TaskLevel.Normal;

			IWfProcess rootProcess = activity.Process.RootProcess;

			if (OguBase.IsNotNullOrEmpty(rootProcess.Creator))
			{
				task.DraftUserID = rootProcess.Creator.ID;
				task.DraftUserName = rootProcess.Creator.DisplayName;
			}

			if (OguBase.IsNotNullOrEmpty(activity.Process.OwnerDepartment))
			{
				task.DraftDepartmentName = rootProcess.OwnerDepartment.GetDepartmentDescription();
			}

			if (DeluxePrincipal.IsAuthenticated)
			{
				task.SourceID = DeluxeIdentity.CurrentUser.ID;
				task.SourceName = DeluxeIdentity.CurrentUser.DisplayName;
			}

			task.Url = GetTaskUrl(activity);

			return task;
		}

		internal static void AppendResourcesToNotifiers(IWfActivity currentActivity, UserTaskCollection notifyTasks, WfResourceDescriptorCollection resources)
		{
			foreach (IUser user in resources.ToUsers())
			{
				UserTask task = BuildOneUserNotifyFromActivity(currentActivity);

				task.SendToUserID = user.ID;
				task.SendToUserName = user.DisplayName;

				notifyTasks.Add(task);
			}
		}

		private static string GetTaskTitle(IWfActivity activity)
		{
			string title = activity.Descriptor.TaskTitle;

			if (string.IsNullOrEmpty(title))
				title = activity.Process.Descriptor.DefaultTaskTitle;

			return title;
		}

		/// <summary>
		/// %%%%%%%%%%%%%%%%%%%
		/// </summary>
		/// <param name="activity"></param>
		/// <returns></returns>
		private static string GetNotifyTitle(IWfActivity activity)
		{
			string title = activity.Descriptor.NotifyTaskTitle;

			if (string.IsNullOrEmpty(title))
				title = activity.Process.Descriptor.DefaultNotifyTaskTitle;

			return title;
		}

		private static string GetTaskUrl(IWfActivity activity)
		{
			string url = activity.Descriptor.Url;

			if (string.IsNullOrEmpty(url))
				url = activity.Process.Descriptor.Url;

			if (url.IsNotEmpty())
			{
				NameValueCollection urlParams = UriHelper.GetUriParamsCollection(url);

				urlParams["resourceID"] = activity.Process.ResourceID;
				urlParams["activityID"] = activity.ID;

				url = UriHelper.CombineUrlParams(url, false, urlParams);
			}

			return url;
		}
	}
}
