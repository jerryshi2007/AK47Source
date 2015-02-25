using System.Linq;
using System.Transactions;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Schemas.Actions;

namespace MCS.Library.SOA.DataObjects.Security.Test.SchemaObject
{
	[TestClass]
	public class SchemaRelationsTest
	{
		[TestMethod]
		[TestCategory(Constants.SchemaObjectCategory)]
		[Description("在组织下添加人员")]
		public void AddUserInOrganizationTest()
		{
			SCOrganization org = SCObjectGenerator.PrepareOrganizationObject();
			SCUser user = SCObjectGenerator.PrepareUserObject();

			SCRelationObject relation = UpdateRelation(org, user);

			org.ClearRelativeData();

			SCRelationObject relationLoaded = SchemaRelationObjectAdapter.Instance.Load(org.ID, user.ID);

			Console.WriteLine("Org ID: {0}, User ID: {1}", org.ID, user.ID);

			Assert.IsNotNull(relationLoaded);
			Assert.AreEqual(org.ID, relationLoaded.ParentID);
			Assert.AreEqual(user.ID, relationLoaded.ID);
			Assert.AreEqual(user.VersionStartTime, org.VersionStartTime);
			Assert.AreEqual(relation.VersionStartTime, org.VersionStartTime);

			ChildrenCheck(SCOrganization.GetRoot(), org, user);
			ParentsCheck(user, org, SCOrganization.GetRoot());
		}

		[TestMethod]
		[TestCategory(Constants.SchemaObjectCategory)]
		[Description("在组织下删除人员和组织的关系")]
		public void RemoveUserAndOrganizationRelationTest()
		{
			SCOrganization org = SCObjectGenerator.PrepareOrganizationObject();
			SCUser user = SCObjectGenerator.PrepareUserObject();

			SCRelationObject relation = UpdateRelation(org, user);

			SCRelationObject relationLoaded = SchemaRelationObjectAdapter.Instance.Load(org.ID, user.ID);

			Console.WriteLine("Org ID: {0}, User ID: {1}", org.ID, user.ID);

			SchemaRelationObjectAdapter.Instance.UpdateStatus(relationLoaded, SchemaObjectStatus.Deleted);

			relationLoaded = SchemaRelationObjectAdapter.Instance.Load(org.ID, user.ID);

			Assert.AreEqual(SchemaObjectStatus.Deleted, relationLoaded.Status);
		}

		private static SCRelationObject UpdateRelation(SCOrganization org, SCUser user)
		{
			SCOrganization root = SCOrganization.GetRoot();

			SCRelationObject orgRelation = new SCRelationObject(root, org);

			SCRelationObject relation = new SCRelationObject(org, user);

			SCActionContext.Current.DoActions(() =>
			{
				using (TransactionScope scope = TransactionScopeFactory.Create())
				{
					SchemaObjectAdapter.Instance.Update(org);
					SchemaRelationObjectAdapter.Instance.Update(orgRelation);
					SchemaObjectAdapter.Instance.Update(user);
					SchemaRelationObjectAdapter.Instance.Update(relation);

					scope.Complete();
				}
			});

			return relation;
		}

		/// <summary>
		/// 逐级检查父子关系
		/// </summary>
		/// <param name="objs"></param>
		private static void ChildrenCheck(params SchemaObjectBase[] objs)
		{
			if (objs.Length > 0)
			{
				SchemaObjectBase prevObj = objs[0];

				for (int i = 1; i < objs.Length; i++)
				{
					SchemaObjectBase currentObj = objs[i];

					if (prevObj is ISCRelationContainer)
					{
						Assert.IsTrue(((ISCRelationContainer)prevObj).CurrentChildren.ContainsKey(currentObj.ID),
							"对象{0}({1})不是对象{2}({3})的子对象",
							currentObj.ID, currentObj.SchemaType,
							prevObj.ID, prevObj.SchemaType);
					}

					prevObj = currentObj;
				}
			}
		}

		private static void ParentsCheck(params SchemaObjectBase[] objs)
		{
			if (objs.Length > 0)
			{
				SchemaObjectBase prevObj = objs[0];

				for (int i = 1; i < objs.Length; i++)
				{
					SchemaObjectBase currentObj = objs[i];

					Assert.IsTrue(prevObj.CurrentParents.ContainsKey(currentObj.ID),
						"对象{0}({1})不是对象{2}({3})的父对象",
						currentObj.ID, currentObj.SchemaType,
						prevObj.ID, prevObj.SchemaType);

					prevObj = currentObj;
				}
			}
		}
	}
}
