using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Schemas.Actions;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Schemas.Adapters
{
	/// <summary>
	/// 带版本信息的对象的快照操作基类
	/// </summary>
	public abstract class VersionedSchemaObjectSnapshotAdapterBase<T> where T : VersionedSchemaObjectBase
	{

		/// <summary>
		/// 获取连接的名称
		/// </summary>
		/// <returns>表示连接名称的字符串</returns>
		protected abstract string GetConnectionName();

		/// <summary>
		/// 更新对象的快照
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="tableName">表名</param>
		/// <param name="snapshotMode"></param>
		public void UpdateCurrentSnapshot(T obj, string tableName, SnapshotModeDefinition snapshotMode)
		{
			obj.NullCheck("obj");

			string sql = new VersionSnapshotUpdateSqlBuilder(snapshotMode, tableName).ToUpdateSql(obj, ORMapping.GetMappingInfo(obj.GetType()));

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				DateTime dt = (DateTime)DbHelper.RunSqlReturnScalar(sql, this.GetConnectionName());

				SCActionContext.Current.TimePoint.IsMinValue(() => SCActionContext.Current.TimePoint = dt);

				scope.Complete();
			}
		}

		/// <summary>
		/// 更新对象的状态
		/// </summary>
		/// <param name="obj"></param>
		public void UpdateCurrentSnapshotStatus(T obj)
		{
			obj.NullCheck("obj");

			string sql = VersionSnapshotUpdateStatusSqlBuilder.Instance.ToUpdateSql(obj, ORMapping.GetMappingInfo(obj.GetType()));

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				DateTime dt = (DateTime)DbHelper.RunSqlReturnScalar(sql, this.GetConnectionName());

				SCActionContext.Current.TimePoint.IsMinValue(() => SCActionContext.Current.TimePoint = dt);

				scope.Complete();
			}
		}
	}
}
