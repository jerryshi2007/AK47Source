using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Principal;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Responsive.Library;

namespace MCSResponsiveOAPortal
{
    public partial class ActivatedList : System.Web.UI.Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.IsPostBack == false)
            {
                UserSettings userSettings = UserSettings.GetSettings(DeluxeIdentity.CurrentUser.ID);
                gridMain.PageSize = userSettings.GetPropertyValue("CommonSettings", "WorkingListPageSize", 20);
            }

            MapQueryString();
        }

        private void MapQueryString()
        {
            this.sfSubject.Value = this.Request.QueryString["sfSubject"];
            this.sfDept.Value = this.Request.QueryString["sfDept"];
            this.sfStart.Value = GetRequestData<DateTime>("sfStart", DateTime.MinValue);
            this.sfEnd.Value = GetRequestData<DateTime>("sfEnd", DateTime.MinValue);
        }

        protected void RefreshClick(object sender, EventArgs e)
        {
            this.gridMain.DataBind();
        }

        protected string GetTaskUrl(object o)
        {
            var r = (DataRowView)o;
            return UserTask.GetNormalizedUrl(r["APPLICATION_NAME"] as string, r["PROGRAM_NAME"] as string, r["URL"] as string);
        }

        T GetRequestData<T>(string field, T defaultValue)
        {
            string f = Request.QueryString[field];
            if (string.IsNullOrEmpty(f))
            {
                return defaultValue;
            }
            else
            {
                return (T)Convert.ChangeType(f, typeof(T));
            }
        }

        private string BuildWhereCondition()
        {
            TaskQueryCondition queryCondition = new TaskQueryCondition();
            queryCondition.DraftDepartmentName = this.Request.QueryString["sfDept"];
            queryCondition.TaskTitle = this.Request.QueryString["sfSubject"];
            queryCondition.CompletedTimeBegin = GetRequestData<DateTime>("sfStart", DateTime.MinValue);
            queryCondition.CompletedTimeEnd = GetRequestData<DateTime>("sfEnd", DateTime.MinValue);

            WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(queryCondition,
                new AdjustConditionValueDelegate(AdjustQueryConditionValue));

            builder.AppendItem("UT.STATUS", (int)TaskStatus.Ban);

            //// LDM 流转中数据的获取
            //if (null != Request.QueryString["process_status"] && Request.QueryString["process_status"] == "running")
            //    builder.AppendItem("PN.STATUS", "running");

            ////  LDM 已办结数据的获取
            //if (null != Request.QueryString["process_status"] && Request.QueryString["process_status"] == "completed")
            //    builder.AppendItem("ISNULL(PN.STATUS,N'completed')", "completed");

            if (queryCondition.ApplicationName == "全部")
            {
                queryCondition.ApplicationName = "";
            }

            string subjectQueryString = string.Empty;

            if (string.IsNullOrEmpty(queryCondition.TaskTitle) == false)
            {
                StringBuilder subjectSB = new StringBuilder();
                //关键词分割符为全角或半角空格
                char[] separators = new char[] { ' ', '　' };
                string[] wordsSplitted = queryCondition.TaskTitle.Split(separators, StringSplitOptions.RemoveEmptyEntries);
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

            processStatusBuilder.AppendItem("PN.STATUS", "Running");
            processStatusBuilder.AppendItem("PN.STATUS", "Maintaining");

            ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection();

            connectiveBuilder.Add(builder).Add(processStatusBuilder);

            string whereCondition = "";

            if (connectiveBuilder.IsEmpty)
                whereCondition = subjectQueryString;
            else if (subjectQueryString == string.Empty)
                whereCondition = connectiveBuilder.ToSqlString(TSqlBuilder.Instance);
            else
                whereCondition = connectiveBuilder.ToSqlString(TSqlBuilder.Instance) + " AND " + subjectQueryString;

            if (string.IsNullOrEmpty(queryCondition.DraftDepartmentName) == false)
            {
                whereCondition += string.Format(" AND CONTAINS(DRAFT_DEPARTMENT_NAME,'\"*"
                    + TSqlBuilder.Instance.CheckQuotationMark(queryCondition.DraftDepartmentName, false) + "*\"')");
            }

            whereCondition += string.Format(" AND SEND_TO_USER = {0}",
                                           TSqlBuilder.Instance.CheckQuotationMark(DeluxeIdentity.CurrentUser.ID, true));

            return whereCondition;
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

        ///// <summary>
        ///// 转义like有关的通配符
        ///// </summary>
        ///// <param name="condition">原查询条件</param>
        ///// <returns>转义后的查询条件</returns>
        //private static string EscapeLikeString(string condition)
        //{
        //    string result = condition;

        //    result = result.Replace("[", "[[]");
        //    result = result.Replace("-", "[-]");
        //    result = result.Replace("_", "[_]");
        //    result = result.Replace("%", "[%]");

        //    return result;
        //}

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
    }
}