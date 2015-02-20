using System;
using System.Collections.Generic;
using System.Linq;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Security;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace PermissionCenter
{
	internal static class DbUtil
	{
		internal static SchemaObjectCollection LoadObjects(string[] ids)
		{
			InSqlClauseBuilder inSql = new InSqlClauseBuilder("ID");
			inSql.AppendItem(ids);

			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendItem("Status", (int)SchemaObjectStatus.Normal);

			return PC.Adapters.SchemaObjectAdapter.Instance.Load(new ConnectiveSqlClauseCollection(inSql, where));
		}

		internal static SchemaObjectCollection LoadObjects(string[] ids, string[] schemaTypes)
		{
			InSqlClauseBuilder inSql = new InSqlClauseBuilder("ID");
			inSql.AppendItem(ids);

			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendItem("Status", (int)SchemaObjectStatus.Normal);

			InSqlClauseBuilder schemaIn = new InSqlClauseBuilder("SchemaType");
			schemaIn.AppendItem(schemaTypes);

			return PC.Adapters.SchemaObjectAdapter.Instance.Load(new ConnectiveSqlClauseCollection(inSql, where, schemaIn));
		}

		internal static SchemaObjectCollection LoadObjectsIgnoreStatus(string[] ids)
		{
			InSqlClauseBuilder inSql = new InSqlClauseBuilder("ID");
			inSql.AppendItem(ids);

			return PC.Adapters.SchemaObjectAdapter.Instance.Load(new ConnectiveSqlClauseCollection(inSql));
		}

		internal static SchemaObjectCollection LoadAndCheckObjects(string description, IBatchErrorAdapter errors, string[] idArray)
		{
			if (idArray.Length == 0)
				throw new ArgumentOutOfRangeException("ids", "未指定任何ID");

			InSqlClauseBuilder ib = new InSqlClauseBuilder("ID");
			ib.AppendItem(idArray);

			WhereSqlClauseBuilder wb = new WhereSqlClauseBuilder();
			wb.AppendItem("Status", (int)SchemaObjectStatus.Normal);

			SchemaObjectCollection result = PC.Adapters.SchemaObjectAdapter.Instance.Load(new ConnectiveSqlClauseCollection(ib, wb));

			ValidateObjectsByIDs(description, errors, result, idArray);
			return result;
		}

		/// <summary>
		/// 根据ID校验对象一致性
		/// </summary>
		/// <param name="description">描述</param>
		/// <param name="errors">一个集合，如果发生错误，将错误添加到此集合 - 或 - <see langword="null"/>时，忽略此集合。</param>
		/// <param name="objects"></param>
		/// <param name="ids"></param>
		internal static void ValidateObjectsByIDs(string description, IBatchErrorAdapter errors, SchemaObjectCollection objects, IEnumerable<string> ids)
		{
			foreach (string id in ids)
			{
				try
				{
					SchemaObjectBase obj = objects[id];

					(obj == null).TrueThrow("类型{0}，对象{1}不存在", description ?? "(未知)", id);
					(obj.Status != SchemaObjectStatus.Normal).TrueThrow("类型{0}，对象{1}已删除", description ?? obj.SchemaType, id);
				}
				catch (System.Exception ex)
				{
					if (errors != null)
						errors.AddError(ex);
				}
			}
		}

		/// <summary>
		/// 确定对象的最新版本是有效的
		/// </summary>
		/// <param name="obj"></param>
		internal static void EnsureObjectEffective(SCBase obj)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");

			obj = (SCBase)PC.Adapters.SchemaObjectAdapter.Instance.Load(obj.ID);

			if (obj == null || obj.Status != SchemaObjectStatus.Normal)
				throw new ObjectNotFoundException("指定的对象无效");
		}

		internal static SCBase GetEffectiveObject(SCSimpleObject obj)
		{
			return DbUtil.GetEffectiveObject(obj, null);
		}

		/// <summary>
		/// 根据<see cref="SCSimpleObject"/>获取有效的对象
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="message">对象不存在时异常消息的字符串，为 <see langword="null"/>时使用自动的字符串</param>
		/// <returns>不会是空</returns>
		/// <exception cref="ArgumentNullException">obj参数为<see langword="null"/></exception>
		/// <exception cref="ObjectNotFoundException">对象不存在或已被删除</exception>
		internal static SCBase GetEffectiveObject(SCSimpleObject obj, string message)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");

			SCBase result = (SCBase)PC.Adapters.SchemaObjectAdapter.Instance.Load(obj.ID);

			if (result == null || result.SchemaType != obj.SchemaType || result.Status != SchemaObjectStatus.Normal)
				throw new ObjectNotFoundException(message ?? string.Format("对象 {0}({1}) 已经删除或不存在", obj.DisplayName, obj.ID));

			return result;
		}

		internal static T GetEffectiveObject<T>(string id) where T : SCBase
		{
			if (id == null)
				throw new ArgumentNullException("id");

			T result = (T)PC.Adapters.SchemaObjectAdapter.Instance.Load(id);

			if (result == null)
				throw new ObjectNotFoundException(string.Format("未找到指定ID：{0} 的 {1} 类型的对象。", id, typeof(T).Name));

			if (result.Status != SchemaObjectStatus.Normal)
				throw new ObjectNotFoundException(string.Format("{0} 无效，对象为已经删除。", result.ToDescription()));

			return result;
		}

		internal static SCBase GetEffectiveObject(string id, string message)
		{
			SCBase result = (SCBase)PC.Adapters.SchemaObjectAdapter.Instance.Load(id);

			if (result == null)
				throw message != null ? new ObjectNotFoundException(message) : ObjectNotFoundException.CreateForID(id);
			else if (result.Status != SchemaObjectStatus.Normal)
				throw message != null ? new ObjectNotFoundException(message) : ObjectNotFoundException.CreateForID(id);

			return result;
		}

		internal static SCRelationObject GetParentOURelation(string ouId)
		{
			if (ouId == PC.SCOrganization.RootOrganizationID)
			{
				return null;
			}
			else
			{
				WhereSqlClauseBuilder wb = new WhereSqlClauseBuilder();
				wb.AppendItem("Status", (int)SchemaObjectStatus.Normal);
				wb.AppendItem("ParentSchemaType", "Organizations");
				wb.AppendItem("ChildSchemaType", "Organizations");
				wb.AppendItem("ObjectID", ouId);

				return PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(wb, DateTime.MinValue).FirstOrDefault();
			}
		}

		internal static void ReOrder(string objectId, string parentId, bool down, bool toEdge)
		{
			var adapter = PC.Adapters.SchemaRelationObjectAdapter.Instance;
			var relations = adapter.LoadByParentID(parentId);
			relations.Sort((m, n) => m.InnerSort.CompareTo(n.InnerSort));
			var subset = relations.FindAll(m => (m.ParentSchemaType == "Organizations" && m.Status == SchemaObjectStatus.Normal));
			int index = -1;
			int count = subset.Count;
			for (int i = count - 1; i >= 0; i--)
			{
				if (subset[i].Child.ID == objectId)
				{
					index = i;
					break;
				}
			}

			if (index >= 0)
			{
				var item = subset[index];
				if (down)
				{
					if (index < subset.Count - 1)
					{
						subset.RemoveAt(index);
						subset.Insert(toEdge ? subset.Count : (index + 1), item);
					}
				}
				else
				{
					if (index > 0)
					{
						subset.RemoveAt(index);
						subset.Insert(toEdge ? 0 : index - 1, item);
					}
				}

				using (System.Transactions.TransactionScope scope = TransactionScopeFactory.Create())
				{
					for (int i = 0; i < count; i++)
					{
						subset[i].InnerSort = i;
						adapter.Update(subset[i]);
					}

					scope.Complete();
				}
			}
		}

		internal static SCRelationObjectCollection LoadCurrentParentRelations(string[] objectIDs, string[] parentSchemaTypes)
		{
			InSqlClauseBuilder insql = new InSqlClauseBuilder("ParentSchemaType");
			insql.AppendItem(parentSchemaTypes);

			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendItem("Status", (int)SchemaObjectStatus.Normal);
			ConnectiveSqlClauseCollection conditions;

			if (objectIDs.Length == 1)
			{
				where.AppendItem("ObjectID", objectIDs[0]);
				conditions = new ConnectiveSqlClauseCollection(where, insql);
			}
			else
			{
				InSqlClauseBuilder insql2 = new InSqlClauseBuilder("ObjectID");
				insql2.AppendItem(objectIDs);

				conditions = new ConnectiveSqlClauseCollection(where, insql, insql2);
			}

			return PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(conditions, DateTime.MinValue);
		}

		internal static MCS.Library.SOA.DataObjects.UserCustomSearchConditionCollection LoadSearchCondition(string resourceKey, string conditionType)
		{
			return UserCustomSearchConditionAdapter.Instance.Load(c =>
			{
				c.AppendItem("RESOURCE_ID", resourceKey);
				c.AppendItem("CONDITION_TYPE", conditionType);
				c.AppendItem("USER_ID", Util.CurrentUser.ID);
			});
		}

		internal static SCApplication LoadApplicationForRole(SCRole role)
		{
			var appRelation = PC.Adapters.SCMemberRelationAdapter.Instance.LoadByMemberID(role.ID, "Applications").Find(m => m.Status == SchemaObjectStatus.Normal);
			if (appRelation == null)
				throw new ObjectNotFoundException("未找到角色对应的应用");

			return (SCApplication)appRelation.Container;
		}
	}
}