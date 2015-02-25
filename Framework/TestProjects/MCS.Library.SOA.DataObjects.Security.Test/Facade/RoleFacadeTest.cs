using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Security.Test.SchemaObject;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Library.SOA.DataObjects.Security.Conditions;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Test.Facade
{
	[TestClass]
	public class RoleFacadeTest : AclBasedTestBase
	{
		[ClassInitialize()]
		public static void Init(TestContext context)
		{
			ReGenOUData();
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void AddRoleTest()
		{
			Trace.CorrelationManager.ActivityId = UuidHelper.NewUuid();

			SCApplication application = SCObjectGenerator.PrepareApplicationObject();

			SCObjectOperations.Instance.AddApplication(application);

			SCRole role = SCObjectGenerator.PrepareRoleObject();

			SCObjectOperations.Instance.AddRole(role, application);

			application.CurrentRoles.ContainsKey(role.ID);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void AddMemberToRoleTest()
		{
			SCApplication application = SCObjectGenerator.PrepareApplicationObject();

			SCObjectOperations.Instance.AddApplication(application);

			SCRole role = SCObjectGenerator.PrepareRoleObject();

			SCObjectOperations.Instance.AddRole(role, application);

			SCUser user1 = SCObjectGenerator.PrepareUserObject("RU1", "User1", "RoleUser1");
			SCUser user2 = SCObjectGenerator.PrepareUserObject("RU2", "User2", "RoleUser2");

			SCObjectOperations.Instance.AddUser(user1, SCOrganization.GetRoot());
			SCObjectOperations.Instance.AddUser(user2, SCOrganization.GetRoot());

			SCObjectOperations.Instance.AddMemberToRole(user1, role);
			SCObjectOperations.Instance.AddMemberToRole(user2, role);

			Assert.AreEqual(2, role.CurrentMembers.Count);
			Assert.IsTrue(role.CurrentMembers.ContainsKey(user1.ID));
			Assert.IsTrue(role.CurrentMembers.ContainsKey(user2.ID));

			Assert.IsTrue(user1.CurrentRoles.ContainsKey(role.ID));
			Assert.IsTrue(user2.CurrentRoles.ContainsKey(role.ID));
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void DeleteRoleTest()
		{
			SCApplication application = SCObjectGenerator.PrepareApplicationObject();

			SCObjectOperations.Instance.AddApplication(application);

			SCRole role = SCObjectGenerator.PrepareRoleObject();

			SCObjectOperations.Instance.AddRole(role, application);

			Console.WriteLine("RoleID: {0}", role.ID);

			SCUser user1 = SCObjectGenerator.PrepareUserObject("RU1", "User1", "RoleUser1");
			SCUser user2 = SCObjectGenerator.PrepareUserObject("RU2", "User2", "RoleUser2");

			SCObjectOperations.Instance.AddUser(user1, SCOrganization.GetRoot());
			SCObjectOperations.Instance.AddUser(user2, SCOrganization.GetRoot());

			SCObjectOperations.Instance.AddMemberToRole(user1, role);
			SCObjectOperations.Instance.AddMemberToRole(user2, role);

			SCObjectAndRelationCollection relations = SCSnapshotAdapter.Instance.QueryRolesContainsMembers(new string[] { "Roles" }, new string[] { role.ID }, false, DateTime.MinValue);

			Assert.AreEqual(role.CurrentMembers.Count, relations.Count);

			SCObjectOperations.Instance.DeleteRole(role);

			Assert.AreEqual(0, application.CurrentRoles.Count);

			Assert.AreEqual(0, user1.CurrentRoles.Count);
			Assert.AreEqual(0, user2.CurrentRoles.Count);

			SCObjectAndRelationCollection relationDeleted = SCSnapshotAdapter.Instance.QueryRolesContainsMembers(new string[] { "Roles" }, new string[] { role.ID }, false, DateTime.MinValue);
			Assert.AreEqual(role.CurrentMembers.Count, relationDeleted.Count);

			SameContainerUserAndContainerSnapshotCollection containsUsers = UserAndContainerSnapshotAdapter.Instance.LoadByContainerID(role.ID);

			containsUsers.ForEach(u => Assert.AreEqual(SchemaObjectStatus.Deleted, u.Status));
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void DeleteRoleMemberTest()
		{
			SCApplication application = SCObjectGenerator.PrepareApplicationObject();

			SCObjectOperations.Instance.AddApplication(application);

			SCRole role = SCObjectGenerator.PrepareRoleObject();

			SCObjectOperations.Instance.AddRole(role, application);

			SCUser user1 = SCObjectGenerator.PrepareUserObject("RU1", "User1", "RoleUser1");
			SCUser user2 = SCObjectGenerator.PrepareUserObject("RU2", "User2", "RoleUser2");

			SCObjectOperations.Instance.AddUser(user1, SCOrganization.GetRoot());
			SCObjectOperations.Instance.AddUser(user2, SCOrganization.GetRoot());

			SCObjectOperations.Instance.AddMemberToRole(user1, role);
			SCObjectOperations.Instance.AddMemberToRole(user2, role);

			SCObjectAndRelationCollection relations = SCSnapshotAdapter.Instance.QueryRolesContainsMembers(new string[] { "Roles" }, new string[] { role.ID }, false, DateTime.MinValue);

			Assert.AreEqual(role.CurrentMembers.Count, relations.Count);

			Console.WriteLine("User1: {0}, User2: {1}, Role: {2}", user1.ID, user2.ID, role.ID);

			SCObjectOperations.Instance.RemoveMemberFromRole(user1, role);
			SCObjectOperations.Instance.RemoveMemberFromRole(user2, role);

			Assert.AreEqual(0, user1.CurrentRoles.Count);
			Assert.AreEqual(0, user2.CurrentRoles.Count);

			SCObjectAndRelationCollection relationDeleted = SCSnapshotAdapter.Instance.QueryRolesContainsMembers(new string[] { "Roles" }, new string[] { role.ID }, false, DateTime.MinValue);
			Assert.AreEqual(role.CurrentMembers.Count, relationDeleted.Count);

			SameContainerUserAndContainerSnapshotCollection containsUsers = UserAndContainerSnapshotAdapter.Instance.LoadByContainerID(role.ID);

			containsUsers.ForEach(u => Assert.AreEqual(SchemaObjectStatus.Deleted, u.Status));
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void UpdateRoleRoleConditionTest()
		{
			SCApplication application = SCObjectGenerator.PrepareApplicationObject();

			SCObjectOperations.Instance.AddApplication(application);

			SCRole role = SCObjectGenerator.PrepareRoleObject();

			SCObjectOperations.Instance.AddRole(role, application);

			SCConditionOwner owner = new SCConditionOwner() { OwnerID = role.ID };

			owner.Conditions.Add(new SCCondition() { OwnerID = role.ID, SortID = 0, Condition = "Users.Status == 1" });
			owner.Conditions.Add(new SCCondition() { OwnerID = role.ID, SortID = 1, Condition = "Users.Status == 3" });

			SCObjectOperations.Instance.UpdateRoleConditions(owner);

			SCConditionOwner ownerLoaded = SCConditionAdapter.Instance.Load(role.ID);

			Assert.AreEqual(owner.OwnerID, ownerLoaded.OwnerID);
			Assert.AreEqual("Default", ownerLoaded.Type);

			Assert.AreEqual(owner.Conditions.Count, ownerLoaded.Conditions.Count);

			for (int i = 0; i < owner.Conditions.Count; i++)
			{
				Assert.AreEqual(owner.Conditions[i].OwnerID, ownerLoaded.Conditions[i].OwnerID);
				Assert.AreEqual(owner.Conditions[i].Condition, ownerLoaded.Conditions[i].Condition);
			}
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		[Description("构造一个包含了群组、人员和组织（内部有人）的角色，然后测试角色的GetCurrentUsers方法")]
		public void GetRoleUsersTest()
		{
			TestRoleData roleData = SCObjectGenerator.PrepareTestRoleWithOrgAndGroup();

			SchemaObjectCollection users = roleData.Role.GetCurrentUsers();

			Assert.IsTrue(users.ContainsKey(roleData.UserInGroup.ID));
			Assert.IsTrue(users.ContainsKey(roleData.UserInOrg.ID));
			Assert.IsTrue(users.ContainsKey(roleData.UserInRole.ID));
		}
	}
}
