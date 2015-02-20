using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Schemas.Actions;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.Actions;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	/// <summary>
	/// 表示用户和容器快照的适配器。无论是角色还是群组，它们最终都是用户的容器，这里存放了用户和容器的直接对应关系。
	/// 一个人在一个容器下只有一条信息
	/// </summary>
	public class UserAndContainerSnapshotAdapter
	{
		/// <summary>
		/// <see cref="UserAndContainerSnapshotAdapter"/>的实例，此字段为只读
		/// </summary>
		public static readonly UserAndContainerSnapshotAdapter Instance = new UserAndContainerSnapshotAdapter();

		private UserAndContainerSnapshotAdapter()
		{
		}

		/// <summary>
		/// 合并用户信息
		/// </summary>
		/// <param name="users"></param>
		public void Merge(string containerID, string containerSchemaType, SchemaObjectCollection users)
		{
			containerID.CheckStringIsNullOrEmpty("containerID");
			containerSchemaType.CheckStringIsNullOrEmpty("containerSchemaType");
			users.NullCheck("users");

			SameContainerUserAndContainerSnapshotCollection existedData = this.LoadByContainerID(containerID);

			SameContainerUserAndContainerSnapshotCollection insertData = GetInsertData(containerID, containerSchemaType, existedData, users);
			SameContainerUserAndContainerSnapshotCollection updateData = GetUpdateData(containerID, containerSchemaType, existedData, users);

			ProcessProgress.Current.MaxStep += insertData.Count;
			ProcessProgress.Current.MaxStep += updateData.Count;

			ExecUpdateSeperately(insertData);
			ExecUpdateSeperately(updateData);
		}

		private void ExecUpdateSeperately(SameContainerUserAndContainerSnapshotCollection data)
		{
			foreach (UserAndContainerSnapshot obj in data)
			{
				string sql = PrepareOneUpdateSql(obj);

				using (TransactionScope scope = TransactionScopeFactory.Create())
				{
					DateTime dtUpdate = (DateTime)DbHelper.RunSqlReturnScalar(sql, this.GetConnectionName());

					SCActionContext.Current.TimePoint.IsMinValue(() => SCActionContext.Current.TimePoint = dtUpdate);

					scope.Complete();
				}

				ProcessProgress.Current.Increment();
				ProcessProgress.Current.Response();
			}
		}

		/// <summary>
		/// 合并用户信息，一个事物内更新所有的
		/// </summary>
		/// <param name="users"></param>
		public void BatchMerge(string containerID, string containerSchemaType, SchemaObjectCollection users)
		{
			containerID.CheckStringIsNullOrEmpty("containerID");
			containerSchemaType.CheckStringIsNullOrEmpty("containerSchemaType");
			users.NullCheck("users");

			SameContainerUserAndContainerSnapshotCollection existedData = this.LoadByContainerID(containerID);

			SameContainerUserAndContainerSnapshotCollection insertData = GetInsertData(containerID, containerSchemaType, existedData, users);
			SameContainerUserAndContainerSnapshotCollection updateData = GetUpdateData(containerID, containerSchemaType, existedData, users);

			string sqlInsert = this.PrepareUpdateSql(insertData);
			string sqlUpdate = this.PrepareUpdateSql(updateData);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				DateTime dtInsert = (DateTime)DbHelper.RunSqlReturnScalar(sqlInsert, this.GetConnectionName());

				SCActionContext.Current.TimePoint.IsMinValue(() => SCActionContext.Current.TimePoint = dtInsert);

				DateTime dtUpdate = (DateTime)DbHelper.RunSqlReturnScalar(sqlUpdate, this.GetConnectionName());

				SCActionContext.Current.TimePoint.IsMinValue(() => SCActionContext.Current.TimePoint = dtUpdate);

				scope.Complete();
			}
		}

		/// <summary>
		/// 插入
		/// </summary>
		/// <param name="mr">成员关系</param>
		public void Insert(SCMemberRelation mr)
		{
			UserAndContainerSnapshot existedObj = this.GetExistedObject(mr);
			string sql = string.Empty;

			UserAndContainerSnapshot obj = new UserAndContainerSnapshot(mr);

			if (existedObj != null)
			{
				//如果已经存在，且状态不是正常的
				obj.VersionStartTime = existedObj.VersionStartTime;

				if (existedObj.Status != SchemaObjectStatus.Normal)
				{
					sql = this.PrepareUpdateSql(new UserAndContainerSnapshot[] { obj });
				}
			}
			else
			{
				//如果不存在
				sql = this.PrepareUpdateSql(new UserAndContainerSnapshot[] { obj });
			}

			if (sql.IsNotEmpty())
			{
				using (TransactionScope scope = TransactionScopeFactory.Create())
				{
					DateTime dt = (DateTime)DbHelper.RunSqlReturnScalar(sql, this.GetConnectionName());

					SCActionContext.Current.TimePoint.IsMinValue(() => SCActionContext.Current.TimePoint = dt);

					scope.Complete();
				}
			}
		}

		/// <summary>
		/// 根据Container来加载UserAndContainerSnapshot的信息
		/// </summary>
		/// <param name="containerID"></param>
		/// <returns></returns>
		public SameContainerUserAndContainerSnapshotCollection LoadByContainerID(string containerID)
		{
			return this.LoadByContainerID(containerID, DateTime.MinValue);
		}

		/// <summary>
		/// 根据Container来加载UserAndContainerSnapshot的信息
		/// </summary>
		/// <param name="containerID"></param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public SameContainerUserAndContainerSnapshotCollection LoadByContainerID(string containerID, DateTime timePoint)
		{
			containerID.CheckStringIsNullOrEmpty("containerID");

			ConnectiveSqlClauseCollection timePointBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint);

			WhereSqlClauseBuilder whereBuilder = new WhereSqlClauseBuilder();

			whereBuilder.AppendItem("ContainerID", containerID);

			ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(whereBuilder, timePointBuilder);

			string sql = string.Format("SELECT * FROM {0} WHERE {1}",
				this.GetLoadingTableName(timePoint), connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

			SameContainerUserAndContainerSnapshotCollection result = new SameContainerUserAndContainerSnapshotCollection();

			using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
			{
				using (IDataReader reader = DbHelper.RunSqlReturnDR(sql, this.GetConnectionName()))
				{
					while (reader.Read())
					{
						UserAndContainerSnapshot item = new UserAndContainerSnapshot();

						ORMapping.DataReaderToObject(reader, item);

						if (result.ContainsKey(item.UserID) == false)
							result.Add(item);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// 根据Container来加载UserAndContainerSnapshot的信息
		/// </summary>
		/// <param name="containerID"></param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public int CountAliveUsersByContainer(string containerID, DateTime timePoint)
		{
			containerID.CheckStringIsNullOrEmpty("containerID");

			ConnectiveSqlClauseCollection timePointBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint);

			WhereSqlClauseBuilder whereBuilder = new WhereSqlClauseBuilder();

			whereBuilder.AppendItem("ContainerID", containerID);
			whereBuilder.AppendItem("Status", (int)SchemaObjectStatus.Normal);

			ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(whereBuilder, timePointBuilder);

			string sql = string.Format("SELECT COUNT(ContainerID) FROM {0} WHERE {1}",
				this.GetLoadingTableName(timePoint), connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

			int result = 0;

			using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
			{
				result = (int)DbHelper.RunSqlReturnScalar(sql, this.GetConnectionName());
			}

			return result;
		}

		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="mr">成员关系</param>
		public void Delete(SCMemberRelation mr)
		{
			UserAndContainerSnapshot existedObj = this.GetExistedObject(mr);
			string sql = string.Empty;

			UserAndContainerSnapshot obj = new UserAndContainerSnapshot(mr);
			obj.Status = SchemaObjectStatus.Deleted;

			if (existedObj != null)
			{
				//如果已经存在，且状态是正常的
				obj.VersionStartTime = existedObj.VersionStartTime;

				if (existedObj.Status == SchemaObjectStatus.Normal)
				{
					sql = this.PrepareUpdateSql(new UserAndContainerSnapshot[] { obj });
				}
			}

			if (sql.IsNotEmpty())
			{
				using (TransactionScope scope = TransactionScopeFactory.Create())
				{
					DateTime dt = (DateTime)DbHelper.RunSqlReturnScalar(sql, this.GetConnectionName());

					SCActionContext.Current.TimePoint.IsMinValue(() => SCActionContext.Current.TimePoint = dt);

					scope.Complete();
				}
			}
		}

		protected virtual ORMappingItemCollection GetMappingInfo()
		{
			return ORMapping.GetMappingInfo(typeof(UserAndContainerSnapshot));
		}

		protected virtual string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}

		public string PrepareOneUpdateSql(UserAndContainerSnapshot obj)
		{
			ORMappingItemCollection mapping = this.GetMappingInfo();
			StringBuilder strB = new StringBuilder();

			string currentTimeSql = TSqlBuilder.Instance.DBCurrentTimeFunction;

			SCActionContext.Current.TimePoint.IsNotMinValue(tp => currentTimeSql = TSqlBuilder.Instance.FormatDateTime(tp));

			strB.Append("DECLARE @currentTime DATETIME");
			strB.Append(TSqlBuilder.Instance.DBStatementSeperator);
			strB.AppendFormat("SET @currentTime = {0}", currentTimeSql);
			strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

			FillOneUpdateSql(strB, obj, mapping);

			strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

			strB.Append("SELECT @currentTime");

			return strB.ToString();
		}

		/// <summary>
		/// 准备更新的Sql语句
		/// </summary>
		/// <param name="obj">快照对象</param>
		/// <returns></returns>
		public string PrepareUpdateSql(IEnumerable<UserAndContainerSnapshot> objs)
		{
			ORMappingItemCollection mapping = this.GetMappingInfo();
			StringBuilder strB = new StringBuilder();

			string currentTimeSql = TSqlBuilder.Instance.DBCurrentTimeFunction;

			SCActionContext.Current.TimePoint.IsNotMinValue(tp => currentTimeSql = TSqlBuilder.Instance.FormatDateTime(tp));

			strB.Append("DECLARE @currentTime DATETIME");
			strB.Append(TSqlBuilder.Instance.DBStatementSeperator);
			strB.AppendFormat("SET @currentTime = {0}", currentTimeSql);
			strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

			foreach (UserAndContainerSnapshot obj in objs)
			{
				FillOneUpdateSql(strB, obj, mapping);

				strB.Append(TSqlBuilder.Instance.DBStatementSeperator);
			}

			strB.Append("SELECT @currentTime");

			return strB.ToString();
		}

		private static void FillOneUpdateSql(StringBuilder strB, UserAndContainerSnapshot obj, ORMappingItemCollection mapping)
		{
			if (obj.VersionStartTime != DateTime.MinValue)
			{
				WhereSqlClauseBuilder primaryKeyBuilder = ORMapping.GetWhereSqlClauseBuilderByPrimaryKey(obj, mapping);

				strB.AppendFormat("UPDATE {0} SET VersionEndTime = @currentTime WHERE {1} AND VersionStartTime = {2}",
					mapping.TableName,
					primaryKeyBuilder.ToSqlString(TSqlBuilder.Instance),
					TSqlBuilder.Instance.FormatDateTime(obj.VersionStartTime));

				strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

				strB.AppendFormat("IF @@ROWCOUNT > 0\n");
				strB.AppendFormat("BEGIN\n");
				strB.AppendFormat("\t{0}\n", PrepareInsertSql(obj, mapping));
				strB.AppendFormat("END\n");
				strB.AppendFormat("ELSE\n");
				strB.AppendFormat("\tRAISERROR ({0}, 16, 1)",
					TSqlBuilder.Instance.CheckUnicodeQuotationMark(string.Format("对象\"{0}\"和\"{1}\"的版本不是最新的，不能更新",
						obj.UserID, obj.ContainerID)));
			}
			else
			{
				strB.AppendFormat(PrepareInsertSql(obj, mapping));
			}
		}

		private static string PrepareInsertSql(UserAndContainerSnapshot obj, ORMappingItemCollection mapping)
		{
			InsertSqlClauseBuilder builder = ORMapping.GetInsertSqlClauseBuilder(obj);

			builder.AppendItem("VersionStartTime", "@currentTime", "=", true);
			builder.AppendItem("VersionEndTime", SCConnectionDefine.MaxVersionEndTime);

			return string.Format("INSERT INTO {0}{1}", mapping.TableName, builder.ToSqlString(TSqlBuilder.Instance));
		}

		private UserAndContainerSnapshot GetExistedObject(SCMemberRelation mr)
		{
			UserAndContainerSnapshot condition = new UserAndContainerSnapshot(mr);

			WhereSqlClauseBuilder keyBuilder = ORMapping.GetWhereSqlClauseBuilderByPrimaryKey(condition, this.GetMappingInfo());

			string sql = string.Format("SELECT TOP 1 {0} FROM {1} WHERE {2} ORDER BY VersionStartTime DESC",
				string.Join(",", ORMapping.GetSelectFieldsName(this.GetMappingInfo())),
				this.GetLoadingTableName(DateTime.MinValue),
				keyBuilder.ToSqlString(TSqlBuilder.Instance));

			DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

			UserAndContainerSnapshot result = null;

			if (table.Rows.Count > 0)
			{
				result = new UserAndContainerSnapshot();
				ORMapping.DataRowToObject(table.Rows[0], result);
			}

			return result;
		}

		private static SameContainerUserAndContainerSnapshotCollection GetInsertData(string containerID, string containerSchemaType, SameContainerUserAndContainerSnapshotCollection existedData, SchemaObjectCollection users)
		{
			SameContainerUserAndContainerSnapshotCollection newInfo = new SameContainerUserAndContainerSnapshotCollection();

			foreach (SchemaObjectBase user in users)
			{
				if (existedData.ContainsKey(user.ID) == false && newInfo.ContainsKey(user.ID) == false)
				{
					UserAndContainerSnapshot uacs = new UserAndContainerSnapshot();

					uacs.ContainerID = containerID;
					uacs.ContainerSchemaType = containerSchemaType;
					uacs.UserID = user.ID;
					uacs.UserSchemaType = "Users";
					uacs.Status = SchemaObjectStatus.Normal;

					newInfo.Add(uacs);
				}
			}

			return newInfo;
		}

		private static SameContainerUserAndContainerSnapshotCollection GetUpdateData(string containerID, string containerSchemaType, SameContainerUserAndContainerSnapshotCollection existedData, SchemaObjectCollection users)
		{
			SameContainerUserAndContainerSnapshotCollection updatedInfo = new SameContainerUserAndContainerSnapshotCollection();

			foreach (UserAndContainerSnapshot uacs in existedData)
			{
				if (users.ContainsKey(uacs.UserID))
				{
					//原来是已删除的，现在改为Normal
					if (uacs.Status != SchemaObjectStatus.Normal)
					{
						uacs.Status = SchemaObjectStatus.Normal;
						updatedInfo.Add(uacs);
					}
				}
				else
				{
					//现在不存在了，状态需要改为已删除
					uacs.Status = SchemaObjectStatus.Deleted;
					updatedInfo.Add(uacs);
				}
			}

			return updatedInfo;
		}

		/// <summary>
		/// 根据时间信息得到需要查询的表名
		/// </summary>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		private string GetLoadingTableName(DateTime timePoint)
		{
			string result = this.GetMappingInfo().TableName;

			if (timePoint == DateTime.MinValue && TimePointContext.Current.UseCurrentTime)
				result += "_Current";

			return result;
		}
	}
}
