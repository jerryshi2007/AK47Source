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
	public class SchemaRoleMembersDataSource : DataViewDataSourceQueryAdapterBase
	{
		private string _SearchRoleID;

		public DataView Query(int startRowIndex, int maximumRows, string roleId, string where, string orderBy, ref int totalCount)
		{
			this._SearchRoleID = roleId;
			return base.Query(startRowIndex, maximumRows, where, orderBy, ref totalCount);
		}

		public int GetQueryCount(string roleId, string where, ref int totalCount)
		{
			this._SearchRoleID = roleId;
			return base.GetQueryCount(where, ref totalCount);
		}

		protected override string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}

		protected override void OnAfterQuery(DataView result)
		{
			base.OnAfterQuery(result);
			DataSourceUtil.FillUserDefaultParent(result, "ID", "ParentID", this.GetConnectionName());
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			if (string.IsNullOrEmpty(qc.OrderByClause))
				qc.OrderByClause = "InnerSort";

			qc.SelectFields = "*";

			string where = qc.WhereClause;
			if (this._SearchRoleID.IsNotEmpty())
			{
				if (where.IsNotEmpty())
					where += " AND ";

				qc.WhereClause = where + "ContainerID = " + TSqlBuilder.Instance.CheckUnicodeQuotationMark(this._SearchRoleID);
			}

			qc.FromClause = BuildFromSqlClause(qc);
			qc.WhereClause = string.Empty;
		}

		private static string BuildFromSqlClause(QueryCondition qc)
		{
			Dictionary<string, string> tableProcessed = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			SchemaDefineCollection schemas = new SchemaDefineCollection();
			schemas.LoadFromConfiguration();


			return "(" + GetSnapshotTableQuery(qc) + ") SchemaObjects";
		}

		private static string GetSnapshotTableQuery(QueryCondition qc)
		{
			StringBuilder strB = new StringBuilder(256);

			string whereClause = qc.WhereClause;

			bool usingCurrentTime = TimePointContext.Current.UseCurrentTime;
			strB.AppendLine("SELECT R.ContainerID, O.ID, O.Name, O.DisplayName, O.CodeName, O.SchemaType, O.Status, O.VersionStartTime, O.VersionEndTime,O.CreatorID,O.CreatorName,O.CreateDate, R.InnerSort");
			strB.AppendFormat("FROM SC.SchemaObjectSnapshot{0} O INNER JOIN SC.SchemaMembersSnapshot{1} R ON O.ID = R.MemberID AND O.SchemaType = R.MemberSchemaType\n", usingCurrentTime ? "_Current" : "", usingCurrentTime ? "_Current" : "");
			strB.Append("WHERE 1 = 1 ");

			//WhereSqlClauseBuilder buildCommon = new WhereSqlClauseBuilder(LogicOperatorDefine.And);
			//buildCommon.AppendItem("R.ContainerSchemaType", "Roles");
			//buildCommon.AppendItem("R.MemberSchemaType", schema.Category);

			//strB.Append(" AND " + buildCommon.ToSqlString(TSqlBuilder.Instance));

			strB.Append(" AND " + DataSourceUtil.SchemaTypeCondition("R.ContainerSchemaType", "Roles").ToSqlString(TSqlBuilder.Instance));
			//strB.Append(" AND " + DataSourceUtil.SchemaTypeCondition("R.MemberSchemaType", schema.Name).ToSqlString(TSqlBuilder.Instance));

			var builderObj = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("O.");

			strB.Append(" AND " + builderObj.ToSqlString(TSqlBuilder.Instance));

			var builderRelation = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("R.");

			strB.Append(" AND " + builderRelation.ToSqlString(TSqlBuilder.Instance));

			if (whereClause.IsNotEmpty())
				strB.Append(" AND " + whereClause);
			return strB.ToString();
		}
	}
}
