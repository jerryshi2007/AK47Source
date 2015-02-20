using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest.DescriptorTest
{
	[TestClass]
	public class WfActivityDescriptorDeleteTest
	{
		[TestMethod]
		[TestCategory(ProcessTestHelper.Descriptor)]
		public void BasicActivityDescriptorDeleteTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

			IWfActivityDescriptor normalActDesp = processDesp.Activities["NormalActivity"];

			int originalActCount = processDesp.Activities.Count;

			normalActDesp.Delete();

			processDesp.Output();

			Assert.AreEqual(originalActCount - 1, processDesp.Activities.Count);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Descriptor)]
		[Description("删除有两条出线的活动，其中有一条退回线")]
		public void DeleteActivityDescriptorWithReturnTransitionsTest()
		{
			IWfProcessDescriptor processDesp = PrepareWithReturnTransitionsProcess();

			IWfActivityDescriptor normalActDesp = processDesp.Activities["NormalActivity"];

			int originalActCount = processDesp.Activities.Count;

			normalActDesp.Delete();

			processDesp.Output();

			Assert.AreEqual(originalActCount - 1, processDesp.Activities.Count);
			Assert.AreEqual(2, processDesp.InitialActivity.ToTransitions.Count);
			Assert.AreEqual(1, processDesp.InitialActivity.FromTransitions.Count);
			Assert.AreEqual(1, processDesp.CompletedActivity.FromTransitions.Count);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Descriptor)]
		[Description("删除有两条出线的活动（线上包含条件）")]
		public void DeleteActivityDescriptorWithTwoTransitionsTest()
		{
			//Init，Complete，Normal和Manager，其中Normal有出线到Manager和Completed
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithCondition();

			IWfActivityDescriptor normalActDesp = processDesp.Activities["NormalActivity"];

			int originalActCount = processDesp.Activities.Count;

			normalActDesp.Delete();

			processDesp.Output();

			Assert.AreEqual(originalActCount - 1, processDesp.Activities.Count);
			Assert.AreEqual(2, processDesp.InitialActivity.ToTransitions.Count);
			Assert.AreEqual(2, processDesp.CompletedActivity.FromTransitions.Count);
			Assert.IsFalse(processDesp.InitialActivity.ToTransitions[0].Condition.IsEmpty);
			Assert.IsFalse(processDesp.InitialActivity.ToTransitions[1].Condition.IsEmpty);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Descriptor)]
		[Description("删除有复杂线的流程。本流程活动为Init、N、N1、N2、N3、C；N有线指向N2。N2有退回线指向N1")]
		public void DeleteActivityDescriptorWithComplexTransitionsTest()
		{
			IWfProcessDescriptor processDesp = PrepareComplexProcessForDelete();

			IWfActivityDescriptor normalActDesp = processDesp.Activities["NormalActivity1"];
			IWfActivityDescriptor normalActivity = processDesp.Activities["NormalActivity"];
			IWfActivityDescriptor normalActivity2 = processDesp.Activities["NormalActivity2"];

			IWfTransitionDescriptor transitionNN2 = normalActivity.ToTransitions.GetTransitionByToActivity(normalActivity2);

			int originalActCount = processDesp.Activities.Count;

			normalActDesp.Delete();

			processDesp.Output();

			Assert.AreEqual(originalActCount - 1, processDesp.Activities.Count);
			Assert.AreEqual(1, normalActivity.ToTransitions.Count);
			Assert.AreEqual(2, normalActivity2.FromTransitions.Count);
			Assert.AreEqual(2, normalActivity2.ToTransitions.Count);

			IWfTransitionDescriptor newTransitionNN2 = normalActivity.ToTransitions.GetTransitionByToActivity(normalActivity2);

			Assert.AreEqual(transitionNN2.Key, newTransitionNN2.Key, "原有的线保留");
		}

		private static IWfProcessDescriptor PrepareWithReturnTransitionsProcess()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

			IWfActivityDescriptor normalActDesp = processDesp.Activities["NormalActivity"];

			normalActDesp.ToTransitions.AddBackwardTransition(processDesp.InitialActivity);

			return processDesp;
		}

		/// <summary>
		/// 创建一个用于删除复杂流程，本流程活动为Init、N、N1、N2、N3、C。
		/// N有线指向N2。N2有退回线指向N1
		/// </summary>
		/// <returns></returns>
		private static IWfProcessDescriptor PrepareComplexProcessForDelete()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateProcessDescriptor();

			IWfActivityDescriptor normalActivity = processDesp.Activities["NormalActivity"];
			IWfActivityDescriptor normalActivity1 = processDesp.Activities["NormalActivity1"];
			IWfActivityDescriptor normalActivity2 = processDesp.Activities["NormalActivity2"];

			//N有线指向N2
			normalActivity.ToTransitions.AddForwardTransition(normalActivity2);

			//N2有退回线指向N1
			normalActivity2.ToTransitions.AddBackwardTransition(normalActivity1);

			return processDesp;
		}
	}
}
