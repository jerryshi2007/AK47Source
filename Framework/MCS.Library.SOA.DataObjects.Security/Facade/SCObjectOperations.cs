using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects.Schemas.Actions;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Conditions;
using MCS.Library.SOA.DataObjects.Security.Configuration;
using MCS.Library.SOA.DataObjects.Security.Locks;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using System.Reflection;

namespace MCS.Library.SOA.DataObjects.Security.Executors
{
	public class SCObjectOperations : ISCObjectOperations
	{
		private bool _NeedCheckPermissions = false;
		private bool _NeedExecuteActions = false;
		private bool _NeedValidationAndStatusCheck = true;
		private bool _AddLock = false;

		/// <summary>
		/// 表示<see cref="SCObjectOperations"/>的实例，此字段为只读
		/// </summary>
		public static readonly ISCObjectOperations Instance = new SCObjectOperations();

		/// <summary>
		/// 没有权限和状态检查的实例
		/// </summary>
		public static readonly ISCObjectOperations InstanceWithoutValidationAndStatusCheck = new SCObjectOperations() { NeedValidationAndStatusCheck = false };

		/// <summary>
		/// 带权限检查的Operation的实例
		/// </summary>
		public static readonly ISCObjectOperations InstanceWithPermissions = new SCObjectOperations(true);

		/// <summary>
		/// 不带权限检查的Operation的实例
		/// </summary>
		public static readonly ISCObjectOperations InstanceWithoutPermissions = new SCObjectOperations(false);

		/// <summary>
		/// 不带权限检查和锁检查的实例
		/// </summary>
		public static readonly ISCObjectOperations InstanceWithoutPermissionsAndLockCheck = new SCObjectOperations(false) { AddLock = false };

		private SCObjectOperations()
		{
		}

		private SCObjectOperations(bool needCheckPermissions)
		{
			this._NeedCheckPermissions = needCheckPermissions;
			this._NeedExecuteActions = true;
			this._AddLock = needCheckPermissions && SCLockSettings.GetConfig().Enabled;
		}

		#region Properties
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
		#endregion

		#region User
		public SchemaObjectBase AddUser(SCUser user, SCOrganization parent)
		{
			SCObjectExecutor executor = null;

			if (parent == null)
			{
				if (this._NeedCheckPermissions)
					CheckSupervisorPermissions(SCOperationType.AddUser);

				executor = new SCObjectExecutor(SCOperationType.AddUser, user) { NeedValidation = this.NeedValidationAndStatusCheck };
			}
			else
			{
				if (this._NeedCheckPermissions)
					CheckPermissions(SCOperationType.AddUser, parent.Schema, "AddChildren", parent.ID);

				executor = new SCOrganizationRelativeExecutor(SCOperationType.AddUser, parent, user) { SaveTargetData = true, NeedValidation = this.NeedValidationAndStatusCheck, NeedParentStatusCheck = this.NeedValidationAndStatusCheck };
			}

			SchemaObjectBase result = null;

			ExecuteWithActions(SCOperationType.AddUser, () => SCActionContext.Current.DoActions(() => result = (SchemaObjectBase)executor.Execute()));

			return result;
		}

		public SchemaObjectBase UpdateUser(SCUser user)
		{
			SCObjectExecutor executor = new SCObjectExecutor(SCOperationType.UpdateUser, user) { NeedValidation = this.NeedValidationAndStatusCheck, NeedStatusCheck = this.NeedValidationAndStatusCheck };

			if (this._NeedCheckPermissions)
			{
				var defOrgRelation = (user.CurrentParentRelations.Find(m => m.Default)) ?? new SCRelationObject(SCOrganization.GetRoot(), user);
				CheckPermissions(SCOperationType.UpdateUser, user.Schema, "UpdateChildren", defOrgRelation.ParentID);
			}

			SchemaObjectBase result = null;

			ExecuteWithActions(SCOperationType.UpdateUser, () => SCActionContext.Current.DoActions(() => result = (SchemaObjectBase)executor.Execute()));

			return result;
		}

		public SchemaObjectBase DeleteUser(SCUser user, SCOrganization parent, bool deletedByContainer)
		{
			SchemaObjectStatus targetStatus = deletedByContainer ? SchemaObjectStatus.DeletedByContainer : SchemaObjectStatus.Deleted;
			SCOperationType op = SCOperationType.None;

			SCExecutorBase executor = null;

			if (parent == null)
			{
				op = SCOperationType.DeleteUser;

				if (this._NeedCheckPermissions)
					CheckPermissions(op, SchemaDefine.GetSchema("Organizations"), "DeleteChildren", user.OwnerID);

				user.Status = targetStatus;

				executor = new SCObjectExecutor(op, user) { NeedDeleteRelations = true, NeedValidation = false, NeedDeleteMemberRelations = this.NeedValidationAndStatusCheck, NeedStatusCheck = this.NeedValidationAndStatusCheck };
			}
			else
			{
				op = SCOperationType.RemoveUserFromOrganization;

				if (this._NeedCheckPermissions)
					CheckPermissions(op, parent.Schema, "DeleteChildren", parent.ID);

				executor = new SCOrganizationRelativeExecutor(op, parent, user) { OverrideExistedRelation = true, NeedValidation = false, NeedStatusCheck = this.NeedValidationAndStatusCheck, NeedParentStatusCheck = this.NeedValidationAndStatusCheck };

				if (((SCOrganizationRelativeExecutor)executor).Relation != null)
					((SCOrganizationRelativeExecutor)executor).Relation.Status = targetStatus;
			}

			SchemaObjectBase result = null;

			ExecuteWithActions(op, () => SCActionContext.Current.DoActions(() => result = (SchemaObjectBase)executor.Execute()));

			return result;
		}

