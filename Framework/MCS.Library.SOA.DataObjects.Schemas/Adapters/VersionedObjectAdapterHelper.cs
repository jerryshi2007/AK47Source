using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;

namespace MCS.Library.SOA.DataObjects.Schemas.Adapters
{
	public class VersionedObjectAdapterHelper
	{
		/// <summary>
		/// <see cref="VersionedObjectAdapterHelper"/>的实例，此字段为只读
		/// </summary>
		public static readonly VersionedObjectAdapterHelper Instance = new VersionedObjectAdapterHelper();

		private VersionedObjectAdapterHelper()
		{
		}

		public void FillData(string tableName, IConnectiveSqlClause connectiveBuilder, string connectionName, Action<DataView> action)
		{
			StringBuilder strB = new StringBuilder();

			strB.AppendFormat("SELECT * FROM {0} WHERE {1}",
				tableName,
				connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

			DataView view = DbHelper.RunSqlReturnDS(strB.ToString(), connectionName).Tables[0].DefaultView;

			if (action != null)
				action(view);
		}

		public void FillData(string tableName, IConnectiveSqlClause connectiveBuilder, string connectionName, Action<IDataReader> action)
		{
			StringBuilder strB = new StringBuilder();

			strB.AppendFormat("SELECT * FROM {0} WHERE {1}",
				tableName,
				connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

			using (IDataReader reader = DbHelper.RunSqlReturnDR(strB.ToString(), connectionName))
			{
				if (action != null)
					action(reader);
			}
		}

		public void FillData(string sql, string connectionName, Action<IDataReader> action)
		{
			using (IDataReader reader = DbHelper.RunSqlReturnDR(sql, connectionName))
			{
				if (action != null)
				{
					action(reader);
				}
			}
		}
	}
}
