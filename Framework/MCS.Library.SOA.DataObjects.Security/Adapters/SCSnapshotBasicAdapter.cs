using System;
using System.Data;
using System.Text;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Schemas.Actions;
using MCS.Library.SOA.DataObjects.Schemas.Adapters;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	/// <summary>
	/// 快照基本数据操作的Adapter
	/// </summary>
	public class SCSnapshotBasicAdapter : VersionedSchemaObjectSnapshotAdapterBase<SchemaObjectBase>
	{
		/// <summary>
		/// <see cref="SCSnapshotAdapter"/>的实例，此字段为只读
		/// </summary>
		public static readonly SCSnapshotBasicAdapter Instance = new SCSnapshotBasicAdapter();

		private SCSnapshotBasicAdapter()
		{
		}

		#region 基本Adapter操作
		/// <summary>
		/// 根据模式类型，查询ID类型和ID检索对象
		/// </summary>
		/// <param name="schemaType">表示模式类型的字符串</param>
		/// <param name="idType">表示ID类型的<see cref="SnapshotQueryIDType"/>值之一</param>
		/// <param name="id">对象的ID</param>
		/// <returns></returns>
		public SchemaObjectBase LoadByID(string schemaType, SnapshotQueryIDType idType, string id)
		{
			return this.LoadByID(schemaType, idType, id, DateTime.MinValue);
		}

		/// <summary>
		/// 根据模式类型，查询ID类型和ID和时间点检索对象
		/// </summary>
		/// <param name="schemaType">表示模式类型的字符串</param>
		/// <param name="idType">表示ID类型的<see cref="SnapshotQueryIDType"/>值之一</param>
		/// <param name="id">对象的ID</param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public SchemaObjectBase LoadByID(string schemaType, SnapshotQueryIDType idType, string id, DateTime timePoint)
		{
			schemaType.CheckStringIsNullOrEmpty("schemaType");
			id.CheckStringIsNullOrEmpty("id");

			SchemaDefine schema = SchemaDefine.GetSchema(schemaType);

			var timeConditon = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "SN.");

			WhereSqlClauseBuilder whereBuilder = new WhereSqlClauseBuilder();

			EnumItemDescriptionAttribute attr = EnumItemDescriptionAttribute.GetAttribute(idType);

			whereBuilder.AppendItem("SN." + attr.ShortName, id);

			string sql = string.Format("SELECT SO.* FROM SC.SchemaObject SO INNER JOIN {0} SN ON SO.ID = SN.ID WHERE {1}",
				schema.SnapshotTable, new ConnectiveSqlClauseCollection(whereBuilder, timeConditon).ToSqlString(TSqlBuilder.Instance));

			DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

			SchemaObjectBase result = null;

			if (table.Rows.Count > 0)
			{
				result = SchemaExtensions.CreateObject(schemaType);

				result.FromString((string)table.Rows[0]["Data"]);
				ORMapping.DataRowToObject(table.Rows[0], result);
			}

			return result;
		}

		///// <summary>
		///// 更新对象的状态
		///// </summary>
		///// <param name="obj"></param>
		///// <param name="tableName">表名</param>
		///// <param name="snapshotMode"></param>
		//private void Update(SchemaObjectBase obj, string tableName, SnapshotModeDefinition snapshotMode)
		//{
		//    obj.NullCheck("obj");

		//    string sql = new VersionSnapshotUpdateSqlBuilder(snapshotMode, tableName).ToUpdateSql(obj, ORMapping.GetMappingInfo(obj.GetType()));

		//    using (TransactionScope scope = TransactionScopeFactory.Create())
		//    {
		//        DateTime dt = (DateTime)DbHelper.RunSqlReturnScalar(sql, this.GetConnectionName());

		//        SCActionContext.Current.TimePoint.IsMinValue(() => SCActionContext.Current.TimePoint = dt);

		//        scope.Complete();
		//    }
		//}

		//public void UpdateCurrentSnapshot(SchemaObjectBase obj, string tableName, SnapshotModeDefinition snapshotMode)
		//{
		//    obj.NullCheck("obj");

		//    //ORMappingItemCollection mappings = ORMapping.GetMappingInfo(obj.GetType());

		//    using (TransactionScope scope = TransactionScopeFactory.Create())
		//    {
		//        this.Update(obj, tableName, snapshotMode);
		//        //this.UpdateCurrentSnapshot(tableName, tableName + "_Current", ORMapping.GetWhereSqlClauseBuilderByPrimaryKey(obj, mappings));

		//        scope.Complete();
		//    }
		//}

		///// <summary>
		///// 更新对象的状态
		///// </summary>
		///// <param name="obj"></param>
		//public void UpdateStatus(SchemaObjectBase obj)
		//{
		//    obj.NullCheck("obj");

		//    string sql = VersionSnapshotUpdateStatusSqlBuilder.Instance.ToUpdateSql(obj, ORMapping.GetMappingInfo(obj.GetType()));

		//    using (TransactionScope scope = TransactionScopeFactory.Create())
		//    {
		//        DateTime dt = (DateTime)DbHelper.RunSqlReturnScalar(sql, this.GetConnectionName());

		//        SCActionContext.Current.TimePoint.IsMinValue(() => SCActionContext.Current.TimePoint = dt);

		//        scope.Complete();
		//    }
		//}

		///// <summary>
		///// 将快照表的当前状态表进行更新
		///// </summary>
		///// <param name="srcTableName"></param>
		///// <param name="targetTableName"></param>
		///// <param name="builder"></param>
		//public void UpdateCurrentSnapshotStatus(SchemaObjectBase obj)
		//{
		//    obj.NullCheck("obj");

		//    ORMappingItemCollection mappings = ORMapping.GetMappingInfo(obj.GetType());

		//    using (TransactionScope scope = TransactionScopeFactory.Create())
		//    {
		//        this.UpdateStatus(obj);

		//        //this.UpdateCurrentSnapshot(obj.Schema.SnapshotTable, obj.Schema.SnapshotTable + "_Current", ORMapping.GetWhereSqlClauseBuilderByPrimaryKey(obj, mappings));

		//        scope.Complete();
		//    }
		//}

		/// <summary>
		/// 更新当前快照表
		/// </summary>
		/// <param name="srcTableName"></param>
		/// <param name="targetTableName"></param>
		/// <param name="builder"></param>
		[Obsolete("由于采用索引视图，因此Current表作废")]
		public void UpdateCurrentSnapshot(string srcTableName, string targetTableName, IConnectiveSqlClause builder)
		{
			string sql = this.GetInternalUpdateCurrentSnapshotSql(srcTableName, targetTableName, builder);

			if (sql.IsNotEmpty())
				DbHelper.RunSqlWithTransaction(sql, this.GetConnectionName());
		}

		/// <summary>
		/// 生成所有的快照
		/// </summary>
		public void GenerateAllSchemaSnapshot()
		{
			using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
			{
				Database db = DatabaseFactory.Create(context);

				//db.BeforeExecution += new DbEventHandler((sender, e) => ((DbCommand)e.Executor).CommandTimeout = 1800);

				//db.ExecuteNonQuery(CommandType.Text, "EXECUTE SC.GenerateAllSchemaSnapshot");

				var cmd = db.CreateStoredProcedureCommand("SC.GenerateAllSchemaSnapshot");
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = 60 * 60;
				db.ExecuteNonQuery(cmd);
			}
		}

		/// <summary>
		/// 得到更新当前快照的SQL
		/// </summary>
		/// <param name="srcTableName"></param>
		/// <param name="targetTableName"></param>
		/// <param name="builder"></param>
		/// <returns></returns>
		private string GetInternalUpdateCurrentSnapshotSql(string srcTableName, string targetTableName, IConnectiveSqlClause builder)
		{
			builder.NullCheck("builder");

			StringBuilder strB = new StringBuilder();

			if (builder.IsEmpty == false)
			{
				IConnectiveSqlClause timePointBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(DateTime.MinValue);

				ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(builder, timePointBuilder);

				string sql = string.Format("SELECT * FROM {0} WHERE Status = {1} AND {2}",
					srcTableName,
					(int)SchemaObjectStatus.Normal,
					connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

				DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

				if (table.Rows.Count > 0)
				{
					UpdateSqlClauseBuilder updateBuilder = new UpdateSqlClauseBuilder();
					DataRowToBuilder(table.Rows[0], updateBuilder, "RowUniqueID");

					InsertSqlClauseBuilder insertBuilder = new InsertSqlClauseBuilder();
					DataRowToBuilder(table.Rows[0], insertBuilder);

					strB.AppendFormat("UPDATE {0} SET {1} WHERE {2}",
						targetTableName,
						updateBuilder.ToSqlString(TSqlBuilder.Instance),
						builder.ToSqlString(TSqlBuilder.Instance));

					strB.Append(TSqlBuilder.Instance.DBStatementSeperator);
					strB.Append("IF @@ROWCOUNT = 0");
					strB.Append(TSqlBuilder.Instance.DBStatementSeperator);
					strB.AppendFormat("INSERT INTO {0} {1}",
						targetTableName,
						insertBuilder.ToSqlString(TSqlBuilder.Instance));
				}
				else
				{
					strB.AppendFormat("DELETE {0} WHERE {1}", targetTableName, builder.ToSqlString(TSqlBuilder.Instance));
				}
			}

			return strB.ToString();
		}

		/// <summary>
		/// 得到更新当前快照的SQL
		/// </summary>
		/// <param name="srcTableName"></param>
		/// <param name="targetTableName"></param>
		/// <param name="builder"></param>
		/// <returns></returns>
		[Obsolete("由于采用索引视图，因此Current表作废")]
		public string GetUpdateCurrentSnapshotSql(string srcTableName, string targetTableName, IConnectiveSqlClause builder)
		{
			builder.NullCheck("builder");

			StringBuilder strB = new StringBuilder();

			if (builder.IsEmpty == false)
			{
				strB.AppendFormat("DELETE {0} WHERE {1}", targetTableName, builder.ToSqlString(TSqlBuilder.Instance));
				strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

				IConnectiveSqlClause timePointBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(DateTime.MinValue);

				ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(builder, timePointBuilder);

				strB.AppendFormat("INSERT INTO {0} SELECT * FROM {1} WHERE Status = {2} AND {3}",
					targetTableName, srcTableName, (int)SchemaObjectStatus.Normal, connectiveBuilder.ToSqlString(TSqlBuilder.Instance));
			}

			return strB.ToString();
		}

		private static void DataRowToBuilder(DataRow row, SqlClauseBuilderIUW builder, params string[] ignorColumns)
		{
			foreach (DataColumn column in row.Table.Columns)
			{
				if (Array.Exists(ignorColumns, (c => c == column.ColumnName)) == false)
					builder.AppendItem(column.ColumnName, row[column.ColumnName]);
			}
		}
		#region Load

		/// <summary>
		/// 根据模式类型和ID载入视图
		/// </summary>
		/// <param name="schemaType">模式类型</param>
		/// <param name="id">ID</param>
		/// <returns></returns>
		public DataRowView Load(string schemaType, string id)
		{
			return this.Load(schemaType, id, DateTime.MinValue);
		}

		/// <summary>
		/// 根据模式类型，ID和时间点载入视图
		/// </summary>
		/// <param name="schemaType">表示模式类型的字符串</param>
		/// <param name="id">ID</param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public DataRowView Load(string schemaType, string id, DateTime timePoint)
		{
			id.NullCheck("id");

			InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("ID");

			inBuilder.AppendItem(id);

			DataView objs = this.Load(schemaType, inBuilder, timePoint);

			DataRowView result = null;

			if (objs.Count > 0)
				result = objs[0];

			return result;
		}

		/// <summary>
		/// 根据模式类型，条件载入视图
		/// </summary>
		/// <param name="schemaType">表示模式类型的字符串</param>
		/// <param name="inBuilder">条件</param>
		/// <returns></returns>
		public DataView Load(string schemaType, IConnectiveSqlClause inBuilder)
		{
			return this.Load(schemaType, inBuilder, DateTime.MinValue);
		}

		/// <summary>
		/// 根据模式类型，条件和时间点载入视图
		/// </summary>
		/// <param name="schemaType">模式类型</param>
		/// <param name="inBuilder">条件</param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public DataView Load(string schemaType, IConnectiveSqlClause inBuilder, DateTime timePoint)
		{
			ObjectSchemaConfigurationElement schemaElem = ObjectSchemaSettings.GetConfig().Schemas[schemaType];

			(schemaElem != null).FalseThrow("不能找到SchemaType为{0}的定义", schemaType);

			var whereBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint);

			ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(inBuilder, whereBuilder);

			DataView result = null;

			VersionedObjectAdapterHelper.Instance.FillData(schemaElem.SnapshotTable, connectiveBuilder, this.GetConnectionName(),
				view =>
				{
					result = view;
				});

			return result;
		}

		/// <summary>
		/// 根据条件，时间点载入视图
		/// </summary>
		/// <param name="inBuilder">包含条件的<see cref="IConnectiveSqlClause"/></param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public DataView Load(IConnectiveSqlClause inBuilder, DateTime timePoint)
		{
			var whereBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint);

			ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(inBuilder, whereBuilder);

			DataView result = null;

			VersionedObjectAdapterHelper.Instance.FillData("SC.SchemaObjectSnapshot", connectiveBuilder, this.GetConnectionName(),
				view =>
				{
					result = view;
				});

			return result;
		}
		#endregion Load
		#endregion 基本Adapter操作

		/// <summary>
		/// 获取连接的名称
		/// </summary>
		/// <returns>表示连接名称的字符串</returns>
		protected override string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}
	}
}