		public SchemaObjectBase ChangeOwner(SCBase obj, SCOrganization targetOrg)
		{
			SCChangeOwnerExecutor executor = new SCChangeOwnerExecutor(SCOperationType.ChangeOwner, obj, targetOrg) { NeedStatusCheck = this.NeedValidationAndStatusCheck };

			if (this._NeedCheckPermissions)
			{
				CheckPermissions(SCOperationType.ChangeOwner, SchemaDefine.GetSchema("Organizations"), "DeleteChildren", obj.Properties.GetValue("OwnerID", string.Empty));
				CheckPermissions(SCOperationType.ChangeOwner, targetOrg.Schema, "AddChildren", targetOrg.ID);
			}

			SchemaObjectBase result = null;

			ExecuteWithActions(SCOperationType.ChangeOwner, () => SCActionContext.Current.DoActions(() => result = (SchemaObjectBase)executor.Execute()));

			return result;
		}

		/// <summary>
		/// 设置用户的默认组织
		/// </summary>
		/// <param name="user"></param>
		/// <param name="parent"></param>
		/// <returns></returns>
		public SCRelationObject SetUserDefaultOrganization(SCUser user, SCOrganization parent)
		{
			SCOrganizationRelativeExecutor executor =
				new SCOrganizationRelativeExecutor(SCOperationType.SetUserDefaultOrganization, parent, user) { OverrideExistedRelation = true, OverrideDefault = true, NeedStatusCheck = this.NeedValidationAndStatusCheck, NeedParentStatusCheck = this.NeedValidationAndStatusCheck };

			SCRelationObject result = executor.Relation;

			if (executor.RelationExisted)
			{
				if (this._NeedCheckPermissions)
				{
					var currentDefault = user.CurrentParentRelations.Where(r => r.Default && r.Status == SchemaObjectStatus.Normal).FirstOrDefault();
					if (currentDefault != null)
					{
						CheckPermissions(SCOperationType.SetUserDefaultOrganization, SchemaDefine.GetSchema("Organizations"), "UpdateChildren", currentDefault.ParentID);
					}

					CheckPermissions(SCOperationType.SetUserDefaultOrganization, SchemaDefine.GetSchema("Organizations"), "UpdateChildren", executor.Relation.ParentID);
				}

				executor.Relation.Default = true;
				ExecuteWithActions(SCOperationType.SetUserDefaultOrganization, () => SCActionContext.Current.DoActions(() => result = (SCRelationObject)executor.Execute()));
			}

			return result;
		}

		public SCRelationObject AddUserToOrganization(SCUser user, SCOrganization parent)
		{
			user.NullCheck("user");
			parent.NullCheck("parent");

			SCOrganizationRelativeExecutor executor =
				new SCOrganizationRelativeExecutor(SCOperationType.AddUserToOrganization, parent, user) { NeedValidation = this.NeedValidationAndStatusCheck, NeedParentStatusCheck = this.NeedValidationAndStatusCheck };

			if (this._NeedCheckPermissions)
				CheckPermissions(SCOperationType.AddUserToOrganization, parent.Schema, "AddChildren", parent.ID);

			SCRelationObject result = null;

			ExecuteWithActions(SCOperationType.AddUserToOrganization, () => SCActionContext.Current.DoActions(() => result = (SCRelationObject)executor.Execute()));

			return result;
		}
		#endregion User

		#region Organization
		public SCRelationObject AddOrganization(SCOrganization org, SCOrganization parent)
		{
			SCOrganizationRelativeExecutor executor =
				new SCOrganizationRelativeExecutor(SCOperationType.AddOrganization, parent, org) { SaveTargetData = true, NeedValidation = this.NeedValidationAndStatusCheck, NeedParentStatusCheck = this.NeedValidationAndStatusCheck, NeedDuplicateRelationCheck = this.NeedValidationAndStatusCheck };

			CheckPermissions(SCOperationType.AddOrganization, parent.Schema, "AddChildren", parent.ID);

			SCRelationObject result = null;

			ExecuteWithActions(SCOperationType.AddOrganization, () => SCActionContext.Current.DoActions(() => result = (SCRelationObject)executor.Execute()));

			return result;
		}

