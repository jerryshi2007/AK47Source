using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;

namespace MCS.Web.Apps.WeChat.Adapters
{
	public abstract class WeChatObjectAdapterBase<T, TCollection> : UpdatableAndLoadableAdapterBase<T, TCollection> where TCollection : EditableDataObjectCollectionBase<T>, new()
	{
		protected override string GetUpdateSql(T data, ORMappingItemCollection mappings, Dictionary<string, object> context)
		{
			UpdateSqlClauseBuilder uBuilder = ORMapping.GetUpdateSqlClauseBuilder(data, mappings, "UpdateTime");

			uBuilder.AppendItem("UpdateTime", "GETDATE()", "=", true);

			WhereSqlClauseBuilder wBuilder = ORMapping.GetWhereSqlClauseBuilderByPrimaryKey(data, mappings);

			string sql = string.Format("UPDATE {0} SET {1} WHERE {2}",
				mappings.TableName,
				uBuilder.ToSqlString(TSqlBuilder.Instance),
				wBuilder.ToSqlString(TSqlBuilder.Instance));

			return sql;
		}

		protected override string GetConnectionName()
		{
			return ConnectionDefine.WeChatInfoDBConnectionName;
		}
	}
}
