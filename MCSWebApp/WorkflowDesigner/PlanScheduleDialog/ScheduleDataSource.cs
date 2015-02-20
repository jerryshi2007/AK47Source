using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using System.ComponentModel;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data.DataObjects;

namespace WorkflowDesigner.PlanScheduleDialog
{
	[DataObject]
	public class ScheduleDataSource : DataViewDataSourceQueryAdapterBase
	{
		protected override string GetConnectionName()
		{
			return WorkflowSettings.GetConfig().ConnectionName;
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			base.OnBuildQueryCondition(qc);
			qc.FromClause = "WF.JOB_SCHEDULE_DEF";
			if (string.IsNullOrEmpty(qc.OrderByClause))
				qc.OrderByClause = "START_TIME DESC";
			qc.SelectFields = "SCHEDULE_ID AS ID, SCHEDULE_NAME AS NAME, SCHEDULE_TYPE AS Type, START_TIME AS StartTime, END_TIME AS EndTime, ENABLED AS Enabled";
		}
	}
}