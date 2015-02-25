using Microsoft.VisualStudio.TestTools.UnitTesting;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Test.Facade
{
	[TestClass]
	public class GroupFacadeTestWithAcl : AclBasedTestBase
	{
		[ClassInitialize()]
		public static void Init(TestContext context)
		{
			ReGenOUData();
		}

		#region 添加群组的测试
		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void AdminAddGroupTest()
		{
			this.InitAdmins();

			PC.SCGroup testGrp = this.NewObject<PC.SCGroup>("测试群组");

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrg = this.NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, mainOrg);

			FacadeWithAcl.AddGroup(testGrp, testOrg);

			var actual = GetGroupByCodeName(testGrp.CodeName);

			Assert.IsNotNull(actual);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void LegalAddGroupTest()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrg = this.NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, mainOrg);

			this.SetContainerMemberAndPermissions(testOrg, mainRole, new string[] { "AddChildren" });

			PC.SCGroup testGrp = this.NewObject<PC.SCGroup>("测试群组");

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.AddGroup(testGrp, testOrg);

			var actual = this.GetGroupByCodeName(testGrp.CodeName);

			Assert.IsNotNull(actual);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		[ExpectedException(typeof(MCS.Library.SOA.DataObjects.Security.Permissions.SCAclPermissionCheckException))]
		public void IllegalAddGroupTest()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrg = this.NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, mainOrg);

			this.SetContainerMemberAndPermissions(testOrg, mainRole, new string[] { "AddChildren" });

			PC.SCGroup testGrp = this.NewObject<PC.SCGroup>("测试群组");

			this.SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			FacadeWithAcl.AddGroup(testGrp, testOrg);

			Assert.Fail("不应执行到此");
		}
		#endregion

		#region 删除群组的测试
		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void AdminDeleteGroupTest()
		{
			this.InitAdmins();

			PC.SCGroup testGrp = this.NewObject<PC.SCGroup>("测试群组");

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrg = this.NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, mainOrg);

			Facade.AddGroup(testGrp, testOrg);

			FacadeWithAcl.DeleteGroup(testGrp, testOrg, false);

			var actual = PC.Adapters.SchemaObjectAdapter.Instance.Load(testGrp.ID);

			Assert.AreNotEqual(actual.Status, SchemaObjectStatus.Normal);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void LegalDeleteGroupTest()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrg = this.NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, mainOrg);

			this.SetContainerMemberAndPermissions(testOrg, mainRole, new string[] { "DeleteChildren" });

			PC.SCGroup testGrp = this.NewObject<PC.SCGroup>("测试群组");

			Facade.AddGroup(testGrp, testOrg);

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.DeleteGroup(testGrp, testOrg, false);

			var actual = PC.Adapters.SchemaObjectAdapter.Instance.Load(testGrp.ID);

			Assert.AreNotEqual(actual.Status, SchemaObjectStatus.Normal);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		[ExpectedException(typeof(MCS.Library.SOA.DataObjects.Security.Permissions.SCAclPermissionCheckException))]
		public void IllegalDeleteGroupTest()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrg = this.NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, mainOrg);

			this.SetContainerMemberAndPermissions(testOrg, mainRole, new string[] { "DeleteChildren" });

			PC.SCGroup testGrp = this.NewObject<PC.SCGroup>("测试群组");

			Facade.AddGroup(testGrp, testOrg);

			this.SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			FacadeWithAcl.DeleteGroup(testGrp, testOrg, false);

			Assert.Fail("不应执行到此");
		}

		#endregion

		#region 群组中加人的测试

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void AdminAddUserToGroupTest()
		{
			this.InitAdmins();

			PC.SCGroup testGrp = this.NewObject<PC.SCGroup>("测试群组");

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrg = this.NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, mainOrg);

			Facade.AddGroup(testGrp, testOrg);

			var testUser = this.GetSCUserByCodeName("jinge");

			FacadeWithAcl.AddUserToGroup(testUser, testGrp);

			var actual = PC.Adapters.SCMemberRelationAdapter.Instance.Load(testGrp.ID, testUser.ID);

			Assert.IsNotNull(actual);

			Assert.AreEqual(actual.Status, SchemaObjectStatus.Normal);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void LegalAddUserToGroupTest()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrg = this.NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, mainOrg);

			this.SetContainerMemberAndPermissions(testOrg, mainRole, new string[] { "EditMembersOfGroups" });

			PC.SCGroup testGrp = this.NewObject<PC.SCGroup>("测试群组");

			Facade.AddGroup(testGrp, testOrg);

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			var testUser = this.GetSCUserByCodeName("jinge");

			FacadeWithAcl.AddUserToGroup(testUser, testGrp);

			var actual = PC.Adapters.SCMemberRelationAdapter.Instance.Load(testGrp.ID, testUser.ID);

			Assert.IsNotNull(actual);

			Assert.AreEqual(actual.Status, SchemaObjectStatus.Normal);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		[ExpectedException(typeof(MCS.Library.SOA.DataObjects.Security.Permissions.SCAclPermissionCheckException))]
		public void IllegalAddUserToGroupTest()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrg = this.NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, mainOrg);

			this.SetContainerMemberAndPermissions(testOrg, mainRole, new string[] { "EditMembersOfGroups" });

			PC.SCGroup testGrp = this.NewObject<PC.SCGroup>("测试群组");

			Facade.AddGroup(testGrp, testOrg);

			this.SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			var testUser = this.GetSCUserByCodeName("jinge");

			FacadeWithAcl.AddUserToGroup(testUser, testGrp);

			Assert.Fail("不应执行到此");
		}

		#endregion

		#region 群组中减人的测试

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void AdminRemoveUserFromGroupTest()
		{
			this.InitAdmins();

			PC.SCGroup testGrp = this.NewObject<PC.SCGroup>("测试群组");

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrg = this.NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, mainOrg);

			Facade.AddGroup(testGrp, testOrg);

			var testUser = this.GetSCUserByCodeName("jinge");

			Facade.AddUserToGroup(testUser, testGrp);

			FacadeWithAcl.RemoveUserFromGroup(testUser, testGrp);

			var actual = PC.Adapters.SCMemberRelationAdapter.Instance.Load(testGrp.ID, testUser.ID);

			Assert.IsNotNull(actual);

			Assert.AreNotEqual(actual.Status, SchemaObjectStatus.Normal);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void LegalRemoveUserFromGroupTest()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrg = this.NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, mainOrg);

			this.SetContainerMemberAndPermissions(testOrg, mainRole, new string[] { "EditMembersOfGroups" });

			PC.SCGroup testGrp = this.NewObject<PC.SCGroup>("测试群组");

			Facade.AddGroup(testGrp, testOrg);

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			var testUser = this.GetSCUserByCodeName("jinge");

			Facade.AddUserToGroup(testUser, testGrp);

			FacadeWithAcl.RemoveUserFromGroup(testUser, testGrp);

			var actual = PC.Adapters.SCMemberRelationAdapter.Instance.Load(testGrp.ID, testUser.ID);

			Assert.IsNotNull(actual);

			Assert.AreNotEqual(actual.Status, SchemaObjectStatus.Normal);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		[ExpectedException(typeof(MCS.Library.SOA.DataObjects.Security.Permissions.SCAclPermissionCheckException))]
		public void IllegalRemoveUserFromGroupTest()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = this.GetOrganizationByCodeName("costEngineer");

			var testOrg = this.NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, mainOrg);

			this.SetContainerMemberAndPermissions(testOrg, mainRole, new string[] { "EditMembersOfGroups" });

			PC.SCGroup testGrp = this.NewObject<PC.SCGroup>("测试群组");

			Facade.AddGroup(testGrp, testOrg);

			this.SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			var testUser = this.GetSCUserByCodeName("jinge");

			FacadeWithAcl.RemoveUserFromGroup(testUser, testGrp);

			Assert.Fail("不应执行到此");
		}

		#endregion

	}
}
