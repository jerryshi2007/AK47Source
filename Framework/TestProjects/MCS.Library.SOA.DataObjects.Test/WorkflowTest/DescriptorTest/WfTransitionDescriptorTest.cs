using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest.DescriptorTest
{
	/// <summary>
	/// 流程活动间的线测试
	/// </summary>
	[TestClass]
	public class WfTransitionDescriptorTest
	{
		[TestMethod]
		[TestCategory(ProcessTestHelper.Descriptor)]
		[Description("测试删除活动的出线")]
		public void RemoveToTransitionTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

			IWfActivityDescriptor fromActDesp = processDesp.InitialActivity;

			IWfActivityDescriptor toActDesp = fromActDesp.ToTransitions.FirstOrDefault().ToActivity;

			fromActDesp.ToTransitions.RemoveByToActivity(toActDesp);

			Assert.IsNull(fromActDesp.ToTransitions.Find(t => t.ToActivity.Key == toActDesp.Key));
			Assert.IsNull(toActDesp.FromTransitions.Find(t => t.FromActivity.Key == fromActDesp.Key));
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Descriptor)]
		[Description("测试删除活动的进线")]
		public void RemoveFromTransitionTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

			IWfActivityDescriptor fromActDesp = processDesp.InitialActivity;

			IWfActivityDescriptor toActDesp = fromActDesp.ToTransitions.FirstOrDefault().ToActivity;

			toActDesp.FromTransitions.RemoveByFromActivity(fromActDesp);

			Assert.IsNull(fromActDesp.ToTransitions.Find(t => t.ToActivity.Key == toActDesp.Key));
			Assert.IsNull(toActDesp.FromTransitions.Find(t => t.FromActivity.Key == fromActDesp.Key));
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Descriptor)]
		[Description("测试清空活动的出线")]
		public void ClearToTransitionTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

			IWfActivityDescriptor fromActDesp = processDesp.InitialActivity;

			IWfActivityDescriptor toActDesp = fromActDesp.ToTransitions.FirstOrDefault().ToActivity;

			fromActDesp.ToTransitions.Clear();

			Assert.AreEqual(0, fromActDesp.ToTransitions.Count);

			Assert.IsNull(fromActDesp.ToTransitions.Find(t => t.ToActivity.Key == toActDesp.Key));
			Assert.IsNull(toActDesp.FromTransitions.Find(t => t.FromActivity.Key == fromActDesp.Key));
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Descriptor)]
		[Description("测试清空活动的进线")]
		public void ClearFromTransitionTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

			IWfActivityDescriptor fromActDesp = processDesp.InitialActivity;

			IWfActivityDescriptor toActDesp = fromActDesp.ToTransitions.FirstOrDefault().ToActivity;

			toActDesp.FromTransitions.Clear();

			Assert.AreEqual(0, toActDesp.FromTransitions.Count);

			Assert.IsNull(fromActDesp.ToTransitions.Find(t => t.ToActivity.Key == toActDesp.Key));
			Assert.IsNull(toActDesp.FromTransitions.Find(t => t.FromActivity.Key == fromActDesp.Key));
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Descriptor)]
		[Description("测试将某一组线Clone后，合并到某个活动的出线中")]
		public void MergeToTransitionsTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

			IWfActivityDescriptor fromActDesp = processDesp.InitialActivity;

			IWfActivityDescriptor toActDesp = fromActDesp.ToTransitions.FirstOrDefault().ToActivity;

			List<IWfTransitionDescriptor> transitionsNeedToMerge = new List<IWfTransitionDescriptor>();

			toActDesp.ToTransitions.CopyTo(transitionsNeedToMerge);
			fromActDesp.ToTransitions.CopyTo(transitionsNeedToMerge);

			IList<IWfTransitionDescriptor> addedTransitions = fromActDesp.CloneAndMergeToTransitions(transitionsNeedToMerge);

			Assert.AreEqual(1, addedTransitions.Count);
			Assert.AreEqual(2, fromActDesp.ToTransitions.Count);

			Assert.IsNotNull(fromActDesp.ToTransitions.Find(t => t.ToActivity == processDesp.CompletedActivity));
			Assert.IsNotNull(processDesp.CompletedActivity.FromTransitions.Find(t => t.FromActivity == fromActDesp));
		}
	}
}
