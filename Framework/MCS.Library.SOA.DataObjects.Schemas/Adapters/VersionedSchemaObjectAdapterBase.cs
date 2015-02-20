using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Schemas.Actions;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Schemas.Adapters
{
	/// <summary>
	/// 带版本信息的Schema对象更新适配器的基类
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class VersionedSchemaObjectAdapterBase<T> where T : VersionedSchemaObjectBase
	{
		/// <summary>
		/// 获取Action的集合
		/// </summary>
		/// <param name="actionName"></param>
		/// <returns></returns>
		protected abstract SchemaObjectUpdateActionCollection GetActions(string actionName);

		/// <summary>
		/// 将模式对象的修改提交到数据库
		/// </summary>
		/// <param name="obj">对其进行更新的<typeparamref name="T"/>对象。</param>
		public void Update(T obj)
		{
			obj.NullCheck("obj");

			this.MergeExistsObjectInfo(obj);

			SchemaObjectUpdateActionCollection actions = GetActions("Update");

			actions.Prepare(obj);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				obj.Properties.Write();

				string sql = VersionSchemaObjectUpdateSqlBuilder.Instance.ToUpdateSql(obj, this.GetMappingInfo());

				DateTime dt = (DateTime)DbHelper.RunSqlReturnScalar(sql, this.GetConnectionName());

				SCActionContext.Current.TimePoint.IsMinValue(() => SCActionContext.Current.TimePoint = dt);

				actions.Persist(obj);

				scope.Complete();
			}
		}

		/// <summary>
		/// 更新模式对象的状态到数据库
		/// </summary>
		/// <param name="obj">对其进行更新的<typeparamref name="T"/>对象。</param>
		/// <param name="status">表示状态的<see cref="SchemaObjectStatus"/>值之一。</param>
		public void UpdateStatus(T obj, SchemaObjectStatus status)
		{
			obj.Status = status;

			string sql = VersionSchemaObjectUpdateStatusSqlBuilder.Instance.ToUpdateSql(obj, this.GetMappingInfo());

			SchemaObjectUpdateActionCollection actions = GetActions("UpdateStatus");

			actions.Prepare(obj);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				DateTime dt = (DateTime)DbHelper.RunSqlReturnScalar(sql, this.GetConnectionName());

				SCActionContext.Current.TimePoint.IsMinValue(() => SCActionContext.Current.TimePoint = dt);

				actions.Persist(obj);

				scope.Complete();
			}
		}

		/// <summary>
		/// 合并现有对象信息
		/// </summary>
		/// <param name="obj">对其进行更新的<typeparamref name="T"/>对象。</param>
		public void MergeExistsObjectInfo(T obj)
		{
			if (SCActionContext.Current.OriginalObject != null &&
				SCActionContext.Current.OriginalObject.SchemaType == obj.SchemaType &&
				SCActionContext.Current.OriginalObject.ID == obj.ID)
			{
				obj.Creator = SCActionContext.Current.OriginalObject.Creator;
				obj.CreateDate = SCActionContext.Current.OriginalObject.CreateDate;
				obj.VersionStartTime = SCActionContext.Current.OriginalObject.VersionStartTime;
			}
			else
			{
				VersionedSimpleObject existedInfo = this.GetExistedObject(obj);

				if (existedInfo != null)
				{
					obj.CreateDate = existedInfo.CreateDate;
					obj.Creator = existedInfo.Creator;
					obj.VersionStartTime = existedInfo.VersionStartTime;
				}
			}
		}

		private VersionedSimpleObject GetExistedObject(T obj)
		{
			WhereSqlClauseBuilder keyBuilder = ORMapping.GetWhereSqlClauseBuilderByPrimaryKey(obj, this.GetMappingInfo());

			string sql = string.Format("SELECT TOP 1 {0} FROM {1} WHERE {2} ORDER BY VersionStartTime DESC",
				string.Join(",", ORMapping.GetSelectFieldsName(this.GetMappingInfo(), "Data")),
				this.GetMappingInfo().TableName,
				keyBuilder.ToSqlString(TSqlBuilder.Instance));

			DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

			VersionedSimpleObject result = null;

			if (table.Rows.Count > 0)
			{
				result = this.CreateSimpleObject();
				ORMapping.DataRowToObject(table.Rows[0], result);
			}

			return result;
		}

		/// <summary>
		/// 创建一个简单对象
		/// </summary>
		/// <returns>一个<see cref="SCSimpleObject"/>对象。</returns>
		protected virtual VersionedSimpleObject CreateSimpleObject()
		{
			return new VersionedSimpleObject();
		}

		/// <summary>
		/// 在派生类中重写时， 获取映射信息的集合
		/// </summary>
		/// <returns><see cref="ORMappingItemCollection"/>，表示映射信息</returns>
		protected virtual ORMappingItemCollection GetMappingInfo()
		{
			return ORMapping.GetMappingInfo(typeof(T));
		}

		/// <summary>
		/// 获取连接的名称
		/// </summary>
		/// <returns>表示连接名称的<see cref="string"/>。</returns>
		protected abstract string GetConnectionName();
	}
}
