using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Test
{
	/// <summary>
	/// Summary description for WfProcessRuntimeTest
	/// </summary>
	[TestClass]
	public class WfProcessRuntimeTest
	{
		public WfProcessRuntimeTest()
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
		[TestCategory(ProcessTestHelper.BranchProcess)]
		[Description("测试分支流程。Parallel && WaitAllBranchProcessesComplete")]
		public void WaitAllCompleteOfParallelBranchProcessTest()
		{
			IWfProcess wfProcess = GetProcessInstanceWithBranchProcessRunning(WfBranchProcessExecuteSequence.Parallel, WfBranchProcessBlockingType.WaitAllBranchProcessesComplete);
			AssertProcessStatus(wfProcess);

			CheckProcessRelationOperationAndStatus(wfProcess, true);

			//
			ProcessTestHelper.CompleteActivityBranchProcessesByBlockingType(wfProcess.CurrentActivity, WfBranchProcessBlockingType.WaitAllBranchProcessesComplete);
			ProcessTestHelper.ProcessPendingActivity(wfProcess.CurrentActivity.ID);

			IWfProcess wfProc = WfRuntime.GetProcessByProcessID(wfProcess.ID);

			Assert.AreEqual(WfActivityStatus.Running, wfProc.CurrentActivity.Status);
			Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(wfProc.CurrentActivity.ID, TaskStatus.Ban));
			wfProc.CurrentActivity.BranchProcessGroups[0].Branches.Exists(p => p.Status == WfProcessStatus.Completed);

		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.BranchProcess)]
		[Description("测试分支流程。Parallel && WaitNoneOfBranchProcessComplete")]
		public void WaitNoneCompleteOfParallelBranchProcessTest()
		{
			IWfProcess wfProcess = GetProcessInstanceWithBranchProcessRunning(WfBranchProcessExecuteSequence.Parallel, WfBranchProcessBlockingType.WaitNoneOfBranchProcessComplete);
			AssertProcessStatus(wfProcess);

			Assert.AreEqual(WfActivityStatus.Running, wfProcess.CurrentActivity.Status);
			Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(wfProcess.CurrentActivity.ID, TaskStatus.Ban));

			Assert.AreEqual(WfProcessStatus.Running, wfProcess.CurrentActivity.BranchProcessGroups[0].Branches[0].Status);
			Assert.AreEqual(WfProcessStatus.Running, wfProcess.CurrentActivity.BranchProcessGroups[0].Branches[1].Status);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.BranchProcess)]
		[Description("测试分支流程。Parallel && WaitAnyoneBranchProcessComplete")]
		public void WaitAnyoneCompleteOfParallelBranchProcessTest()
		{
			IWfProcess wfProcess = GetProcessInstanceWithBranchProcessRunning(WfBranchProcessExecuteSequence.Parallel, WfBranchProcessBlockingType.WaitAnyoneBranchProcessComplete);
			AssertProcessStatus(wfProcess);

			CheckProcessRelationOperationAndStatus(wfProcess, true);

			//
			ProcessTestHelper.CompleteActivityBranchProcessesByBlockingType(wfProcess.CurrentActivity, WfBranchProcessBlockingType.WaitAnyoneBranchProcessComplete);
			ProcessTestHelper.ProcessPendingActivity(wfProcess.CurrentActivity.ID);

			IWfProcess wfProc = WfRuntime.GetProcessByProcessID(wfProcess.ID);

			Assert.AreEqual(WfActivityStatus.Running, wfProc.Activities[1].Status);
			Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(wfProc.CurrentActivity.ID, TaskStatus.Ban));
			Assert.AreEqual(WfProcessStatus.Completed, wfProc.CurrentActivity.BranchProcessGroups[0].Branches[0].Status);
			Assert.AreEqual(WfProcessStatus.Running, wfProc.CurrentActivity.BranchProcessGroups[0].Branches[1].Status);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.BranchProcess)]
		[Description("测试分支流程。Serial && WaitAllBranchProcessesComplete")]
		public void WaitAllCompleteOfSerialBranchProcessTest()
		{
			IWfProcess wfProcess = GetProcessInstanceWithBranchProcessRunning(WfBranchProcessExecuteSequence.Serial, WfBranchProcessBlockingType.WaitAllBranchProcessesComplete);
			AssertProcessStatus(wfProcess);

			CheckProcessRelationOperationAndStatus(wfProcess, false);

			//第一个子流程办结
			wfProcess.Activities[1].BranchProcessGroups[0].Branches[0].CompleteProcess(true);
			WfRuntime.PersistWorkflows();

			wfProcess = WfRuntime.GetProcessByProcessID(wfProcess.ID);
			Assert.AreEqual(WfProcessStatus.Completed, wfProcess.CurrentActivity.BranchProcessGroups[0].Branches[0].Status);
			Assert.AreEqual(WfProcessStatus.Running, wfProcess.CurrentActivity.BranchProcessGroups[0].Branches[1].Status);
			Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(wfProcess.CurrentActivity.BranchProcessGroups[0].Branches[1].InitialActivity.ID, TaskStatus.Ban));


			//第二个子流程办结
			wfProcess.CurrentActivity.BranchProcessGroups[0].Branches[1].CompleteProcess(true);
			WfRuntime.PersistWorkflows();


			ProcessTestHelper.ProcessPendingActivity(wfProcess.CurrentActivity.ID);

			IWfProcess wfProc = WfRuntime.GetProcessByProcessID(wfProcess.ID);
			Assert.AreEqual(WfProcessStatus.Completed, wfProc.CurrentActivity.BranchProcessGroups[0].Branches[0].Status);
			Assert.AreEqual(WfProcessStatus.Completed, wfProc.CurrentActivity.BranchProcessGroups[0].Branches[1].Status);

			Assert.AreEqual(WfActivityStatus.Running, wfProc.CurrentActivity.Status);
			Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(wfProc.CurrentActivity.ID, TaskStatus.Ban));
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.BranchProcess)]
		[Description("测试分支流程。Serial && WaitNoneOfBranchProcessComplete")]
		public void WaitNoneCompleteOfSerialBranchProcessTest()
		{
			IWfProcess wfProcess = GetProcessInstanceWithBranchProcessRunning(WfBranchProcessExecuteSequence.Serial, WfBranchProcessBlockingType.WaitNoneOfBranchProcessComplete);

			Assert.AreEqual(WfActivityStatus.Running, wfProcess.CurrentActivity.Status);
			Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(wfProcess.CurrentActivity.ID, TaskStatus.Ban));

			Assert.AreEqual(WfProcessStatus.Running, wfProcess.CurrentActivity.BranchProcessGroups[0].Branches[0].Status);
			Assert.AreEqual(WfProcessStatus.NotRunning, wfProcess.CurrentActivity.BranchProcessGroups[0].Branches[1].Status);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.BranchProcess)]
		[Description("测试分支流程。Serial && WaitAnyoneBranchProcessComplete")]
		public void WaitAnyoneCompleteOfSerialBranchProcessTest()
		{
			IWfProcess wfProcess = GetProcessInstanceWithBranchProcessRunning(WfBranchProcessExecuteSequence.Serial, WfBranchProcessBlockingType.WaitAnyoneBranchProcessComplete);

			CheckProcessRelationOperationAndStatus(wfProcess, false);

			//
			ProcessTestHelper.CompleteActivityBranchProcessesByBlockingType(wfProcess.CurrentActivity, WfBranchProcessBlockingType.WaitAnyoneBranchProcessComplete);
			ProcessTestHelper.ProcessPendingActivity(wfProcess.CurrentActivity.ID);

			IWfProcess wfProc = WfRuntime.GetProcessByProcessID(wfProcess.ID);
			Assert.AreEqual(WfProcessStatus.Completed, wfProc.CurrentActivity.BranchProcessGroups[0].Branches[0].Status);
			Assert.AreEqual(WfProcessStatus.Running, wfProc.CurrentActivity.BranchProcessGroups[0].Branches[1].Status);
			Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(wfProcess.CurrentActivity.BranchProcessGroups[0].Branches[1].InitialActivity.ID, TaskStatus.Ban));
			Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(wfProc.CurrentActivity.ID, TaskStatus.Ban));
		}

		private static void AssertProcessStatus(IWfProcess process)
		{
			Assert.IsTrue(process.Status == WfProcessStatus.Running);
			Assert.IsTrue(process.InitialActivity.CanMoveTo == true);
			Assert.IsTrue(process.ElapsedActivities.Count == 1);
			Assert.AreEqual(WfActivityStatus.Completed, process.ElapsedActivities[0].Status);
		}

		private static void CheckProcessRelationOperationAndStatus(IWfProcess wfProcess, bool isParallel)
		{
			Assert.AreEqual(WfActivityStatus.Pending, wfProcess.CurrentActivity.Status);
			Assert.IsFalse(ProcessTestHelper.ExistsActivityUserTasks(wfProcess.CurrentActivity.ID, TaskStatus.Ban));

			if (isParallel)
			{
				wfProcess.CurrentActivity.BranchProcessGroups[0].Branches.Exists(p => p.Status == WfProcessStatus.Running);
				wfProcess.CurrentActivity.BranchProcessGroups[0].Branches.Exists(p => ProcessTestHelper.ExistsActivityUserTasks(p.ID, TaskStatus.Ban));
			}
			else
			{
				Assert.AreEqual(WfProcessStatus.Running, wfProcess.CurrentActivity.BranchProcessGroups[0].Branches[0].Status);
				Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(wfProcess.CurrentActivity.BranchProcessGroups[0].Branches[0].CurrentActivity.ID, TaskStatus.Ban));
				Assert.AreEqual(WfProcessStatus.NotRunning, wfProcess.Activities[1].BranchProcessGroups[0].Branches[1].Status);
			}
		}

		#region Restore Process
		[TestMethod]
		[Description("Restore Canceled流程")]
		[TestCategory(ProcessTestHelper.ProcessBehavior_Cancel)]
		public void RestoreCanceledProcessTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();

			WfRuntime.PersistWorkflows();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			IWfActivityDescriptor nextActivityDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;

			WfTransferParams tp = ProcessTestHelper.GetInstanceOfWfTransferParams(nextActivityDesp, OguObject.requestor);
			process.MoveTo(tp);

			WfRuntime.PersistWorkflows();

			UserTaskCollection beforeCancelTasks = UserTaskAdapter.Instance.LoadUserTasks(b => b.AppendItem("ACTIVITY_ID", process.CurrentActivity.ID));

			Console.WriteLine("Current Activity ID: {0}", process.CurrentActivity.ID);
			Console.WriteLine("User Task Count: {0}", beforeCancelTasks.Count);

			process = CancelProcess(process, false);
			
			process.RestoreProcess();

			WfRuntime.PersistWorkflows();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			UserTaskCollection afterRestoreTasks = UserTaskAdapter.Instance.LoadUserTasks(b => b.AppendItem("ACTIVITY_ID", process.CurrentActivity.ID));

			Console.WriteLine("Restored User Task Count: {0}", afterRestoreTasks.Count);

			Assert.AreEqual(beforeCancelTasks.Count, afterRestoreTasks.Count);

			Assert.AreEqual(WfProcessStatus.Running, process.Status);
			Assert.AreEqual(WfActivityStatus.Running, process.CurrentActivity.Status);
		}

		[TestMethod]
		[Description("Restore Canceled pending流程")]
		[TestCategory(ProcessTestHelper.ProcessBehavior_Cancel)]
		public void RestoreCanceledPendingProcessTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();

			process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity.Condition.Expression = "1 == 0";

			WfRuntime.PersistWorkflows();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			IWfActivityDescriptor nextActivityDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;

			WfTransferParams tp = ProcessTestHelper.GetInstanceOfWfTransferParams(nextActivityDesp, OguObject.requestor);
			process.MoveTo(tp);

			Assert.AreEqual(WfActivityStatus.Pending, process.CurrentActivity.Status);

			WfRuntime.PersistWorkflows();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			process.CancelProcess(true);

			WfRuntime.PersistWorkflows();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			process.RestoreProcess();

			WfRuntime.PersistWorkflows();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			Assert.AreEqual(WfProcessStatus.Running, process.Status);
			Assert.AreEqual(WfActivityStatus.Pending, process.CurrentActivity.Status);
		}
		#endregion

		#region 作废流程
		[TestMethod]
		[Description("取消单一流程，当前流程在初始节点")]
		[TestCategory(ProcessTestHelper.ProcessBehavior_Cancel)]
		public void CancelProcessInInitialActivityTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();
			WfRuntime.PersistWorkflows();

			CancelProcess(process, false);
		}

		[TestMethod]
		[Description("取消单一流程，当前流程在非初始节点")]
		[TestCategory(ProcessTestHelper.ProcessBehavior_Cancel)]
		public void CancelProcessInOtherActivityTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();

			WfRuntime.PersistWorkflows();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			IWfActivityDescriptor nextActivityDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;

			WfTransferParams tp = ProcessTestHelper.GetInstanceOfWfTransferParams(nextActivityDesp, OguObject.requestor);
			process.MoveTo(tp);

			WfRuntime.PersistWorkflows();

			UserTaskCollection tasks = UserTaskAdapter.Instance.LoadUserTasks(b => b.AppendItem("ACTIVITY_ID", process.CurrentActivity.ID));

			Console.WriteLine("Current Activity ID: {0}", process.CurrentActivity.ID);
			Console.WriteLine("User Task Count: {0}", tasks.Count);

			process = CancelProcess(process, false);

			UserTaskCollection userAccomlishedTasks = UserTaskAdapter.Instance.GetUserAccomplishedTasks(UserTaskIDType.ActivityID, UserTaskFieldDefine.All, false, process.CurrentActivity.ID);

			Console.WriteLine("User Accomplished Task Count: {0}", userAccomlishedTasks.Count);

			Assert.AreEqual(tasks.Count, userAccomlishedTasks.Count);
		}

		[TestMethod]
		[Description("取消流程中的子流程")]
		[TestCategory(ProcessTestHelper.ProcessBehavior_Cancel)]
		public void CancelSubProcessTest()
		{
			IWfProcess process = GetProcessInstanceWithBranchProcessRunning(WfBranchProcessExecuteSequence.Parallel, WfBranchProcessBlockingType.WaitAllBranchProcessesComplete);

			IWfProcess branchProcess = process.CurrentActivity.BranchProcessGroups[0].Branches[0];
			CancelProcess(branchProcess, false);

			IWfProcess branchProcess2 = process.CurrentActivity.BranchProcessGroups[0].Branches[1];
			CancelProcess(branchProcess2, false);

			process = WfRuntime.GetProcessByProcessID(process.ID);
			process.MoveTo(new WfTransferParams(process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity));
			Assert.AreEqual(WfActivityStatus.Completed, process.Activities[1].Status);
		}

		[TestMethod]
		[Description("取消流程中的子流程，查看串行中下一子流程的启动状态")]
		[TestCategory(ProcessTestHelper.ProcessBehavior_Cancel)]
		public void CancelSerialSubProcessTest()
		{
			IWfProcess process = GetProcessInstanceWithBranchProcessRunning(WfBranchProcessExecuteSequence.Serial, WfBranchProcessBlockingType.WaitAllBranchProcessesComplete);

			IWfProcess branchProcess = process.CurrentActivity.BranchProcessGroups[0].Branches[0];
			CheckOperationAndStatusBeforeCancelProcess(branchProcess);

			CancelProcess(branchProcess, false);

			process = WfRuntime.GetProcessByProcessID(process.ID);
			//检查第二个流程是否启动
			branchProcess = process.CurrentActivity.BranchProcessGroups[0].Branches[1];
			Assert.AreEqual(WfProcessStatus.Running, branchProcess.Status);

			ProcessTestHelper.CompleteActivityBranchProcessesByBlockingType(process.CurrentActivity, WfBranchProcessBlockingType.WaitAllBranchProcessesComplete);

			WfRuntime.PersistWorkflows();

			ProcessTestHelper.ProcessPendingActivity(process.CurrentActivity.ID);

			process = WfRuntime.GetProcessByProcessID(process.ID);
			branchProcess = process.CurrentActivity.BranchProcessGroups[0].Branches[1];
			Assert.AreEqual(WfProcessStatus.Completed, branchProcess.Status, "第二个子流程的状态");
		}

		[TestMethod]
		[Description("取消带子流程的流程，不包括当前点的子流程")]
		[TestCategory(ProcessTestHelper.ProcessBehavior_Cancel)]
		public void CancelProcessWithBranchProcess()
		{
			IWfProcess process = GetProcessInstanceWithBranchProcessRunning(WfBranchProcessExecuteSequence.Parallel, WfBranchProcessBlockingType.WaitAnyoneBranchProcessComplete);

			CancelProcess(process, false);
		}

		[TestMethod]
		[Description("取消带子流程的流程，包括当前点的子流程,流程中为运行状态 ")]
		[TestCategory(ProcessTestHelper.ProcessBehavior_Cancel)]
		public void CancelProcessAndBranchProcessRunning()
		{
			IWfProcess process = GetProcessInstanceWithBranchProcessRunning(WfBranchProcessExecuteSequence.Parallel, WfBranchProcessBlockingType.WaitAllBranchProcessesComplete);

			CheckOperationAndStatusBeforeCancelProcess(process.CurrentActivity.BranchProcessGroups[0].Branches[0]);
			CheckOperationAndStatusBeforeCancelProcess(process.CurrentActivity.BranchProcessGroups[0].Branches[1]);

			CancelProcess(process, true);

			process = WfRuntime.GetProcessByProcessID(process.ID);

			CheckOperationAndStatusAfterCancelProcess(process.CurrentActivity.BranchProcessGroups[0].Branches[0]);
			CheckOperationAndStatusAfterCancelProcess(process.CurrentActivity.BranchProcessGroups[0].Branches[1]);
		}

		[TestMethod]
		[Description("取消带子流程的流程，包括当前点的子流程,流程中有完成状态")]
		[TestCategory(ProcessTestHelper.ProcessBehavior_Cancel)]
		public void CancelProcessAndBranchProcessCompleted()
		{
			IWfProcess process = GetProcessInstanceWithBranchProcessRunning(WfBranchProcessExecuteSequence.Parallel, WfBranchProcessBlockingType.WaitAnyoneBranchProcessComplete);

			ProcessTestHelper.CompleteActivityBranchProcessesByBlockingType(process.CurrentActivity, WfBranchProcessBlockingType.WaitAnyoneBranchProcessComplete);
			ProcessTestHelper.ProcessPendingActivity(process.CurrentActivity.ID);

			Assert.AreEqual(WfProcessStatus.Completed, process.CurrentActivity.BranchProcessGroups[0].Branches[0].Status);
			Assert.AreEqual(WfProcessStatus.Running, process.CurrentActivity.BranchProcessGroups[0].Branches[1].Status);

			CancelProcess(process, true);

			process = WfRuntime.GetProcessByProcessID(process.ID);
			CheckOperationAndStatusAfterCancelProcess(process.Activities[1].BranchProcessGroups[0].Branches[0]);
			CheckOperationAndStatusAfterCancelProcess(process.Activities[1].BranchProcessGroups[0].Branches[1]);
		}

		private IWfProcess CancelProcess(IWfProcess process, bool cancelAllBranchProcess)
		{
			process = WfRuntime.GetProcessByProcessID(process.ID);
			CheckOperationAndStatusBeforeCancelProcess(process);

			process.CancelProcess(cancelAllBranchProcess);
			WfRuntime.PersistWorkflows();

			process = WfRuntime.GetProcessByProcessID(process.ID);
			CheckOperationAndStatusAfterCancelProcess(process);

			return process;
		}

		private static void CheckOperationAndStatusBeforeCancelProcess(IWfProcess process)
		{
			Assert.AreEqual(WfProcessStatus.Running, process.Status);

			if (process.CurrentActivity.BranchProcessGroups.IsBlocking(WfBranchGroupBlockingType.WaitAllBranchGroupsComplete))
			{
				Assert.AreEqual(WfActivityStatus.Pending, process.CurrentActivity.Status);
				Assert.IsFalse(ProcessTestHelper.ExistsActivityUserTasks(process.CurrentActivity.ID, TaskStatus.Ban));
			}
			else
			{
				Assert.AreEqual(WfActivityStatus.Running, process.CurrentActivity.Status);
				Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(process.CurrentActivity.ID, TaskStatus.Ban));
			}
		}

		private void CheckOperationAndStatusAfterCancelProcess(IWfProcess process)
		{
			Assert.AreEqual(WfProcessStatus.Aborted, process.Status);
			Assert.AreEqual(WfActivityStatus.Aborted, process.CurrentActivity.Status);
			Assert.IsFalse(ProcessTestHelper.ExistsActivityUserTasks(process.CurrentActivity.ID, TaskStatus.Ban));
			if (process.CurrentActivity.Descriptor.ActivityType != WfActivityType.CompletedActivity)
			{
				Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(process.CurrentActivity.ID, TaskStatus.Yue));
			}
		}
		#endregion 作废流程

		#region 撤回流程
		[TestMethod]
		[Description("撤回流程,即被撤回的点到目标点中不存在子流程")]
		[TestCategory(ProcessTestHelper.ProcessBehavior_Withdraw)]
		public void WithdrawProcessTest()
		{
			IWfProcess process = GetProcessInstance(true);

			string fromActivityID = process.CurrentActivity.ID;

			process.Withdraw(process.Activities[2], false);

			WfRuntime.PersistWorkflows();

			CheckOperationAndStatusAfterWithdrawProcess(process.ID, fromActivityID, process.Activities[2].ID);
		}

		[TestMethod]
		[Description("撤回流程，即撤回的点到目标点中存在子流程，但不取消子流程")]
		[TestCategory(ProcessTestHelper.ProcessBehavior_Withdraw)]
		public void WithdrawProcessWithBranchProcess()
		{
			IWfProcess process = GetProcessInstance(true);

			string fromActivityID = process.CurrentActivity.ID;

			process.Withdraw(process.Activities[0], false);
			WfRuntime.PersistWorkflows();

			//撤销两步后查阅状态
			CheckOperationAndStatusAfterWithdrawProcess(process.ID, fromActivityID, process.Activities[0].ID);
		}

		[TestMethod]
		[Description("撤回流程，即撤回的点到目标点中存在子流程，同时取消子流程，区别：当前节点已挂起")]
		[TestCategory(ProcessTestHelper.ProcessBehavior_Withdraw)]
		public void WithdrawProcessAndCancelBranchProcessRunning()
		{
			IWfProcess process = GetProcessInstance(false);

			string fromActivityId = process.CurrentActivity.ID;

			process.Withdraw(process.Activities[0], true);
			WfRuntime.PersistWorkflows();

			CheckOperationAndStatusAfterWithdrawProcess(process.ID, fromActivityId, process.Activities[0].ID);

		}

		[TestMethod]
		[Description("撤回被挂起的活动点.再次执行时不带分支流程，查看这个点的分支流程")]
		[TestCategory(ProcessTestHelper.ProcessBehavior_Withdraw)]
		public void WithdrawAndReplayRunProcessActivity()
		{
			IWfProcess process = GetProcessInstance(false);
			string fromActivityId = process.CurrentActivity.ID;

			string branchProcessID1 = process.CurrentActivity.BranchProcessGroups[0].Branches[0].ID;
			string branchProcessID2 = process.CurrentActivity.BranchProcessGroups[0].Branches[1].ID;

			process.Withdraw(process.Activities[0], true);
			WfRuntime.PersistWorkflows();

			CheckOperationAndStatusAfterWithdrawProcess(process.ID, fromActivityId, process.Activities[0].ID);

			IWfProcess proc = WfRuntime.GetProcessByProcessID(process.ID);

			WfTransferParams pa = new WfTransferParams(proc.Activities[1].Descriptor);
			proc.MoveTo(pa);

			Assert.AreEqual(2, proc.CurrentActivity.BranchProcessGroups[0].Branches.Count, "当前节点的分支流程应为在撤回前的个数，即2");
			Assert.AreEqual(branchProcessID1, proc.CurrentActivity.BranchProcessGroups[0].Branches[0].ID);
			Assert.AreEqual(branchProcessID2, proc.CurrentActivity.BranchProcessGroups[0].Branches[1].ID);
		}

		[TestMethod]
		[Description("撤回被挂起的活动点.再次执行时带分支流程，查看这个点的分支流程")]
		[TestCategory(ProcessTestHelper.ProcessBehavior_Withdraw)]
		public void WithdrawAndReplayRunProcessActivityWithBranchProcess()
		{
			IWfProcess process = GetProcessInstance(false);

			string fromActivityId = process.CurrentActivity.ID;
			process.Withdraw(process.Activities[0], true);//撤回
			WfRuntime.PersistWorkflows();

			CheckOperationAndStatusAfterWithdrawProcess(process.ID, fromActivityId, process.Activities[0].ID);

			process = WfRuntime.GetProcessByProcessID(process.ID);
			WfTransferParams pa = ProcessTestHelper.GetInstanceOfWfTransferParams(process, WfBranchProcessExecuteSequence.Parallel, WfBranchProcessBlockingType.WaitAllBranchProcessesComplete);
			process.MoveTo(pa);

			Assert.AreEqual(4, process.CurrentActivity.BranchProcessGroups[0].Branches.Count, "当前节点的分支流程应为撤回前与当前的，即4");
		}

		[TestMethod]
		[Description("撤回流程，即撤回的点到目标点中存在子流程，同时取消子流程，区别：被挂起的节点已流转到下头节点了")]
		[TestCategory(ProcessTestHelper.ProcessBehavior_Withdraw)]
		public void WithdrawProcessAndCancelBranchProcessCompleted()
		{
			IWfProcess process = GetProcessInstance(true);

			string fromActivityId = process.CurrentActivity.ID;

			process.Withdraw(process.Activities[0], true);
			WfRuntime.PersistWorkflows();

			CheckOperationAndStatusAfterWithdrawProcess(process.ID, fromActivityId, process.Activities[0].ID);

			process = WfRuntime.GetProcessByProcessID(process.ID);
			CheckOperationAndStatusAfterCancelProcess(process.Activities[1].BranchProcessGroups[0].Branches[0]);
			CheckOperationAndStatusAfterCancelProcess(process.Activities[1].BranchProcessGroups[0].Branches[1]);
		}
		#endregion 撤回流程

		private static IWfProcess GetProcessInstanceWithBranchProcessRunning(WfBranchProcessExecuteSequence sequence, WfBranchProcessBlockingType blockingType)
		{
			IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();
			WfTransferParams tp = ProcessTestHelper.GetInstanceOfWfTransferParams(process, sequence, blockingType);

			process.MoveTo(tp);

			WfRuntime.PersistWorkflows();

			return WfRuntime.GetProcessByProcessID(process.ID);
		}

		private static IWfProcess GetProcessInstance(bool movetoCompletedActivity)
		{
			IWfProcess process = GetProcessInstanceWithBranchProcessRunning(WfBranchProcessExecuteSequence.Parallel, WfBranchProcessBlockingType.WaitAnyoneBranchProcessComplete);

			process = WfRuntime.GetProcessByProcessID(process.ID);

			if (movetoCompletedActivity)
			{
				//使第二个节点可以正常流转
				ProcessTestHelper.CompleteActivityBranchProcessesByBlockingType(process.CurrentActivity, WfBranchProcessBlockingType.WaitAnyoneBranchProcessComplete);
				ProcessTestHelper.ProcessPendingActivity(process.CurrentActivity.ID);

				IWfProcess wfProc = WfRuntime.GetProcessByProcessID(process.ID);

				Assert.AreEqual(WfActivityStatus.Running, wfProc.Activities[1].Status);
				Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(wfProc.CurrentActivity.ID, TaskStatus.Ban));
				Assert.AreEqual(WfProcessStatus.Completed, wfProc.CurrentActivity.BranchProcessGroups[0].Branches[0].Status);
				Assert.AreEqual(WfProcessStatus.Running, wfProc.CurrentActivity.BranchProcessGroups[0].Branches[1].Status);


				//初始节点已完成，从第二个节点开始
				for (int i = 2; i < process.Activities.Count; i++)
				{
					if (process.Activities[i].Descriptor.ActivityType != WfActivityType.CompletedActivity)
					{
						process = WfRuntime.GetProcessByProcessID(process.ID);
						WfTransferParams transferPara = new WfTransferParams(process.Activities[i].Descriptor);
						transferPara.Assignees.Add((IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object);

						process.MoveTo(transferPara);
						WfRuntime.PersistWorkflows();
					}
				}
			}

			process = WfRuntime.GetProcessByProcessID(process.ID);
			return process;
		}

		private static void CheckOperationAndStatusAfterWithdrawProcess(string processId, string fromActivityId, string toActivityId)
		{
			IWfProcess process = WfRuntime.GetProcessByProcessID(processId);
			bool flag = false;

			for (int i = 0; i < process.Activities.Count; i++)
			{
				if (flag)
				{
					Assert.AreEqual(WfActivityStatus.NotRunning, process.Activities[i].Status);
					Assert.IsFalse(ProcessTestHelper.ExistsActivityUserTasks(process.Activities[i].ID, TaskStatus.Ban));
				}
				if (process.Activities[i].ID == toActivityId)
				{
					Assert.AreEqual(WfActivityStatus.Running, process.Activities[i].Status);
					Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(process.Activities[i].ID, TaskStatus.Ban));
					flag = true;
				}

				if (process.Activities[i].ID == fromActivityId)
				{
					Assert.IsFalse(ProcessTestHelper.ExistsActivityUserTasks(process.Activities[i].ID, TaskStatus.Ban));
					Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(process.Activities[i].ID, TaskStatus.Yue));
					break;
				}
			}

		}
	}
}
