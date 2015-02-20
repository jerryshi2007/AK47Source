using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.Builder;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Data.DataObjects;

namespace MCS.OA.Portal.Common
{
	public class CompleteListQuery : ObjectDataSourceQueryAdapterBase<ExtendedUserTask, ExtendedUserTaskCollection>
	{

		protected override string GetConnectionName()
		{
			return ConnectionDefine.SearchConnectionName;
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			qc.SelectFields = @"UT.[PROCESS_ID], UT.[TASK_GUID], UT.[APPLICATION_NAME],UT.[DRAFT_USER_ID],UT.[DRAFT_USER_NAME],UT.[PROGRAM_NAME], UT.[TASK_TITLE], UT.[URL], UT.[EMERGENCY], UT.[PURPOSE], UT.[SOURCE_NAME], UT.[READ_TIME], UT.[RESOURCE_ID], UT.[COMPLETED_TIME], UT.[DRAFT_DEPARTMENT_NAME],UT.[STATUS],UT.[SEND_TO_USER],PN.[OWNER_ACTIVITY_ID], P.PARAM_VALUE AS PROJECT_NAME";
			qc.OrderByClause = GetOrderByString(qc.OrderByClause);
			qc.FromClause = @"WF.PROCESS_INSTANCES(NOLOCK) PN RIGHT JOIN WF.USER_ACCOMPLISHED_TASK (NOLOCK) UT ON UT.[PROCESS_ID] = PN.[INSTANCE_ID] LEFT JOIN WF.PROCESS_RELATIVE_PARAMS(NOLOCK) P ON UT.PROCESS_ID = P.PROCESS_ID AND P.PARAM_KEY = 'ProjectName'";

			base.OnBuildQueryCondition(qc);
		}

		protected string GetOrderByString(string inputOrderBy)
		{
			if (string.IsNullOrEmpty(inputOrderBy))
				inputOrderBy = " UT.COMPLETED_TIME DESC";
			else
				inputOrderBy = "UT." + inputOrderBy;

			return inputOrderBy;
		}
	}
}