		public SCOrganization UpdateOrganization(SCOrganization org)
		{
			SCObjectExecutor executor = new SCObjectExecutor(SCOperationType.UpdateOrganization, org) { NeedValidation = this.NeedValidationAndStatusCheck, NeedStatusCheck = this.NeedValidationAndStatusCheck };
			SchemaObjectBase parent = org.CurrentParents.FirstOrDefault();

			if (parent != null)
				CheckPermissions(SCOperationType.UpdateOrganization, parent.Schema, "UpdateChildren", parent.ID);
			else
				CheckSupervisorPermissions(SCOperationType.UpdateOrganization);

			SCOrganization result = null;

			ExecuteWithActions(SCOperationType.UpdateOrganization, () => SCActionContext.Current.DoActions(() => result = (SCOrganization)executor.Execute()));

			return result;
		}

		public SchemaObjectBase DeleteOrganization(SCOrganization org, SCOrganization parent, bool deletedByContainer)
		{
			if (parent == null)
				parent = (SCOrganization)org.CurrentParents.FirstOrDefault();

			(parent != null).FalseThrow("不能找到ID为{0}的父对象", org.ID);

			CheckOrganizationChildrenPermissions(SCOperationType.DeleteOrganization, "DeleteChildren", parent);

			SCOrganizationRelativeExecutor executor
				= new SCOrganizationRelativeExecutor(SCOperationType.DeleteOrganization, parent, org) { NeedValidation = false, NeedDeleteRelations = true, SaveTargetData = true, OverrideExistedRelation = true, NeedStatusCheck = this.NeedValidationAndStatusCheck, NeedParentStatusCheck = this.NeedValidationAndStatusCheck };

			executor.Relation.Status = SchemaObjectStatus.Deleted;
			org.Status = deletedByContainer ? SchemaObjectStatus.DeletedByContainer : SchemaObjectStatus.Deleted; ;

			SchemaObjectBase result = null;

			ExecuteWithActions(SCOperationType.DeleteOrganization, () => SCActionContext.Current.DoActions(() => result = (SchemaObjectBase)executor.Execute()));

			return result;
		}

		/// <summary>
		/// 带递归地删除相关对象
		/// </summary>
		/// <param name="objs"></param>
		public SCOrganization DeleteObjectsRecursively(SchemaObjectCollection objs, SCOrganization parent)
		{
			SCDeleteObjectsRecursivelyExecutor executor = new SCDeleteObjectsRecursivelyExecutor(parent, objs) { NeedStatusCheck = this.NeedValidationAndStatusCheck };

			CheckPermissions(SCOperationType.DeleteOrganization, parent.Schema, "DeleteChildren", parent.ID);

			SCOrganization result = null;

			ExecuteWithActions(SCOperationType.DeleteOrganization, () => SCActionContext.Current.DoActions(() => result = (SCOrganization)executor.Execute()));

			return result;
		}

		/// <summary>
		/// 将<see cref="SCBase"/>添加到<paramref name="targetOrg"/>
		/// </summary>
		/// <param name="orginalOrg">移动人员时必须指定此参数，表示人员的原始组织。</param>
		/// <param name="obj">表示要进行组织移动的对象</param>
		/// <param name="targetOrg">表示要将对象移动到其中的组织</param>
		/// <returns></returns>
		public SCRelationObject MoveObjectToOrganization(SCOrganization orginalOrg, SCBase obj, SCOrganization targetOrg)
		{
			SCMoveObjectExecutor executor = new SCMoveObjectExecutor(SCOperationType.MoveObject, orginalOrg, obj, targetOrg) { NeedStatusCheck = this.NeedValidationAndStatusCheck };

			if (orginalOrg != null)
				CheckPermissions(SCOperationType.MoveObject, orginalOrg.Schema, "DeleteChildren", orginalOrg.ID);

			CheckPermissions(SCOperationType.MoveObject, targetOrg.Schema, "AddChildren", targetOrg.ID);

			SCRelationObject result = null;

			ExecuteWithActions(SCOperationType.MoveObject, () => SCActionContext.Current.DoActions(() => result = (SCRelationObject)executor.Execute()));

			return result;
		}
		#endregion Organization

		#region Group
		public SchemaObjectBase AddGroup(SCGroup group, SCOrganization parent)
		{
			SCOrganizationRelativeExecutor executor =
				new SCOrganizationRelativeExecutor(SCOperationType.AddGroup, parent, group) { SaveTargetData = true, NeedValidation = this.NeedValidationAndStatusCheck, NeedParentStatusCheck = this.NeedValidationAndStatusCheck, NeedDuplicateRelationCheck = this.NeedValidationAndStatusCheck };

			if (this._NeedCheckPermissions)
				CheckPermissions(SCOperationType.AddGroup, parent.Schema, "AddChildren", parent.ID);

			SCRelationObject result = null;

			ExecuteWithActions(SCOperationType.AddGroup, () => SCActionContext.Current.DoActions(() => result = (SCRelationObject)executor.Execute()));

			return result;
		}

