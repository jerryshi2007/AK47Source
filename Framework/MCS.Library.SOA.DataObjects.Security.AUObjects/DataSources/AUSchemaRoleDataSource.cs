using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.DataSources
{
	[DataObject]
	public class AUSchemaRoleDataSource : AUDataViewDataSource
	{
		private string schemaID;
		private bool normalOnly;

		public DataView Query(int startRowIndex, int maximumRows, string schemaID, bool normalOnly, string where, string orderBy, ref int totalCount)
		{
			this.schemaID = schemaID;
			this.normalOnly = normalOnly;

			return base.Query(startRowIndex, maximumRows, where, orderBy, ref totalCount);
		}

		public int GetQueryCount(string where, string schemaID, bool normalOnly, ref int totalCount)
		{
			this.schemaID = schemaID;
			this.normalOnly = normalOnly;

			return base.GetQueryCount(where, ref totalCount);
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			base.OnBuildQueryCondition(qc);
			qc.SelectFields = "S.*";
			qc.FromClause = this.FromSqlClause;
			if (string.IsNullOrEmpty(qc.OrderByClause))
				qc.OrderByClause = "S.Name ASC";

			var timeCondition = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("S.");
			var timeConditionM = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("M.");
			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			var allConditions = new ConnectiveSqlClauseCollection(timeCondition, timeConditionM, where);
			where.AppendCondition("M.ContainerID", this.schemaID);

			if (normalOnly)
			{
				where.AppendCondition("S.Status", (int)SchemaObjectStatus.Normal);
				where.AppendCondition("M.Status", (int)SchemaObjectStatus.Normal);
			}

			if (string.IsNullOrEmpty(qc.WhereClause) == false)
				qc.WhereClause += " AND ";
			qc.WhereClause += allConditions.ToSqlString(TSqlBuilder.Instance);
		}

		private string FromSqlClause
		{
			get
			{
				if (TimePointContext.Current.UseCurrentTime && normalOnly)
					return @"SC.AUSchemaRoleSnapshot_Current S INNER JOIN SC.SchemaMembersSnapshot M ON M.MemberID = S.ID AND S.SchemaType = M.MemberSchemaType";
				else
					return @"SC.AUSchemaRoleSnapshot S INNER JOIN SC.SchemaMembersSnapshot_Current M ON M.MemberID = S.ID AND S.SchemaType = M.MemberSchemaType";
			}
		}
	}
}
