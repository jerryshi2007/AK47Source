using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using System.Data;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.DataSources
{
	internal static class DataSourceUtil
	{
		internal static IConnectiveSqlClause SchemaTypeCondition(string fieldName, params string[] categories)
		{
			if (fieldName == null)
				throw new ArgumentNullException("field");

			if (categories != null && categories.Length == 1)
			{
				WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
				where.AppendItem(fieldName, categories[0]);
				return where;
			}
			else
			{
				InSqlClauseBuilder inSql = new InSqlClauseBuilder(fieldName);
				inSql.AppendItem(categories);
				return inSql;
			}
		}

		/// <summary>
		/// 将缺省组织ID添加到查询结果
		/// </summary>
		/// <param name="result"></param>
		/// <param name="userIdField">源表中用户ID的列名</param>
		/// <param name="parentIdField">用于添加到源表中缺省组织的ID的列名</param>
		/// <param name="connectionName">连接名</param>
		public static void FillUserDefaultParent(DataView result, string userIdField, string parentIdField, string connectionName)
		{
			if (result.Table.Columns.Contains(parentIdField) == false)
			{
				result.Table.Columns.Add(parentIdField, typeof(string));
			}

			var rows = result.Table.Rows;
			string[] ids = new string[rows.Count];
			for (int i = 0; i < rows.Count; i++)
			{
				ids[i] = (string)rows[i][userIdField];
			}

			if (ids.Length > 0)
			{
				InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("K.ObjectID");
				inBuilder.AppendItem(ids);

				var defaultParentTimeCondition = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("K.");
				var defaultParentWhere = new WhereSqlClauseBuilder();
				defaultParentWhere.AppendItem("K.Status", (int)SchemaObjectStatus.Normal);
				defaultParentWhere.AppendItem("K.IsDefault", 1);

				var defaultParentAllConditions = new ConnectiveSqlClauseCollection(inBuilder, defaultParentWhere, defaultParentTimeCondition);

				string parentIDSql = TimePointContext.Current.UseCurrentTime ? "SELECT K.ObjectID, K.ParentID FROM SC.SchemaRelationObjectsSnapshot_Current K WHERE " + defaultParentAllConditions.ToSqlString(TSqlBuilder.Instance) : "SELECT K.ObjectID, K.ParentID FROM SC.SchemaRelationObjectsSnapshot K WHERE " + defaultParentAllConditions.ToSqlString(TSqlBuilder.Instance);

				DataSet ds = DbHelper.RunSqlReturnDS(parentIDSql, connectionName);

				var rows2 = ds.Tables[0].Rows;
				rows.ForEach<DataRow>(r =>
				{
					rows2.ForEach<DataRow>(r2 =>
					{
						if (r2["ObjectID"].Equals(r[userIdField]))
						{
							r[parentIdField] = r2["ParentID"];
						}
					});
				});
			}
		}
	}
}