		public SchemaObjectBase UpdateGroup(SCGroup group)
		{
			SCObjectExecutor executor = new SCObjectExecutor(SCOperationType.UpdateGroup, group) { NeedValidation = this.NeedValidationAndStatusCheck, NeedStatusCheck = this.NeedValidationAndStatusCheck };

			if (this._NeedCheckPermissions)
				CheckOrganizationChildrenPermissions(SCOperationType.UpdateGroup, "UpdateChildren", group);

			SchemaObjectBase result = null;

			ExecuteWithActions(SCOperationType.UpdateGroup, () => SCActionContext.Current.DoActions(() => result = (SchemaObjectBase)executor.Execute()));

			return result;
		}

		public SchemaObjectBase DeleteGroup(SCGroup group, SCOrganization parent, bool deletedByContainer)
		{
			group.Status = deletedByContainer ? SchemaObjectStatus.DeletedByContainer : SchemaObjectStatus.Deleted;

			SCExecutorBase executor = new SCObjectExecutor(SCOperationType.DeleteGroup, group) { NeedDeleteRelations = true, NeedValidation = false, NeedDeleteMemberRelations = true, NeedDeleteConditions = true, NeedStatusCheck = this.NeedValidationAndStatusCheck };

			if (parent == null)
				parent = (SCOrganization)SchemaRelationObjectAdapter.Instance.LoadByObjectID(new string[] { group.ID }).Find(m => m.Status == SchemaObjectStatus.Normal).Parent;

			if (this._NeedCheckPermissions)
				CheckPermissions(SCOperationType.DeleteGroup, parent.Schema, "DeleteChildren", parent.ID);

			SchemaObjectBase result = null;

			ExecuteWithActions(SCOperationType.DeleteGroup, () => SCActionContext.Current.DoActions(() => result = (SchemaObjectBase)executor.Execute()));

			return result;
		}

		public SCMemberRelation AddUserToGroup(SCUser user, SCGroup group)
		{
			SCMemberRelativeExecutor executor = new SCMemberRelativeExecutor(SCOperationType.AddUserToGroup, group, user) { NeedStatusCheck = this.NeedValidationAndStatusCheck, NeedContainerStatusCheck = this.NeedValidationAndStatusCheck };

			if (this._NeedCheckPermissions)
				CheckOrganizationChildrenPermissions(SCOperationType.AddUserToGroup, "EditMembersOfGroups", group);

			SCMemberRelation result = null;

			ExecuteWithActions(SCOperationType.AddUserToGroup, () => SCActionContext.Current.DoActions(() => result = (SCMemberRelation)executor.Execute()));

			return result;
		}

		public SCMemberRelation RemoveUserFromGroup(SCUser user, SCGroup group)
		{
			SCMemberRelativeExecutor executor = new SCMemberRelativeExecutor(SCOperationType.RemoveUserFromGroup, group, user) { OverrideExistedRelation = true, NeedStatusCheck = this.NeedValidationAndStatusCheck, NeedContainerStatusCheck = this.NeedValidationAndStatusCheck };

			if (this._NeedCheckPermissions)
				CheckOrganizationChildrenPermissions(SCOperationType.RemoveUserFromGroup, "EditMembersOfGroups", group);

			executor.Relation.Status = SchemaObjectStatus.Deleted;

			SCMemberRelation result = null;

			ExecuteWithActions(SCOperationType.RemoveUserFromGroup, () => SCActionContext.Current.DoActions(() => result = (SCMemberRelation)executor.Execute()));

			return result;
		}
		#endregion Group

		#region Secretary
		/// <summary>
		/// 为某人指定秘书
		/// </summary>
		/// <param name="user"></param>
		/// <param name="secretary"></param>
		/// <returns></returns>
		public SCSecretaryRelation AddSecretaryToUser(SCUser secretary, SCUser user)
		{
			SCSecretaryRelativeExecutor executor = new SCSecretaryRelativeExecutor(SCOperationType.AddSecretaryToUser, user, secretary) { NeedStatusCheck = true, NeedContainerStatusCheck = this.NeedValidationAndStatusCheck };

			if (this._NeedCheckPermissions)
			{
				var hereParents = secretary.CurrentParentRelations;
				var thereParents = user.CurrentParentRelations;
				var hereIds = (from p in hereParents where p.Status == SchemaObjectStatus.Normal select p.ParentID).ToArray();
				var thereIds = (from p in thereParents where p.Status == SchemaObjectStatus.Normal select p.ParentID).ToArray();

				CheckPermissions(SCOperationType.AddSecretaryToUser, SchemaDefine.GetSchema("Organizations"), "UpdateChildren", hereIds);
				CheckPermissions(SCOperationType.AddSecretaryToUser, SchemaDefine.GetSchema("Organizations"), "UpdateChildren", thereIds);
			}

			SCSecretaryRelation result = null;

			ExecuteWithActions(SCOperationType.AddSecretaryToUser, () => SCActionContext.Current.DoActions(() => result = (SCSecretaryRelation)executor.Execute()));

			return result;
		}

