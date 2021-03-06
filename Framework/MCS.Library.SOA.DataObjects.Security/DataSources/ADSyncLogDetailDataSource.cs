﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using System.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Security.DataSources
{
	public class ADSyncLogDetailDataSource : DataViewDataSourceQueryAdapterBase
	{
		private string syncID;

		public ADSyncLogDetailDataSource()
			: base("SC.ADSynchronizeLogDetail")
		{
		}

		public DataView Query(string syncID, int startRowIndex, int maximumRows, string where, string orderBy, ref int totalCount)
		{
			this.syncID = syncID;
			return this.Query(startRowIndex, maximumRows, where, orderBy, ref totalCount);
		}

		public int GetQueryCount(string syncID, string where, ref int totalCount)
		{
			this.syncID = syncID;
			return this.GetQueryCount(where, ref totalCount);
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			base.OnBuildQueryCondition(qc);

			WhereSqlClauseBuilder wb = new WhereSqlClauseBuilder();
			wb.AppendItem("SynchronizeID", this.syncID);

			if (string.IsNullOrEmpty(qc.WhereClause))
				qc.WhereClause = wb.ToSqlString(TSqlBuilder.Instance);
			else
				qc.WhereClause = " AND " + wb.ToSqlString(TSqlBuilder.Instance);
		}

		protected override string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}
	}
}
