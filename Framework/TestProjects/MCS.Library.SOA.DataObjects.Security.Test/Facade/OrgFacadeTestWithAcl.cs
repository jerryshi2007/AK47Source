using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Test.Facade
{
	[TestClass]
	public class OrgFacadeTestWithAcl : AclBasedTestBase
	{
		[ClassInitialize()]
		public static void Init(TestContext context)
		{
			ReGenOUData();
		}

		#region 添加组织测试
		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void AdminAddOrganizationTest()
		{
			InitAdmins();

			var mainOrg = base.GetOrganizationByCodeName("costEngineer");

			var orgParent = this.NewObject<PC.SCOrganization>("父组织");

			Facade.AddOrganization(orgParent, mainOrg);

			var testOrg = this.NewObject<PC.SCOrganization>("测试组织");

			FacadeWithAcl.AddOrganization(testOrg, orgParent);

			var actual = PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(orgParent.ID, testOrg.ID);

			Assert.IsNotNull(actual);

			Assert.AreEqual(actual.Status, SchemaObjectStatus.Normal);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void LegalAddOrganizationTest()
		{
			InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = base.GetOrganizationByCodeName("costEngineer");

			var orgParent = this.NewObject<PC.SCOrganization>("父组织");

			Facade.AddOrganization(orgParent, mainOrg);

			this.SetContainerMemberAndPermissions(orgParent, mainRole, new string[] { "AddChildren" });

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			var testOrg = this.NewObject<PC.SCOrganization>("测试组织");

			FacadeWithAcl.AddOrganization(testOrg, orgParent);

			var actual = PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(orgParent.ID, testOrg.ID);

			Assert.IsNotNull(actual);

			Assert.AreEqual(actual.Status, SchemaObjectStatus.Normal);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		[ExpectedException(typeof(MCS.Library.SOA.DataObjects.Security.Permissions.SCAclPermissionCheckException))]
		public void IllegalAddOrganizationTest()
		{
			InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = base.GetOrganizationByCodeName("costEngineer");

			var orgParent = this.NewObject<PC.SCOrganization>("父组织");

			Facade.AddOrganization(orgParent, mainOrg);

			this.SetContainerMemberAndPermissions(mainOrg, mainRole, new string[] { "AddChildren" });

			this.SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			var testOrg = this.NewObject<PC.SCOrganization>("测试组织");

			FacadeWithAcl.AddOrganization(testOrg, orgParent);

			Assert.Fail("不应执到此");
		}
		#endregion

		#region 删除组织测试
		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void AdminDeleteOrganizationTest()
		{
			InitAdmins();

			var mainOrg = base.GetOrganizationByCodeName("costEngineer");

			var orgParent = this.NewObject<PC.SCOrganization>("父组织");

			Facade.AddOrganization(orgParent, mainOrg);

			var testOrg = this.NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, orgParent);

			FacadeWithAcl.DeleteOrganization(testOrg, orgParent, false);

			var actual = PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(orgParent.ID, testOrg.ID);

			Assert.IsNotNull(actual);

			Assert.AreNotEqual(actual.Status, SchemaObjectStatus.Normal);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void LegalDeleteOrganizationTest()
		{
			InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = base.GetOrganizationByCodeName("costEngineer");

			var orgParent = this.NewObject<PC.SCOrganization>("父组织");

			Facade.AddOrganization(orgParent, mainOrg);

			this.SetContainerMemberAndPermissions(mainOrg, mainRole, new string[] { "DeleteChildren" });

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			var testOrg = this.NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, orgParent);

			FacadeWithAcl.DeleteOrganization(testOrg, orgParent, false);

			var actual = PC.Adapters.SchemaRelationObjectAdapter.Instance.Load(orgParent.ID, testOrg.ID);

			Assert.IsNotNull(actual);

			Assert.AreNotEqual(actual.Status, SchemaObjectStatus.Normal);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		[ExpectedException(typeof(MCS.Library.SOA.DataObjects.Security.Permissions.SCAclPermissionCheckException))]
		public void IllegalDeleteOrganizationTest()
		{
			InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = base.GetOrganizationByCodeName("costEngineer");

			var orgParent = this.NewObject<PC.SCOrganization>("父组织");

			Facade.AddOrganization(orgParent, mainOrg);

			this.SetContainerMemberAndPermissions(mainOrg, mainRole, new string[] { "DeleteChildren" });

			this.SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			var testOrg = this.NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, orgParent);

			FacadeWithAcl.DeleteOrganization(testOrg, orgParent, false);

			Assert.Fail("不应执到此");
		}
		#endregion

		#region 修改组织测试
		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void AdminUpdateOrganizationTest()
		{
			InitAdmins();

			var mainOrg = base.GetOrganizationByCodeName("costEngineer");

			var orgParent = this.NewObject<PC.SCOrganization>("父组织");

			Facade.AddOrganization(orgParent, mainOrg);

			var testOrg = this.NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, orgParent);

			FacadeWithAcl.UpdateOrganization(testOrg);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void LegalUpdateOrganizationTest()
		{
			InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = base.GetOrganizationByCodeName("costEngineer");

			var orgParent = this.NewObject<PC.SCOrganization>("父组织");

			Facade.AddOrganization(orgParent, mainOrg);

			this.SetContainerMemberAndPermissions(orgParent, mainRole, new string[] { "UpdateChildren" });

			this.SetCurrentPrincipal(this.GetUserByCodeName("wanglch"));

			var testOrg = this.NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, orgParent);

			FacadeWithAcl.UpdateOrganization(testOrg);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		[ExpectedException(typeof(MCS.Library.SOA.DataObjects.Security.Permissions.SCAclPermissionCheckException))]
		public void IllegalUpdateOrganizationTest()
		{
			InitAdmins();

			var mainRole = this.CreateDefaultRoleWithMembers(this.GetSCUsersByCodeNames("wanglch"));

			var mainOrg = base.GetOrganizationByCodeName("costEngineer");

			var orgParent = this.NewObject<PC.SCOrganization>("父组织");

			Facade.AddOrganization(orgParent, mainOrg);

			this.SetContainerMemberAndPermissions(mainOrg, mainRole, new string[] { "UpdateChildren" });

			this.SetCurrentPrincipal(this.GetUserByCodeName("chenke"));

			var testOrg = this.NewObject<PC.SCOrganization>("测试组织");

			Facade.AddOrganization(testOrg, orgParent);

			FacadeWithAcl.UpdateOrganization(testOrg);

			Assert.Fail("不应执到此");
		}
		#endregion
	}
}
