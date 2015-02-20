using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.OA.Portal.Common;
using MCS.Web.Library;
using MCS.Library.Core;
using MCS.OA.Portal.TaskList;
using MCS.Web.Library.Script;

namespace MCS.OA.Portal.TaskList
{
	public partial class UnCompletedTaskList : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetNoStore();

			bindingControl.Data = QueryCondition;
			if (!IsPostBack)
			{
				if (Request.QueryString.GetValue("mode", TaskStatus.Ban).Equals(TaskStatus.Ban))
				{
					ShowToDoList();
				}
			}
		}

		protected void ShowToDoList()
		{
			lblTitle.Text = "待办事项";
			//LDM 在这里设置页面的页大小，原来的代码设定为固定值20
			UserSettings userSettings = UserSettings.GetSettings(DeluxeIdentity.CurrentUser.ID);
			GridViewTask.PageSize = userSettings.GetPropertyValue("CommonSettings", "ToDoListPageSize", this.GridViewTask.PageSize);

			WhereSqlClauseBuilder wscb = new WhereSqlClauseBuilder();
			wscb.AppendItem("SEND_TO_USER", DeluxeIdentity.CurrentUser.ID);
			whereCondition.Value = wscb.ToSqlString(TSqlBuilder.Instance);

			//保存原始待办人ID到控件中
			this.hiddenOriginalUserID.Value = DeluxeIdentity.CurrentUser.ID;

			ExecQuery();
		}

		protected void QueryBtnClick(object sender, EventArgs e)
		{
			//ClearAdvancedConditions();
			ExecQuery();
		}

		protected void GridViewTask_ExportData(object sender, EventArgs e)
		{
			ExecQuery();
		}

		private void ExecQuery()
		{
			// AddPersonQueryStringFromOrganizations();添加组织结构人员

			bindingControl.CollectData();
			WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(QueryCondition,
				new AdjustConditionValueDelegate(AdjustQueryConditionValue));

			if (QueryCondition.ApplicationName == "全部")
			{
				QueryCondition.ApplicationName = "";
			}

			//builder.AppendItem("STATUS", (int)TaskStatus.Ban);
			builder.AppendItem("SEND_TO_USER", DeluxeIdentity.CurrentUser.ID);

			WhereSqlClauseBuilder wBuilder1 = new WhereSqlClauseBuilder();
			wBuilder1.AppendItem("EXPIRE_TIME", "null", "is", true);
			WhereSqlClauseBuilder wBuilder2 = new WhereSqlClauseBuilder();
			wBuilder2.AppendItem("EXPIRE_TIME", "not null", "is", true);
			wBuilder2.AppendItem("EXPIRE_TIME", "getdate()", ">", true);
			ConnectiveSqlClauseCollection collection = new ConnectiveSqlClauseCollection(LogicOperatorDefine.Or);
			collection.Add(wBuilder1);
			collection.Add(wBuilder2);

			WhereSqlClauseBuilder wBuilder3 = new WhereSqlClauseBuilder();
			wBuilder3.AppendItem("STATUS", TaskStatus.Yue.ToString("d"));

			ConnectiveSqlClauseCollection collection1 = new ConnectiveSqlClauseCollection(LogicOperatorDefine.And);
			collection1.Add(wBuilder3);
			collection1.Add(collection);

			ConnectiveSqlClauseCollection collection2 = new ConnectiveSqlClauseCollection(LogicOperatorDefine.Or);
			WhereSqlClauseBuilder wBuilder4 = new WhereSqlClauseBuilder();
			wBuilder4.AppendItem("STATUS", TaskStatus.Ban.ToString("d"));
			collection2.Add(collection1);
			collection2.Add(wBuilder4);

			ConnectiveSqlClauseCollection collection3 = new ConnectiveSqlClauseCollection(LogicOperatorDefine.And);
			collection3.Add(builder);
			collection3.Add(collection2);

			whereCondition.Value = collection3.ToSqlString(TSqlBuilder.Instance);

			if (QueryCondition.DraftDepartmentName != string.Empty)
			{
				whereCondition.Value += string.Format(" AND CONTAINS(DRAFT_DEPARTMENT_NAME,'\"*"
					+ TSqlBuilder.Instance.CheckQuotationMark(QueryCondition.DraftDepartmentName, false) + "*\"')");
			}

			if (QueryCondition.TaskTitle != string.Empty)
			{
				whereCondition.Value += string.Format(" AND CONTAINS(TASK_TITLE,'\"*"
					+ TSqlBuilder.Instance.CheckQuotationMark(QueryCondition.TaskTitle, false) + "*\"')");

			}

			LastQueryRowCount = -1;
			this.GridViewTask.SelectedKeys.Clear();
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

		/// <summary>
		/// 获取查询任务的条件
		/// </summary>
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

				GridCommon.HighlightTopItem(e.Row);
				GridCommon.SetUnreadItemBold(e.Row);

				UserTask task = (UserTask)e.Row.DataItem;
				if (this.GridViewTask.ExportingDeluxeGrid)
				{
					e.Row.Cells[2].Text = Server.HtmlEncode(task.TaskTitle).ToString().Replace(" ", "&nbsp;");
				}
				else
				{
					e.Row.Cells[3].Text = GridCommon.GetTaskURL(task);
				}

				//e.Row.Cells[4].Text = task.ExpireTime == DateTime.MinValue ? string.Empty : e.Row.Cells[4].Text;
			}
		}

		protected void RefreshButton_Click(object sender, EventArgs e)
		{
			// 更新列表数据，保留页码，这件事情将由Grid控件完成
			// 若更新后的页码不存在，则取最大页码

			ShowToDoList();
		}

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
				case "CategoryID":
					ignored = true;
					break;
				case "Purpose":
					data = ((string)data).Trim();
					data = TSqlBuilder.Instance.EscapeLikeString((string)data);
					result = "%" + data + "%";
					break;
				case "DeliverTimeEnd":
				case "ExpireTimeEnd":
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
				case "TaskTitle":
					ignored = true;
					break;
			}

			return result;
		}

		//执行修改
		protected void multiProcess_ExecuteStep(object data)
		{
			ReplaceAssigneeHelperData rahd = JSONSerializerExecute.Deserialize<ReplaceAssigneeHelperData>(data);
			ReplaceAssigneeHelper.ExecuteReplace(rahd);
		}

		#region 暂时取消高级查询
		//protected void ButtonAdvanced_Click(object sender, EventArgs e)
		//{
		//    ExecQuery();
		//}
		#endregion

		#region MCS项目中没有用到的方法集合

		/// <summary>
		/// 刷新类别列表
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		//protected void RefreshCategoryButton_Click(object sender, EventArgs e)
		//{
		//    this.DropDownListCategory.InitData();
		//}

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


		//private void InitMoreActions()
		//{
		//    this.DropDownListMoreActions.Attributes["onchange"] = string.Format("return dropDownListSelected('{0}');", this.GridViewTask.ClientID);
		//    this.DropDownListMoreActions.Items.Clear();

		//    if (CurrentPageTaskStatus == TaskStatus.Yue)
		//    {
		//        FillTaskStateActionsToMoreActions();
		//    }
		//    FillCategoryActionsToMoreActions();
		//    FillTopActionsToMoreActions();
		//}

		///// <summary>
		///// 生成分类操作选项至更多操作下拉框
		// ///</summary>
		//private void FillCategoryActionsToMoreActions()
		//{
		//    /* 周杨于20090704注释
		//    this.DropDownListMoreActions.Items.Add(GetSeparatorItem("--分类操作--"));
		//    TaskCategoryCollection taskCategories = TaskCategoryAdapter.Instance.GetCategoriesByUserID(DeluxeIdentity.CurrentUser.ID);
		//    foreach (TaskCategory category in taskCategories)
		//    {
		//        this.DropDownListMoreActions.Items.Add(new ListItem(string.Format("作为\"{0}\"类", category.CategoryName), category.CategoryID));
		//    }
		//    this.DropDownListMoreActions.Items.Add(new ListItem("去除分类", "NULL"));
		//    this.DropDownListMoreActions.Items.Add(new ListItem("类别管理", "CategoryManage"));
		//    */
		//}

		///// <summary>
		///// 生成置顶操作选项至更多操作下拉框
		///// </summary>
		////private void FillTopActionsToMoreActions()
		////{
		////    this.DropDownListMoreActions.Items.Add(GetSeparatorItem("--置顶操作--"));
		////    this.DropDownListMoreActions.Items.Add(new ListItem("置顶", "SetTopFlag"));
		////    this.DropDownListMoreActions.Items.Add(new ListItem("取消置顶", "CancelTopFlag"));
		////}

		///// <summary>
		///// 生成消息处理选项至更多操作下拉框
		///// </summary>
		//private void FillTaskStateActionsToMoreActions()
		//{
		//    this.DropDownListMoreActions.Items.Add(GetSeparatorItem("--消息处理--"));
		//    this.DropDownListMoreActions.Items.Add(new ListItem("待阅转已阅", "SetTaskCompleted"));
		//}

		#region 更多操作的按钮
		//private void SetMoreActionsSeparatorStyle()
		//{
		//    foreach (ListItem item in this.DropDownListMoreActions.Items)
		//    {
		//        if (item.Value == "$$$ListSeparator$$$")
		//            item.Attributes["style"] = "color:#888888;";
		//    }
		//}

		///// <summary>
		///// 根据“更多操作”下拉框选中项对列表的选中项进行操作
		///// </summary>
		///// <param name="sender"></param>
		///// <param name="e"></param>
		//protected void DropDownListMoreActions_SelectedIndexChanged(object sender, EventArgs e)
		//{
		//    switch (this.DropDownListMoreActions.SelectedValue)
		//    {
		//        //设置置顶
		//        case "SetTopFlag":
		//            UserTaskAdapter.Instance.SetTaskAtTop(GridViewTask.SelectedKeys.ToArray());
		//            break;
		//        //取消置顶
		//        case "CancelTopFlag":
		//            UserTaskAdapter.Instance.CancelTaskAtTop(GridViewTask.SelectedKeys.ToArray());
		//            break;
		//        case "SetTaskCompleted":
		//            TaskCommon.SetUnreadCompleted(GridViewTask.SelectedKeys.ToArray());
		//            break;
		//        case "$$$ListSeparator$$$":
		//            //万一不慎选中分割符，啥也不做
		//            break;
		//        //为任务分类
		//        default:
		//            UserTaskAdapter.Instance.SetTaskCategory(this.DropDownListMoreActions.SelectedValue, GridViewTask.SelectedKeys.ToArray());
		//            break;
		//    }

		//    this.GridViewTask.SelectedKeys.Clear();
		//    this.DropDownListMoreActions.SelectedIndex = 0;
		//}

		//private ListItem GetSeparatorItem(string separatorText)
		//{
		//    ListItem separatorItem = new ListItem();
		//    //separatorItem.Attributes["style"] = "color:#888888;";
		//    separatorItem.Value = "$$$ListSeparator$$$";
		//    separatorItem.Text = separatorText;

		//    return separatorItem;
		//}
		//    SetMoreActionsSeparatorStyle(); 76行
		//InitMoreActions();Note：紧急程度
		// SetMoreActionsSeparatorStyle();

		///// <summary>
		///// 获取文件的文件类型描述(在config文件中配置)  //LDM
		///// </summary>
		//// public string GetFileType(UserTask userTask)
		////  {
		////string defaultFileType = "公共";todo：要实现的方案
		////string appName = ApplicationCategorySettings.GetConfig().GetCategoryName(userTask.ApplicationName, defaultFileType);
		////string programName = ApplicationCategorySettings.GetConfig().GetProgramName(userTask.ApplicationName, userTask.ProgramName, defaultFileType);
		////return appName + "/" + programName;
		////  return null;
		//// }
		#endregion


		//this.GridViewTask.Columns[0].Visible = false;
		//this.GridViewTask.Columns[7].Visible = false;
		// InitMoreActions();Note：紧急程度


		//枚举类型的活还得自己干
		//if (QueryCondition.Emergency >= 0)
		//{
		//    builder.AppendItem(//note:紧急程度注
		//    ((ConditionMappingAttribute)Attribute.GetCustomAttribute(typeof(TaskQueryCondition).GetProperty("Emergency"), typeof(ConditionMappingAttribute))).DataFieldName,
		//    (int)QueryCondition.Emergency);
		//}

		/*  周杨于20090704注释
		if (QueryCondition.CategoryID != string.Empty && QueryCondition.CategoryID != "Others")
		{
			builder.AppendItem("CATEGORY_GUID", QueryCondition.CategoryID);
		}

		选择了“未分类”
		if (QueryCondition.CategoryID == "Others")
		{
			whereCondition.Value += string.Format(" AND (CATEGORY_GUID IS NULL OR CATEGORY_GUID NOT IN (SELECT CATEGORY_GUID FROM USER_TASK_CATEGORY WHERE USER_ID = {0}))",
				TSqlBuilder.Instance.CheckQuotationMark(DeluxeIdentity.CurrentUser.ID, true));
		}
		*/

		#endregion
	}
}