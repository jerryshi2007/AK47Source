using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.Data;

namespace PermissionCenter.Adapters
{
	internal sealed class QueryByUserAdapter
	{
		private static readonly string roleAndUserSql = @"SELECT R.ID AS RoleID, R.Name AS RoleName,R.CodeName AS RoleCodeName, R.Comment ,A.ID AS AppID,A.Name AS AppName, A.CodeName AS AppCodeName
FROM SC.SchemaUserSnapshot_Current U INNER JOIN SC.SchemaMembersSnapshot_Current M ON U.ID = M.MemberID 
INNER JOIN SC.SchemaRoleSnapshot_Current R ON R.ID = M.ContainerID 
INNER JOIN SC.SchemaMembersSnapshot_Current M0 ON M0.MemberID = R.ID 
INNER JOIN SC.SchemaApplicationSnapshot_Current A ON A.ID = M0.ContainerID
WHERE ";

		private static readonly string roleSql = @"SELECT R.ID AS RoleID, R.Name AS RoleName,R.CodeName AS RoleCodeName, R.Comment ,A.ID AS AppID,A.Name AS AppName, A.CodeName AS AppCodeName
FROM SC.SchemaRoleSnapshot_Current R INNER JOIN SC.SchemaMembersSnapshot_Current M0 ON M0.MemberID = R.ID 
INNER JOIN SC.SchemaApplicationSnapshot_Current A ON A.ID = M0.ContainerID
WHERE ";

		private class TimePointScope : IDisposable
		{
			private TimePointContext state;
			private bool disposed = false;

			public TimePointScope()
			{
				this.state = TimePointContext.GetCurrentState();
			}

			public void Dispose()
			{
				if (this.disposed)
					throw new InvalidOperationException("The TimePointScope is disposed");

				TimePointContext.RestoreCurrentState(state);
				this.disposed = true;
			}
		}

		public static readonly QueryByUserAdapter Instance = new QueryByUserAdapter();

		public QueryByUserAdapter()
		{

		}

		public DataTable QueryRoles(string userID)
		{
			using (TimePointScope scope = new TimePointScope())
			{
				var allConditions = MergeConditionForRoleAndUser((WhereSqlClauseBuilder)new WhereSqlClauseBuilder().AppendItem("U.ID", userID));

				string sql = roleAndUserSql + allConditions.ToSqlString(TSqlBuilder.Instance);

				return DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];
			}
		}

