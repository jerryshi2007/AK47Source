using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PC = MCS.Library.SOA.DataObjects.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Security.Permissions;

namespace MCS.Library.SOA.DataObjects.Security.Test.Facade
{
	[TestClass]
	public class MoveObjectFacadeTestWithAcl : AclBasedTestBase
	{
		[ClassInitialize()]
		public static void Init(TestContext context)
		{
			ReGenOUData();
		}

		#region 移动组织
		[TestMethod]
		[TestCategory(Constants.MoveObjectCategory)]
		public void AdminMoveOrganizationTest()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrgA = this.NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB1 = this.NewObject<PC.SCOrganization>("测试组织B1");
			var testOrgB2 = this.NewObject<PC.SCOrganization>("测试组织B2");
			var testOrgC = this.NewObject<PC.SCOrganization>("测试组织C");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB1, testOrgA);
			Facade.AddOrganization(testOrgB2, testOrgA);
			Facade.AddOrganization(testOrgC, testOrgB1);

			FacadeWithAcl.MoveObjectToOrganization(testOrgB1, testOrgC, testOrgB2);

			var actual = PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(testOrgB2.ID, testOrgC.ID);

			Assert.IsNotNull(actual);
		}

		[TestMethod]
		[TestCategory(Constants.MoveObjectCategory)]
		public void LegalMoveOrganizationTest()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrgA = this.NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB1 = this.NewObject<PC.SCOrganization>("测试组织B1");
			var testOrgB2 = this.NewObject<PC.SCOrganization>("测试组织B2");
			var testOrgC = this.NewObject<PC.SCOrganization>("测试组织C");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB1, testOrgA);
			Facade.AddOrganization(testOrgB2, testOrgA);
			Facade.AddOrganization(testOrgC, testOrgB1);

			this.SetContainerMemberAndPermissions(testOrgB1, mainRole, new string[] { "DeleteChildren" });
			this.SetContainerMemberAndPermissions(testOrgB2, mainRole, new string[] { "AddChildren" });

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.MoveObjectToOrganization(testOrgB1, testOrgC, testOrgB2);

			var actual = PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(testOrgB2.ID, testOrgC.ID);

			Assert.IsNotNull(actual);
		}

		[TestMethod]
		[TestCategory(Constants.MoveObjectCategory)]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalMoveOrganizationTest1()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrgA = this.NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB1 = this.NewObject<PC.SCOrganization>("测试组织B1");
			var testOrgB2 = this.NewObject<PC.SCOrganization>("测试组织B2");
			var testOrgC = this.NewObject<PC.SCOrganization>("测试组织C");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB1, testOrgA);
			Facade.AddOrganization(testOrgB2, testOrgA);
			Facade.AddOrganization(testOrgC, testOrgB1);

			this.SetContainerMemberAndPermissions(testOrgB1, mainRole, new string[] { "DeleteChildren" });
			this.SetContainerMemberAndPermissions(testOrgB2, mainRole, new string[] { });

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.MoveObjectToOrganization(testOrgB1, testOrgC, testOrgB2);

			Assert.Fail("不应执行到此");
		}

		[TestMethod]
		[TestCategory(Constants.MoveObjectCategory)]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalMoveOrganizationTest2()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrgA = this.NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB1 = this.NewObject<PC.SCOrganization>("测试组织B1");
			var testOrgB2 = this.NewObject<PC.SCOrganization>("测试组织B2");
			var testOrgC = this.NewObject<PC.SCOrganization>("测试组织C");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB1, testOrgA);
			Facade.AddOrganization(testOrgB2, testOrgA);
			Facade.AddOrganization(testOrgC, testOrgB1);

			this.SetContainerMemberAndPermissions(testOrgB1, mainRole, new string[] { });
			this.SetContainerMemberAndPermissions(testOrgB2, mainRole, new string[] { "AddChildren" });

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.MoveObjectToOrganization(testOrgB1, testOrgC, testOrgB2);

			Assert.Fail("不应执行到此");
		}

		[TestMethod]
		[TestCategory(Constants.MoveObjectCategory)]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalMoveOrganizationTest3()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrgA = this.NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB1 = this.NewObject<PC.SCOrganization>("测试组织B1");
			var testOrgB2 = this.NewObject<PC.SCOrganization>("测试组织B2");
			var testOrgC = this.NewObject<PC.SCOrganization>("测试组织C");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB1, testOrgA);
			Facade.AddOrganization(testOrgB2, testOrgA);
			Facade.AddOrganization(testOrgC, testOrgB1);

			this.SetContainerMemberAndPermissions(testOrgB1, mainRole, new string[] { });
			this.SetContainerMemberAndPermissions(testOrgB2, mainRole, new string[] { });

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.MoveObjectToOrganization(testOrgB1, testOrgC, testOrgB2);

			Assert.Fail("不应执行到此");
		}
		#endregion

		#region 移动群组

		[TestMethod]
		[TestCategory(Constants.MoveObjectCategory)]
		public void AdminMoveGroupTest()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrgA = this.NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB1 = this.NewObject<PC.SCOrganization>("测试组织B1");
			var testOrgB2 = this.NewObject<PC.SCOrganization>("测试组织B2");
			var testGroup = this.NewObject<PC.SCGroup>("测试群组");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB1, testOrgA);
			Facade.AddOrganization(testOrgB2, testOrgA);
			Facade.AddGroup(testGroup, testOrgB1);

			FacadeWithAcl.MoveObjectToOrganization(testOrgB1, testGroup, testOrgB2);

			var actual = PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(testOrgB2.ID, testGroup.ID);

			Assert.IsNotNull(actual);
		}

		[TestMethod]
		[TestCategory(Constants.MoveObjectCategory)]
		public void LegalMoveGroupTest()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrgA = this.NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB1 = this.NewObject<PC.SCOrganization>("测试组织B1");
			var testOrgB2 = this.NewObject<PC.SCOrganization>("测试组织B2");
			var testGroup = this.NewObject<PC.SCGroup>("测试群组");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB1, testOrgA);
			Facade.AddOrganization(testOrgB2, testOrgA);
			Facade.AddGroup(testGroup, testOrgB1);

			this.SetContainerMemberAndPermissions(testOrgB1, mainRole, new string[] { "DeleteChildren" });
			this.SetContainerMemberAndPermissions(testOrgB2, mainRole, new string[] { "AddChildren" });

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.MoveObjectToOrganization(testOrgB1, testGroup, testOrgB2);

			var actual = PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(testOrgB2.ID, testGroup.ID);

			Assert.IsNotNull(actual);
		}

		[TestMethod]
		[TestCategory(Constants.MoveObjectCategory)]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalMoveGroupTest1()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrgA = this.NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB1 = this.NewObject<PC.SCOrganization>("测试组织B1");
			var testOrgB2 = this.NewObject<PC.SCOrganization>("测试组织B2");
			var testGroup = this.NewObject<PC.SCGroup>("测试群组");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB1, testOrgA);
			Facade.AddOrganization(testOrgB2, testOrgA);
			Facade.AddGroup(testGroup, testOrgB1);

			this.SetContainerMemberAndPermissions(testOrgB1, mainRole, new string[] { "DeleteChildren" });
			this.SetContainerMemberAndPermissions(testOrgB2, mainRole, new string[] { });

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.MoveObjectToOrganization(testOrgB1, testGroup, testOrgB2);

			Assert.Fail("不应执行到此");
		}

		[TestMethod]
		[TestCategory(Constants.MoveObjectCategory)]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalMoveGroupTest2()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrgA = this.NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB1 = this.NewObject<PC.SCOrganization>("测试组织B1");
			var testOrgB2 = this.NewObject<PC.SCOrganization>("测试组织B2");
			var testGroup = this.NewObject<PC.SCGroup>("测试群组");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB1, testOrgA);
			Facade.AddOrganization(testOrgB2, testOrgA);
			Facade.AddGroup(testGroup, testOrgB1);

			this.SetContainerMemberAndPermissions(testOrgB1, mainRole, new string[] { });
			this.SetContainerMemberAndPermissions(testOrgB2, mainRole, new string[] { "AddChildren" });

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.MoveObjectToOrganization(testOrgB1, testGroup, testOrgB2);

			Assert.Fail("不应执行到此");
		}

		[TestMethod]
		[TestCategory(Constants.MoveObjectCategory)]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalMoveGroupTest3()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrgA = this.NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB1 = this.NewObject<PC.SCOrganization>("测试组织B1");
			var testOrgB2 = this.NewObject<PC.SCOrganization>("测试组织B2");
			var testGroup = this.NewObject<PC.SCGroup>("测试群组");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB1, testOrgA);
			Facade.AddOrganization(testOrgB2, testOrgA);
			Facade.AddGroup(testGroup, testOrgB1);

			this.SetContainerMemberAndPermissions(testOrgB1, mainRole, new string[] { });
			this.SetContainerMemberAndPermissions(testOrgB2, mainRole, new string[] { });

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.MoveObjectToOrganization(testOrgB1, testGroup, testOrgB2);

			Assert.Fail("不应执行到此");
		}
		#endregion

		#region 移动人员

		[TestMethod]
		[TestCategory(Constants.MoveObjectCategory)]
		public void AdminMoveUserTest()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrgA = this.NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB1 = this.NewObject<PC.SCOrganization>("测试组织B1");
			var testOrgB2 = this.NewObject<PC.SCOrganization>("测试组织B2");
			var testUser = this.NewObject<PC.SCUser>("测试群组");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB1, testOrgA);
			Facade.AddOrganization(testOrgB2, testOrgA);
			Facade.AddUser(testUser, testOrgB1);

			FacadeWithAcl.MoveObjectToOrganization(testOrgB1, testUser, testOrgB2);

			var actual = PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(testOrgB2.ID, testUser.ID);

			Assert.IsNotNull(actual);

			Assert.AreEqual(actual.Default, true);
		}

		[TestMethod]
		[TestCategory(Constants.MoveObjectCategory)]
		public void LegalMoveUserTest()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrgA = this.NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB1 = this.NewObject<PC.SCOrganization>("测试组织B1");
			var testOrgB2 = this.NewObject<PC.SCOrganization>("测试组织B2");
			var testUser = this.NewObject<PC.SCUser>("测试群组");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB1, testOrgA);
			Facade.AddOrganization(testOrgB2, testOrgA);
			Facade.AddUser(testUser, testOrgB1);

			this.SetContainerMemberAndPermissions(testOrgB1, mainRole, new string[] { "DeleteChildren" });
			this.SetContainerMemberAndPermissions(testOrgB2, mainRole, new string[] { "AddChildren" });

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.MoveObjectToOrganization(testOrgB1, testUser, testOrgB2);

			var actual = PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(testOrgB2.ID, testUser.ID);

			Assert.IsNotNull(actual);

			Assert.AreEqual(actual.Default, true);
		}

		[TestMethod]
		[TestCategory(Constants.MoveObjectCategory)]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalMoveUserTest1()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrgA = this.NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB1 = this.NewObject<PC.SCOrganization>("测试组织B1");
			var testOrgB2 = this.NewObject<PC.SCOrganization>("测试组织B2");
			var testUser = this.NewObject<PC.SCUser>("测试群组");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB1, testOrgA);
			Facade.AddOrganization(testOrgB2, testOrgA);
			Facade.AddUser(testUser, testOrgB1);

			this.SetContainerMemberAndPermissions(testOrgB1, mainRole, new string[] { "DeleteChildren" });
			this.SetContainerMemberAndPermissions(testOrgB2, mainRole, new string[] { });

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.MoveObjectToOrganization(testOrgB1, testUser, testOrgB2);

			Assert.Fail("不应执行到此");
		}

		[TestMethod]
		[TestCategory(Constants.MoveObjectCategory)]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalMoveUserTest2()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrgA = this.NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB1 = this.NewObject<PC.SCOrganization>("测试组织B1");
			var testOrgB2 = this.NewObject<PC.SCOrganization>("测试组织B2");
			var testUser = this.NewObject<PC.SCUser>("测试群组");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB1, testOrgA);
			Facade.AddOrganization(testOrgB2, testOrgA);
			Facade.AddUser(testUser, testOrgB1);

			this.SetContainerMemberAndPermissions(testOrgB1, mainRole, new string[] { });
			this.SetContainerMemberAndPermissions(testOrgB2, mainRole, new string[] { "AddChildren" });

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.MoveObjectToOrganization(testOrgB1, testUser, testOrgB2);

			Assert.Fail("不应执行到此");
		}

		[TestMethod]
		[TestCategory(Constants.MoveObjectCategory)]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void IllegalMoveUserTest3()
		{
			this.InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrgA = this.NewObject<PC.SCOrganization>("测试组织A");
			var testOrgB1 = this.NewObject<PC.SCOrganization>("测试组织B1");
			var testOrgB2 = this.NewObject<PC.SCOrganization>("测试组织B2");
			var testUser = this.NewObject<PC.SCUser>("测试群组");

			Facade.AddOrganization(testOrgA, mainOrg);
			Facade.AddOrganization(testOrgB1, testOrgA);
			Facade.AddOrganization(testOrgB2, testOrgA);
			Facade.AddUser(testUser, testOrgB1);

			this.SetContainerMemberAndPermissions(testOrgB1, mainRole, new string[] { });
			this.SetContainerMemberAndPermissions(testOrgB2, mainRole, new string[] { });

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.MoveObjectToOrganization(testOrgB1, testUser, testOrgB2);

			Assert.Fail("不应执行到此");
		}
		#endregion
	}
}
