using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.Script;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Test.Executor
{
	/// <summary>
	/// 管理员调整流程的Executor测试
	/// </summary>
	[TestClass]
	public class AdminEditProcessExecutorTest
	{
		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("测试编辑活动属性Executor的测试")]
		public void BasicEditActivityExecutorTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			IWfActivity targetActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			const string targetName = "修改后的名称";

			targetActivity.Descriptor.Properties.SetValue("Name", targetName);

			WfEditActivityPropertiesExecutor executor = new WfEditActivityPropertiesExecutor(process.CurrentActivity, process, targetActivity.Descriptor, false);

			executor.Execute();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			targetActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			Assert.AreEqual(targetName, targetActivity.Descriptor.Properties.GetValue("Name", string.Empty));

			UserOperationLog log = UserOperationLogAdapter.Instance.Load(builder => builder.AppendItem("ACTIVITY_ID", process.CurrentActivity.ID)).FirstOrDefault();

			Assert.IsNotNull(log);
			Console.WriteLine("{0}: {1}", log.OperationName, log.Subject);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("测试编辑活动属性及其主线活动属性的Executor的测试")]
		public void BasicEditActivityWithMainStreamExecutorTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			IWfActivity targetActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			const string targetName = "修改后的名称";

			targetActivity.Descriptor.Properties.SetValue("Name", targetName);

			WfEditActivityPropertiesExecutor executor = new WfEditActivityPropertiesExecutor(process.CurrentActivity, process, targetActivity.Descriptor, true);

			executor.Execute();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			IWfActivityDescriptor msActDesp = process.Activities.FindActivityByDescriptorKey("NormalActivity").GetMainStreamActivityDescriptor();

			Assert.AreEqual(targetName, msActDesp.Properties.GetValue("Name", string.Empty));
			Assert.IsTrue(msActDesp.IsMainStreamActivity);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("测试编辑活动的条件属性的Executor的测试")]
		public void EditActivityConditionExecutorTest()
		{
			WfConverterHelper.RegisterConverters();

			IWfProcess process = WfProcessTestCommon.StartupSimpleProcessDescriptorWithActivityCondition();

			IWfActivity targetActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			const string targetCondition = "Amount > 5000000";
			targetActivity.Descriptor.Condition.Expression = targetCondition;

			targetActivity.Descriptor.Properties["Condition"].StringValue = JSONSerializerExecute.Serialize(targetActivity.Descriptor.Condition);

			WfEditActivityPropertiesExecutor executor = new WfEditActivityPropertiesExecutor(process.CurrentActivity, process, targetActivity.Descriptor, true);

			executor.Execute();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			targetActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			Assert.AreEqual(targetCondition, targetActivity.Descriptor.Condition.Expression);

			IWfActivityDescriptor msActDesp = process.Activities.FindActivityByDescriptorKey("NormalActivity").GetMainStreamActivityDescriptor();

			Console.WriteLine(msActDesp.Condition.Expression);

			Assert.AreEqual(targetCondition, msActDesp.Condition.Expression);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("测试编辑流程属性及其主线流程属性的Executor的测试")]
		public void BasicEditProcessWithMainStreamExecutorTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			const string targetName = "修改后的名称";

			process.Descriptor.Properties.SetValue("Name", targetName);

			WfEditProcessPropertiesExecutor executor = new WfEditProcessPropertiesExecutor(process.CurrentActivity, process, process.Descriptor, true);

			executor.Execute();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			Assert.AreEqual(targetName, process.Descriptor.Properties.GetValue("Name", string.Empty));

			IWfProcessDescriptor msProcessDesp = process.MainStream;

			Assert.AreEqual(targetName, msProcessDesp.Properties.GetValue("Name", string.Empty));
			Assert.IsTrue(msProcessDesp.IsMainStream);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("测试编辑线属性及其主线流程的线属性的Executor的测试")]
		public void BasicEditTransitionWithMainStreamExecutorTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			const string targetName = "修改后的名称";

			IWfActivity targetActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			IWfTransitionDescriptor targetDesp = process.Descriptor.InitialActivity.ToTransitions.GetTransitionByToActivity(targetActivity.Descriptor);

			targetDesp.Properties.SetValue("Name", targetName);

			WfEditTransitionPropertiesExecutor executor = new WfEditTransitionPropertiesExecutor(process.CurrentActivity, process, targetDesp, true);

			executor.Execute();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			targetActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");
			targetDesp = process.Descriptor.InitialActivity.ToTransitions.GetTransitionByToActivity(targetActivity.Descriptor);

			Assert.AreEqual(targetName, targetDesp.Properties.GetValue("Name", string.Empty));

			IWfActivityDescriptor msFromActDesp = process.MainStream.InitialActivity;
			IWfActivityDescriptor msToActDesp = process.MainStream.Activities["NormalActivity"];

			targetDesp = msFromActDesp.ToTransitions.GetTransitionByToActivity(msToActDesp);

			Assert.IsNotNull(targetDesp);
			Assert.AreEqual(targetName, targetDesp.Properties.GetValue("Name", string.Empty));
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("测试增加线属性的Executor的测试")]
		public void BasicAdminAddTransitionExecutorTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			const string targetName = "修改后的名称";

			IWfActivity fromActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			IWfTransitionDescriptor targetDesp = fromActivity.Descriptor.ToTransitions.AddBackwardTransition(process.Descriptor.InitialActivity);

			targetDesp.Properties.SetValue("Name", targetName);

			WfAdminAddTransitionExecutor executor = new WfAdminAddTransitionExecutor(process.CurrentActivity, process, targetDesp, false);

			executor.Execute();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			fromActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			targetDesp = fromActivity.Descriptor.ToTransitions.GetTransitionByToActivity(process.Descriptor.InitialActivity);

			Assert.IsNotNull(targetDesp);
			Assert.AreEqual(targetName, targetDesp.Properties.GetValue("Name", string.Empty));
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("测试增加线属性的Executor的测试")]
		public void BasicAdminAddTransitionWithMainStreamExecutorTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			const string targetName = "修改后的名称";

			IWfActivity fromActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			IWfTransitionDescriptor targetDesp = fromActivity.Descriptor.ToTransitions.AddBackwardTransition(process.Descriptor.InitialActivity);

			targetDesp.Properties.SetValue("Name", targetName);

			WfAdminAddTransitionExecutor executor = new WfAdminAddTransitionExecutor(process.CurrentActivity, process, targetDesp, true);

			executor.Execute();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			fromActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			targetDesp = fromActivity.Descriptor.ToTransitions.GetTransitionByToActivity(process.Descriptor.InitialActivity);

			Assert.IsNotNull(targetDesp);
			Assert.AreEqual(targetName, targetDesp.Properties.GetValue("Name", string.Empty));

			IWfActivityDescriptor msFromActDesp = process.MainStream.Activities["NormalActivity"];
			IWfActivityDescriptor msToActDesp = process.MainStream.InitialActivity;

			targetDesp = msFromActDesp.ToTransitions.GetTransitionByToActivity(msToActDesp);

			Assert.IsNotNull(targetDesp);
			Assert.AreEqual(targetName, targetDesp.Properties.GetValue("Name", string.Empty));
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		public void BasicAdminAddActivityExecutorTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			const string targetName = "修改后的名称";

			IWfActivityDescriptor targetDesp = new WfActivityDescriptor(process.Descriptor.FindNotUsedActivityKey(), WfActivityType.NormalActivity);

			targetDesp.Properties.SetValue("Name", targetName);
			process.Descriptor.Activities.Add(targetDesp);

			WfActivityBase.CreateActivityInstance(targetDesp, process);

			WfAdminAddActivityExecutor executor = new WfAdminAddActivityExecutor(process.CurrentActivity, process, null, targetDesp, false);

			executor.Execute();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			targetDesp = process.Descriptor.Activities[targetDesp.Key];

			Assert.IsNotNull(targetDesp);
			Assert.AreEqual(targetName, targetDesp.Properties.GetValue("Name", string.Empty));
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		public void BasicAdminAddActivityWithFromActivityExecutorTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			const string targetName = "修改后的名称";

			IWfActivity fromActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			IWfActivityDescriptor targetDesp = new WfActivityDescriptor(process.Descriptor.FindNotUsedActivityKey(), WfActivityType.NormalActivity);

			targetDesp.Properties.SetValue("Name", targetName);
			process.Descriptor.Activities.Add(targetDesp);

			WfActivityBase.CreateActivityInstance(targetDesp, process);

			WfAdminAddActivityExecutor executor = new WfAdminAddActivityExecutor(process.CurrentActivity, process, fromActivity.Descriptor, targetDesp, false);

			executor.Execute();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			targetDesp = process.Descriptor.Activities[targetDesp.Key];

			Assert.IsNotNull(targetDesp);
			Assert.AreEqual(targetName, targetDesp.Properties.GetValue("Name", string.Empty));

			fromActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			Assert.IsNotNull(fromActivity.Descriptor.ToTransitions.GetTransitionByToActivity(targetDesp));
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		public void BasicAdminAddActivityWithMainStreamExecutorTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			const string targetName = "修改后的名称";

			IWfActivityDescriptor targetDesp = new WfActivityDescriptor(process.Descriptor.FindNotUsedActivityKey(), WfActivityType.NormalActivity);

			targetDesp.Properties.SetValue("Name", targetName);
			process.Descriptor.Activities.Add(targetDesp);

			WfActivityBase.CreateActivityInstance(targetDesp, process);

			WfAdminAddActivityExecutor executor = new WfAdminAddActivityExecutor(process.CurrentActivity, process, null, targetDesp, true);

			executor.Execute();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			targetDesp = process.Descriptor.Activities[targetDesp.Key];

			Assert.IsNotNull(targetDesp);
			Assert.AreEqual(targetName, targetDesp.Properties.GetValue("Name", string.Empty));

			IWfActivityDescriptor msActDesp = targetDesp.Instance.GetMainStreamActivityDescriptor();
			Assert.IsNotNull(msActDesp);
			Assert.AreEqual(process.MainStream, msActDesp.Process);
			Assert.AreEqual(targetName, msActDesp.Properties.GetValue("Name", string.Empty));
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		public void BasicAdminAddActivityWithMainStreamAndFromActivityExecutorTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			const string targetName = "修改后的名称";

			IWfActivityDescriptor targetDesp = new WfActivityDescriptor(process.Descriptor.FindNotUsedActivityKey(), WfActivityType.NormalActivity);

			targetDesp.Properties.SetValue("Name", targetName);
			process.Descriptor.Activities.Add(targetDesp);

			WfActivityBase.CreateActivityInstance(targetDesp, process);

			IWfActivity fromActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			WfAdminAddActivityExecutor executor = new WfAdminAddActivityExecutor(process.CurrentActivity, process, fromActivity.Descriptor, targetDesp, true);

			executor.Execute();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			targetDesp = process.Descriptor.Activities[targetDesp.Key];

			Assert.IsNotNull(targetDesp);
			Assert.AreEqual(targetName, targetDesp.Properties.GetValue("Name", string.Empty));
			fromActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			Assert.IsNotNull(fromActivity.Descriptor.ToTransitions.GetTransitionByToActivity(targetDesp));

			IWfActivityDescriptor msActDesp = targetDesp.Instance.GetMainStreamActivityDescriptor();
			Assert.IsNotNull(msActDesp);
			Assert.AreEqual(process.MainStream, msActDesp.Process);
			Assert.AreEqual(targetName, msActDesp.Properties.GetValue("Name", string.Empty));

			IWfActivityDescriptor msFromActDesp = fromActivity.GetMainStreamActivityDescriptor();

			Assert.IsNotNull(msFromActDesp);
			Assert.AreEqual(process.MainStream, msFromActDesp.Process);
			Assert.IsNotNull(msFromActDesp.ToTransitions.GetTransitionByToActivity(msActDesp));
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		public void BasicAdminDeleteActivityExecutorTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			IWfActivityDescriptor targetDesp = process.Descriptor.Activities["NormalActivity"];

			WfAdminDeleteActivityExecutor executor = new WfAdminDeleteActivityExecutor(process.CurrentActivity, targetDesp, false);

			executor.Execute();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			targetDesp = process.Descriptor.Activities[targetDesp.Key];

			Assert.IsNull(targetDesp);
			Assert.AreEqual(0, process.Descriptor.InitialActivity.ToTransitions.Count);
			Assert.AreEqual(0, process.Descriptor.CompletedActivity.FromTransitions.Count);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		public void BasicAdminDeleteActivityWithMainStreamExecutorTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			IWfActivityDescriptor targetDesp = process.Descriptor.Activities["NormalActivity"];

			WfAdminDeleteActivityExecutor executor = new WfAdminDeleteActivityExecutor(process.CurrentActivity, targetDesp, true);

			executor.Execute();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			IWfProcessDescriptor processDesp = process.Descriptor;
			targetDesp = processDesp.Activities["NormalActivity"];

			Assert.IsNull(targetDesp);
			Assert.AreEqual(0, processDesp.InitialActivity.ToTransitions.Count);
			Assert.AreEqual(0, processDesp.CompletedActivity.FromTransitions.Count);

			processDesp = process.MainStream;
			targetDesp = processDesp.Activities["NormalActivity"];

			Assert.IsNull(targetDesp);
			Assert.AreEqual(0, processDesp.InitialActivity.ToTransitions.Count);
			Assert.AreEqual(0, processDesp.CompletedActivity.FromTransitions.Count);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		public void BasicAdminDeleteTransitionExecutorTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			IWfActivityDescriptor targetDesp = process.Descriptor.Activities["NormalActivity"];

			IWfTransitionDescriptor transitionDesp = targetDesp.ToTransitions.FirstOrDefault();

			IWfActivityDescriptor nextActDesp = transitionDesp.ToActivity;

			WfAdminDeleteTransitionExecutor executor = new WfAdminDeleteTransitionExecutor(process.CurrentActivity, process.Descriptor, transitionDesp, false);

			executor.Execute();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			IWfProcessDescriptor processDesp = process.Descriptor;

			targetDesp = processDesp.Activities[targetDesp.Key];
			nextActDesp = processDesp.CompletedActivity;

			Assert.IsNull(targetDesp.ToTransitions.FirstOrDefault());
			Assert.IsNull(nextActDesp.FromTransitions.FirstOrDefault());
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		public void BasicAdminDeleteTransitionWithMainStreamExecutorTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			IWfActivityDescriptor targetDesp = process.Descriptor.Activities["NormalActivity"];

			IWfTransitionDescriptor transitionDesp = targetDesp.ToTransitions.FirstOrDefault();

			IWfActivityDescriptor nextActDesp = transitionDesp.ToActivity;

			WfAdminDeleteTransitionExecutor executor = new WfAdminDeleteTransitionExecutor(process.CurrentActivity, process.Descriptor, transitionDesp, true);

			executor.Execute();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			IWfProcessDescriptor processDesp = process.Descriptor;

			targetDesp = processDesp.Activities[targetDesp.Key];
			nextActDesp = processDesp.CompletedActivity;

			Assert.IsNull(targetDesp.ToTransitions.FirstOrDefault());
			Assert.IsNull(nextActDesp.FromTransitions.FirstOrDefault());

			processDesp = process.MainStream;

			targetDesp = processDesp.Activities[targetDesp.Key];
			nextActDesp = processDesp.CompletedActivity;

			Assert.IsNull(targetDesp.ToTransitions.FirstOrDefault());
			Assert.IsNull(nextActDesp.FromTransitions.FirstOrDefault());
		}
	}
}
