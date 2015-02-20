using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Security.DataSources
{
	/// <summary>
	/// 提供历史查询功能（非分页方式）
	/// </summary>
	public class ObjectHistoryDataSource
	{
		protected virtual string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}

		/// <summary>
		/// 获取与制定对象关联的
		/// </summary>
		/// <param name="id"></param>
		/// <param name="schemaType"></param>
		/// <returns></returns>
		public IEnumerable<HistoryEntry> GetAllHistoryEntry(string id, ref string schemaType)
		{
			HistoryEntryCollection entries = new HistoryEntryCollection();
			schemaType = null;
			using (var context = DbHelper.GetDBContext(this.GetConnectionName()))
			{
				Database db = DatabaseFactory.Create(context);

				WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
				builder.AppendItem("ID", id);
				var sql = "SELECT TOP 1 SchemaType FROM [SC].[SchemaObjectSnapshot] WHERE " + builder.ToSqlString(TSqlBuilder.Instance);
				sql += @"
SELECT [VersionStartTime]
      ,[VersionEndTime]
      ,[CreatorID]
      ,[CreatorName]
      ,[Status]
FROM [SC].[SchemaObjectSnapshot]
WHERE
" + builder.ToSqlString(TSqlBuilder.Instance) + " ORDER BY VersionStartTime DESC";
				var cmd = db.GetSqlStringCommand(sql);
				using (var dr = db.ExecuteReader(cmd))
				{
					if (dr.Read())
					{
						schemaType = dr.GetString(0);
						if (dr.NextResult())
						{
							ORMapping.DataReaderToCollection(entries, dr);
						}
						else
						{
							cmd.Cancel();
						}
					}
				}

				return entries;
			}
		}
	}
}
