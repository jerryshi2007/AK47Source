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
	public class SchemaApplicationDataSource : DataViewDataSourceQueryAdapterBase
	{
		public SchemaApplicationDataSource()
			: base(TimePointContext.Current.UseCurrentTime ? "SC.SchemaApplicationSnapshot_Current" : "SC.SchemaApplicationSnapshot")
		{
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			if (qc.OrderByClause.IsNullOrEmpty())
				qc.OrderByClause = "VersionStartTime DESC";

			qc.WhereClause.IsNotEmpty((s) => qc.WhereClause += " AND ");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			var timeCondition = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder();

			builder.AppendItem("Status", (int)SchemaObjectStatus.Normal);

			builder.AppendItem("SchemaType", "Applications");

			var conditions = new ConnectiveSqlClauseCollection(timeCondition, builder);

			qc.WhereClause += conditions.ToSqlString(TSqlBuilder.Instance);
		}

		protected override string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}
	}
}
