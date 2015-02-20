using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using MCS.OA.Portal.Common;
using MCS.Web.Library;
using MCS.Library.Data.Builder;
using MCS.Library.Principal;
using MCS.Library.OGUPermission;
using System.Text;
using MCS.Library.Core;
using MCS.Web.Library.MVC;


namespace MCS.OA.Portal.TaskList
{
	public partial class CompletedTaskList : System.Web.UI.Page
	{

		private string ProcessStatus
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "ProcessStatus", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "ProcessStatus", value);
			}
		}

		[ControllerMethod(true)]
		protected void DefaultProcessRequest(string process_status)
		{
			CurrentPageTaskStatus = TaskStatus.Ban;
			//LDM 在这里设置页面的页大小
			//LDM 原来的代码设定为固定值20
			this.GridViewTask.PageSize = UserSettings.GetSettings(DeluxeIdentity.CurrentUser.ID).GetPropertyValue("CommonSettings", "ProcessedListPageSize", 20);
			//this.advancedSearch.Attributes.Add("onclick", "onAdvancedSearchClick('"
			//    + CurrentPageTaskStatus.ToString("d") + "')");

			ProcessStatus = process_status;

			if (ProcessStatus == "Completed")
			{
				LblTitle.Text = "已办结事项";
				this.GridViewTask.GridTitle = "已办结列表";
			}
			else
			{
				LblTitle.Text = "流转中事项";
				this.GridViewTask.GridTitle = "流转中列表";
			}
			ExecQuery();
		}

		private void InitPageSize()
		{
			string pageSizeKey = "WorkingListPageSize";

			if (ProcessStatus == "Completed")
				pageSizeKey = "CompletedTaskListPageSize";

			UserSettings userSettings = UserSettings.GetSettings(DeluxeIdentity.CurrentUser.ID);
			GridViewTask.PageSize = userSettings.GetPropertyValue("CommonSettings", pageSizeKey, this.GridViewTask.PageSize);
		}

		/// <summary>
		/// 设置或获取当前列表展示的任务类型
		/// </summary>
		private TaskStatus CurrentPageTaskStatus
		{
			get
			{
				return WebControlUtility.GetViewStateValue<TaskStatus>(ViewState, "CurrentPageTaskStatus", TaskStatus.Ban);
			}
			set
			{
				WebControlUtility.SetViewStateValue<TaskStatus>(ViewState, "CurrentPageTaskStatus", value);
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetNoStore();
			bindingControl.Data = QueryCondition;
			if (!IsPostBack)
			{
				ControllerHelper.ExecuteMethodByRequest(this);
			}
		}

		protected void QueryBtnClick(object sender, EventArgs e)
		{
			ClearAdvancedConditions();
			ExecQuery();
		}

		protected void GridViewTask_ExportData(object sender, EventArgs e)
		{
			ExecQuery();
		}

		private void ExecQuery()
		{
			InitPageSize();

			bindingControl.CollectData();

			WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(QueryCondition,
				new AdjustConditionValueDelegate(AdjustQueryConditionValue));

			builder.AppendItem("UT.STATUS", (int)TaskStatus.Ban);

			//// LDM 流转中数据的获取
			//if (null != Request.QueryString["process_status"] && Request.QueryString["process_status"] == "Running")
			//    builder.AppendItem("PN.STATUS", "Running");

			////  LDM 已办结数据的获取
			//if (null != Request.QueryString["process_status"] && Request.QueryString["process_status"] == "Completed")
			//    builder.AppendItem("ISNULL(PN.STATUS,N'Completed')", "Completed");

			if (QueryCondition.ApplicationName == "全部")
			{
				QueryCondition.ApplicationName = "";
			}

			string subjectQueryString = string.Empty;

			if (string.IsNullOrEmpty(QueryCondition.TaskTitle) == false)
			{
				StringBuilder subjectSB = new StringBuilder();
				//关键词分割符为全角或半角空格
				char[] separators = new char[] { ' ', '　' };
				string[] wordsSplitted = QueryCondition.TaskTitle.Split(separators, StringSplitOptions.RemoveEmptyEntries);
				//将关键词构造为谓词查询格式
				for (int i = 0; i < wordsSplitted.Length; i++)
				{
					if (i > 0)
					{
						subjectSB.Append(" AND ");
					}

					subjectSB.Append("\"");
					subjectSB.Append(wordsSplitted[i].Replace("\"", "\"\""));
					subjectSB.Append("\"");
				}
				subjectQueryString = string.Format("CONTAINS(TASK_TITLE,{0})", TSqlBuilder.Instance.CheckQuotationMark(subjectSB.ToString(), true));
			}

			WhereSqlClauseBuilder processStatusBuilder = new WhereSqlClauseBuilder(LogicOperatorDefine.Or);

			switch (WebUtility.GetRequestQueryString("process_status", "Running"))
			{
				case "Running":
					processStatusBuilder.AppendItem("PN.STATUS", "Running");
					processStatusBuilder.AppendItem("PN.STATUS", "Maintaining");
					break;
				case "Completed":
					processStatusBuilder.AppendItem("PN.STATUS", "Completed");
					processStatusBuilder.AppendItem("PN.STATUS", "Aborted");
					break;
			}

			ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection();

			connectiveBuilder.Add(builder).Add(processStatusBuilder);

			if (connectiveBuilder.IsEmpty)
				whereCondition.Value = subjectQueryString;
			else if (subjectQueryString == string.Empty)
				whereCondition.Value = connectiveBuilder.ToSqlString(TSqlBuilder.Instance);
			else
				whereCondition.Value = connectiveBuilder.ToSqlString(TSqlBuilder.Instance) + " AND " + subjectQueryString;

			if (QueryCondition.DraftDepartmentName != string.Empty)
			{
				whereCondition.Value += string.Format(" AND CONTAINS(DRAFT_DEPARTMENT_NAME,'\"*"
					+ TSqlBuilder.Instance.CheckQuotationMark(QueryCondition.DraftDepartmentName, false) + "*\"')");
			}

			whereCondition.Value += string.Format(" AND SEND_TO_USER = {0}",
										   TSqlBuilder.Instance.CheckQuotationMark(DeluxeIdentity.CurrentUser.ID, true));
			LastQueryRowCount = -1;
			this.GridViewTask.PageIndex = 0;
		}

		private int LastQueryRowCount
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "LastQueryRowCount", -1);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "LastQueryRowCount", value);
			}
		}

		private TaskQueryCondition QueryCondition
		{
			get
			{
				TaskQueryCondition result = (TaskQueryCondition)ViewState["QueryCondition"];

				if (result == null)
				{
					result = new TaskQueryCondition();
					ViewState["QueryCondition"] = result;
				}
				return result;
			}
		}



		protected void ObjectDataSourceTask_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			e.InputParameters["totalCount"] = LastQueryRowCount;
		}

		protected void ObjectDataSourceTask_Selected(object sender, ObjectDataSourceStatusEventArgs e)
		{
			LastQueryRowCount = (int)e.OutputParameters["totalCount"];
		}

		protected void GridViewTask_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				//GridCommon.SetRowStyleWhenMouseOver(e.Row);

				GridCommon.SetRowStyleWhenMouseOver(e.Row, "selecteditem", "taitem", "titem");

				UserTask task = (UserTask)e.Row.DataItem;

				e.Row.Cells[2].Text = GridCommon.GetTaskURL(task);

				// e.Row.Cells[5].Text = GridCommon.TimeDisplayFormat(task.CompletedTime);
			}
		}

		/// <summary>
		/// //LDM 设置已办结列表的状态列为不可见
		/// </summary>
		protected void GridViewTask_PreRender(object sender, EventArgs e)
		{
			string status = Request.QueryString["process_status"];
			if (status == "Completed")
			{
				((GridView)sender).Columns[4].Visible = false;
			}
		}

		protected void RefreshButton_Click(object sender, EventArgs e)
		{
			this.GridViewTask.PageSize = UserSettings.GetSettings(DeluxeIdentity.CurrentUser.ID).GetPropertyValue("CommonSettings", "ProcessedListPageSize", 20);
			ExecQuery();
		}

		protected void ButtonAdvanced_Click(object sender, EventArgs e)
		{
			ExecQuery();
		}

		/// <summary>
		/// 重置高级查询的查询条件
		/// </summary>
		private void ClearAdvancedConditions()
		{
			this.emergencySelector.Value = "-1";
			//this.beginTimeInput.Value = string.Empty;
			//this.endTimeInput.Value = string.Empty;
			this.TextBoxPurpose.Value = string.Empty;
			this.PersonID.Value = string.Empty;
			this.FromPerson.Value = string.Empty;
			this.OrgID.Value = string.Empty;
		}

		#region 暂时取消高级查询
		///// <summary>
		///// 根据部门ID取出部门下的人员ID，并加入PersonID串
		///// </summary>
		//private void AddPersonQueryStringFromOrganizations()
		//{
		//    if (this.OrgID.Value != string.Empty)
		//    {
		//        string[] orgIDs = this.OrgID.Value.Split(',');

		//        OguObjectCollection<IOrganization> orgs = OguMechanismFactory.GetMechanism().GetObjects<IOrganization>(SearchOUIDType.Guid, orgIDs);

		//        foreach (IOrganization org in orgs)
		//        {
		//            OguObjectCollection<IUser> users = org.GetAllChildren<IUser>(false);
		//            foreach (IUser user in users)
		//            {
		//                if (this.PersonID.Value != string.Empty)
		//                {
		//                    this.PersonID.Value += ",";
		//                }
		//                this.PersonID.Value += TSqlBuilder.Instance.CheckQuotationMark(user.ID, true);
		//            }
		//        }
		//    }
		//}
		#endregion

		/// <summary>
		/// 对绑定的查询对象属性做更改的委托
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="data"></param>
		/// <param name="ignored"></param>
		/// <returns></returns>
		private static object AdjustQueryConditionValue(string propertyName, object data, ref bool ignored)
		{
			object result = data;
			switch (propertyName)
			{
				//由于枚举类型首项一般为0，ConditionMapping.GetWhereSqlClauseBuilder会把0值当成int的默认值过滤掉
				//所以枚举类型的查询条件应当单独处理
				case "Emergency":
					ignored = true;
					break;
				//已办标题使用全文检索，格式单独处理
				case "TaskTitle":
					ignored = true;
					break;
				case "Purpose":
					data = ((string)data).Trim();
					data = TSqlBuilder.Instance.EscapeLikeString((string)data);
					result = "%" + data + "%";
					break;
				case "CompletedTimeEnd":
					result = ((DateTime)data).AddDays(1);
					break;
				case "SourceID":
					result = "(" + data + ")";
					break;
				case "DraftDepartmentName":
					ignored = true;
					break;
				case "ApplicationName":
					if (data.ToString() == "全部")
					{
						ignored = true;
					}
					break;
			}

			return result;
		}



	}
}