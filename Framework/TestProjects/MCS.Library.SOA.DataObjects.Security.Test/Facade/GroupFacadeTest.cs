using System;
using System.Diagnostics;
using System.Linq;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Library.SOA.DataObjects.Security.Logs;
using MCS.Library.SOA.DataObjects.Security.Test.SchemaObject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Test.Facade
{
	[TestClass]
	public class GroupFacadeTest
	{
		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void AddGroupExecutorTest()
		{
			Trace.CorrelationManager.ActivityId = UuidHelper.NewUuid();

			SCGroup group = SCObjectGenerator.PrepareGroupObject();

			SCObjectOperations.Instance.AddGroup(group, SCOrganization.GetRoot());

			SCGroup groupLoaded = (SCGroup)SchemaObjectAdapter.Instance.Load(group.ID);

			Assert.AreEqual(group.ID, groupLoaded.ID);

			SCOperationLog log = SCOperationLogAdapter.Instance.LoadByResourceID(group.ID).FirstOrDefault();

			Assert.IsNotNull(log);
			Assert.AreEqual(Trace.CorrelationManager.ActivityId.ToString(), log.CorrelationID);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void AddUserToGroupExecutorTest()
		{
			SCGroup group = SCObjectGenerator.PrepareGroupObject();

			SCObjectOperations.Instance.AddGroup(group, SCOrganization.GetRoot());

			SCUser user1 = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user1, SCOrganization.GetRoot());

			SCMemberRelation mr = SCObjectOperations.Instance.AddUserToGroup(user1, group);

			Assert.AreEqual(group.SchemaType, mr.ContainerSchemaType);
			Assert.AreEqual(user1.SchemaType, mr.MemberSchemaType);

			SCUser user2 = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user2, SCOrganization.GetRoot());

			SCObjectOperations.Instance.AddUserToGroup(user2, group);

			Assert.AreEqual(2, group.CurrentUsers.Count);
			Assert.IsTrue(group.CurrentUsers.ContainsKey(user1.ID));
			Assert.IsTrue(group.CurrentUsers.ContainsKey(user2.ID));

			Assert.IsTrue(user1.CurrentGroups.ContainsKey(group.ID));
			Assert.IsTrue(user2.CurrentGroups.ContainsKey(group.ID));

			SameContainerUserAndContainerSnapshotCollection ugSnapshot = UserAndContainerSnapshotAdapter.Instance.LoadByContainerID(group.ID);

			Assert.IsTrue(ugSnapshot.ContainsKey(user1.ID));
			Assert.IsTrue(ugSnapshot.ContainsKey(user2.ID));
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		[Description("合并组内人员测试。合并后UserAndContainerSnapshot表中，相同的人员只有一个")]
		public void MergeUserInGroupExecutorTest()
		{
			// 准备4位用户。User1位状态不变，User2待删除，User3新增加，User4复活
			SCGroup group = SCObjectGenerator.PrepareGroupObject();

			SCObjectOperations.Instance.AddGroup(group, SCOrganization.GetRoot());

			SCUser user1 = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user1, SCOrganization.GetRoot());

			SCMemberRelation mr = SCObjectOperations.Instance.AddUserToGroup(user1, group);

			Assert.AreEqual(group.SchemaType, mr.ContainerSchemaType);
			Assert.AreEqual(user1.SchemaType, mr.MemberSchemaType);

			SCUser user2 = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user2, SCOrganization.GetRoot());

			SCObjectOperations.Instance.AddUserToGroup(user2, group);

			SCUser user3 = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user3, SCOrganization.GetRoot());

			SCUser user4 = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user4, SCOrganization.GetRoot());
			SCObjectOperations.Instance.AddUserToGroup(user4, group);
			SCObjectOperations.Instance.RemoveUserFromGroup(user4, group);

			user4.Status = SchemaObjectStatus.Normal;

			SchemaObjectCollection needMergeUsers = new SchemaObjectCollection() { user1, user3, user4 };

			UserAndContainerSnapshotAdapter.Instance.Merge(group.ID, group.SchemaType, needMergeUsers);

			Console.WriteLine("Group ID: {0}", group.ID);

			SameContainerUserAndContainerSnapshotCollection ugSnapshot = UserAndContainerSnapshotAdapter.Instance.LoadByContainerID(group.ID);

			Assert.IsTrue(ugSnapshot.ContainsKey(user1.ID));
			Assert.AreEqual(SchemaObjectStatus.Normal, ugSnapshot[user1.ID].Status);

			Assert.IsTrue(ugSnapshot.ContainsKey(user2.ID));
			Assert.AreEqual(SchemaObjectStatus.Deleted, ugSnapshot[user2.ID].Status);

			Assert.IsTrue(ugSnapshot.ContainsKey(user3.ID));
			Assert.AreEqual(SchemaObjectStatus.Normal, ugSnapshot[user3.ID].Status);

			Assert.IsTrue(ugSnapshot.ContainsKey(user4.ID));
			Assert.AreEqual(SchemaObjectStatus.Normal, ugSnapshot[user4.ID].Status);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void RemoveUserFromGroupExecutorTest()
		{
			SCGroup group = SCObjectGenerator.PrepareGroupObject();

			SCObjectOperations.Instance.AddGroup(group, SCOrganization.GetRoot());

			SCUser user1 = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user1, SCOrganization.GetRoot());

			SCObjectOperations.Instance.AddUserToGroup(user1, group);

			SCObjectOperations.Instance.RemoveUserFromGroup(user1, group);

			Assert.IsTrue(group.AllMembersRelations.ContainsKey(user1.ID));
			Assert.IsFalse(group.CurrentMembersRelations.ContainsKey(user1.ID));

			Assert.IsTrue(user1.AllMemberOfRelations.ContainsKey(group.ID));
			Assert.IsFalse(user1.CurrentMemberOfRelations.ContainsKey(group.ID));
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void AddOrRemoveUserToGroupExecutorTest()
		{
			SCGroup group = SCObjectGenerator.PrepareGroupObject();

			SCObjectOperations.Instance.AddGroup(group, SCOrganization.GetRoot());

			SCUser user1 = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user1, SCOrganization.GetRoot());

			Console.WriteLine("UserID: {0}, GroupID: {1}", user1.ID, group.ID);

			SCMemberRelation mr = SCObjectOperations.Instance.AddUserToGroup(user1, group);

			SCObjectOperations.Instance.RemoveUserFromGroup(user1, group);

			// 确认删除
			Assert.IsFalse(group.CurrentMembersRelations.ContainsKey(user1.ID));
			Assert.IsFalse(user1.CurrentMemberOfRelations.ContainsKey(group.ID));

			SCObjectOperations.Instance.AddUserToGroup(user1, group);

			// 重置数据
			group.ClearRelativeData();
			user1.ClearRelativeData();

			// 确认又加回来了
			Assert.IsTrue(group.CurrentUsers.ContainsKey(user1.ID));
			Assert.IsTrue(user1.CurrentGroups.ContainsKey(group.ID));

			Console.WriteLine("UserID: {0}, GroupID: {1}", user1.ID, group.ID);
		}
	}
}
