using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Security.DataSources
{
	public class ConditionalUsersDataSource : DataViewDataSourceQueryAdapterBase
	{
		private string orderBy;
		private string whereClause;
		private string containerId;

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			qc.FromClause = TimePointContext.Current.UseCurrentTime ? "SC.SchemaUserSnapshot_Current O INNER JOIN SC.UserAndContainerSnapshot_Current R ON O.ID = R.UserID" : "SC.SchemaUserSnapshot O INNER JOIN SC.UserAndContainerSnapshot R ON O.ID = R.UserID";
			qc.SelectFields = "O.*";
			if (string.IsNullOrEmpty(this.orderBy))
				qc.OrderByClause = "R.VersionStartTime DESC";
			else
				qc.OrderByClause = orderBy;
			base.OnBuildQueryCondition(qc);

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			var timeCondition1 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("R.");
			var timeCondition2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("O.");

			builder.AppendItem("R.ContainerID", this.containerId);
			InnerBuildWhere(builder);

			builder.AppendItem("O.Status", (int)SchemaObjectStatus.Normal);
			builder.AppendItem("R.Status", (int)SchemaObjectStatus.Normal);

			var allConditions = new ConnectiveSqlClauseCollection(timeCondition1, timeCondition2, builder);

			if (string.IsNullOrEmpty(qc.WhereClause))
			{
				qc.WhereClause = allConditions.ToSqlString(TSqlBuilder.Instance);
			}
			else
			{
				qc.WhereClause = allConditions.ToSqlString(TSqlBuilder.Instance) + " AND (" + qc.WhereClause + ")";
			}
		}

		protected override void OnAfterQuery(DataView result)
		{
			base.OnAfterQuery(result);
			DataSourceUtil.FillUserDefaultParent(result, "ID", "ParentID", this.GetConnectionName());
		}

		protected virtual void InnerBuildWhere(WhereSqlClauseBuilder builder)
		{
			builder.AppendItem<int>("R.Status", (int)SchemaObjectStatus.Normal);
		}

		public DataView Query(string containerId, string where, ref int totalCount, string orderBy, int startRowIndex, int maximumRows)
		{
			this.containerId = containerId;
			this.orderBy = orderBy;
			this.whereClause = where;
			return base.Query(startRowIndex, maximumRows, where, orderBy, ref totalCount);
		}

		public int GetQueryCount(string containerId, string where, ref int totalCount)
		{
			this.containerId = containerId;
			return base.GetQueryCount(where, ref totalCount);
		}

		protected override string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}
	}
}
