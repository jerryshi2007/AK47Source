using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Core;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Test.Facade
{
	[TestClass]
	public class ObjectDestroyTest
	{
		internal T NewObject<T>(string title) where T : PC.SCBase, new()
		{
			string uuid = UuidHelper.NewUuidString();

			return new T()
			{
				ID = uuid,
				Name = title + uuid,
				DisplayName = title + uuid,
				CodeName = uuid
			};
		}

		internal T NewObject<T>() where T : PC.SCBase, new()
		{
			return this.NewObject<T>(typeof(T).Name);
		}

		protected PC.Executors.ISCObjectOperations Facade
		{
			get { return PC.Executors.SCObjectOperations.Instance; }
		}


		private T LoadObject<T>(string objectID) where T : PC.SCBase
		{
			return (T)PC.Adapters.SchemaObjectAdapter.Instance.Load(objectID);
		}

		[TestMethod]
		public void ClearUserWithOrganizationTest()
		{
			var testOrg = NewObject<PC.SCOrganization>();
			Facade.AddOrganization(testOrg, PC.SCOrganization.GetRoot());

			var testUser = NewObject<PC.SCUser>();
			Facade.AddUser(testUser, testOrg);

			Facade.DeleteUser(testUser, null, false);

			var actual = LoadObject<PC.SCUser>(testUser.ID);

			Assert.IsNotNull(actual);
			Assert.AreNotEqual(SchemaObjectStatus.Normal, actual.Status);
			Assert.AreEqual(0, actual.CurrentParentRelations.Count);

			var org = LoadObject<PC.SCOrganization>(testOrg.ID);
			Assert.IsFalse(org.CurrentChildrenRelations.Exists(m => m.ID == testUser.ID && m.Status == SchemaObjectStatus.Normal));
		}

		[TestMethod]
		public void ClearUserWithGroupTest()
		{
			var testOrg = NewObject<PC.SCOrganization>();
			Facade.AddOrganization(testOrg, PC.SCOrganization.GetRoot());

			var testGroup = NewObject<PC.SCGroup>();
			Facade.AddGroup(testGroup, testOrg);

			var testUser = NewObject<PC.SCUser>();
			Facade.AddUser(testUser, testOrg);
			Facade.AddUserToGroup(testUser, testGroup);

			Facade.DeleteUser(testUser, null, false);

			var actual = LoadObject<PC.SCUser>(testUser.ID);

			Assert.AreEqual(0, actual.CurrentGroups.Count);

			var grp = LoadObject<PC.SCGroup>(testGroup.ID);

			Assert.AreEqual(0, grp.CurrentMembersRelations.Count);
		}

		[TestMethod]
		public void ClearUserWithSecretaryTest()
		{
			var testOrg = NewObject<PC.SCOrganization>();
			Facade.AddOrganization(testOrg, PC.SCOrganization.GetRoot());

			var testUser = NewObject<PC.SCUser>();
			Facade.AddUser(testUser, testOrg);

			var testSecretary = NewObject<PC.SCUser>();
			Facade.AddUser(testSecretary, testOrg);

			Facade.AddSecretaryToUser(testSecretary, testUser);

			Facade.DeleteUser(testUser, null, false);

			var actual = LoadObject<PC.SCUser>(testUser.ID);

			Assert.AreEqual(0, actual.CurrentSecretaries.Count);

			var secretary = LoadObject<PC.SCUser>(testSecretary.ID);

			Assert.AreEqual(0, secretary.CurrentSecretariesOf.Count);

		}

		[TestMethod]
		public void ClearSecretaryWithUserTest()
		{
			var testOrg = NewObject<PC.SCOrganization>();
			Facade.AddOrganization(testOrg, PC.SCOrganization.GetRoot());

			var testUser = NewObject<PC.SCUser>();
			Facade.AddUser(testUser, testOrg);

			var testSecretary = NewObject<PC.SCUser>();
			Facade.AddUser(testSecretary, testOrg);

			Facade.AddSecretaryToUser(testSecretary, testUser);

			Facade.DeleteUser(testSecretary, null, false);

			var actual = LoadObject<PC.SCUser>(testSecretary.ID);

			Assert.AreEqual(0, actual.CurrentSecretariesOf.Count);

			var secretary = LoadObject<PC.SCUser>(testUser.ID);

			Assert.AreEqual(0, secretary.CurrentSecretaries.Count);

		}

		[TestMethod]
		public void ClearOrganizationWithChildrenTest()
		{
			var testOrg = NewObject<PC.SCOrganization>();
			Facade.AddOrganization(testOrg, PC.SCOrganization.GetRoot());

			var testSubOrg = NewObject<PC.SCOrganization>();
			Facade.AddOrganization(testSubOrg, testOrg);

			var testChildOrg = NewObject<PC.SCOrganization>();
			Facade.AddOrganization(testChildOrg, testSubOrg);

			var testChildUser = NewObject<PC.SCUser>();
			Facade.AddUser(testChildUser, testSubOrg);

			var testChildGroup = NewObject<PC.SCGroup>();
			Facade.AddGroup(testChildGroup, testSubOrg);

			PC.SchemaObjectCollection collection = new PC.SchemaObjectCollection();
			collection.Add(testSubOrg);

			Facade.DeleteObjectsRecursively(collection, testOrg);

			var actualSubOrg = LoadObject<PC.SCOrganization>(testSubOrg.ID);
			var actualChildOrg = LoadObject<PC.SCOrganization>(testChildOrg.ID);
			var actualChildUser = LoadObject<PC.SCUser>(testChildUser.ID);
			var actualChildGroup = LoadObject<PC.SCGroup>(testChildGroup.ID);

			Assert.AreNotEqual(SchemaObjectStatus.Normal, actualSubOrg.Status);

			Assert.AreEqual(0, actualSubOrg.CurrentChildren.Count);

			Assert.AreEqual(0, actualChildGroup.CurrentParentRelations.Count);
			Assert.AreEqual(0, actualChildOrg.CurrentParentRelations.Count);
			Assert.AreEqual(0, actualChildUser.CurrentParentRelations.Count);
		}

		[TestMethod]
		public void ClearGroupWithMembersTest()
		{
			var testOrg = NewObject<PC.SCOrganization>();
			Facade.AddOrganization(testOrg, PC.SCOrganization.GetRoot());

			var testGroup = NewObject<PC.SCGroup>();
			Facade.AddGroup(testGroup, testOrg);

			var testUser = NewObject<PC.SCUser>();
			Facade.AddUser(testUser, testOrg);

			Facade.AddUserToGroup(testUser, testGroup);

			Facade.DeleteGroup(testGroup, testOrg, false);

			var actual = LoadObject<PC.SCGroup>(testGroup.ID);

			Assert.AreEqual(0, actual.CurrentMembersRelations.Count);

			Assert.AreEqual(0, actual.CurrentParentRelations.Count);

			testOrg = LoadObject<PC.SCOrganization>(testOrg.ID);

			Assert.AreEqual(1, testOrg.CurrentChildren.Count);

		}

		[TestMethod]
		public void ClearRoleWithMembersTest()
		{
			var testApp = NewObject<PC.SCApplication>();
			Facade.AddApplication(testApp);

			var testRole = NewObject<PC.SCRole>();
			Facade.AddRole(testRole, testApp);

			var testOrg = NewObject<PC.SCOrganization>();
			var testUser = NewObject<PC.SCUser>();
			var testGroup = NewObject<PC.SCGroup>();

			Facade.AddOrganization(testOrg, PC.SCOrganization.GetRoot());
			Facade.AddUser(testUser, testOrg);
			Facade.AddGroup(testGroup, testOrg);

			Facade.AddMemberToRole(testOrg, testRole);
			Facade.AddMemberToRole(testGroup, testRole);
			Facade.AddMemberToRole(testGroup, testRole);

			Facade.DeleteRole(testRole);

			var actualRole = LoadObject<PC.SCRole>(testRole.ID);

			Assert.AreEqual(0, testApp.CurrentRolesRelations.Count);

			Assert.AreEqual(0, testRole.CurrentChildren.Count);
			Assert.AreEqual(0, testRole.CurrentMembersRelations.Count);

			var actualGroup = LoadObject<PC.SCGroup>(testGroup.ID);
			Assert.AreEqual(0, actualGroup.CurrentRolesRelations.Count);

			var actualOrganization = LoadObject<PC.SCOrganization>(testOrg.ID);
			Assert.AreEqual(0, actualOrganization.CurrentRolesRelations.Count);

			var actualUser = LoadObject<PC.SCUser>(testUser.ID);
			Assert.AreEqual(0, actualUser.CurrentRolesRelations.Count);
		}

		[TestMethod]
		public void ClearRoleWithPermissions()
		{
			var testApp = NewObject<PC.SCApplication>();
			Facade.AddApplication(testApp);

			var testRole = NewObject<PC.SCRole>();
			Facade.AddRole(testRole, testApp);

			var testPermission = NewObject<PC.SCPermission>();
			Facade.AddPermission(testPermission, testApp);

			Facade.JoinRoleAndPermission(testRole, testPermission);

			Facade.DeleteRole(testRole);

			var role = LoadObject<PC.SCRole>(testRole.ID);

			Assert.AreEqual(0, role.CurrentPermissions.Count);
		}

		[TestMethod]
		public void ClearPermissionWithRolePermissions()
		{
			var testApp = NewObject<PC.SCApplication>();
			Facade.AddApplication(testApp);

			var testRole = NewObject<PC.SCRole>();
			Facade.AddRole(testRole, testApp);

			var testPermission = NewObject<PC.SCPermission>();
			Facade.AddPermission(testPermission, testApp);

			Facade.JoinRoleAndPermission(testRole, testPermission);

			Facade.DeletePermission(testPermission);

			var role = LoadObject<PC.SCRole>(testRole.ID);

			Assert.AreEqual(0, role.CurrentPermissions.Count);
		}
	}
}
