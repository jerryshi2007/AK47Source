using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.OGUPermission;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest.JsonSerializationTest
{
	[TestClass]
	public class WfControlNextStepJsonTest
	{
		[TestMethod]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		public void WfControlNextStepJsonSerilizationTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();

			process.InitialActivity.Descriptor.ToTransitions[0].ToActivity.Resources.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object));

			WfConverterHelper.RegisterConverters();

			WfControlNextStep nextStep = new WfControlNextStep(process.InitialActivity.Descriptor.ToTransitions[0], process.CurrentActivity);

			string serializedData = JSONSerializerExecute.Serialize(nextStep);

			Console.WriteLine(serializedData);

			WfControlNextStep deserializedNextStep = JSONSerializerExecute.Deserialize<WfControlNextStep>(serializedData);

			string reserializedData = JSONSerializerExecute.Serialize(deserializedNextStep);

			Assert.AreEqual(serializedData, reserializedData);
		}
	}
}
