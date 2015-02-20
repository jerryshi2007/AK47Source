using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.Conditions;
using MCS.Library.SOA.DataObjects.Security.Permissions;

namespace MCS.Library.SOA.DataObjects.Security.Executors
{
	public interface ISCObjectOperations
	{
		SchemaObjectBase AddUser(SCUser user, SCOrganization parent);

		SchemaObjectBase UpdateUser(SCUser user);

		/// <summary>
		/// 有Parent参数，表示删除关系，没有的表示删除对象
		/// </summary>
		/// <param name="user"></param>
		/// <param name="parent"></param>
		/// <param name="deletedByContainer">是否被动删除</param>
		/// <returns></returns>
		SchemaObjectBase DeleteUser(SCUser user, SCOrganization parent, bool deletedByContainer);

		/// <summary>
		/// 修改对象的所有者
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="targetOrg"></param>
		/// <returns></returns>
		SchemaObjectBase ChangeOwner(SCBase obj, SCOrganization targetOrg);

		SCRelationObject AddUserToOrganization(SCUser user, SCOrganization parent);

		/// <summary>
		/// 设置用户的默认组织
		/// </summary>
		/// <param name="user"></param>
		/// <param name="parent"></param>
		/// <returns></returns>
		SCRelationObject SetUserDefaultOrganization(SCUser user, SCOrganization parent);

		SCRelationObject AddOrganization(SCOrganization org, SCOrganization parent);

		SCOrganization UpdateOrganization(SCOrganization org);

		SchemaObjectBase DeleteOrganization(SCOrganization org, SCOrganization parent, bool deletedByContainer);

		/// <summary>
		/// 带递归地删除相关对象
		/// </summary>
		/// <param name="objectsToDelete">待删除对象</param>
		/// <param name="parent"></param>
		SCOrganization DeleteObjectsRecursively(SchemaObjectCollection objectsToDelete, SCOrganization parent);

		/// <summary>
		/// 从一个组织下移动对象到另一个组织，如果该对象在原组织内不存在，则取对象默认组织。
		/// </summary>
		/// <param name="orginalOrg">原来所属的组织，主要是用于用户类的对象，组织和群组都不需要此参数</param>
		/// <param name="obj"></param>
		/// <param name="targetOrg"></param>
		/// <returns></returns>
		SCRelationObject MoveObjectToOrganization(SCOrganization orginalOrg, SCBase obj, SCOrganization targetOrg);

		SchemaObjectBase AddGroup(SCGroup group, SCOrganization parent);

		SchemaObjectBase UpdateGroup(SCGroup group);

		SchemaObjectBase DeleteGroup(SCGroup group, SCOrganization parent, bool deletedByContainer);

		SCMemberRelation AddUserToGroup(SCUser user, SCGroup group);

		SCMemberRelation RemoveUserFromGroup(SCUser user, SCGroup group);

		/// <summary>
		/// 为某人指定秘书
		/// </summary>
		/// <param name="user"></param>
		/// <param name="secretary"></param>
		/// <returns></returns>
		SCSecretaryRelation AddSecretaryToUser(SCUser secretary, SCUser user);

		/// <summary>
		/// 解除某人的秘书关系
		/// </summary>
		/// <param name="secretary"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		SCSecretaryRelation RemoveSecretaryFromUser(SCUser secretary, SCUser user);

		SchemaObjectBase AddApplication(SCApplication application);

		SchemaObjectBase UpdateApplication(SCApplication application);

		SchemaObjectBase DeleteApplication(SCApplication application);

		SchemaObjectBase AddRole(SCRole role, SCApplication application);

		SchemaObjectBase UpdateRole(SCRole role);

		SchemaObjectBase DeleteRole(SCRole role);

		SchemaObjectBase AddPermission(SCPermission permission, SCApplication application);

		SchemaObjectBase UpdatePermission(SCPermission permission);

		SchemaObjectBase DeletePermission(SCPermission permission);

		SCMemberRelation AddMemberToRole(SCBase member, SCRole role);

		SCMemberRelation RemoveMemberFromRole(SCBase member, SCRole role);

		SCRelationObject JoinRoleAndPermission(SCRole role, SCPermission permission);

		SchemaObjectBase DoOperation(SCObjectOperationMode opMode, SchemaObjectBase data, SchemaObjectBase parent, bool deletedByContainer = false);

		SCRelationObject DisjoinRoleAndPermission(SCRole role, SCPermission permission);

		/// <summary>
		/// 更新群组的条件
		/// </summary>
		/// <param name="group"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		SCConditionOwner UpdateGroupConditions(SCConditionOwner owner);

		/// <summary>
		/// 更新角色的条件
		/// </summary>
		/// <param name="role"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		SCConditionOwner UpdateRoleConditions(SCConditionOwner owner);

		/// <summary>
		/// 更新对象的权限信息
		/// </summary>
		/// <param name="container">Acl的容器</param>
		/// <returns>返回容器本身</returns>
		SCAclContainer UpdateObjectAcl(SCAclContainer container);

		/// <summary>
		/// 递归替换子对象的Acl
		/// </summary>
		/// <param name="container"></param>
		/// <param name="forceReplace">是否强制覆盖</param>
		/// <returns></returns>
		ISCAclContainer ReplaceAclRecursively(ISCAclContainer container, bool forceReplace);

		/// <summary>
		/// 替换对象的图片属性
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="propertyName"></param>
		/// <param name="image"></param>
		/// <returns></returns>
		SchemaObjectBase UpdateObjectImageProperty(SchemaObjectBase obj, string propertyName, ImageProperty image);
	}
}