		/// <summary>
		/// 解除某人的秘书关系
		/// </summary>
		/// <param name="secretary"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public SCSecretaryRelation RemoveSecretaryFromUser(SCUser secretary, SCUser user)
		{
			SCMemberRelativeExecutor executor = new SCMemberRelativeExecutor(SCOperationType.RemoveSecretaryFromUser, user, secretary) { OverrideExistedRelation = true, NeedStatusCheck = this.NeedValidationAndStatusCheck, NeedContainerStatusCheck = this.NeedValidationAndStatusCheck };

			if (this._NeedCheckPermissions)
			{
				var hereParents = secretary.CurrentParentRelations;
				var thereParents = user.CurrentParentRelations;
				var hereIds = (from p in hereParents where p.Status == SchemaObjectStatus.Normal select p.ParentID).ToArray();
				var thereIds = (from p in thereParents where p.Status == SchemaObjectStatus.Normal select p.ParentID).ToArray();

				CheckPermissions(SCOperationType.RemoveSecretaryFromUser, SchemaDefine.GetSchema("Organizations"), "UpdateChildren", hereIds);
				CheckPermissions(SCOperationType.RemoveSecretaryFromUser, SchemaDefine.GetSchema("Organizations"), "UpdateChildren", thereIds);
			}
			executor.Relation.Status = SchemaObjectStatus.Deleted;

			SCSecretaryRelation result = null;

			ExecuteWithActions(SCOperationType.RemoveSecretaryFromUser, () => SCActionContext.Current.DoActions(() => result = (SCSecretaryRelation)executor.Execute()));

			return result;
		}
		#endregion Secretary

		#region Application
		public SchemaObjectBase AddApplication(SCApplication application)
		{
			if (this._NeedCheckPermissions)
				CheckSupervisorPermissions(SCOperationType.AddApplication);

			SCObjectExecutor executor = new SCObjectExecutor(SCOperationType.AddApplication, application) { NeedValidation = this.NeedValidationAndStatusCheck };

			SchemaObjectBase result = null;

			ExecuteWithActions(SCOperationType.AddApplication, () => SCActionContext.Current.DoActions(() => result = (SchemaObjectBase)executor.Execute()));

			return result;
		}

		public SchemaObjectBase UpdateApplication(SCApplication application)
		{
			SCObjectExecutor executor = new SCObjectExecutor(SCOperationType.UpdateApplication, application) { NeedValidation = this.NeedValidationAndStatusCheck, NeedStatusCheck = this.NeedValidationAndStatusCheck };

			if (this._NeedCheckPermissions)
				CheckPermissions(SCOperationType.UpdateApplication, application.Schema, "UpdateApplications", application.ID);

			SchemaObjectBase result = null;

			ExecuteWithActions(SCOperationType.UpdateApplication, () => SCActionContext.Current.DoActions(() => result = (SchemaObjectBase)executor.Execute()));

			return result;
		}

		public SchemaObjectBase DeleteApplication(SCApplication application)
		{
			SCObjectExecutor executor = new SCObjectExecutor(SCOperationType.DeleteApplication, application) { NeedValidation = false, NeedDeleteMemberRelations = true, NeedStatusCheck = this.NeedValidationAndStatusCheck };

			if (this._NeedCheckPermissions)
				CheckSupervisorPermissions(SCOperationType.DeleteApplication);

			application.Status = SchemaObjectStatus.Deleted;

			SchemaObjectBase result = null;

			ExecuteWithActions(SCOperationType.DeleteApplication, () => SCActionContext.Current.DoActions(() => result = (SchemaObjectBase)executor.Execute()));

			return result;
		}
		#endregion Application

		#region Role
		public SchemaObjectBase AddRole(SCRole role, SCApplication application)
		{
			SCMemberRelativeExecutor executor = new SCMemberRelativeExecutor(SCOperationType.AddRole, application, role) { NeedValidation = this.NeedValidationAndStatusCheck, SaveTargetData = true, NeedContainerStatusCheck = this.NeedValidationAndStatusCheck };

			if (this._NeedCheckPermissions)
				CheckPermissions(SCOperationType.AddRole, application.Schema, "AddRoles", application.ID);

			SCMemberRelation result = null;

			ExecuteWithActions(SCOperationType.AddRole, () => SCActionContext.Current.DoActions(() => result = (SCMemberRelation)executor.Execute()));

			return result;
		}

		public SchemaObjectBase UpdateRole(SCRole role)
		{
			SCObjectExecutor executor = new SCObjectExecutor(SCOperationType.UpdateRole, role) { NeedValidation = this.NeedValidationAndStatusCheck, NeedStatusCheck = this.NeedValidationAndStatusCheck };

			if (this._NeedCheckPermissions)
				CheckApplicationMemberPermissions(SCOperationType.UpdateRole, "UpdateRoles", role);

			SchemaObjectBase result = null;

			ExecuteWithActions(SCOperationType.UpdateRole, () => SCActionContext.Current.DoActions(() => result = (SchemaObjectBase)executor.Execute()));

			return result;
		}

