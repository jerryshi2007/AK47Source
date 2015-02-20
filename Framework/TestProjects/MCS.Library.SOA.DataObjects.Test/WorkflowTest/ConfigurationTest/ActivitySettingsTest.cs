using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest.ConfigurationTest
{
	[TestClass]
	public class ActivitySettingsTest
	{
		[TestMethod]
		[TestCategory(ProcessTestHelper.Descriptor)]
		public void CreateInstanceTest()
		{
			WfActivitySettings setting = WfActivitySettings.GetConfig();
			var process1 = new WfProcessDescriptor() { Key = "Process1" };

			IWfActivityDescriptor actDescriptor = new WfActivityDescriptor("Init", WfActivityType.InitialActivity)
			{
				CodeName = "test",
				Description = "test",
				Enabled = true
			};

			process1.Activities.Add(actDescriptor);

			IWfActivity initActivity = setting.GetActivityBuilder(actDescriptor).CreateActivity(actDescriptor);

			Assert.IsTrue(initActivity.EnterActions.Count > 0);
			Assert.IsTrue(initActivity.LeaveActions.Count > 0);

			initActivity.EnterActions.PrepareActions(null);
			initActivity.EnterActions.PersistActions(null);

			initActivity.LeaveActions.PrepareActions(null);
			initActivity.LeaveActions.PersistActions(null);

			actDescriptor = new WfActivityDescriptor("Completed", WfActivityType.CompletedActivity)
			{
				CodeName = "test",
				Description = "test",
				Enabled = true
			};

			process1.Activities.Add(actDescriptor);

			var completedActivity = setting.GetActivityBuilder(actDescriptor).CreateActivity(actDescriptor);

			Assert.IsTrue(completedActivity.EnterActions.Count > 0);
			Assert.IsTrue(completedActivity.LeaveActions.Count > 0);
		}
	}
}
