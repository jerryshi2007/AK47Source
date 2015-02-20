using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Responsive.Library;
using System.Web.UI.HtmlControls;
using System.Text;
using MCS.Library.Passport;

namespace MCSResponsiveOAPortal
{
    public partial class AdvancedSearch : System.Web.UI.Page
    {
        private static bool MostSupervisor
        {
            get
            {
                return RolesDefineConfig.GetConfig().IsCurrentUserInRoles("ProcessAdmin");
            }
        }

        private static bool NormalSupervisior
        {
            get
            {
                return RolesDefineConfig.GetConfig().IsCurrentUserInRoles("ProcessAdmin", "AdminFormQuery", "WorkflowQueryAdmin");
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            MapQueryString();
        }

        private void MapQueryString()
        {
            this.sfSubject.Value = this.Request.QueryString["sfSubject"];
            this.sfDept.Value = this.Request.QueryString["sfDept"];
            this.sfStart.Value = GetRequestData<DateTime>("sfStart", DateTime.MinValue);
            this.sfEnd.Value = GetRequestData<DateTime>("sfEnd", DateTime.MinValue);
            this.sfApplicant.Value = this.Request.QueryString["sfApplicant"];
            this.BindSelection();
        }

        private void BindSelection()
        {
            this.sfAppName.Items.Clear();
            this.sfAppName.Items.AddRange(ToOptions(WfApplicationAdapter.Instance.LoadAll()));
            string appName = this.Request[sfAppName.ClientID];
            if (string.IsNullOrEmpty(appName) == false)
            {
                this.sfProgramName.Items.Clear();
                this.sfProgramName.Items.AddRange(ToOptions(WfApplicationAdapter.Instance.LoadProgramsByApplication(appName)));
            }

            this.sfAppName.Value = appName;
            this.sfProgramName.Value = this.Request[sfProgramName.ClientID];
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

        protected string GetTaskUrl(object item)
        {
            var r = (DataRowView)item;
            return UserTask.GetNormalizedUrl(r["APPLICATION_NAME"] as string, r["PROGRAM_NAME_MCS"] as string, r["URL"] as string);
        }

        private ListItem[] ToOptions(WfApplicationCollection apps)
        {
            var result = new ListItem[apps.Count + 1];
            result[0] = new ListItem("全部", string.Empty);
            int i = 1;
            foreach (var item in apps)
            {
                result[i++] = new ListItem(item.Name, item.CodeName);
            }

            return result;
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

        protected void ObjectDataSourceFormQuery_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["sfSearch"]))
            {
                e.Cancel = true;
            }
            else if (e.ExecutingSelectCount == false)
            {
                e.InputParameters["where"] = BuildWhere();
            }

        }

        private object BuildWhere()
        {
            DbConnectionMappingContext.ClearAllMappings();

            FormQueryCondition queryCondition = new FormQueryCondition()
            {
                Subject = this.Request.QueryString["sfSubject"],
                CreateTimeBegin = GetRequestData<DateTime>("sfStart", DateTime.MinValue),
                CreateTimeEnd = GetRequestData<DateTime>("sfEnd", DateTime.MinValue),
                DraftDepartmentName = this.Request.QueryString["sfDept"] ?? string.Empty,
                CurrentUsersName = this.Request.QueryString["sfApplicant"] ?? string.Empty,
                ApplicationName = this.Request.QueryString[sfAppName.ClientID],
                ProgramName_SinoOcean = this.Request.QueryString[sfProgramName.ClientID]

            };

            WhereSqlClauseBuilder builder = ConditionMapping.GetWhereSqlClauseBuilder(queryCondition,
                new AdjustConditionValueDelegate(AdjustQueryConditionValue));

            //申请部门
            if (string.IsNullOrEmpty(queryCondition.DraftDepartmentName) == false)
            {
                builder.AppendItem("CONTAINS(DRAFT_DEPARTMENT_NAME," + GetFullTextParameter(queryCondition.DraftDepartmentName) + ")", "", "", true);
            }

            var allConditions = new ConnectiveSqlClauseCollection(builder);

            //if (MostSupervisor == false || NormalSupervisior == false)
            //{
            //    //检查是否具有分类授权
            //    WfApplicationAuthCollection authInfo = WfApplicationAuthAdapter.Instance.GetUserApplicationAuthInfo(DeluxeIdentity.Current.User);
            //    var builder2 = authInfo.GetApplicationAndProgramBuilder("APPLICATION_NAME", "PROGRAM_NAME");
            //    allConditions.Add(builder2);
            //}

            string whereCondition = allConditions.ToSqlString(TSqlBuilder.Instance);

            return whereCondition;
        }

        private static string GetFullTextParameter(string query)
        {
            string result = null;
            if (string.IsNullOrEmpty(query) == false)
            {
                string[] parts = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    StringBuilder fullTextBuilder = new StringBuilder();
                    fullTextBuilder.Append("'");
                    int i;
                    for (i = 0; i < parts.Length - 1; i++)
                    {
                        if (parts[i].Length > 0)
                        {
                            fullTextBuilder.AppendFormat("\"{0}\"", parts[i].Replace("\"", string.Empty));
                            fullTextBuilder.Append(" AND ");
                        }
                    }

                    fullTextBuilder.AppendFormat("\"{0}\"", parts[parts.Length - 1].Replace("\"", string.Empty));

                    fullTextBuilder.Append("'");
                    result = fullTextBuilder.ToString();
                }
            }

            return result;
        }

        protected void ObjectDataSourceFormQuery_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
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
                //标题使用全文检索，格式单独处理
                case "DraftDepartmentName":
                    ignored = true;
                    break;
            }

            return result;
        }
    }
}