		public SchemaObjectBase DeleteRole(SCRole role)
		{
			SCObjectExecutor executor = new SCObjectExecutor(SCOperationType.DeleteRole, role) { NeedValidation = false, NeedDeleteMemberRelations = true, NeedDeleteRelations = true, NeedDeleteConditions = true, NeedStatusCheck = this.NeedValidationAndStatusCheck };

			if (this._NeedCheckPermissions)
				CheckApplicationMemberPermissions(SCOperationType.DeleteRole, "DeleteRoles", role);

			role.Status = SchemaObjectStatus.Deleted;

			SchemaObjectBase result = null;

			ExecuteWithActions(SCOperationType.DeleteRole, () => SCActionContext.Current.DoActions(() => result = (SchemaObjectBase)executor.Execute()));

			return result;
		}

		public SCMemberRelation AddMemberToRole(SCBase member, SCRole role)
		{
			SCMemberRelativeExecutor executor = new SCMemberRelativeExecutor(SCOperationType.AddMemberToRole, role, member) { NeedStatusCheck = this.NeedValidationAndStatusCheck, NeedContainerStatusCheck = this.NeedValidationAndStatusCheck };

			if (this._NeedCheckPermissions)
				CheckApplicationMemberPermissions(SCOperationType.AddMemberToRole, "ModifyMembersInRoles", role);

			SCMemberRelation result = null;

			ExecuteWithActions(SCOperationType.AddMemberToRole, () => SCActionContext.Current.DoActions(() => result = (SCMemberRelation)executor.Execute()));

			return result;
		}

		public SCMemberRelation RemoveMemberFromRole(SCBase member, SCRole role)
		{
			SCMemberRelativeExecutor executor = new SCMemberRelativeExecutor(SCOperationType.RemoveMemberFromRole, role, member) { OverrideExistedRelation = true, NeedStatusCheck = this.NeedValidationAndStatusCheck, NeedContainerStatusCheck = this.NeedValidationAndStatusCheck };

			if (this._NeedCheckPermissions)
				CheckApplicationMemberPermissions(SCOperationType.RemoveMemberFromRole, "ModifyMembersInRoles", role);

			executor.Relation.Status = SchemaObjectStatus.Deleted;

			SCMemberRelation result = null;

			ExecuteWithActions(SCOperationType.RemoveMemberFromRole, () => SCActionContext.Current.DoActions(() => result = (SCMemberRelation)executor.Execute()));

			return result;
		}
		#endregion Role

		#region Permission
		public SchemaObjectBase AddPermission(SCPermission permission, SCApplication application)
		{
			SCMemberRelativeExecutor executor = new SCMemberRelativeExecutor(SCOperationType.AddPermission, application, permission) { NeedValidation = this.NeedValidationAndStatusCheck, SaveTargetData = true, NeedContainerStatusCheck = this.NeedValidationAndStatusCheck };

			if (this._NeedCheckPermissions)
				CheckPermissions(SCOperationType.AddPermission, application.Schema, "AddPermissions", application.ID);

			SCMemberRelation result = null;

			ExecuteWithActions(SCOperationType.AddPermission, () => SCActionContext.Current.DoActions(() => result = (SCMemberRelation)executor.Execute()));

			return result;
		}

		public SchemaObjectBase UpdatePermission(SCPermission permission)
		{
			SCObjectExecutor executor = new SCObjectExecutor(SCOperationType.UpdatePermission, permission) { NeedValidation = this.NeedValidationAndStatusCheck, NeedStatusCheck = this.NeedValidationAndStatusCheck };

			if (this._NeedCheckPermissions)
				CheckApplicationMemberPermissions(SCOperationType.UpdatePermission, "UpdatePermissions", permission);

			SchemaObjectBase result = null;

			ExecuteWithActions(SCOperationType.UpdatePermission, () => SCActionContext.Current.DoActions(() => result = (SchemaObjectBase)executor.Execute()));

			return result;
		}

		public SchemaObjectBase DeletePermission(SCPermission permission)
		{
			SCObjectExecutor executor = new SCObjectExecutor(SCOperationType.DeletePermission, permission) { NeedValidation = false, NeedDeleteMemberRelations = true, NeedDeleteRelations = true, NeedStatusCheck = this.NeedValidationAndStatusCheck };

			if (this._NeedCheckPermissions)
				CheckApplicationMemberPermissions(SCOperationType.DeletePermission, "DeletePermissions", permission);

			permission.Status = SchemaObjectStatus.Deleted;

			SchemaObjectBase result = null;

			ExecuteWithActions(SCOperationType.DeletePermission, () => SCActionContext.Current.DoActions(() => result = (SchemaObjectBase)executor.Execute()));

			return result;
		}

		public SCRelationObject JoinRoleAndPermission(SCRole role, SCPermission permission)
		{
			SCJoinRoleAndPermissionExecutor executor =
				new SCJoinRoleAndPermissionExecutor(SCOperationType.JoinRoleAndPermission, role, permission) { NeedStatusCheck = this.NeedValidationAndStatusCheck };

			if (this._NeedCheckPermissions)
				CheckApplicationMemberPermissions(SCOperationType.JoinRoleAndPermission, "EditRelationOfRolesAndPermissions", permission);

			SCRelationObject result = null;

			ExecuteWithActions(SCOperationType.JoinRoleAndPermission, () => SCActionContext.Current.DoActions(() => result = (SCRelationObject)executor.Execute()));

			return result;
		}

