using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.OGUPermission;
using MCS.Library.Data;
using MCS.Library.Data.Builder;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest.JsonSerializationTest
{
	/// <summary>
	/// Summary description for WfProcessDescriptorJsonTest
	/// </summary>
	[TestClass]
	public class WfProcessDescriptorJsonTest
	{
		public WfProcessDescriptorJsonTest()
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
		[TestCategory(ProcessTestHelper.JSONConverter)]
		[Description("测试流程描述的JSON序列化")]
		public void WfProcessDescriptorConverter()
		{
			WfConverterHelper.RegisterConverters();
			WfProcessDescriptor processDesp = (WfProcessDescriptor)WfProcessTestCommon.CreateSimpleProcessDescriptor();

			string result = JSONSerializerExecute.Serialize(processDesp);

			Console.WriteLine(result);

			WfProcessDescriptor deserializedProcessDesp = JSONSerializerExecute.Deserialize<WfProcessDescriptor>(result);

			//再次序列化时,字条串不相等,是因为创建流程时没有Key
			string reSerialized = JSONSerializerExecute.Serialize(deserializedProcessDesp);

			Console.WriteLine(reSerialized);
			Assert.AreEqual(result, reSerialized);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		[Description("测试节点描述的JSON序列化")]
		public void WfActivitySerilizeConverterTest()
		{
			WfConverterHelper.RegisterConverters();

			WfActivityDescriptor normalAct = new WfActivityDescriptor("NormalActivity", WfActivityType.NormalActivity);
			normalAct.Name = "Normal";
			normalAct.CodeName = "Normal Activity";

			WfServiceOperationDefinition enterSvcDef = new WfServiceInvokerFactory().SvcOpDef;
			enterSvcDef.AddressDef.RequestMethod = WfServiceRequestMethod.Get;
			enterSvcDef.OperationName = "StringTypeService";
			enterSvcDef.RtnXmlStoreParamName = "EnterServiceRtnXml";

			enterSvcDef.Params.Add(new WfServiceOperationParameter()
			{
				Name = "input",
				Type = WfSvcOperationParameterType.String,
				Value = "this is a get action!"
			});

			normalAct.EnterEventExecuteServices.Add(enterSvcDef);

			string result = JSONSerializerExecute.Serialize(normalAct);

			Console.WriteLine(result);

			WfActivityDescriptor deserializedActDesp = JSONSerializerExecute.Deserialize<WfActivityDescriptor>(result);

			string reSerialized = JSONSerializerExecute.Serialize(deserializedActDesp);

			Assert.AreEqual(result, reSerialized);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		[Description("测试变量描述的JSON序列化")]
		public void WfVariableDescriptorConveterTest()
		{
			WfConverterHelper.RegisterConverters();

			WfVariableDescriptor variableDesc = new WfVariableDescriptor("variable");
			variableDesc.Description = "变量";

			string result = JSONSerializerExecute.Serialize(variableDesc);

			Console.WriteLine(result);

			WfVariableDescriptor deserializedActDesp = JSONSerializerExecute.Deserialize<WfVariableDescriptor>(result);

			string reSerialized = JSONSerializerExecute.Serialize(deserializedActDesp);

			Assert.AreEqual(result, reSerialized);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		[Description("测试流程中变量描述的JSON序列化")]
		public void WfVariabledOfPrecessDescriptorConverterTest()
		{
			WfConverterHelper.RegisterConverters();

			WfProcessDescriptor processDesp = (WfProcessDescriptor)WfProcessTestCommon.CreateSimpleProcessDescriptor();

			//给流程中变量描述赋值
			string guid = Guid.NewGuid().ToString();
			WfVariableDescriptor variDesc = new WfVariableDescriptor(guid);
			variDesc.Name = "yo";
			variDesc.Description = "流程中变量的赋值";
			variDesc.Enabled = true;
			variDesc.OriginalType = DataType.String;
			variDesc.OriginalValue = "原来的变量值";
			processDesp.Variables.Add(variDesc);

			string result = JSONSerializerExecute.Serialize(processDesp);
			Console.WriteLine(result);

			WfProcessDescriptor reProcessDesp = JSONSerializerExecute.Deserialize<WfProcessDescriptor>(result);

			string reSerialized = JSONSerializerExecute.Serialize(reProcessDesp);

			Assert.AreEqual(result, reSerialized);

			Assert.AreEqual(processDesp.Variables[0].Name, reProcessDesp.Variables[0].Name);
			Assert.AreEqual("流程中变量的赋值", reProcessDesp.Variables[0].Description);
			Assert.AreEqual(true, reProcessDesp.Variables[0].Enabled);
			Assert.AreEqual(processDesp.Variables[0].OriginalType, reProcessDesp.Variables[0].OriginalType);

		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		[Description("流程中Condition描述的JSON序列化")]
		public void WfConditionOfProcessDescriptorConverterTest()
		{
			WfConverterHelper.RegisterConverters();

			WfProcessDescriptor processDesp = (WfProcessDescriptor)WfProcessTestCommon.CreateSimpleProcessDescriptor();
			WfConditionDescriptor conditionDesp = new WfConditionDescriptor() { Expression = "A>5" };
			((WfForwardTransitionDescriptor)processDesp.InitialActivity.ToTransitions[0]).Condition = conditionDesp;

			string result = JSONSerializerExecute.Serialize(processDesp);
			Console.WriteLine(result);

			WfProcessDescriptor reProcessDesp = JSONSerializerExecute.Deserialize<WfProcessDescriptor>(result);

			Assert.AreEqual(conditionDesp.Expression, ((WfForwardTransitionDescriptor)reProcessDesp.InitialActivity.ToTransitions[0]).Condition.Expression);

			string reSerialized = JSONSerializerExecute.Serialize(reProcessDesp);

			Assert.AreEqual(result, reSerialized);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		[Description("测试用户资源描述的JSON序列化")]
		public void WfUserResourceDescriptorConveterTest()
		{
			WfConverterHelper.RegisterConverters();

			IUser user = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object;

			WfUserResourceDescriptor userDesp = new WfUserResourceDescriptor(user);

			string result = JSONSerializerExecute.Serialize(userDesp);

			Console.WriteLine(result);

			WfUserResourceDescriptor deserializedUserDesp = JSONSerializerExecute.Deserialize<WfUserResourceDescriptor>(result);

			string reSerialized = JSONSerializerExecute.Serialize(deserializedUserDesp);

			Assert.AreEqual(result, reSerialized);
			ResourceConverterTest(userDesp);

		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		[Description("运行时对应到人员的,表示某个活动的执行人")]
		public void WfActivityOperatorResourceDescriptorConverterTest()
		{
			WfConverterHelper.RegisterConverters();

			WfActivityOperatorResourceDescriptor actOperatorResDesp = new WfActivityOperatorResourceDescriptor();
			actOperatorResDesp.ActivityKey = "asdfasdf";
			string result = JSONSerializerExecute.Serialize(actOperatorResDesp);
			Console.WriteLine(result);

			WfActivityOperatorResourceDescriptor deserializedDesp = JSONSerializerExecute.Deserialize<WfActivityOperatorResourceDescriptor>(result);

			string reSerialized = JSONSerializerExecute.Serialize(deserializedDesp);

			Assert.AreEqual(result, reSerialized);

			ResourceConverterTest(actOperatorResDesp);

		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		[Description("运行时对应到人员的,表示某个活动的指派人")]
		[TestCategory("JSON Converter")]
		public void WfActivityAssigneesResourceDescriptorConverterTest()
		{
			WfConverterHelper.RegisterConverters();

			WfActivityAssigneesResourceDescriptor actAssigneesResDesp = new WfActivityAssigneesResourceDescriptor();
			actAssigneesResDesp.ActivityKey = "asfasf";
			string result = JSONSerializerExecute.Serialize(actAssigneesResDesp);
			Console.WriteLine(result);

			WfActivityAssigneesResourceDescriptor deserializedDesp = JSONSerializerExecute.Deserialize<WfActivityAssigneesResourceDescriptor>(result);

			string reSerialized = JSONSerializerExecute.Serialize(deserializedDesp);

			Assert.AreEqual(result, reSerialized);

			ResourceConverterTest(actAssigneesResDesp);
		}

		private static void ResourceConverterTest(WfResourceDescriptor resDesp)
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();
			processDesp.Activities[1].Resources.Add(resDesp);
			string re = JSONSerializerExecute.Serialize(processDesp);
			WfProcessDescriptor reProcessDesp = JSONSerializerExecute.Deserialize<WfProcessDescriptor>(re);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		[Description("测试用户部门资源描述的JSON序列化")]
		public void WfDepartmentResourceDescriptorConveterTest()
		{
			WfConverterHelper.RegisterConverters();

			IUser user = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object;

			IOrganization orga = user.Parent;
			WfDepartmentResourceDescriptor deptResuDesp = new WfDepartmentResourceDescriptor(orga);

			string result = JSONSerializerExecute.Serialize(deptResuDesp);

			Console.WriteLine(result);

			WfDepartmentResourceDescriptor deserializedDeptDesp = JSONSerializerExecute.Deserialize<WfDepartmentResourceDescriptor>(result);

			string reSerialized = JSONSerializerExecute.Serialize(deserializedDeptDesp);

			Assert.AreEqual(result, reSerialized);

			ResourceConverterTest(deptResuDesp);

		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		[Description("测试角色资源描述的JSON序列化")]
		public void WfRoleResourceDescriptorConverterTest()
		{
			WfConverterHelper.RegisterConverters();

			IUser user = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object;

			RoleCollection roleColl = user.Roles["OGU_ADMIN"];

			WfRoleResourceDescriptor roleResourceDesp = new WfRoleResourceDescriptor(roleColl[0]);
			string result = JSONSerializerExecute.Serialize(roleResourceDesp);
			Console.WriteLine(result);
			WfRoleResourceDescriptor deserializedRoleDesp = JSONSerializerExecute.Deserialize<WfRoleResourceDescriptor>(result);

			string reSerialized = JSONSerializerExecute.Serialize(deserializedRoleDesp);

			Assert.AreEqual(result, reSerialized);

			ResourceConverterTest(roleResourceDesp);

		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		[Description("测试用户组资源描述的JSON序列化")]
		public void WfGroupResourceDescriptorConverterTest()
		{
			WfConverterHelper.RegisterConverters();

			IUser user = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object;

			OguObjectCollection<IGroup> groupColl = user.MemberOf;

			foreach (var group in groupColl)
			{
				WfGroupResourceDescriptor groupDesp = new WfGroupResourceDescriptor(group);

				string result = JSONSerializerExecute.Serialize(groupDesp);

				Console.WriteLine(result);

				WfGroupResourceDescriptor deserializedGroupDesp = JSONSerializerExecute.Deserialize<WfGroupResourceDescriptor>(result);

				string reSerialized = JSONSerializerExecute.Serialize(deserializedGroupDesp);

				Assert.AreEqual(result, reSerialized);

				ResourceConverterTest(groupDesp);
			}

		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		[Description("测试流程分枝模板描述的JSON序列化")]
		public void WfBranchProcessTemplateDescriptorConverterTest()
		{
			WfConverterHelper.RegisterConverters();

			WfBranchProcessTemplateDescriptor branchProcessTempDesp = new WfBranchProcessTemplateDescriptor(Guid.NewGuid().ToString());
			branchProcessTempDesp.BlockingType = WfBranchProcessBlockingType.WaitAnyoneBranchProcessComplete;
			branchProcessTempDesp.ExecuteSequence = WfBranchProcessExecuteSequence.Serial;


			string result = JSONSerializerExecute.Serialize(branchProcessTempDesp);
			Console.WriteLine(result);

			WfBranchProcessTemplateDescriptor deserializedBranchProcTempDesp = JSONSerializerExecute.Deserialize<WfBranchProcessTemplateDescriptor>(result);
			string reSerialized = JSONSerializerExecute.Serialize(deserializedBranchProcTempDesp);

			Assert.AreEqual(result, reSerialized);

			WfProcessDescriptor processDesp = (WfProcessDescriptor)WfProcessTestCommon.CreateSimpleProcessDescriptor();

			processDesp.InitialActivity.BranchProcessTemplates.Add(branchProcessTempDesp);

			string procResult = JSONSerializerExecute.Serialize(processDesp);
			Console.WriteLine(procResult);

			WfProcessDescriptor deserializedProcDesp = JSONSerializerExecute.Deserialize<WfProcessDescriptor>(procResult);
			string procReSerialized = JSONSerializerExecute.Serialize(deserializedProcDesp);

			Assert.AreEqual(procResult, procReSerialized);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		[Description("测试相关外部人员的JSON序列化测试")]
		public void WfExternalUserTest()
		{
			WfConverterHelper.RegisterConverters();

			WfExternalUser externalUser = new WfExternalUser();
			externalUser.Key = "user0";
			externalUser.Name = "zLing";
			externalUser.Gender = Gender.Female;
			externalUser.Email = "zhangling@artmterch.com";
			externalUser.MobilePhone = "13552630000";
			externalUser.Phone = "0409987";
			externalUser.Title = "programer";

			string result = JSONSerializerExecute.Serialize(externalUser);
			WfExternalUser deserializedUser = JSONSerializerExecute.Deserialize<WfExternalUser>(result);
			string reSerialized = JSONSerializerExecute.Serialize(deserializedUser);
			Assert.AreEqual(result, reSerialized);


			//创建流程描述
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();
			processDesp.ExternalUsers.Add(externalUser); //作用于流程
			processDesp.InitialActivity.ExternalUsers.Add(externalUser);//作用于节点

			string procDesp = JSONSerializerExecute.Serialize(processDesp);
			WfProcessDescriptor reProcessDesp = JSONSerializerExecute.Deserialize<WfProcessDescriptor>(procDesp);
			string reuslt = JSONSerializerExecute.Serialize(reProcessDesp);

			Assert.AreEqual(procDesp, reuslt);
			Assert.AreEqual(processDesp.ExternalUsers[0].Name, processDesp.InitialActivity.ExternalUsers[0].Name);

		}


		[TestMethod]
		[Description("测试流程的相关链接的JSON序列化")]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		public void WfRelativeLinkDespJSONTest()
		{
			WfConverterHelper.RegisterConverters();

			WfProcessDescriptor processDesp = (WfProcessDescriptor)WfProcessTestCommon.CreateSimpleProcessDescriptor();
			string result = JSONSerializerExecute.Serialize(processDesp);
			IWfProcessDescriptor procDesp = JSONSerializerExecute.Deserialize<WfProcessDescriptor>(result);

			Assert.AreEqual(processDesp.RelativeLinks[0].Name, procDesp.RelativeLinks[0].Name);
			Assert.AreEqual(processDesp.RelativeLinks[0].Url, procDesp.RelativeLinks[0].Url);

		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		[Description("测试得到空流程定义的JSON串")]
		public void EmptyProcessDescriptorJsonStringTest()
		{
			Console.WriteLine(WfConverterHelper.GetEmptyProcessDescriptorJsonString());
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		[Description("测试空开始节点定义的JSON串")]
		public void EmptyInitialActivityDescriptorJsonString()
		{
			Console.WriteLine(WfConverterHelper.GetEmptyInitialActivityDescriptorJsonString());
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		[Description("测试空办结节点定义的JSON串")]
		public void EmptyCompletedActivityDescriptorJsonString()
		{
			Console.WriteLine(WfConverterHelper.GetEmptyCompletedActivityDescriptorJsonString());
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		[Description("测试空正常节点定义的JSON串")]
		public void EmptyNormalActivityDescriptorJsonString()
		{
			Console.WriteLine(WfConverterHelper.GetEmptyNormalActivityDescriptorJsonString());
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		[Description("测试空前向线定义的JSON串")]
		public void EmptyForwardTransitionDescriptorJsonString()
		{
			Console.WriteLine(WfConverterHelper.GetEmptyForwardTransitionDescriptorJsonString());
		}

	}
}
