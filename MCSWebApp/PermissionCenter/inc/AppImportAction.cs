using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter
{
	public class AppImportAction : ImportAction
	{
		#region 属性

		/// <summary>
		/// 获取或设置是否导入Acl
		/// </summary>
		public bool IncludeAcls { get; set; }

		/// <summary>
		/// 获取或设置是否导入角色
		/// </summary>
		public bool IncludeRoles { get; set; }

		/// <summary>
		/// 获取或设置是否导入功能
		/// </summary>
		public bool IncludePermissions { get; set; }

		/// <summary>
		/// 获取或设置是否导入角色功能定义
		/// </summary>
		public bool IncludeRoleDefinitions { get; set; }

		/// <summary>
		/// 获取或设置是否导入角色成员
		/// </summary>
		public bool IncludeRoleMembers { get; set; }

		/// <summary>
		/// 获取或设置是否导入角色条件
		/// </summary>
		public bool IncludeRoleConditions { get; set; }

		/// <summary>
		/// 获取或设置是否使用复制模式
		/// </summary>
		/// <value>如果是<see langword="true"/>，将采取复制的行为，如果是<see langword="false"/>将采用合并的行为。</value>
		public bool CopyMode { get; set; }
		#endregion

		public override void DoImport(SCObjectSet objectSet, IImportContext context)
		{
			var executor = PC.Executors.SCObjectOperations.InstanceWithPermissions;

			var apps = ImportService.Instance.FilterNormalObjectsBySchemaCategories(objectSet.Objects, "Applications").ToList();

			int allCount = apps.Count;
			int step = 0;
			context.AppendLog("发现" + allCount + "个应用");
			Dictionary<object, object> mappings = new Dictionary<object, object>(); // 保存新旧对象的对应关系

			// 统计需要哪些附加动作
			int extStepCount = 0;
			if (this.IncludePermissions)
				extStepCount++;
			if (this.IncludeRoles)
				extStepCount++;
			if (this.IncludeRoleMembers)
				extStepCount++;
			if (this.IncludeRoleDefinitions)
				extStepCount++;
			if (this.IncludeRoleConditions)
				extStepCount++;
			if (this.IncludeAcls)
				extStepCount++;

			foreach (PC.SCApplication app in apps)
			{
				this.ImportApplication(objectSet, context, executor, allCount, step++, extStepCount, mappings, app);
			}
		}

		#region 帮助方法
		internal static PC.SCRole MakeCopy(PC.SCRole obj)
		{
			return new PC.SCRole()
			{
				Creator = MCS.Library.Principal.DeluxeIdentity.CurrentRealUser,
				ID = UuidHelper.NewUuidString(),
				Name = obj.Name + "copy",
				DisplayName = obj.DisplayName + "copy",
				CodeName = Util.MakeNoConflictCodeName(obj.CodeName, "Roles")
			};
		}

		internal static PC.SCPermission MakeCopy(PC.SCPermission obj)
		{
			return new PC.SCPermission()
			{
				Creator = MCS.Library.Principal.DeluxeIdentity.CurrentRealUser,
				ID = UuidHelper.NewUuidString(),
				Name = obj.Name + "copy",
				DisplayName = obj.DisplayName + "copy",
				CodeName = Util.MakeNoConflictCodeName(obj.CodeName, "Permissions")
			};
		}

		internal static PC.SCApplication MakeCopy(PC.SCApplication app)
		{
			return new PC.SCApplication()
			{
				Creator = MCS.Library.Principal.DeluxeIdentity.CurrentRealUser,
				ID = UuidHelper.NewUuidString(),
				Name = app.Name + "copy",
				DisplayName = app.DisplayName + "copy",
				CodeName = Util.MakeNoConflictCodeName(app.CodeName, "Applications")
			};
		}

		#endregion

		private void ImportApplication(SCObjectSet objectSet, IImportContext context, PC.Executors.ISCObjectOperations executor, int allCount, int currentStep, int extStepCount, Dictionary<object, object> mappings, PC.SCApplication app)
		{
			var app2 = this.CopyMode ? AppImportAction.MakeCopy(app) : app;
			var summaryName = app2.DisplayName ?? app.Name;
			context.SetStatus(currentStep, allCount, "正在导入项目:" + summaryName);
			context.AppendLog("正在导入应用" + summaryName);
			executor.AddApplication((PC.SCApplication)app2);

			var permissionRelation = from p in objectSet.Membership where p.ContainerID == app.ID && p.MemberSchemaType == "Permissions" orderby p.InnerSort ascending select p;
			var permissions = (from o in objectSet.Objects join p in permissionRelation on o.ID equals p.ID select (PC.SCPermission)o).ToArray();

			var roleRelations = from r in objectSet.Membership where r.ContainerID == app.ID && r.MemberSchemaType == "Roles" orderby r.InnerSort ascending select r;
			var roles = (from o in objectSet.Objects join r in roleRelations on o.ID equals r.ID select (PC.SCRole)o).ToArray();

			var acls = (from acl in objectSet.Acls where acl.Status == SchemaObjectStatus.Normal && acl.ContainerID == app.ID orderby acl.SortID ascending select acl).ToArray();

			int allStepCount = allCount * (extStepCount + 1);
			int step = currentStep * (extStepCount + 1);

			if (this.IncludePermissions)
			{
				context.SetStatus(step++, allStepCount, "正在查找功能...");
				this.ImportPermissions(context, executor, mappings, app2, permissions, this.CopyMode);
			}

			if (this.IncludeRoles)
			{
				context.SetStatus(step++, allStepCount, "正在查找角色...");
				this.ImportRoles(context, executor, mappings, app2, roles);
			}

			if (this.IncludeAcls)
			{
				context.SetStatus(step++, allStepCount, "正在查找Acl...");
				this.ImportAcl(context, executor, app2, acls);
			}

			if (this.IncludeRoleMembers)
			{
				context.SetStatus(step++, allStepCount, "正在查找角色成员...");
				this.ImportRoleMembers(objectSet, context, executor, mappings, roles);
			}

			if (this.IncludeRoleConditions)
			{
				context.SetStatus(step++, allStepCount, "正在查找角色条件...");
				this.ImportRoleConditions(objectSet, context, executor, mappings, roles);
			}

			if (this.IncludeRoles && this.IncludePermissions && this.IncludeRoleDefinitions)
			{
				context.SetStatus(step++, allStepCount, "正在查找角色功能定义...");
				this.ImportRolePermissions(objectSet, context, executor, mappings, permissions, roles, this.CopyMode);
			}
		}

		private void ImportAcl(IImportContext context, PC.Executors.ISCObjectOperations executor, PC.SCApplication targetApp, PC.Permissions.SCAclItem[] acls)
		{
			if (acls.Length > 0)
			{
				var container = new PC.Permissions.SCAclContainer(targetApp);

				context.AppendLogFormat("正在替 {0} 合并ACL定义\r\n", targetApp.ToDescription());

				var oldPermissions = PC.Adapters.SCAclAdapter.Instance.LoadByContainerID(targetApp.ID, DateTime.MinValue);

				PC.Permissions.SCAclMemberCollection members = new PC.Permissions.SCAclMemberCollection();

				foreach (var item in acls)
				{
					ImportService.Instance.WithEffectObject<PC.SchemaObjectBase>(item.MemberID, role =>
					{
						members.Add(item.ContainerPermission, role);
					}, null);
				}

				if (oldPermissions != null && oldPermissions.Count > 0)
				{
					members.MergeChangedItems(oldPermissions);
				}

				container.Members.CopyFrom(members);

				PC.Adapters.SCAclAdapter.Instance.Update(container);
			}
		}

		private void ImportRolePermissions(SCObjectSet objectSet, IImportContext context, PC.Executors.ISCObjectOperations executor, Dictionary<object, object> mappings, PC.SCPermission[] permissions, PC.SCRole[] roles, bool copyMode)
		{
			if (roles.Length > 0 && permissions.Length > 0 && objectSet.HasRelations)
			{
				foreach (var r in roles)
				{
					PC.SCRole role2 = r;
					if (mappings.ContainsKey(r))
					{
						role2 = (PC.SCRole)mappings[r];
					}

					context.AppendLogFormat("正在替 {0} 查找功能定义\r\n", role2.ToDescription());

					foreach (var p in permissions)
					{
						PC.SCPermission permission2 = p;
						if (mappings.ContainsKey(p))
							permission2 = (PC.SCPermission)mappings[p];

						var actual = PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(role2.ID, permission2.ID);
						var actualHasPermission = actual != null && actual.Status == SchemaObjectStatus.Normal;

						var refHasPermission = (from rr in objectSet.Relations where rr.Status == SchemaObjectStatus.Normal && rr.ParentID == r.ID && rr.ID == p.ID && rr.ChildSchemaType == "Permissions" && rr.ParentSchemaType == "Roles" select rr).FirstOrDefault() != null;

						if (refHasPermission == false && actualHasPermission)
						{
							context.AppendLogFormat("正在替角色 {0} 解除功能 {1}\r\n", role2.ToDescription(), permission2.ToDescription());
							executor.DisjoinRoleAndPermission(role2, permission2);
						}
						else if (refHasPermission && actualHasPermission == false)
						{
							context.AppendLogFormat("正在替角色 {0} 指定功能 {1}\r\n", role2.ToDescription(), permission2.ToDescription());
							executor.JoinRoleAndPermission(role2, permission2);
						}
					}
				}
			}
		}

		private void ImportPermissions(IImportContext context, PC.Executors.ISCObjectOperations executor, Dictionary<object, object> mappings, PC.SCApplication app2, PC.SCPermission[] permissions, bool copyMode)
		{
			if (permissions.Length > 0)
			{
				context.AppendLog("正准备导入功能");
				foreach (var p in permissions)
				{
					var permission2 = copyMode ? MakeCopy(p) : p;
					mappings.Add(p, permission2);
					context.AppendLog("正在导入功能" + permission2.DisplayName ?? permission2.Name);
					executor.AddPermission(permission2, app2);
				}
			}
		}

		private void ImportRoles(IImportContext context, PC.Executors.ISCObjectOperations executor, Dictionary<object, object> mappings, PC.SCApplication app2, PC.SCRole[] roles)
		{
			if (this.IncludeRoles && roles.Length > 0)
			{
				context.AppendLog("正准备导入角色");
				foreach (var p in roles)
				{
					var role2 = this.CopyMode ? AppImportAction.MakeCopy(p) : p;
					mappings.Add(p, role2);
					context.AppendLog("正在导入角色" + role2.ToDescription());
					executor.AddRole(role2, app2);
				}
			}
		}

		private void ImportRoleMembers(SCObjectSet objectSet, IImportContext context, PC.Executors.ISCObjectOperations executor, Dictionary<object, object> mappings, PC.SCRole[] roles)
		{
			if (roles.Length > 0)
			{
				foreach (var r in roles)
				{
					PC.SCRole role2 = r;
					if (mappings.ContainsKey(r))
					{
						role2 = (PC.SCRole)mappings[r];
					}

					// 查找角色成员
					var roleMemberIds = (from m in objectSet.Membership where m.ContainerSchemaType == "Roles" && m.ContainerID == r.ID select m.ID).ToArray();
					if (roleMemberIds.Length > 0)
					{
						var roleMembers = DbUtil.LoadObjects(roleMemberIds);
						foreach (PC.SCBase obj in roleMembers)
						{
							context.AppendLog("正在导入角色成员" + obj.DisplayName ?? obj.Name);
							executor.AddMemberToRole(obj, role2);
						}
					}
				}
			}
		}

		private void ImportRoleConditions(SCObjectSet objectSet, IImportContext context, PC.Executors.ISCObjectOperations executor, Dictionary<object, object> mappings, PC.SCRole[] roles)
		{
			if (roles.Length > 0 && objectSet.HasConditions)
			{
				foreach (var r in roles)
				{
					PC.SCRole role2 = r;
					if (mappings.ContainsKey(r))
					{
						role2 = (PC.SCRole)mappings[r];
					}

					var roleConditions = (from c in objectSet.Conditions where c.OwnerID == r.ID select c).ToArray();

					if (roleConditions.Length > 0)
					{
						context.AppendLogFormat("正在替角色{0}添加条件\r\n", role2.ToDescription());
						PC.Conditions.SCConditionOwner owner = new PC.Conditions.SCConditionOwner()
						{
							OwnerID = role2.ID,
							Type = "Default"
						};

						foreach (var c in roleConditions)
						{
							owner.Conditions.Add(c);
						}

						PC.Adapters.SCConditionAdapter.Instance.UpdateConditions(owner);
					}
				}
			}
		}
	}
}