		public SCRelationObject DisjoinRoleAndPermission(SCRole role, SCPermission permission)
		{
			SCJoinRoleAndPermissionExecutor executor =
				new SCJoinRoleAndPermissionExecutor(SCOperationType.DisjoinRoleAndPermission, role, permission) { OverrideExistedRelation = true, NeedStatusCheck = this.NeedValidationAndStatusCheck };

			if (this._NeedCheckPermissions)
				CheckApplicationMemberPermissions(SCOperationType.DisjoinRoleAndPermission, "EditRelationOfRolesAndPermissions", permission);

			executor.Relation.Status = SchemaObjectStatus.Deleted;

			SCRelationObject result = null;

			ExecuteWithActions(SCOperationType.DisjoinRoleAndPermission, () => SCActionContext.Current.DoActions(() => result = (SCRelationObject)executor.Execute()));

			return result;
		}
		#endregion Permission

		#region Conditions
		/// <summary>
		/// 更新群组的条件
		/// </summary>
		/// <param name="owner"></param>
		/// <returns></returns>
		public SCConditionOwner UpdateGroupConditions(SCConditionOwner owner)
		{
			SCUpdateConditionsExecutor executor = new SCUpdateConditionsExecutor(SCOperationType.UpdateGroupConditions, owner);

			if (this._NeedCheckPermissions)
				CheckOrganizationChildrenPermissions(SCOperationType.UpdateGroupConditions, "EditMembersOfGroups", owner.OwnerID);

			SCConditionOwner result = null;

			ExecuteWithActions(SCOperationType.UpdateGroupConditions, () => SCActionContext.Current.DoActions(() => result = (SCConditionOwner)executor.Execute()));

			return result;
		}

		/// <summary>
		/// 更新角色的条件
		/// </summary>
		/// <param name="owner"></param>
		/// <returns></returns>
		public SCConditionOwner UpdateRoleConditions(SCConditionOwner owner)
		{
			SCUpdateConditionsExecutor executor = new SCUpdateConditionsExecutor(SCOperationType.UpdateRoleConditions, owner);

			if (this._NeedCheckPermissions)
				CheckApplicationMemberPermissions(SCOperationType.UpdateRoleConditions, "ModifyMembersInRoles", owner.OwnerID);

			SCConditionOwner result = null;

			ExecuteWithActions(SCOperationType.UpdateRoleConditions, () => SCActionContext.Current.DoActions(() => result = (SCConditionOwner)executor.Execute()));

			return result;
		}
		#endregion Conditions

		#region Permissions
		/// <summary>
		/// 更新对象的权限信息
		/// </summary>
		/// <param name="container">Acl的容器</param>
		/// <returns>返回容器本身</returns>
		public SCAclContainer UpdateObjectAcl(SCAclContainer container)
		{
			SCUpdateObjectAclExecutor executor = new SCUpdateObjectAclExecutor(SCOperationType.UpdateObjectAcl, container);

			if (this._NeedCheckPermissions)
				CheckUpdateAclPermissions(SCOperationType.UpdateObjectAcl, container.ContainerID);

			SCAclContainer result = null;

			ExecuteWithActions(SCOperationType.UpdateObjectAcl, () => SCActionContext.Current.DoActions(() => result = (SCAclContainer)executor.Execute()));

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
			SCReplaceAclRecursivelyExecutor executor =
				new SCReplaceAclRecursivelyExecutor(SCOperationType.ReplaceAclRecursively, container) { ForceReplace = forceReplace };

			if (this._NeedCheckPermissions)
				CheckUpdateAclPermissions(SCOperationType.UpdateObjectAcl, ((SchemaObjectBase)container).ID);

			ISCAclContainer result = null;

			ExecuteWithActions(SCOperationType.UpdateObjectAcl, () => SCActionContext.Current.DoActions(() => result = (ISCAclContainer)executor.Execute()));

			return result;
		}
		#endregion

		#region SchemaObjectBase
		/// <summary>
		/// 替换对象的图片属性
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="propertyName"></param>
		/// <param name="image"></param>
		/// <returns></returns>
		public SchemaObjectBase UpdateObjectImageProperty(SchemaObjectBase obj, string propertyName, ImageProperty image)
		{
			SCUpdateObjectImageExecutor executor = new SCUpdateObjectImageExecutor(SCOperationType.UpdateObjectImage, obj, propertyName, image) { NeedStatusCheck = true };

			string ownerID = obj.Properties.GetValue("OwnerID", string.Empty);

			if (ownerID.IsNotEmpty())
			{
				if (this._NeedCheckPermissions)
					CheckPermissions(SCOperationType.UpdateObjectImage, SchemaDefine.GetSchema("Organizations"), "UpdateChildren", ownerID);
			}
			else
			{
				if (this._NeedCheckPermissions)
					CheckOrganizationChildrenPermissions(SCOperationType.UpdateObjectImage, "UpdateChildren", obj);
			}

			SchemaObjectBase result = null;

			ExecuteWithActions(SCOperationType.UpdateObjectImage, () => SCActionContext.Current.DoActions(() => result = (SchemaObjectBase)executor.Execute()));

			return result;
		}

