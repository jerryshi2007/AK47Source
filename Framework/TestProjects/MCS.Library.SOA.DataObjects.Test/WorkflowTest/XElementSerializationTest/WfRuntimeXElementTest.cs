using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Core;
using System.Xml.Linq;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects.Test
{
	/// <summary>
	/// Summary description for WfRuntimeXElementTest
	/// </summary>
	[TestClass]
	public class WfRuntimeXElementTest
	{
		public WfRuntimeXElementTest()
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


		[TestMethod]
		[TestCategory(ProcessTestHelper.XElementSerialize)]
		[Description("简单流程实例的XElemnt序列化测试")]
		public void SimpleProcessRuntimeSerializeTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			if (process.Activities != null)
			{
				foreach (var activity in process.Activities)
				{
					if (activity.Descriptor != process.Descriptor.InitialActivity)
					{
						Assert.IsTrue(activity.Status == WfActivityStatus.NotRunning);
						WfTransferParams transferPara = new WfTransferParams(activity.Descriptor);
						process.MoveTo(transferPara);
					}
				}
			}

			XElementFormatter formatter = new XElementFormatter();

			XElement root = formatter.Serialize(process);

			Console.WriteLine(root.ToString());

			IWfProcess clonedProcess = (IWfProcess)formatter.Deserialize(root);

			Assert.IsTrue(clonedProcess.Status == WfProcessStatus.Completed);

			for (int i = 0; i < clonedProcess.Activities.Count; i++)
			{
				Assert.IsTrue(clonedProcess.Activities[i].Status == WfActivityStatus.Completed);

				if (i < clonedProcess.Activities.Count - 1)
				{
					Assert.AreEqual(clonedProcess.Activities[i].Descriptor.ToTransitions[0], clonedProcess.Activities[i + 1].Descriptor.FromTransitions[0]);
				}

			}

			Assert.IsNotNull(clonedProcess.Descriptor.Activities[process.Descriptor.InitialActivity.Key], "验证反序列化后集合字典的完整性");

			XElement reRoot = formatter.Serialize(clonedProcess);
			//31000是否允许被撤回默认值不同true false

			Assert.AreEqual(root.ToString(), reRoot.ToString());

			
			IWfProcess reClonedProcess = (IWfProcess)formatter.Deserialize(reRoot);

			Assert.AreEqual(clonedProcess.ToString(), reClonedProcess.ToString());

			
		}


		[TestMethod]
		[Description("流程序列化的执行时间测试")]
		[TestCategory(ProcessTestHelper.ExecuteTime)]
		public void ProcessSerializeExecutionTimeTest()
		{
			IWfProcessDescriptor processDesc = WfProcessTestCommon.CreateSimpleProcessDescriptor();

			int totalProcesses = 40;

			//准备流程实例
			List<IWfProcess> processes = ProcessTestHelper.StartupMultiProcesses(processDesc, totalProcesses);

			ProcessTestHelper.OutputExecutionTime(() =>
			{
				foreach (IWfProcess process in processes)
				{
					XElementFormatter formatter = new XElementFormatter();
					XElement root = formatter.Serialize(process);
				}
			},
				string.Format("序列化{0}个流程", totalProcesses));
		}

	}
}
