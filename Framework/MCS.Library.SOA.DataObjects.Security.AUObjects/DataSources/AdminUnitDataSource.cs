using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.DataSources
{
	public class AdminUnitDataSource : AUDataViewDataSource
	{
		private string schemaID = null;
		private string parentID = null;

		public DataView Query(int startRowIndex, int maximumRows, string schemaID, string parentID, string where, string orderBy, ref int totalCount)
		{
			this.schemaID = schemaID;
			this.parentID = parentID;

			if (string.IsNullOrEmpty(this.parentID) && string.IsNullOrEmpty(schemaID))
				throw new ArgumentException("schemaID和parentID不能同时为空");

			if (string.IsNullOrEmpty(this.schemaID))
			{
				// 没有指定schemaID的情况下，推断schemaID
				AUCommon.DoDbAction(() =>
				{
					var obj = PC.Adapters.SchemaObjectAdapter.Instance.Load(parentID, DateTime.MinValue);
					if (obj is AUSchema)
					{
						this.schemaID = obj.ID;
					}
					else if (obj is AdminUnit)
					{
						this.schemaID = ((AdminUnit)obj).AUSchemaID;
					}
					else
					{
						throw new NotSupportedException("parentID既不是一个AdminUnit，也不是一个AUSchema");
					}
				});
			}
			else if (string.IsNullOrEmpty(this.parentID))
			{
				this.parentID = schemaID;
			}

			return base.Query(startRowIndex, maximumRows, where, orderBy, ref totalCount);
		}

		public int GetQueryCount(string where, string schemaID, string parentID, ref int totalCount)
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
			var timeCondition2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("R.");
			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder().NormalFor("S.Status").NormalFor("R.Status");
			var allConditions = new ConnectiveSqlClauseCollection(timeCondition, timeCondition2, where);
			where.AppendCondition("S.AUSchemaID", this.schemaID);
			where.AppendCondition("R.ParentID", this.parentID);
			if (string.IsNullOrEmpty(qc.WhereClause) == false)
				qc.WhereClause += " AND ";

			qc.WhereClause += allConditions.ToSqlString(TSqlBuilder.Instance);
		}

		private string FromSqlClause
		{
			get
			{
				if (TimePointContext.Current.UseCurrentTime)
					return @"SC.AdminUnitSnapshot_Current S INNER JOIN SC.SchemaRelationObjectsSnapshot_Current R ON S.ID = R.ObjectID ";
				else
					return @"SC.AdminUnitSnapshot S INNER JOIN SC.SchemaRelationObjectsSnapshot R ON S.ID = R.ObjectID "; ;
			}
		}
	}
}