		#endregion

		public SchemaObjectBase DoOperation(SCObjectOperationMode opMode, SchemaObjectBase data, SchemaObjectBase parent, bool deletedByContainer = false)
		{
			data.NullCheck("data");

			SchemaOperationDefine sod = data.Schema.Operations[opMode];

			(sod != null).FalseThrow("不能找到Schema类型为{0}，操作为{1}的方法定义", data.SchemaType, opMode);

			return this.DoOperation(sod, data, parent, deletedByContainer);
		}

		/// <summary>
		/// 执行操作
		/// </summary>
		/// <param name="data">用于操作的<see cref="SchemaObjectBase"/>对象</param>
		/// <param name="parent">用于操作的父<see cref="SchemaObjectBase"/>对象</param>
		/// <param name="deletedByContainer">是否被容器删除</param>
		/// <returns></returns>
		private SchemaObjectBase DoOperation(SchemaOperationDefine sod, SchemaObjectBase data, SchemaObjectBase parent, bool deletedByContainer = false)
		{
			sod.NullCheck("sod");
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
			catch (System.Exception ex)
			{
				throw ex.GetRealException();
			}
		}

		#region CheckPermission
		private void CheckUpdateAclPermissions(SCOperationType opType, string containerID)
		{
			if (this.NeedCheckPermissionAndCurrentUserIsNotSupervisor)
			{
				SchemaObjectBase containerObj = SchemaObjectAdapter.Instance.Load(containerID);

				if (containerObj != null)
				{
					if (containerObj is ISCApplicationMember)
						CheckApplicationMemberPermissions(opType, "UpdateRoles", (ISCApplicationMember)containerObj);
					else
						CheckOrganizationChildrenPermissions(opType, "EditPermissionsOfChildren", containerObj);
				}
			}
		}

		private void CheckApplicationMemberPermissions(SCOperationType opType, string permissionName, ISCApplicationMember member)
		{
			if (this.NeedCheckPermissionAndCurrentUserIsNotSupervisor)
			{
				SCApplication application = member.CurrentApplication;

				if (application != null)
					CheckPermissions(opType, application.Schema, permissionName, application.ID);
			}
		}

		private void CheckApplicationMemberPermissions(SCOperationType opType, string permissionName, string memberID)
		{
			if (this.NeedCheckPermissionAndCurrentUserIsNotSupervisor)
			{
				ISCApplicationMember member = SchemaObjectAdapter.Instance.Load(memberID) as ISCApplicationMember;

				if (member != null)
				{
					CheckApplicationMemberPermissions(opType, permissionName, member);
				}
			}
		}

		private void CheckOrganizationChildrenPermissions(SCOperationType opType, string permissionName, SchemaObjectBase child)
		{
			SchemaObjectBase parent = child.CurrentParents.FirstOrDefault();

			if (parent != null)
				CheckPermissions(opType, parent.Schema, permissionName, parent.ID);
			else
				CheckSupervisorPermissions(opType);
		}

		private void CheckOrganizationChildrenPermissions(SCOperationType opType, string permissionName, string childID)
		{
			SchemaObjectBase child = SchemaObjectAdapter.Instance.Load(childID);

			if (child != null)
			{
				CheckOrganizationChildrenPermissions(opType, permissionName, child);
			}
		}

		private void CheckPermissions(SCOperationType opType, SchemaDefine schemaInfo, string permissionName, params string[] containerIDs)
		{
			if (NeedCheckPermissionAndCurrentUserIsNotSupervisor && DeluxePrincipal.Current.HasPermissions(permissionName, containerIDs) == false)
				throw SCAclPermissionCheckException.CreateException(opType, schemaInfo, permissionName);
		}

		private void CheckSupervisorPermissions(SCOperationType opType)
		{
			if (NeedCheckPermissionAndCurrentUserIsNotSupervisor)
			{
				string opDesp = EnumItemDescriptionAttribute.GetDescription(opType);

				throw new SCAclPermissionCheckException(string.Format("您不是超级管理员，不能执行\"{0}\"操作", opDesp));
			}
		}

		/// <summary>
		/// 是否需要权限检查且当前人员不是超级管理员
		/// </summary>
		private bool NeedCheckPermissionAndCurrentUserIsNotSupervisor
		{
			get
			{
				return this._NeedCheckPermissions && DeluxePrincipal.Current.IsSupervisor() == false;
			}
		}
		#endregion CheckPermission

		#region Action Wrapper
		private void ExecuteWithActions(SCOperationType operationType, Action action)
		{
			SCDataOperationLockContext.Current.DoAddLockAction(this._AddLock, EnumItemDescriptionAttribute.GetDescription(operationType), () =>
			{
				if (this._NeedExecuteActions && action != null)
				{
					SCObjectOperationActionCollection actions = SCObjectOperationActionSettings.GetConfig().GetActions();

					actions.BeforeExecute(operationType);

					action();

					actions.AfterExecute(operationType);
				}
				else
					action();
			});
		}
		#endregion
	}
}
