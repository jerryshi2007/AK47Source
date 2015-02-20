using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.DataSources
{
	public abstract class AUMemberViewDataSourceBase : AUDataViewDataSource
	{
		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			base.OnBuildQueryCondition(qc);
			qc.SelectFields = SelectFields;
			qc.FromClause = this.FromSqlClause;
			if (string.IsNullOrEmpty(qc.OrderByClause))
				qc.OrderByClause = DefaultOrderByClause; ;

			var timeCondition = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("S.");
			var timeCondition1 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("M.");
			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			var allConditions = new ConnectiveSqlClauseCollection(timeCondition, timeCondition1, where);
			BuildWhere(where);
			if (string.IsNullOrEmpty(qc.WhereClause) == false)
				qc.WhereClause += " AND ";
			qc.WhereClause += allConditions.ToSqlString(TSqlBuilder.Instance);
		}

		protected virtual void BuildWhere(WhereSqlClauseBuilder where)
		{
			where.AppendCondition("S.Status", (int)SchemaObjectStatus.Normal);
			where.AppendCondition("M.Status", (int)SchemaObjectStatus.Normal);
		}

		protected virtual string SelectFields { get { return "S.*"; } }

		protected virtual string FromSqlClause
		{
			get
			{
				if (TimePointContext.Current.UseCurrentTime)
				{
					return SnapshotTableName + @" S INNER JOIN SC.SchemaMembersSnapshot M ON M.MemberID = S.ID ";
				}
				else
				{
					return CurrentSnapshotTableName + @" S INNER JOIN SC.SchemaMembersSnapshot_Current M ON M.MemberID = S.ID ";
				}
			}
		}

		protected abstract string SnapshotTableName { get; }

		protected abstract string CurrentSnapshotTableName { get; }

		public virtual string DefaultOrderByClause
		{
			get
			{
				return "S.Name ASC";
			}
		}
	}
}
