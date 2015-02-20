using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Executors;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Actions;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Configuration;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Locks;
using System.Diagnostics;
using MCS.Library.SOA.DataObjects.Schemas.Actions;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using System.Reflection;
using MCS.Library.SOA.DataObjects.Security.Conditions;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Operations
{
	/// <summary>
	/// 访问者外观
	/// </summary>
	public sealed class Facade : IFacade, IAUOperations, IAUSchemaOperations, IAUAclOperations, IDynamicOperations
	{
		private bool _AddLock = false;
		private bool _NeedExecuteActions = false;
		private bool _NeedCheckPermissions = false;
		private bool _NeedValidationAndStatusCheck = true;

		/// <summary>
		/// 缺省的实例
		/// </summary>
		public static readonly IFacade DefaultInstance = new Facade() { };
		/// <summary>
		/// 不带校验和状态检查的实例
		/// </summary>
		public static readonly IFacade InstanceWithoutValidationAndStatusCheck = new Facade() { _NeedValidationAndStatusCheck = false };
		/// <summary>
		/// 带权限的实例
		/// </summary>
		public static readonly IFacade InstanceWithPermissions = new Facade(true);
		/// <summary>
		/// 带权限的实例
		/// </summary>
		public static readonly IFacade InstanceWithoutPermissionsAndLockCheck = new Facade(false) { _AddLock = false };
		/// <summary>
		/// 不带权限的实例
		/// </summary>
		public static readonly IFacade InstanceWithoutPermissions = new Facade(false) { _AddLock = true };

		Facade()
		{
		}

		private Facade(bool needCheckPermissions)
		{
			this._NeedCheckPermissions = needCheckPermissions;
			this._NeedExecuteActions = true;
			this._AddLock = needCheckPermissions && SCLockSettings.GetConfig().Enabled;
		}

		public bool AddLock
		{
			get
			{
				return this._AddLock;
			}
			set
			{
				this._AddLock = value;
			}
		}

		/// <summary>
		/// 是否需要执行Operation后的Action
		/// </summary>
		public bool NeedExecuteActions
		{
			get
			{
				return this._NeedExecuteActions;
			}
			set
			{
				this._NeedExecuteActions = value;
			}
		}

		/// <summary>
		/// 是否需要执行校验和状态检查
		/// </summary>
		public bool NeedValidationAndStatusCheck
		{
			get
			{
				return this._NeedValidationAndStatusCheck;
			}
			set
			{
				this._NeedValidationAndStatusCheck = value;
			}
		}

		public bool NeedCheckPermissionAndCurrentUserIsNotSupervisor
		{
			get
			{
				return this._NeedCheckPermissions && DeluxePrincipal.Current.IsSupervisor() == false;
			}
		}

		public SchemaObjectBase DoOperation(SCObjectOperationMode opMode, SchemaObjectBase data, SchemaObjectBase parent, bool deletedByContainer = false)
		{
			data.NullCheck("data");

			SchemaOperationDefine sod = data.Schema.Operations[opMode];

			(sod != null).FalseThrow("不能找到Schema类型为{0}，操作为{1}的方法定义", data.SchemaType, opMode);

			return this.DoOperation(sod, data, parent, deletedByContainer);
		}

		private SchemaObjectBase DoOperation(SchemaOperationDefine sod, SchemaObjectBase data, SchemaObjectBase parent, bool deletedByContainer)
		{
			sod.MethodName.CheckStringIsNullOrEmpty("MethodName");

			Type type = this.GetType();

			MethodInfo mi = type.GetMethod(sod.MethodName);

			(mi != null).FalseThrow("不能在类型{0}中找到方法{1}", type.FullName, sod.MethodName);

			object[] parameters = null;

			if (sod.HasParentParemeter || parent != null)
			{
				if (sod.OperationMode == SCObjectOperationMode.Delete)
					parameters = new object[] { data, parent, deletedByContainer };
				else
					parameters = new object[] { data, parent };
			}
			else
				parameters = new object[] { data };

			try
			{
				return (SchemaObjectBase)mi.Invoke(this, parameters);
			}
			catch (TargetParameterCountException ex)
			{
				Exception realException = ex.GetRealException();

				throw new ApplicationException(string.Format("调用方法{0}出现异常，参数个数为{1}: {2}",
					sod.MethodName, parameters.Length, realException.Message), realException);
			}
		}

		public void AddAdminSchema(AUSchema schema)
		{
			CheckSupervisior();

			AUSchemaExecutor executor = new Executors.AUSchemaExecutor(AUOperationType.AddAdminSchema, schema);

			ExecuteWithActions(AUOperationType.AddAdminSchema, () => SCActionContext.Current.DoActions(() =>
			{
				executor.Execute();
			}));
		}

		public void DeleteAdminSchema(AUSchema schema)
		{
			CheckSupervisior();

			AUSchemaExecutor executor = new Executors.AUSchemaExecutor(AUOperationType.RemoveAdminSchema, schema);

			schema.Status = SchemaObjectStatus.Deleted;

			ExecuteWithActions(AUOperationType.RemoveAdminSchema, () => SCActionContext.Current.DoActions(() =>
			{
				executor.Execute();
			}));
		}

		public void UpdateAdminSchema(AUSchema schema)
		{
			CheckSupervisior();

			AUSchemaExecutor executor = new Executors.AUSchemaExecutor(AUOperationType.UpdateAdminSchema, schema);

			ExecuteWithActions(AUOperationType.UpdateAdminSchema, () => SCActionContext.Current.DoActions(() =>
			{
				executor.Execute();
			}));
		}

		public void AddAdminSchemaRole(AUSchemaRole role, AUSchema schema)
		{
			CheckSupervisior();

			AUSchemaRoleExecutor executor = new AUSchemaRoleExecutor(AUOperationType.AddSchemaRole, schema, role)
			{
				SaveTargetData = true,
				OverrideExistedRelation = true,
				NeedContainerStatusCheck = true,
			};

			role.Status = SchemaObjectStatus.Normal;

			ExecuteWithActions(AUOperationType.AddSchemaRole, () => SCActionContext.Current.DoActions(() =>
			{
				executor.Execute();
			}));
		}

		public void DeleteAdminSchemaRole(AUSchemaRole role)
		{
			CheckSupervisior();

			Executors.AUSchemaRoleExecutor executor = new Executors.AUSchemaRoleExecutor(AUOperationType.RemoveSchemaRole, role.GetCurrentOwnerAUSchema(), role)
			{
				SaveTargetData = true,
				OverrideExistedRelation = true,
				NeedContainerStatusCheck = true,
			};

			role.Status = SchemaObjectStatus.Deleted;

			ExecuteWithActions(AUOperationType.RemoveSchemaRole, () => SCActionContext.Current.DoActions(() =>
			{
				executor.Execute();
			}));
		}

		public void UpdateAdminSchemaRole(AUSchemaRole role)
		{
			CheckSupervisior();

			AUObjectExecutor executor = new Executors.AUObjectExecutor(AUOperationType.UpdateAdminSchemaRole, role)
			{
			};

			ExecuteWithActions(AUOperationType.UpdateAdminSchemaRole, () => SCActionContext.Current.DoActions(() =>
			{
				executor.Execute();
			}));
		}

		public void AddAdminUnit(AdminUnit unit, AdminUnit parent)
		{
			if (parent == null)
				CheckAUSchemaPermission(unit.GetUnitSchema());
			else
				CheckUnitPermission(AUOperationType.AddAdminUnit, "AddSubUnit", parent);

			AdminUnitExecutor executor = new Executors.AdminUnitExecutor(AUOperationType.AddAdminUnit, parent, unit)
			{
				NeedValidation = this.NeedValidationAndStatusCheck,
				NeedParentStatusCheck = this.NeedValidationAndStatusCheck,
			};

			ExecuteWithActions(AUOperationType.AddAdminUnit, () => SCActionContext.Current.DoActions(() =>
			{
				executor.Execute();
			}));
		}

		public void AddAdminUnitWithMembers(AdminUnit unit, AdminUnit parent, AURole[] roles, AUAdminScope[] scopes)
		{
			if (parent == null)
				CheckAUSchemaPermission(unit.GetUnitSchema());
			else
				CheckUnitPermission(AUOperationType.AddAdminUnit, "AddSubUnit", parent);

			AdminUnitExecutor executor = new Executors.AdminUnitExecutor(AUOperationType.AddAdminUnit, parent, unit)
			{
				NeedValidation = this.NeedValidationAndStatusCheck,
				NeedParentStatusCheck = this.NeedValidationAndStatusCheck,
				InputRoles = roles,
				InputAdminScopes = scopes

			};

			ExecuteWithActions(AUOperationType.AddAdminUnit, () => SCActionContext.Current.DoActions(() =>
			{
				executor.Execute();
			}));
		}

		/// <summary>
		/// 确保当前用户是超级管理员
		/// </summary>
		private void CheckSupervisior()
		{
			if (this._NeedCheckPermissions)
			{
				if (Facade.IsSupervisior() == false)
					throw new SCAclPermissionCheckException("只有总管理员才可以做这个操作");
			}
		}

		private static bool IsSupervisior()
		{
			return AUPermissionHelper.IsSupervisor(DeluxePrincipal.Current);
		}

		private void CheckAUSchemaPermission(AUSchema schema)
		{
			if (this._NeedCheckPermissions)
			{
				if (IsSupervisior() == false)
					if (string.IsNullOrEmpty(schema.MasterRole) || DeluxePrincipal.Current.IsInRole(schema.MasterRole) == false)
						throw new SCAclPermissionCheckException(string.Format("当前用户不属于管理架构 {0} 的管理单元管理员或总管理员，不能添加顶级管理单元", schema.ToDescription()));
			}
		}

		private void CheckUnitPermission(AUOperationType opType, string permissionName, AdminUnit unit)
		{
			unit.NullCheck("unit");
			if (unit.Status != SchemaObjectStatus.Normal)
				throw new AUStatusCheckException(unit, opType);

			if (this._NeedCheckPermissions)
			{
				if (unit == null || unit.Status != SchemaObjectStatus.Normal)
					throw new ArgumentException(string.Format("不存在参数 unit 指定的管理单元", "unit"));

				if (DeluxePrincipal.Current.HasPermissions(permissionName, new string[] { unit.ID }) == false)
				{
					//如果没有权限，检查是否超级管理员或者拥有架构权限
					if (AUPermissionHelper.IsSupervisor(DeluxePrincipal.Current) == false)
					{
						var schema = unit.GetUnitSchema();
						if (string.IsNullOrEmpty(schema.MasterRole) || DeluxePrincipal.Current.IsInRole(schema.MasterRole) == false)
							throw CreateAclException(opType, unit.Schema, permissionName);
					}
				}
			}
		}

		private SCAclPermissionCheckException CreateAclException(AUOperationType opType, SchemaDefine schemaInfo, string permissionName)
		{
			string opDesp = EnumItemDescriptionAttribute.GetDescription(opType);

			SCAclPermissionItem permissionInfo = schemaInfo.PermissionSet[permissionName];

			string permissionDesp = string.Empty;

			if (permissionInfo != null)
			{
				permissionDesp = permissionInfo.Description;

				if (permissionDesp.IsNullOrEmpty())
					permissionDesp = permissionInfo.Name;
			}

			return new SCAclPermissionCheckException(string.Format("不能执行\"{0}\"操作，您没有\"{0}\"权限", opDesp, permissionDesp));
		}

		private void CheckUpdateAclPermissions(AUOperationType opType, string containerID)
		{
			if (this.NeedCheckPermissionAndCurrentUserIsNotSupervisor)
			{
				AdminUnit unit = null;

				AUCommon.DoDbAction(() =>
				{
					unit = (AdminUnit)PC.Adapters.SchemaObjectAdapter.Instance.Load(containerID);
				});

				if (unit == null || unit.Status != SchemaObjectStatus.Normal)
					throw new AUObjectException("指定的管理单元不存在");

				CheckUnitPermission(opType, "EditSubUnitAcl", unit);
			}
		}

		/// <summary>
		/// 移动管理单元
		/// </summary>
		/// <param name="unit">将被移动的管理单元</param>
		/// <param name="newParent">一个表示目标单元的<see cref="AdminUnit"/> ，或者为null，表示作为顶级管理单元</param>
		public void MoveAdminUnit(AdminUnit unit, AdminUnit newParent)
		{
			unit.NullCheck("unit");

			var parent = GetUnitParent(unit, false);

			if (parent is AUSchema)
				CheckAUSchemaPermission((AUSchema)parent);
			else
				CheckUnitPermission(AUOperationType.AddAdminUnit, "DeleteSubUnit", (AdminUnit)parent);

			if (newParent != null)
				CheckUnitPermission(AUOperationType.AddAdminUnit, "AddSubUnit", newParent);
			else
				CheckAUSchemaPermission(unit.GetUnitSchema());

			MoveAUExecutor executor = new MoveAUExecutor(AUOperationType.MoveAdminUnit, unit, newParent)
			{
				NeedStatusCheck = this.NeedValidationAndStatusCheck
			};

			ExecuteWithActions(AUOperationType.MoveAdminUnit, () => SCActionContext.Current.DoActions(() =>
			{
				executor.Execute();
			}));
		}

		public void UpdateAdminUnit(AdminUnit unit)
		{
			CheckUnitPermission(AUOperationType.AddAdminUnit, "EditProperty", unit);

			AUObjectExecutor executor = new AUObjectExecutor(AUOperationType.UpdateAdminUnit, unit)
			{
				NeedValidation = this.NeedValidationAndStatusCheck,
				NeedStatusCheck = this.NeedValidationAndStatusCheck
			};

			ExecuteWithActions(AUOperationType.UpdateAdminUnit, () => SCActionContext.Current.DoActions(() =>
			{
				executor.Execute();
			}));
		}

		public void DeleteAdminUnit(AdminUnit unit)
		{
			var parent = GetUnitParent(unit, false);

			if (parent is AdminUnit)
			{
				CheckUnitPermission(AUOperationType.RemoveAdminUnit, "DeleteSubUnit", (AdminUnit)parent);
			}
			else if (parent is AUSchema)
			{
				CheckAUSchemaPermission((AUSchema)parent);
			}

			AdminUnitExecutor executor = new AdminUnitExecutor(AUOperationType.RemoveAdminUnit, null, unit)
			{
				NeedValidation = this.NeedValidationAndStatusCheck,
				NeedParentStatusCheck = this.NeedValidationAndStatusCheck,
			};

			unit.Status = SchemaObjectStatus.Deleted;

			ExecuteWithActions(AUOperationType.AddAdminUnit, () => SCActionContext.Current.DoActions(() =>
			{
				executor.Execute();
			}));
		}

		private static SchemaObjectBase GetUnitParent(AdminUnit unit, bool allowNull)
		{
			SchemaObjectBase parent = null;
			var parentRelaion = unit.GetCurrentVeryParentRelation();
			if (parentRelaion != null)
				AUCommon.DoDbAction(() => { parent = parentRelaion.Parent; });
			else if (allowNull == false)
				throw new AUObjectException("此管理单元没有任何父级，这可能是数据存在错误或此管理单元未添加到系统。");

			return parent;
		}

		public void AddObjectToScope(AUAdminScopeItem item, AUAdminScope scope)
		{
			AdminUnit unit = scope.GetOwnerUnit();
			CheckUnitPermission(AUOperationType.AddAdminUnit, "EditAdminScope", unit);

			AUMemberRelativeExecutor executor = new AUMemberRelativeExecutor(AUOperationType.AddAUScopeItem, scope, item)
			{
				SaveTargetData = false,
				NeedValidation = this.NeedValidationAndStatusCheck,
				NeedContainerStatusCheck = this.NeedValidationAndStatusCheck,
			};

			ExecuteWithActions(AUOperationType.AddAUScopeItem, () => SCActionContext.Current.DoActions(() =>
			{
				executor.Execute();
			}));
		}

		public void RemoveObjectFromScope(AUAdminScopeItem item, AUAdminScope scope)
		{
			AdminUnit unit = scope.GetOwnerUnit();
			CheckUnitPermission(AUOperationType.AddAdminUnit, "EditAdminScope", unit);

			AUMemberRelativeExecutor executor = new AUMemberRelativeExecutor(AUOperationType.AddAUScopeItem, scope, item)
			{
				SaveTargetData = false,
				NeedValidation = this.NeedValidationAndStatusCheck,
				NeedContainerStatusCheck = this.NeedValidationAndStatusCheck,
			};

			executor.Data.Status = SchemaObjectStatus.Deleted;
			executor.Relation.Status = SchemaObjectStatus.Deleted;

			ExecuteWithActions(AUOperationType.AddAUScopeItem, () => SCActionContext.Current.DoActions(() =>
			{
				executor.Execute();
			}));
		}

		public void AddUserToRole(SCUser user, AdminUnit unit, AUSchemaRole role)
		{
			CheckUnitPermission(AUOperationType.RemoveUserFromRole, "EditRoleMembers", unit);

			SchemaObjectBase r = Adapters.AUSnapshotAdapter.Instance.LoadAURole(role.ID, unit.ID, true, DateTime.MinValue);
			if (r == null)
				throw new AUObjectValidationException("没有找到此管理单元的角色，请尝试重新添加此角色");

			AUMemberRelativeExecutor executor = new Executors.AUMemberRelativeExecutor(AUOperationType.AddUserToRole, r, user)
			{
				SaveTargetData = false,
				NeedValidation = false,
				NeedContainerStatusCheck = this.NeedValidationAndStatusCheck,
			};

			ExecuteWithActions(AUOperationType.AddUserToRole, () => SCActionContext.Current.DoActions(() =>
			{
				executor.Execute();
			}));
		}

		public void RemoveUserFromRole(SCUser user, AdminUnit unit, AUSchemaRole role)
		{
			CheckUnitPermission(AUOperationType.RemoveUserFromRole, "EditRoleMembers", unit);

			SchemaObjectBase r = Adapters.AUSnapshotAdapter.Instance.LoadAURole(role.ID, unit.ID, true, DateTime.MinValue);
			if (r == null)
				throw new AUObjectValidationException("没有找到此管理单元的角色，请尝试重新添加此角色");

			AUMemberRelativeExecutor executor = new AUMemberRelativeExecutor(AUOperationType.RemoveUserFromRole, r, user)
			{
				OverrideExistedRelation = true,
				SaveTargetData = false,
				NeedStatusCheck = false,
				NeedContainerStatusCheck = this.NeedValidationAndStatusCheck,
			};

			executor.Relation.Status = SchemaObjectStatus.Deleted;

			SCMemberRelation result = null;

			ExecuteWithActions(AUOperationType.RemoveUserFromRole, () => SCActionContext.Current.DoActions(() => result = (SCMemberRelation)executor.Execute()));
		}

		public void ReplaceUsersInRole(SCUser[] users, AdminUnit unit, AUSchemaRole role)
		{
			CheckUnitPermission(AUOperationType.RemoveUserFromRole, "EditRoleMembers", unit);

			AURoleMemberExecutor executor = new AURoleMemberExecutor(AUOperationType.RemoveUserFromRole, role, unit, users)
			{
				NeedStatusCheck = this.NeedValidationAndStatusCheck,
				NeedContainerStatusCheck = this.NeedValidationAndStatusCheck
			};

			ExecuteWithActions(AUOperationType.RemoveUserFromRole, () => SCActionContext.Current.DoActions(() => executor.Execute()));
		}

		public void UpdateScopeCondition(AUAdminScope scope, SCCondition condition)
		{
			AdminUnit unit = scope.GetOwnerUnit();
			CheckUnitPermission(AUOperationType.AddAdminUnit, "EditAdminScope", unit);

			if (condition.OwnerID != scope.ID)
				throw new AUObjectValidationException("条件的OwnerID必须是Scope的ID");

			AUUpdateConditionsExecutor executor = new AUUpdateConditionsExecutor(AUOperationType.UpdateScopeCondition, scope, condition)
			{
			};

			ExecuteWithActions(AUOperationType.UpdateScopeCondition, () => SCActionContext.Current.DoActions(() => executor.Execute()));
		}

		[DebuggerNonUserCode]
		private void ExecuteWithActions(AUOperationType operationType, Action action)
		{
			AUCommon.DoDbAction(() =>
			{
				MCS.Library.SOA.DataObjects.Security.Locks.SCDataOperationLockContext.Current.DoAddLockAction(this._AddLock, EnumItemDescriptionAttribute.GetDescription(operationType), () =>
				{
					if (this._NeedExecuteActions && action != null)
					{
						AUObjectOperationActionCollection actions = AUObjectOperationActionSettings.GetConfig().GetActions();

						actions.BeforeExecute(operationType);

						action();

						actions.AfterExecute(operationType);
					}
					else
						action();
				});
			});
		}

		/// <summary>
		/// 更新对象的权限信息
		/// </summary>
		/// <param name="container">Acl的容器</param>
		/// <returns>返回容器本身</returns>
		public SCAclContainer UpdateObjectAcl(SCAclContainer container)
		{
			if (this._NeedCheckPermissions)
				CheckUpdateAclPermissions(AUOperationType.UpdateObjectAcl, container.ContainerID);

			AUUpdateObjectAclExecutor executor = new AUUpdateObjectAclExecutor(AUOperationType.UpdateObjectAcl, container);

			SCAclContainer result = null;

			ExecuteWithActions(AUOperationType.UpdateObjectAcl, () => SCActionContext.Current.DoActions(() => result = (SCAclContainer)executor.Execute()));

			return result;
		}

		/// <summary>
		/// 递归替换子对象的Acl
		/// </summary>
		/// <param name="container"></param>
		/// <param name="forceReplace">是否强制覆盖</param>
		/// <returns></returns>
		public ISCAclContainer ReplaceAclRecursively(ISCAclContainer container, bool forceReplace)
		{

			if (this._NeedCheckPermissions)
				CheckUpdateAclPermissions(AUOperationType.UpdateObjectAcl, ((SchemaObjectBase)container).ID);

			AUReplaceAclRecursivelyExecutor executor =
				new AUReplaceAclRecursivelyExecutor(AUOperationType.ReplaceAclRecursively, container) { ForceReplace = forceReplace };

			ISCAclContainer result = null;

			ExecuteWithActions(AUOperationType.UpdateObjectAcl, () => SCActionContext.Current.DoActions(() => result = (ISCAclContainer)executor.Execute()));

			return result;
		}
	}

	public interface IFacade : IAUOperations, IAUSchemaOperations, IAUAclOperations, IDynamicOperations
	{
	};
}
