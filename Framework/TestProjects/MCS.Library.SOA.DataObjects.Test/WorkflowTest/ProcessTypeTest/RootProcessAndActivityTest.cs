using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest.ProcessTypeTest
{
	[TestClass]
	public class RootProcessAndActivityTest
	{
		[TestMethod]
		[Description("简单的根计划流程测试，第一级是计划流程，第二级是审批流程")]
		[TestCategory(ProcessTestHelper.ProcessType)]
		public void ScheduleRootTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithBranchTemplate();

			((WfProcessDescriptor)processDesp).ProcessType = WfProcessType.Schedule;

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			IWfActivityDescriptor normalActivity = processDesp.InitialActivity.ToTransitions[0].ToActivity;

			WfTransferParams transferParams0 = new WfTransferParams(normalActivity);

			//初始化后，流转到有分支流程的活动
			process.MoveTo(transferParams0);

			Assert.AreEqual(process.ID, normalActivity.Instance.BranchProcessGroups[0].Branches[0].ScheduleRootProcess.ID);
			Assert.AreEqual(normalActivity.Instance.ID, normalActivity.Instance.BranchProcessGroups[0].Branches[0].InitialActivity.ScheduleRootActivity.ID);
		}

		[TestMethod]
		[Description("简单的根计划流程测试，第一级是计划流程，第二级也是计划流程")]
		[TestCategory(ProcessTestHelper.ProcessType)]
		public void TwoScheduleProcessRootTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithBranchTemplate();

			((WfProcessDescriptor)processDesp).ProcessType = WfProcessType.Schedule;

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			IWfActivityDescriptor normalActivity = processDesp.InitialActivity.ToTransitions[0].ToActivity;

			WfTransferParams transferParams0 = new WfTransferParams(normalActivity);

			//初始化后，流转到有分支流程的活动
			process.MoveTo(transferParams0);

			normalActivity.Instance.BranchProcessGroups.ForEach(g => g.Branches.ForEach(p => ((WfProcessDescriptor)p.Descriptor).ProcessType = WfProcessType.Schedule));

			Assert.AreEqual(normalActivity.Instance.BranchProcessGroups[0].Branches[0].ID,
				normalActivity.Instance.BranchProcessGroups[0].Branches[0].ScheduleRootProcess.ID);

			Assert.AreEqual(normalActivity.Instance.BranchProcessGroups[0].Branches[0].InitialActivity.ID,
				normalActivity.Instance.BranchProcessGroups[0].Branches[0].InitialActivity.ScheduleRootActivity.ID);
		}

		[TestMethod]
		[Description("全部是计划流程，则返回根流程")]
		[TestCategory(ProcessTestHelper.ProcessType)]
		public void AllApprovalProcessScheduleRootTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithBranchTemplate();

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			IWfActivityDescriptor normalActivity = processDesp.InitialActivity.ToTransitions[0].ToActivity;

			WfTransferParams transferParams0 = new WfTransferParams(normalActivity);

			//初始化后，流转到有分支流程的活动
			process.MoveTo(transferParams0);

			Assert.AreEqual(process.ID, normalActivity.Instance.BranchProcessGroups[0].Branches[0].ScheduleRootProcess.ID);
			Assert.AreEqual(normalActivity.Instance.ID, normalActivity.Instance.BranchProcessGroups[0].Branches[0].InitialActivity.ScheduleRootActivity.ID);
		}
	}
}
