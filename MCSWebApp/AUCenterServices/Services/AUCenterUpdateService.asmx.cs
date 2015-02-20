using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MCS.Library.SOA.DataObjects.Security.AUClient;
using MCS.Library.SOA.DataObjects.Schemas.Client;
using System.Xml.Serialization;
using System.Runtime;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Adapters;

namespace AUCenterServices.Services
{
	/// <summary>
	/// Summary description for AUCenterUpdateService
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	[XmlInclude(typeof(ClientSchemaObjectBase))]
	[XmlInclude(typeof(ClientGenericObject))]
	[XmlInclude(typeof(ClientAdminUnit))]
	[XmlInclude(typeof(ClientAUAdminScope))]
	[XmlInclude(typeof(ClientAUAdminScopeItem))]
	[XmlInclude(typeof(ClientAURole))]
	[XmlInclude(typeof(ClientAURoleDisplayItem))]
	[XmlInclude(typeof(ClientAUSchema))]
	[XmlInclude(typeof(ClientAUSchemaRole))]
	[XmlInclude(typeof(ClientNamedObject))]
	[XmlInclude(typeof(ClientConditionItem))]
	[XmlInclude(typeof(ClientAclItem))]
	public class AUCenterUpdateService : System.Web.Services.WebService, IAUCenterUpdateService
	{
		public static void CheckIDProvided(ClientSchemaObjectBase obj)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");

			if (string.IsNullOrEmpty(obj.ID))
				throw new ArgumentException("对象的ID未提供有效值");
		}

