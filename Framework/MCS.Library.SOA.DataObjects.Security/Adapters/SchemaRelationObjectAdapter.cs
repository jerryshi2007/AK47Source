using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Schemas.Adapters;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	/// <summary>
	/// 表示关系对象的适配器。这里的关系对象主要表示组织和人员等父子关系。这个适配器是这些关系的读写操作
	/// </summary>
	public class SchemaRelationObjectAdapter : SchemaObjectAdapterBase<SCRelationObject>
	{
		/// <summary>
		/// <see cref="SchemaRelationObjectAdapter"/>的实例，此字段为只读
		/// </summary>
		public static readonly SchemaRelationObjectAdapter Instance = new SchemaRelationObjectAdapter();

		private SchemaRelationObjectAdapter()
		{
		}

		/// <summary>
		/// 根据对象ID载入关系对象
		/// </summary>
		/// <param name="objectIDs">一个或多个表示对象ID的字符串</param>
		/// <returns></returns>
		public SCParentsRelationObjectCollection LoadByObjectID(params string[] objectIDs)
		{
			return LoadByObjectID(objectIDs, DateTime.MinValue);
		}

		/// <summary>
		/// 根据对象ID和时间点载入关系对象
		/// </summary>
		/// <param name="objectIDs">一个或多个表示对象ID的字符串</param>
		/// <param name="timePoint">表示时间点的<see cref="DateTime"/>或 DateTime.MinValue 表示当前时间</param>
		/// <returns></returns>
		public SCParentsRelationObjectCollection LoadByObjectID(string[] objectIDs, DateTime timePoint)
		{
			return LoadByObjectID(objectIDs, true, timePoint);
		}

		/// <summary>
		/// 根据对象ID和时间点载入关系对象
		/// </summary>
		/// <param name="objectIDs">一个或多个表示对象ID的字符串</param>
		/// <param name="includingDeleted">为true时仅包含正常对象</param>
		/// <param name="timePoint">表示时间点的<see cref="DateTime"/>或 DateTime.MinValue 表示当前时间</param>
		/// <returns></returns>
		public SCParentsRelationObjectCollection LoadByObjectID(string[] objectIDs, bool includingDeleted, DateTime timePoint)
		{
			objectIDs.NullCheck("objectIDs");

			InSqlClauseBuilder builder = new InSqlClauseBuilder("ObjectID");

			builder.AppendItem(objectIDs);

			if (includingDeleted == false)
			{
				WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();

				where.AppendItem("Status", (int)SchemaObjectStatus.Normal);

				return Load<SCParentsRelationObjectCollection>(new ConnectiveSqlClauseCollection(where, builder), timePoint);
			}
			else
				return Load<SCParentsRelationObjectCollection>(builder, timePoint);
		}

		/// <summary>
		/// 根据对象ID和时间点载入关系对象
		/// </summary>
		/// <param name="objectIDs">一个表示对象ID的字符串</param>
		/// <param name="includingDeleted">为true时仅包含正常对象</param>
		/// <param name="timePoint">表示时间点的<see cref="DateTime"/>或 DateTime.MinValue 表示当前时间</param>
		/// <returns></returns>
		public SCParentsRelationObjectCollection LoadByObjectID(string objectID, bool includingDeleted, DateTime timePoint)
		{
			objectID.NullCheck("objectID");

			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendItem("ObjectID", objectID);

			if (includingDeleted == false)
				where.AppendItem("Status", (int)SchemaObjectStatus.Normal);

			return Load<SCParentsRelationObjectCollection>(where, timePoint);
		}

		/// <summary>
		/// 根据父级ID载入关系对象
		/// </summary>
		/// <param name="parentIDs">一个或多个表示父级ID的字符串</param>
		public SCChildrenRelationObjectCollection LoadByParentID(params string[] parentIDs)
		{
			return LoadByParentID(parentIDs, DateTime.MinValue);
		}

		/// <summary>
		/// 根据父级ID和时间点载入关系对象
		/// </summary>
		/// <param name="parentIDs">一个或多个表示父级ID的字符串</param>
		/// <param name="timePoint">表示时间点的<see cref="DateTime"/>或 DateTime.MinValue 表示当前时间</param>
		/// <returns></returns>
		public SCChildrenRelationObjectCollection LoadByParentID(string[] parentIDs, DateTime timePoint)
		{
			return LoadByParentID(parentIDs, false, timePoint);
		}

		/// <summary>
		/// 根据父级ID和时间点载入关系对象
		/// </summary>
		/// <param name="parentIDs">一个或多个表示父级ID的字符串</param>
		/// <param name="normalOnly">为true时，仅包含正常对象</param>
		/// <param name="timePoint">表示时间点的<see cref="DateTime"/>或 DateTime.MinValue 表示当前时间</param>
		/// <returns></returns>
		public SCChildrenRelationObjectCollection LoadByParentID(string[] parentIDs, bool normalOnly, DateTime timePoint)
		{
			parentIDs.NullCheck("parentIDs");

			InSqlClauseBuilder builder = new InSqlClauseBuilder("ParentID");

			builder.AppendItem(parentIDs);

			if (normalOnly)
			{
				WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
				where.AppendItem("Status", (int)SchemaObjectStatus.Normal);
				return Load<SCChildrenRelationObjectCollection>(new ConnectiveSqlClauseCollection(where, builder), timePoint);
			}
			else
				return Load<SCChildrenRelationObjectCollection>(builder, timePoint);
		}

		/// <summary>
		/// 根据父级ID和时间点载入关系对象
		/// </summary>
		/// <param name="parentID">一个表示父级ID的字符串</param>
		/// <param name="normalOnly">为true时，仅包含正常对象</param>
		/// <param name="timePoint">表示时间点的<see cref="DateTime"/>或 DateTime.MinValue 表示当前时间</param>
		/// <returns></returns>
		public SCChildrenRelationObjectCollection LoadByParentID(string parentID, bool normalOnly, DateTime timePoint)
		{
			parentID.NullCheck("parentID");

			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendItem("ParentID", parentID);
			if (normalOnly)
				where.AppendItem("Status", (int)SchemaObjectStatus.Normal);
			return Load<SCChildrenRelationObjectCollection>(where, timePoint);
		}

		/// <summary>
		/// 根据父级ID和对象ID载入对象
		/// </summary>
		/// <param name="parentID">父级ID</param>
		/// <param name="objectID">对象ID</param>
		/// <returns></returns>
		public SCRelationObject Load(string parentID, string objectID)
		{
			return Load(parentID, objectID, DateTime.MinValue);
		}

		/// <summary>
		/// 根据指定的条件和时间点载入对象
		/// </summary>
		/// <typeparam name="T">表示对象集合的类型的<see cref="SCParentsOrChildrenRelationObjectCollectionBase"/>的派生类型</typeparam>
		/// <param name="builder">包含条件的<see cref="IConnectiveSqlClause"/></param>
		/// <param name="timePoint">表示时间点的<see cref="DateTime"/>或 DateTime.MinValue 表示当前时间</param>
		/// <returns></returns>
		public T Load<T>(IConnectiveSqlClause builder, DateTime timePoint) where T : SCParentsOrChildrenRelationObjectCollectionBase, new()
		{
			IConnectiveSqlClause timeBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint);

			ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(builder, timeBuilder);

			T result = null;

			VersionedObjectAdapterHelper.Instance.FillData(GetMappingInfo().TableName, connectiveBuilder, this.GetConnectionName(),
				view =>
				{
					result = new T();

					result.LoadFromDataView(view);
				});

			return result;
		}

		/// <summary>
		/// 根据父级ID和对象ID和时间点载入对象
		/// </summary>
		/// <param name="parentID">父级ID</param>
		/// <param name="objectID">对象ID</param>
		/// <param name="timePoint">表示时间点的<see cref="DateTime"/>或 DateTime.MinValue 表示当前时间</param>
		/// <returns></returns>
		public SCRelationObject Load(string parentID, string objectID, DateTime timePoint)
		{
			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("ParentID", parentID);
			builder.AppendItem("ObjectID", objectID);

			SCRelationObjectCollection relations = Load(builder, timePoint);

			return relations.FirstOrDefault();
		}

		/// <summary>
		/// 按照FullPath进行查询
		/// </summary>
		/// <param name="fullPath"></param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public SCRelationObjectCollection LoadByFullPath(string fullPath, bool includingDeleted, DateTime timePoint)
		{
			fullPath.CheckStringIsNullOrEmpty("fullPath");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("FullPath", fullPath);

			if (includingDeleted == false)
				builder.AppendItem("Status", (int)SchemaObjectStatus.Normal);

			return Load(builder, timePoint);
		}

		/// <summary>
		/// 根据条件和时间点载入对象
		/// </summary>
		/// <param name="builder">包含条件的<see cref="IConnectiveSqlClause"/></param>
		/// <param name="timePoint">表示时间点的<see cref="DateTime"/>或 DateTime.MinValue 表示当前时间</param>
		/// <returns></returns>
		public SCRelationObjectCollection Load(IConnectiveSqlClause builder, DateTime timePoint)
		{
			IConnectiveSqlClause timeBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint);

			ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(builder, timeBuilder);

			SCRelationObjectCollection result = null;

			VersionedObjectAdapterHelper.Instance.FillData(GetMappingInfo().TableName, connectiveBuilder, this.GetConnectionName(),
				view =>
				{
					result = new SCRelationObjectCollection();

					result.LoadFromDataView(view);
				});

			return result;
		}

		/// <summary>
		/// 递归载入指定对象的子级关系
		/// </summary>
		/// <param name="obj">要获取子级关系的<see cref="SchemaObjectBase"/></param>
		/// <returns></returns>
		public SCSimpleRelationObjectCollection LoadAllChildrenRelationsRecursively(SchemaObjectBase obj)
		{
			return LoadAllChildrenRelationsRecursively(obj, null, null);
		}

		/// <summary>
		/// 递归获取所有子对象的关系
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="handler"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public SCSimpleRelationObjectCollection LoadAllChildrenRelationsRecursively(SchemaObjectBase obj, LoadingRelationsRecursivelyHandler handler, object context)
		{
			obj.NullCheck("obj");

			SCSimpleRelationObjectCollection result = new SCSimpleRelationObjectCollection();

			InSqlClauseBuilder builder = new InSqlClauseBuilder("ParentID");

			builder.AppendItem(obj.ID);

			FillAllChildrenRelationsRecursively(result, builder, handler, context);

			return result;
		}

		private void FillAllChildrenRelationsRecursively(SCSimpleRelationObjectCollection relations, InSqlClauseBuilder builder, LoadingRelationsRecursivelyHandler handler, object context)
		{
			if (builder.Count > 0)
			{
				string sql = string.Format("SELECT ParentID, ObjectID FROM {0} WHERE {1} AND VersionStartTime <= GETDATE() AND VersionEndTime > GETDATE() AND STATUS = 1",
						GetMappingInfo().TableName, builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

				DataTable table = DbHelper.RunSqlReturnDS(sql, this.GetConnectionName()).Tables[0];

				SCSimpleRelationObjectCollection tempRelations = new SCSimpleRelationObjectCollection();

				ORMapping.DataViewToCollection(tempRelations, table.DefaultView);

				InSqlClauseBuilder subBuilder = new InSqlClauseBuilder("ParentID");

				foreach (SCSimpleRelationObject r in tempRelations)
				{
					subBuilder.AppendItem(r.ID);
					relations.Add(r);
				}

				if (handler != null)
					handler(tempRelations, context);

				FillAllChildrenRelationsRecursively(relations, subBuilder, handler, context);
			}
		}

		/// <summary>
		/// 创建简单对象
		/// </summary>
		/// <returns></returns>
		protected override VersionedSimpleObject CreateSimpleObject()
		{
			return new SCSimpleRelationObject();
		}

		public int GetChildrenCount(string id, string[] childSchemaTypes, DateTime timePoint)
		{
			IConnectiveSqlClause timeBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint);

			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendItem("ParentID", id);

			InSqlClauseBuilder inSql = null;

			if (childSchemaTypes != null && childSchemaTypes.Length > 0)
			{
				inSql = new InSqlClauseBuilder("ChildSchemaType").AppendItem(childSchemaTypes);
			}

			ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(where, timeBuilder);

			if (inSql != null)
				connectiveBuilder.Add(inSql);

			string sql = "SELECT COUNT(ID) AS C FROM " + GetMappingInfo().TableName + "WHERE " + connectiveBuilder.ToSqlString(TSqlBuilder.Instance);

			int result = (int)DbHelper.RunSqlReturnScalar(sql, this.GetConnectionName());

			return result;
		}

		public int GetMaxInnerSort(string id, string[] childSchemaTypes, DateTime timePoint)
		{
			IConnectiveSqlClause timeBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint);

			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendItem("ParentID", id);

			InSqlClauseBuilder inSql = null;

			if (childSchemaTypes != null && childSchemaTypes.Length > 0)
			{
				inSql = new InSqlClauseBuilder("ChildSchemaType").AppendItem(childSchemaTypes);
			}

			ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(where, timeBuilder);

			if (inSql != null)
				connectiveBuilder.Add(inSql);


			string sql = "SELECT ISNULL(MAX(InnerSort), 0) AS C FROM " + GetMappingInfo().TableName + " WHERE " + connectiveBuilder.ToSqlString(TSqlBuilder.Instance);

			int result = (int)DbHelper.RunSqlReturnScalar(sql, this.GetConnectionName());

			return result;
		}

	}
}
