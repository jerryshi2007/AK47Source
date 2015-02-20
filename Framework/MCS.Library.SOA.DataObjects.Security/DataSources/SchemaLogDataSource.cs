using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Security.DataSources
{
	public class SchemaLogDataSource : DataViewDataSourceQueryAdapterBase
	{
		private string catelog;
		private string operationType;

		protected override string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}

		public DataView Query(int startRowIndex, int maximumRows, string where, string orderBy, string catelog, string operationType, ref int totalCount)
		{
			this.catelog = catelog;
			this.operationType = operationType;
			return base.Query(startRowIndex, maximumRows, where, orderBy, ref totalCount);
		}

		public int GetQueryCount(string where, string catelog, string operationType, ref int totalCount)
		{
			this.catelog = catelog;
			this.operationType = operationType;

			return base.GetQueryCount(where, ref totalCount);
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			qc.FromClause = "SC.OperationLog";
			qc.SelectFields = "*";
			if (string.IsNullOrEmpty(qc.OrderByClause))
				qc.OrderByClause = "CreateTime DESC";

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			if (this.catelog.IsNotEmpty())
			{
				builder.AppendItem("SchemaType", this.catelog);
			}

			if (this.operationType.IsNotEmpty())
			{
				builder.AppendItem("OperationType", operationType);
			}

			if (builder.IsEmpty == false && qc.WhereClause.IsNotEmpty())
			{
				qc.WhereClause += " AND ";
			}

			qc.WhereClause += builder.ToSqlString(TSqlBuilder.Instance);
		}
	}
}
