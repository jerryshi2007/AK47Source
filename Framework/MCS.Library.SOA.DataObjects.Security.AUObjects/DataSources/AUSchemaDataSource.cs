using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MCS.Library.Data.Builder;
using System.Data;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.DataSources
{
	[DataObject]
	public class AUSchemaDataSource : AUDataViewDataSource
	{
		private string categoryID = null;

		public DataView Query(int startRowIndex, int maximumRows, string categoryID, string where, string orderBy, ref int totalCount)
		{
			this.categoryID = categoryID;
			return base.Query(startRowIndex, maximumRows, where, orderBy, ref totalCount);
		}

		public int GetQueryCount(string where, string categoryID, ref int totalCount)
		{
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
			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			var allConditions = new ConnectiveSqlClauseCollection(timeCondition, where);
			where.AppendCondition("S.CategoryID", this.categoryID);
			if (string.IsNullOrEmpty(qc.WhereClause) == false)
				qc.WhereClause += " AND ";
			qc.WhereClause += allConditions.ToSqlString(TSqlBuilder.Instance);
		}

		private string FromSqlClause
		{
			get
			{
				if (TimePointContext.Current.UseCurrentTime)
					return @"SC.AUSchemaSnapshot_Current S";
				else
					return @"SC.AUSchemaSnapshot S"; ;
			}
		}
	}
}
