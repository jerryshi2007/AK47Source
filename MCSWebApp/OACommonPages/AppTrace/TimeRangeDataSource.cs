using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;
using MCS.Library.Data;
using System.Data;
using MCS.Library.Data.DataObjects;

namespace MCS.OA.CommonPages.AppTrace
{
	public class TimeRangeDataSource : ObjectDataSourceQueryAdapterBase<WfProcessCurrentInfo, WfProcessCurrentInfoCollection>
	{
		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			qc.FromClause = "WF.PROCESS_INSTANCES (NOLOCK)";

			if (qc.OrderByClause.IsNullOrEmpty())
				qc.OrderByClause = "CREATE_TIME DESC";

			qc.SelectFields = ORMapping.GetSelectFieldsNameSql<WfProcessCurrentInfo>();
		}

		protected override string GetConnectionName()
		{
			return WorkflowSettings.GetConfig().ConnectionName;
		}

		public static string[] QueryGuidsByCondition(IConnectiveSqlClause condition)
		{
			QueryCondition qc = new QueryCondition();
			qc.FromClause = "WF.PROCESS_INSTANCES (NOLOCK)";
			qc.SelectFields = "INSTANCE_ID";
			qc.OrderByClause = "CREATE_TIME DESC";
			qc.WhereClause = condition.ToSqlString(TSqlBuilder.Instance);

			string sql = string.Format("SELECT {0} FROM {1} WHERE 1 = 1 {2} {3} ORDER BY {4}",
						qc.SelectFields,
						qc.FromClause,
						qc.WhereClause.IsNotEmpty() ? " AND " + qc.WhereClause : string.Empty,
						qc.GroupBy.IsNotEmpty() ? "GROUP BY " + qc.GroupBy : string.Empty,
						qc.OrderByClause);

			using (DbContext context = DbContext.GetContext(WorkflowSettings.GetConfig().ConnectionName))
			{
				Database db = DatabaseFactory.Create(context);

				using (var dr = db.ExecuteReader(CommandType.Text, sql))
				{
					List<string> guids = new List<string>();
					while (dr.Read())
					{
						guids.Add(dr.GetString(0));
					}

					return guids.ToArray();
				}
			}
		}
	}
}