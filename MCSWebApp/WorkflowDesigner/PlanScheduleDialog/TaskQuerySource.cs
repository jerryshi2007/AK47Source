using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using System.Data;
using MCS.Library.Data.DataObjects;

namespace WorkflowDesigner.PlanScheduleDialog
{
	public class TaskQuerySource : DataViewDataSourceQueryAdapterBase
	{
		public TaskQuerySource()
			: base("WF.SYS_TASK")
		{
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			base.OnBuildQueryCondition(qc);
			if (string.IsNullOrEmpty(qc.OrderByClause))
			{
				qc.OrderByClause = "SORT_ID DESC";
			}
		}

		protected override string GetConnectionName()
		{
			return WorkflowSettings.GetConfig().ConnectionName;
		}
	}
}