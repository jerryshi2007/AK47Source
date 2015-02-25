using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Configuration;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Library.SOA.DataObjects.Security.Test.SchemaObject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Test.Permissions
{
	[TestClass]
	public class PermissionSetTest
	{
		[TestMethod]
		[TestCategory(Constants.PermissionSetCategory)]
		[Description("Acl中权限定义的测试")]
		public void AclPermissionsDefineTest()
		{
			ObjectSchemaConfigurationElement element = ObjectSchemaSettings.GetConfig().Schemas["Applications"];

			Assert.IsNotNull(element);

			SCAclPermissionItemCollection permissionDefine = new SCAclPermissionItemCollection(element.PermissionSet);

			Console.WriteLine("Application permissions:");
			permissionDefine.ForEach(pd => pd.Output(Console.Out));

			element = ObjectSchemaSettings.GetConfig().Schemas["Organizations"];

			Assert.IsNotNull(element);

			permissionDefine = new SCAclPermissionItemCollection(element.PermissionSet);

			Console.WriteLine("Organization permissions:");
			permissionDefine.ForEach(pd => pd.Output(Console.Out));
		}

		[TestMethod]
		[TestCategory(Constants.PermissionSetCategory)]
		[Description("在组织上添加权限的测试")]
		public void AddAclPermissionsTest()
		{
			//准备组织数据
			SCOrganization organization = SCObjectGenerator.PrepareOrganizationObject();

			SCObjectOperations.Instance.AddOrganization(organization, SCOrganization.GetRoot());

			//准备应用
			SCApplication application = SCObjectGenerator.PrepareApplicationObject();

			SCObjectOperations.Instance.AddApplication(application);

			SCRole role1 = SCObjectGenerator.PrepareRoleObject();

			//准备角色
			SCObjectOperations.Instance.AddRole(role1, application);

			//准备人员
			SCUser user1 = SCObjectGenerator.PrepareUserObject("RU1", "User1", "RoleUser1");
			SCObjectOperations.Instance.AddUser(user1, organization);

			//将人员添加到角色
			SCObjectOperations.Instance.AddMemberToRole(user1, role1);

			SCRole role2 = SCObjectGenerator.PrepareRoleObject();

			//准备角色
			SCObjectOperations.Instance.AddRole(role2, application);

			//准备人员
			SCUser user2 = SCObjectGenerator.PrepareUserObject("RU1", "User1", "RoleUser1");
			SCObjectOperations.Instance.AddUser(user2, organization);

			//将人员添加到角色
			SCObjectOperations.Instance.AddMemberToRole(user2, role2);

			//准备Container
			SCAclContainer container = new SCAclContainer(organization);

			container.Members.Add("AddChildren", role1);
			container.Members.Add("AddChildren", role2);

			Console.WriteLine("Container ID: {0}", container.ContainerID);

			SCObjectOperations.Instance.UpdateObjectAcl(container);

			SCAclMemberCollection members = SCAclAdapter.Instance.LoadByContainerID(organization.ID, DateTime.MinValue);

			Assert.IsTrue(members.ContainsKey("AddChildren", role1.ID));
			Assert.AreEqual(SchemaObjectStatus.Normal, members["AddChildren", role1.ID].Status);

			Assert.IsTrue(members.ContainsKey("AddChildren", role2.ID));
			Assert.AreEqual(SchemaObjectStatus.Normal, members["AddChildren", role2.ID].Status);
		}

		[TestMethod]
		[TestCategory(Constants.PermissionSetCategory)]
		[Description("继承权限的测试")]
		public void InheritAclPermissionsTest()
		{
			//准备组织数据
			SCOrganization parent = SCObjectGenerator.PrepareOrganizationObject();

			SCObjectOperations.Instance.AddOrganization(parent, SCOrganization.GetRoot());

			//准备应用
			SCApplication application = SCObjectGenerator.PrepareApplicationObject();

			SCObjectOperations.Instance.AddApplication(application);

			SCRole role1 = SCObjectGenerator.PrepareRoleObject();

			//准备角色
			SCObjectOperations.Instance.AddRole(role1, application);

			//准备人员
			SCUser user1 = SCObjectGenerator.PrepareUserObject("RU1", "User1", "RoleUser1");
			SCObjectOperations.Instance.AddUser(user1, parent);

			//将人员添加到角色
			SCObjectOperations.Instance.AddMemberToRole(user1, role1);

			SCRole role2 = SCObjectGenerator.PrepareRoleObject();

			//准备角色
			SCObjectOperations.Instance.AddRole(role2, application);

			//准备人员
			SCUser user2 = SCObjectGenerator.PrepareUserObject("RU1", "User1", "RoleUser1");
			SCObjectOperations.Instance.AddUser(user2, parent);

			//将人员添加到角色
			SCObjectOperations.Instance.AddMemberToRole(user2, role2);

			//准备Container
			SCAclContainer container = new SCAclContainer(parent);

			container.Members.Add("AddChildren", role1);
			container.Members.Add("AddChildren", role2);

			SCObjectOperations.Instance.UpdateObjectAcl(container);

			SCOrganization organization = SCObjectGenerator.PrepareOrganizationObject();

			SCObjectOperations.Instance.AddOrganization(organization, parent);

			SCAclMemberCollection members = SCAclAdapter.Instance.LoadByContainerID(organization.ID, DateTime.MinValue);

			Assert.IsTrue(members.ContainsKey("AddChildren", role1.ID));
			Assert.AreEqual(SchemaObjectStatus.Normal, members["AddChildren", role1.ID].Status);

			Assert.IsTrue(members.ContainsKey("AddChildren", role2.ID));
			Assert.AreEqual(SchemaObjectStatus.Normal, members["AddChildren", role2.ID].Status);
		}

		[TestMethod]
		[TestCategory(Constants.PermissionSetCategory)]
		[Description("删除Acl容器的测试")]
		public void DeleteAclContainerTest()
		{
			//准备组织数据
			SCOrganization organization = SCObjectGenerator.PrepareOrganizationObject();

			SCObjectOperations.Instance.AddOrganization(organization, SCOrganization.GetRoot());

			//准备应用
			SCApplication application = SCObjectGenerator.PrepareApplicationObject();

			SCObjectOperations.Instance.AddApplication(application);

			SCRole role1 = SCObjectGenerator.PrepareRoleObject();

			//准备角色
			SCObjectOperations.Instance.AddRole(role1, application);

			//准备人员
			SCUser user1 = SCObjectGenerator.PrepareUserObject("RU1", "User1", "RoleUser1");
			SCObjectOperations.Instance.AddUser(user1, organization);

			//将人员添加到角色
			SCObjectOperations.Instance.AddMemberToRole(user1, role1);

			SCRole role2 = SCObjectGenerator.PrepareRoleObject();

			//准备角色
			SCObjectOperations.Instance.AddRole(role2, application);

			//准备人员
			SCUser user2 = SCObjectGenerator.PrepareUserObject("RU1", "User1", "RoleUser1");
			SCObjectOperations.Instance.AddUser(user2, organization);

			//将人员添加到角色
			SCObjectOperations.Instance.AddMemberToRole(user2, role2);

			//准备Container
			SCAclContainer container = new SCAclContainer(organization);

			container.Members.Add("AddChildren", role1);
			container.Members.Add("AddChildren", role2);

			SCObjectOperations.Instance.UpdateObjectAcl(container);

			Console.WriteLine("ContainerID: {0}", container.ContainerID);

			SCObjectOperations.Instance.DeleteOrganization(organization, SCOrganization.GetRoot(), false);

			SCAclMemberCollection members = SCAclAdapter.Instance.LoadByContainerID(organization.ID, DateTime.MinValue);

			Assert.IsFalse(members.ContainsKey("AddChildren", role1.ID));

			Assert.IsFalse(members.ContainsKey("AddChildren", role2.ID));
		}

		[TestMethod]
		[TestCategory(Constants.PermissionSetCategory)]
		[Description("删除Acl成员的测试")]
		public void DeleteAclMemberTest()
		{
			//准备组织数据
			SCOrganization organization = SCObjectGenerator.PrepareOrganizationObject();

			SCObjectOperations.Instance.AddOrganization(organization, SCOrganization.GetRoot());

			//准备应用
			SCApplication application = SCObjectGenerator.PrepareApplicationObject();

			SCObjectOperations.Instance.AddApplication(application);

			SCRole role1 = SCObjectGenerator.PrepareRoleObject();

			//准备角色
			SCObjectOperations.Instance.AddRole(role1, application);

			//准备人员
			SCUser user1 = SCObjectGenerator.PrepareUserObject("RU1", "User1", "RoleUser1");
			SCObjectOperations.Instance.AddUser(user1, organization);

			//将人员添加到角色
			SCObjectOperations.Instance.AddMemberToRole(user1, role1);

			SCRole role2 = SCObjectGenerator.PrepareRoleObject();

			//准备角色
			SCObjectOperations.Instance.AddRole(role2, application);

			//准备人员
			SCUser user2 = SCObjectGenerator.PrepareUserObject("RU1", "User1", "RoleUser1");
			SCObjectOperations.Instance.AddUser(user2, organization);

			//将人员添加到角色
			SCObjectOperations.Instance.AddMemberToRole(user2, role2);

			//准备Container
			SCAclContainer container = new SCAclContainer(organization);

			container.Members.Add("AddChildren", role1);
			container.Members.Add("AddChildren", role2);

			SCObjectOperations.Instance.UpdateObjectAcl(container);

			Console.WriteLine("ContainerID: {0}", container.ContainerID);

			SCObjectOperations.Instance.DeleteRole(role1);
			SCObjectOperations.Instance.DeleteRole(role2);

			SCAclMemberCollection members = SCAclAdapter.Instance.LoadByContainerID(organization.ID, DateTime.MinValue);

			Assert.IsFalse(members.ContainsKey("AddChildren", role1.ID));
			Assert.IsFalse(members.ContainsKey("AddChildren", role2.ID));
		}

		[TestMethod]
		[TestCategory(Constants.PermissionSetCategory)]
		[Description("清除Acl成员的测试")]
		public void ClearAclMembersTest()
		{
			//准备组织数据
			SCOrganization organization = SCObjectGenerator.PrepareOrganizationObject();

			SCObjectOperations.Instance.AddOrganization(organization, SCOrganization.GetRoot());

			//准备应用
			SCApplication application = SCObjectGenerator.PrepareApplicationObject();

			SCObjectOperations.Instance.AddApplication(application);

			SCRole role1 = SCObjectGenerator.PrepareRoleObject();

			//准备角色
			SCObjectOperations.Instance.AddRole(role1, application);

			//准备人员
			SCUser user1 = SCObjectGenerator.PrepareUserObject("RU1", "User1", "RoleUser1");
			SCObjectOperations.Instance.AddUser(user1, organization);

			//将人员添加到角色
			SCObjectOperations.Instance.AddMemberToRole(user1, role1);

			SCRole role2 = SCObjectGenerator.PrepareRoleObject();

			//准备角色
			SCObjectOperations.Instance.AddRole(role2, application);

			//准备人员
			SCUser user2 = SCObjectGenerator.PrepareUserObject("RU1", "User1", "RoleUser1");
			SCObjectOperations.Instance.AddUser(user2, organization);

			//将人员添加到角色
			SCObjectOperations.Instance.AddMemberToRole(user2, role2);

			//准备Container
			SCAclContainer container = new SCAclContainer(organization);

			container.Members.Add("AddChildren", role1);
			container.Members.Add("AddChildren", role2);

			SCObjectOperations.Instance.UpdateObjectAcl(container);

			Console.WriteLine("ContainerID: {0}", container.ContainerID);

			//清空Members
			container.Members.Clear();

			SCAclMemberCollection originalMembers = SCAclAdapter.Instance.LoadByContainerID(organization.ID, DateTime.MinValue);

			Assert.IsTrue(container.Members.MergeChangedItems(originalMembers));

			SCObjectOperations.Instance.UpdateObjectAcl(container);

			SCAclMemberCollection members = SCAclAdapter.Instance.LoadByContainerID(organization.ID, DateTime.MinValue);

			Assert.IsFalse(members.ContainsKey("AddChildren", role1.ID));
			Assert.IsFalse(members.ContainsKey("AddChildren", role2.ID));
		}
	}
}
