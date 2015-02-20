using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.DataObjects;

namespace MCS.OA.CommonPages.AsyncPersist
{
	public class AchivedDataSource : DataViewDataSourceQueryAdapterBase
	{
		public AchivedDataSource()
		{
		}

		protected override string GetConnectionName()
		{
			return WfRuntime.ProcessContext.SimulationContext.GetConnectionName(AppLogSettings.GetConfig().ConnectionName);
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			if (string.IsNullOrEmpty(qc.OrderByClause))
				qc.OrderByClause = "B.CREATE_TIME DESC";
			qc.SelectFields = "PROCESS_ID,PROCESS_NAME,B.CREATE_TIME, A.CREATE_TIME AS GEN_TIME,CREATOR_ID,CREATOR_NAME,START_TIME,DEPARTMENT_NAME,B.UPDATE_TAG";
			qc.FromClause = "WF.PERSIST_QUEUE_ARCHIEVED A (NOLOCK) INNER JOIN WF.PROCESS_INSTANCES B ON A.PROCESS_ID = B.INSTANCE_ID";
			base.OnBuildQueryCondition(qc);
		}
	}
}