		public DataTable QueryMatrices(string userID)
		{
			HashSet<string> keys = new HashSet<string>(SOARolePropertiesAdapter.Instance.OperatorBelongToRoleIDsDirectly(userID));

			InSqlClauseBuilder roleInSql = new InSqlClauseBuilder("R.ID");
			roleInSql.AppendItem(keys.ToArray());

			if (roleInSql.IsEmpty == false)
			{
				string sql = roleSql + MergeConditionForRole(roleInSql).ToSqlString(TSqlBuilder.Instance);

				return DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];
			}
			else
			{
				return EmptyTable();
			}
		}

		private DataTable EmptyTable()
		{
			DataTable empty = new DataTable("Table");
			empty.Columns.Add("RoleID", typeof(string));
			empty.Columns.Add("RoleName", typeof(string));
			empty.Columns.Add("RoleCodeName", typeof(string));
			empty.Columns.Add("Comment", typeof(string));
			empty.Columns.Add("AppID", typeof(string));
			empty.Columns.Add("AppName", typeof(string));
			empty.Columns.Add("AppCodeName", typeof(string));

			return empty;
		}

		private static ConnectiveSqlClauseCollection MergeConditionForRoleAndUser(IConnectiveSqlClause where)
		{
			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("R.SchemaType", "Roles");
			builder.AppendItem("A.SchemaType", "Applications");
			builder.AppendItem("R.Status", (int)SchemaObjectStatus.Normal);
			builder.AppendItem("U.Status", (int)SchemaObjectStatus.Normal);
			builder.AppendItem("M.Status", (int)SchemaObjectStatus.Normal);
			builder.AppendItem("M0.Status", (int)SchemaObjectStatus.Normal);

			var versionRole = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("R.");
			var versionUser = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("U.");
			var versionApp = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("A.");
			var versionM = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("M.");
			var versionM0 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("M0.");

			return new ConnectiveSqlClauseCollection(where, builder, versionRole, versionUser, versionM, versionM0, versionApp);
		}

		private static ConnectiveSqlClauseCollection MergeConditionForRole(IConnectiveSqlClause where)
		{
			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("R.SchemaType", "Roles");
			builder.AppendItem("A.SchemaType", "Applications");
			builder.AppendItem("R.Status", (int)SchemaObjectStatus.Normal);
			builder.AppendItem("M0.Status", (int)SchemaObjectStatus.Normal);

			var versionRole = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("R.");
			var versionApp = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("A.");
			var versionM0 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder("M0.");

			return new ConnectiveSqlClauseCollection(where, builder, versionRole, versionM0, versionApp);
		}

		private string GetConnectionName()
		{
			return ConnectionNameMappingSettings.GetConfig().GetConnectionName("PermissionsCenter", "PermissionsCenter");
		}

		public int DisplaceUserInRole(string roleID, string userID, string[] displacingUserIDArray)
		{
			if (string.IsNullOrEmpty(userID))
				throw new ArgumentException("userID is required and cannot be empty", "userID");

			if (displacingUserIDArray == null)
				throw new ArgumentNullException("displacingUserIDArray");

			bool userIsInTarget = false;
			int result = 0;
			HashSet<string> allKeys = new HashSet<string>(displacingUserIDArray); // 除重用
			if (allKeys.Contains(userID))
			{
				userIsInTarget = true;
			}

			displacingUserIDArray = allKeys.ToArray();


			using (TimePointScope scope = new TimePointScope())
			{
				SchemaObjectCollection loadedUsers = displacingUserIDArray.Length > 0 ? LoadObjects(displacingUserIDArray) : new SchemaObjectCollection();

				SCUser sourceUser = userIsInTarget ? (SCUser)loadedUsers[userID] : (SCUser)PC.Adapters.SchemaObjectAdapter.Instance.Load(userID);

				if (sourceUser != null && sourceUser.Status == MCS.Library.SOA.DataObjects.Schemas.SchemaProperties.SchemaObjectStatus.Normal)
				{
					if (string.IsNullOrEmpty(roleID) == false)
					{
						SCRole role = (SCRole)PC.Adapters.SchemaObjectAdapter.Instance.Load(roleID);
						if (role != null && role.Status == MCS.Library.SOA.DataObjects.Schemas.SchemaProperties.SchemaObjectStatus.Normal)
						{
							result = DoDisplaceUserInRole(loadedUsers, sourceUser, role);
						}
					}
					else
					{
						var posibleRoleRelations = PC.Adapters.SCMemberRelationAdapter.Instance.LoadByMemberID(sourceUser.ID, "Roles", true, DateTime.MinValue);
						if (posibleRoleRelations.Count > 0)
						{
							var possibleRoles = LoadObjects(posibleRoleRelations.ToContainerIDArray());
							using (System.Transactions.TransactionScope tran = TransactionScopeFactory.Create())
							{
								bool allSuccess = true;
								foreach (SCRole r in possibleRoles)
								{
									if (DoDisplaceUserInRole(loadedUsers, sourceUser, r) == 0)
									{
										allSuccess = false;
										break;
									}
								}

								if (allSuccess)
								{
									tran.Complete();
									result = 1;
								}
								else
								{
									result = 0;
								}
							}
						}
					}
				}
			}

			return result;
		}

		private SchemaObjectCollection LoadObjects(string[] displacingUserIDArray)
		{
			InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("ID");
			inBuilder.AppendItem(displacingUserIDArray);

			var where = new WhereSqlClauseBuilder().AppendItem("Status", (int)SchemaObjectStatus.Normal);
			return PC.Adapters.SchemaObjectAdapter.Instance.Load(new ConnectiveSqlClauseCollection(inBuilder, (IConnectiveSqlClause)where), DateTime.MinValue);
		}

		private static int DoDisplaceUserInRole(SchemaObjectCollection targetUsers, SCUser sourceUser, SCRole role)
		{
			int result = 0;
			var members = PC.Adapters.SCMemberRelationAdapter.Instance.LoadByContainerID(role.ID, DateTime.MinValue);
			if (members.ContainsKey(sourceUser.ID) && members[sourceUser.ID].Status == MCS.Library.SOA.DataObjects.Schemas.SchemaProperties.SchemaObjectStatus.Normal)
			{
				using (System.Transactions.TransactionScope scope = TransactionScopeFactory.Create())
				{
					bool hasAnyChanges = false;
					if (targetUsers.ContainsKey(sourceUser.ID) == false)
					{
						PC.Executors.SCObjectOperations.InstanceWithoutPermissions.RemoveMemberFromRole(sourceUser, role);
						hasAnyChanges = true;
					}

					foreach (SCUser user in targetUsers)
					{
						if (user.ID != sourceUser.ID && user.Status == SchemaObjectStatus.Normal)
						{
							if (members.ContainsKey(user.ID) == false || members[user.ID].Status != SchemaObjectStatus.Normal)
							{
								PC.Executors.SCObjectOperations.InstanceWithoutPermissions.AddMemberToRole(user, role);
								hasAnyChanges = true;
							}
						}
					}

					scope.Complete();

					if (hasAnyChanges)
						result = 1;
				}
			}
			return result;
		}
	}
}