using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;


namespace MCS.Library.SOA.DataObjects.Security.Test.Facade
{
	[TestClass]
	public class RoleFacadeTestWithAcl : AclBasedTestBase
	{
		[ClassInitialize()]
		public static void Init(TestContext context)
		{
			ReGenOUData();
		}

		#region 角色
		[TestMethod]
		[Description("测试总管理员创建角色")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfAdminCreateRole()
		{
			InitAdmins();

			this.SetCurrentPrincipal(this.GetUserByCodeName("wangli5"));

			var mainApp = this.CreateDefaultApp();

			var testRole = base.NewObject<SCRole>("测试角色");

			FacadeWithAcl.AddRole(testRole, mainApp);

			Assert.IsNotNull((PC.SCRole)PC.Adapters.SchemaObjectAdapter.Instance.Load(testRole.ID), "未创建角色");
		}

		[TestMethod]
		[Description("测试一般管理创建角色")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfCreateRole()
		{
			InitAdmins();

			var mainApp = this.CreateDefaultApp();
			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			base.SetContainerMemberAndPermissions(mainApp, mainRole, new string[] { "AddRoles" });

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			var testRole = NewObject<SCRole>("测试角色");
			FacadeWithAcl.AddRole(testRole, mainApp);

			//应该创建成功

			Assert.IsNotNull((PC.SCRole)PC.Adapters.SchemaObjectAdapter.Instance.Load(testRole.ID), "未创建角色");
		}

		[TestMethod]
		[Description("测试非管理创建角色")]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfIllegalCreateRole()
		{
			InitAdmins();

			var mainApp = this.CreateDefaultApp();
			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			base.SetContainerMemberAndPermissions(mainApp, mainRole, new string[] { "AddRoles" });

			this.SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			var testRole = NewObject<SCRole>("测试角色");
			FacadeWithAcl.AddRole(testRole, mainApp);
			Assert.Fail("不应该执行到此");
		}

		[TestMethod]
		[Description("测试总管理员删除角色")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfAdminDeleteRole()
		{
			base.InitAdmins();
			var mainApp = this.CreateDefaultApp();
			var testRole = NewObject<SCRole>("测试角色");
			this.CreateRole(mainApp, testRole);

			FacadeWithAcl.DeleteRole(testRole);

			var actual = (PC.SCRole)PC.Adapters.SchemaObjectAdapter.Instance.Load(testRole.ID);
			Assert.AreNotEqual(actual.Status, SchemaObjectStatus.Normal);
		}

		[TestMethod]
		[Description("测试一般管理员删除角色")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfDeleteRole()
		{
			base.InitAdmins();

			var mainApp = this.CreateDefaultApp();
			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			this.SetContainerMemberAndPermissions(mainApp, mainRole, new string[] { "DeleteRoles" });

			var testRole = NewObject<SCRole>("测试角色");
			this.CreateRole(mainApp, testRole);

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.DeleteRole(testRole);

			var actual = (PC.SCRole)PC.Adapters.SchemaObjectAdapter.Instance.Load(testRole.ID);
			Assert.AreNotEqual(actual.Status, SchemaObjectStatus.Normal);
		}

		[TestMethod]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		[Description("测试非管理员删除角色")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfIllegalDeleteRole()
		{
			base.InitAdmins();

			var mainApp = this.CreateDefaultApp();

			var testRole = NewObject<SCRole>("测试角色");
			this.CreateRole(mainApp, testRole);

			this.SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			FacadeWithAcl.DeleteRole(testRole);
			Assert.Fail("不应该执行到此");
		}

		[TestMethod]
		[Description("测试总管理员修改角色")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfAdminUpdateRole()
		{
			base.InitAdmins();
			var mainApp = this.CreateDefaultApp();
			var testRole = NewObject<SCRole>("测试角色");
			this.CreateRole(mainApp, testRole);

			FacadeWithAcl.UpdateRole(testRole);

		}

		[TestMethod]
		[Description("测试一般管理员修改角色")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfUpdateRole()
		{
			base.InitAdmins();

			var mainApp = this.CreateDefaultApp();
			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			this.SetContainerMemberAndPermissions(mainApp, mainRole, new string[] { "UpdateRoles" });

			var testRole = NewObject<SCRole>("测试角色");
			this.CreateRole(mainApp, testRole);

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.UpdateRole(testRole);
		}

		[TestMethod]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		[Description("测试非管理员修改角色")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfIllegalUpdateRole()
		{
			base.InitAdmins();

			var mainApp = this.CreateDefaultApp();

			var testRole = NewObject<SCRole>("测试角色");
			this.CreateRole(mainApp, testRole);

			this.SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			FacadeWithAcl.UpdateRole(testRole);
			Assert.Fail("不应该执行到此");
		}
		#endregion
	}
}
