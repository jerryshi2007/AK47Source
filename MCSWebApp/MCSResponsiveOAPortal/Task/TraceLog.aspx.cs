using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Principal;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;

namespace MCSResponsiveOAPortal
{
    public partial class TraceLog : System.Web.UI.Page
    {
        private LogQueryCondition QueryCondition
        {
            get
            {
                LogQueryCondition result = (LogQueryCondition)ViewState["QueryCondition"];

                if (null == result)
                {
                    result = new LogQueryCondition();
                    ViewState["QueryCondition"] = result;
                }
                return result;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.IsPostBack == false)
            {
                UserSettings userSettings = UserSettings.GetSettings(DeluxeIdentity.CurrentUser.ID);
                gridMain.PageSize = userSettings.GetPropertyValue("CommonSettings", "ToDoListPageSize", 20);

                string rid = this.Request.QueryString["resourceID"];
                string pid = this.Request.QueryString["processID"];
                bool hit = false;

                if (string.IsNullOrEmpty(rid) == false)
                {
                    this.QueryCondition.ResourceID = rid;
                    hit = true;
                }

                if (string.IsNullOrEmpty(pid) == false)
                {
                    this.QueryCondition.ProcessID = pid;
                    hit = true;
                }

                if (!hit)
                {
                    this.QueryCondition.TagNull = 2;
                }
            }
        }

        protected void dsMain_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (e.ExecutingSelectCount == false)
            {
                e.InputParameters["where"] = BuildWhereCondition();
            }
        }

        private string BuildWhereCondition()
        {
            WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(QueryCondition,
                 new AdjustConditionValueDelegate(AdjustQueryConditionValue));

            //对数据源中的where进行赋值操作，自动完成查询操作
            string condition = builder.ToSqlString(TSqlBuilder.Instance);

            if (string.IsNullOrEmpty(condition))
            {
                condition = "RESOURCE_ID IS NOT NULL";
            }
            else
            {
                condition = condition + " AND RESOURCE_ID IS NOT NULL";
            }

            return condition;
        }

        private static object AdjustQueryConditionValue(string propertyName, object data, ref bool ignored)
        {
            object result = data;
            switch (propertyName)
            {
                case "Title":
                    result = string.Format("%{0}%", (string)data);
                    break;
                case "ActivityName":
                    result = string.Format("%{0}%", (string)data);
                    break;
                case "EndDate":
                    result = ((DateTime)data).AddDays(1);
                    break;
                case "Operator":
                    result = "(" + data + ")";
                    break;
            }
            return result;
        }
    }
}