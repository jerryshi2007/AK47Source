using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.DataSources
{
	/// <summary>
	/// 表示管理单元固定成员的数据源
	/// </summary>
	[DataObject]
	public class AUAdminScopeConstMemberDataSource : AUDataViewDataSource
	{
		private SchemaObjectBase scope;

		public DataView Query(int startRowIndex, int maximumRows, string unitID, string scopeType, string where, string orderBy, ref int totalCount)
		{
			unitID.NullCheck("unitID");
			scopeType.NullCheck("scopeType");

			this.scope = Adapters.AUSnapshotAdapter.Instance.LoadAUScope(unitID, scopeType, true, DateTime.MinValue).FirstOrDefault();
			if (this.scope == null || this.scope.Status != Schemas.SchemaProperties.SchemaObjectStatus.Normal)
				throw new AUObjectException("不存在指定的单元或者管理范围");

			return base.Query(startRowIndex, maximumRows, where, orderBy, ref totalCount);
		}

		public int GetQueryCount(string where, string unitID, string scopeType, ref int totalCount)
		{
			return base.GetQueryCount(where, ref totalCount);
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			base.OnBuildQueryCondition(qc);
			qc.SelectFields = "S.*";
			qc.FromClause = this.FromSqlClause;
			if (string.IsNullOrEmpty(qc.OrderByClause))
				qc.OrderByClause = "S.SearchContent ASC";

			var timeCondition = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("S.");
			var timeCondition2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("M.");
			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder().NormalFor("S.Status").NormalFor("M.Status");
			var allConditions = new ConnectiveSqlClauseCollection(timeCondition, timeCondition2, where);
			where.AppendCondition("M.ContainerID", this.scope.ID);
			if (string.IsNullOrEmpty(qc.WhereClause) == false)
				qc.WhereClause += " AND ";

			qc.WhereClause += allConditions.ToSqlString(TSqlBuilder.Instance);
		}

		private string FromSqlClause
		{
			get
			{
				if (TimePointContext.Current.UseCurrentTime)
					return @"SC.AUAdminScopeItemSnapshot_Current S INNER JOIN SC.SchemaMembersSnapshot_Current M ON S.ID = M.MemberID";
				else
					return @"SC.AUAdminScopeItemSnapshot S INNER JOIN SC.SchemaMembersSnapshot M ON S.ID = M.MemberID";
			}
		}
	}
}
