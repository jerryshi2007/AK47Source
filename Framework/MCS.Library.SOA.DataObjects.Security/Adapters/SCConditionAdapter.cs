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
using MCS.Library.SOA.DataObjects.Security.Conditions;
using MCS.Library.SOA.DataObjects.Schemas.Actions;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	/// <summary>
	/// Condition适配器。用于读写SC.Conditions表。将表达式和其Owner的关系保存下来
	/// </summary>
	public class SCConditionAdapter
	{
		public static readonly SCConditionAdapter Instance = new SCConditionAdapter();

		private SCConditionAdapter()
		{
		}

		/// <summary>
		/// 根据ownerID和type加载ConditionOwner对象
		/// </summary>
		/// <param name="ownerID"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public SCConditionOwner Load(string ownerID, string type = null)
		{
			return this.Load(ownerID, type, DateTime.MinValue);
		}

		/// <summary>
		/// 根据ownerID和type加载ConditionOwner对象
		/// </summary>
		/// <param name="ownerID"></param>
		/// <param name="type"></param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public SCConditionOwner Load(string ownerID, string type, DateTime timePoint)
		{
			if (type.IsNullOrEmpty())
				type = "Default";

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("OwnerID", ownerID);
			builder.AppendItem("Type", type);

			SCConditionCollection conditions = this.Load(builder, timePoint);

			SCConditionOwner owner = new SCConditionOwner() { OwnerID = ownerID, Type = type };

			owner.Conditions.CopyFrom(conditions);

			return owner;
		}

		/// <summary>
		/// 根据condition所提供的SQL查询条件查询
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public SCConditionCollection Load(IConnectiveSqlClause condition, DateTime timePoint)
		{
			ConnectiveSqlClauseCollection timePointBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint);
			ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(condition, timePointBuilder);

			string sql = string.Format("SELECT * FROM {0} WHERE {1} ORDER BY SortID",
				this.GetMappingInfo().TableName,
				connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

			SCConditionCollection result = new SCConditionCollection();

			using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
			{
				using (IDataReader reader = DbHelper.RunSqlReturnDR(sql, this.GetConnectionName()))
				{
					ORMapping.DataReaderToCollection(result, reader);

					return result;
				}
			}
		}

		/// <summary>
		/// 修改条件
		/// </summary>
		/// <param name="owner"></param>
		public void UpdateConditions(SCConditionOwner owner)
		{
			this.UpdateConditions(owner.OwnerID, owner.Type, owner.Conditions);
		}

		/// <summary>
		/// 修改条件
		/// </summary>
		/// <param name="ownerID"></param>
		/// <param name="type"></param>
		/// <param name="conditions"></param>
		public void UpdateConditions(string ownerID, string type, SCConditionCollection conditions)
		{
			ownerID.CheckStringIsNullOrEmpty("ownerID");

			ORMappingItemCollection mappings = this.GetMappingInfo();

			string sql = this.GetUpdateSql(ownerID, type, conditions);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				DateTime dt = (DateTime)DbHelper.RunSqlReturnScalar(sql, this.GetConnectionName());

				SCActionContext.Current.TimePoint.IsMinValue(() => SCActionContext.Current.TimePoint = dt);
				//WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

				//builder.AppendItem("OwnerID", ownerID);
				//builder.AppendItem("Type", type);

				//SCSnapshotBasicAdapter.Instance.UpdateCurrentSnapshot(mappings.TableName, mappings.TableName + "_Current", builder);

				scope.Complete();
			}
		}

		/// <summary>
		/// 根据Owner删除条件
		/// </summary>
		/// <param name="ownerID"></param>
		/// <param name="type"></param>
		public void DeleteByOwner(SCConditionOwner owner)
		{
			this.UpdateConditions(owner.OwnerID, owner.Type, new SCConditionCollection());
		}

		public void DeleteByOwner(string ownerID, string type)
		{
			this.UpdateConditions(ownerID, type, new SCConditionCollection());
		}

		private string GetUpdateSql(string ownerID, string type, SCConditionCollection conditions)
		{
			if (type.IsNullOrEmpty())
				type = "Default";

			conditions.ForEach(c => { c.OwnerID = ownerID; c.Type = type; });

			return VersionStrategyUpdateSqlHelper.ConstructUpdateSql(null, (strB, context) =>
			{
				ConnectiveSqlClauseCollection connectiveBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder();

				WhereSqlClauseBuilder keyBuilder = new WhereSqlClauseBuilder();

				keyBuilder.AppendItem("OwnerID", ownerID);
				keyBuilder.AppendItem("type", type);

				connectiveBuilder.Add(keyBuilder);

				strB.AppendFormat("UPDATE {0} SET VersionEndTime = {1} WHERE {2}",
					this.GetMappingInfo().TableName, "@currentTime", connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

				for (int i = 0; i < conditions.Count; i++)
				{
					SCCondition condition = conditions[i];

					condition.SortID = i;
					strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

					condition.VersionEndTime = ConnectionDefine.MaxVersionEndTime;
					condition.Type = type;
					InsertSqlClauseBuilder insertBuilder = ORMapping.GetInsertSqlClauseBuilder(condition, this.GetMappingInfo(), "VersionStartTime");

					insertBuilder.AppendItem("VersionStartTime", "@currentTime", "=", true);

					strB.AppendFormat("INSERT INTO {0}{1}", this.GetMappingInfo().TableName, insertBuilder.ToSqlString(TSqlBuilder.Instance));
				}
			});
		}

		/// <summary>
		/// 在派生类中重写时， 获取映射信息的集合
		/// </summary>
		/// <returns><see cref="ORMappingItemCollection"/>，表示映射信息</returns>
		protected virtual ORMappingItemCollection GetMappingInfo()
		{
			return ORMapping.GetMappingInfo(typeof(SCCondition));
		}

		/// <summary>
		/// 获取连接的名称
		/// </summary>
		/// <returns>表示连接名称的<see cref="string"/>。</returns>
		protected virtual string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}
	}
}
