using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.OGUPermission;
using MCS.Library.Passport;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.Script;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Test
{
	/// <summary>
	/// Summary description for WfDescriptorTest
	/// </summary>
	[TestClass]
	public class WfDescriptorTest
	{
		public WfDescriptorTest()
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
		[Description("简单流程描述的XElement序列化测试")]
		public void SimpleProcessDescriptorSerializeTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

			XElementFormatter formatter = new XElementFormatter();

			XElement root = formatter.Serialize(processDesp);

			Console.WriteLine(root.ToString());

			IWfProcessDescriptor clonedProcessDesp = (IWfProcessDescriptor)formatter.Deserialize(root);

			Assert.IsTrue(clonedProcessDesp.InitialActivity.CanReachTo(clonedProcessDesp.CompletedActivity));
			Assert.AreEqual(clonedProcessDesp, clonedProcessDesp.InitialActivity.Process);
			Assert.AreEqual(clonedProcessDesp, clonedProcessDesp.CompletedActivity.Process);
			//因增加了一个活动点,此处这样比较断言肯定失败.故此将其注销掉
			//Assert.AreEqual(clonedProcessDesp.InitialActivity.ToTransitions[0], clonedProcessDesp.CompletedActivity.FromTransitions[0]);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.XElementSerialize)]
		[Description("带资源的简单流程描述的XElement序列化测试")]
		public void SimpleProcessDescriptorWithResourceSerializeTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

			processDesp.InitialActivity.Resources.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object));

			XElementFormatter formatter = new XElementFormatter();

			XElement root = formatter.Serialize(processDesp);

			Console.WriteLine(root.ToString());

			IWfProcessDescriptor clonedProcessDesp = (IWfProcessDescriptor)formatter.Deserialize(root);

			Assert.IsTrue(clonedProcessDesp.InitialActivity.CanReachTo(clonedProcessDesp.CompletedActivity));
			Assert.AreEqual(processDesp.InitialActivity.Resources.Count, clonedProcessDesp.InitialActivity.Resources.Count);

			Assert.AreEqual(clonedProcessDesp, clonedProcessDesp.InitialActivity.Process);
			Assert.AreEqual(clonedProcessDesp, clonedProcessDesp.CompletedActivity.Process);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.XElementSerialize)]
		[Description("带部门的简单流程的XElement序列化测试")]
		public void SimpleProcessDescriptorWithDeptResSerializeTest()
		{
			WfConverterHelper.RegisterConverters();

			WfProcessDescriptor processDesp = (WfProcessDescriptor)WfProcessTestCommon.CreateSimpleProcessDescriptor();

			IUser user = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object;
			IOrganization orga = user.Parent;
			WfDepartmentResourceDescriptor deptResuDesp = new WfDepartmentResourceDescriptor(orga);

			processDesp.InitialActivity.Resources.Add(deptResuDesp);

			string result = JSONSerializerExecute.Serialize(processDesp);

			processDesp = JSONSerializerExecute.Deserialize<WfProcessDescriptor>(result);

			XElementFormatter formatter = new XElementFormatter();

			XElement root = formatter.Serialize(processDesp);

			Console.WriteLine(root.ToString());

			IWfProcessDescriptor clonedProcessDesp = (IWfProcessDescriptor)formatter.Deserialize(root);

		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.XElementSerialize)]
		[Description("Process的Context存在内容时的XElement序列化测试")]
		public void ProcessSerializeTest()
		{
			IWfProcessDescriptor processDesc = WfProcessTestCommon.CreateSimpleProcessDescriptor();

			WfProcessStartupParams startupParams = new WfProcessStartupParams();
			startupParams.ProcessDescriptor = processDesc;
			IWfProcess process = WfRuntime.StartWorkflow(startupParams);
			((WfProcess)process).ResourceID = UuidHelper.NewUuidString();

			WfProcessContext context = process.Context;
			context.Add("UCC", "the same");

			XElementFormatter formatter = new XElementFormatter();

			//formatter.OutputShortType = false;

			XElement root = formatter.Serialize(process);

			Console.WriteLine(root.ToString());

			IWfProcess clonedProcess = (IWfProcess)formatter.Deserialize(root);

			Assert.IsNotNull(clonedProcess.Context["UCC"]);
			Assert.AreEqual(process.Context.Count, clonedProcess.Context.Count);
			Assert.AreEqual(process.Context["UCC"], clonedProcess.Context["UCC"]);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.XElementSerialize)]
		[Description("Activity的Context存在内容时的XElement序列化测试")]
		public void ActivitySerializeTest()
		{
			IWfProcessDescriptor processDesc = WfProcessTestCommon.CreateSimpleProcessDescriptor();

			WfProcessStartupParams startupParams = new WfProcessStartupParams();
			startupParams.ProcessDescriptor = processDesc;
			IWfProcess process = WfRuntime.StartWorkflow(startupParams);

			Sky sky = new Sky();
			sky.air = "清新";
			sky.Cloud = 1;
			Sky space = new Sky();
			space.air = "干净";
			space.Cloud = 1;

			process.InitialActivity.Context.Add("DDO", sky);
			process.InitialActivity.Context.Add("DFO", space);

			XElementFormatter formatter = new XElementFormatter();
			XElement root = formatter.Serialize(process);

			Console.WriteLine(root.ToString());

			IWfProcess clonedProcess = (IWfProcess)formatter.Deserialize(root);

			Assert.IsNotNull(clonedProcess.InitialActivity.Context["DDO"]);
			Assert.AreEqual(process.InitialActivity.Context.Count, clonedProcess.InitialActivity.Context.Count);
			Assert.AreEqual(((Sky)process.InitialActivity.Context["DDO"]).air, ((Sky)clonedProcess.InitialActivity.Context["DDO"]).air);
			Assert.AreEqual(((Sky)process.InitialActivity.Context["DDO"]).Cloud, ((Sky)clonedProcess.InitialActivity.Context["DDO"]).Cloud);
			Assert.AreEqual(((Sky)process.InitialActivity.Context["DFO"]).air, ((Sky)clonedProcess.InitialActivity.Context["DFO"]).air);
			Assert.AreEqual(((Sky)process.InitialActivity.Context["DFO"]).Cloud, ((Sky)clonedProcess.InitialActivity.Context["DFO"]).Cloud);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.XElementSerialize)]
		[Description("测试相关外部人员的XElement序列化测试")]
		public void WfExternalUserTest()
		{
			WfExternalUser externalUser = new WfExternalUser();
			externalUser.Key = "user0";
			externalUser.Name = "zLing";
			externalUser.Gender = Gender.Female;
			externalUser.Email = "zhangling@artmterch.com";
			externalUser.MobilePhone = "13552630000";
			externalUser.Phone = "0409987";
			externalUser.Title = "programer";

			IWfProcessDescriptor processDesc = WfProcessTestCommon.CreateSimpleProcessDescriptor();

			processDesc.ExternalUsers.Add(externalUser);

			WfProcessStartupParams startupParams = new WfProcessStartupParams();
			startupParams.ProcessDescriptor = processDesc;
			IWfProcess process = WfRuntime.StartWorkflow(startupParams);

			XElementFormatter formatter = new XElementFormatter();

			XElement rootProc = formatter.Serialize(process);
			IWfProcess clonedProcess = (IWfProcess)formatter.Deserialize(rootProc);

			XElement resultProc = formatter.Serialize(clonedProcess);

			Assert.AreEqual(processDesc.ExternalUsers[0].Name, clonedProcess.Descriptor.ExternalUsers[0].Name);
			Assert.AreEqual(processDesc.ExternalUsers[0].Gender, clonedProcess.Descriptor.ExternalUsers[0].Gender);

			Assert.AreEqual(rootProc.ToString(), resultProc.ToString());
		}

		/// <summary>
		/// 矩阵资源的Xml序列化测试
		/// </summary>
		[TestMethod]
		[TestCategory(ProcessTestHelper.XElementSerialize)]
		public void WfActivityMatrixResourceSerializationTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.GetDynamicProcessDesp();

			XElementFormatter formatter = new XElementFormatter();

			XElement rootProc = formatter.Serialize(processDesp);
			IWfProcessDescriptor clonedProcessDesp = (IWfProcessDescriptor)formatter.Deserialize(rootProc);

			IWfActivityDescriptor normalActDesp = processDesp.Activities["NormalActivity"];
			IWfActivityDescriptor clonedNormalActDesp = clonedProcessDesp.Activities["NormalActivity"];

			Assert.IsNotNull(normalActDesp);
			Assert.IsNotNull(clonedNormalActDesp);

			WfActivityMatrixResourceDescriptor matrixResource = (WfActivityMatrixResourceDescriptor)normalActDesp.Resources[0];
			WfActivityMatrixResourceDescriptor clonedMatrixResource = (WfActivityMatrixResourceDescriptor)clonedNormalActDesp.Resources[0];

			Assert.AreEqual(matrixResource.PropertyDefinitions.Count, clonedMatrixResource.PropertyDefinitions.Count);
			Assert.AreEqual(matrixResource.Rows.Count, clonedMatrixResource.Rows.Count);
		}

		//[TestMethod]
		//[TestCategory(ProcessTestHelper.XElementSerialize)] 
		//public void AddAssigneeToProcessDesp()
		//{
		//	XmlDocument doc = new XmlDocument();
		//	doc.Load (@"D:\DATA1.xml");

		//	string text = doc.InnerXml;

		//	XElement ele= XElement.Parse(text);

		//	XElementFormatter formatter = new XElementFormatter();

		//	IWfProcess proc = (IWfProcess)formatter.Deserialize(ele);

		//	IWfActivityDescriptor actDesp = proc.Descriptor.Activities ["N13"];
		//	if (actDesp !=null )
		//	{
		//		Assert.AreEqual(proc.CurrentActivity.Status , WfActivityStatus.Running);
		//		Assert.AreEqual(proc.CurrentActivity.Assignees.Count, 0);


		//		IUser user = new OguUser("8a84254f-2d8f-4082-b762-7c917549b678");
		//		string name = user.DisplayName;
		//		proc.CurrentActivity.Assignees.Add(user);
		//	}

		//	XElement rootProc = formatter.Serialize(proc);

		//	Assert.AreNotEqual(text, rootProc.Value);
		//}
	}


	[Serializable]
	[XElementSerializable]
	public class Sky
	{
		private int _cloud;
		public int Cloud { get { return _cloud; } set { _cloud = value; } }

		private string _air;
		public string air { get { return _air; } set { _air = value; } }
	}
}
