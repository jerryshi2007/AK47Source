using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using System.Data;
using MCS.Library.Caching;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Adapters
{
	public sealed class AUSnapshotAdapter
	{
		private AUSnapshotAdapter()
		{
		}

		public static readonly AUSnapshotAdapter Instance = new AUSnapshotAdapter();

		#region SQL模板
		private static readonly string sqlTemplateSubUnits = @"
DECLARE @time DATETIME
DECLARE @status int

SET @time = {0};
SET @status = 1;

WITH PARENT_OBJS(ID, ParentID,VersionStartTime,Status) AS
(
	SELECT ID, ParentID ,VersionStartTime,Status FROM SC.[SchemaObjectAndParentView]
	WHERE [Status] = @status AND [R_Status] = @status AND VersionStartTime <= @time AND VersionEndTime > @time
		AND R_VersionStartTime <= @time AND R_VersionEndTime > @time AND SchemaType = 'AdminUnits' AND {1}
	UNION ALL
	SELECT O.ID, O.ParentID, O.VersionStartTime, O.Status
	FROM SC.[SchemaObjectAndParentView] O INNER JOIN PARENT_OBJS P ON P.ID = O.ParentID
	WHERE O.[Status] = @status AND P.[Status] = @status AND R_Status = @status AND O.VersionStartTime <= @time AND O.VersionEndTime > @time
		AND R_VersionStartTime <= @time AND R_VersionEndTime > @time AND SchemaType = 'AdminUnits'
)
SELECT OO.* FROM PARENT_OBJS RO INNER JOIN SC.SchemaObject OO ON RO.ID=OO.ID AND RO.VersionStartTime = OO.VersionStartTime
";
		#endregion

		/// <summary>
		/// 根据下级名称，载入指定的管理单元
		/// </summary>
		/// <param name="name">下级名称</param>
		/// <param name="parentID">父级ID</param>
		/// <param name="childSchemaType">子级Schema类型</param>
		/// <param name="normalOnly">是否仅包含正常对象</param>
		/// <param name="timePoint">时间点</param>
		/// <returns></returns>
		public SchemaObjectCollection LoadAUByChildName(string name, string parentID, string childSchemaType, bool normalOnly, DateTime timePoint)
		{
			string sql = @"SELECT O.* FROM SC.SchemaObject O 
INNER JOIN SC.AdminUnitSnapshot S ON O.ID = S.ID AND O.VersionStartTime = S.VersionStartTime AND O.SchemaType = S.SchemaType 
INNER JOIN SC.SchemaRelationObjects R ON O.ID = R.ObjectID AND O.SchemaType = R.ChildSchemaType
WHERE 1=1 AND ";

			var timeCondition1 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "S.");
			var timeCondition2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "O.");
			var timeCondition3 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "R.");

			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendItem("R.ParentID", parentID);
			where.AppendItem("S.Name", name);
			where.AppendItem("R.ChildSchemaType", childSchemaType);

			if (normalOnly)
				where.NormalFor("R.Status").NormalFor("O.Status");

			sql += new ConnectiveSqlClauseCollection(timeCondition1, timeCondition2, timeCondition3, where).ToSqlString(TSqlBuilder.Instance);

			return LoadSchemaObjects(sql);
		}

		/// <summary>
		/// 载入所有下级单元对象
		/// </summary>
		/// <param name="parentIDs"></param>
		/// <param name="normalOnly"></param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public SchemaObjectCollection LoadCurrentSubUnitsDeeply(string[] parentIDs, DateTime timePoint)
		{
			parentIDs.NullCheck("parentIDs");

			if (parentIDs.Length > 0)
			{
				InSqlClauseBuilder inSql = new InSqlClauseBuilder("ParentID").AppendItem(parentIDs);

				string timeFilterClause = timePoint == DateTime.MinValue ? TSqlBuilder.Instance.DBCurrentTimeFunction : TSqlBuilder.Instance.FormatDateTime(timePoint);
				string conditionClause = inSql.ToSqlString(TSqlBuilder.Instance);
				string sql = string.Format(sqlTemplateSubUnits, timeFilterClause, conditionClause);
				return LoadSchemaObjects(sql);
			}
			else
				return new SchemaObjectCollection();
		}

		/// <summary>
		/// 载入所有下级管理单元
		/// </summary>
		/// <param name="schemaID">管理单元的AUSchemaID</param>
		/// <param name="parentID">上级管理单元的ID</param>
		/// <param name="normalOnly">是否仅含正常数据</param>
		/// <param name="timePoint">时间点</param>
		/// <returns></returns>
		public SchemaObjectCollection LoadSubUnits(string schemaID, string[] parentIDs, bool normalOnly, DateTime timePoint)
		{
			parentIDs.NullCheck("parentIDs");

			if (parentIDs.Length > 0)
			{
				WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
				where.AppendCondition("S.AUSchemaID", schemaID);
				where.AppendItem("R.ChildSchemaType", AUCommon.SchemaAdminUnit);

				if (normalOnly)
					where.NormalFor("R.Status").NormalFor("O.Status");

				InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("R.ParentID");

				return LoadSubUnits(timePoint, new ConnectiveSqlClauseCollection(where, inBuilder));
			}
			else
				return new SchemaObjectCollection();
		}

		/// <summary>
		/// 载入所有下级管理单元
		/// </summary>
		/// <param name="schemaID">管理单元的AUSchemaID</param>
		/// <param name="parentID">上级管理单元，如果为null则取schemaID</param>
		/// <param name="normalOnly">是否仅含正常数据</param>
		/// <param name="timePoint">时间点</param>
		/// <returns></returns>
		public SchemaObjectCollection LoadSubUnits(string schemaID, string parentID, bool normalOnly, DateTime timePoint)
		{
			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			if (parentID == null)
				parentID = schemaID;
			where.AppendItem("R.ParentID", parentID);
			where.AppendCondition("S.AUSchemaID", schemaID);
			where.AppendItem("R.ChildSchemaType", AUCommon.SchemaAdminUnit);

			if (normalOnly)
				where.NormalFor("R.Status").NormalFor("O.Status");

			return LoadSubUnits(timePoint, where);
		}

		internal SchemaObjectCollection LoadSubUnits(DateTime timePoint, IConnectiveSqlClause where)
		{
			string sql = @"SELECT O.* FROM SC.SchemaObject O 
INNER JOIN SC.AdminUnitSnapshot S ON O.ID = S.ID AND O.VersionStartTime = S.VersionStartTime AND O.SchemaType = S.SchemaType 
INNER JOIN SC.SchemaRelationObjects R ON O.ID = R.ObjectID AND O.SchemaType = R.ChildSchemaType
WHERE 1=1 AND ";

			var timeCondition1 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "S.");
			var timeCondition2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "O.");
			var timeCondition3 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "R.");

			sql += new ConnectiveSqlClauseCollection(timeCondition1, timeCondition2, timeCondition3, where).ToSqlString(TSqlBuilder.Instance);

			return LoadSchemaObjects(sql);
		}

		/// <summary>
		/// 根据条件，载入指定对象的所有容器对象
		/// </summary>
		/// <param name="memberID"></param>
		/// <param name="memberSchemaType"></param>
		/// <param name="containerSchemaType"></param>
		/// <param name="normalOnly"></param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public SchemaObjectCollection LoadContainers(string memberID, string memberSchemaType, string containerSchemaType, bool normalOnly, DateTime timePoint)
		{
			memberID.NullCheck("memberID");
			containerSchemaType.NullCheck("containerSchemaType");
			memberSchemaType.NullCheck("memberSchemaType");

			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendCondition("M.MemberID", memberID);
			where.AppendCondition("M.MemberSchemaType", memberSchemaType);
			where.AppendCondition("M.ContainerSchemaType", containerSchemaType);

			return LoadContainers(where, normalOnly, timePoint);
		}

		/// <summary>
		/// 根据下级名称，载入指定的管理单元
		/// </summary>
		/// <param name="conditions">条件集,O对象表，M关系表，S对象的快照表</param>
		/// <param name="snapshotTable">快照表的名称</param>
		/// <param name="normalOnly">是否仅包含正常对象</param>
		/// <param name="timePoint">时间点</param>
		/// <returns></returns>
		private SchemaObjectCollection LoadContainers(IConnectiveSqlClause conditions, bool normalOnly, DateTime timePoint)
		{
			string sql = @"SELECT O.* FROM SC.SchemaObject O 
INNER JOIN SC.SchemaMembers M ON O.ID = M.ContainerID AND O.SchemaType = M.ContainerSchemaType
WHERE 1=1 AND ";

			var timeCondition2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "O.");
			var timeCondition3 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "M.");

			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			if (normalOnly)
				where.NormalFor("M.Status").NormalFor("O.Status");

			sql += new ConnectiveSqlClauseCollection(timeCondition2, timeCondition3, where, conditions).ToSqlString(TSqlBuilder.Instance);

			return LoadSchemaObjects(sql);
		}

		/// <summary>
		/// 根据类别ID和时间点载入管理单元Schema
		/// </summary>
		/// <param name="name">名称</param>
		/// <param name="categoryID">类别ID</param>
		/// <param name="timePoint">时间点</param>
		/// <param name="normalOnly">是否仅筛选正常对象</param>
		/// <returns></returns>
		public SchemaObjectCollection LoadAUSchemaByCategory(string categoryID, bool normalOnly, DateTime timePoint)
		{
			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendItem("S.CategoryID", categoryID);
			var result = LoadAUSchema(where, normalOnly, timePoint);

			return result;
		}

		/// <summary>
		/// 根据名称，类别ID和时间点载入管理单元Schema
		/// </summary>
		/// <param name="name">名称</param>
		/// <param name="categoryID">类别ID</param>
		/// <param name="timePoint">时间点</param>
		/// <param name="normalOnly">是否仅筛选正常对象</param>
		/// <returns></returns>
		public SchemaObjectCollection LoadAUSchemaByName(string name, string categoryID, bool normalOnly, DateTime timePoint)
		{
			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendItem("S.Name", name);
			where.AppendItem("S.CategoryID", categoryID);
			var result = LoadAUSchema(where, normalOnly, timePoint);

			return result;
		}

		/// <summary>
		/// 根据管理单元Schema名称载入管理单元Schema
		/// </summary>
		/// <param name="name">名称</param>
		/// <param name="categoryID">类别的ID</param>
		/// <returns></returns>
		public SchemaObjectCollection LoadAUSchemaByName(string name, string categoryID, bool normalOnly)
		{
			return LoadAUSchemaByName(name, categoryID, normalOnly, DateTime.MinValue);
		}

		/// <summary>
		/// 根据代码名称和时间点，载入管理单元Schema
		/// </summary>
		/// <param name="codeName">代码名称</param>
		/// <param name="timePoint">时间点</param>
		/// <returns></returns>
		public SchemaObjectCollection LoadAUSchemaByCodeName(string codeName, bool normalOnly, DateTime timePoint)
		{
			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendItem("S.CodeName", codeName);
			return LoadAUSchema(where, normalOnly, timePoint);
		}

		/// <summary>
		/// 根据代码名称载入当前管理单元Schema
		/// </summary>
		/// <param name="codeName">代码名称</param>
		/// <returns></returns>
		public SchemaObjectCollection LoadAUSchemaByCodeName(string codeName, bool normalOnly)
		{
			return LoadAUSchemaByCodeName(codeName, normalOnly, DateTime.MinValue);
		}

		public SchemaObjectCollection LoadAUSchema(string id, bool normalOnly, DateTime timePoint)
		{
			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendItem("S.ID", id);
			return LoadAUSchema(where, normalOnly, timePoint);
		}

		/// <summary>
		/// 载入管理单元Schema
		/// </summary>
		/// <param name="condition">包含查询条件的<see cref="IConnectiveSqlClause"/></param>
		/// <param name="timePoint">时间点</param>
		/// <param name="includingDeleted">是否包含删除的</param>
		/// <returns></returns>
		internal SchemaObjectCollection LoadAUSchema(IConnectiveSqlClause condition, bool normalOnly, DateTime timePoint)
		{
			//TODO:测试
			string sql = @"SELECT O.* FROM SC.SchemaObject O INNER JOIN SC.AUSchemaSnapshot S
ON O.ID = S.ID AND O.VersionStartTime = S.VersionStartTime AND O.SchemaType = S.SchemaType
 WHERE 1=1 AND ";

			var timeCondition1 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "S.");
			var timeCondition2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "O.");


			SchemaObjectCollection result = new SchemaObjectCollection();

			ConnectiveSqlClauseCollection allConditions = new ConnectiveSqlClauseCollection(timeCondition1, timeCondition2, condition);

			if (normalOnly)
				allConditions.Add(new WhereSqlClauseBuilder().NormalFor("O.Status"));

			sql += allConditions.ToSqlString(TSqlBuilder.Instance);
			return LoadSchemaObjects(sql);
		}

		/// <summary>
		/// 载入管理单元
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="timePoint">时间点</param>
		/// <returns></returns>
		/// <remarks>O为SchemaObject,S为AdminUnitSnapshot</remarks>
		internal SchemaObjectCollection LoadAU(IConnectiveSqlClause condition, DateTime timePoint)
		{
			//TODO:测试
			string sql = @"SELECT O.* FROM SC.SchemaObject O INNER JOIN SC.AdminUnitSnapshot S
ON O.ID = S.ID AND O.VersionStartTime = S.VersionStartTime AND O.SchemaType = S.SchemaType
 WHERE 1=1 AND ";

			var timeCondition1 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "S.");
			var timeCondition2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "O.");

			sql += new ConnectiveSqlClauseCollection(timeCondition1, timeCondition2, condition).ToSqlString(TSqlBuilder.Instance);

			return LoadSchemaObjects(sql);
		}

		private string GetConnectionName()
		{
			return AUCommon.DBConnectionName;
		}

		/// <summary>
		/// 载入指定SchemaID的管理单元
		/// </summary>
		/// <param name="auSchemaID"></param>
		/// <param name="normalOnly">为true时表示所有</param>
		/// <param name="timePoint">时间点</param>
		/// <returns></returns>
		public SchemaObjectCollection LoadAUBySchemaID(string auSchemaID, bool normalOnly, DateTime timePoint)
		{
			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendItem("S.AUSchemaID", auSchemaID);
			if (normalOnly)
				where.NormalFor("O.Status");

			return LoadAU(where, timePoint);
		}

		/// <summary>
		/// 载入指定SchemaID的管理单元
		/// </summary>
		/// <param name="auSchemaID">管理架构的ID</param>
		/// <param name="unitIDs">管理单元ID的集合，或为null，表示所有架构。空数组表示所有</param>
		/// <param name="normalOnly">为true时表示所有</param>
		/// <param name="timePoint">时间点</param>
		/// <returns></returns>
		public SchemaObjectCollection LoadAUBySchemaID(string auSchemaID, string[] unitIDs, bool normalOnly, DateTime timePoint)
		{
			auSchemaID.NullCheck("auSchemaID");

			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendItem("S.AUSchemaID", auSchemaID);
			if (normalOnly)
				where.NormalFor("O.Status");

			InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("S.ID");
			if (unitIDs != null)
			{
				return LoadAU(new ConnectiveSqlClauseCollection(where, new InSqlClauseBuilder("S.ID").AppendItem(unitIDs)), timePoint);
			}
			else
			{
				return LoadAU(where, DateTime.MinValue);
			}
		}

		public SchemaObjectCollection LoadAUBySchemaID(string[] auSchemaIDs, bool normalOnly, DateTime timePoint)
		{
			auSchemaIDs.NullCheck("auSchemaIDs");
			if (auSchemaIDs.Length > 0)
			{
				InSqlClauseBuilder inSql = new InSqlClauseBuilder("S.AUSchemaID");
				inSql.AppendItem(auSchemaIDs);

				return LoadAU(inSql, timePoint);
			}
			else
			{
				return new SchemaObjectCollection();
			}
		}

		public SchemaObjectCollection LoadNormalAUBySchemaID(string auSchemaID, DateTime timePoint)
		{
			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder().NormalFor("O.Status").AppendCondition("S.AUSchemaID", auSchemaID);
			return LoadAU(where, timePoint);
		}

		public SchemaObjectCollection LoadNormalAUBySchemaID(string[] auSchemaIDs, DateTime timePoint)
		{
			auSchemaIDs.NullCheck("auSchemaIDs");
			if (auSchemaIDs.Length > 0)
			{
				InSqlClauseBuilder inSql = new InSqlClauseBuilder("S.AUSchemaID");
				inSql.AppendItem(auSchemaIDs);

				WhereSqlClauseBuilder where = new WhereSqlClauseBuilder().NormalFor("O.Status").NormalFor("S.Status");
				ConnectiveSqlClauseCollection conditions = new ConnectiveSqlClauseCollection(inSql, where);

				return LoadAU(conditions, timePoint);
			}
			else
			{
				return new SchemaObjectCollection();
			}
		}

		public SchemaObjectCollection LoadAUScope(string auID, string scope, bool normalOnly, DateTime timePoint)
		{
			return LoadAUScope(auID, scope, true, true, timePoint);
		}

		public SchemaObjectCollection LoadAUScope(string auID, string scope, bool normalRelation, bool normalObject, DateTime timePoint)
		{
			auID.NullCheck("auID");

			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder().NormalFor("O.Status").NormalFor("S.Status");
			where.AppendCondition("R.ContainerID", auID);
			where.AppendCondition("S.ScopeSchemaType", scope);
			if (normalRelation)
				where.NormalFor("R.Status");

			if (normalObject)
			{
				where.NormalFor("O.Status");
			}

			return LoadAUScope(where, timePoint);
		}

		public SchemaObjectCollection LoadAUScope(string auID, string[] scopes, bool normalOnly, DateTime timePoint)
		{
			auID.NullCheck("auID");
			scopes.NullCheck("scopes");

			if (scopes.Length > 0)
			{
				WhereSqlClauseBuilder where = new WhereSqlClauseBuilder().NormalFor("O.Status").NormalFor("S.Status");
				where.AppendCondition("R.ContainerID", auID);
				where.NormalFor("R.Status");

				InSqlClauseBuilder inSql = new InSqlClauseBuilder("S.ScopeSchemaType");
				inSql.AppendItem(scopes);

				if (normalOnly)
				{
					where.NormalFor("O.Status");
				}

				return LoadAUScope(new ConnectiveSqlClauseCollection(where, inSql), timePoint);
			}
			else
			{
				return new SchemaObjectCollection();
			}
		}

		public SchemaObjectCollection LoadAUScope(string auID, bool normalOnly, DateTime timePoint)
		{
			auID.NullCheck("auID");

			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder().NormalFor("O.Status").NormalFor("S.Status");
			where.AppendCondition("R.ContainerID", auID);
			where.NormalFor("R.Status");

			if (normalOnly)
			{
				where.NormalFor("O.Status");
			}

			return LoadAUScope(where, timePoint);
		}

		internal SchemaObjectCollection LoadAUScope(IConnectiveSqlClause condition, DateTime timePoint)
		{
			string sql = @"SELECT O.* FROM SC.SchemaObject O INNER JOIN SC.AUAdminScopeSnapshot S
ON O.ID = S.ID AND O.VersionStartTime = S.VersionStartTime AND O.SchemaType = S.SchemaType
INNER JOIN SC.SchemaMembers R ON O.ID = R.MemberID AND O.SchemaType = R.MemberSchemaType
 WHERE ";

			var timeCondition1 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "S.");
			var timeCondition2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "O.");

			sql += new ConnectiveSqlClauseCollection(timeCondition1, timeCondition2, condition).ToSqlString(TSqlBuilder.Instance);

			return LoadSchemaObjects(sql);
		}

		private SchemaObjectCollection LoadSchemaObjects(string sql)
		{
			return LoadSchemaObjects<SchemaObjectBase, SchemaObjectCollection>(sql);
		}

		private TCollection LoadSchemaObjects<TKey, TCollection>(string sql)
			where TCollection : SchemaObjectEditableKeyedCollectionBase<TKey, TCollection>, new()
			where TKey : SchemaObjectBase
		{
			TCollection result = new TCollection();

			AUCommon.DoDbAction(() =>
			{
				using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
				{
					result.LoadFromDataView(DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0].DefaultView);
				}
			});

			return result;
		}

		/// <summary>
		/// 根据角色ID载入指定AUSchemaID中的角色
		/// </summary>
		/// <param name="schemaID">AUSchema的ID</param>
		/// <param name="normalOnly">如果为true，则仅提取正常的，否则包含已删除的</param>
		/// <param name="schemaRoleIDs">仅包含角色的ID</param>
		/// <param name="timePoint">时间点</param>
		/// <returns></returns>
		public AUSchemaRoleCollection LoadAUSchemaRoles(string schemaID, string[] schemaRoleIDs, bool normalOnly, DateTime timePoint)
		{
			schemaRoleIDs.NullCheck("schemaRoleIDs");

			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendCondition("R.ContainerID", schemaID);
			if (normalOnly) where.NormalFor("O.Status").NormalFor("R.Status");

			InSqlClauseBuilder inSql = new InSqlClauseBuilder("R.MemberID").AppendItem(schemaRoleIDs);
			return LoadAUSchemaRoles(new ConnectiveSqlClauseCollection(where, inSql), normalOnly, timePoint);
		}

		/// <summary>
		/// 载入指定AUSchemaID中的角色
		/// </summary>
		/// <param name="schemaID">AUSchema的ID</param>
		/// <param name="normalOnly">如果为true，则仅提取正常的，否则包含已删除的</param>
		/// <param name="timePoint">时间点</param>
		/// <returns></returns>
		public AUSchemaRoleCollection LoadAUSchemaRoles(string schemaID, bool normalOnly, DateTime timePoint)
		{
			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendCondition("R.ContainerID", schemaID);
			if (normalOnly) where.NormalFor("O.Status").NormalFor("R.Status");
			return LoadAUSchemaRoles(where, normalOnly, timePoint);
		}

		/// <summary>
		/// 根据Schema和代码名称，载入管理架构角色
		/// </summary>
		/// <param name="schemaID"></param>
		/// <param name="codeNames"></param>
		/// <param name="normalOnly"></param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public AUSchemaRoleCollection LoadAUSchemaRolesByCodeNames(string schemaID, string[] codeNames, bool normalOnly, DateTime timePoint)
		{
			codeNames.NullCheck("codeNames");
			if (codeNames.Length > 0)
			{
				WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
				where.AppendCondition("R.ContainerID", schemaID);
				if (normalOnly) where.NormalFor("O.Status").NormalFor("R.Status");

				InSqlClauseBuilder inSql = new InSqlClauseBuilder("S.CodeName");
				inSql.AppendItem(codeNames);

				return LoadAUSchemaRoles(new ConnectiveSqlClauseCollection(where, inSql), normalOnly, timePoint);
			}
			else
			{
				return new AUSchemaRoleCollection();
			}
		}

		public AUSchemaRole LoadAUSchemaRoleByCodeName(string schemaID, string codeName, bool normalOnly, DateTime timePoint)
		{
			codeName.NullCheck("codeName");
			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendCondition("R.ContainerID", schemaID);
			if (normalOnly) where.NormalFor("O.Status").NormalFor("R.Status");

			where.AppendCondition("S.CodeName", codeName);

			return LoadAUSchemaRoles(where, normalOnly, timePoint).FirstOrDefault();
		}

		internal AUSchemaRoleCollection LoadAUSchemaRoles(IConnectiveSqlClause condition, bool normalOnly, DateTime timePoint)
		{
			string sql = @"SELECT O.* FROM SC.SchemaObject O INNER JOIN SC.AUSchemaRoleSnapshot S
ON O.ID = S.ID AND O.VersionStartTime = S.VersionStartTime AND O.SchemaType = S.SchemaType
INNER JOIN SC.SchemaMembers R ON O.ID = R.MemberID AND O.SchemaType = R.MemberSchemaType
 WHERE ";

			var timeCondition1 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "S.");
			var timeCondition2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "O.");

			sql += new ConnectiveSqlClauseCollection(timeCondition1, timeCondition2, condition).ToSqlString(TSqlBuilder.Instance);

			return LoadSchemaObjects<AUSchemaRole, AUSchemaRoleCollection>(sql);
		}

		public AURole LoadAURole(string schemaRoleID, string unitID, bool normalOnly, DateTime timePoint)
		{
			string sql = @"SELECT O.* FROM SC.SchemaObject O INNER JOIN SC.AURoleSnapshot S
ON O.ID = S.ID AND O.VersionStartTime = S.VersionStartTime AND O.SchemaType = S.SchemaType
INNER JOIN SC.SchemaMembers R ON O.ID = R.MemberID AND O.SchemaType = R.MemberSchemaType
 WHERE";
			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendCondition("S.SchemaRoleID", schemaRoleID);
			where.AppendCondition("R.ContainerID", unitID);
			where.NormalFor("R.Status");

			if (normalOnly) where.NormalFor("O.Status");

			var timeCondition1 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "S.");
			var timeCondition2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "O.");

			sql += new ConnectiveSqlClauseCollection(timeCondition1, timeCondition2, where).ToSqlString(TSqlBuilder.Instance);

			return (AURole)LoadSchemaObjects(sql).FirstOrDefault();
		}

		/// <summary>
		/// 只是查询指定管理单元的所有角色
		/// </summary>
		/// <param name="unitIds"></param>
		/// <param name="normalOnly"></param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public SchemaObjectCollection LoadAURoles(string[] unitIds, bool normalOnly, DateTime timePoint)
		{
			unitIds.NullCheck("unitIds");

			if (unitIds.Length > 0)
			{
				return LoadAURolesInner(unitIds, AUCommon.ZeroLengthStringArray, normalOnly, DateTime.MinValue);
			}
			else
			{
				return new SchemaObjectCollection();
			}
		}

		/// <summary>
		/// 只是查询所有相同Schema角色ID的管理单元角色
		/// </summary>
		/// <param name="schemaRoleID"></param>
		/// <param name="normalOnly"></param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		internal SchemaObjectCollection LoadAURolesBySchemaRoleID(string[] schemaRoleIDs, bool normalOnly, DateTime timePoint)
		{
			schemaRoleIDs.NullCheck("schemaRoleIDs");
			if (schemaRoleIDs.Length == 0)
				throw new ArgumentException("schemaRoleIDs不包含任何元素", "schemaRoleIDs");

			return LoadAURolesInner(AUCommon.ZeroLengthStringArray, schemaRoleIDs, normalOnly, DateTime.MinValue);
		}

		/// <summary>
		/// 只是查询所有相同Schema角色ID的管理单元角色
		/// </summary>
		/// <param name="schemaRoleIDs"></param>
		/// <param name="normalOnly"></param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		internal SchemaObjectCollection LoadAURoles(string schemaRoleID, bool normalOnly, DateTime timePoint)
		{
			schemaRoleID.NullCheck("schemaRoleIDs");

			return LoadAURolesInner(AUCommon.ZeroLengthStringArray, new string[] { schemaRoleID }, normalOnly, DateTime.MinValue);
		}

		/// <summary>
		/// 载入指定的管理单元的角色(用于删除)
		/// </summary>
		/// <param name="unitIds">管理单元的ID的集合(如果此参数长度为0则取出所有)</param>
		/// <param name="schemaRoleIDs">管理架构角色的ID(这些角色ID应来自同一个Schema，如果此数组长度为0则取出所有)</param>
		/// <param name="normalOnly">是否仅包含正常的</param>
		/// <param name="timePoint">时间点</param>
		/// <returns></returns>
		internal SchemaObjectCollection LoadAURoles(string[] unitIds, string[] schemaRoleIDs, bool normalOnly, DateTime timePoint)
		{
			unitIds.NullCheck("unitIds");
			schemaRoleIDs.NullCheck("schemaRoleIDs");

			if (unitIds.Length == 0 && schemaRoleIDs.Length == 0)
				throw new InvalidOperationException("unitIds和schemaRoleIDs不能同时为长度为0");

			return LoadAURolesInner(unitIds, schemaRoleIDs, normalOnly, timePoint);
		}

		private SchemaObjectCollection LoadAURolesInner(string[] unitIds, string[] schemaRoleIDs, bool normalOnly, DateTime timePoint)
		{
			string sql = @"SELECT O.* FROM SC.SchemaObject O INNER JOIN SC.AURoleSnapshot S
ON O.ID = S.ID AND O.VersionStartTime = S.VersionStartTime AND O.SchemaType = S.SchemaType
INNER JOIN SC.SchemaMembers R ON O.ID = R.MemberID AND O.SchemaType = R.MemberSchemaType
 WHERE";
			IConnectiveSqlClause inBuilder = DbBuilderHelper.In("S.SchemaRoleID", schemaRoleIDs);

			IConnectiveSqlClause inBuilder2 = DbBuilderHelper.In("R.ContainerID", unitIds);

			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();

			if (normalOnly) where.NormalFor("O.Status").NormalFor("R.Status");

			var timeCondition1 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "S.");
			var timeCondition2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "O.");

			sql += new ConnectiveSqlClauseCollection(timeCondition1, timeCondition2, where, inBuilder, inBuilder2).ToSqlString(TSqlBuilder.Instance);

			return LoadSchemaObjects(sql);
		}

		public List<AURoleDisplayItem> LoadAURoleDisplayItems(string unitID, bool normalOnly, DateTime timePoint)
		{
			string sql = @"SELECT SR.Name, SR.CodeName, SR.DisplayName, S.ID, S.SchemaRoleID, S.Status FROM SC.AURoleSnapshot S
INNER JOIN SC.SchemaMembers R ON S.ID = R.MemberID AND S.SchemaType = R.MemberSchemaType
INNER JOIN SC.AUSchemaRoleSnapshot SR ON SR.ID = S.SchemaRoleID
 WHERE";
			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendCondition("R.ContainerID", unitID).AppendCondition("R.ContainerSchemaType", AUCommon.SchemaAdminUnit);
			if (normalOnly)
				where.NormalFor("R.Status").NormalFor("S.Status").NormalFor("SR.Status");

			var timeCondition1 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "S.");
			var timeCondition2 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "SR.");
			var timeCondition3 = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint, "R.");

			sql += new ConnectiveSqlClauseCollection(timeCondition1, timeCondition2, timeCondition3, where).ToSqlString(TSqlBuilder.Instance);

			List<AURoleDisplayItem> result = new List<AURoleDisplayItem>();

			AUCommon.DoDbAction(() =>
			{
				using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
				{
					using (IDataReader dr = DbHelper.RunSqlReturnDR(sql, this.GetConnectionName()))
					{
						while (dr.Read())
						{
							AURoleDisplayItem item = new AURoleDisplayItem();
							ORMapping.DataReaderToObject<AURoleDisplayItem>(dr, item);
							result.Add(item);
						}
					}
				}
			});

			return result;
		}

		public SchemaObjectCollection LoadScopeItems(string scopeType, bool normalOnly, DateTime timePoint)
		{
			string sql = "SELECT * FROM SC.SchemaObject WHERE ";

			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			if (normalOnly)
				where.NormalFor("Status");

			where.AppendItem("SchemaType", scopeType);
			var timeBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint);
			sql += new ConnectiveSqlClauseCollection(timeBuilder, where).ToSqlString(TSqlBuilder.Instance);

			return LoadSchemaObjects(sql);
		}

		public SchemaObjectCollection LoadScopeItems(string[] ids, string scopeType, bool normalOnly, DateTime timePoint)
		{
			ids.NullCheck("ids");

			string sql = "SELECT * FROM SC.SchemaObject WHERE ";

			if (ids.Length > 0)
			{
				InSqlClauseBuilder inSql = new InSqlClauseBuilder("ID").AppendItem(ids); ;
				WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
				if (normalOnly)
					where.NormalFor("Status");

				where.AppendItem("SchemaType", scopeType);
				var timeBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint);
				sql += new ConnectiveSqlClauseCollection(inSql, timeBuilder, where).ToSqlString(TSqlBuilder.Instance);

				return LoadSchemaObjects(sql);
			}
			else
			{
				return new SchemaObjectCollection();
			}
		}
	}
}
