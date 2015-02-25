using MCS.Library.SOA.DataObjects.Security.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using MCS.Library.SOA.DataObjects.Security.Test.SchemaObject;
using MCS.Library.SOA.DataObjects.Security;
using PC = MCS.Library.SOA.DataObjects.Security;
using System.Threading;

namespace MCS.Library.SOA.DataObjects.Security.Test.Permissions
{


	/// <summary>
	///这是 SCAclAdapterTest 的测试类，旨在
	///包含所有 SCAclAdapterTest 单元测试
	///</summary>
	[TestClass()]
	public class SCAclAdapterTest
	{


		private TestContext testContextInstance;

		/// <summary>
		///获取或设置测试上下文，上下文提供
		///有关当前测试运行及其功能的信息。
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region 附加测试特性
		// 
		//编写测试时，还可使用以下特性:
		//
		//使用 ClassInitialize 在运行类中的第一个测试前先运行代码
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//使用 ClassCleanup 在运行完类中的所有测试后再运行代码
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//使用 TestInitialize 在运行每个测试前先运行代码
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//使用 TestCleanup 在运行完每个测试后运行代码
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		/// <summary>
		///LoadCurrentContainerAndPermissions 的测试
		///</summary>
		[TestMethod()]
		public void LoadCurrentContainerAndPermissionsTest()
		{
			SCAclAdapter adapter = SCAclAdapter.Instance;

			SCObjectGenerator.PreareTestOguObjectForDelete();
			var parent1 = (SCOrganization)SchemaObjectAdapter.Instance.LoadByCodeName("Organizations", "groupHQ", DateTime.MinValue);

			var role1 = (SCRole)SchemaObjectAdapter.Instance.LoadByCodeName("Roles", "系统管理员", DateTime.MinValue);

			var role2 = (SCRole)SchemaObjectAdapter.Instance.LoadByCodeName("Roles", "系统维护员", DateTime.MinValue);

			var container = new PC.Permissions.SCAclContainer(parent1);

			container.Members.Add("AddChildren", role1);
			container.Members.Add("DeleteChildren", role1);

			container.Members.Add("UpdateChildren", role2);
			container.Members.Add("EditPermissionsOfChildren", role2);
			container.Members.Add("AddChildren", role2);

			PC.Executors.SCObjectOperations.Instance.UpdateObjectAcl(container);

			var user = (SCUser)SchemaObjectAdapter.Instance.LoadByCodeName("Users", "fanhy", DateTime.MinValue);
			Thread.Sleep(2000);

			var result = adapter.LoadCurrentContainerAndPermissions(user.ID, new string[] { parent1.ID });

			Assert.IsTrue((from PC.Permissions.SCContainerAndPermission p in result where p.ContainerPermission == "AddChildren" && p.ContainerID == parent1.ID select p).Any());

			Assert.IsTrue((from PC.Permissions.SCContainerAndPermission p in result where p.ContainerPermission == "DeleteChildren" && p.ContainerID == parent1.ID select p).Any());
		}
	}
}
