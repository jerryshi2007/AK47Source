using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Security;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Test.SchemaObject;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Test.Facade
{
	[TestClass]
	public class RoleAndPermissionTestWithAcl : AclBasedTestBase
	{
		[ClassInitialize()]
		public static void Init(TestContext context)
		{
			ReGenOUData();
		}

		protected override void InitAdmins()
		{
			if (this.Special)
				base.InitAdminsAndUser(false);
			else
				base.InitAdmins();
		}

		#region 角色成员操作
		[TestMethod]
		[Description("测试总管理员添加角色成员")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfAdminAddRoleMembers()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();

			var mainRole = this.CreateDefaultRole();

			Facade.AddUser(new SCUser() { ID = Guid.NewGuid().ToString(), Name = "范海燕", CodeName = "fanhy", DisplayName = "fanyh" }, SCOrganization.GetRoot());

			var fanhy = GetSCUserByCodeName("fanhy");
			FacadeWithAcl.AddMemberToRole(fanhy, mainRole);

			var members = PC.Adapters.SCMemberRelationAdapter.Instance.LoadByContainerID(mainRole.ID);

			Assert.IsTrue((from m in members where m.ID == fanhy.ID select m).Any());
		}

		[TestMethod]
		[Description("测试一般管理添加角色成员")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfAddRoleMembers()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();
			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			base.SetContainerMemberAndPermissions(mainApp, mainRole, new string[] { "ModifyMembersInRoles" });

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			var fanhy = GetSCUserByCodeName("fanhy");
			FacadeWithAcl.AddMemberToRole(fanhy, mainRole);

			var members = PC.Adapters.SCMemberRelationAdapter.Instance.LoadByContainerID(mainRole.ID);

			Assert.IsTrue((from m in members where m.ID == fanhy.ID select m).Any());
		}

		[TestMethod]
		[Description("测试非管理添加角色成员")]
		[TestCategory(Constants.FacadeCategory)]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void TestOfIllegalAddRoleMembers()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();
			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			base.SetContainerMemberAndPermissions(mainApp, mainRole, new string[] { "ModifyMembersInRoles" });

			this.SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			var fanhy = GetSCUserByCodeName("fanhy");
			FacadeWithAcl.AddMemberToRole(fanhy, mainRole);

			Assert.Fail("不应该执行到此");
		}

		[TestMethod]
		[Description("测试总管理员删除角色成员")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfAdminDeleteRoleMembers()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();

			var mainRole = this.CreateDefaultRole();

			var fanhy = GetSCUserByCodeName("fanhy");
			Facade.AddMemberToRole(fanhy, mainRole);

			//Sleep(200);

			FacadeWithAcl.RemoveMemberFromRole(fanhy, mainRole);

			var members = PC.Adapters.SCMemberRelationAdapter.Instance.LoadByContainerID(mainRole.ID);

			Assert.IsFalse((from m in members where m.ID == fanhy.ID && m.Status == SchemaObjectStatus.Normal select m).Any());
		}

		[TestMethod]
		[Description("测试一般管理员删除角色成员")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfDeleteRoleMembers()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();
			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			base.SetContainerMemberAndPermissions(mainApp, mainRole, new string[] { "ModifyMembersInRoles" });

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			var fanhy = GetSCUserByCodeName("fanhy");
			Facade.AddMemberToRole(fanhy, mainRole);

			FacadeWithAcl.RemoveMemberFromRole(fanhy, mainRole);

			var members = PC.Adapters.SCMemberRelationAdapter.Instance.LoadByContainerID(mainRole.ID);

			Assert.IsFalse((from m in members where m.ID == fanhy.ID && m.Status == SchemaObjectStatus.Normal select m).Any());
		}

		[TestMethod]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		[Description("测试非管理员删除角色成员")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfIllegalDeleteRoleMembers()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();
			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			base.SetContainerMemberAndPermissions(mainApp, mainRole, new string[] { "ModifyMembersInRoles" });

			this.SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			var fanhy = GetSCUserByCodeName("fanhy");
			Facade.AddMemberToRole(fanhy, mainRole);

			FacadeWithAcl.RemoveMemberFromRole(fanhy, mainRole);

			Assert.Fail("不应该执行到此");
		}

		#endregion

		#region 角色权限操作
		[TestMethod]
		[Description("测试总管理员修改角色权限")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfAdminAssignPermissions()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();

			var mainRole = this.CreateDefaultRole();

			var testRole = this.NewObject<SCRole>("测试角色");
			Facade.AddRole(testRole, mainApp);

			var testPermission = this.NewObject<SCPermission>("测试功能");
			Facade.AddPermission(testPermission, mainApp);

			FacadeWithAcl.JoinRoleAndPermission(testRole, testPermission);

			var pm = PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(testRole.ID, testPermission.ID);

			Assert.IsNotNull(pm);
			Assert.AreEqual(pm.Status, SchemaObjectStatus.Normal);

			FacadeWithAcl.DisjoinRoleAndPermission(testRole, testPermission);

			pm = PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(testRole.ID, testPermission.ID);

			Assert.IsNotNull(pm);
			Assert.AreNotEqual(pm.Status, SchemaObjectStatus.Normal);

		}

		[TestMethod]
		[Description("测试一般管理修改角色权限")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfAssignPermissions()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();
			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			base.SetContainerMemberAndPermissions(mainApp, mainRole, new string[] { "EditRelationOfRolesAndPermissions" });

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			var testRole = this.NewObject<SCRole>("测试角色");
			Facade.AddRole(testRole, mainApp);

			var testPermission = this.NewObject<SCPermission>("测试功能");
			Facade.AddPermission(testPermission, mainApp);

			FacadeWithAcl.JoinRoleAndPermission(testRole, testPermission);

			var pm = PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(testRole.ID, testPermission.ID);

			Assert.IsNotNull(pm);
			Assert.AreEqual(pm.Status, SchemaObjectStatus.Normal);

			FacadeWithAcl.DisjoinRoleAndPermission(testRole, testPermission);

			pm = PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(testRole.ID, testPermission.ID);

			Assert.IsNotNull(pm);
			Assert.AreNotEqual(pm.Status, SchemaObjectStatus.Normal);
		}

		[TestMethod]
		[Description("测试非管理修改角色权限")]
		[TestCategory(Constants.FacadeCategory)]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void TestOfIllegalAssignPermissions()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();
			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			this.SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			var testRole = this.NewObject<SCRole>("测试角色");
			Facade.AddRole(testRole, mainApp);

			var testPermission = this.NewObject<SCPermission>("测试功能");
			Facade.AddPermission(testPermission, mainApp);

			FacadeWithAcl.JoinRoleAndPermission(testRole, testPermission);

			Assert.Fail("不应该执行到此");
		}

		[TestMethod]
		[Description("测试非管理修改角色权限")]
		[TestCategory(Constants.FacadeCategory)]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void TestOfIllegalDisassignPermissions()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();
			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			this.SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			var testRole = this.NewObject<SCRole>("测试角色");
			Facade.AddRole(testRole, mainApp);

			var testPermission = this.NewObject<SCPermission>("测试功能");
			Facade.AddPermission(testPermission, mainApp);

			Facade.JoinRoleAndPermission(testRole, testPermission);

			//Sleep(200);

			FacadeWithAcl.DisjoinRoleAndPermission(testRole, testPermission);

			Assert.Fail("不应该执行到此");
		}

		#endregion
	}

	[TestClass]
	public class PermissionTestWithSA
	{
		RoleAndPermissionTestWithAcl innerTest;

		public PermissionTestWithSA()
		{
			innerTest = new RoleAndPermissionTestWithAcl() { Special = true };

		}

		[TestMethod]
		[Description("测试总管理员添加角色成员")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfAdminAddRoleMembersSpecial()
		{
			this.innerTest.TestOfAdminAddRoleMembers();
		}

		[TestMethod]
		[Description("测试总管理员修改角色权限")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfAdminAssignPermissionsSpecial()
		{
			this.innerTest.TestOfAdminAssignPermissions();
		}
	}
}
