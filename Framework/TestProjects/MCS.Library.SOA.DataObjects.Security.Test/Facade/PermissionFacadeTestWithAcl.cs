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
	public class PermissionFacadeTestWithAcl : AclBasedTestBase
	{
		[ClassInitialize()]
		public static void Init(TestContext context)
		{
			ReGenOUData();
		}

		#region 权限
		[TestMethod]
		[Description("测试总管理员创建权限")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfAdminCreatePermission()
		{
			InitAdmins();

			var mainApp = this.CreateDefaultApp();

			var testPermission = base.NewObject<SCPermission>("测试权限");

			FacadeWithAcl.AddPermission(testPermission, mainApp);

			Assert.IsNotNull((PC.SCPermission)PC.Adapters.SchemaObjectAdapter.Instance.Load(testPermission.ID), "未创建权限");
		}

		[TestMethod]
		[Description("测试一般管理创建权限")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfCreatePermission()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();
			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			base.SetContainerMemberAndPermissions(mainApp, mainRole, new string[] { "AddPermissions" });

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			var testPermission = NewObject<SCPermission>("测试权限");
			FacadeWithAcl.AddPermission(testPermission, mainApp);

			//应该创建成功

			Assert.IsNotNull((PC.SCPermission)PC.Adapters.SchemaObjectAdapter.Instance.Load(testPermission.ID), "未创建权限");
		}

		[TestMethod]
		[Description("测试非管理创建权限")]
		[TestCategory(Constants.FacadeCategory)]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		public void TestOfIllegalCreatePermission()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();
			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			base.SetContainerMemberAndPermissions(mainApp, mainRole, new string[] { "AddPermissions" });

			this.SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			var testPermission = NewObject<SCPermission>("测试权限");
			FacadeWithAcl.AddPermission(testPermission, mainApp);
			Assert.Fail("不应该执行到此");
		}

		[TestMethod]
		[Description("测试总管理员删除权限")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfAdminDeletePermission()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();
			var testPermission = NewObject<SCPermission>("测试权限");
			this.CreatePermission(mainApp, testPermission);

			FacadeWithAcl.DeletePermission(testPermission);

			var actual = (PC.SCPermission)PC.Adapters.SchemaObjectAdapter.Instance.Load(testPermission.ID);
			Assert.AreNotEqual(actual.Status, SchemaObjectStatus.Normal);
		}

		[TestMethod]
		[Description("测试一般管理员删除权限")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfDeletePermission()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();
			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			this.SetContainerMemberAndPermissions(mainApp, mainRole, new string[] { "DeletePermissions" });

			var testPermission = NewObject<SCPermission>("测试权限");
			this.CreatePermission(mainApp, testPermission);

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.DeletePermission(testPermission);

			var actual = (PC.SCPermission)PC.Adapters.SchemaObjectAdapter.Instance.Load(testPermission.ID);
			Assert.AreNotEqual(actual.Status, SchemaObjectStatus.Normal);
		}

		[TestMethod]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		[Description("测试非管理员删除权限")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfIllegalDeletePermission()
		{
			base.InitAdmins();

			var mainApp = this.CreateDefaultApp();

			var testPermission = NewObject<SCPermission>("测试权限");
			this.CreatePermission(mainApp, testPermission);

			this.SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			FacadeWithAcl.DeletePermission(testPermission);
			Assert.Fail("不应该执行到此");
		}

		[TestMethod]
		[Description("测试总管理员修改权限")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfAdminUpdatePermission()
		{
			base.InitAdmins();
			var mainApp = this.CreateDefaultApp();
			var testPermission = NewObject<SCPermission>("测试权限");
			this.CreatePermission(mainApp, testPermission);

			FacadeWithAcl.UpdatePermission(testPermission);

		}

		[TestMethod]
		[Description("测试一般管理员修改权限")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfUpdatePermission()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();
			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));
			this.SetContainerMemberAndPermissions(mainApp, mainRole, new string[] { "UpdatePermissions" });

			var testPermission = NewObject<SCPermission>("测试权限");
			this.CreatePermission(mainApp, testPermission);

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			FacadeWithAcl.UpdatePermission(testPermission);
		}

		[TestMethod]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		[Description("测试非管理员修改权限")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfIllegalUpdatePermission()
		{
			this.InitAdmins();

			var mainApp = this.CreateDefaultApp();

			var testPermission = NewObject<SCPermission>("测试权限");
			this.CreatePermission(mainApp, testPermission);

			this.SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			FacadeWithAcl.UpdatePermission(testPermission);
			Assert.Fail("不应该执行到此");
		}
		#endregion
	}
}
