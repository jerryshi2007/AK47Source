using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Security.Test.SchemaObject;
using MCS.Library.SOA.DataObjects.Security.Conditions;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.Test.Conditions
{
	[TestClass]
	public class ConditionAndSnapshotTest
	{
		[TestMethod]
		[TestCategory(Constants.ConditionCategory)]
		[Description("带条件的角色、群组功能测试")]
		public void GenerateUserAndContainerSnapshotTest()
		{
			TestRoleData roleData = SCObjectGenerator.PrepareTestRoleWithOrgAndGroup();

			SCConditionCalculator calculator = new SCConditionCalculator();

			ProcessProgress.Clear();
			ProcessProgress.Current.RegisterResponser(TestProgressResponser.Instance);

			calculator.GenerateUserAndContainerSnapshot(new List<ISCUserContainerObject>() { roleData.Group });
			calculator.GenerateUserAndContainerSnapshot(new List<ISCUserContainerObject>() { roleData.Role });

			SameContainerUserAndContainerSnapshotCollection snapshot = UserAndContainerSnapshotAdapter.Instance.LoadByContainerID(roleData.Role.ID);

			Console.WriteLine(roleData.ToString());
			Console.Error.WriteLine("Error: {0}", ProcessProgress.Current.GetDefaultError());
			Console.WriteLine("Output: {0}", ProcessProgress.Current.GetDefaultOutput());

			Assert.IsTrue(snapshot.ContainsKey(roleData.UserInGroup.ID));
			Assert.IsTrue(snapshot.ContainsKey(roleData.UserInOrg.ID));
			Assert.IsTrue(snapshot.ContainsKey(roleData.UserInRole.ID));
			Assert.IsTrue(snapshot.ContainsKey(roleData.UserInConditionRole.ID));
			Assert.IsTrue(snapshot.ContainsKey(roleData.UserInConditionGroup.ID));
		}

		[TestMethod]
		[TestCategory(Constants.ConditionCategory)]
		[Description("计算所有角色、群组中的人员功能测试")]
		public void GenerateAllUserAndContainerSnapshotTest()
		{
			ProcessProgress.Clear();
			ProcessProgress.Current.RegisterResponser(TestProgressResponser.Instance);

			TestRoleData roleData = SCObjectGenerator.PrepareTestRoleWithOrgAndGroup();

			SCConditionCalculator calculator = new SCConditionCalculator();

			calculator.GenerateAllUserAndContainerSnapshot();

			Console.Error.WriteLine("Error: {0}", ProcessProgress.Current.GetDefaultError());
			Console.WriteLine("Output: {0}", ProcessProgress.Current.GetDefaultOutput());
		}

		[TestMethod]
		[TestCategory(Constants.ConditionCategory)]
		[Description("计算内置函数IsChild(CodeName)")]
		public void IsChildByCodeNameBuiltInFunctionTest()
		{
			SchemaObjectAdapter.Instance.ClearAllData();

			TestRoleData roleData = SCObjectGenerator.PrepareTestRoleWithOrgAndGroup(data => string.Format("IsChild(\"{0}\", \"{1}\")", "CodeName", data.Organization.CodeName));

			SCConditionCalculator calculator = new SCConditionCalculator();

			ProcessProgress.Clear();
			ProcessProgress.Current.RegisterResponser(TestProgressResponser.Instance);

			calculator.GenerateUserAndContainerSnapshot(new List<ISCUserContainerObject>() { roleData.BuiltInFunctionRole });

			SameContainerUserAndContainerSnapshotCollection snapshot = UserAndContainerSnapshotAdapter.Instance.LoadByContainerID(roleData.BuiltInFunctionRole.ID);

			Console.WriteLine(roleData.ToString());
			Console.Error.WriteLine("Error: {0}", ProcessProgress.Current.GetDefaultError());
			Console.WriteLine("Output: {0}", ProcessProgress.Current.GetDefaultOutput());

			Assert.IsTrue(snapshot.ContainsKey(roleData.UserInOrg.ID));
		}

		[TestMethod]
		[TestCategory(Constants.ConditionCategory)]
		[Description("计算内置函数IsChildAll(ID)")]
		public void IsChildAllByIDBuiltInFunctionTest()
		{
			SchemaObjectAdapter.Instance.ClearAllData();

			TestRoleData roleData = SCObjectGenerator.PrepareTestRoleWithOrgAndGroup(data => string.Format("IsChildAll(\"{0}\", \"{1}\")", "Guid", data.SidelineOrganization.ID));

			SCConditionCalculator calculator = new SCConditionCalculator();

			ProcessProgress.Clear();
			ProcessProgress.Current.RegisterResponser(TestProgressResponser.Instance);

			calculator.GenerateUserAndContainerSnapshot(new List<ISCUserContainerObject>() { roleData.BuiltInFunctionRole });

			SameContainerUserAndContainerSnapshotCollection snapshot = UserAndContainerSnapshotAdapter.Instance.LoadByContainerID(roleData.BuiltInFunctionRole.ID);

			Console.WriteLine(roleData.ToString());
			Console.Error.WriteLine("Error: {0}", ProcessProgress.Current.GetDefaultError());
			Console.WriteLine("Output: {0}", ProcessProgress.Current.GetDefaultOutput());

			Assert.IsFalse(snapshot.ContainsKey(roleData.SidelineUserInOrg.ID));
		}

		[TestMethod]
		[TestCategory(Constants.ConditionCategory)]
		[Description("计算内置函数IsChild(ID)")]
		public void IsChildByIDBuiltInFunctionTest()
		{
			SchemaObjectAdapter.Instance.ClearAllData();

			TestRoleData roleData = SCObjectGenerator.PrepareTestRoleWithOrgAndGroup(data => string.Format("IsChild(\"{0}\", \"{1}\")", "Guid", data.Organization.ID));

			SCConditionCalculator calculator = new SCConditionCalculator();

			ProcessProgress.Clear();
			ProcessProgress.Current.RegisterResponser(TestProgressResponser.Instance);

			calculator.GenerateUserAndContainerSnapshot(new List<ISCUserContainerObject>() { roleData.BuiltInFunctionRole });

			SameContainerUserAndContainerSnapshotCollection snapshot = UserAndContainerSnapshotAdapter.Instance.LoadByContainerID(roleData.BuiltInFunctionRole.ID);

			Console.WriteLine(roleData.ToString());
			Console.Error.WriteLine("Error: {0}", ProcessProgress.Current.GetDefaultError());
			Console.WriteLine("Output: {0}", ProcessProgress.Current.GetDefaultOutput());

			Assert.IsTrue(snapshot.ContainsKey(roleData.UserInOrg.ID));
		}

		[TestMethod]
		[TestCategory(Constants.ConditionCategory)]
		[Description("计算内置函数IsChild(FullPath)")]
		public void IsChildByFullPathBuiltInFunctionTest()
		{
			SchemaObjectAdapter.Instance.ClearAllData();

			TestRoleData roleData = SCObjectGenerator.PrepareTestRoleWithOrgAndGroup(data => string.Format("IsChild(\"{0}\", \"{1}\")", "FullPath", data.Organization.CurrentParentRelations.FirstOrDefault().FullPath));

			SCConditionCalculator calculator = new SCConditionCalculator();

			ProcessProgress.Clear();
			ProcessProgress.Current.RegisterResponser(TestProgressResponser.Instance);

			calculator.GenerateUserAndContainerSnapshot(new List<ISCUserContainerObject>() { roleData.BuiltInFunctionRole });

			SameContainerUserAndContainerSnapshotCollection snapshot = UserAndContainerSnapshotAdapter.Instance.LoadByContainerID(roleData.BuiltInFunctionRole.ID);

			Console.WriteLine(roleData.ToString());
			Console.Error.WriteLine("Error: {0}", ProcessProgress.Current.GetDefaultError());
			Console.WriteLine("Output: {0}", ProcessProgress.Current.GetDefaultOutput());

			Assert.IsTrue(snapshot.ContainsKey(roleData.UserInOrg.ID));
		}

		[TestMethod]
		[TestCategory(Constants.ConditionCategory)]
		[Description("计算内置函数IsDescendantAll(CodeName)")]
		public void IsDescendantAllByCodeNameBuiltInFunctionTest()
		{
			SchemaObjectAdapter.Instance.ClearAllData();

			TestRoleData roleData = SCObjectGenerator.PrepareTestRoleWithOrgAndGroup(data => string.Format("IsDescendantAll(\"{0}\", \"{1}\")", "CodeName", data.SidelineUserInOrg.CodeName));

			SCConditionCalculator calculator = new SCConditionCalculator();

			ProcessProgress.Clear();
			ProcessProgress.Current.RegisterResponser(TestProgressResponser.Instance);

			calculator.GenerateUserAndContainerSnapshot(new List<ISCUserContainerObject>() { roleData.BuiltInFunctionRole });

			SameContainerUserAndContainerSnapshotCollection snapshot = UserAndContainerSnapshotAdapter.Instance.LoadByContainerID(roleData.BuiltInFunctionRole.ID);

			Console.WriteLine(roleData.ToString());
			Console.Error.WriteLine("Error: {0}", ProcessProgress.Current.GetDefaultError());
			Console.WriteLine("Output: {0}", ProcessProgress.Current.GetDefaultOutput());

			Assert.IsFalse(snapshot.ContainsKey(roleData.SidelineUserInOrg.ID));
		}

		[TestMethod]
		[TestCategory(Constants.ConditionCategory)]
		[Description("计算内置函数IsDescendant(CodeName)")]
		public void IsDescendantByCodeNameBuiltInFunctionTest()
		{
			SchemaObjectAdapter.Instance.ClearAllData();

			TestRoleData roleData = SCObjectGenerator.PrepareTestRoleWithOrgAndGroup(data => string.Format("IsDescendant(\"{0}\", \"{1}\")", "CodeName", data.Organization.CodeName));

			SCConditionCalculator calculator = new SCConditionCalculator();

			ProcessProgress.Clear();
			ProcessProgress.Current.RegisterResponser(TestProgressResponser.Instance);

			calculator.GenerateUserAndContainerSnapshot(new List<ISCUserContainerObject>() { roleData.BuiltInFunctionRole });

			SameContainerUserAndContainerSnapshotCollection snapshot = UserAndContainerSnapshotAdapter.Instance.LoadByContainerID(roleData.BuiltInFunctionRole.ID);

			Console.WriteLine(roleData.ToString());
			Console.Error.WriteLine("Error: {0}", ProcessProgress.Current.GetDefaultError());
			Console.WriteLine("Output: {0}", ProcessProgress.Current.GetDefaultOutput());

			Assert.IsTrue(snapshot.ContainsKey(roleData.UserInOrg.ID));
		}

		[TestMethod]
		[TestCategory(Constants.ConditionCategory)]
		[Description("计算内置函数IsDescendant(ID)")]
		public void IsDescendantByIDBuiltInFunctionTest()
		{
			SchemaObjectAdapter.Instance.ClearAllData();

			TestRoleData roleData = SCObjectGenerator.PrepareTestRoleWithOrgAndGroup(data => string.Format("IsDescendant(\"{0}\", \"{1}\")", "Guid", data.Organization.ID));

			SCConditionCalculator calculator = new SCConditionCalculator();

			ProcessProgress.Clear();
			ProcessProgress.Current.RegisterResponser(TestProgressResponser.Instance);

			calculator.GenerateUserAndContainerSnapshot(new List<ISCUserContainerObject>() { roleData.BuiltInFunctionRole });

			SameContainerUserAndContainerSnapshotCollection snapshot = UserAndContainerSnapshotAdapter.Instance.LoadByContainerID(roleData.BuiltInFunctionRole.ID);

			Console.WriteLine(roleData.ToString());
			Console.Error.WriteLine("Error: {0}", ProcessProgress.Current.GetDefaultError());
			Console.WriteLine("Output: {0}", ProcessProgress.Current.GetDefaultOutput());

			Assert.IsTrue(snapshot.ContainsKey(roleData.UserInOrg.ID));
		}

		[TestMethod]
		[TestCategory(Constants.ConditionCategory)]
		[Description("计算内置函数IsDescendant(FullPath)")]
		public void IsDescendantByFullPathBuiltInFunctionTest()
		{
			SchemaObjectAdapter.Instance.ClearAllData();

			TestRoleData roleData = SCObjectGenerator.PrepareTestRoleWithOrgAndGroup(data => string.Format("IsDescendant(\"{0}\", \"{1}\")", "FullPath", data.Organization.CurrentParentRelations.FirstOrDefault().FullPath));

			SCConditionCalculator calculator = new SCConditionCalculator();

			ProcessProgress.Clear();
			ProcessProgress.Current.RegisterResponser(TestProgressResponser.Instance);

			calculator.GenerateUserAndContainerSnapshot(new List<ISCUserContainerObject>() { roleData.BuiltInFunctionRole });

			SameContainerUserAndContainerSnapshotCollection snapshot = UserAndContainerSnapshotAdapter.Instance.LoadByContainerID(roleData.BuiltInFunctionRole.ID);

			Console.WriteLine(roleData.ToString());
			Console.Error.WriteLine("Error: {0}", ProcessProgress.Current.GetDefaultError());
			Console.WriteLine("Output: {0}", ProcessProgress.Current.GetDefaultOutput());

			Assert.IsTrue(snapshot.ContainsKey(roleData.UserInOrg.ID));
		}
	}
}
