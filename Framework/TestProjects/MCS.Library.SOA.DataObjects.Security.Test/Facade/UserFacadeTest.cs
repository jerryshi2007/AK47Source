using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Security.Test.SchemaObject;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Library.SOA.DataObjects.Security.Logs;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Test.Facade
{
	[TestClass]
	public class UserFacadeTest
	{
		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void AddUserExecutorTest()
		{
			Trace.CorrelationManager.ActivityId = UuidHelper.NewUuid();

			SCUser user = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user, null);

			SCUser userLoaded = (SCUser)SchemaObjectAdapter.Instance.Load(user.ID);

			Assert.AreEqual(user.ID, userLoaded.ID);

			SCOperationLog log = SCOperationLogAdapter.Instance.LoadByResourceID(user.ID).FirstOrDefault();

			Assert.IsNotNull(log);
			Assert.AreEqual(Trace.CorrelationManager.ActivityId.ToString(), log.CorrelationID);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void AddUserOperationTest()
		{
			Trace.CorrelationManager.ActivityId = UuidHelper.NewUuid();

			SCUser user = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.DoOperation(SCObjectOperationMode.Add, user, null);

			SCUser userLoaded = (SCUser)SchemaObjectAdapter.Instance.Load(user.ID);

			Assert.AreEqual(user.ID, userLoaded.ID);

			SCOperationLog log = SCOperationLogAdapter.Instance.LoadByResourceID(user.ID).FirstOrDefault();

			Assert.IsNotNull(log);
			Assert.AreEqual(Trace.CorrelationManager.ActivityId.ToString(), log.CorrelationID);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void DeleteUserExecutorTest()
		{
			SCUser user = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user, null);

			SCUser userLoaded = (SCUser)SchemaObjectAdapter.Instance.Load(user.ID);

			SCObjectOperations.Instance.DeleteUser(userLoaded, null, false);

			userLoaded = (SCUser)SchemaObjectAdapter.Instance.Load(user.ID);

			Assert.AreEqual(SchemaObjectStatus.Deleted, userLoaded.Status);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		[Description("删除用户同时删除关系")]
		public void DeleteUserAndRelationExecutorTest()
		{
			SCOrganization root = SCOrganization.GetRoot();

			SCUser user = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user, root);

			SCUser userLoaded = (SCUser)SchemaObjectAdapter.Instance.Load(user.ID);

			SCObjectOperations.Instance.DeleteUser(userLoaded, null, false);

			userLoaded = (SCUser)SchemaObjectAdapter.Instance.Load(user.ID);

			Assert.AreEqual(SchemaObjectStatus.Deleted, userLoaded.Status);

			userLoaded.AllParentRelations.ForEach(r => Assert.AreEqual(SchemaObjectStatus.Deleted, r.Status));
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		[Description("删除用户和组织之间的关系")]
		public void DeleteUserRelationExecutorTest()
		{
			SCOrganization root = SCOrganization.GetRoot();

			SCUser user = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user, root);

			SCUser userLoaded = (SCUser)SchemaObjectAdapter.Instance.Load(user.ID);

			SCObjectOperations.Instance.DeleteUser(userLoaded, root, false);

			userLoaded = (SCUser)SchemaObjectAdapter.Instance.Load(user.ID);

			Assert.AreEqual(SchemaObjectStatus.Normal, userLoaded.Status);

			userLoaded.AllParentRelations.ForEach(r =>
			{
				if (r.ParentID != SCOrganization.RootOrganizationID) Assert.AreEqual(SchemaObjectStatus.Deleted, r.Status);
			});
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void AddUserToOrganizationTest()
		{
			SCUser user = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user, null);

			SCUser userLoaded = (SCUser)SchemaObjectAdapter.Instance.Load(user.ID);
			SCOrganization org = SCOrganization.GetRoot();

			SCObjectOperations.Instance.AddUserToOrganization(user, org);

			SCRelationObject relation = SchemaRelationObjectAdapter.Instance.Load(org.ID, userLoaded.ID);

			Assert.IsNotNull(relation);
			string oldFullPath = relation.FullPath;

			user.Name = "ChangedUserName";
			SCObjectOperations.Instance.UpdateUser(user);

			relation = SchemaRelationObjectAdapter.Instance.Load(org.ID, userLoaded.ID);

			Assert.AreNotEqual(oldFullPath, relation.FullPath);
			Assert.IsTrue(relation.FullPath.IndexOf(user.Name) >= 0);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void AddUserToOrganizationWithDefaultPropertyTest()
		{
			SCUser user = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user, null);

			SCUser userLoaded = (SCUser)SchemaObjectAdapter.Instance.Load(user.ID);

			SCOrganization root = SCOrganization.GetRoot();

			SCOrganization org1 = SCObjectGenerator.PrepareOrganizationObject();
			SCObjectOperations.Instance.AddOrganization(org1, root);

			SCOrganization org2 = SCObjectGenerator.PrepareOrganizationObject();
			SCObjectOperations.Instance.AddOrganization(org2, root);

			SCObjectOperations.Instance.AddUserToOrganization(user, org1);
			SCObjectOperations.Instance.AddUserToOrganization(user, org2);

			SCRelationObject relation1 = SchemaRelationObjectAdapter.Instance.Load(org1.ID, userLoaded.ID);
			SCRelationObject relation2 = SchemaRelationObjectAdapter.Instance.Load(org2.ID, userLoaded.ID);

			Assert.IsTrue(relation1.Default);
			Assert.IsFalse(relation2.Default);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void RemoveUserToOrganizationWithDefaultPropertyTest()
		{
			SCUser user = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user, null);

			SCUser userLoaded = (SCUser)SchemaObjectAdapter.Instance.Load(user.ID);

			SCOrganization root = SCOrganization.GetRoot();

			SCOrganization org1 = SCObjectGenerator.PrepareOrganizationObject();
			SCObjectOperations.Instance.AddOrganization(org1, root);

			SCOrganization org2 = SCObjectGenerator.PrepareOrganizationObject();
			SCObjectOperations.Instance.AddOrganization(org2, root);

			SCObjectOperations.Instance.AddUserToOrganization(user, org1);
			SCObjectOperations.Instance.AddUserToOrganization(user, org2);

			SCObjectOperations.Instance.DeleteUser(user, org1, false);

			SCRelationObject relation1 = SchemaRelationObjectAdapter.Instance.Load(org1.ID, userLoaded.ID);
			SCRelationObject relation2 = SchemaRelationObjectAdapter.Instance.Load(org2.ID, userLoaded.ID);

			Assert.IsTrue(relation2.Default);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void SetUserDefaultOrganizationTest()
		{
			SCUser user = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user, null);

			SCUser userLoaded = (SCUser)SchemaObjectAdapter.Instance.Load(user.ID);

			SCOrganization root = SCOrganization.GetRoot();

			SCOrganization org1 = SCObjectGenerator.PrepareOrganizationObject();
			SCObjectOperations.Instance.AddOrganization(org1, root);

			SCOrganization org2 = SCObjectGenerator.PrepareOrganizationObject();
			SCObjectOperations.Instance.AddOrganization(org2, root);

			SCObjectOperations.Instance.AddUserToOrganization(user, org1);
			SCObjectOperations.Instance.AddUserToOrganization(user, org2);

			SCObjectOperations.Instance.SetUserDefaultOrganization(user, org2);

			SCRelationObject relation1 = SchemaRelationObjectAdapter.Instance.Load(org1.ID, userLoaded.ID);
			SCRelationObject relation2 = SchemaRelationObjectAdapter.Instance.Load(org2.ID, userLoaded.ID);

			Assert.IsFalse(relation1.Default);
			Assert.IsTrue(relation2.Default);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void AddSecretaryToUserExecutorTest()
		{
			SCUser secretary = SCObjectGenerator.PrepareUserObject("罗", "剑", "罗剑" + UuidHelper.NewUuidString());

			SCObjectOperations.Instance.AddUser(secretary, SCOrganization.GetRoot());

			SCUser user1 = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user1, SCOrganization.GetRoot());

			SCSecretaryRelation sr = SCObjectOperations.Instance.AddSecretaryToUser(secretary, user1);

			Assert.AreEqual(secretary.SchemaType, sr.ContainerSchemaType);
			Assert.AreEqual(user1.SchemaType, sr.MemberSchemaType);

			SCUser user2 = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user2, SCOrganization.GetRoot());

			SCObjectOperations.Instance.AddSecretaryToUser(secretary, user2);

			Assert.IsTrue(user1.CurrentSecretaries.ContainsKey(secretary.ID));
			Assert.IsTrue(user2.CurrentSecretaries.ContainsKey(secretary.ID));

			Assert.IsTrue(secretary.CurrentSecretariesOf.ContainsKey(user1.ID));
			Assert.IsTrue(secretary.CurrentSecretariesOf.ContainsKey(user2.ID));
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void RemoveSecretaryFromUserExecutorTest()
		{
			SCUser secretary = SCObjectGenerator.PrepareUserObject("罗", "剑", "罗剑" + UuidHelper.NewUuidString());

			SCObjectOperations.Instance.AddUser(secretary, SCOrganization.GetRoot());

			SCUser user1 = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user1, SCOrganization.GetRoot());

			SCSecretaryRelation sr = SCObjectOperations.Instance.AddSecretaryToUser(secretary, user1);

			Assert.IsTrue(user1.CurrentSecretaries.ContainsKey(secretary.ID));
			Assert.IsTrue(secretary.CurrentSecretariesOf.ContainsKey(user1.ID));

			secretary.ClearRelativeData();
			user1.ClearRelativeData();

			SCObjectOperations.Instance.RemoveSecretaryFromUser(secretary, user1);

			Assert.IsFalse(user1.CurrentSecretaries.ContainsKey(secretary.ID));
			Assert.IsFalse(secretary.CurrentSecretariesOf.ContainsKey(user1.ID));
		}
	}
}
