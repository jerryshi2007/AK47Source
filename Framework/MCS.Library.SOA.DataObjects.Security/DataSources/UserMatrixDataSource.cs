using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Security.DataSources
{
	/// <summary>
	/// 查询用户的角色矩阵
	/// </summary>
	public class UserMatrixDataSource
	{
		private class InnerDataSource : DataViewDataSourceQueryAdapterBase
		{
			protected override string GetConnectionName()
			{
				return SCConnectionDefine.DBConnectionName;
			}

			protected override void OnBuildQueryCondition(QueryCondition qc)
			{
				qc.FromClause = TimePointContext.Current.UseCurrentTime ? @"
SC.SchemaRoleSnapshot_Current R INNER JOIN SC.SchemaMembersSnapshot_Current S
ON S.MemberID = R.ID INNER JOIN  SC.SchemaApplicationSnapshot_Current A 
ON A.ID = S.ContainerID" : @"SC.SchemaRoleSnapshot R INNER JOIN SC.SchemaMembersSnapshot S
ON S.MemberID = R.ID
INNER JOIN  SC.SchemaApplicationSnapshot A ON A.ID = S.ContainerID";

				qc.SelectFields = "R.*, A.ID AS  AppID,A.DisplayName AS AppName,A.Name As AppName2";

				base.OnBuildQueryCondition(qc);

				if (string.IsNullOrWhiteSpace(qc.OrderByClause))
				{
					qc.OrderByClause = "A.DisplayName";
				}

				if (string.IsNullOrWhiteSpace(qc.WhereClause))
				{
					qc.WhereClause = "1=2";
				}
				else
				{
					qc.WhereClause = "(" + qc.WhereClause + ")";
				}
			}
		}

		private int lastQueryCount;
		private InnerDataSource innerSource = new InnerDataSource();

		public UserMatrixDataSource()
		{
		}

		public int GetQueryCount(string userId, string where, ref int totalCount)
		{
			return lastQueryCount;
		}

		public DataView Query(string userID, string where, int startRowIndex, int maximumRows, string orderBy, ref int totalCount)
		{
			List<string> operatorIDs = new List<string>();

			SchemaObjectCollection userRoles =
				SCSnapshotAdapter.Instance.QueryUserBelongToRoles(new string[] { "Roles" }, string.Empty, new string[] { userID }, false, DBTimePointActionContext.Current.TimePoint);

			userRoles.ForEach(r => operatorIDs.Add(r.ID));

			operatorIDs.Add(userID);

			List<string> roleMatrixIDs = SOARolePropertiesAdapter.Instance.OperatorBelongToRoleIDsDirectly(operatorIDs.ToArray());

			InSqlClauseBuilder roleInSql = new InSqlClauseBuilder("R.ID");
			roleInSql.AppendItem(roleMatrixIDs.ToArray());

			DataView view = this.innerSource.Query(startRowIndex, maximumRows, roleInSql.ToSqlString(TSqlBuilder.Instance), orderBy, ref totalCount);

			this.lastQueryCount = totalCount;

			return view;
		}
	}
}
