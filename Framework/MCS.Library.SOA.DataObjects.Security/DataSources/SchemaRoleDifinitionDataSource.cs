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
	/// <summary>
	/// 表示角色的权限定义的数据源
	/// </summary>
	public class SchemaRoleDifinitionDataSource : DataViewDataSourceQueryAdapterBase
	{
		#region 参考查询原型
		/*查询的原型
         * --查询该应用所有的权限
SELECT Q1.*,Q2.ObjectID AS ApplyID
FROM(
SELECT P1.*,M1.ContainerID as AppID
FROM SC.SchemaMembersSnapshot M1
INNER JOIN SC.SchemaPermissionSnapshot P1 ON M1.MemberID = P1.ID
--LEFT JOIN SC.SchemaRelationObjectsSnapshot R2 ON R2.ObjectID = P1.ID
WHERE 
M1.Status = 1
AND M1.ContainerSchemaType = 'Applications' AND M1.MemberSchemaType = 'Permissions'

AND M1.ContainerID = 
(SELECT M.ContainerID FROM
SC.SchemaMembersSnapshot M
WHERE M.MemberID = 'c4da52e6-ec93-bc45-4ac9-a1f2faa151bf'
AND M.ContainerSchemaType ='Applications'
AND M.MemberSchemaType = 'Roles'
AND M.Status = 1
)

) Q1 LEFT JOIN (
--查现有的权限
SELECT *
FROM SC.SchemaRelationObjectsSnapshot R2 
WHERE R2.ParentSchemaType = 'Roles'
AND R2.ChildSchemaType = 'Permissions'
AND R2.Status = 1
AND R2.ParentID = 'c4da52e6-ec93-bc45-4ac9-a1f2faa151bf'
) Q2 ON Q1.ID = Q2.ObjectID
         */
		#endregion
		private string roleId;

		protected override string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}

		public DataView Query(string roleId, string where, ref int totalCount, string orderBy, int startRowIndex, int maximumRows)
		{
			if (roleId == null)
				throw new ArgumentNullException("roleId");
			this.roleId = roleId;
			return base.Query(startRowIndex, maximumRows, where, orderBy, ref totalCount);
		}

		public int GetQueryCount(string roleId, string where, ref int totalCount)
		{
			this.roleId = roleId;
			return base.GetQueryCount(where, ref totalCount);
		}

		protected override void OnBuildQueryCondition(QueryCondition qc)
		{
			StringBuilder sb = new StringBuilder(1024);
			var wbP1 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("P1.");
			var wbM = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("M.");
			var wbM1 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("M1.");
			var wbR2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("R2.");
			var whereM = new WhereSqlClauseBuilder();
			var whereR2 = new WhereSqlClauseBuilder();
			var mConditions = new ConnectiveSqlClauseCollection(wbM, whereM);
			var r2Conditions = new ConnectiveSqlClauseCollection(wbR2, whereR2);

			whereM.AppendItem("M.MemberID", roleId);
			whereR2.AppendItem("R2.ParentID", roleId);
			string sqlWhere = qc.WhereClause;
			if (string.IsNullOrEmpty(sqlWhere))
				sqlWhere = wbM1.ToSqlString(TSqlBuilder.Instance);
			else
				sqlWhere = "(" + sqlWhere + ") AND " + wbM1.ToSqlString(TSqlBuilder.Instance);
			sb.AppendFormat((TimePointContext.Current.UseCurrentTime ? @"(
SELECT P1.*,M1.InnerSort,M1.ContainerID as AppID
FROM SC.SchemaMembersSnapshot_Current M1
INNER JOIN SC.SchemaPermissionSnapshot_Current P1 ON M1.MemberID = P1.ID
WHERE
P1.Status = 1 AND
M1.Status = 1
AND {0}
AND {1}
AND M1.ContainerSchemaType = 'Applications' AND M1.MemberSchemaType = 'Permissions'
AND M1.ContainerID = 
(SELECT M.ContainerID FROM
SC.SchemaMembersSnapshot_Current M
WHERE
{2}
AND M.ContainerSchemaType ='Applications'
AND M.MemberSchemaType = 'Roles'
AND M.Status = 1
)

) Q1 LEFT JOIN (
--查现有的权限
SELECT *
FROM SC.SchemaRelationObjectsSnapshot_Current R2 
WHERE R2.ParentSchemaType = 'Roles'
AND R2.ChildSchemaType = 'Permissions'
AND R2.Status = 1
AND {3}
) Q2 ON Q1.ID = Q2.ObjectID"
				: @"(
SELECT P1.*,M1.InnerSort,M1.ContainerID as AppID
FROM SC.SchemaMembersSnapshot M1
INNER JOIN SC.SchemaPermissionSnapshot P1 ON M1.MemberID = P1.ID
WHERE
P1.Status = 1 AND
M1.Status = 1
AND {0}
AND {1}
AND M1.ContainerSchemaType = 'Applications' AND M1.MemberSchemaType = 'Permissions'
AND M1.ContainerID = 
(SELECT M.ContainerID FROM
SC.SchemaMembersSnapshot M
WHERE
{2}
AND M.ContainerSchemaType ='Applications'
AND M.MemberSchemaType = 'Roles'
AND M.Status = 1
)

) Q1 LEFT JOIN (
--查现有的权限
SELECT *
FROM SC.SchemaRelationObjectsSnapshot R2 
WHERE R2.ParentSchemaType = 'Roles'
AND R2.ChildSchemaType = 'Permissions'
AND R2.Status = 1
AND {3}
) Q2 ON Q1.ID = Q2.ObjectID"), sqlWhere, wbP1.ToSqlString(TSqlBuilder.Instance), mConditions.ToSqlString(TSqlBuilder.Instance), r2Conditions.ToSqlString(TSqlBuilder.Instance));

			qc.FromClause = sb.ToString();
			qc.WhereClause = "";
			if (string.IsNullOrEmpty(qc.OrderByClause))
				qc.OrderByClause = "Q1.InnerSort";
			qc.SelectFields = "Q1.*,Q2.ObjectID AS ApplyID";
			base.OnBuildQueryCondition(qc);
		}
	}
}
