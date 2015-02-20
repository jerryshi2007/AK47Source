using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Core;
using MCS.Library.Passport;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;

namespace InvalidAssigneesNotificationService
{
	public partial class InvalidAssigneesNotificationServiceControl : UserControl
	{
		public InvalidAssigneesNotificationServiceControl()
		{
			InitializeComponent();

			this.BindGrid();
		}

		private void BindGrid()
		{
			this.GV_InvalidAssigneesNotifications.DataSource = DbHelper.RunSqlReturnDS(InvalidAssigneesNotificationHelper.GetInvalidAssigneesNotificationsSQL).Tables[0];
		}

		private void Bt_Send_Click(object sender, EventArgs e)
		{
			InvalidAssigneesNotificationServiceThread notificationServiceThread = new InvalidAssigneesNotificationServiceThread();
			notificationServiceThread.OnThreadTaskStart();

			this.BindGrid();
			MessageBox.Show("处理完成", "提示");
		}

		/*
		private void Bt_Send_Click(object sender, EventArgs e)
		{
			//(1)：先拿到所有的管理员;
			//(2): 拼一个 userTask 对象，只是收件人为空。
			//(3): 遍历所有的管员，clone 第一个userTask对象，并将添加到 UserTaskCollection 对象里
			//(4): UserTaskAdapter.Instance.SendUserTasks 发送数据
			int count = InvalidAssigneesNotificationHelper.InvalidAssigneesNotificationCount();
			if (count > 0)
			{
				IEnumerable<IUser> allUsers = InvalidAssigneesNotificationHelper.GetAllUsers();

				InvalidAssigneesNotification currentIANotification = new InvalidAssigneesNotification() { NotificationID = UuidHelper.NewUuidString(), CreateTime = DateTime.Now };
				currentIANotification.Description = string.Format("{0} 待处理{1}条流程信息", DateTime.Now.ToString("yyyy-MM-dd"), count);

				UserTask itemUserTask = InvalidAssigneesNotificationHelper.BuildUserTask(currentIANotification.NotificationID);

				InvalidAssigneesNotificationHelper.SendUserTask(allUsers, itemUserTask);

				InvalidAssigneesNotificationAdapter.Instance.AddData(currentIANotification);

				DataTable dt = this.GV_InvalidAssigneesNotifications.DataSource as DataTable;

				InvalidAssignessUrlCollection ivalidAssignessUrls = InvalidAssigneesNotificationHelper.PreInvalidAssignessUrls(currentIANotification.NotificationID, dt);
				InvalidAssignessUrlsAdapter.Instance.BulkAdd(ivalidAssignessUrls);

				this.BindGrid();
				MessageBox.Show("处理完成", "提示");
			}
		} */
	}
}
