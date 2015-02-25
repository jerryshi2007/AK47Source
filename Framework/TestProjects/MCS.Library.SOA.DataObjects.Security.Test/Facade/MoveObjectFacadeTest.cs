using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Library.SOA.DataObjects.Security.Test.SchemaObject;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Security.Test.Facade
{
	[TestClass]
	public class MoveObjectFacadeTest
	{
		[TestMethod]
		[TestCategory(Constants.MoveObjectCategory)]
		public void MoveUserTest()
		{
			SCOrganization root = SCOrganization.GetRoot();

			SCUser user = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user, root);

			SCOrganization targetOrg = SCObjectGenerator.PrepareOrganizationObject();

			SCObjectOperations.Instance.AddOrganization(targetOrg, root);

			SCObjectOperations.Instance.MoveObjectToOrganization(root, user, targetOrg);

			user = (SCUser)SchemaObjectAdapter.Instance.Load(user.ID);

			user.ClearRelativeData();
			targetOrg.ClearRelativeData();
			root.ClearRelativeData();

			Assert.IsTrue(user.CurrentParents.ContainsKey(targetOrg.ID));
			Assert.IsTrue(targetOrg.CurrentChildren.ContainsKey(user.ID));
			Assert.IsFalse(root.CurrentChildren.ContainsKey(user.ID));
			Assert.AreEqual(targetOrg.ID, user.OwnerID);
		}

		[TestMethod]
		[TestCategory(Constants.MoveObjectCategory)]
		public void MoveUserNotChangeOwnerTest()
		{
			SCOrganization root = SCOrganization.GetRoot();

			SCUser user = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user, root);

			SCOrganization sidelineOrg = SCObjectGenerator.PrepareOrganizationObject();

			SCObjectOperations.Instance.AddOrganization(sidelineOrg, root);

			//设置了兼职
			SCObjectOperations.Instance.AddUser(user, sidelineOrg);

			SCOrganization targetOrg = SCObjectGenerator.PrepareOrganizationObject();

			SCObjectOperations.Instance.AddOrganization(targetOrg, root);

			SCObjectOperations.Instance.MoveObjectToOrganization(sidelineOrg, user, targetOrg);

			user = (SCUser)SchemaObjectAdapter.Instance.Load(user.ID);

			user.ClearRelativeData();
			targetOrg.ClearRelativeData();
			sidelineOrg.ClearRelativeData();
			root.ClearRelativeData();

			Assert.IsTrue(user.CurrentParents.ContainsKey(targetOrg.ID));
			Assert.IsTrue(targetOrg.CurrentChildren.ContainsKey(user.ID));
			Assert.IsFalse(sidelineOrg.CurrentChildren.ContainsKey(user.ID));
			Assert.AreNotEqual(targetOrg.ID, user.OwnerID);
		}

		[TestMethod]
		[TestCategory(Constants.MoveObjectCategory)]
		[Description("测试修改用户的所有者")]
		public void ChangeOwnerTest()
		{
			SCOrganization root = SCOrganization.GetRoot();

			SCUser user = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user, root);

			SCOrganization sidelineOrg = SCObjectGenerator.PrepareOrganizationObject();

			SCObjectOperations.Instance.AddOrganization(sidelineOrg, root);

			//设置了兼职
			SCObjectOperations.Instance.AddUser(user, sidelineOrg);

			SCObjectOperations.Instance.ChangeOwner(user, sidelineOrg);

			user = (SCUser)SchemaObjectAdapter.Instance.Load(user.ID);

			user.ClearRelativeData();
			sidelineOrg.ClearRelativeData();
			root.ClearRelativeData();

			Console.WriteLine(user.OwnerName);
			Assert.AreEqual(sidelineOrg.ID, user.OwnerID);
		}

		[TestMethod]
		[TestCategory(Constants.MoveObjectCategory)]
		public void MoveGroupTest()
		{
			SCOrganization root = SCOrganization.GetRoot();

			SCGroup group = SCObjectGenerator.PrepareGroupObject();

			SCObjectOperations.Instance.AddGroup(group, root);

			SCOrganization newOrg = SCObjectGenerator.PrepareOrganizationObject();

			SCObjectOperations.Instance.AddOrganization(newOrg, root);

			SCObjectOperations.Instance.MoveObjectToOrganization(null, group, newOrg);

			group.ClearRelativeData();
			newOrg.ClearRelativeData();
			root.ClearRelativeData();

			Assert.IsTrue(group.CurrentParents.ContainsKey(newOrg.ID));
			Assert.IsTrue(newOrg.CurrentChildren.ContainsKey(group.ID));
			Assert.IsFalse(root.CurrentChildren.ContainsKey(group.ID));
		}
	}
}
