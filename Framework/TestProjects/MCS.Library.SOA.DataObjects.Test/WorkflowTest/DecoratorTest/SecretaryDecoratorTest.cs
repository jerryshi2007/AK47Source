using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Test.Executor;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest.DecoratorTest
{
	/// <summary>
	/// Summary description for SecretaryDecoratorTest
	/// </summary>
	[TestClass]
	public class SecretaryDecoratorTest
	{
		public SecretaryDecoratorTest()
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
		[Description("简单的秘书测试")]
		[TestCategory(ProcessTestHelper.Decorator)]
		public void SimpleSecretaryTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithSecretary();

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			OutputProcessCandidates(process);
			ReturnExecutorTestHelper.OutputMainStream(process);

			Assert.AreEqual(5, processDesp.Activities.Count);
			Assert.AreEqual(5, process.MainStream.Activities.Count);
		}

		[TestMethod]
		[Description("简单的带退回线的秘书测试")]
		[TestCategory(ProcessTestHelper.Decorator)]
		public void SimpleSecretaryWithReturnLineTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithSecretaryAndReturnLine();

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			OutputProcessCandidates(process);
			ReturnExecutorTestHelper.OutputMainStream(process);

			Assert.AreEqual(5, processDesp.Activities.Count);
			Assert.AreEqual(5, process.MainStream.Activities.Count);

			IWfActivity normalActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			IWfActivityDescriptor firstSecretary = normalActivity.Descriptor.FromTransitions.FirstOrDefault().FromActivity;

			Assert.AreEqual(1, firstSecretary.ToTransitions.Count);

			IWfActivityDescriptor secondSecretary = normalActivity.Descriptor.ToTransitions.FirstOrDefault().ToActivity;

			Assert.AreEqual(2, secondSecretary.ToTransitions.Count);

			Assert.AreEqual(2, normalActivity.Descriptor.ToTransitions.Count);

			foreach (IWfTransitionDescriptor transition in normalActivity.Descriptor.ToTransitions)
			{
				Assert.AreEqual(secondSecretary.Key, transition.ToActivity.Key);
			}
		}

		[TestMethod]
		[Description("重复生成秘书测试")]
		[TestCategory(ProcessTestHelper.Decorator)]
		public void DuplicateGenerateSimpleSecretaryTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithSecretary();

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			WfRuntime.DecorateProcess(process);

			OutputProcessCandidates(process);
			ReturnExecutorTestHelper.OutputMainStream(process);

			Assert.AreEqual(5, processDesp.Activities.Count);
			Assert.AreEqual(5, process.MainStream.Activities.Count);
		}

		[TestMethod]
		[Description("替换领导后生成秘书测试")]
		[TestCategory(ProcessTestHelper.Decorator)]
		public void ChangeGenerateSimpleSecretaryTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithSecretary();

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			ChangeActivityResource(process.Descriptor.Activities["NormalActivity"], (IUser)OguObjectSettings.GetConfig().Objects["vp"].Object);

			WfRuntime.DecorateProcess(process);

			OutputProcessCandidates(process);
			ReturnExecutorTestHelper.OutputMainStream(process);

			Assert.AreEqual(5, processDesp.Activities.Count);
			Assert.AreEqual(5, process.MainStream.Activities.Count);
		}

		[TestMethod]
		[Description("将有秘书的领导切换成没有秘书的，然后再切换回有秘书的领导")]
		[TestCategory(ProcessTestHelper.Decorator)]
		public void ChangeLeaderToApproverThenChangeToLeaderSecretaryTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithSecretary();

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			ChangeActivityResource(process.Descriptor.Activities["NormalActivity"], (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object);

			WfRuntime.DecorateProcess(process);

			Console.WriteLine("没有秘书的");
			OutputProcessCandidates(process);
			ReturnExecutorTestHelper.OutputMainStream(process);

			ChangeActivityResource(process.Descriptor.Activities["NormalActivity"], (IUser)OguObjectSettings.GetConfig().Objects["ceo"].Object);

			WfRuntime.DecorateProcess(process);

			Console.WriteLine("有秘书的");
			OutputProcessCandidates(process);
			ReturnExecutorTestHelper.OutputMainStream(process);

			Assert.AreEqual(5, processDesp.Activities.Count);
			Assert.AreEqual(5, process.MainStream.Activities.Count);
		}

		[TestMethod]
		[Description("两次替换领导后生成秘书测试")]
		[TestCategory(ProcessTestHelper.Decorator)]
		public void DoubleChangeGenerateSimpleSecretaryTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithSecretary();

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			ChangeActivityResource(process.Descriptor.Activities["NormalActivity"], (IUser)OguObjectSettings.GetConfig().Objects["vp"].Object);

			WfRuntime.DecorateProcess(process);

			ChangeActivityResource(process.Descriptor.Activities["NormalActivity"], (IUser)OguObjectSettings.GetConfig().Objects["ceo"].Object);

			WfRuntime.DecorateProcess(process);

			OutputProcessCandidates(process);
			ReturnExecutorTestHelper.OutputMainStream(process);

			Assert.AreEqual(5, processDesp.Activities.Count);
			Assert.AreEqual(5, process.MainStream.Activities.Count);
		}

		[TestMethod]
		[Description("清除秘书活动测试")]
		[TestCategory(ProcessTestHelper.Decorator)]
		public void ClearSecretaryTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithSecretary();

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			ChangeActivityResource(process.Descriptor.Activities["NormalActivity"], (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object);

			WfRuntime.DecorateProcess(process);

			OutputProcessCandidates(process);
			ReturnExecutorTestHelper.OutputMainStream(process);

			Assert.AreEqual(3, processDesp.Activities.Count);
			Assert.AreEqual(3, process.MainStream.Activities.Count);
		}

		[TestMethod]
		[Description("清除秘书活动然后再添加秘书测试")]
		[TestCategory(ProcessTestHelper.Decorator)]
		public void ClearThenResetSecretaryTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithSecretary();

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			ChangeActivityResource(process.Descriptor.Activities["NormalActivity"], (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object);
			WfRuntime.DecorateProcess(process);

			ChangeActivityResource(process.Descriptor.Activities["NormalActivity"], (IUser)OguObjectSettings.GetConfig().Objects["ceo"].Object);
			WfRuntime.DecorateProcess(process);

			OutputProcessCandidates(process);
			ReturnExecutorTestHelper.OutputMainStream(process);

			Assert.AreEqual(5, processDesp.Activities.Count);
			Assert.AreEqual(5, process.MainStream.Activities.Count);
		}

		[TestMethod]
		[Description("首节点就带秘书的测试")]
		[TestCategory(ProcessTestHelper.Decorator)]
		public void InitialActivitySecretaryTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();
			processDesp.InitialActivity.Properties.SetValue("AutoAppendSecretary", true);

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			ChangeActivityResource(process.Descriptor.InitialActivity,
				(IUser)OguObjectSettings.GetConfig().Objects["ceo"].Object,
				(IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object);
			WfRuntime.DecorateProcess(process);

			OutputProcessCandidates(process);
			ReturnExecutorTestHelper.OutputMainStream(process);

			Assert.AreEqual(5, processDesp.Activities.Count);
			Assert.AreEqual(process.InitialActivity.Descriptor.Key, process.Descriptor.InitialActivity.Key);
			Assert.AreEqual(WfActivityType.InitialActivity, process.Descriptor.InitialActivity.ActivityType);
			Assert.AreEqual(WfActivityType.NormalActivity, process.Descriptor.Activities["Initial"].ActivityType);

			Assert.AreEqual(5, process.MainStream.Activities.Count);
			Assert.AreEqual(WfActivityType.InitialActivity, process.MainStream.InitialActivity.ActivityType);
		}

		[TestMethod]
		[Description("清除首结点秘书活动然后再添加秘书测试")]
		[TestCategory(ProcessTestHelper.Decorator)]
		public void ClearThenResetInitialActivitySecretaryTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();
			processDesp.InitialActivity.Properties.SetValue("AutoAppendSecretary", true);

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			ChangeActivityResource(process.Descriptor.InitialActivity, (IUser)OguObjectSettings.GetConfig().Objects["ceo"].Object);
			WfRuntime.DecorateProcess(process);

			ChangeActivityResource(process.Descriptor.Activities["Initial"], (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object);
			WfRuntime.DecorateProcess(process);

			OutputProcessCandidates(process);
			ReturnExecutorTestHelper.OutputMainStream(process);

			Assert.AreEqual(3, processDesp.Activities.Count);
			Assert.AreEqual(process.InitialActivity.Descriptor.Key, process.Descriptor.InitialActivity.Key);
			Assert.AreEqual(WfActivityType.InitialActivity, process.Descriptor.InitialActivity.ActivityType);
			Assert.AreEqual(WfActivityType.InitialActivity, process.Descriptor.Activities["Initial"].ActivityType);

			Assert.AreEqual(3, process.MainStream.Activities.Count);
			Assert.AreEqual(WfActivityType.InitialActivity, process.MainStream.InitialActivity.ActivityType);
		}

		[TestMethod]
		[Description("并行分支流程的秘书测试")]
		[TestCategory(ProcessTestHelper.Decorator)]
		public void ParallelBranchActivitySecretaryTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithBranchTemplate();

			IWfBranchProcessTemplateDescriptor template = processDesp.Activities["NormalActivity"].BranchProcessTemplates["Consign"];

			template.Resources.Clear();
			template.Resources.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["ceo"].Object));

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);
			WfProcessTestCommon.MoveToNextDefaultActivity(process);

			IWfProcess consignBranchProcess = process.Activities.FindActivityByDescriptorKey("NormalActivity").BranchProcessGroups["Consign"].Branches.FirstOrDefault();
			IWfProcess distributeBranchProcess = process.Activities.FindActivityByDescriptorKey("NormalActivity").BranchProcessGroups["Distribute"].Branches.FirstOrDefault();

			Console.WriteLine("Consign Process");
			OutputProcessCandidates(consignBranchProcess);
			ReturnExecutorTestHelper.OutputMainStream(consignBranchProcess);

			Console.WriteLine("");

			Console.WriteLine("Distribute Process");
			OutputProcessCandidates(distributeBranchProcess);
			ReturnExecutorTestHelper.OutputMainStream(distributeBranchProcess);

			Assert.AreEqual(4, consignBranchProcess.Activities.Count);
			Assert.AreEqual(2, distributeBranchProcess.Activities.Count);

			Assert.AreEqual(4, consignBranchProcess.MainStream.Activities.Count);
			Assert.AreEqual(2, distributeBranchProcess.MainStream.Activities.Count);
		}

		[TestMethod]
		[Description("分支流程且带同意线的秘书测试")]
		[TestCategory(ProcessTestHelper.Decorator)]
		public void BranchProcessContainsSecretaryWithAgreeLineTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateBranchProcessWithSecretaryAndAgreeLine();

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			WfProcessTestCommon.MoveToNextDefaultActivityWithExecutor(process);

			process = WfRuntime.GetProcessByProcessID(process.ID);

			process.CurrentActivity.BranchProcessGroups[0].Branches.ForEach(subProcess =>
				{
					Console.WriteLine("分支流程ID={0}", subProcess.ID);
					OutputProcessCandidatesAndOutTransitions(subProcess);
				});
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="process"></param>
		private static void OutputProcessCandidatesAndOutTransitions(IWfProcess process)
		{
			//process.Descriptor.Activities.ForEach(a =>
			//    {
			//        Console.WriteLine("Key={0}, Name={1}, User={2}",
			//        a.Key,
			//        a.Name,
			//        GetAssigneesName(a.Instance.Candidates));

			//        foreach (IWfTransitionDescriptor t in a.ToTransitions)
			//        {
			//            Console.WriteLine("\tTransition Key={0}, NextActKey={1}, Name={2}, AffectProcessReturnValue={3}, AffectedProcessReturnValue={4}, DefaultSelect={5}",
			//                t.Key,
			//                t.ToActivity.Key,
			//                t.Name,
			//                t.Properties.GetValue("AffectProcessReturnValue", false),
			//                t.Properties.GetValue("AffectedProcessReturnValue", false),
			//                t.DefaultSelect);
			//        }
			//    });

			//OutputAllTransitions(process.Descriptor);
			process.Descriptor.ProbeAllActivities(actArgs =>
			{
				Console.WriteLine("Key={0}, Name={1}, User={2}",
					actArgs.ActivityDescriptor.Key,
					actArgs.ActivityDescriptor.Name,
					GetAssigneesName(actArgs.ActivityDescriptor.Instance.Candidates));

				foreach (IWfTransitionDescriptor t in actArgs.ActivityDescriptor.ToTransitions)
				{
					Console.WriteLine("\tTransition Key={0}, NextActKey={1}, Name={2}, AffectProcessReturnValue={3}, AffectedProcessReturnValue={4}, DefaultSelect={5}",
						t.Key,
						t.ToActivity.Key,
						t.Name,
						t.Properties.GetValue("AffectProcessReturnValue", false),
						t.Properties.GetValue("AffectedProcessReturnValue", false),
						t.DefaultSelect);
				}

				return true;
			},
			transition => transition.Enabled);
		}

		private static void OutputAllTransitions(IWfProcessDescriptor processDesp)
		{
			Dictionary<IWfTransitionDescriptor, IWfTransitionDescriptor> transitions = new Dictionary<IWfTransitionDescriptor, IWfTransitionDescriptor>();

			foreach (IWfActivityDescriptor actDesp in processDesp.Activities)
			{
				actDesp.ToTransitions.ForEach(t =>
				{
					if (transitions.ContainsKey(t) == false)
						transitions.Add(t, t);
				});
			}

			foreach (IWfActivityDescriptor actDesp in processDesp.Activities)
			{
				actDesp.FromTransitions.ForEach(t =>
				{
					if (transitions.ContainsKey(t) == false)
						transitions.Add(t, t);
				});
			}

			foreach (KeyValuePair<IWfTransitionDescriptor, IWfTransitionDescriptor> kp in transitions)
			{
				Console.WriteLine(kp.Key.Key);
			}
		}

		private static void OutputProcessCandidates(IWfProcess process)
		{
			process.Descriptor.ProbeAllActivities(actArgs =>
			{
				Console.WriteLine("Key={0}, Name={1}, User={2}",
					actArgs.ActivityDescriptor.Key,
					actArgs.ActivityDescriptor.Name,
					GetAssigneesName(actArgs.ActivityDescriptor.Instance.Candidates));

				return true;
			},
			transition => transition.Enabled);
		}

		private static string GetAssigneesName(WfAssigneeCollection assignees)
		{
			StringBuilder strB = new StringBuilder();

			foreach (WfAssignee assignee in assignees)
			{
				if (strB.Length > 0)
					strB.Append(",");

				strB.Append(assignee.User.DisplayName);
			}

			return strB.ToString();
		}

		private static void ChangeActivityResource(IWfActivityDescriptor actDesp, params IUser[] users)
		{
			actDesp.Resources.Clear();

			foreach (IUser user in users)
			{
				actDesp.Resources.Add(new WfUserResourceDescriptor(user));
			}

			actDesp.Instance.GenerateCandidatesFromResources();
		}
	}
}
