using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Web;
using System.Web.Services;
using System.Xml.Serialization;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Schemas.Client;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.Client;
using PermissionCenter.Clients;
using PermissionCenter.Extensions;
using PC = MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter.Services
{
	/// <summary>
	/// PermissionCenterUpdateService 的摘要说明
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/", Description = "权限中心数据操作服务。")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
	// [System.Web.Script.Services.ScriptService]
	[XmlInclude(typeof(ClientSchemaObjectBase))]
	[XmlInclude(typeof(ClientSCBase))]
	[XmlInclude(typeof(ClientSCUser))]
	[XmlInclude(typeof(ClientSCOrganization))]
	[XmlInclude(typeof(ClientSCGroup))]
	[XmlInclude(typeof(ClientSCApplication))]
	[XmlInclude(typeof(ClientSCRole))]
	[XmlInclude(typeof(ClientSCPermission))]
	[XmlInclude(typeof(ClientConditionItem))]
	[XmlInclude(typeof(ClientAclItem))]
	[XmlInclude(typeof(ClientSchemaMember))]
	[XmlInclude(typeof(ClientSchemaRelation))]
	[XmlInclude(typeof(ClientSchemaObjectCollection))]
	public class PermissionCenterUpdateService : System.Web.Services.WebService
	{
		private PC.Executors.ISCObjectOperations Facade
		{
			[TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
			get
			{
				bool bLock = ((UpdatableRequestSoapMessage)UpdatableRequestSoapMessage.Current).WithLock;
				return bLock ? PC.Executors.SCObjectOperations.InstanceWithoutPermissions : PC.Executors.SCObjectOperations.InstanceWithoutPermissionsAndLockCheck;
			}
		}

		[TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
		private static void EnsureID(ClientSchemaObjectBase clientObject)
		{
			if (clientObject.ID.IsNullOrEmpty())
				clientObject.ID = UuidHelper.NewUuidString();
		}

		[WebMethod(Description = "生成一个新的GUID")]
		public string NewID()
		{
			return UuidHelper.NewUuidString();
		}

		[WebMethod(Description = "获取当前Facade的类型（测试用）")]
		public string GetFacadeType()
		{
			string result = "Unknown";

			if (this.Facade == PC.Executors.SCObjectOperations.InstanceWithoutPermissions)
				result = "InstanceWithoutPermissions";
			else if (this.Facade == PC.Executors.SCObjectOperations.InstanceWithoutPermissionsAndLockCheck)
				result = "InstanceWithoutPermissionsAndLockCheck";

			return result;
		}

		[WebMethod(Description = "将指定的对象发回，来检查对象是否一致。")]
		public ClientSchemaObjectBase Echo(ClientSchemaObjectBase clientObj, bool viaSCObject)
		{
			if (viaSCObject)
				return clientObj.ToSchemaObject().ToClientSchemaObject();
			else
				return clientObj;
		}

		[WebMethod(Description = "添加应用")]
		public ClientSchemaObjectBase AddApplication(ClientSCApplication clientApp)
		{
			EnsureID(clientApp);

			return Facade.AddApplication((PC.SCApplication)clientApp.ToSchemaObject(false)).ToClientSchemaObject();
		}

		[WebMethod(Description = "修改应用")]
		public ClientSchemaObjectBase UpdateApplication(ClientSCApplication clientApp)
		{
			return Facade.UpdateApplication((PC.SCApplication)clientApp.ToSchemaObject()).ToClientSchemaObject();
		}

		[WebMethod(Description = "删除应用")]
		public ClientSchemaObjectBase DeleteApplication(ClientSCApplication clientApp)
		{
			return Facade.DeleteApplication((PC.SCApplication)clientApp.ToSchemaObject()).ToClientSchemaObject();
		}

		[WebMethod(Description = "添加角色")]
		public ClientSchemaObjectBase AddRole(ClientSCRole clientRole, ClientSCApplication clientApp)
		{
			EnsureID(clientRole);

			return Facade.AddRole((PC.SCRole)clientRole.ToSchemaObject(), (PC.SCApplication)clientApp.ToSchemaObject()).ToClientSchemaObject();
		}

		[WebMethod(Description = "修改角色")]
		public ClientSchemaObjectBase UpdateRole(ClientSCRole clientRole)
		{
			return Facade.UpdateRole((PC.SCRole)clientRole.ToSchemaObject()).ToClientSchemaObject();
		}

		[WebMethod(Description = "删除角色")]
		public ClientSchemaObjectBase DeleteRole(ClientSCRole clientRole)
		{
			return Facade.DeleteRole((PC.SCRole)clientRole.ToSchemaObject()).ToClientSchemaObject();
		}

		[WebMethod(Description = "添加功能")]
		public ClientSchemaObjectBase AddPermission(ClientSCPermission clientPermission, ClientSCApplication clientApp)
		{
			EnsureID(clientPermission);
			return Facade.AddPermission((PC.SCPermission)clientPermission.ToSchemaObject(false), (PC.SCApplication)clientApp.ToSchemaObject()).ToClientSchemaObject();
		}

		[WebMethod(Description = "修改功能")]
		public ClientSchemaObjectBase UpdatePermission(ClientSCPermission clientPermission)
		{
			return Facade.UpdatePermission((PC.SCPermission)clientPermission.ToSchemaObject()).ToClientSchemaObject();
		}

		[WebMethod(Description = "删除功能")]
		public ClientSchemaObjectBase DeletePermission(ClientSCPermission clientPermission)
		{
			return Facade.DeletePermission((PC.SCPermission)clientPermission.ToSchemaObject()).ToClientSchemaObject();
		}

		[WebMethod(Description = "关联角色和功能")]
		public ClientSchemaRelation JoinRoleAndPermission(ClientSCRole clientRole, ClientSCPermission clientPermission)
		{
			return Facade.JoinRoleAndPermission((PC.SCRole)clientRole.ToSchemaObject(), (PC.SCPermission)clientPermission.ToSchemaObject()).ToClientSchemaObject();
		}

		[WebMethod(Description = "解除角色和功能关联")]
		public ClientSchemaRelation DisjoinRoleAndPermission(ClientSCRole clientRole, ClientSCPermission clientPermission)
		{
			return Facade.DisjoinRoleAndPermission((PC.SCRole)clientRole.ToSchemaObject(), (PC.SCPermission)clientPermission.ToSchemaObject()).ToClientSchemaObject();
		}

		[WebMethod(Description = "添加组织")]
		public ClientSchemaObjectBase AddOrganization(ClientSCOrganization clientOrganization, ClientSCOrganization clientParent)
		{
			EnsureID(clientOrganization);
			return Facade.AddOrganization((PC.SCOrganization)clientOrganization.ToSchemaObject(false), clientParent != null ? (PC.SCOrganization)clientParent.ToSchemaObject() : null).ToClientSchemaObject();
		}

		[WebMethod(Description = "修改组织")]
		public ClientSchemaObjectBase UpdateOrganization(ClientSCOrganization clientOrganization)
		{
			return Facade.UpdateOrganization((PC.SCOrganization)clientOrganization.ToSchemaObject()).ToClientSchemaObject();
		}

		[WebMethod(Description = "删除组织")]
		public ClientSchemaObjectBase DeleteOrganization(ClientSCOrganization clientOrganization, ClientSCOrganization clientParent)
		{
			return Facade.DeleteOrganization((PC.SCOrganization)clientOrganization.ToSchemaObject(), clientParent != null ? (PC.SCOrganization)clientParent.ToSchemaObject() : null, false).ToClientSchemaObject();
		}

		[WebMethod(Description = "添加用户")]
		public ClientSchemaObjectBase AddUser(ClientSCUser clientUser, ClientSCOrganization clientParent)
		{
			EnsureID(clientUser);
			return Facade.AddUser((PC.SCUser)clientUser.ToSchemaObject(false), clientParent != null ? (PC.SCOrganization)clientParent.ToSchemaObject() : null).ToClientSchemaObject();
		}

		[WebMethod(Description = "修改用户")]
		public ClientSchemaObjectBase UpdateUser(ClientSCUser clientUser)
		{
			return Facade.UpdateUser((PC.SCUser)clientUser.ToSchemaObject()).ToClientSchemaObject();
		}

		[WebMethod(Description = "删除用户")]
		public ClientSchemaObjectBase DeleteUser(ClientSCUser clientUser, ClientSCOrganization clientParent)
		{
			return Facade.DeleteUser((PC.SCUser)clientUser.ToSchemaObject(), clientParent != null ? (PC.SCOrganization)clientParent.ToSchemaObject() : null, false).ToClientSchemaObject();
		}

		[WebMethod(Description = "添加群组")]
		public ClientSchemaObjectBase AddGroup(ClientSCGroup clientGroup, ClientSCOrganization clientParent)
		{
			EnsureID(clientGroup);
			return Facade.AddGroup((PC.SCGroup)clientGroup.ToSchemaObject(false), clientParent != null ? (PC.SCOrganization)clientParent.ToSchemaObject() : null).ToClientSchemaObject();
		}

		[WebMethod(Description = "修改群组")]
		public ClientSchemaObjectBase UpdateGroup(ClientSCGroup clientGroup)
		{
			return Facade.UpdateGroup((PC.SCGroup)clientGroup.ToSchemaObject()).ToClientSchemaObject();
		}

		[WebMethod(Description = "删除群组")]
		public ClientSchemaObjectBase DeleteGroup(ClientSCGroup clientGroup, ClientSCOrganization clientParent)
		{
			return Facade.DeleteGroup((PC.SCGroup)clientGroup.ToSchemaObject(), clientParent != null ? (PC.SCOrganization)clientParent.ToSchemaObject() : null, false).ToClientSchemaObject();
		}

		[WebMethod(Description = "改变所有者")]
		public ClientSchemaObjectBase ChangeOwner(ClientSCBase clientObject, ClientSCOrganization clientParent)
		{
			return Facade.ChangeOwner((PC.SCBase)clientObject.ToSchemaObject(), (PC.SCOrganization)clientParent.ToSchemaObject()).ToClientSchemaObject();
		}

		[WebMethod(Description = "递归删除")]
		public ClientSCOrganization DeleteObjectsRecursively(ClientSchemaObjectCollection clientObjects, ClientSCOrganization clientParent)
		{
			return (ClientSCOrganization)Facade.DeleteObjectsRecursively(clientObjects.ToSchemaObjectCollection(), (PC.SCOrganization)clientParent.ToSchemaObject()).ToClientSCBaseObject();
		}

		[WebMethod(Description = "移动组织机构对象")]
		public ClientSchemaRelation MoveObjectToOrganization(ClientSCOrganization clientSource, ClientSCBase clientScObject, ClientSCOrganization clientTarget)
		{
			return (ClientSchemaRelation)Facade.MoveObjectToOrganization((PC.SCOrganization)clientSource.ToSchemaObject(), (PC.SCBase)clientScObject.ToSchemaObject(), (PC.SCOrganization)clientTarget.ToSchemaObject()).ToClientSchemaObject();
		}

		[WebMethod(Description = "向群组添加用户")]
		public ClientSchemaMember AddUserToGroup(ClientSCUser clientUser, ClientSCGroup clientGroup)
		{
			return (ClientSchemaMember)Facade.AddUserToGroup((PC.SCUser)clientUser.ToSchemaObject(), (PC.SCGroup)clientGroup.ToSchemaObject()).ToClientSchemaObject();
		}

		[WebMethod(Description = "向用户添加秘书")]
		public ClientSchemaMember AddSecretaryToUser(ClientSCUser clientSecretary, ClientSCUser clientUser)
		{
			return (ClientSchemaMember)Facade.AddSecretaryToUser((PC.SCUser)clientSecretary.ToSchemaObject(), (PC.SCUser)clientUser.ToSchemaObject()).ToClientSchemaObject();
		}

		[WebMethod(Description = "向组角色添加成员")]
		public ClientSchemaMember AddMemberToRole(ClientSCBase clientObject, ClientSCRole clientRole)
		{
			return (ClientSchemaMember)Facade.AddMemberToRole((PC.SCBase)clientObject.ToSchemaObject(), (PC.SCRole)clientRole.ToSchemaObject()).ToClientSchemaObject();
		}

		[WebMethod(Description = "从群组移除用户")]
		public ClientSchemaMember RemoveUserFromGroup(ClientSCUser clientUser, ClientSCGroup clientGroup)
		{
			return (ClientSchemaMember)Facade.RemoveUserFromGroup((PC.SCUser)clientUser.ToSchemaObject(), (PC.SCGroup)clientGroup.ToSchemaObject()).ToClientSchemaObject();
		}

		[WebMethod(Description = "从用户移除秘书")]
		public ClientSchemaMember RemoveSecretaryFromUser(ClientSCUser clientSecretary, ClientSCUser clientUser)
		{
			return (ClientSchemaMember)Facade.RemoveSecretaryFromUser((PC.SCUser)clientSecretary.ToSchemaObject(), (PC.SCUser)clientUser.ToSchemaObject()).ToClientSchemaObject();
		}

		[WebMethod(Description = "从角色移除成员")]
		public ClientSchemaMember RemoveMemberFromRole(ClientSCBase clientObject, ClientSCRole clientRole)
		{
			return (ClientSchemaMember)Facade.RemoveMemberFromRole((PC.SCBase)clientObject.ToSchemaObject(), (PC.SCRole)clientRole.ToSchemaObject()).ToClientSchemaObject();
		}

		[WebMethod(Description = "设置用户缺省组织")]
		public ClientSchemaRelation SetUserDefaultOrganization(ClientSCUser clientUser, ClientSCOrganization clientParent)
		{
			return (ClientSchemaRelation)Facade.SetUserDefaultOrganization((PC.SCUser)clientUser.ToSchemaObject(), (PC.SCOrganization)clientParent.ToSchemaObject()).ToClientSchemaObject();
		}

		[WebMethod(Description = "更新对象ACL")]
		public void UpdateObjectAcl(string ownerID, ClientAclItem[] clientAcls)
		{
			var owner = PC.Adapters.SchemaObjectAdapter.Instance.Load(ownerID);

			if (owner == null || owner.Status != SchemaObjectStatus.Normal)
				throw new InvalidOperationException("指定对象不存在或已删除");

			PC.Permissions.SCAclContainer container = new PC.Permissions.SCAclContainer(owner);

			foreach (ClientAclItem item in clientAcls)
			{
				if (item.Status == ClientSchemaObjectStatus.Normal)
					container.Members.Add(item.ToSCAcl());
			}

			container.Members.MergeChangedItems(PC.Adapters.SCAclAdapter.Instance.LoadByContainerID(ownerID, DateTime.MinValue));

			Facade.UpdateObjectAcl(container);
		}

		[WebMethod(Description = "替换子对象ACL设置为当前对象ACL")]
		public void ReplaceAclRecursively(string ownerID, bool force)
		{
			var owner = PC.Adapters.SchemaObjectAdapter.Instance.Load(ownerID);
			if (owner == null || owner.Status != SchemaObjectStatus.Normal)
				throw new InvalidOperationException("未能根据ownerID检索到有效对象");

			if (owner is PC.ISCAclContainer)
			{
				Facade.ReplaceAclRecursively((PC.ISCAclContainer)owner, force);
			}
			else
			{
				throw new InvalidOperationException("ownerID指定的对象不实现ISAclContainer");
			}
		}

		[WebMethod(Description = "更新群组条件")]
		public void UpdateGroupConditions(string ownerID, string conditionType, ClientConditionItem[] items)
		{
			PC.Conditions.SCConditionOwner owner = PC.Adapters.SCConditionAdapter.Instance.Load(ownerID, conditionType) ?? new PC.Conditions.SCConditionOwner() { OwnerID = ownerID, Type = conditionType };

			PC.Conditions.SCConditionCollection conditions = new PC.Conditions.SCConditionCollection();

			foreach (ClientConditionItem item in items)
			{
				if (item.OwnerID != ownerID)
					throw new InvalidOperationException("不一致的OwnerID:" + item.OwnerID);
				else if (item.Status == ClientSchemaObjectStatus.Normal)
				{
					conditions.Add(new PC.Conditions.SCCondition()
					{
						OwnerID = ownerID,
						Status = SchemaObjectStatus.Normal,
						Description = item.Description ?? string.Empty,
						Condition = item.Condition,
						SortID = item.SortID,
						Type = item.Type,
						VersionEndTime = item.VersionEndTime,
						VersionStartTime = item.VersionStartTime
					});
				}
			}

			owner.Conditions.ReplaceItemsWith(conditions, ownerID, conditionType);

			Facade.UpdateGroupConditions(owner);
		}

		[WebMethod(Description = "更新角色条件")]
		public void UpdateRoleConditions(string ownerID, string conditionType, ClientConditionItem[] items)
		{
			PC.Conditions.SCConditionOwner owner = PC.Adapters.SCConditionAdapter.Instance.Load(ownerID, conditionType) ?? new PC.Conditions.SCConditionOwner() { OwnerID = ownerID, Type = conditionType };

			PC.Conditions.SCConditionCollection conditions = new PC.Conditions.SCConditionCollection();

			foreach (ClientConditionItem item in items)
			{
				if (item.OwnerID != ownerID)
					throw new InvalidOperationException("不一致的OwnerID:" + item.OwnerID);
				else if (item.Status == ClientSchemaObjectStatus.Normal)
				{
					conditions.Add(new PC.Conditions.SCCondition()
					{
						OwnerID = ownerID,
						Status = SchemaObjectStatus.Normal,
						Description = item.Description ?? string.Empty,
						Condition = item.Condition,
						SortID = item.SortID,
						Type = item.Type,
						VersionEndTime = item.VersionEndTime,
						VersionStartTime = item.VersionStartTime
					});
				}
			}

			owner.Conditions.ReplaceItemsWith(conditions, ownerID, conditionType);

			Facade.UpdateRoleConditions(owner);
		}

		[WebMethod(Description = "更新照片属性")]
		public ClientSchemaObjectBase UpdateObjectImageProperty(ClientSchemaObjectBase obj, string propertyName, string imageID)
		{
			var image = MCS.Library.SOA.DataObjects.ImagePropertyAdapter.Instance.Load(imageID);
			if (image == null) throw new ApplicationException("图像未找到");

			return Facade.UpdateObjectImageProperty(obj.ToSchemaObject(), propertyName, image).ToClientSchemaObject();
		}

	}
}
