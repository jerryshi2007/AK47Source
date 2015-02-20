using System;
using System.Linq;
using System.Web;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Security;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace PermissionCenter
{
	/// <summary>
	/// 导入群组的操作
	/// </summary>
	public class AllGroupImportAction : ImportAction
	{
		/// <summary>
		/// 初始化<see cref="AllGroupImportAction"/>的新实例。
		/// </summary>
		public AllGroupImportAction()
		{
			this.IncludeConditions = true;
			this.IncludeMembers = true;
			this.IncludeSelf = true;
		}

		/// <summary>
		/// 获取或设置一个值，表示导入时是否包含条件
		/// </summary>
		public bool IncludeConditions { get; set; }

		/// <summary>
		/// 获取或设置一个值，表示导入时是否包含成员
		/// </summary>
		public bool IncludeMembers { get; set; }

		/// <summary>
		/// 获取或设置一个值，表示导入时是否包含群组
		/// </summary>
		public bool IncludeSelf { get; set; }

		/// <summary>
		/// 执行导入操作
		/// </summary>
		/// <param name="objectSet">所有数据</param>
		/// <param name="context">导入的执行上下文</param>
		public override void DoImport(SCObjectSet objectSet, IImportContext context)
		{
			var exec = PC.Executors.SCObjectOperations.InstanceWithPermissions;

			SchemaObjectCollection parentObjects = null; // 预先查出父级组织
			if (this.IncludeSelf && objectSet.HasObjects && objectSet.HasRelations)
			{
				parentObjects = this.GetParentObjects(objectSet.Relations);
			}

			SchemaObjectCollection memberObjects = null; // 预先查出成员
			if (this.IncludeMembers && objectSet.HasMembership)
			{
				memberObjects = this.GetMemberObjects(objectSet.Membership);
			}

			int allCount = objectSet.Objects.Count;
			int count = 0;
			foreach (var grp in objectSet.Objects)
			{
				count++;
				ImportOneGroup(objectSet, context, exec, parentObjects, memberObjects, allCount, count, grp);
			}
		}

		private void ImportOneGroup(SCObjectSet objectSet, IImportContext context, PC.Executors.ISCObjectOperations exec, SchemaObjectCollection parentObjects, SchemaObjectCollection memberObjects, int allCount, int count, SchemaObjectBase grp)
		{
			if (grp.SchemaType == "Groups")
			{
				try
				{
					var summaryName = grp.Properties.GetValue<string>("Name", "未命名");
					context.SetStatus(count, allCount, "正在导入对象：" + summaryName);

					if (this.IncludeSelf)
					{
						SCRelationObject parentOrgRelation = (SCRelationObject)objectSet.Relations.Find(m => ((SCRelationObject)m).ParentSchemaType == "Organizations" && ((SCRelationObject)m).ChildSchemaType == "Groups" && ((SCRelationObject)m).ID == grp.ID);
						if (parentOrgRelation == null)
							throw new HttpException("未找到群组的父级组织");

						var parentOrg = (SCOrganization)(parentObjects != null ? (from p in parentObjects where p.ID == parentOrgRelation.ParentID select p).FirstOrDefault() : null);
						if (parentOrg == null || parentOrg.Status != SchemaObjectStatus.Normal)
							throw new HttpException("群组的父级组织不存在或者已删除，未能导入群组。");

						exec.AddGroup((SCGroup)grp, parentOrg);

						context.AppendLog("已执行导入对象" + summaryName);
					}

					ImportMembers(objectSet, context, exec, memberObjects, grp);

					ImportConditions(objectSet, context, grp);
				}
				catch (Exception ex)
				{
					context.AppendLog("对项的操作失败，原因是：" + ex.Message);
				}
			}
			else
			{
				context.AppendLog("已跳过不是群组的项");
			}
		}

		private void ImportConditions(SCObjectSet objectSet, IImportContext context, SchemaObjectBase grp)
		{
			if (this.IncludeConditions && objectSet.HasConditions)
			{
				PC.Conditions.SCConditionOwner owner = new PC.Conditions.SCConditionOwner()
				{
					OwnerID = grp.ID,
					Type = "Default"
				};
				var conditions = from c in objectSet.Conditions where c.OwnerID == grp.ID && c.Type == "Default" select c;
				foreach (var c in conditions)
				{
					owner.Conditions.Add(c);
				}

				context.AppendLog("正在添加条件表达式");
				PC.Adapters.SCConditionAdapter.Instance.UpdateConditions(owner);
				context.AppendLog("添加条件表达式结束。");
			}
		}

		private void ImportMembers(SCObjectSet objectSet, IImportContext context, PC.Executors.ISCObjectOperations exec, SchemaObjectCollection memberObjects, SchemaObjectBase grp)
		{
			if (this.IncludeMembers && objectSet.HasMembership)
			{
				var memberRelations = objectSet.Membership.FindAll(m => m.ContainerID == grp.ID && m.Status == SchemaObjectStatus.Normal);
				if (memberRelations.Count > 0 && memberObjects != null)
				{
					context.AppendLogFormat("正在试图添加{0}个群组成员\r\n", memberRelations.Count);
					foreach (var r in memberRelations)
					{
						var user = (SCUser)(from m in memberObjects where m.ID == r.ID select m).FirstOrDefault();
						if (user != null)
						{
							try
							{
								exec.AddUserToGroup(user, (SCGroup)grp);
								context.AppendLogFormat("已经向群组{0}添加群组成员:{1}\r\n", ((SCGroup)grp).DisplayName, user.DisplayName);
							}
							catch (Exception ex)
							{
								context.AppendLogFormat("无法导入群组 {0} 的成员 {1}: {2}\r\n", ((SCGroup)grp).DisplayName, user.DisplayName, ex.Message);
							}
						}
						else
						{
							context.AppendLogFormat("已跳过不存在的成员\r\n");
						}
					}
				}
			}
		}

		/// <summary>
		/// 获取指定关系中的成员对象
		/// </summary>
		/// <param name="acls">关系</param>
		/// <returns></returns>
		private SchemaObjectCollection GetMemberObjects(SCMemberRelationCollection relations)
		{
			var memberId = (from p in relations where p.Status == SchemaObjectStatus.Normal select p.ID).ToArray();
			if (memberId.Length > 0)
			{
				InSqlClauseBuilder inSql = new InSqlClauseBuilder("ID");
				inSql.AppendItem(memberId);

				WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
				where.AppendItem("Status", (int)SchemaObjectStatus.Normal);

				return PC.Adapters.SchemaObjectAdapter.Instance.Load(new ConnectiveSqlClauseCollection(inSql, where, VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder()));
			}
			else
			{
				return new SchemaObjectCollection();
			}
		}

		/// <summary>
		/// 获取指定关系中的父级对象
		/// </summary>
		/// <param name="acls">关系</param>
		/// <returns></returns>
		private SchemaObjectCollection GetParentObjects(SCRelationObjectCollection relations)
		{
			var parentIds = (from p in relations where p.Status == SchemaObjectStatus.Normal select p.ParentID).ToArray();
			if (parentIds.Length > 0)
			{
				InSqlClauseBuilder inSql = new InSqlClauseBuilder("ID");
				inSql.AppendItem(parentIds);

				WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
				where.AppendItem("Status", (int)SchemaObjectStatus.Normal);

				return PC.Adapters.SchemaObjectAdapter.Instance.Load(new ConnectiveSqlClauseCollection(inSql, where, VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder()));
			}
			else
			{
				return new SchemaObjectCollection();
			}
		}
	}
}