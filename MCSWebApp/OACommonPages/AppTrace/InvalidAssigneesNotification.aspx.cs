using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using MCS.Web.Library.MVC;
using MCS.Library.Data.Mapping;
using MCS.Web.Library;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Core;

namespace MCS.OA.CommonPages.AppTrace
{
	[Serializable]
	public sealed class InvalidAssigneesUrlsCondition
	{
		[ConditionMapping("NOTIFICATION_ID")]
		public string NotificationID { get; set; }
	}

	public partial class InvalidAssigneesNotification : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.IsPostBack == false && this.IsCallback == false)
			{
				ControllerHelper.ExecuteMethodByRequest(this);
				this.BindDataGrid();
			}
			/*
			if (this.IsPostBack == false)
			{
				this.QueryCondition.NotificationID = WebUtility.GetRequestParamString("notificationID", string.Empty);
				this.UserTaskID = WebUtility.GetRequestParamString("userTaskID", string.Empty);
				this.confirmButton.Visible = bool.Parse(WebUtility.GetRequestParamString("isTask", "true"));
			}*/
		}

		/// <summary>
		/// 此处这样用，主要是考虑以后扩展查询条件
		/// </summary>
		private InvalidAssigneesUrlsCondition QueryCondition
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "QueryCondition", new InvalidAssigneesUrlsCondition());
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "QueryCondition", value);
			}
		}

		private string UserTaskID
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "UserTaskID", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "UserTaskID", value);
			}
		}

		private void BindDataGrid()
		{
			WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(this.QueryCondition);

			this.objectDataSource.Condition = builder;
			this.objectDataSource.LastQueryRowCount = -1;

			this.dataGrid.PageIndex = 0;
			this.dataGrid.SelectedKeys.Clear();
			this.dataGrid.DataBind();
		}

		[ControllerMethod(true)]
		protected void LoadNotificationObject(string notificationID, string userTaskID, bool isTask)
		{
			this.QueryCondition = new InvalidAssigneesUrlsCondition();
			this.QueryCondition.NotificationID = notificationID;
			this.UserTaskID = userTaskID;
			this.confirmButton.Visible = isTask;
		}

		protected void Submitbtn_Click(object sender, EventArgs e)
		{
			try
			{
				UserTask clonedUserTask = UserTaskAdapter.Instance.LoadSingleUserTaskByID(this.UserTaskID);
				UserTaskCollection tasks= new UserTaskCollection();
				tasks.Add(clonedUserTask);
				UserTaskAdapter.Instance.SetUserTasksAccomplished(tasks);
				UserTaskAdapter.Instance.DeleteUserTasks(tasks);
				
				/*
				ORMappingItemCollection mapping = ORMapping.GetMappingInfo<UserTask>().Clone();
				mapping.TableName = "WF.USER_ACCOMPLISHED_TASK";
				clonedUserTask.Url = UriHelper.ReplaceUriParams(clonedUserTask.Url, nvs =>
				{
					nvs["isTask"] = "false";
				});

				mapping.Remove("CATEGORY_GUID");
				mapping.Remove("TOP_FLAG");

				StringBuilder sqlBuilder = new StringBuilder();

				sqlBuilder.Append(ORMapping.GetInsertSql(clonedUserTask, mapping, TSqlBuilder.Instance));
				sqlBuilder.Append(TSqlBuilder.Instance.DBStatementSeperator);
				sqlBuilder.AppendFormat("DELETE FROM WF.USER_TASK WHERE  TASK_GUID =N'{0}'", clonedUserTask.TaskID);

				DbHelper.RunSqlWithTransaction(sqlBuilder.ToString()); */

				Page.ClientScript.RegisterStartupScript(this.GetType(), "关闭当前窗口",
				string.Format("top.close();"),
				true);
			}
			catch (Exception ex)
			{
				WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
			}
		}
	}
}