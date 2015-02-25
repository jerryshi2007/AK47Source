using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Schemas.Actions;
using MCS.Library.SOA.DataObjects.Schemas.Adapters;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Library.SOA.DataObjects.Security.Test.SchemaObject;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Security.Test.Snapshot
{
	[TestClass]
	public class SnapshotSqlTest
	{
		[TestMethod]
		[Description("对象快照的更新语句测试")]
		[TestCategory(Constants.SnapshotCategory)]
		public void SCObjectSnapshotUpdateSqlTest()
		{
			SCUser user = SCObjectGenerator.PrepareUserObject();

			SchemaObjectAdapter.Instance.MergeExistsObjectInfo(user);

			string sql = VersionSnapshotUpdateSqlBuilder.Instance.ToUpdateSql(user, ORMapping.GetMappingInfo(user.GetType()));

			Console.WriteLine(sql);

			SCActionContext.Current.DoActions(() => SCSnapshotBasicAdapter.Instance.UpdateCurrentSnapshot(user, user.Schema.SnapshotTable, SnapshotModeDefinition.IsInSnapshot));

			DataRowView drv = SCSnapshotBasicAdapter.Instance.Load(user.Schema.Name, user.ID);

			Assert.IsNotNull(drv);
		}

		[TestMethod]
		[Description("根据对象快照进行ID查询的测试")]
		[TestCategory(Constants.SnapshotCategory)]
		public void SCObjectSnapshotLoadByIDTest()
		{
			SCUser user = SCObjectGenerator.PrepareUserObject();

			string codeName = UuidHelper.NewUuidString();

			user.Properties.SetValue("CodeName", codeName);

			SCActionContext.Current.DoActions(() => SchemaObjectAdapter.Instance.Update(user));

			SchemaObjectBase obj =
				SCSnapshotBasicAdapter.Instance.LoadByID(user.SchemaType, SnapshotQueryIDType.CodeName, codeName);

			Assert.AreEqual(user.ID, obj.ID);
			Assert.AreEqual(codeName, obj.Properties.GetValue("CodeName", string.Empty));
		}

		[TestMethod]
		[Description("根据对象快照进行用户和权限的查询测试")]
		[TestCategory(Constants.SnapshotCategory)]
		public void SCQueryPermissionsByUserIDsSnapshotTest()
		{
			SCApplication application = SCObjectGenerator.PrepareApplicationObject();

			SCObjectOperations.Instance.AddApplication(application);

			SCRole role = SCObjectGenerator.PrepareRoleObject();

			SCObjectOperations.Instance.AddRole(role, application);

			SCPermission permission = SCObjectGenerator.PreparePermissionObject();

			SCObjectOperations.Instance.AddPermission(permission, application);

			SCRelationObject relation = SCObjectOperations.Instance.JoinRoleAndPermission(role, permission);

			SCUser user1 = SCObjectGenerator.PrepareUserObject("RU1", "User1", "RoleUser1");

			SCObjectOperations.Instance.AddUser(user1, SCOrganization.GetRoot());

			SCObjectOperations.Instance.AddMemberToRole(user1, role);

			SchemaObjectCollection result = SCSnapshotAdapter.Instance.QueryPermissionsByUserIDs(new string[] { user1.ID }, false, DateTime.MinValue);

			Assert.IsTrue(result.Count > 0);

			Console.WriteLine(result[0].Properties.GetValue("Name", string.Empty));

			Assert.AreEqual(permission.ID, result[0].ID);
		}

		[TestMethod]
		[Description("根据对象快照加载对象的父亲")]
		[TestCategory(Constants.SnapshotCategory)]
		public void SCObjectSnapshotLoadParentInfoTest()
		{
			SCOrganization parent2 = SCObjectGenerator.PrepareOrganizationObject("第二组织", "第二组织");
			SCObjectOperations.Instance.AddOrganization(parent2, SCOrganization.GetRoot());

			SCOrganization parent22 = SCObjectGenerator.PrepareOrganizationObject("第二.二组织", "第二.二组织");
			SCObjectOperations.Instance.AddOrganization(parent22, parent2);

			SCUser user2 = SCObjectGenerator.PrepareUserObject("徐", "磊", "xulei");
			SCObjectOperations.Instance.AddUser(user2, parent22);

			SCOrganization parent3 = SCObjectGenerator.PrepareOrganizationObject("第三组织", "第三组织");
			SCObjectOperations.Instance.AddOrganization(parent3, SCOrganization.GetRoot());

			SCOrganization parent32 = SCObjectGenerator.PrepareOrganizationObject("第三.二组织", "第三.二组织");
			SCObjectOperations.Instance.AddOrganization(parent32, parent3);

			SCUser user3 = SCObjectGenerator.PrepareUserObject("李", "琪", "liqi");
			SCObjectOperations.Instance.AddUser(user3, parent32);

			Dictionary<string, SCSimpleObjectCollection> parents = SCSnapshotAdapter.Instance.LoadAllParentsInfo(false, user2.ID, user3.ID);

			Assert.AreEqual(2, parents.Count);

			foreach (KeyValuePair<string, SCSimpleObjectCollection> kp in parents)
			{
				IEnumerable<IOrganization> orgs = parents[kp.Key].ToOguObjects<IOrganization>();
				Assert.AreEqual(2, orgs.Count());

				Console.WriteLine(parents[kp.Key].JoinNameToFullPath());
				orgs.ForEach(o => Console.WriteLine(o.DisplayName));
			}
		}

		//[TestMethod]
		//[Description("按照全路径进行查询")]
		//[TestCategory(Constants.SnapshotCategory)]
		//public void QueryByMultiFullPathsTest()
		//{
		//    SCUser user = SCObjectGenerator.PrepareUserObject();

		//    string ticks = DateTime.Now.Ticks.ToString();

		//    SCOrganization parent3 = SCObjectGenerator.PrepareOrganizationObject("第三组织" + ticks, "第三组织");
		//    SCObjectOperations.Instance.AddOrganization(parent3, SCOrganization.GetRoot());

		//    SCOrganization parent32 = SCObjectGenerator.PrepareOrganizationObject("第三.二组织" + ticks, "第三.二组织");
		//    SCObjectOperations.Instance.AddOrganization(parent32, parent3);

		//    SCUser user3 = SCObjectGenerator.PrepareUserObject("李", "琪" + ticks, "liqi");
		//    SCObjectOperations.Instance.AddUser(user3, parent32);

		//    string path = string.Format(@"第三组织{0}\第三.二组织{0}", ticks);

		//    Thread.Sleep(3000);

		//    SCObjectAndRelationCollection objects = SCSnapshotAdapter.Instance.QueryObjectAndRelationByKeywordAndParentFullPaths(new string[] { "Users" }, new string[] { path }, "琪", 999, true, true, false, DateTime.MinValue);

		//    SchemaObjectCollection objs = objects.ToSchemaObjects();

		//    Assert.IsTrue(objects.Count > 0);
		//    Assert.IsTrue(objs.Count > 0);
		//    Assert.IsTrue(objs[0].Properties.Count > 0);
		//}

		[TestMethod]
		[Description("查询每一种类型对象的个数（状态为正常的）")]
		[TestCategory(Constants.SnapshotCategory)]
		public void QueryCountGroupBySchemaTest()
		{
			Dictionary<string, int> result = SCSnapshotAdapter.Instance.QueryCountGroupBySchema(DateTime.MinValue);

			foreach (KeyValuePair<string, int> kp in result)
			{
				Console.WriteLine("Schema: {0}, Count: {1:#,##0}", kp.Key, kp.Value);
			}
		}

		#region QueryObjectAndRelation
		[TestMethod]
		[Description("查询直接的子成员")]
		[TestCategory(Constants.SnapshotCategory)]
		public void QueryDirectChildrenTest()
		{
			SCUser user = SCObjectGenerator.PrepareUserObject();

			string ticks = DateTime.Now.Ticks.ToString();

			SCOrganization parent3 = SCObjectGenerator.PrepareOrganizationObject("第三组织" + ticks, "第三组织");
			SCObjectOperations.Instance.AddOrganization(parent3, SCOrganization.GetRoot());

			SCOrganization parent32 = SCObjectGenerator.PrepareOrganizationObject("第三.二组织" + ticks, "第三.二组织");
			SCObjectOperations.Instance.AddOrganization(parent32, parent3);

			SCUser user3 = SCObjectGenerator.PrepareUserObject("李", "琪" + ticks, "liqi");
			SCObjectOperations.Instance.AddUser(user3, parent32);

			string path = string.Format(@"第三组织{0}\第三.二组织{0}\李琪{0}", ticks);

			SCObjectAndRelationCollection result = SCSnapshotAdapter.Instance.QueryObjectAndRelationByParentIDs(new string[0], new string[] { parent32.ID }, false, true, false, DateTime.MinValue);

			result.Sort((x, y) => x.InnerSort - y.InnerSort);

			result.FillDetails();

			Assert.IsTrue(result.Count > 0);

			Console.WriteLine(result[0].Detail.Properties.GetValue("Name", string.Empty));
			Assert.AreEqual(user3.ID, result[0].Detail.ID);
		}

		[TestMethod]
		[Description("查询组成员")]
		[TestCategory(Constants.SnapshotCategory)]
		public void QueryGroupMembersTest()
		{
			SCGroup group = SCObjectGenerator.PrepareGroupObject();
			SCObjectOperations.Instance.AddGroup(group, SCOrganization.GetRoot());

			SCUser user1 = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user1, SCOrganization.GetRoot());

			SCMemberRelation mr = SCObjectOperations.Instance.AddUserToGroup(user1, group);

			SCObjectAndRelationCollection result = SCSnapshotAdapter.Instance.QueryContainerContainsUsersByIDs(new string[] { "Groups" }, new string[] { group.ID }, false, DateTime.MinValue);

			Assert.IsTrue(result.Count > 0);

			result.FillDetails();

			Console.WriteLine(result[0].Detail.Properties.GetValue("Name", string.Empty));
			Console.WriteLine(result[0].ParentID);
			Assert.AreEqual(user1.ID, result[0].Detail.ID);
		}

		[TestMethod]
		[Description("查询成员所属的组")]
		[TestCategory(Constants.SnapshotCategory)]
		public void QueryMemberOfGroupsTest()
		{
			SCGroup group = SCObjectGenerator.PrepareGroupObject();
			SCObjectOperations.Instance.AddGroup(group, SCOrganization.GetRoot());

			SCUser user1 = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user1, SCOrganization.GetRoot());

			SCMemberRelation mr = SCObjectOperations.Instance.AddUserToGroup(user1, group);

			Console.WriteLine("UserID: {0}, GroupID: {1}", user1.ID, group.ID);

			SCObjectAndRelationCollection result = SCSnapshotAdapter.Instance.QueryUserBelongToContainersByIDs(new string[] { "Groups" }, new string[] { user1.ID }, false, DateTime.MinValue);

			Assert.IsTrue(result.Count > 0);

			result.FillDetails();

			Console.WriteLine(result[0].Detail.Properties.GetValue("Name", string.Empty));
			Console.WriteLine(result[0].ParentID);
			Assert.AreEqual(group.ID, result[0].Detail.ID);
		}
		#endregion QueryObjectAndRelation

		[TestMethod]
		[Description("拼音测试")]
		[TestCategory(Constants.SnapshotCategory)]
		public void PinYinTest()
		{
			string data = "abc沈峥123";

			List<string> pinyinList = SCSnapshotAdapter.Instance.GetPinYin(data);

			foreach (string pinyin in pinyinList)
			{
				Console.WriteLine(pinyin);
			}
		}

		[TestMethod]
		[Description("拼音测试")]
		[TestCategory(Constants.SnapshotCategory)]
		public void EnglishPinYinTest()
		{
			string data = "Jacob Shen";

			List<string> pinyinList = SCSnapshotAdapter.Instance.GetPinYin(data);

			Assert.AreEqual(1, pinyinList.Count);
			Assert.AreEqual("Jacob Shen", pinyinList[0]);

			foreach (string pinyin in pinyinList)
			{
				Console.WriteLine(pinyin);
			}
		}
	}
}
