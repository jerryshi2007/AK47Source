using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MCS.Library.Data.Builder;
using MCS.Library.Passport;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using System.ComponentModel;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.DataObjects;

namespace MCSResponsiveOAPortal.DataSources
{
    [DataObject]
    public sealed class AdvancedSearchDataSource : DataViewDataSourceQueryAdapterBase
    {
        protected override string GetConnectionName()
        {
            return ConnectionDefine.DBConnectionName;
        }

        protected override void OnBuildQueryCondition(QueryCondition qc)
        {
            if (qc.OrderByClause == string.Empty)
                qc.OrderByClause = "CREATE_TIME DESC";

            qc.SelectFields = @" *";

            if (string.IsNullOrEmpty(qc.WhereClause))
            {
                qc.FromClause = @" (SELECT DISTINCT ACI.[APPLICATION_NAME],ACI.[PROGRAM_NAME] AS [PROGRAM_NAME_MCS],
												ACI.[RESOURCE_ID],ACI.[SUBJECT],ACI.[EMERGENCY],ACI.[URL],ACI.[CREATOR_ID],ACI.[CREATOR_NAME],
			        ACI.[CREATE_TIME],ACI.[DRAFT_DEPARTMENT_NAME],CIM.[PROCESS_ID] FROM  wf.APPLICATIONS_COMMON_INFO AS ACI (NOLOCK) LEFT JOIN wf.COMMON_INFO_MAPPING AS CIM (NOLOCK) ON ACI.RESOURCE_ID = CIM.COMMON_INFO_ID) AS T";
            }
            else
            {
                qc.FromClause = string.Format(@" (SELECT DISTINCT ACI.[APPLICATION_NAME],ACI.[PROGRAM_NAME] AS [PROGRAM_NAME_MCS],
												ACI.[RESOURCE_ID],ACI.[SUBJECT],ACI.[EMERGENCY],ACI.[URL],ACI.[CREATOR_ID],ACI.[CREATOR_NAME],
			        ACI.[CREATE_TIME],ACI.[DRAFT_DEPARTMENT_NAME],CIM.[PROCESS_ID] FROM  wf.APPLICATIONS_COMMON_INFO AS ACI (NOLOCK) LEFT JOIN wf.COMMON_INFO_MAPPING AS CIM (NOLOCK) ON ACI.RESOURCE_ID = CIM.COMMON_INFO_ID WHERE {0}) AS T ", qc.WhereClause);

                qc.WhereClause = string.Empty;
            }

            qc.WhereClause = GetFilterByQuery(qc);
            base.OnBuildQueryCondition(qc);

        }

        public string GetFilterByQuery(QueryCondition qc)
        {
            if (string.IsNullOrEmpty(qc.WhereClause))
                qc.WhereClause = "1 = 1";

            var addition = string.IsNullOrEmpty(qc.WhereClause) ? "1 = 1" : qc.WhereClause;

            if (RolesDefineConfig.GetConfig().IsCurrentUserInRoles("ProcessAdmin", "AdminFormQuery", "WorkflowQueryAdmin") == false)
            {
                ConnectiveSqlClauseCollection cscc = WfAclAdapter.Instance.GetAclQueryConditionsByUser(DeluxeIdentity.CurrentUser.ID);

                string condition = "RESOURCE_ID IN (SELECT RESOURCE_ID FROM WF.ACL WHERE " + cscc.ToSqlString(TSqlBuilder.Instance) + ")";

                //检查是否具有分类授权
                WfApplicationAuthCollection authInfo = WfApplicationAuthAdapter.Instance.GetUserApplicationAuthInfo(DeluxeIdentity.Current.User);
                var cateCondition = authInfo.GetApplicationAndProgramBuilder("APPLICATION_NAME", "PROGRAM_NAME_MCS").ToSqlString(TSqlBuilder.Instance);
                if (string.IsNullOrEmpty(cateCondition) == false)
                {
                    condition = "(" + condition + " OR " + cateCondition + ")";
                }

                addition += " AND " + condition;

                qc.WhereClause = addition;
            }

            return qc.WhereClause;
        }

        protected override void OnAfterQuery(System.Data.DataView result)
        {
            base.OnAfterQuery(result);

            List<string> resourceIDList = new List<string>();
            List<string> processIDList = new List<string>();

            foreach (DataRowView rowView in result)
            {
                if (!resourceIDList.Contains(rowView["RESOURCE_ID"].ToString()))
                    resourceIDList.Add(rowView["RESOURCE_ID"].ToString());

                if (!processIDList.Contains(rowView["PROCESS_ID"].ToString()))
                    processIDList.Add(rowView["PROCESS_ID"].ToString());
            }

            var atpc = new WfProcessCurrentInfoCollection();

            WfProcessCurrentInfoCollection resourceAtpc = WfProcessCurrentInfoAdapter.Instance.Load(resourceIDList.ToArray());
            WfProcessCurrentInfoCollection processAtpc = WfProcessCurrentInfoAdapter.Instance.LoadByProcessID(processIDList.ToArray());

            resourceAtpc.ForEach(atp =>
            {
                if (atpc.Exists(p => p.InstanceID == atp.InstanceID) == false)
                    atpc.Add(atp);
            });

            processAtpc.ForEach(atp =>
            {
                if (atpc.Exists(p => p.InstanceID == atp.InstanceID) == false)
                    atpc.Add(atp);
            });

            result.Table.Columns.Add("Status");

            foreach (DataRowView rowView in result)
            {
                var processInfo = atpc.Find(p => p.InstanceID == rowView["PROCESS_ID"].ToString() || p.ResourceID == rowView["RESOURCE_ID"].ToString());

                rowView["Status"] = processInfo != null ? (int)processInfo.Status : 0;
            }
        }
    }
}