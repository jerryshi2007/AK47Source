using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Data.DataObjects;

namespace PermissionCenter.DataSources
{
	public class ScheduleDataSource : DataViewDataSourceQueryAdapterBase
	{
		public ScheduleDataSource()
			: base("WF.JOBS")
		{
		}

		protected override string GetConnectionName()
		{
			return "HB2008";
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			base.OnBuildQueryCondition(qc);
			if (string.IsNullOrEmpty(qc.OrderByClause))
			{
				qc.OrderByClause = "CREATE_TIME DESC";
			}
		}
	}
}