using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Principal;
using System.Text;
using MCS.Library.Passport;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.Builder;
using System.Data;
using System.Collections.Specialized;
using MCS.Library.Core;

namespace MCSResponsiveOAPortal
{
    public partial class SearchResult : System.Web.UI.Page
    {
        private DataSet ds;

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

            string fullText = GetFullTextParameter(this.Request.QueryString["query"]);
            if (fullText != null)
            {
                this.ds = DataSources.FullTextSearcher.Instance.SearchFullDataByQuery(fullText);
            }
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

        protected string GetNormalizedUrl(string appCodeName, string progCodeName, string url)
        {
            if (NormalSupervisior)
            {
                NameValueCollection reqParams = UriHelper.GetUriParamsCollection(url);

                reqParams["mode"] = "Admin";

                url = UriHelper.CombineUrlParams(url, reqParams);
            }

            return UserTask.GetNormalizedUrl(appCodeName, progCodeName, url);
        }

        protected void dsMain_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (e.ExecutingSelectCount == false)
            {
                string where = "1=1";

                if (MostSupervisor == false || NormalSupervisior == false)
                {
                    ConnectiveSqlClauseCollection cscc = WfAclAdapter.Instance.GetAclQueryConditionsByUser(DeluxeIdentity.CurrentUser.ID);

                    string resourceIDList = "SELECT RESOURCE_ID FROM WF.ACL WHERE " + cscc.ToSqlString(TSqlBuilder.Instance);

                    where = "ACI.RESOURCE_ID IN (" + resourceIDList + ")";

                    //检查是否具有分类授权
                    WfApplicationAuthCollection authInfo = WfApplicationAuthAdapter.Instance.GetUserApplicationAuthInfo(DeluxeIdentity.Current.User);
                    var builder = authInfo.GetApplicationAndProgramBuilder("APPLICATION_NAME", "PROGRAM_NAME");
                    if (builder.IsEmpty == false)
                    {
                        where = "(" + where + " OR (" + builder.ToSqlString(TSqlBuilder.Instance) + "))";
                    }
                }

                string fullText = GetFullTextParameter(Request.QueryString["query"]);
                if (fullText != null)
                    where += " AND CONTAINS(ACI.*," + fullText + ")";

                e.InputParameters["where"] = where;
            }
        }

        protected void dsMain_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            int o = (int)e.OutputParameters["totalCount"];
            if (o > 0)
                this.txtSummary.Text = string.Format("共有约{0}条记录", o.ToString("N0"));
        }
    }
}