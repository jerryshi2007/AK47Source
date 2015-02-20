using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Security.DataSources
{
	public class SchemaDeletedUserDataSource : DataViewDataSourceQueryAdapterBase
	{
		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			qc.SelectFields = "U.* ";
			qc.FromClause = "SC.SchemaUserSnapshot U ";

			if (qc.OrderByClause.IsNullOrEmpty())
				qc.OrderByClause = "U.VersionStartTime DESC";

			qc.WhereClause.IsNotEmpty((s) => qc.WhereClause += " AND ");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
			builder.AppendItem("U.Status", (int)SchemaObjectStatus.Normal, "!=");

			var timeCondition = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("U.");

			var allConditions = new ConnectiveSqlClauseCollection(builder, timeCondition);

			qc.WhereClause += allConditions.ToSqlString(TSqlBuilder.Instance);
		}

		protected override string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}
	}
}
