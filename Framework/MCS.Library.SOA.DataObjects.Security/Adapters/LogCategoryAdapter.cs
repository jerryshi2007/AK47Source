using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	public sealed class LogCategoryAdapter
	{
		public static readonly LogCategoryAdapter Instance = new LogCategoryAdapter();

		private string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}

		public IEnumerable<LogCategory> LoadCategories()
		{
			LogCategoryCollection list = new LogCategoryCollection();

			var sql = @"SELECT [Category],[OperationType]
FROM [SC].[OperationLog] GROUP BY Category,OperationType";

			using (var context = DbHelper.GetDBContext(this.GetConnectionName()))
			{
				Database db = DatabaseFactory.Create(context);

				var cmd = db.GetSqlStringCommand(sql);
				using (var dr = db.ExecuteReader(cmd))
				{
					ORMapping.DataReaderToCollection(list, dr);
				}
			}

			return list;
		}
	}
}
