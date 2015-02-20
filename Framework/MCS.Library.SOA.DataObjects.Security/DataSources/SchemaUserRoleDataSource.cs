using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Security.DataSources
{
	public class SchemaUserRoleDataSource : DataViewDataSourceQueryAdapterBase
	{
		private string userId;
		private string orderBy;
		private string whereClause;

		public SchemaUserRoleDataSource()
		{
		}

		protected override string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			qc.FromClause = TimePointContext.Current.UseCurrentTime ? @"SC.UserAndContainerSnapshot_Current C 
INNER JOIN SC.SchemaUserSnapshot_Current U ON U.ID =C.UserID
INNER JOIN SC.SchemaRoleSnapshot_Current R ON R.ID = C.ContainerID 
INNER JOIN SC.SchemaMembersSnapshot_Current S ON S.MemberID = R.ID
INNER JOIN  SC.SchemaApplicationSnapshot_Current A ON A.ID = S.ContainerID" : @"SC.UserAndContainerSnapshot C 
INNER JOIN SC.SchemaUserSnapshot U ON U.ID =C.UserID
INNER JOIN SC.SchemaRoleSnapshot R ON R.ID = C.ContainerID 
INNER JOIN SC.SchemaMembersSnapshot S ON S.MemberID = R.ID
INNER JOIN  SC.SchemaApplicationSnapshot A ON A.ID = S.ContainerID";
			qc.SelectFields = "R.*, A.ID AS  AppID,A.DisplayName AS AppName,A.Name As AppName2,A.CodeName as AppCodeName";
			base.OnBuildQueryCondition(qc);
			WhereSqlClauseBuilder wb = new WhereSqlClauseBuilder();
			wb.AppendItem("C.UserID", this.userId);
			if (string.IsNullOrWhiteSpace(orderBy))
			{
				qc.OrderByClause = "A.DisplayName";
			}
			else
			{
				qc.OrderByClause = orderBy;
			}
			if (string.IsNullOrWhiteSpace(qc.WhereClause))
			{
				qc.WhereClause = wb.ToSqlString(TSqlBuilder.Instance);
			}
			else
			{
				qc.WhereClause = wb.ToSqlString(TSqlBuilder.Instance) + " AND (" + qc.WhereClause + ")";
			}
		}

		public DataView Query(string userId, string where, ref int totalCount, string orderBy, int startRowIndex, int maximumRows)
		{
			this.userId = userId;
			this.orderBy = orderBy;
			this.whereClause = where;
			return base.Query(startRowIndex, maximumRows, where, orderBy, ref totalCount);
		}

		public int GetQueryCount(string userId, string where, ref int totalCount)
		{
			this.userId = userId;
			return base.GetQueryCount(where, ref totalCount);
		}
	}
}
