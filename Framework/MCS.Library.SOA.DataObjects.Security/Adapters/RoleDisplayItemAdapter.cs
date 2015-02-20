using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using System.Data;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	[Serializable]
	public sealed class RoleDisplayItemAdapter
	{
		public static readonly RoleDisplayItemAdapter Instance = new RoleDisplayItemAdapter();

		private RoleDisplayItemAdapter()
		{
		}

		public IList<RoleDisplayItem> LoadByRoleIds(string[] ids)
		{
			IList<RoleDisplayItem> result = null;
			if (ids.Length > 0 && ids.Length > 0)
			{
				string sql = @"SELECT O.*, A.ID AS AppID, A.Name As AppName, A.DisplayName AS AppDisplayName FROM  
 SC.SchemaObjectSnapshot O INNER JOIN SC.SchemaMembersSnapshot R ON R.MemberID = O.ID
 INNER JOIN SC.SchemaObjectSnapshot_Current A ON R.ContainerID = A.ID
 WHERE ";

				InSqlClauseBuilder inBuilder1 = new InSqlClauseBuilder("O.SchemaType");
				inBuilder1.AppendItem(SchemaInfo.FilterByCategory("Roles").ToSchemaNames());

				InSqlClauseBuilder inBuilder2 = new InSqlClauseBuilder("O.ID");
				inBuilder2.AppendItem(ids);

				var timeLimit1 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("O.");
				var timeLimit2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("R.");

				WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
				builder.AppendItem("O.Status", (int)SchemaObjectStatus.Normal);
				builder.AppendItem("R.Status", (int)SchemaObjectStatus.Normal);

				sql += new ConnectiveSqlClauseCollection(inBuilder1, inBuilder2, timeLimit1, timeLimit2, builder).ToSqlString(TSqlBuilder.Instance);

				DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

				result = new List<RoleDisplayItem>(table.Rows.Count);
				foreach (DataRow row in table.Rows)
				{
					var obj = new RoleDisplayItem();
					ORMapping.DataRowToObject(row, obj);
					result.Add(obj);
				}
			}
			else
			{
				result = new List<RoleDisplayItem>();
			}

			return result;
		}

		/// <summary>
		/// 获取连接的名称
		/// </summary>
		/// <returns>表示连接名称的字符串</returns>
		private string GetConnectionName()
		{
			return "PermissionsCenter"; // SCConnectionDefine.DBConnectionName;
		}
	}
}
