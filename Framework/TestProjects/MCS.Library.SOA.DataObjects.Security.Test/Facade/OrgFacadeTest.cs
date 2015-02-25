using System;
using System.Diagnostics;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Library.SOA.DataObjects.Security.Test.Conditions;
using MCS.Library.SOA.DataObjects.Security.Test.SchemaObject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Test.Facade
{
	[TestClass]
	public class OrgFacadeTest
	{
		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void AddOrganizationTest()
		{
			SCOrganization root = SCOrganization.GetRoot();

			SCOrganization newOrg = SCObjectGenerator.PrepareOrganizationObject();

			SCObjectOperations.Instance.AddOrganization(newOrg, root);

			SchemaObjectBase objLoaded = SchemaObjectAdapter.Instance.Load(newOrg.ID);

			Assert.AreEqual(newOrg.Name, objLoaded.Properties.GetValue("Name", string.Empty));
			Assert.IsTrue(objLoaded.CurrentParents.Count > 0);
			Assert.AreEqual(root.ID, objLoaded.CurrentParents[0].ID);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void UpdateOrganizationNameTest()
		{
			SCOrganization root = SCOrganization.GetRoot();

			SCOrganization newOrg = SCObjectGenerator.PrepareOrganizationObject();
			SCUser user = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddOrganization(newOrg, root);

			SchemaObjectBase objLoaded = SchemaObjectAdapter.Instance.Load(newOrg.ID);

			Assert.AreEqual(newOrg.Name, objLoaded.Properties.GetValue("Name", string.Empty));
			Assert.IsTrue(objLoaded.CurrentParents.Count > 0);
			Assert.AreEqual(root.ID, objLoaded.CurrentParents[0].ID);

			SCObjectOperations.Instance.AddUser(user, newOrg);

			newOrg.ClearRelativeData();

			newOrg.Name = "ChangedOrgName";

			SCObjectOperations.Instance.UpdateOrganization(newOrg);

			Console.WriteLine("Org ID: {0}, User ID: {1}", newOrg.ID, user.ID);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		[Description("进行创建对象")]
		[Conditional("DEBUG")]
		public void GenDataTest()
		{
			SCObjectGenerator.PreareTestOguObjectForDelete();
		}

		[TestMethod]
		public void MoveBackTest()
		{
			SchemaObjectAdapter.Instance.HistoryMoveBack(new DateTime(2012, 12, 18));
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		[Description("递归删除子对象，本场景模拟在组织下选一堆对象进行删除")]
		public void DeleteObjectsRecursivelyTest()
		{

			// 调用SCObjectGenerator，准备测试数据，包括人员、组织群组、组织中包含子组织和人员
			SCObjectGenerator.PreareTestOguObjectForDelete();
			SCOrganization parent = (SCOrganization)SchemaObjectAdapter.Instance.Load("91841971-44CB-4895-8B31-D9EA7432A74A"); // cityCompany

			SchemaObjectCollection objectsToDelete = new SchemaObjectCollection() 
			{
                SchemaObjectAdapter.Instance.Load ("471F24D5-962E-46B9-849B-639D0AEB2B16") // beijingYuanlian
            };

			ProcessProgress.Clear();
			ProcessProgress.Current.RegisterResponser(TestProgressResponser.Instance);

			// 执行删除，过程中会输出状态信息
			SCObjectOperations.Instance.DeleteObjectsRecursively(objectsToDelete, parent);

			// 输出
			Console.Error.WriteLine("Error: {0}", ProcessProgress.Current.GetDefaultError());
			Console.WriteLine("Output: {0}", ProcessProgress.Current.GetDefaultOutput());

			// 验证结果

			Assert.IsFalse(this.ObjectDeleted("2B67F7D0-9362-401F-977F-3E267E87298B")); // wangfaping
			Assert.IsFalse(this.ObjectDeleted("3729DAC3-80E0-476C-8C4A-264E0F67BBC2")); // liumh
			Assert.IsTrue(this.ObjectDeleted("066352AA-8349-4D21-B83F-C909BA5B8352"));  // beijingYuanlianExecutives
			Assert.IsTrue(this.ObjectDeleted("2F28C437-BBF9-4969-9C07-639BD9716B1E")); // yuanyangAobei

			Assert.IsFalse(this.ObjectDeleted("16903BF9-74B5-4B58-9204-8BB20F341D88")); // beijingZhonglian

			Assert.IsTrue(this.ObjectDeleted("CA093A1E-B207-48DB-B3B2-B085A81DA36A")); // groupA

			Assert.IsTrue(this.ObjectDeleted("A465FFC8-A742-41F3-A1B6-0D40FC5EA3D5")); // groupB

			Assert.IsFalse(this.ObjectDeleted("D1C28431-DD5D-496E-865B-85C6D89ED3D6")); // zhaolin1
		}

		private bool ObjectDeleted(string id)
		{
			return this.ObjectNullOrDeleted(SchemaObjectAdapter.Instance.Load(id));
		}

		private bool ObjectNullOrDeleted(SchemaObjectBase schemaObjectBase)
		{
			return (schemaObjectBase == null || schemaObjectBase.Status != SchemaObjectStatus.Normal);
		}

		private bool ObjectDeleted(string schemaType, string codeName)
		{
			return this.ObjectNullOrDeleted(SchemaObjectAdapter.Instance.LoadByCodeName(schemaType, codeName, DateTime.MinValue));
		}
	}
}
