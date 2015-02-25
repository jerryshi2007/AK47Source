using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Test.Facade
{
	[TestClass]
	public class UserFacadeTestWithAcl : AclBasedTestBase
	{
		[ClassInitialize()]
		public static void Init(TestContext context)
		{
			ReGenOUData();
		}

		#region 创建人员测试
		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void AdminAddUserTest()
		{
			InitAdmins();

			PC.SCUser user = NewObject<PC.SCUser>("测试用户");

			FacadeWithAcl.AddUser(user, null);

			var actual = GetSCUserByCodeName(user.CodeName);

			Assert.IsNotNull(actual);
		}


		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void LegalAddUserTest()
		{
			InitAdmins();

			var mainApp = this.CreateDefaultApp();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = base.GetOrganizationByCodeName("costEngineer");

			var testOrg = this.NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, mainOrg);
			base.SetContainerMemberAndPermissions(testOrg, mainRole, new string[] { "AddChildren" });

			PC.SCUser user = NewObject<PC.SCUser>("测试用户");

			SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.AddUser(user, testOrg);

			var actual = GetSCUserByCodeName(user.CodeName);

			Assert.IsNotNull(actual);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalAddUserTest()
		{
			InitAdmins();

			var mainApp = this.CreateDefaultApp();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = base.GetOrganizationByCodeName("costEngineer");

			var testOrg = this.NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, mainOrg);
			base.SetContainerMemberAndPermissions(testOrg, mainRole, new string[] { "AddChildren" });

			PC.SCUser user = NewObject<PC.SCUser>("测试用户");

			SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			FacadeWithAcl.AddUser(user, testOrg);

			Assert.Fail("不应该执行到此");
		}
		#endregion

		#region 从组织中删除用户测试
		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		[Description("删除用户和组织之间的关系")]
		public void AdminRemoveUserFromOrgTest()
		{
			InitAdmins();

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrg = NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, mainOrg);

			var testUser = NewObject<PC.SCUser>("测试用户");

			Facade.AddUser(testUser, testOrg);

			FacadeWithAcl.DeleteUser(testUser, testOrg, true);

			var relation = PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(testOrg.ID, testUser.ID);

			Assert.IsTrue(relation == null || relation.Status != SchemaObjectStatus.Normal);

		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		[Description("删除用户和组织之间的关系")]
		public void LegalRemoveUserFromOrgTest()
		{
			InitAdmins();

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var mainApp = this.CreateDefaultApp();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var testOrg = NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, mainOrg);

			base.SetContainerMemberAndPermissions(testOrg, mainRole, new string[] { "DeleteChildren" });

			var testUser = NewObject<PC.SCUser>("测试用户");

			Facade.AddUser(testUser, testOrg);

			SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.DeleteUser(testUser, testOrg, true);

			var relation = PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(testOrg.ID, testUser.ID);

			Assert.IsTrue(relation == null || relation.Status != SchemaObjectStatus.Normal);

		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		[Description("删除用户和组织之间的关系")]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalRemoveUserFromOrgTest()
		{
			InitAdmins();

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var mainApp = this.CreateDefaultApp();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var testOrg = NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, mainOrg);

			base.SetContainerMemberAndPermissions(testOrg, mainRole, new string[] { "DeleteChildren" });

			var testUser = NewObject<PC.SCUser>("测试用户");

			Facade.AddUser(testUser, testOrg);

			SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			base.RecalculateRoleUsers();

			FacadeWithAcl.DeleteUser(testUser, testOrg, true);

			var relation = PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(testOrg.ID, testUser.ID);

			Assert.Fail("不应执行到此");

		}
		#endregion

		#region 将人员添加到现有组织中的测试
		[TestMethod]
		public void AdminAddUserToOrganizationTest()
		{
			InitAdmins();

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var mainApp = this.CreateDefaultApp();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var testOrg = NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, mainOrg);

			var testUser = this.NewObject<PC.SCUser>("测试用户");

			Facade.AddUser(testUser, null);

			FacadeWithAcl.AddUserToOrganization(testUser, testOrg);

			var actual = PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(testOrg.ID, testUser.ID);

			Assert.IsNotNull(actual);
		}


		[TestMethod]
		public void LegalAddUserToOrganizationTest()
		{
			InitAdmins();

			var mainApp = this.CreateDefaultApp();
			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrg = NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, mainOrg);

			this.SetContainerMemberAndPermissions(testOrg, mainRole, new string[] { "AddChildren" });

			SetCurrentPrincipal(this.GetSCUserByCodeName("wanglch"));

			var testUser = this.NewObject<PC.SCUser>("测试用户");

			Facade.AddUser(testUser, null);

			FacadeWithAcl.AddUserToOrganization(testUser, testOrg);

			var actual = PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(testOrg.ID, testUser.ID);

			Assert.IsNotNull(actual);
		}

		[TestMethod]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalAddUserToOrganizationTest()
		{
			InitAdmins();

			var mainApp = this.CreateDefaultApp();
			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrg = NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, mainOrg);

			this.SetContainerMemberAndPermissions(testOrg, mainRole, new string[] { "AddChildren" });

			SetCurrentPrincipal(this.GetSCUserByCodeName("chenke"));

			var testUser = this.NewObject<PC.SCUser>("测试用户");

			Facade.AddUser(testUser, null);

			FacadeWithAcl.AddUserToOrganization(testUser, testOrg);

			Assert.Fail("不应执行到此");
		}

		#endregion

		#region 添加人员秘书

		[TestMethod]
		public void AdminAddSecretaryTest()
		{
			InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");
			var testUserB = this.NewObject<PC.SCUser>("测试用户B");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUser(testUserB, testOrgB);

			FacadeWithAcl.AddSecretaryToUser(testUserA, testUserB);

			var actual = PC.Adapters.SCMemberRelationAdapter.Instance.Load(testUserB.ID, testUserA.ID);

			Assert.IsNotNull(actual);

			Assert.AreEqual(actual.Status, SchemaObjectStatus.Normal);
		}


		[TestMethod]
		public void LegalAddSecretaryTest()
		{
			InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");
			var testUserB = this.NewObject<PC.SCUser>("测试用户B");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUser(testUserB, testOrgB);

			SetContainerMemberAndPermissions(testOrgA, mainRole, new string[] { "UpdateChildren" });
			SetContainerMemberAndPermissions(testOrgB, mainRole, new string[] { "UpdateChildren" });

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.AddSecretaryToUser(testUserA, testUserB);

			var actual = PC.Adapters.SCMemberRelationAdapter.Instance.Load(testUserB.ID, testUserA.ID);

			Assert.IsNotNull(actual);

			Assert.AreEqual(actual.Status, SchemaObjectStatus.Normal);
		}

		[TestMethod]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalAddSecretaryTest1()
		{
			InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");
			var testUserB = this.NewObject<PC.SCUser>("测试用户B");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUser(testUserB, testOrgB);

			this.SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			FacadeWithAcl.AddSecretaryToUser(testUserA, testUserB);

			Assert.Fail("不应执行到此");
		}

		[TestMethod]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalAddSecretaryTest2()
		{
			InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");
			var testUserB = this.NewObject<PC.SCUser>("测试用户B");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUser(testUserB, testOrgB);

			SetContainerMemberAndPermissions(testOrgA, mainRole, new string[] { "UpdateChildren" });
			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.AddSecretaryToUser(testUserA, testUserB);

			Assert.Fail("不应执行到此");
		}

		[TestMethod]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalAddSecretaryTest3()
		{
			InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");
			var testUserB = this.NewObject<PC.SCUser>("测试用户B");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUser(testUserB, testOrgB);

			SetContainerMemberAndPermissions(testOrgB, mainRole, new string[] { "UpdateChildren" });
			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.AddSecretaryToUser(testUserA, testUserB);

			Assert.Fail("不应执行到此");
		}

		#endregion

		#region 删除人员秘书

		[TestMethod]
		public void AdminRemoveSecretaryTest()
		{
			InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");
			var testUserB = this.NewObject<PC.SCUser>("测试用户B");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUser(testUserB, testOrgB);

			Facade.AddSecretaryToUser(testUserA, testUserB);

			FacadeWithAcl.RemoveSecretaryFromUser(testUserA, testUserB);

			var actual = PC.Adapters.SCMemberRelationAdapter.Instance.Load(testUserB.ID, testUserA.ID);

			Assert.IsNotNull(actual);

			Assert.AreNotEqual(actual.Status, SchemaObjectStatus.Normal);
		}


		[TestMethod]
		public void LegalRemoveSecretaryTest()
		{
			InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");
			var testUserB = this.NewObject<PC.SCUser>("测试用户B");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUser(testUserB, testOrgB);

			SetContainerMemberAndPermissions(testOrgA, mainRole, new string[] { "UpdateChildren" });
			SetContainerMemberAndPermissions(testOrgB, mainRole, new string[] { "UpdateChildren" });

			Facade.AddSecretaryToUser(testUserA, testUserB);

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.RemoveSecretaryFromUser(testUserA, testUserB);

			var actual = PC.Adapters.SCMemberRelationAdapter.Instance.Load(testUserB.ID, testUserA.ID);

			Assert.IsNotNull(actual);

			Assert.AreNotEqual(actual.Status, SchemaObjectStatus.Normal);
		}

		[TestMethod]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalRemoveSecretaryTest1()
		{
			InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");
			var testUserB = this.NewObject<PC.SCUser>("测试用户B");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUser(testUserB, testOrgB);

			Facade.AddSecretaryToUser(testUserA, testUserB);

			this.SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			FacadeWithAcl.RemoveSecretaryFromUser(testUserA, testUserB);

			Assert.Fail("不应执行到此");
		}

		[TestMethod]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalRemoveSecretaryTest2()
		{
			InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");
			var testUserB = this.NewObject<PC.SCUser>("测试用户B");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUser(testUserB, testOrgB);

			SetContainerMemberAndPermissions(testOrgA, mainRole, new string[] { "UpdateChildren" });
			Facade.AddSecretaryToUser(testUserA, testUserB);

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.RemoveSecretaryFromUser(testUserA, testUserB);

			Assert.Fail("不应执行到此");
		}

		[TestMethod]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalRemoveSecretaryTest3()
		{
			InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");
			var testUserB = this.NewObject<PC.SCUser>("测试用户B");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUser(testUserB, testOrgB);

			SetContainerMemberAndPermissions(testOrgB, mainRole, new string[] { "UpdateChildren" });

			Facade.AddSecretaryToUser(testUserA, testUserB);

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.RemoveSecretaryFromUser(testUserA, testUserB);

			Assert.Fail("不应执行到此");
		}

		#endregion

		#region 设置默认组织

		[TestMethod]
		public void AdminSetDefaultOrganizationTest()
		{
			InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");


			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUserToOrganization(testUserA, testOrgB);

			FacadeWithAcl.SetUserDefaultOrganization(testUserA, testOrgB);

			var actual = PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(testOrgB.ID, testUserA.ID);

			Assert.IsNotNull(actual);

			Assert.AreEqual(actual.Status, SchemaObjectStatus.Normal);

			Assert.AreEqual(actual.Default, true);
		}


		[TestMethod]
		public void LegalSetDefaultOrganizationTest()
		{
			InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");


			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUserToOrganization(testUserA, testOrgB);

			SetContainerMemberAndPermissions(testOrgA, mainRole, new string[] { "UpdateChildren" });
			SetContainerMemberAndPermissions(testOrgB, mainRole, new string[] { "UpdateChildren" });
			SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.SetUserDefaultOrganization(testUserA, testOrgB);

			var actual = PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(testOrgB.ID, testUserA.ID);

			Assert.IsNotNull(actual);

			Assert.AreEqual(actual.Status, SchemaObjectStatus.Normal);

			Assert.AreEqual(actual.Default, true);
		}

		[TestMethod]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalSetDefaultOrganizationTest1()
		{
			InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");


			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUserToOrganization(testUserA, testOrgB);

			SetContainerMemberAndPermissions(testOrgA, mainRole, new string[] { "UpdateChildren" });
			SetContainerMemberAndPermissions(testOrgB, mainRole, new string[] { "UpdateChildren" });
			SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			FacadeWithAcl.SetUserDefaultOrganization(testUserA, testOrgB);

			Assert.Fail("不应执行到此");
		}

		[TestMethod]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalSetDefaultOrganizationTest2()
		{
			InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");


			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUserToOrganization(testUserA, testOrgB);

			SetContainerMemberAndPermissions(testOrgA, mainRole, new string[] { "UpdateChildren" });

			SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.SetUserDefaultOrganization(testUserA, testOrgB);

			Assert.Fail("不应执行到此");
		}

		[TestMethod]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalSetDefaultOrganizationTest3()
		{
			InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");


			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUserToOrganization(testUserA, testOrgB);

			SetContainerMemberAndPermissions(testOrgB, mainRole, new string[] { "UpdateChildren" });
			SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.SetUserDefaultOrganization(testUserA, testOrgB);

			Assert.Fail("不应执行到此");
		}

		#endregion

		#region 修改人员测试

		[TestMethod]
		public void AdminUpdateUserTest()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");


			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUserToOrganization(testUserA, testOrgB);

			FacadeWithAcl.UpdateUser(testUserA);
		}

		[TestMethod]
		public void LegalUpdateUserTest()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUserToOrganization(testUserA, testOrgB);

			SetContainerMemberAndPermissions(testOrgA, mainRole, new string[] { "UpdateChildren" });

			SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.UpdateUser(testUserA);
		}

		[TestMethod]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalUpdateUserTest()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");


			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUserToOrganization(testUserA, testOrgB);

			SetContainerMemberAndPermissions(testOrgA, mainRole, new string[] { });
			SetContainerMemberAndPermissions(testOrgB, mainRole, new string[] { "UpdateChildren" });
			SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.UpdateUser(testUserA);

			Assert.Fail("不应执行到此");
		}

		#endregion

		#region 从组织移除人员测试

		[TestMethod]
		public void AdminDeleteUserTest()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");


			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUserToOrganization(testUserA, testOrgB);

			FacadeWithAcl.DeleteUser(testUserA, testOrgA, false);

			var actual = (PC.SCUser)PC.Adapters.SchemaObjectAdapter.Instance.Load(testUserA.ID);

			Assert.IsNotNull(actual);
			Assert.AreEqual(actual.OwnerID, testOrgA.ID);
			Assert.AreEqual(actual.Status, SchemaObjectStatus.Normal);
		}

		[TestMethod]
		public void LegalDeleteUserTest()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUserToOrganization(testUserA, testOrgB);

			SetContainerMemberAndPermissions(testOrgA, mainRole, new string[] { "DeleteChildren" });

			SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.DeleteUser(testUserA, testOrgA, false);

			var actual = (PC.SCUser)PC.Adapters.SchemaObjectAdapter.Instance.Load(testUserA.ID);

			Assert.IsNotNull(actual);
			Assert.AreEqual(actual.OwnerID, testOrgA.ID);
			Assert.AreEqual(actual.Status, SchemaObjectStatus.Normal);
		}

		[TestMethod]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalDeleteUserTest()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");


			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUserToOrganization(testUserA, testOrgB);

			SetContainerMemberAndPermissions(testOrgA, mainRole, new string[] { });
			SetContainerMemberAndPermissions(testOrgB, mainRole, new string[] { "DeleteChildren" });
			SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.DeleteUser(testUserA, testOrgA, false);

			Assert.Fail("不应执行到此");
		}

		#endregion

		#region 删除人员测试

		[TestMethod]
		public void AdminDestroyUserTest()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");


			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUserToOrganization(testUserA, testOrgB);

			FacadeWithAcl.DeleteUser(testUserA, null, false);

			var actual = (PC.SCUser)PC.Adapters.SchemaObjectAdapter.Instance.Load(testUserA.ID);

			Assert.IsNotNull(actual);
			Assert.AreEqual(actual.OwnerID, testOrgA.ID);
			Assert.AreNotEqual(actual.Status, SchemaObjectStatus.Normal);
		}

		[TestMethod]
		public void LegalDestroyUserTest()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUserToOrganization(testUserA, testOrgB);

			SetContainerMemberAndPermissions(testOrgA, mainRole, new string[] { "DeleteChildren" });

			SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.DeleteUser(testUserA, null, false);

			var actual = (PC.SCUser)PC.Adapters.SchemaObjectAdapter.Instance.Load(testUserA.ID);

			Assert.IsNotNull(actual);
			Assert.AreEqual(actual.OwnerID, testOrgA.ID);
			Assert.AreNotEqual(actual.Status, SchemaObjectStatus.Normal);
		}

		[TestMethod]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalDestroyUserTest()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			var mainOrg = this.GetOrganizationByCodeName("costEngineer");
			var testOrgA = NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB = NewObject<PC.SCOrganization>("测试组织B");
			var testUserA = this.NewObject<PC.SCUser>("测试用户A");


			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB, mainOrg);
			Facade.AddUser(testUserA, testOrgA);
			Facade.AddUserToOrganization(testUserA, testOrgB);

			SetContainerMemberAndPermissions(testOrgA, mainRole, new string[] { });
			SetContainerMemberAndPermissions(testOrgB, mainRole, new string[] { "DeleteChildren" });
			SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.DeleteUser(testUserA, null, false);

			Assert.Fail("不应执行到此");
		}

		#endregion
	}
}
