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
		// 递归应用
		private abstract class HierarchicalAction
		{
			private OguFullImportAction owner;

			public OguFullImportAction Owner
			{
				get { return this.owner; }
			}

			public HierarchicalAction(OguFullImportAction owner)
			{
				this.owner = owner;
			}

			/// <summary>
			/// 在开始爬组织之前调用一次
			/// </summary>
			public virtual void ExecutePreOperation(SCObjectSet objectSet, IImportContext context, IDictionary<string, PC.SchemaObjectBase> knownObjects, PC.SCOrganization org, Dictionary<string, IList<PC.SCOrganization>> orgToOrgRelations, Dictionary<string, IList<PC.SCUser>> orgToUserRelations, Dictionary<string, IList<PC.SCGroup>> orgToGroupRelations)
			{
			}

			/// <summary>
			/// 在所有组织爬完之后调用一次
			/// </summary>
			public virtual void ExecutePostOperation(SCObjectSet objectSet, IImportContext context, IDictionary<string, PC.SchemaObjectBase> knownObjects, PC.SCOrganization org, Dictionary<string, IList<PC.SCOrganization>> orgToOrgRelations, Dictionary<string, IList<PC.SCUser>> orgToUserRelations, Dictionary<string, IList<PC.SCGroup>> orgToGroupRelations)
			{
			}

			/// <summary>
			/// 在递归的每个组织中调用一次
			/// </summary>
			/// <param name="objectSet"></param>
			/// <param name="context"></param>
			/// <param name="knownObjects"></param>
			/// <param name="relation"></param>
			/// <param name="orgToOrgRelations"></param>
			/// <param name="orgToUserRelations"></param>
			/// <param name="orgToGroupRelations"></param>
			public abstract void ExecuteEachOrganization(SCObjectSet objectSet, IImportContext context, IDictionary<string, PC.SchemaObjectBase> knownObjects, PC.SCOrganization org, Dictionary<string, IList<PC.SCOrganization>> orgToOrgRelations, Dictionary<string, IList<PC.SCUser>> orgToUserRelations, Dictionary<string, IList<PC.SCGroup>> orgToGroupRelations);
		}

		/// <summary>
		/// 执行ACL的操作
		/// </summary>
		private class AclAction : HierarchicalAction
		{
			public AclAction(OguFullImportAction owner)
				: base(owner)
			{
			}

			public override void ExecuteEachOrganization(SCObjectSet objectSet, IImportContext context, IDictionary<string, PC.SchemaObjectBase> knownObjects, PC.SCOrganization org, Dictionary<string, IList<PC.SCOrganization>> orgToOrgRelations, Dictionary<string, IList<PC.SCUser>> orgToUserRelations, Dictionary<string, IList<PC.SCGroup>> orgToGroupRelations)
			{
				if (objectSet.HasAcls)
				{
					context.SetStatus(Owner.currentSteps, Owner.allSteps, string.Format("正在寻找 {0} 的ACL。", org.ToDescription()));

					var allAcls = ImportService.Instance.FilterAcls(objectSet.Acls, acl => acl.ContainerID == org.ID && acl.Status == SchemaObjectStatus.Normal).ToList();

					var summaryName = org.ToDescription();

					try
					{
						var newContainer = new PC.Permissions.SCAclContainer(org);

						foreach (var acl in allAcls)
						{
							ImportService.Instance.WithEffectObject<PC.SchemaObjectBase>(acl.MemberID, knownObjects, role =>
							{
								newContainer.Members.Add(acl.ContainerPermission, role);
							}, null);
						}

						var oldMembers = PC.Adapters.SCAclAdapter.Instance.LoadByContainerID(org.ID, DateTime.MinValue);

						if (oldMembers != null)
						{
							newContainer.Members.MergeChangedItems(oldMembers);
						}

						context.SetStatusAndLog(Owner.currentSteps, Owner.allSteps, string.Format("正在替换 {0} 的ACL:", summaryName));

						PC.Adapters.SCAclAdapter.Instance.Update(newContainer);
					}
					catch (Exception ex)
					{
						context.AppendLogFormat("对象 {0} 的ACL操作失败，原因是：{1}\r\n", summaryName, ex.Message);
					}
				}
			}
		}

		private class UserAction : HierarchicalAction
		{
			private Dictionary<string, PC.SCUser> importedUsers = new Dictionary<string, PC.SCUser>();
			private Dictionary<string, string> importedSecretaries = new Dictionary<string, string>(); // 上司，秘书表

			public UserAction(OguFullImportAction owner)
				: base(owner)
			{
			}

			public bool ImportSecretaries { get; set; }

			public override void ExecutePostOperation(SCObjectSet objectSet, IImportContext context, IDictionary<string, PC.SchemaObjectBase> knownObjects, PC.SCOrganization org, Dictionary<string, IList<PC.SCOrganization>> orgToOrgRelations, Dictionary<string, IList<PC.SCUser>> orgToUserRelations, Dictionary<string, IList<PC.SCGroup>> orgToGroupRelations)
			{
				if (this.ImportSecretaries)
				{
					var allUsers = this.importedUsers.Values.ToArray();
					var reducedMemberships = ImportService.Instance.FilterMembership(objectSet.Membership, m => Util.IsUser(m.MemberSchemaType) && Util.IsUser(m.ContainerSchemaType)).ToList();

					if (reducedMemberships.Count > 0)
					{
						int count = reducedMemberships.Count;
						int step = 0;
						context.SetStatus(Owner.currentSteps, Owner.allSteps, "正在查找人员秘书");

						foreach (var r in reducedMemberships)
						{
							step++;
							try
							{
								if (this.importedUsers.ContainsKey(r.ContainerID))
								{
									var boss = this.importedUsers[r.ContainerID];

									// 导入的用户是上司的
									ImportService.Instance.WithEffectObject<PC.SCUser>(r.ID, knownObjects, secretary =>
									{
										context.SetSubStatusAndLog(step, count, string.Format("正在替 {0} 添加秘书 {1}", boss.Name, secretary.Name));
										PC.Executors.SCObjectOperations.Instance.AddSecretaryToUser(secretary, boss);
									}, null);
								}
								else if (this.importedUsers.ContainsKey(r.ID))
								{
									var secretary = this.importedUsers[r.ID];

									// 作为秘书的
									ImportService.Instance.WithEffectObject<PC.SCUser>(r.ContainerID, knownObjects, boss =>
									{
										context.SetSubStatusAndLog(step, count, string.Format("正在替 {0} 添加秘书 {1}", boss.Name, secretary.Name));
										PC.Executors.SCObjectOperations.Instance.AddSecretaryToUser(secretary, boss);
									}, null);
								}
							}
							catch (Exception ex)
							{
								context.SetSubStatusAndLog(step, count, string.Format("未能完成添加秘书操作：{0}", ex.ToString()));
							}
						}
					}
				}
			}

			public override void ExecuteEachOrganization(SCObjectSet objectSet, IImportContext context, IDictionary<string, PC.SchemaObjectBase> knownObjects, PC.SCOrganization org, Dictionary<string, IList<PC.SCOrganization>> orgToOrgRelations, Dictionary<string, IList<PC.SCUser>> orgToUserRelations, Dictionary<string, IList<PC.SCGroup>> orgToGroupRelations)
			{
				if (orgToUserRelations.ContainsKey(org.ID))
				{
					var users = orgToUserRelations[org.ID];

					if (users.Count > 0)
					{
						context.SetStatus(Owner.currentSteps, Owner.allSteps, string.Format("正在查找 {0} 中的用户", org.ToDescription()));
					}

					for (int i = 0; i < users.Count; i++)
					{
						var user = users[i];
						try
						{
							if (this.importedUsers.ContainsKey(user.ID) == false)
							{
								context.SetSubStatusAndLog(i + 1, users.Count, string.Format("正在向组织 {0} 导入用户 {1}", org.Name, user.Name));

								PC.Executors.SCObjectOperations.Instance.AddUser(user, org);
								this.importedUsers.Add(user.ID, user);

								if (knownObjects.ContainsKey(user.ID) == false)
									knownObjects.Add(user.ID, user);
							}
							else
							{
								context.SetSubStatusAndLog(i + 1, users.Count, string.Format("正在向组织 {0} 添加用户 {1}", org.Name, user.Name));
								PC.Executors.SCObjectOperations.Instance.AddUserToOrganization(user, org);

								if (knownObjects.ContainsKey(user.ID) == false)
									knownObjects.Add(user.ID, user);
							}
						}
						catch (Exception ex)
						{
							context.AppendLog(string.Format("向组织 {0} 导入用户 {1} 时出错：", org.Name, user.Name, ex.ToString()));
						}
					}
				}
			}
		}

		private class GroupAction : HierarchicalAction
		{
			public bool ImportMembers { get; set; }

			public bool ImportConditions { get; set; }

			public GroupAction(OguFullImportAction owner)
				: base(owner)
			{
			}

			public override void ExecuteEachOrganization(SCObjectSet objectSet, IImportContext context, IDictionary<string, PC.SchemaObjectBase> knownObjects, PC.SCOrganization org, Dictionary<string, IList<PC.SCOrganization>> orgToOrgRelations, Dictionary<string, IList<PC.SCUser>> orgToUserRelations, Dictionary<string, IList<PC.SCGroup>> orgToGroupRelations)
			{
				if (orgToGroupRelations.ContainsKey(org.ID))
				{
					var groups = orgToGroupRelations[org.ID];

					if (groups.Count > 0)
					{
						context.SetStatus(Owner.currentSteps, Owner.allSteps, string.Format("正在查找 {0} 中的群组", org.ToDescription()));
					}

					for (int i = 0; i < groups.Count; i++)
					{
						var group = groups[i];
						try
						{
							context.SetSubStatusAndLog(i + 1, groups.Count, string.Format("正在向组织 {0} 添加群组 {1}", org.Name, group.Name));

							PC.Executors.SCObjectOperations.Instance.AddGroup(group, org);

							knownObjects.Add(group.ID, group);

							this.DoImportMembers(objectSet, context, knownObjects, group);

							this.DoImportConditions(objectSet, context, group);
						}
						catch (Exception ex)
						{
							context.AppendLog(string.Format("向组织 {0} 导入群组 {1} 时出错：", org.ToDescription(), group.Name, ex.ToString()));
						}
					}
				}
			}

			private void DoImportConditions(SCObjectSet objectSet, IImportContext context, PC.SCGroup group)
			{
				if (this.ImportConditions && objectSet.HasConditions)
				{
					context.SetStatusAndLog(Owner.currentSteps, Owner.allSteps, string.Format("正在替换群组 {0} 的条件", group.Name));

					try
					{
						var owner = PC.Adapters.SCConditionAdapter.Instance.Load(group.ID, "Default") ?? new PC.Conditions.SCConditionOwner() { OwnerID = group.ID, Type = "Default" };

						PC.Conditions.SCConditionCollection src = new PC.Conditions.SCConditionCollection();
						src.CopyFrom(ImportService.Instance.FilterConditions(objectSet.Conditions, c => c.OwnerID == group.ID));

						owner.Conditions.ReplaceItemsWith(src, group.ID, "Default");

						PC.Adapters.SCConditionAdapter.Instance.UpdateConditions(owner);
					}
					catch (Exception ex)
					{
						context.AppendLog(string.Format("替换群组 {0} 条件成员时出错:{1}", group.ToDescription(), ex.ToString()));
					}
				}
			}

			private void DoImportMembers(SCObjectSet objectSet, IImportContext context, IDictionary<string, PC.SchemaObjectBase> knownObjects, PC.SCGroup group)
			{
				if (this.ImportMembers && objectSet.HasMembership)
				{
					var members = ImportService.Instance.FilterMembership(objectSet.Membership, m => m.ContainerID == group.ID && m.Status == SchemaObjectStatus.Normal).ToArray();
					if (members.Length > 0)
					{
						context.SetStatus(Owner.currentSteps, Owner.allSteps, string.Format("正在查找群组 {0} 的固定成员", group.ToDescription()));

						for (int j = 0; j < members.Length; j++)
						{
							try
							{
								var gm = members[j];

								ImportService.Instance.WithEffectObject<PC.SCUser>(gm.ID, knownObjects, user =>
								{
									context.SetSubStatusAndLog(j, members.Length, string.Format("正在向群组 {0} 添加成员 {1} ", group.Name, user.Name));
									PC.Executors.SCObjectOperations.Instance.AddUserToGroup(user, group);
								}, null);
							}
							catch (Exception ex2)
							{
								context.SetSubStatusAndLog(j, members.Length, string.Format("向群组 {0} 添加成员时出错：", group.ToDescription(), ex2.ToString()));
							}
						}
					}
				}
			}
		}
	}
}