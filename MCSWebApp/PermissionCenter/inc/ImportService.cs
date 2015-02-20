using System;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace PermissionCenter
{
	internal sealed class ImportService
	{
		public static readonly ImportService Instance = new ImportService();

		private ImportService()
		{
		}

		private PC.Executors.ISCObjectOperations Actor
		{
			get
			{
				return PC.Executors.SCObjectOperations.InstanceWithPermissions;
			}
		}

		/// <summary>
		/// 根据指定的类别过滤对象
		/// </summary>
		/// <param name="objectSet">对象集</param>
		/// <param name="categories">过滤的类别</param>
		/// <returns></returns>
		internal IEnumerable<SchemaObjectBase> FilterNormalObjectsBySchemaCategories(IEnumerable<SchemaObjectBase> objectSet, params string[] categories)
		{
			if (categories.Length > 0)
			{
				var schemas = SchemaInfo.FilterByCategory(categories);
				foreach (var obj in objectSet)
				{
					foreach (var schema in schemas)
					{
						if (obj.SchemaType == schema.Name && obj.Status == SchemaObjectStatus.Normal)
						{
							yield return obj;
							break;
						}
					}
				}
			}
			else
			{
				foreach (var item in objectSet)
				{
					yield return item;
				}
			}
		}

		/// <summary>
		/// 根据指定的类别过滤对象
		/// </summary>
		/// <param name="objectSet">对象集</param>
		/// <param name="categories">过滤的类别</param>
		/// <returns></returns>
		internal IEnumerable<T> FilterNormalObjects<T>(IEnumerable<SchemaObjectBase> objectSet) where T : SchemaObjectBase
		{
			foreach (var obj in objectSet)
			{
				if (obj.Status == SchemaObjectStatus.Normal && obj is T)
				{
					yield return (T)obj;
				}
			}
		}

		/// <summary>
		/// 根据指定的类别过滤对象
		/// </summary>
		/// <param name="objectSet">对象集</param>
		/// <param name="categories">过滤的类别</param>
		/// <returns></returns>
		internal IEnumerable<T> FilterNormalObjects<T>(IEnumerable<SchemaObjectBase> objectSet, Func<T, bool> filter) where T : SchemaObjectBase
		{
			foreach (var obj in objectSet)
			{
				if (obj.Status == SchemaObjectStatus.Normal && obj is T)
				{
					T o = (T)obj;
					if (filter != null)
					{
						if (filter(o))
							yield return o;
					}
					else
					{
						yield return o;
					}
				}
			}
		}

		/// <summary>
		/// 过滤有效关系
		/// </summary>
		/// <param name="acls">关系的集合</param>
		///  <param name="filter">指定的过滤器Lambda表达式 或 <see langword="null"/> </param>
		/// <returns></returns>
		internal IEnumerable<SCRelationObject> FilterRelations(IEnumerable<SCRelationObject> relations, Func<SCRelationObject, bool> filter)
		{
			foreach (SCRelationObject r in relations)
			{
				if (r.Status == SchemaObjectStatus.Normal)
				{
					if (filter != null)
					{
						if (filter(r))
							yield return r;
					}
					else
					{
						yield return r;
					}
				}
			}
		}

		/// <summary>
		/// 过滤有效的成员关系
		/// </summary>
		/// <param name="conditions">关系的集合</param>
		/// <param name="filter">指定的过滤器Lambda表达式 或 <see langword="null"/> </param>
		/// <returns></returns>
		internal IEnumerable<SCSimpleRelationBase> FilterMembership(IEnumerable<SCSimpleRelationBase> members, Func<SCSimpleRelationBase, bool> filter)
		{
			foreach (SCSimpleRelationBase item in members)
			{
				if (item.Status == SchemaObjectStatus.Normal)
				{
					if (filter != null)
					{
						if (filter(item))
						{
							yield return item;
						}
					}
					else
					{
						yield return item;
					}
				}
			}
		}

		internal IEnumerable<PC.Conditions.SCCondition> FilterConditions(IEnumerable<PC.Conditions.SCCondition> conditions, Func<PC.Conditions.SCCondition, bool> filter)
		{
			foreach (PC.Conditions.SCCondition item in conditions)
			{
				if (item.Status == SchemaObjectStatus.Normal)
				{
					if (filter != null)
					{
						if (filter(item))
						{
							yield return item;
						}
					}
					else
					{
						yield return item;
					}
				}
			}
		}

		/// <summary>
		/// 过滤ACL
		/// </summary>
		/// <param name="acls">acl的集合</param>
		///  <param name="filter">指定的过滤器Lambda表达式 或 <see langword="null"/> </param>
		/// <returns></returns>
		internal IEnumerable<PC.Permissions.SCAclItem> FilterAcls(IEnumerable<PC.Permissions.SCAclItem> acls, Func<PC.Permissions.SCAclItem, bool> filter)
		{
			foreach (PC.Permissions.SCAclItem acl in acls)
			{
				if (acl.Status == SchemaObjectStatus.Normal)
				{
					if (filter != null)
					{
						if (filter(acl))
							yield return acl;
					}
					else
					{
						yield return acl;
					}
				}
			}
		}

		/// <summary>
		/// 载入指定的对象，对其进行操作
		/// </summary>
		/// <typeparam name="T"><see cref="SchemaObjectBase"/>类型</typeparam>
		/// <param name="objectID">对象的ID</param>
		/// <param name="success">成功时的操作</param>
		/// <param name="fail">未找到对象时的操作 或为 <see langword="null"/>。</param>
		internal void WithEffectObject<T>(string objectID, System.Action<T> success, System.Action fail) where T : SchemaObjectBase
		{
			var obj = (T)PC.Adapters.SchemaObjectAdapter.Instance.Load(objectID);

			if (success == null)
				throw new ArgumentNullException("success");

			if (obj != null)
			{
				success(obj);
			}
			else
			{
				if (fail != null)
					fail();
			}
		}

		/// <summary>
		/// 载入指定的对象，对其进行操作
		/// </summary>
		/// <typeparam name="T"><see cref="SchemaObjectBase"/>类型</typeparam>
		/// <param name="objectID">对象的ID</param>
		/// <param name="success">成功时的操作</param>
		/// <param name="fail">未找到对象时的操作 或为 <see langword="null"/>。</param>
		internal void WithEffectObject<T>(string objectID, IDictionary<string, SchemaObjectBase> knownObjects, System.Action<T> success, System.Action fail) where T : SchemaObjectBase
		{
			if (success == null)
				throw new ArgumentNullException("success");

			T obj = null;
			if (knownObjects != null)
			{
				if (knownObjects.ContainsKey(objectID))
					obj = (T)knownObjects[objectID];
				else
				{
					obj = (T)PC.Adapters.SchemaObjectAdapter.Instance.Load(objectID);
					if (obj != null && obj.Status == SchemaObjectStatus.Normal)
					{
						knownObjects.Add(obj.ID, obj);
					}
				}
			}
			else
			{
				obj = (T)PC.Adapters.SchemaObjectAdapter.Instance.Load(objectID);
			}

			if (obj != null && obj.Status == SchemaObjectStatus.Normal)
			{
				success(obj);
			}
			else
			{
				if (fail != null)
					fail();
			}
		}

		/// <summary>
		/// 检查指定的关系是否存在
		/// </summary>
		/// <param name="parentID">父级ID</param>
		/// <param name="childID">子级ID</param>
		/// <param name="create">不存在时创建的操作</param>
		/// <param name="exists">存在时的操作 或为 <see langword="null"/></param>
		internal void CheckRelation(string parentID, string childID, Action create, Action exists)
		{
			create.NullCheck("create");
			parentID.NullCheck("parentID");
			childID.NullCheck("childID");

			var relation = PC.Adapters.SCMemberRelationAdapter.Instance.Load(parentID, childID);

			if (relation == null || relation.Status != SchemaObjectStatus.Normal)
			{
				create();
			}
			else
			{
				if (exists != null)
					exists();
			}
		}

		/// <summary>
		/// 检查指定的成员关系是否存在
		/// </summary>
		/// <param name="parentID">父级ID</param>
		/// <param name="childID">子级ID</param>
		/// <param name="create">不存在时创建的操作</param>
		/// <param name="exists">存在时的操作 或为 <see langword="null"/></param>
		internal void CheckMembership(string containerID, string memberID, Action create, Action exists)
		{
			create.NullCheck("create");
			containerID.NullCheck("containerID");
			memberID.NullCheck("memberID");

			var relation = PC.Adapters.SCMemberRelationAdapter.Instance.Load(containerID, memberID);

			if (relation == null || relation.Status != SchemaObjectStatus.Normal)
			{
				create();
			}
			else
			{
				if (exists != null)
					exists();
			}
		}

		internal void ImportUsers(List<SchemaObjectBase> pendingUsers, SCObjectSet objectSet, IImportContext context, bool includeOrganizations)
		{
			int count = 0;
			int allCount = pendingUsers.Count;

			foreach (SCUser item in pendingUsers)
			{
				count++;
				try
				{
					var summaryName = item.ToDescription();
					context.SetStatus(count, allCount, "正在导入项目:" + summaryName);

					if (includeOrganizations && objectSet.HasRelations)
					{
						var user = (SCUser)item;
						bool anyOrg = false;
						foreach (var relation in ImportService.Instance.FilterRelations(objectSet.Relations, r => { return r.ChildSchemaType == user.SchemaType && r.ParentSchemaType == "Organizations" && r.ID == user.ID; }))
						{
							try
							{
								ImportService.Instance.WithEffectObject<SCOrganization>(relation.ParentID, (org) =>
								{
									anyOrg = true;

									context.AppendLogFormat("正在添加{0}并设置为组织{1}的成员\r\n", summaryName, org.ToDescription());
									Actor.AddUser(user, org);
								}, () =>
								{
									// 未找到组织
								});
							}
							catch (Exception ex)
							{
								context.AppendLogFormat("发生了错误" + ex.ToString() + "\r\n");
							}
						}

						if (anyOrg == false)
						{
							context.AppendLogFormat("未找到人员{0}的任何组织,将仅添加用户。\r\n", summaryName);
							this.Actor.AddUser(user, null);
						}
					}
					else
					{
						this.Actor.AddUser((SCUser)item, null);
					}

					context.AppendLog("已执行导入项目" + summaryName);
				}
				catch (Exception ex)
				{
					context.AppendLog("对项的操作失败，原因是：" + ex.Message);
				}
			}
		}

		internal void ImportSecretaries(List<SchemaObjectBase> pendingUsers, SCObjectSet objectSet, IImportContext context)
		{
			int count = 0;
			int allCount = pendingUsers.Count;

			context.AppendLog("正在查找秘书");

			foreach (SCUser user in pendingUsers)
			{
				count++;

				context.SetStatus(count, allCount, "正在查找用户" + user.DisplayName + "的秘书");
				foreach (var s in ImportService.Instance.FilterMembership(objectSet.Membership, m => m.ContainerID == user.ID && m.ContainerSchemaType == user.SchemaType))
				{
					try
					{
						ImportService.Instance.WithEffectObject<SCUser>(s.ID, (secretary) =>
						{
							context.AppendLogFormat("正在替用户{0}添加秘书{1}\r\n", user.DisplayName, secretary.DisplayName);
							Actor.AddSecretaryToUser(secretary, user);
						}, null);
					}
					catch (Exception ex)
					{
						context.AppendLog("添加秘书时出现错误:" + ex.Message);
					}
				}
			}
		}

		internal void ImportBosses(List<SchemaObjectBase> pendingUsers, SCObjectSet objectSet, IImportContext context)
		{
			int count = 0;
			int allCount = pendingUsers.Count;

			context.AppendLog("正在查找上司");
			foreach (SCUser user in pendingUsers)
			{
				count++;

				context.SetStatus(count, allCount, "正在查找用户" + user.DisplayName + "的上司");
				foreach (var s in ImportService.Instance.FilterMembership(objectSet.Membership, m => m.ID == user.ID && m.ContainerSchemaType == user.SchemaType))
				{
					try
					{
						ImportService.Instance.WithEffectObject<SCUser>(s.ContainerID, secretary =>
						{
							context.AppendLogFormat("正在替用户{0}添加上司{1}\r\n", user.DisplayName, secretary.DisplayName);
							Actor.AddSecretaryToUser(user, secretary);
						}, null);
					}
					catch (Exception ex)
					{
						context.AppendLog("添加上司时出现错误:" + ex.Message);
					}
				}
			}
		}

		internal void AddUsersToGroups(List<SchemaObjectBase> pendingUsers, SCObjectSet objectSet, IImportContext context)
		{
			int count = 0;
			int allCount = pendingUsers.Count;
			context.AppendLog("正在查找群组");

			foreach (SCUser user in pendingUsers)
			{
				count++;

				context.SetStatus(count, allCount, "正在查找用户" + user.DisplayName + "的群组");
				foreach (var s in ImportService.Instance.FilterMembership(objectSet.Membership, m => m.ID == user.ID && m.ContainerSchemaType == "Groups"))
				{
					try
					{
						ImportService.Instance.WithEffectObject<SCGroup>(s.ContainerID, grp =>
						{
							ImportService.Instance.CheckRelation(grp.ID, user.ID, () =>
							{
								context.AppendLogFormat("正在将用户{0}添加到群组{1}\r\n", user.ToDescription(), grp.ToDescription());
								Actor.AddUserToGroup(user, grp);
							}, () =>
							{
								context.AppendLogFormat("群组{0}已经存在用户{1}，已跳过。\r\n", grp.ToDescription(), user.ToDescription());
							});
						}, null);
					}
					catch (Exception ex)
					{
						context.AppendLog("添加到群组时出现错误:" + ex.Message);
					}
				}
			}
		}
	}
}