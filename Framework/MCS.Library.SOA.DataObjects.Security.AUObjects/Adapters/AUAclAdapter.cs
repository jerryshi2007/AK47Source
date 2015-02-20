using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Library.Data;
using System.Data;
using MCS.Library.Data.Mapping;
using System.Transactions;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Actions;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Schemas.Actions;
using MCS.Library.SOA.DataObjects.Schemas.Adapters;
using PCC = MCS.Library.SOA.DataObjects.Security.Client;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Adapters
{
	/// <summary>
	/// 权限中心授权信息的Adapter，专门用来读写Acl信息
	/// </summary>
	public class AUAclAdapter
	{
		public static readonly AUAclAdapter Instance = new AUAclAdapter();

		private AUAclAdapter()
		{
		}

		/// <summary>
		/// 根据userID和一组ContainerID，加载该Member所拥有的权限
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="containerIDs"></param>
		/// <returns></returns>
		public SCContainerAndPermissionCollection LoadCurrentContainerAndPermissions(string userID, IEnumerable<string> containerIDs)
		{
			var ids = containerIDs.ToArray();
			SCContainerAndPermissionCollection result = null;

			var roleIDs = (from r in new OguUser(userID).Roles.GetAllRoles() select r.ID).ToArray();

			if (ids.Length > 0 && roleIDs.Length > 0)
			{
				var timeConditon1 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("A.");
				var condition = new WhereSqlClauseBuilder();
				condition.AppendItem("A.Status", (int)SchemaObjectStatus.Normal);

				InSqlClauseBuilder inSql = new InSqlClauseBuilder("A.ContainerID");
				inSql.AppendItem(ids);

				InSqlClauseBuilder inSqlRole = new InSqlClauseBuilder("A.MemberID");
				inSqlRole.AppendItem(roleIDs);

				var sql = string.Format(
					"SELECT A.* FROM SC.Acl_Current A WHERE {0} ORDER BY SortID ",
					new ConnectiveSqlClauseCollection(timeConditon1, condition, inSql, inSqlRole).ToSqlString(TSqlBuilder.Instance));

				result = new SCContainerAndPermissionCollection();

				AUCommon.DoDbAction(() =>
				{
					using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
					{
						using (IDataReader reader = DbHelper.RunSqlReturnDR(sql, this.GetConnectionName()))
						{
							while (reader.Read())
							{
								string containerID = (string)reader["ContainerID"];
								string permission = (string)reader["ContainerPermission"];

								if (result.ContainsKey(containerID, permission) == false)
								{
									result.Add(new SCContainerAndPermission()
									{
										ContainerID = containerID,
										ContainerPermission = permission
									});
								}
							}
						}
					}
				});

				return result;
			}
			else
			{
				result = new SCContainerAndPermissionCollection();
			}

			return result;
		}

		/// <summary>
		/// 根据userID和一组ContainerID，加载该Member所拥有的权限。这是使用Cache的版本
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="containerIDs"></param>
		/// <returns></returns>
		public SCContainerAndPermissionCollection GetCurrentContainerAndPermissions(string memberID, IEnumerable<string> containerIDs)
		{
			// 目前直接使用，暂不使用Cache。但是前端需要调用这个方法。而单元测试仅测试LoadCurrentContainerAndPermissions
			return this.LoadCurrentContainerAndPermissions(memberID, containerIDs);
		}

		public SCAclContainerCollection LoadContainers(IConnectiveSqlClause condition, DateTime timePoint)
		{
			ConnectiveSqlClauseCollection timePointBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint);
			ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(condition, timePointBuilder);

			string sql = string.Format(
				"SELECT * FROM {0} WHERE {1} ORDER BY SortID",
				this.GetLoadingTableName(timePoint),
				connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

			SCAclContainerCollection result = new SCAclContainerCollection();

			AUCommon.DoDbAction(() =>
			{
				using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
				{
					using (IDataReader reader = DbHelper.RunSqlReturnDR(sql, this.GetConnectionName()))
					{
						ORMapping.DataReaderToCollection(result, reader);
					}
				}
			});

			return result;
		}

		public SCAclContainerCollection LoadByMemberID(string memberID, SchemaObjectStatus status, DateTime timePoint)
		{
			memberID.CheckStringIsNullOrEmpty("memberID");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("MemberID", memberID);
			builder.AppendItem("Status", (int)status);

			return this.LoadContainers(builder, timePoint);
		}

		public SCAclContainerCollection LoadByMemberID(string memberID, DateTime timePoint)
		{

			memberID.CheckStringIsNullOrEmpty("memberID");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("MemberID", memberID);

			return this.LoadContainers(builder, timePoint);
		}

		public SCAclMemberCollection LoadMembers(IConnectiveSqlClause condition, DateTime timePoint)
		{
			ConnectiveSqlClauseCollection timePointBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint);
			ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(condition, timePointBuilder);

			string sql = string.Format(
				"SELECT * FROM {0} WHERE {1} ORDER BY SortID",
				this.GetLoadingTableName(timePoint),
				connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

			SCAclMemberCollection result = new SCAclMemberCollection();

			AUCommon.DoDbAction(() =>
			{
				using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
				{
					using (IDataReader reader = DbHelper.RunSqlReturnDR(sql, this.GetConnectionName()))
					{
						ORMapping.DataReaderToCollection(result, reader);
					}
				}
			});

			return result;
		}

		public SCAclMemberCollection LoadByContainerID(string containerID, SchemaObjectStatus status, DateTime timePoint)
		{
			containerID.CheckStringIsNullOrEmpty("containerID");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("ContainerID", containerID);
			builder.AppendItem("Status", (int)status);

			return this.LoadMembers(builder, timePoint);
		}

		/// <summary>
		/// 加载某个容器下的Acl信息
		/// </summary>
		/// <param name="containerID"></param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public SCAclMemberCollection LoadByContainerID(string containerID, DateTime timePoint)
		{
			containerID.CheckStringIsNullOrEmpty("containerID");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("ContainerID", containerID);

			return this.LoadMembers(builder, timePoint);
		}

		/// <summary>
		/// 更新一个容器下的Acl信息
		/// </summary>
		/// <param name="containerID"></param>
		/// <param name="aclItems"></param>
		public void Update(SCAclContainer container)
		{
			container.NullCheck("container");

			container.FillMembersProperties();

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("ContainerID", container.ContainerID);

			string sql = this.GetUpdateSql(container.ContainerID, container.Members);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				DateTime dt = (DateTime)DbHelper.RunSqlReturnScalar(sql, this.GetConnectionName());

				SCActionContext.Current.TimePoint.IsMinValue(() => SCActionContext.Current.TimePoint = dt);

				scope.Complete();
			}
		}

		/// <summary>
		/// 统一修改Acl成员项的状态
		/// </summary>
		/// <param name="members"></param>
		/// <param name="status"></param>
		public void UpdateStatus(SCAclContainerOrMemberCollectionBase members, SchemaObjectStatus status)
		{
			members.NullCheck("members");

			ORMappingItemCollection mappings = this.GetMappingInfo();

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				foreach (SCAclItem aclItem in members)
				{
					aclItem.Status = status;

					string sql = VersionCommonObjectUpdateStatusSqlBuilder<SCAclItem>.Instance.ToUpdateSql(aclItem, mappings);

					DateTime dt = (DateTime)DbHelper.RunSqlReturnScalar(sql, this.GetConnectionName());

					SCActionContext.Current.TimePoint.IsMinValue(() => SCActionContext.Current.TimePoint = dt);
				}

				scope.Complete();
			}
		}

		/// <summary>
		/// 在派生类中重写时， 获取映射信息的集合
		/// </summary>
		/// <returns><see cref="ORMappingItemCollection"/>，表示映射信息</returns>
		protected virtual ORMappingItemCollection GetMappingInfo()
		{
			return ORMapping.GetMappingInfo(typeof(SCAclItem));
		}

		/// <summary>
		/// 获取连接的名称
		/// </summary>
		/// <returns>表示连接名称的<see cref="string"/>。</returns>
		protected virtual string GetConnectionName()
		{
			return AUCommon.DBConnectionName;
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

		private string GetUpdateSql(string containerID, SCAclContainerOrMemberCollectionBase members)
		{
			return VersionStrategyUpdateSqlHelper.ConstructUpdateSql(
				null, (strB, context) =>
				{
					string tableName = this.GetMappingInfo().TableName;

					ConnectiveSqlClauseCollection connectiveBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder();

					WhereSqlClauseBuilder keyBuilder = new WhereSqlClauseBuilder();

					keyBuilder.AppendItem("ContainerID", containerID);

					connectiveBuilder.Add(keyBuilder);

					strB.AppendFormat(
						"UPDATE {0} SET VersionEndTime = {1} WHERE {2}",
						tableName,
						"@currentTime",
						connectiveBuilder.ToSqlString(TSqlBuilder.Instance));

					for (int i = 0; i < members.Count; i++)
					{
						SCAclItem aclitem = members[i];

						aclitem.SortID = i;
						strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

						aclitem.VersionEndTime = ConnectionDefine.MaxVersionEndTime;

						InsertSqlClauseBuilder insertBuilder = ORMapping.GetInsertSqlClauseBuilder(aclitem, this.GetMappingInfo(), "VersionStartTime");

						insertBuilder.AppendItem("VersionStartTime", "@currentTime", "=", true);

						strB.AppendFormat("INSERT INTO {0}{1}", tableName, insertBuilder.ToSqlString(TSqlBuilder.Instance));
					}
				});
		}
	}
}
