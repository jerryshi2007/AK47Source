using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.Builder;
using MCS.Library.Passport;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.OA.Stat.Common;
using MCS.Library.Data.DataObjects;

namespace MCS.OA.Stat.Query
{
	public class FormQuery : ObjectDataSourceQueryAdapterBase<AppCommonInfoProcess, AppCommonInfoProcessCollection>
	{

		protected override string GetConnectionName()
		{
			return ConnectionNameMappingSettings.GetConfig().GetConnectionName("HB2008", "");
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

			if (RolesDefineConfig.GetConfig().IsCurrentUserInRoles("ProcessAdmin", "AdminFormQuery", "WorkflowQueryAdmin") == false)
			{
				ConnectiveSqlClauseCollection cscc = WfAclAdapter.Instance.GetAclQueryConditionsByUser(DeluxeIdentity.CurrentUser.ID);

				string resourceIDList = string.Format("SELECT RESOURCE_ID FROM WF.ACL WHERE {0}", cscc.ToSqlString(TSqlBuilder.Instance));

				qc.WhereClause = string.Format("{0} AND RESOURCE_ID IN ({1})", qc.WhereClause, resourceIDList);
			}

			return qc.WhereClause;
		}
	}
}