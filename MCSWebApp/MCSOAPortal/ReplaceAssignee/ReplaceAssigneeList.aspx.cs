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
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.Script;
using MCS.Library.Passport;

namespace MCS.OA.Portal.TaskList
{
    public partial class ReplaceAssigneeList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //检查是否有系统管理员权限
            ExceptionHelper.FalseThrow(DeluxePrincipal.Current.IsInRole(RolesDefineConfig.GetConfig().RolesDefineCollection["ProcessAdmin"].Roles), "你没有查看此页的权限");
            Response.Cache.SetNoStore();
            
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
            lblTitle.Text = "修改待办人";
            //LDM 在这里设置页面的页大小，原来的代码设定为固定值20
            UserSettings userSettings = UserSettings.GetSettings(DeluxeIdentity.CurrentUser.ID);
            GridViewTask.PageSize = userSettings.GetPropertyValue("CommonSettings", "ToDoListPageSize", this.GridViewTask.PageSize);
            
            //查询当前登录用户的待办事项
            ExecQuery("0");
        }

        //按userID查询待办事项
        private void ExecQuery(string userID)
        {
            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            builder.AppendItem("STATUS", (int)TaskStatus.Ban);
            builder.AppendItem("SEND_TO_USER", userID);
            
            whereCondition.Value = builder.ToSqlString(TSqlBuilder.Instance);

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
                GridCommon.SetRowStyleWhenMouseOver(e.Row);
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
            }
        }

        //执行修改
        protected void multiProcess_ExecuteStep(object data)
        {
            ReplaceAssigneeHelperData rahd = JSONSerializerExecute.Deserialize<ReplaceAssigneeHelperData>(data);
            ReplaceAssigneeHelper.ExecuteReplace(rahd);
        }

        //查询处理
        protected void btnQuery_Click(object sender, EventArgs e)
        {
            //将原始待办人ID存入hidden控件中
			if (originalUserSelector.SelectedSingleData != null)
			{
				this.hiddenOriginalUserID.Value = originalUserSelector.SelectedSingleData.ID;
			
				ExecQuery(this.hiddenOriginalUserID.Value);
			}
        }
    }
}