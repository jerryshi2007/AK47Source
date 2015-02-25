using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Test.Facade
{
	[TestClass]
	public class ApplicationFacadeTestWithAcl : AclBasedTestBase
	{
		[ClassInitialize()]
		public static void Init(TestContext context)
		{
			ReGenOUData();
		}

		#region 应用
		[TestMethod]
		[Description("测试总管理员创建应用")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfAdminCreateApplication()
		{
			this.InitAdmins();

			var app = this.NewObject<SCApplication>("测试应用");
			FacadeWithAcl.AddApplication(app);

			Assert.IsNotNull((PC.SCApplication)PC.Adapters.SchemaObjectAdapter.Instance.Load(app.ID));
		}

		[TestMethod]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		[Description("测试非管理员创建应用")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfIllegalCreateApplication()
		{
			this.InitAdmins();

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			var app = this.NewObject<SCApplication>("测试应用");
			FacadeWithAcl.AddApplication(app);

			Assert.Fail("不应该执行到此处");
		}

		[TestMethod]
		[Description("测试总管理员删除应用")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfAdminDeleteApplication()
		{
			this.InitAdmins();

			var app = this.NewObject<SCApplication>("测试应用");

			FacadeWithAcl.AddApplication(app);

			Sleep(200);

			FacadeWithAcl.DeleteApplication(app);

			Assert.AreNotEqual(PC.Adapters.SchemaObjectAdapter.Instance.Load(app.ID).Status, SchemaObjectStatus.Normal);
		}

		[TestMethod]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		[Description("测试非管理员删除应用")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfIllegalDeleteApplication()
		{
			this.InitAdmins();

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			var app = this.NewObject<SCApplication>("测试应用");

			Facade.AddApplication(app);

			Sleep(200);

			FacadeWithAcl.DeleteApplication(app);

			Assert.Fail("不应该执行到此处");
		}

		[TestMethod]
		[Description("测试总管理员修改应用")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfAdminUpdateApplication()
		{
			this.InitAdmins();

			var app = this.NewObject<SCApplication>("测试应用");

			FacadeWithAcl.AddApplication(app);

			Sleep(200);

			FacadeWithAcl.UpdateApplication(app);
		}

		[TestMethod]
		[Description("测试一般管理员修改应用")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfLegalUpdateApplication()
		{
			this.InitAdmins();

			this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var app = this.NewObject<SCApplication>("测试应用");

			Facade.AddApplication(app);

			var role = this.NewObject<SCRole>("测试角色");

			Facade.AddRole(role, app);

			var chenke = this.GetSCUserByCodeName("chenke");

			Facade.AddMemberToRole(chenke, role);

			this.SetContainerMemberAndPermissions(app, role, new string[] { "UpdateApplications" });

			this.SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			this.RecalculateRoleUsers();

			FacadeWithAcl.UpdateApplication(app);
		}

		[TestMethod]
		[ExpectedException(typeof(SCAclPermissionCheckException))]
		[Description("测试非管理员修改应用")]
		[TestCategory(Constants.FacadeCategory)]
		public void TestOfIllegalUpdateApplication()
		{
			this.InitAdmins();

			var app = this.NewObject<SCApplication>("测试应用");

			Facade.AddApplication(app);

			this.SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			FacadeWithAcl.UpdateApplication(app);

			Assert.Fail("不应该执行到此处");
		}
		#endregion
	}
}
