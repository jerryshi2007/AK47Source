using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter
{
	public partial class OguFullImportAction : OguImportAction
	{
		private int currentSteps = 0;
		private int allSteps = 0;

		private class Stat
		{
			public Stat()
			{
				this.OrgsCount = this.GroupsCount = this.UsersCount = 0;
			}

			public int OrgsCount { get; set; }

			public int GroupsCount { get; set; }

			public int UsersCount { get; set; }
		}

		public bool IncludeOrganizations { get; set; }

		public bool IncludeAcl { get; set; }

		public bool IncludeUser { get; set; }

		public bool IncludeSecretaries { get; set; }

		public bool IncludeGroup { get; set; }

		public bool IncludeGroupConditions { get; set; }

		public bool IncludeGroupMembers { get; set; }

		public OguFullImportAction(PC.SCOrganization parent)
			: base(parent)
		{
		}

		public override void DoImport(SCObjectSet objectSet, IImportContext context)
		{
			if (objectSet.HasRelations && objectSet.HasObjects)
			{
				context.SetStatus(0, 1, "正在分析数据。");

				// 查找组织关系
				var pendingOperations = new List<Action<object>>();

				var objects = objectSet.Objects;
				Dictionary<string, IList<PC.SCOrganization>> orgToOrgRelations = new Dictionary<string, IList<PC.SCOrganization>>();
				Dictionary<string, IList<PC.SCUser>> orgToUserRelations = new Dictionary<string, IList<PC.SCUser>>();
				Dictionary<string, IList<PC.SCGroup>> orgToGroupRelations = new Dictionary<string, IList<PC.SCGroup>>();
				Dictionary<string, PC.SchemaObjectBase> knownObjects = new Dictionary<string, PC.SchemaObjectBase>(); // 缓存已知对象，避免多次往返

				context.SetStatus(0, 1, "正在统计需要导入的对象");
				Stat stat = new Stat(); // 统计信息

				FindFullOURelations(objectSet, orgToOrgRelations, orgToUserRelations, orgToGroupRelations, new PC.SCOrganization[] { this.Parent }, stat); // 爬出所有组织关系

				Dictionary<PC.SCOrganization, IList<PC.SCRelationObject>> userToOrgRelations = new Dictionary<PC.SCOrganization, IList<PC.SCRelationObject>>();

				this.allSteps = this.CalculateSteps(stat);
				this.currentSteps = 0;
				bool orgValid = false; // 必须校验组织
				context.SetStatus(0, this.allSteps, "正在导入数据。");

				// 递归导入组织,并剔除错误的数据
				orgValid = this.PrepareOrganizations(objectSet, context, knownObjects, orgToOrgRelations, this.Parent, this.IncludeOrganizations == false);

				if (this.IncludeAcl)
				{
					// 递归导入Acl
					var action = new AclAction(this);
					action.ExecutePreOperation(objectSet, context, knownObjects, this.Parent, orgToOrgRelations, orgToUserRelations, orgToGroupRelations);
					this.DoHierarchicalAction(objectSet, context, knownObjects, orgToOrgRelations, orgToUserRelations, orgToGroupRelations, this.Parent, action);
					action.ExecutePostOperation(objectSet, context, knownObjects, this.Parent, orgToOrgRelations, orgToUserRelations, orgToGroupRelations);
				}

				if (this.IncludeUser)
				{
					var action = new UserAction(this);
					action.ImportSecretaries = this.IncludeSecretaries;
					action.ExecutePreOperation(objectSet, context, knownObjects, this.Parent, orgToOrgRelations, orgToUserRelations, orgToGroupRelations);
					this.DoHierarchicalAction(objectSet, context, knownObjects, orgToOrgRelations, orgToUserRelations, orgToGroupRelations, this.Parent, action);
					action.ExecutePostOperation(objectSet, context, knownObjects, this.Parent, orgToOrgRelations, orgToUserRelations, orgToGroupRelations);
				}

				if (this.IncludeGroup)
				{
					var action = new GroupAction(this);
					action.ImportMembers = this.IncludeGroupMembers;
					action.ImportConditions = this.IncludeGroupConditions;
					action.ExecutePreOperation(objectSet, context, knownObjects, this.Parent, orgToOrgRelations, orgToUserRelations, orgToGroupRelations);
					this.DoHierarchicalAction(objectSet, context, knownObjects, orgToOrgRelations, orgToUserRelations, orgToGroupRelations, this.Parent, action);
					action.ExecutePostOperation(objectSet, context, knownObjects, this.Parent, orgToOrgRelations, orgToUserRelations, orgToGroupRelations);
				}
			}
		}

		private void DoHierarchicalAction(SCObjectSet objectSet, IImportContext context, IDictionary<string, PC.SchemaObjectBase> knownObjects, Dictionary<string, IList<PC.SCOrganization>> orgToOrgRelations, Dictionary<string, IList<PC.SCUser>> orgToUserRelations, Dictionary<string, IList<PC.SCGroup>> orgToGroupRelations, PC.SCOrganization currentParent, HierarchicalAction actionAdapter)
		{
			// 爬树
			PC.SchemaObjectCollection objects = objectSet.Objects;

			if (orgToOrgRelations.ContainsKey(currentParent.ID))
			{
				var childList = orgToOrgRelations[currentParent.ID];

				for (int i = 0; i < childList.Count; i++)
				{
					this.currentSteps++;

					var org = childList[i];

					System.Diagnostics.Debug.WriteLine("正在处理组织" + org.Name);

					actionAdapter.ExecuteEachOrganization(objectSet, context, knownObjects, org, orgToOrgRelations, orgToUserRelations, orgToGroupRelations);

					this.DoHierarchicalAction(objectSet, context, knownObjects, orgToOrgRelations, orgToUserRelations, orgToGroupRelations, org, actionAdapter);
				}
			}
		}

		/// <summary>
		/// 准备组织
		/// </summary>
		/// <param name="objectSet"></param>
		/// <param name="context"></param>
		/// <param name="knownObjects">向其中写入已经确认存在于数据库中的项</param>
		/// <param name="orgToOrgRelations">组织关系</param>
		/// <param name="allSteps"></param>
		/// <param name="currentSteps"></param>
		/// <param name="currentParent"></param>
		/// <param name="checkOnly"><see langword="true"/>表示只检查组织关系，不实际导入。</param>
		/// <returns></returns>
		private bool PrepareOrganizations(SCObjectSet objectSet, IImportContext context, IDictionary<string, PC.SchemaObjectBase> knownObjects, Dictionary<string, IList<PC.SCOrganization>> orgToOrgRelations, PC.SCOrganization currentParent, bool checkOnly)
		{
			bool valid = true;

			PC.SchemaObjectCollection objects = objectSet.Objects;

			if (orgToOrgRelations.ContainsKey(currentParent.ID))
			{
				var childList = orgToOrgRelations[currentParent.ID];

				for (int i = 0; i < childList.Count; i++)
				{
					this.currentSteps++;

					var org = childList[i];

					try
					{
						// 导入该组织
						if (checkOnly == false)
						{
							var msg = string.Format("正在导入组织{0}", org.ToDescription());
							context.AppendLog(msg);
							context.SetStatus(this.currentSteps, this.allSteps, msg);
							PC.Executors.SCObjectOperations.Instance.AddOrganization(org, currentParent);

							if (knownObjects.ContainsKey(org.ID) == false)
								knownObjects.Add(org.ID, org);
						}
						else
						{
							context.SetStatus(this.currentSteps, this.allSteps, string.Format("正在校验组织 {0} ", org.ToDescription()));

							if (Util.IsNullOrDeleted(PC.Adapters.SchemaObjectAdapter.Instance.Load(org.ID)))
							{
								context.AppendLog(string.Format("组织 {0} 不存在，导入时将排除此组织及子对象", org.ToDescription()));
								valid = false;
								childList.RemoveAt(i);
								i--;
								continue;
							}
							else if (Util.IsNullOrDeleted(PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(currentParent.ID, org.ID)))
							{
								context.AppendLog(string.Format("组织 {0} 已存在，但已被移至其他位置，导入时将排除此组织及子对象。", org.ToDescription()));
								valid = false;
								childList.RemoveAt(i);
								i--;
								continue;
							}
							else
							{
								if (knownObjects.ContainsKey(org.ID) == false)
									knownObjects.Add(org.ID, org);
							}
						}

						valid &= this.PrepareOrganizations(objectSet, context, knownObjects, orgToOrgRelations, org, checkOnly);
					}
					catch (Exception ex)
					{
						context.AppendLog(string.Format("导入组织 {0} 时出现错误，已跳过了该组织及子对象：{1}", org.ToDescription(), ex.ToString()));
						valid = false;
						childList.RemoveAt(i);
						i--;
					}
				}
			}

			return valid;
		}

		private int CalculateSteps(Stat stat)
		{
			int allSteps = 0;

			allSteps += stat.OrgsCount;

			if (this.IncludeUser)
				allSteps += stat.OrgsCount;

			if (this.IncludeAcl)
				allSteps += stat.OrgsCount;

			if (this.IncludeGroup)
				allSteps += stat.OrgsCount;

			return allSteps;
		}

		/// <summary>
		/// 获取所有组织节点表
		/// </summary>
		/// <param name="objectSet"></param>
		/// <param name="childOrgs">向其中写入组织的子组织</param>
		/// <param name="childUsers">向其中写入组织的子用户</param>
		/// <param name="childGroups">向其中写入组织的子群组</param>
		/// <param name="parents"></param>
		/// <param name="stat"></param>
		private static void FindFullOURelations(SCObjectSet objectSet, Dictionary<string, IList<PC.SCOrganization>> childOrgs, Dictionary<string, IList<PC.SCUser>> childUsers, Dictionary<string, IList<PC.SCGroup>> childGroups, IEnumerable<PC.SCOrganization> parents, Stat stat)
		{
			var objects = objectSet.Objects;

			foreach (var item in parents)
			{
				List<PC.SCOrganization> orgs = new List<PC.SCOrganization>();
				List<PC.SCUser> users = new List<PC.SCUser>();
				List<PC.SCGroup> groups = new List<PC.SCGroup>();
				foreach (var r in objectSet.Relations.FindAll(r => r.ParentID == item.ID && r.Status == SchemaObjectStatus.Normal).OrderBy(r => r.InnerSort))
				{
					if (Util.IsOrganization(r.ChildSchemaType))
					{
						if (objects.ContainsKey(r.ID))
						{
							orgs.Add((PC.SCOrganization)objects[r.ID]);
							stat.OrgsCount++;
						}
					}
					else if (Util.IsGroup(r.ChildSchemaType))
					{
						if (objects.ContainsKey(r.ID))
						{
							groups.Add((PC.SCGroup)objects[r.ID]);
							stat.GroupsCount++;
						}
					}
					else if (Util.IsUser(r.ChildSchemaType))
					{
						if (objects.ContainsKey(r.ID))
						{
							users.Add((PC.SCUser)objects[r.ID]);
							stat.UsersCount++;
						}
					}
				}

				childOrgs.Add(item.ID, orgs);
				childGroups.Add(item.ID, groups);
				childUsers.Add(item.ID, users);

				FindFullOURelations(objectSet, childOrgs, childUsers, childGroups, orgs, stat);
			}
		}
	}
}