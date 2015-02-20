using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Principal;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCSResponsiveOAPortal
{
    public partial class TodoList : System.Web.UI.Page
    {
        public TaskQueryCondition Condition
        {
            get
            {
                TaskQueryCondition condition = (TaskQueryCondition)this.ViewState["Condition"];
                if (condition == null)
                {
                    condition = new TaskQueryCondition();
                    this.ViewState["Condition"] = condition;
                }

                return condition;
            }

            set
            {
                this.ViewState["Condition"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Page.IsPostBack == false)
            {
                UserSettings userSettings = UserSettings.GetSettings(DeluxeIdentity.CurrentUser.ID);
                gridMain.PageSize = userSettings.GetPropertyValue("CommonSettings", "ToDoListPageSize", this.gridMain.PageSize);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            var advMode = this.Condition != null && this.Condition.MoreModeEnabled;
            this.searchFields.Attributes["class"] = advMode ? "mode-more" : "";
        }

        protected override void DataBindChildren()
        {
            int selIndex = this.sfProgramName.SelectedIndex;
            if (string.IsNullOrEmpty(sfAppName.SelectedValue) == false)
            {
                this.sfProgramName.Items.Clear();
                this.sfProgramName.Items.AddRange(ToOptions(WfApplicationAdapter.Instance.LoadProgramsByApplication(sfAppName.SelectedValue)));
            }

            base.DataBindChildren();

            this.sfProgramName.Value = this.Request.Form["sfProgramName"];
        }

        private ListItem[] ToOptions(WfProgramInApplicationCollection programs)
        {
            var result = new ListItem[programs.Count + 1];
            result[0] = new ListItem("全部", string.Empty);
            int i = 1;
            foreach (var item in programs)
            {
                result[i++] = new ListItem(item.Name, item.CodeName);
            }

            return result;
        }

        protected string GetTaskUrl(object o)
        {
            var r = (DataRowView)o;
            return UserTask.GetNormalizedUrl(r["APPLICATION_NAME"] as string, r["PROGRAM_NAME"] as string, r["URL"] as string);
        }

        private string BuildWhereCondition()
        {
            WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(this.Condition,
               new AdjustConditionValueDelegate(AdjustQueryConditionValue));

            builder.AppendItem("SEND_TO_USER", DeluxeIdentity.CurrentUser.ID);

            WhereSqlClauseBuilder whereExpireTime = new WhereSqlClauseBuilder(LogicOperatorDefine.Or);
            whereExpireTime.AppendItem("EXPIRE_TIME", "null", "is", true);
            whereExpireTime.AppendItem("EXPIRE_TIME", "getdate()", ">", true);

            WhereSqlClauseBuilder whereStatus = new WhereSqlClauseBuilder();
            whereStatus.AppendItem("STATUS", ((int)TaskStatus.Yue).ToString());

            var conditionGroup1 = new ConnectiveSqlClauseCollection(whereExpireTime, whereStatus);

            WhereSqlClauseBuilder whereStatus2 = new WhereSqlClauseBuilder();
            whereStatus2.AppendItem("STATUS", ((int)TaskStatus.Ban).ToString());
            //collection2.Add(collection1);
            ConnectiveSqlClauseCollection collection2 = new ConnectiveSqlClauseCollection(LogicOperatorDefine.Or, conditionGroup1, whereStatus2);

            var allConditions = new ConnectiveSqlClauseCollection(builder, collection2);
            return allConditions.ToSqlString(TSqlBuilder.Instance);
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
                case "ExpireTimeEnd":
                    result = ((DateTime)data).AddDays(1);
                    break;
                case "SourceID":
                    result = "(" + data + ")";
                    break;
                //case "TaskTitle":
                //    ignored = true;
                //    break;
            }

            return result;
        }

        protected void RefreshClick(object sender, EventArgs e)
        {
            this.gridMain.DataBind();
        }

        protected void dsMain_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (e.ExecutingSelectCount == false)
            {
                e.InputParameters["where"] = BuildWhereCondition();
            }
        }

        protected void dsMain_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {

        }

        protected void gridMain_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView row = (DataRowView)e.Row.DataItem;
                object lastReaded = row["READ_TIME"];
                if (lastReaded is DBNull || ((DateTime)lastReaded) == DateTime.MinValue)
                {
                    e.Row.CssClass += " unread";
                }
                else
                {
                    e.Row.CssClass += " read";
                }
            }
        }

        protected void SearchClick(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "Search")
            {
                TaskQueryCondition condition = new TaskQueryCondition();
                condition.TaskTitle = sfTitle.Text;

                if (e.CommandArgument.ToString() == "More")
                {
                    condition.DeliverTimeBegin = sfReceiveStart.Value;
                    condition.DeliverTimeEnd = sfReceiveEnd.Value;
                    condition.DraftDepartmentName = sfUnit.Text;
                    condition.ApplicationName = sfAppName.SelectedValue;
                    condition.ProgramName = sfProgramName.Value;
                    //condition.UserID  = sfDrafter.Text;
                    condition.DraftUserName = sfPreviousMan.Text;
                    condition.MoreModeEnabled = true;
                }

                this.Condition = condition;
                this.Page.DataBind();
            }
        }
    }
}