		private MCS.Library.SOA.DataObjects.Security.AUObjects.Operations.IFacade Facade
		{
			[TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
			get
			{
				bool bLock = ((UpdatableRequestSoapMessage)UpdatableRequestSoapMessage.Current).WithLock;
				return bLock ? MCS.Library.SOA.DataObjects.Security.AUObjects.Operations.Facade.DefaultInstance : MCS.Library.SOA.DataObjects.Security.AUObjects.Operations.Facade.InstanceWithoutPermissions;
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

			if (((UpdatableRequestSoapMessage)UpdatableRequestSoapMessage.Current).WithLock)
				result = "WithLock";
			else
				result = "WithoutLock";

			return result;
		}

		[WebMethod(Description = "添加管理架构")]
		public void AddAdminSchema(ClientAUSchema schema)
		{
			EnsureID(schema);

			this.Facade.AddAdminSchema((AUSchema)schema.ToSchemaObject(false));
		}

		[WebMethod(Description = "添加管理脚骨角色")]
		public void AddAdminSchemaRole(ClientAUSchemaRole role, ClientAUSchema schema)
		{
			CheckIDProvided(schema);
			EnsureID(role);
			this.Facade.AddAdminSchemaRole((AUSchemaRole)role.ToSchemaObject(false), (AUSchema)schema.ToSchemaObject(true));
		}

		[WebMethod(Description = "删除管理架构角色")]
		public void DeleteAdminSchemaRole(ClientAUSchemaRole role)
		{
			CheckIDProvided(role);
			this.Facade.DeleteAdminSchemaRole((AUSchemaRole)role.ToSchemaObject(true));
		}

		[WebMethod(Description = "更新管理架构角色")]
		public void UpdateAdminSchemaRole(ClientAUSchemaRole role)
		{
			CheckIDProvided(role);
			this.Facade.UpdateAdminSchemaRole((AUSchemaRole)role.ToSchemaObject(true));
		}

		[WebMethod(Description = "添加管理单元")]
		public void AddAdminUnit(ClientAdminUnit unit, ClientAdminUnit parent)
		{
			EnsureID(unit);

			if (parent != null)
				CheckIDProvided(parent);

			AdminUnit auParent = parent != null ? (AdminUnit)parent.ToSchemaObject(true) : null;
			this.Facade.AddAdminUnit((AdminUnit)unit.ToSchemaObject(false), auParent);
		}

		[WebMethod(Description = "添加管理单元和成员")]
		public void AddAdminUnitWithMembers(ClientAdminUnit unit, ClientAdminUnit parent, ClientAURole[] roles, ClientAUAdminScope[] scopes)
		{
			roles.NullCheck("roles");
			scopes.NullCheck("scopes");
			CheckIDProvided(unit);

			AURole[] auRoles = new AURole[roles.Length];
			for (int i = roles.Length - 1; i >= 0; i--)
				auRoles[i] = (AURole)roles[i].ToSchemaObject();

			AUAdminScope[] auScopes = new AUAdminScope[scopes.Length];
			for (int i = scopes.Length - 1; i >= 0; i--)
				auScopes[i] = (AUAdminScope)scopes[i].ToSchemaObject();

			EnsureID(unit);
			AdminUnit auParent = parent != null ? (AdminUnit)parent.ToSchemaObject(true) : null;

			this.Facade.AddAdminUnitWithMembers((AdminUnit)unit.ToSchemaObject(false), auParent, auRoles, auScopes);
		}

		[WebMethod(Description = "移动管理单元")]
		public void MoveAdminUnit(ClientAdminUnit unit, ClientAdminUnit newParent)
		{
			CheckIDProvided(unit);
			CheckIDProvided(newParent);
			this.Facade.MoveAdminUnit((AdminUnit)unit.ToSchemaObject(true), (AdminUnit)newParent.ToSchemaObject(true));
		}

		[WebMethod(Description = "更新管理单元")]
		public void UpdateAdminUnit(ClientAdminUnit unit)
		{
			CheckIDProvided(unit);
			this.Facade.UpdateAdminUnit((AdminUnit)unit.ToSchemaObject(true));
		}

		[WebMethod(Description = "删除管理单元")]
		public void DeleteAdminUnit(ClientAdminUnit unit)
		{
			CheckIDProvided(unit);
			this.Facade.DeleteAdminUnit((AdminUnit)unit.ToSchemaObject(true));
		}

		[WebMethod(Description = "添加管理范围的对象")]
		public void AddObjectToScope(ClientAUAdminScopeItem item, ClientAUAdminScope scope)
		{
			CheckIDProvided(item);
			CheckIDProvided(scope);
			this.Facade.AddObjectToScope((AUAdminScopeItem)item.ToSchemaObject(), (AUAdminScope)scope.ToSchemaObject());
		}

		[WebMethod(Description = "从管理范围移除对象")]
		public void RemoveObjectFromScope(ClientAUAdminScopeItem item, ClientAUAdminScope scope)
		{
			CheckIDProvided(item);
			CheckIDProvided(scope);
			this.Facade.RemoveObjectFromScope((AUAdminScopeItem)item.ToSchemaObject(), (AUAdminScope)scope.ToSchemaObject());
		}

		[WebMethod(Description = "向角色添加用户")]
		public void AddUserToRole(ClientGenericObject user, ClientAdminUnit unit, ClientAUSchemaRole role)
		{
			user.NullCheck("user"); unit.NullCheck("unit"); role.NullCheck("role");
			CheckIDProvided(user); CheckIDProvided(unit); CheckIDProvided(role);

			if (string.IsNullOrEmpty(user.ID))
				throw new ArgumentException("user的ID必须不为null", "user");

			SCUser scUser = new SCUser()
			{
				ID = user.ID,
				Name = user.Properties.GetValue("Name", string.Empty),
				CodeName = user.Properties.GetValue("CodeName", string.Empty)
			};

			this.Facade.AddUserToRole(scUser, (AdminUnit)unit.ToSchemaObject(), (AUSchemaRole)role.ToSchemaObject());
		}

		[WebMethod(Description = "从角色移除用户")]
		public void RemoveUserFromRole(ClientGenericObject user, ClientAdminUnit unit, ClientAUSchemaRole role)
		{
			user.NullCheck("user"); unit.NullCheck("unit"); role.NullCheck("role");
			CheckIDProvided(user); CheckIDProvided(unit); CheckIDProvided(role);

			SCUser scUser = new SCUser()
			{
				ID = user.ID,
				Name = user.Properties.GetValue("Name", string.Empty),
				CodeName = user.Properties.GetValue("CodeName", string.Empty)
			};

			this.Facade.RemoveUserFromRole(scUser, (AdminUnit)unit.ToSchemaObject(), (AUSchemaRole)role.ToSchemaObject());
		}

		[WebMethod(Description = "替换角色中的用户")]
		public void ReplaceUsersInRole(ClientGenericObject[] users, ClientAdminUnit unit, ClientAUSchemaRole role)
		{
			users.NullCheck("users"); unit.NullCheck("unit"); role.NullCheck("role");
			 CheckIDProvided(unit); CheckIDProvided(role);
			SCUser[] scUsers = new SCUser[users.Length];
			for (int i = users.Length - 1; i >= 0; i--)
				scUsers[i] = (SCUser)users[i].ToSchemaObject();

			this.Facade.ReplaceUsersInRole(scUsers, (AdminUnit)unit.ToSchemaObject(), (AUSchemaRole)role.ToSchemaObject());
		}

		[WebMethod(Description = "更新管理范围的条件")]
		public void UpdateScopeCondition(ClientAUAdminScope scope, ClientConditionItem condition)
		{
			CheckIDProvided(scope); 
			this.Facade.UpdateScopeCondition((AUAdminScope)scope.ToSchemaObject(), new MCS.Library.SOA.DataObjects.Security.Conditions.SCCondition()
			{
				Condition = condition.Condition,
				Description = condition.Description,
				OwnerID = condition.OwnerID,
				SortID = condition.SortID,
				Status = (SchemaObjectStatus)condition.Status,
				Type = condition.Type,
				VersionEndTime = condition.VersionEndTime,
				VersionStartTime = condition.VersionStartTime
			});
		}

		[WebMethod(Description = "更新对象ACL")]
		public void UpdateObjectAcl(string ownerID, ClientAclItem[] clientAcls)
		{
			var owner = AUCommon.DoDbProcess(() => SchemaObjectAdapter.Instance.Load(ownerID));

			if (owner == null || owner.Status != SchemaObjectStatus.Normal)
				throw new InvalidOperationException("指定对象不存在或已删除");

			SCAclContainer container = new SCAclContainer(owner);

			foreach (ClientAclItem item in clientAcls)
			{
				if (item.Status == ClientSchemaObjectStatus.Normal)
					container.Members.Add(item.ToSCAcl());
			}

			container.Members.MergeChangedItems(AUAclAdapter.Instance.LoadByContainerID(ownerID, DateTime.MinValue));

			this.Facade.UpdateObjectAcl(container);
		}

		[WebMethod(Description = "替换子对象ACL设置为当前对象ACL")]
		public void ReplaceAclRecursively(string ownerID, bool force)
		{
			var owner = AUCommon.DoDbProcess(() => SchemaObjectAdapter.Instance.Load(ownerID));
			if (owner == null || owner.Status != SchemaObjectStatus.Normal)
				throw new InvalidOperationException("未能根据ownerID检索到有效对象");

			if (owner is ISCAclContainer)
			{
				Facade.ReplaceAclRecursively((ISCAclContainer)owner, force);
			}
			else
			{
				throw new InvalidOperationException("ownerID指定的对象不实现ISAclContainer");
			}
		}
	}
}
