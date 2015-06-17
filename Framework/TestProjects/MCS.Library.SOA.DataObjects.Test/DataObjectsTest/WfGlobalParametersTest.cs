using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MCS.Library.SOA.DataObjects.Test.DataObjectsTest
{
	/// <summary>
	/// Summary description for WfGlobalParametersTest
	/// </summary>
	[TestClass]
	public class WfGlobalParametersTest
	{
		public WfGlobalParametersTest()
		{
		}

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
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

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

        /// <summary>
        /// 已经迁移到MCS.Library.SOA.DataObjects.Tenant.Test
        /// </summary>
		[TestMethod]
		[Description("全局设置的测试")]
		[TestCategory(ProcessTestHelper.BasicDataObject)]
		public void WfGlobalParametersReadTest()
		{
			WfGlobalParameters settings = WfGlobalParameters.LoadProperties("Default");

			settings.Properties.SetValue("AppName", "SinoOcean");

			settings.Update();

			Thread.Sleep(500);

			WfGlobalParameters settingsLoaded = WfGlobalParameters.Default;

			Assert.AreEqual(settings.Properties["AppName"].StringValue,
				settingsLoaded.Properties["AppName"].StringValue);
		}

        /// <summary>
        /// 已经迁移到MCS.Library.SOA.DataObjects.Tenant.Test
        /// </summary>
		[TestMethod]
		[Description("App和Program的参数读取测试")]
		[TestCategory(ProcessTestHelper.BasicDataObject)]
		public void WfGlobalAppProgramParametersReadTest()
		{
			WfGlobalParameters settings = WfGlobalParameters.LoadProperties("ADMINISTRATION", "CONTRACT");

			settings.Properties.SetValue("AppName", "Seagull II");

			settings.Update();

			Thread.Sleep(500);

			WfGlobalParameters settingsLoaded = WfGlobalParameters.GetProperties("ADMINISTRATION", "CONTRACT");

			Assert.AreEqual(settings.Properties["AppName"].StringValue,
				settingsLoaded.Properties["AppName"].StringValue);
		}

        /// <summary>
        /// 已经迁移到MCS.Library.SOA.DataObjects.Tenant.Test
        /// </summary>
		[TestMethod]
		[Description("App和Program的参数类别大小写无关读取测试")]
		[TestCategory(ProcessTestHelper.BasicDataObject)]
		public void WfGlobalAppProgramParametersCaseInsensitiveReadTest()
		{
			WfGlobalParameters settings = WfGlobalParameters.GetProperties("ADMINISTRATION", "CONTRACT");

			settings.Properties.SetValue("AppName", "Seagull II");

			settings.Update();

			Thread.Sleep(500);

			WfGlobalParameters settingsLoaded = WfGlobalParameters.GetProperties("Administration", "Contract");
			//读两遍，仅用于确认Cache
			settingsLoaded = WfGlobalParameters.GetProperties("Administration", "CONTRACT");
			
			Assert.AreEqual(settings.Properties["AppName"].StringValue,
				settingsLoaded.Properties["AppName"].StringValue);
		}

        /// <summary>
        /// 已经迁移到MCS.Library.SOA.DataObjects.Tenant.Test
        /// </summary>
		[TestMethod]
		[Description("App和Program的递归参数读取测试")]
		[TestCategory(ProcessTestHelper.BasicDataObject)]
		public void WfGlobalAppProgramParametersRecursivelyReadTest()
		{
			WfGlobalParameters defaultSettings = WfGlobalParameters.LoadProperties("Default");

			defaultSettings.Properties.SetValue("AppName", "SinoOcean");
			defaultSettings.Properties.SetValue("ProgName", "DefaultProg");

			defaultSettings.Update();

			WfGlobalParameters appSettings = WfGlobalParameters.LoadProperties("ADMINISTRATION", "CONTRACT");

			appSettings.Properties.SetValue("AppName", string.Empty);
			appSettings.Properties.SetValue("ProgName", "ContractProg");

			appSettings.Update();

			Thread.Sleep(500);

			string pvApp = WfGlobalParameters.GetValueRecursively("ADMINISTRATION", "CONTRACT", "AppName", "DefaultValue");

			Assert.AreEqual("SinoOcean", pvApp);

			string pvProg = WfGlobalParameters.GetValueRecursively("ADMINISTRATION", "CONTRACT", "ProgName", "DefaultValue");

			Assert.AreEqual("ContractProg", pvProg);
		}

		[TestMethod]
		[Description("读取所有应用信息")]
		[TestCategory(ProcessTestHelper.BasicDataObject)]
		public void LoadAllApplicationsTest()
		{
			WfApplicationCollection applications = WfApplicationAdapter.Instance.LoadAll();

			applications.ForEach(app => Console.WriteLine("CodeName={0}, Name={1}", app.CodeName, app.Name));
		}

		[TestMethod]
		[Description("读取所有应用信息")]
		[TestCategory(ProcessTestHelper.BasicDataObject)]
		public void LoadAllProgramsInApplicationsTest()
		{
			WfApplicationCollection applications = WfApplicationAdapter.Instance.LoadAll();

			foreach (WfApplication app in applications)
			{
				Console.WriteLine("App: CodeName={0}, Name={1}", app.CodeName, app.Name);

				WfProgramInApplicationCollection programs = WfApplicationAdapter.Instance.LoadProgramsByApplication(app.CodeName);

				programs.ForEach(prog => Console.WriteLine("CodeName={0}, Name={1}", prog.CodeName, prog.Name));

				Console.WriteLine();
			}
		}
	}
}
