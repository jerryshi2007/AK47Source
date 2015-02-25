using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Conditions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Security.Test.SchemaObject;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Test.Conditions
{
	[TestClass]
	public class BasicConditionTest
	{
		[TestMethod]
		[TestCategory(Constants.ConditionCategory)]
		public void SimpleConditionTest()
		{
			SCConditionOwner owner = new SCConditionOwner();

			SCUser user1 = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user1, SCOrganization.GetRoot());

			SCUser user2 = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user2, SCOrganization.GetRoot());

			owner.OwnerID = UuidHelper.NewUuidString();

			string expression = string.Format("Users.CodeName == \"{0}\"", user2.CodeName);

			owner.Conditions.Add(new SCCondition() { Description = "基本测试", Condition = expression });

			SCConditionCalculator calculator = new SCConditionCalculator();

			IEnumerable<SchemaObjectBase> result = calculator.Calculate(owner);

			Assert.AreEqual(1, result.Count());

			foreach (SchemaObjectBase obj in result)
			{
				Console.WriteLine("ID:{0}, Name: {1}", obj.ID, obj.Properties.GetValue("Name", string.Empty));
			}
		}

		[TestMethod]
		[TestCategory(Constants.ConditionCategory)]
		public void UpdateConditionTest()
		{
			string ownerID = UuidHelper.NewUuidString();

			SCConditionCollection conditions = new SCConditionCollection();

			conditions.Add(new SCCondition() { OwnerID = ownerID, SortID = 0, Condition = "Users.Status == 1" });
			conditions.Add(new SCCondition() { OwnerID = ownerID, SortID = 1, Condition = "Users.Status == 3" });

			SCConditionAdapter.Instance.UpdateConditions(ownerID, "Default", conditions);

			SCConditionOwner owner = SCConditionAdapter.Instance.Load(ownerID);

			Assert.AreEqual(ownerID, owner.OwnerID);
			Assert.AreEqual(conditions.Count, owner.Conditions.Count);

			for (int i = 0; i < owner.Conditions.Count; i++)
			{
				Assert.AreEqual(conditions[i].OwnerID, owner.Conditions[i].OwnerID);
				Assert.AreEqual(conditions[i].Condition, owner.Conditions[i].Condition);
			}
		}

		[TestMethod]
		[TestCategory(Constants.ConditionCategory)]
		public void DeleteConditionsContainerTest()
		{
			SCUser userInGroup;
			SCUser userNotInGroup;

			SCGroup group = SCObjectGenerator.PrepareGroupWithConditions(out userInGroup, out userNotInGroup);

			Console.WriteLine("Group ID: {0}", group.ID);

			SCObjectOperations.Instance.DeleteGroup(group, null, false);

			SCConditionOwner owner = SCConditionAdapter.Instance.Load(group.ID);

			owner.Conditions.ForEach(c => Assert.AreEqual(SchemaObjectStatus.Deleted, c.Status));
		}

		[TestMethod]
		[TestCategory(Constants.ConditionCategory)]
		public void DuplicateUpdateConditionTest()
		{
			string ownerID = UuidHelper.NewUuidString();

			SCConditionCollection conditions = new SCConditionCollection();

			conditions.Add(new SCCondition() { OwnerID = ownerID, SortID = 0, Condition = "Users.Status == 1" });
			conditions.Add(new SCCondition() { OwnerID = ownerID, SortID = 1, Condition = "Users.Status == 3" });

			DBTimePointActionContext.Current.DoActions(() => SCConditionAdapter.Instance.UpdateConditions(ownerID, "Default", conditions));

			SCConditionOwner originalOwner = SCConditionAdapter.Instance.Load(ownerID);

			conditions.RemoveAt(1);
			conditions[0].Condition = "Users.Status == 2";

			DBTimePointActionContext.Current.DoActions(() => SCConditionAdapter.Instance.UpdateConditions(ownerID, "Default", conditions));

			SCConditionOwner oldOwner = SCConditionAdapter.Instance.Load(ownerID, string.Empty, originalOwner.Conditions[0].VersionStartTime);

			Assert.AreEqual(2, oldOwner.Conditions.Count);
			Assert.AreEqual(originalOwner.Conditions[0].Condition, oldOwner.Conditions[0].Condition);

			SCConditionOwner newOwner = SCConditionAdapter.Instance.Load(ownerID);

			Assert.AreEqual(1, newOwner.Conditions.Count);
			Assert.AreEqual(conditions[0].Condition, newOwner.Conditions[0].Condition);
		}

		[TestMethod]
		[TestCategory(Constants.ConditionCategory)]
		[Description("带条件的群组功能测试")]
		public void SimpleGroupConditionsTest()
		{
			SCUser userInGroup;
			SCUser userNotInGroup;

			SCGroup group = SCObjectGenerator.PrepareGroupWithConditions(out userInGroup, out userNotInGroup);

			SCConditionCalculator calculator = new SCConditionCalculator();

			SCConditionOwner owner = SCConditionAdapter.Instance.Load(group.ID, string.Empty);

			SchemaObjectCollection calculateResult = calculator.Calculate(owner);

			ConditionCalculateResultAdapter.Instance.Update(group.ID, calculateResult);

			SchemaObjectCollection loadedResult = ConditionCalculateResultAdapter.Instance.LoadCurrentUsers(group.ID);

			Assert.IsTrue(loadedResult.ContainsKey(userInGroup.ID));
			Assert.IsFalse(loadedResult.ContainsKey(userNotInGroup.ID));
		}
	}
}
