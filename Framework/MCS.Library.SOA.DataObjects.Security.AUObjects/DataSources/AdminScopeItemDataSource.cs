using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.DataSources
{
	[DataObject]
	public class AdminScopeItemDataSource : AUDataViewDataSource
	{
		private string scopeType;
		public DataView Query(int startRowIndex, int maximumRows, string scopeType, string where, string orderBy, ref int totalCount)
		{
			scopeType.NullCheck("scopeType");
			this.scopeType = scopeType;
			return base.Query(startRowIndex, maximumRows, where, orderBy, ref totalCount);
		}

		public int GetQueryCount(string where, string scopeType, ref int totalCount)
		{
			return base.GetQueryCount(where, ref totalCount);
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			base.OnBuildQueryCondition(qc);
			qc.SelectFields = "*";
			qc.FromClause = TimePointContext.Current.UseCurrentTime ? "SC.AUAdminScopeItemSnapshot_Current" : "SC.AUAdminScopeItemSnapshot";

			if (string.IsNullOrEmpty(qc.OrderByClause))
				qc.OrderByClause = "AUScopeItemName ASC";

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
			builder.AppendCondition("SchemaType", this.scopeType).NormalFor("Status");
			var timeBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder();

			var allCondition = new ConnectiveSqlClauseCollection(builder, timeBuilder).ToSqlString(TSqlBuilder.Instance);

			if (string.IsNullOrEmpty(qc.WhereClause))
			{
				qc.WhereClause = allCondition;
			}
			else
			{
				qc.WhereClause = qc.WhereClause + "AND (" + allCondition + ")";
			}

		}
	}
}
