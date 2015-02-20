using System;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest.UserFunction
{
	[TestClass]
	public class WfUserFunctionTest
	{
		[TestMethod]
		public void InlineUserFunctionTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

			IWfActivityDescriptor normalActivityDesp = processDesp.Activities["NormalActivity"];

			normalActivityDesp.Resources.Clear();
			normalActivityDesp.Resources.Add(new WfDynamicResourceDescriptor("InlineUserFunc", string.Format("InlineUser(\"{0}\")", "ceo")));

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			IWfActivity normalActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			Assert.IsTrue(normalActivity.Candidates.Count > 0);
			Assert.AreEqual(OguObjectSettings.GetConfig().Objects["ceo"].Object.ID, normalActivity.Candidates[0].User.ID);
		}
	}
}
