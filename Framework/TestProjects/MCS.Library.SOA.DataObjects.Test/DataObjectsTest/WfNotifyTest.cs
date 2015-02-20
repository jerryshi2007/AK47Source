using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Test.DataObjectsTest
{
	/// <summary>
	/// Summary description for WfNotifyTest
	/// </summary>
	[TestClass]
	public class WfNotifyTest
	{
		public WfNotifyTest()
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
		[TestCategory(ProcessTestHelper.Notify)]
		[Description("取消流程时，被通知人是否收到通知")]
		public void NoticeOfCancelProcess()
		{
			IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();

			WfUserResourceDescriptor userResDesp = GetUserResDespInstance();
			process.Descriptor.CancelEventReceivers.Add(userResDesp);
			WfRuntime.PersistWorkflows();

			process = WfRuntime.GetProcessByProcessID(process.ID);
			process.CancelProcess(false);
			WfRuntime.PersistWorkflows();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(userResDesp.User.ID, process.CurrentActivity.ID, TaskStatus.Yue));

		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Notify)]
		[Description("流程即将进入节点时，查看被通知人是否收到通知")]
		public void NoticeOfEnterActivity()
		{
			IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();

			IWfActivityDescriptor nextActivityDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
			
			WfUserResourceDescriptor userResDesp = GetUserResDespInstance();
			nextActivityDesp.EnterEventReceivers.Add(userResDesp);

			WfTransferParams tp = ProcessTestHelper.GetInstanceOfWfTransferParams(nextActivityDesp, OguObject.approver2);
			process.MoveTo(tp);
			WfRuntime.PersistWorkflows();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(userResDesp.User.ID, process.CurrentActivity.ID, TaskStatus.Yue));

		}

		private static WfUserResourceDescriptor GetUserResDespInstance()
		{
			WfUserResourceDescriptor userResDesp = new WfUserResourceDescriptor();
			userResDesp.User = (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object;
			return userResDesp;
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Notify)]
		[Description("离开节点，检查被通知人是否收到通知")]
		public void NoticeOfLeaveActivity()
		{
			IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();

			IWfActivityDescriptor nextActivityDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;

			WfUserResourceDescriptor userResDesp = GetUserResDespInstance();
			nextActivityDesp.LeaveEventReceivers.Add(userResDesp);

			WfTransferParams tp = ProcessTestHelper.GetInstanceOfWfTransferParams(nextActivityDesp, OguObject.approver2);
			process.MoveTo(tp);

			WfRuntime.PersistWorkflows();
			process = WfRuntime.GetProcessByProcessID(process.ID);

			process.MoveTo(new WfTransferParams(process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity));

			WfRuntime.PersistWorkflows();
			process = WfRuntime.GetProcessByProcessID(process.ID);
			string actKey = process.CurrentActivity.Descriptor.FromTransitions[0].FromActivity.Key;

			Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(userResDesp.User.ID, process.Activities[1].ID, TaskStatus.Yue));
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Notify)]
		[Description("使带子流程节点可以继续流转后，查看带子流程的节点的待阅")]
		public void NoticeOfAllBranchProcessCompleted()
		{
			IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();

			IWfActivityDescriptor actDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
			WfUserResourceDescriptor userResDesp = GetUserResDespInstance();
			actDesp.EnterEventReceivers.Add(userResDesp);

			WfTransferParams tp = ProcessTestHelper.GetInstanceOfWfTransferParams(process, WfBranchProcessExecuteSequence.Parallel, WfBranchProcessBlockingType.WaitAllBranchProcessesComplete);
			process.MoveTo(tp);

			WfRuntime.PersistWorkflows();
			process = WfRuntime.GetProcessByProcessID(process.ID);
			ProcessTestHelper.CompleteActivityBranchProcessesByBlockingType(process.CurrentActivity, WfBranchProcessBlockingType.WaitAllBranchProcessesComplete);
			ProcessTestHelper.ProcessPendingActivity(process.CurrentActivity.ID);

			WfRuntime.PersistWorkflows();
			process = WfRuntime.GetProcessByProcessID(process.ID);

			Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(userResDesp.User.ID, process.Activities[1].ID, TaskStatus.Yue), "节点状态由pending到running后，则被通知人会收到１个待阅");

		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Notify)]
		[Description("存在委托的时候，查看被通知的人收到的通知")]
		public void RepeatNoticeOfDelegation()
		{ 
			//sourceUser: "approver1"; destinationUser:"requestor"
			WfDelegation delegation = WfDelegationTest.PrepareDelegation();

			WfDelegationAdapter.Instance.Update(delegation);  

			try
			{
				IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();

				IWfActivityDescriptor actDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;

				WfTransferParams tp = new WfTransferParams(actDesp);
				IUser user1 = (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object;
				IUser user2 = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object; 
				tp.Assignees.Add(user1);
				tp.Assignees.Add(user2);  

				process.MoveTo(tp);
				WfRuntime.PersistWorkflows();


				Assert.AreEqual(3, process.CurrentActivity.Assignees.Count);

				int count = Find(process.CurrentActivity.Assignees, user2.ID);
				Assert.AreEqual(2, count); //存在相同的人员，只是Assignees类型不同（委派与非委派）

				process = WfRuntime.GetProcessByProcessID(process.ID);
				Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(process.CurrentActivity.ID, TaskStatus.Ban));
				Assert.AreEqual(2, ProcessTestHelper.GetActivityUserTasks(process.CurrentActivity.ID, TaskStatus.Ban).Count);//两个人２条待办。如果委托后有重复没排除，会是３条

			}
			finally
			{
				WfDelegationAdapter.Instance.Delete(delegation);　//清理委托
			}
		}

		private int Find(WfAssigneeCollection wfAssigneeCollection, string id)
		{
			int i = 0;
			foreach (var assignee in wfAssigneeCollection)
			{
				if (string.Compare(assignee.User.ID, id) == 0)
				{
					i++;
				}
			}
			return i;
		}

	}
}
