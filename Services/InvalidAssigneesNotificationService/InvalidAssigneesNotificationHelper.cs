using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MCS.Library.OGUPermission;
using MCS.Library.Passport;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Principal;
using MCS.Library.Core;


namespace InvalidAssigneesNotificationService
{
	internal static class InvalidAssigneesNotificationHelper
	{
		internal static readonly string GetInvalidAssigneesNotificationsSQL = string.Format(@"
 SELECT TOP ({0}) PCA.ACTIVITY_ID, PCA.ACTIVITY_DESC_KEY, PCA.STATUS, UT.URL, UT.USER_ID, UT.USER_NAME,PCA.PROCESS_ID, PRI.PROCESS_NAME, PCA.START_TIME, PCA.END_TIME
 FROM WF.PROCESS_CURRENT_ACTIVITIES AS PCA INNER JOIN
      WF.INVALID_ASSIGNEES AS PIA ON PCA.PROCESS_ID = PIA.PROCESS_ID AND PCA.ACTIVITY_ID = PIA.ACTIVITY_ID INNER JOIN
      WF.PROCESS_INSTANCES AS PRI ON PCA.PROCESS_ID = PRI.INSTANCE_ID LEFT OUTER JOIN
      WF.PROCESS_CURRENT_ASSIGNEES AS UT ON PCA.ACTIVITY_ID = UT.ACTIVITY_ID AND PCA.PROCESS_ID = UT.PROCESS_ID
 WHERE (PCA.START_TIME > (SELECT ISNULL(MAX(CREATE_TIME), DATEADD(YEAR, - 10, GETDATE())) FROM WF.INVALID_ASSIGNEES_NOTIFICATION ))
ORDER BY PCA.START_TIME DESC ", SendInvalidNotificationSettingsSection.GetConfig().MaxSendCount);

		/// <summary>
		/// 获取所有管理员用户信息
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<IUser> GetAllUsers()
		{
			/*
			RolesDefineConfig rolesConfig = RolesDefineConfig.GetConfig().;

			List<string> roleNames = new List<string>();

			foreach (RolesDefine roleDefine in rolesConfig.RolesDefineCollection)
			{
				roleNames.Add(roleDefine.Name);
			}
			               
			IRole[] roles = rolesConfig.GetRolesInstances(roleNames.ToArray()); */

			IRole[] roles = RolesDefineConfig.GetConfig().RolesDefineCollection["ProcessAdmin"].GetRolesInstances();

			Dictionary<string, IUser> allUsers = new Dictionary<string, IUser>();

			foreach (IRole role in roles)
			{
				WfRoleResourceDescriptor roleDescriptor = new WfRoleResourceDescriptor(role);
				WfResourceDescriptorCollection roleDescriptors = new WfResourceDescriptorCollection(roleDescriptor);
				roleDescriptors.Add(roleDescriptor);

				OguDataCollection<IUser> roleUsers = roleDescriptors.ToUsers();

				foreach (IUser user in roleUsers)
				{
					if (allUsers.ContainsKey(user.ID) == false)
						allUsers.Add(user.ID, user);
				}
			}

			return allUsers.Values;
		}
		/*
		public static int InvalidAssigneesNotificationCount()
		{
			return (int)DbHelper.RunSqlReturnScalar(GetInvalidNotificationCountSQL);
		} */

		/// <summary>
		/// 生成管理员代办消息
		/// </summary>
		/// <param name="notificationID"></param>
		/// <returns></returns>
		public static UserTask BuildUserTask(string taskTitle)
		{
			UserTask currentUserTask = new UserTask();
			currentUserTask.ApplicationName = "OACommonPages";
			currentUserTask.ProgramName = "OACommonPages";
			/*currentUserTask.ResourceID = ;
			currentUserTask.ActivityID = "";
			currentUserTask.ProcessID = ""; */
			currentUserTask.Status = TaskStatus.Ban;
			currentUserTask.TaskStartTime = DateTime.Now;
			currentUserTask.TaskTitle = taskTitle;
			//string.Format("待处理流程列表", DateTime.Now.ToString("yyyy-MM-dd"));
			currentUserTask.Level = TaskLevel.Normal;
			currentUserTask.Url = "/MCSWebApp/OACommonPages/AppTrace/InvalidAssigneesNotification.aspx?notificationID={0}&&userTaskID={1}&&isTask=true";
			/*currentUserTask.DraftUserID = "";
			currentUserTask.DraftUserName = "";
			currentUserTask.SendToUserID = "";
			currentUserTask.SendToUserName = "";
			currentUserTask.SourceID = "";
			currentUserTask.SourceName = ""; */

			return currentUserTask;
		}

		/// <summary>
		/// 给管里员批量发送待办
		/// </summary>
		/// <param name="allUsers"></param>
		/// <param name="itemUserTask"></param>
		public static void SendUserTask(IEnumerable<IUser> allUsers, UserTask itemUserTask, string notificationID)
		{
			UserTaskCollection senuserTasks = new UserTaskCollection();
			foreach (IUser user in allUsers)
			{
				UserTask item = itemUserTask.Clone();
				item.TaskID = UuidHelper.NewUuidString();
				item.ProcessID = item.TaskID;
				item.ResourceID = item.TaskID;
				item.Url = string.Format(item.Url, notificationID, item.TaskID);
				item.SendToUserID = user.ID;
				item.SendToUserName = user.Name;

				senuserTasks.Add(item);
			}

			UserTaskAdapter.Instance.SendUserTasks(senuserTasks);
		}

		/// <summary>
		/// 记录当前发送待办相关流程信息
		/// </summary>
		/// <param name="notificationID"></param>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static InvalidAssignessUrlCollection PreInvalidAssignessUrls(string notificationID, DataTable dt)
		{
			Dictionary<string, InvalidAssigneeUrl> dic = new Dictionary<string, InvalidAssigneeUrl>();

			foreach (DataRow dr in dt.Rows)
			{
				string key = string.Format("{0}@{1}@{2}", notificationID, dr["ACTIVITY_ID"].ToString(), dr["PROCESS_ID"].ToString());
				if (dic.ContainsKey(key) == false)
				{
					InvalidAssigneeUrl item = new InvalidAssigneeUrl();
					item.NotificationID = notificationID;
					item.ActivityID = dr["ACTIVITY_ID"].ToString();
					item.ProcessID = dr["PROCESS_ID"].ToString();
					item.ProcessName = dr["PROCESS_NAME"].ToString();
					item.Url = dr["URL"].ToString();
					item.ActivityKey = dr["ACTIVITY_DESC_KEY"].ToString();

					dic.Add(key, item);
				}
				else if (string.IsNullOrEmpty(dic[key].Url) == true && string.IsNullOrEmpty(dr["URL"].ToString()) == false)
					dic[key].Url = dr["URL"].ToString();
			}

			InvalidAssignessUrlCollection invalidAssignessUrls = new InvalidAssignessUrlCollection();

			foreach (InvalidAssigneeUrl item in dic.Values)
			{
				invalidAssignessUrls.Add(item);
			}

			return invalidAssignessUrls;
		}
	}
}
