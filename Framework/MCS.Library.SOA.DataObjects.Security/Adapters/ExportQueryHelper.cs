using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	public static class ExportQueryHelper
	{
		/// <summary>
		/// 载入指定对象的双向关系
		/// </summary>
		/// <param name="ids">对象的ID</param>
		/// <returns></returns>
		public static SCRelationObjectCollection LoadFullRelations(string[] ids)
		{
			if (ids == null || ids.Length == 0)
				throw new ArgumentNullException("ids");

			var idsParent = new InSqlClauseBuilder("ParentID");
			idsParent.AppendItem(ids);

			var idsChild = new InSqlClauseBuilder("ObjectID");
			idsChild.AppendItem(ids);

			var idConditions = new ConnectiveSqlClauseCollection(LogicOperatorDefine.Or, idsParent, idsChild);

			var builder = new WhereSqlClauseBuilder();
			builder.AppendItem("Status", (int)SchemaObjectStatus.Normal);

			return Adapters.SchemaRelationObjectAdapter.Instance.Load(new ConnectiveSqlClauseCollection(idConditions, builder), DateTime.MinValue);
		}

		public static SCMemberRelationCollection LoadMembershipFor(string[] memberIds, string containerId)
		{
			if (memberIds.Length > 0)
			{
				WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();

				where.AppendItem("Status", (int)SchemaObjectStatus.Normal);

				if (containerId != null)
				{
					where.AppendItem("ContainerID", containerId);
				}

				InSqlClauseBuilder idIn = new InSqlClauseBuilder("MemberID");

				idIn.AppendItem(memberIds);

				return Adapters.SCMemberRelationAdapter.Instance.Load(new ConnectiveSqlClauseCollection(where, idIn), DateTime.MinValue);
			}
			else
			{
				return new SCMemberRelationCollection();
			}
		}

		/// <summary>
		/// 载入指定ID的对象的成员关系
		/// </summary>
		/// <param name="memberIds">作为成员的对象的ID</param>
		/// <returns></returns>
		public static SCMemberRelationCollection LoadMembershipFor(string[] memberIds)
		{
			return LoadMembershipFor(memberIds, null);
		}

		public static Conditions.SCConditionCollection LoadConditions(string[] ids)
		{
			if (ids.Length > 0)
			{
				var mIdIn = new InSqlClauseBuilder("OwnerID");
				mIdIn.AppendItem(ids);

				WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
				where.AppendItem("Status", (int)SchemaObjectStatus.Normal);

				return Adapters.SCConditionAdapter.Instance.Load(new ConnectiveSqlClauseCollection(mIdIn, where), DateTime.MinValue);
			}
			else
			{
				return new Conditions.SCConditionCollection();
			}
		}

		public static SCMemberRelationCollection LoadFullMemberships(string[] ids)
		{
			return LoadFullMemberships(ids, true);
		}

		public static SCMemberRelationCollection LoadFullMemberships(string[] ids, bool normalOnly)
		{
			if (ids == null)
				throw new ArgumentNullException("ids");

			if (ids.Length > 0)
			{
				var idsParent = new InSqlClauseBuilder("ContainerID");
				idsParent.AppendItem(ids);

				var idsChild = new InSqlClauseBuilder("MemberID");
				idsChild.AppendItem(ids);

				var idConditions = new ConnectiveSqlClauseCollection(LogicOperatorDefine.Or, idsParent, idsChild);

				var builder = new WhereSqlClauseBuilder();
				if (normalOnly)
					builder.AppendItem("Status", (int)SchemaObjectStatus.Normal);

				return Adapters.SCMemberRelationAdapter.Instance.Load(new ConnectiveSqlClauseCollection(idConditions, builder), DateTime.MinValue);
			}
			else
			{
				return new SCMemberRelationCollection();
			}
		}

		/// <summary>
		/// 根据ID载入正常对象
		/// </summary>
		/// <param name="ids">对象的ID</param>
		/// <param name="category">不为<see langword="null"/>时，限定对象的类型</param>
		/// <returns></returns>
		public static SchemaObjectCollection LoadObjects(string[] ids, string schemaType)
		{
			return LoadObjects(ids, schemaType, true);
		}


		/// <summary>
		/// 根据ID载入对象
		/// </summary>
		/// <param name="ids">对象的ID</param>
		/// <param name="schemaType">不为<see langword="null"/>时，限定对象的类型</param>
		/// <param name="normalOnly">如果为true，仅包含正常对象</param>
		/// <returns></returns>
		public static SchemaObjectCollection LoadObjects(string[] ids, string schemaType, bool normalOnly)
		{
			if (ids.Length > 0)
			{
				WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
				if (normalOnly)
					where.AppendItem("Status", (int)SchemaObjectStatus.Normal);

				if (schemaType != null)
					where.AppendItem("SchemaType", schemaType);

				InSqlClauseBuilder idIn = new InSqlClauseBuilder("ID");
				idIn.AppendItem(ids);

				return Adapters.SchemaObjectAdapter.Instance.Load(new ConnectiveSqlClauseCollection(where, idIn), DateTime.MinValue);
			}
			else
			{
				return new SchemaObjectCollection();
			}
		}

		/// <summary>
		/// 载入指定ID对象的成员关系
		/// </summary>
		/// <param name="containerIDs">对象的ID，其成员的关系将被获取。</param>
		/// <returns></returns>
		public static SCMemberRelationCollection LoadMembershipOf(string[] containerIDs)
		{
			return LoadMembershipOf(containerIDs, null, null);
		}

		/// <summary>
		/// 载入指定ID对象的成员关系
		/// </summary>
		/// <param name="containerIDs">对象的ID，其成员的关系将被获取。</param>
		/// <param name="containerSchemaType">当不为<see langword="null"/>时，限定容器的模式类型</param>
		/// <returns></returns>
		public static SCMemberRelationCollection LoadMembershipOf(string[] containerIDs, string containerSchemaType)
		{
			return LoadMembershipOf(containerIDs, containerSchemaType, null);
		}

		/// <summary>
		/// 载入指定ID对象的成员关系
		/// </summary>
		/// <param name="containerIDs">对象的ID，其成员的关系将被获取。</param>
		/// <param name="containerSchemaType">当不为<see langword="null"/>时，限定容器的模式类型</param>
		/// <returns></returns>
		public static SCMemberRelationCollection LoadMembershipOf(string[] containerIDs, string containerSchemaType, string memberSchemaType)
		{
			return LoadMembershipOf(containerIDs, containerSchemaType, memberSchemaType, true);
		}

		/// <summary>
		/// 载入指定ID对象的成员关系
		/// </summary>
		/// <param name="containerIDs">对象的ID，其成员的关系将被获取。</param>
		/// <param name="containerSchemaType">当不为<see langword="null"/>时，限定容器的模式类型</param>
		/// <param name="memberSchemaType">当不为<see langword="null"/>时，限定容器成员的模式类型</param>
		/// <param name="normalOnly">当为true时，仅筛选关系正常的对象</param>
		/// <returns></returns>
		public static SCMemberRelationCollection LoadMembershipOf(string[] containerIDs, string containerSchemaType, string memberSchemaType, bool normalOnly)
		{
			if (containerIDs.Length > 0)
			{
				var mInsql = new InSqlClauseBuilder("ContainerID");
				mInsql.AppendItem(containerIDs);

				var mWhere = new WhereSqlClauseBuilder();
				mWhere.AppendItem("Status", (int)SchemaObjectStatus.Normal);

				if (containerSchemaType != null)
				{
					mWhere.AppendItem("ContainerSchemaType", containerSchemaType);
				}

				if (memberSchemaType != null)
				{
					mWhere.AppendItem("MemberSchemaType", memberSchemaType);
				}

				var mAllConditions = new ConnectiveSqlClauseCollection(mInsql, mWhere);

				return Adapters.SCMemberRelationAdapter.Instance.Load(mAllConditions, DateTime.MinValue);
			}
			else
			{
				return new SCMemberRelationCollection();
			}
		}

		public static Permissions.SCAclContainerCollection LoadAclsFor(string[] objectIDs)
		{
			if (objectIDs.Length > 0)
			{
				InSqlClauseBuilder idIn1 = new InSqlClauseBuilder("ContainerID");
				WhereSqlClauseBuilder where1 = new WhereSqlClauseBuilder();

				where1.AppendItem("Status", (int)SchemaObjectStatus.Normal);
				idIn1.AppendItem(objectIDs);

				var acls = Adapters.SCAclAdapter.Instance.LoadContainers(new ConnectiveSqlClauseCollection(idIn1, where1), DateTime.MinValue);

				return acls;
			}
			else
			{
				return new Permissions.SCAclContainerCollection();
			}
		}

		public static SCSimpleRelationObjectCollection LoadRelations(string parentId, string[] requestIds)
		{
			if (requestIds.Length == 0)
			{
				return new SCSimpleRelationObjectCollection();
			}
			else
			{
				WhereSqlClauseBuilder whereSql = new WhereSqlClauseBuilder();

				whereSql.AppendItem("ParentID", parentId);
				whereSql.AppendItem("Status", (int)SchemaObjectStatus.Normal);

				InSqlClauseBuilder inSql = new InSqlClauseBuilder("ObjectID");
				inSql.AppendItem(requestIds);

				var relations = Adapters.SchemaRelationObjectAdapter.Instance.Load(new ConnectiveSqlClauseCollection(whereSql, inSql), DateTime.MinValue);

				SCSimpleRelationObjectCollection result = new SCSimpleRelationObjectCollection();

				foreach (var r in relations)
				{
					result.Add(new SCSimpleRelationObject()
					{
						CreateDate = r.CreateDate,
						Creator = r.Creator,
						ID = r.ID,
						ParentID = r.ParentID,
						SchemaType = r.SchemaType,
						Status = r.Status,
						VersionEndTime = r.VersionEndTime,
						VersionStartTime = r.VersionStartTime
					});
				}

				return result;
			}
		}
	}
}
