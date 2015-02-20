using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter
{
	public class RoleImportAction : ImportAction
	{
		public RoleImportAction(string appId)
		{
			this.ApplicationId = appId;
		}

		public bool CopyMode { get; set; }

		public bool IncludeRoleDefinitions { get; set; }

		public string ApplicationId { get; set; }

		public bool IncludeConstMembers { get; set; }

		public bool IncludeConditions { get; set; }

		public override void DoImport(SCObjectSet objectSet, IImportContext context)
		{
			if (string.IsNullOrEmpty(this.ApplicationId))
				throw new HttpException("没有指定ApplicationId的情况下无法导入。");

			var app = (PC.SCApplication)PC.Adapters.SchemaObjectAdapter.Instance.Load(this.ApplicationId);
			if (app == null)
				throw new HttpException("指定的应用并不存在");

			if (objectSet.HasObjects)
			{
				var exec = PC.Executors.SCObjectOperations.InstanceWithPermissions;

				PC.SCRole[] pendingRoles;
				if (this.CopyMode)
				{
					pendingRoles = (from r in objectSet.Objects where r is PC.SCRole select (PC.SCRole)r).ToArray();
				}
				else
				{
					pendingRoles = (from r in objectSet.Objects join m in objectSet.Membership on r.ID equals m.ID where r.SchemaType == "Roles" && m.ContainerID == app.ID select (PC.SCRole)r).ToArray();
				}

				int count = 0;
				int allCount = pendingRoles.Length;

				foreach (var role in pendingRoles)
				{
					var role2 = this.CopyMode ? AppImportAction.MakeCopy(role) : role;

					context.SetStatus(count, allCount, "正在导入角色:" + role2.DisplayName);
					context.AppendLog("正在导入角色" + role2.ToDescription());
					exec.AddRole(role2, app); // 导入角色

					if (this.IncludeConstMembers && objectSet.HasMembership)
					{
						context.SetStatus(count, allCount, "正在查找并添加角色成员");
						context.AppendLog("正在查找角色成员");
						ImportRoleMembers(objectSet, context, exec, role, role2);
					}

					if (this.IncludeConditions && objectSet.HasConditions)
					{
						context.SetStatus(count, allCount, "正在查找并添加角色条件");
						context.AppendLog("正在查找角色条件");
						this.ImportRoleConditions(objectSet, context, exec, role, role2);
					}

					if (this.IncludeRoleDefinitions && this.CopyMode == false && objectSet.HasRelations)
					{
						this.ImportRoleDefinitions(objectSet, context, exec, count, allCount, role, role2);
					}

					count++;
				}
			}
		}

		private static void ImportRoleMembers(SCObjectSet objectSet, IImportContext context, PC.Executors.ISCObjectOperations exec, PC.SCRole roleRef, PC.SCRole roleActual)
		{
			foreach (var m in objectSet.Membership)
			{
				if (m.ContainerID == roleRef.ID)
				{
					var objSc = PC.Adapters.SchemaObjectAdapter.Instance.Load(m.ID) as PC.SCBase;
					if (objSc != null)
					{
						context.AppendLogFormat("正在替角色 {0} 导入角色成员 {1}\r\n", roleActual.DisplayName, objSc.DisplayName);
						exec.AddMemberToRole(objSc, roleActual);
					}
				}
			}
		}

		private void ImportRoleDefinitions(SCObjectSet objectSet, IImportContext context, PC.Executors.ISCObjectOperations exec, int count, int allCount, PC.SCRole role, PC.SCRole role2)
		{
			context.SetStatus(count, allCount, "正在查找并添加角色功能定义");
			context.AppendLogFormat("正在替 {0} 查找角色功能定义\r\n", role2.ToDescription());
			var permissionIds = (from p in objectSet.Relations where p.ParentID == role.ID && p.ChildSchemaType == "Permissions" select p.ID).ToArray();

			var permissions = permissionIds.Length > 0 ? DbUtil.LoadObjects(permissionIds) : null;

			if (permissions != null)
			{
				foreach (PC.SCPermission p in permissions)
				{
					var relation = PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(role.ID, p.ID);
					if (relation == null || relation.Status != SchemaObjectStatus.Normal)
					{
						string text = string.Format("正在替角色 {0} 指定功能 {1}\r\n", role.DisplayName ?? role.Name, p.DisplayName ?? p.Name);
						context.SetStatus(count, allCount, text);
						context.AppendLog(text);
						exec.JoinRoleAndPermission(role, (PC.SCPermission)p);
					}
				}
			}
		}

		private void ImportRoleConditions(SCObjectSet objectSet, IImportContext context, PC.Executors.ISCObjectOperations exec, PC.SCRole roleRef, PC.SCRole roleActual)
		{
			var conditons = (from c in objectSet.Conditions where c.OwnerID == roleRef.ID select c).ToArray();

			var owner = new PC.Conditions.SCConditionOwner()
			{
				OwnerID = roleActual.ID,
				Type = "Default"
			};
			foreach (var c in conditons)
			{
				owner.Conditions.Add(new PC.Conditions.SCCondition()
				{
					Description = c.Description,
					Condition = c.Condition,
					OwnerID = roleActual.ID,
					Type = "Default"
				});
			}

			context.AppendLogFormat("正在替角色 {0} 导入角色条件\r\n", roleActual.DisplayName);
			PC.Adapters.SCConditionAdapter.Instance.UpdateConditions(owner);
		}
	}
}