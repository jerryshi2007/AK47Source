using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Services;
using MCS.Library.SOA.DataObjects;
using System.Data;
using MCS.Library.OGUPermission;
using MCS.Library.Core;
using MCS.Library.Data;
using System.Transactions;

namespace InvalidAssigneesNotificationService
{
	internal class InvalidAssigneesNotificationServiceThread : ThreadTaskBase
	{
		public override void OnThreadTaskStart()
		{
			if (this.Params != null)
				this.Params.Log.Write("处理人员不存在流程Task开始执行：" + DateTime.Now.ToString());

			Run();

			if (this.Params != null)
				this.Params.Log.Write("处理人员不存在流程Task执行完毕：" + DateTime.Now.ToString());
		}

		//(1)：先拿到所有的管理员;
		//(2): 拼一个 userTask 对象，只是收件人为空。
		//(3): 遍历所有的管员，clone 第一个userTask对象，并将添加到 UserTaskCollection 对象里
		//(4): UserTaskAdapter.Instance.SendUserTasks 发送数据
		public void Run()
		{
			try
			{
				DataTable dt = DbHelper.RunSqlReturnDS(InvalidAssigneesNotificationHelper.GetInvalidAssigneesNotificationsSQL).Tables[0];

				if (dt.Rows.Count > 0)
				{
					IEnumerable<IUser> allUsers = InvalidAssigneesNotificationHelper.GetAllUsers();

					InvalidAssigneesNotification currentIANotification = new InvalidAssigneesNotification() { NotificationID = UuidHelper.NewUuidString(), CreateTime = DateTime.Now };

					InvalidAssignessUrlCollection ivalidAssignessUrls = InvalidAssigneesNotificationHelper.PreInvalidAssignessUrls(currentIANotification.NotificationID, dt);

					//todo: 一百以内并数字
					if (ivalidAssignessUrls.Count < SendInvalidNotificationSettingsSection.GetConfig().MaxSendCount)
						currentIANotification.Description = string.Format("待处理流程人员异常 {0}条", ivalidAssignessUrls.Count);
					else
						currentIANotification.Description = string.Format("待处理流程人员异常 {0}条以上",SendInvalidNotificationSettingsSection.GetConfig().MaxSendCount);

					UserTask userTaskItem = InvalidAssigneesNotificationHelper.BuildUserTask(currentIANotification.Description);

					using (TransactionScope ts = TransactionScopeFactory.Create())
					{
						InvalidAssigneesNotificationHelper.SendUserTask(allUsers, userTaskItem, currentIANotification.NotificationID);

						InvalidAssigneesNotificationAdapter.Instance.AddData(currentIANotification);

						InvalidAssigneesUrlAdapter.Instance.BulkAdd(ivalidAssignessUrls);

						ts.Complete();
					}
				}
			}
			catch (Exception ex)
			{
				this.Params.Log.Write(string.Format("处理人员不存流程：{0}; {1}", DateTime.Now.ToString(), ex.ToString()));
			}
		}
	}
}
