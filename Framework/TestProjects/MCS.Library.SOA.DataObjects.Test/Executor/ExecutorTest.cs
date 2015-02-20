using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Workflow.Tasks;

namespace MCS.Library.SOA.DataObjects.Test
{
	[TestClass]
	public class ExecutorTest
	{
		[TestMethod]
		[Description("启动流程Executor测试")]
		[TestCategory(ProcessTestHelper.Executor)]
		public void StartWorkflowExecutorTest()
		{
			IUser user = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object;

			string procResourceId = GetStartWorkflowResourceID(user);

			WfProcessCollection processes = WfRuntime.GetProcessByResourceID(procResourceId);

			Assert.AreEqual(1, processes.Count);
			Assert.AreEqual(0, processes[0].ElapsedActivities.Count);

			WfUserResourceDescriptor uDesp = (WfUserResourceDescriptor)processes[0].InitialActivity.Descriptor.Resources[0];

			Assert.AreEqual(user.ID, uDesp.User.ID);

		}

		private static string GetStartWorkflowResourceID(IUser user)
		{
			IWfProcessDescriptor procDesp = WfProcessTestCommon.CreateProcessDescriptor();
			WfUserResourceDescriptor userDesp = new WfUserResourceDescriptor(user);
			procDesp.InitialActivity.Resources.Add(userDesp);

			WfProcessStartupParams startParams = WfProcessTestCommon.GetInstanceOfWfProcessStartupParams(procDesp);
			WfStartWorkflowExecutor exec = new WfStartWorkflowExecutor(null, startParams);
			exec.Execute();

			return startParams.ResourceID;
		}

		[TestMethod]
		[Description("流程流转Executor测试")]
		[TestCategory(ProcessTestHelper.Executor)]
		public void MoveToExecutorTest()
		{
			IUser user = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object;
			string procResourceId = GetStartWorkflowResourceID(user);

			WfProcessCollection processes = WfRuntime.GetProcessByResourceID(procResourceId);
			IWfProcess process = processes[0];
			string nextActivityDespKey = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity.Key;

			MoveToExecutor(process);

			IWfProcess proc = WfRuntime.GetProcessByProcessID(process.ID);
			Assert.AreEqual(nextActivityDespKey, proc.CurrentActivity.Descriptor.Key);
		}

		private static void MoveToExecutor(IWfProcess process)
		{
			IWfActivityDescriptor nextActivityDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
			WfTransferParams pa = ProcessTestHelper.GetInstanceOfWfTransferParams(nextActivityDesp, OguObject.approver1);
			WfMoveToExecutor moveExec = new WfMoveToExecutor(process.CurrentActivity, process.CurrentActivity, pa);

			moveExec.Execute();
		}

		[TestMethod]
		[Description("会签Executor测试")]
		[TestCategory(ProcessTestHelper.Executor)]
		public void ConsignExecutorTest()
		{
			IUser user = (IUser)OguObjectSettings.GetConfig().Objects["approver2"].Object;
			string procResourceID = GetStartWorkflowResourceID(user);
			string processID = string.Empty;

			WfProcessCollection processes = WfRuntime.GetProcessByResourceID(procResourceID);
			processID = processes[0].ID;
			MoveToExecutor(processes[0]);
			Assert.AreEqual(1, processes[0].ElapsedActivities.Count);

			processes = WfRuntime.GetProcessByResourceID(procResourceID);
			List<IUser> users;
			ConsignExecutor(processes[0], out users);

			processes = WfRuntime.GetProcessByResourceID(procResourceID);
			Assert.AreEqual(3, processes.Count, "共3个流程，主流程与两个子流程");

			IWfProcess proc = WfRuntime.GetProcessByProcessID(processID);
			Assert.AreEqual(7, proc.Activities.Count);
			string k = proc.CurrentActivity.Descriptor.FromTransitions[0].FromActivity.Key;
			Assert.IsNotNull(proc.Activities.FindActivityByDescriptorKey(k).CreatorInstanceID, "添加会签点的活动确实没值");

			Assert.AreEqual(WfActivityStatus.Pending, proc.CurrentActivity.Status);//NO
			Assert.AreEqual(users.Count, proc.CurrentActivity.BranchProcessGroups[0].Branches.Count);

			//使子流程完成
			proc = WfRuntime.GetProcessByProcessID(processID);
			ProcessTestHelper.CompleteActivityBranchProcessesByBlockingType(proc.CurrentActivity, WfBranchProcessBlockingType.WaitAllBranchProcessesComplete);
			WfRuntime.PersistWorkflows();
			ProcessTestHelper.ProcessPendingActivity(proc.CurrentActivity.ID);

			proc = WfRuntime.GetProcessByProcessID(processID);
			Assert.AreEqual(WfActivityStatus.Running, proc.CurrentActivity.Status, "此为添加的会签点");
			Assert.AreEqual(2, proc.CurrentActivity.BranchProcessGroups[0].Branches.Count, "存在两个子流程");
			Assert.AreEqual(2, proc.ElapsedActivities.Count);
			MoveToExecutor(proc);
			Assert.AreEqual(3, proc.ElapsedActivities.Count);
		}

		[TestMethod]
		[Description("通过纯流转操作来验证会签流程和解锁操作")]
		[TestCategory(ProcessTestHelper.Executor)]
		public void ConsignExecutorWithMoveToTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			List<IUser> users = null;

			//添加会签活动且启动了分支流程
			ConsignExecutorWithCondition(process, out users);

			process = WfRuntime.GetProcessByProcessID(process.ID);

			Assert.IsTrue(process.CurrentActivity.BranchProcessGroups.Count > 0);
			Assert.AreEqual(users.Count, process.CurrentActivity.BranchProcessGroups[0].Branches.Count);

			IWfActivity originalActivity = process.CurrentActivity;

			Assert.AreEqual(WfActivityStatus.Pending, process.CurrentActivity.Status);

			foreach (IWfProcess branchProcess in process.CurrentActivity.BranchProcessGroups[0].Branches)
			{
				IWfProcess subProcess = WfRuntime.GetProcessByProcessID(branchProcess.ID);

				MoveToExecutor(subProcess);
			}

			process = WfRuntime.GetProcessByProcessID(process.ID);

			Assert.AreEqual(originalActivity.ID, process.CurrentActivity.ID);
			Assert.AreEqual(WfActivityStatus.Running, process.CurrentActivity.Status);
		}

		private static void ConsignExecutor(IWfProcess process, out List<IUser> users)
		{
			WfAssigneeCollection assignees = new WfAssigneeCollection();
			assignees.Add((IUser)OguObjectSettings.GetConfig().Objects[OguObject.approver1.ToString()].Object);

			users = new List<IUser>();
			users.Add((IUser)OguObjectSettings.GetConfig().Objects[OguObject.requestor.ToString()].Object);
			users.Add((IUser)OguObjectSettings.GetConfig().Objects[OguObject.approver2.ToString()].Object);

			WfConsignExecutor exec = new WfConsignExecutor(process.CurrentActivity, process.CurrentActivity, assignees, users, new List<IUser>(), WfBranchProcessBlockingType.WaitAllBranchProcessesComplete, WfBranchProcessExecuteSequence.Serial);
			exec.Execute();
		}

		private static void ConsignExecutorWithCondition(IWfProcess process, out List<IUser> users)
		{
			WfAssigneeCollection assignees = new WfAssigneeCollection();
			assignees.Add((IUser)OguObjectSettings.GetConfig().Objects[OguObject.approver1.ToString()].Object);

			users = new List<IUser>();
			users.Add((IUser)OguObjectSettings.GetConfig().Objects[OguObject.requestor.ToString()].Object);
			users.Add((IUser)OguObjectSettings.GetConfig().Objects[OguObject.approver2.ToString()].Object);

			WfConsignExecutor exec = new WfConsignExecutor(process.CurrentActivity, process.CurrentActivity, assignees, users, new List<IUser>(), WfBranchProcessBlockingType.WaitAllBranchProcessesComplete, WfBranchProcessExecuteSequence.Parallel);
			exec.Execute();
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("业务上的单步撤回")]
		public void SingleStepWithDrawTest()
		{
			IUser user = (IUser)OguObjectSettings.GetConfig().Objects["approver2"].Object;
			string procResourceId = GetStartWorkflowResourceID(user);

			WfProcessCollection processes;
			for (int i = 0; i < 2; i++)
			{
				processes = WfRuntime.GetProcessByResourceID(procResourceId);
				MoveToExecutor(processes[0]);
			}

			processes = WfRuntime.GetProcessByResourceID(procResourceId);
			IWfProcess process = processes[0];
			Assert.AreEqual(2, process.ElapsedActivities.Count);

			WfWithdrawExecutor exec = new WfWithdrawExecutor(process.CurrentActivity, process.CurrentActivity);
			exec.Execute();

			processes = WfRuntime.GetProcessByResourceID(procResourceId);
			MoveToExecutor(processes[0]);

			processes = WfRuntime.GetProcessByResourceID(procResourceId);
			WfActivityCollection coll = processes[0].ElapsedActivities;
			Assert.AreEqual(coll[coll.Count - 1].Descriptor.ToTransitions[0].ToActivity.Key, processes[0].CurrentActivity.Descriptor.Key);

			//撤回的单步是指业务上的一步，如在上一步动态添加的两个点，则当前就会撤回到动态添加两个点的活动
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("业务上的撤回单步操作，同时撤回时有存在动态添加的活动")]
		public void SingleStepWithDrawByDynamicAdd()
		{
			//Initial  NormalActivity  NormalActivity1 NormalActivity2 NormalActivity3 Completed
			IUser user = (IUser)OguObjectSettings.GetConfig().Objects["approver2"].Object;
			string procResourceId = GetStartWorkflowResourceID(user);

			WfProcessCollection processes = WfRuntime.GetProcessByResourceID(procResourceId);
			string processId = processes[0].ID;
			MoveToExecutor(processes[0]);
			Assert.AreEqual(1, processes[0].ElapsedActivities.Count);//initial

			processes = WfRuntime.GetProcessByResourceID(procResourceId);
			List<IUser> users;
			ConsignExecutor(processes[0], out users);

			IWfProcess proc = WfRuntime.GetProcessByProcessID(processId);
			Assert.AreEqual(2, proc.ElapsedActivities.Count);//NormalActivity
			ProcessTestHelper.CompleteActivityBranchProcessesByBlockingType(proc.CurrentActivity, WfBranchProcessBlockingType.WaitAllBranchProcessesComplete);
			WfRuntime.PersistWorkflows();
			ProcessTestHelper.ProcessPendingActivity(proc.CurrentActivity.ID);

			proc = WfRuntime.GetProcessByProcessID(processId);
			WfWithdrawExecutor withdrawExec = new WfWithdrawExecutor(proc.CurrentActivity, proc.CurrentActivity);//"NO"
			withdrawExec.Execute();

			proc = WfRuntime.GetProcessByProcessID(processId);
			Assert.AreEqual(1, proc.ElapsedActivities.Count);
			Assert.AreEqual(proc.Descriptor.GetMainStreamActivities().Count, proc.Activities.Count);
			Assert.AreEqual(proc.ElapsedActivities[0].Descriptor.ToTransitions[0].ToActivity.Key, proc.CurrentActivity.Descriptor.Key);
			Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(proc.CurrentActivity.ID, TaskStatus.Ban));
			Assert.IsTrue(proc.CurrentActivity.Descriptor.ToTransitions.Count == 1);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("业务上的撤回单步操作，同时撤回时有存在动态添加的活动。如：会签，撤回，再会签。")]
		public void SingleStepWithdrawByDynamicAddReplay()
		{
			//会签都是针对同样的两个人操作，且串行
			IUser user = (IUser)OguObjectSettings.GetConfig().Objects["approver2"].Object;
			string procResourceId = GetStartWorkflowResourceID(user);

			WfProcessCollection processes = WfRuntime.GetProcessByResourceID(procResourceId);
			string processID = processes[0].ID;

			MoveToExecutor(processes[0]);
			Assert.AreEqual(1, processes[0].ElapsedActivities.Count);//initial

			IWfProcess proc = WfRuntime.GetProcessByProcessID(processID);
			List<IUser> users;
			ConsignExecutorWithCondition(proc, out users);

			proc = WfRuntime.GetProcessByProcessID(processID);
			WfWithdrawExecutor withdrawExec = new WfWithdrawExecutor(proc.CurrentActivity, proc.CurrentActivity);
			withdrawExec.Execute();

			proc = WfRuntime.GetProcessByProcessID(processID);
			Assert.AreEqual(1, proc.ElapsedActivities.Count);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("加签活动")]
		public void AddApproverExecutorTest()
		{
			//Initial  NormalActivity  NormalActivity1 NormalActivity2 NormalActivity3 Completed
			IUser user = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object;

			IWfProcessDescriptor procDesp = WfProcessTestCommon.CreateProcessDescriptor();
			WfUserResourceDescriptor userDesp = new WfUserResourceDescriptor(user);
			procDesp.InitialActivity.Resources.Add(userDesp);

			WfProcessStartupParams startParams = WfProcessTestCommon.GetInstanceOfWfProcessStartupParams(procDesp);
			WfStartWorkflowExecutor exec = new WfStartWorkflowExecutor(null, startParams);
			IWfProcess proc = exec.Execute(); //启动流程

			IWfProcess wfp = WfRuntime.GetProcessByProcessID(proc.ID);
			MoveToExecutor(wfp);//流转一步 ,即第2个节点

			IWfProcess p = WfRuntime.GetProcessByProcessID(proc.ID);
			WfAssigneeCollection assignees = new WfAssigneeCollection();
			assignees.Add((IUser)OguObjectSettings.GetConfig().Objects[OguObject.approver1.ToString()].Object);

			WfAddApproverExecutor addExec = new WfAddApproverExecutor(p.CurrentActivity, p.CurrentActivity, assignees);
			addExec.Execute(); //加签,即流转到加签点上

			IWfProcess process = WfRuntime.GetProcessByProcessID(p.ID);

			Assert.AreEqual(process.Descriptor.GetMainStreamActivities().Count, process.Activities.Count - 2, "动态添加两个活动,因被加签人要回到加签人");
			Assert.AreEqual(2, process.ElapsedActivities.Count);
			Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(process.CurrentActivity.ID, TaskStatus.Ban));
			Assert.IsTrue(process.CurrentActivity.Descriptor.ToTransitions.Count == 1);
			Assert.IsNotNull(process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity);

			IWfProcess wfProcess9 = WfRuntime.GetProcessByProcessID(process.ID);

			WfWithdrawExecutor withdrawExec = new WfWithdrawExecutor(wfProcess9.CurrentActivity, wfProcess9.CurrentActivity);
			withdrawExec.Execute();

			IWfProcess wfProcess1 = WfRuntime.GetProcessByProcessID(wfProcess9.ID);
			Assert.AreEqual(wfProcess1.Activities.Count, wfProcess1.Descriptor.GetMainStreamActivities().Count, "此处应该撤回到加签的活动点上,同时被加签的两个点都应该移除");
			Assert.AreEqual(1, wfProcess1.ElapsedActivities.Count);
			Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(wfProcess1.CurrentActivity.ID, TaskStatus.Ban));
			Assert.IsTrue(wfProcess1.CurrentActivity.Descriptor.ToTransitions.Count == 1);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("退件活动")]
		public void ReturnExecutorTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();
			ProcessTestHelper.MoveToAndSaveNextActivityWithDefSelectTranstion(OguObject.approver1, process);

			process = WfRuntime.GetProcessByProcessID(process.ID);
			ProcessTestHelper.MoveToAndSaveNextActivityWithDefSelectTranstion(OguObject.approver2, process);

			process = WfRuntime.GetProcessByProcessID(process.ID);
			ProcessTestHelper.MoveToAndSaveNextActivityWithDefSelectTranstion(OguObject.requestor, process);

			process = WfRuntime.GetProcessByProcessID(process.ID);
			WfReturnExecutor returnExec = new WfReturnExecutor(process.CurrentActivity, process.Activities[1]);
			returnExec.Execute();

			process = WfRuntime.GetProcessByProcessID(process.ID);
			Assert.IsTrue(process.CurrentActivity.CreatorInstanceID != "");
			Assert.AreEqual(process.ElapsedActivities[process.ElapsedActivities.Count - 1].ID, process.CurrentActivity.CreatorInstanceID);

			WfMainStreamActivityDescriptorCollection coll = process.Descriptor.GetMainStreamActivities();
			Assert.AreEqual(6, coll.Count);
			Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(process.CurrentActivity.ID, TaskStatus.Ban));
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("退件活动,退件到起草人")]
		public void ReturnExecutorTestReplay()
		{
			IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();
			ProcessTestHelper.MoveToAndSaveNextActivityWithDefSelectTranstion(OguObject.approver1, process);

			process = WfRuntime.GetProcessByProcessID(process.ID);
			ProcessTestHelper.MoveToAndSaveNextActivityWithDefSelectTranstion(OguObject.approver2, process);

			//退件到初始活动
			process = WfRuntime.GetProcessByProcessID(process.ID);
			WfReturnExecutor returnExec = new WfReturnExecutor(process.CurrentActivity, process.Activities[0]);
			returnExec.Execute();

			Assert.IsTrue(process.CurrentActivity.CreatorInstanceID != "");
			Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(process.CurrentActivity.ID, TaskStatus.Ban));

			process = WfRuntime.GetProcessByProcessID(process.ID);
			IWfActivity act = process.Activities.FindActivityByDescriptorKey(process.CurrentActivity.Descriptor.AssociatedActivityKey);
			Assert.AreEqual(act.Assignees.Count, process.CurrentActivity.Assignees.Count);

			Assert.AreEqual(process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity.AssociatedActivityKey, act.Descriptor.ToTransitions[0].ToActivity.Key);
			Assert.IsTrue(process.Activities.FindActivityByDescriptorKey(process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity.Key).Assignees.Count > 0);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("替换待办人员")]
		public void ReplaceAssigneeExecutorTestReplay()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

			IWfActivityDescriptor normalActDesp = processDesp.Activities["NormalActivity"];

			IUser originalApprover = (IUser)OguObjectSettings.GetConfig().Objects[OguObject.approver1.ToString()].Object;

			normalActDesp.Resources.Clear();
			normalActDesp.Resources.Add(new WfUserResourceDescriptor(originalApprover));

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			WfProcessTestCommon.MoveToNextDefaultActivityWithExecutor(process);

			process = WfRuntime.GetProcessByProcessID(process.ID);

			IUser newApprover = (IUser)OguObjectSettings.GetConfig().Objects[OguObject.approver2.ToString()].Object;

			WfReplaceAssigneesExecutor executor = new WfReplaceAssigneesExecutor(process.CurrentActivity, process.CurrentActivity, originalApprover, new IUser[] { newApprover });

			executor.Execute();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			UserTaskCollection tasks = UserTaskAdapter.Instance.LoadUserTasks(build => build.AppendItem("ACTIVITY_ID", process.CurrentActivity.ID));

			Assert.AreEqual(1, tasks.Count);
			Assert.AreEqual(newApprover.ID, tasks[0].SendToUserID);
		}

		#region Executor with MainStream
		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("添加加签环节基本模式Executor的测试")]
		public void BaseAddApproverExecutorStandardModeTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			int originalActCount = process.Activities.Count;

			WfProcessTestCommon.MoveToNextDefaultActivity(process);

			IWfActivity targetActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");
			WfAddApproverExecutor executor = new WfAddApproverExecutor(process.CurrentActivity, targetActivity);

			executor.Assignees.Add((IUser)OguObjectSettings.GetConfig().Objects[OguObject.approver1.ToString()].Object);
			executor.Execute();

			Assert.AreEqual(originalActCount + 2, process.Activities.Count);
			Assert.AreEqual(originalActCount, process.MainStream.Activities.Count);

			WfMainStreamActivityDescriptorCollection processDespMSA = process.Descriptor.GetMainStreamActivities();
			WfMainStreamActivityDescriptorCollection processMSA = process.GetMainStreamActivities(true);

			processDespMSA.Output("流程描述中的主线活动");
			processMSA.Output("主线流程中的主线活动");
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("添加加签环节基本模式然后再撤回Executor的测试")]
		public void BaseAddApproverExecutorStandardModeWithWithdrawTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			int originalActCount = process.Activities.Count;

			WfProcessTestCommon.MoveToNextDefaultActivity(process);

			int originalElapsedActCount = process.ElapsedActivities.Count;

			IWfActivity targetActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");
			WfAddApproverExecutor executor = new WfAddApproverExecutor(process.CurrentActivity, targetActivity);

			executor.Assignees.Add((IUser)OguObjectSettings.GetConfig().Objects[OguObject.approver1.ToString()].Object);
			executor.Execute();

			Console.WriteLine("Elapsed activities: {0}", process.ElapsedActivities.Count);

			process = WfRuntime.GetProcessByProcessID(process.ID);
			WfWithdrawExecutor withdrawExecutor = new WfWithdrawExecutor(process.CurrentActivity, process.CurrentActivity);

			withdrawExecutor.Execute();

			Console.WriteLine("Elapsed activities: {0}", process.ElapsedActivities.Count);

			Assert.AreEqual(originalElapsedActCount, process.ElapsedActivities.Count);
			Assert.AreEqual(originalActCount, process.Activities.Count, "撤回后，恢复为加签之前的状态");
			Assert.AreEqual(originalActCount, process.MainStream.Activities.Count, "撤回后，主线流程的活动也没有变化");

			WfMainStreamActivityDescriptorCollection processDespMSA = process.Descriptor.GetMainStreamActivities();
			WfMainStreamActivityDescriptorCollection processMSA = process.GetMainStreamActivities(true);

			processDespMSA.Output("流程描述中的主线活动");
			processMSA.Output("主线流程中的主线活动");
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("添加加签环节仅审批环节模式Executor的测试")]
		public void BaseAddApproverExecutorOnlyAddApproverModeTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			int originalActCount = process.Activities.Count;

			WfProcessTestCommon.MoveToNextDefaultActivity(process);

			IWfActivity targetActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");
			WfAddApproverExecutor executor = new WfAddApproverExecutor(process.CurrentActivity, targetActivity);

			executor.AddApproverMode = WfAddApproverMode.OnlyAddApprover;

			executor.Assignees.Add((IUser)OguObjectSettings.GetConfig().Objects[OguObject.approver1.ToString()].Object);
			executor.Execute();

			Assert.AreEqual(originalActCount + 1, process.Activities.Count);
			Assert.AreEqual(originalActCount + 1, process.MainStream.Activities.Count);

			WfMainStreamActivityDescriptorCollection processDespMSA = process.Descriptor.GetMainStreamActivities();
			WfMainStreamActivityDescriptorCollection processMSA = process.GetMainStreamActivities(true);

			processDespMSA.Output("流程描述中的主线活动");
			processMSA.Output("主线流程中的主线活动");
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("添加环节Executor的基本测试")]
		public void BasicAddActivityExecutorTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			int originalActCount = process.Activities.Count;

			IWfActivity targetActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			WfActivityDescriptorCreateParams createParams = new WfActivityDescriptorCreateParams();

			WfAddActivityExecutor executor = new WfAddActivityExecutor(process.CurrentActivity, targetActivity, createParams);

			executor.Execute();

			Assert.IsNotNull(executor.AddedActivity.GetMainStreamActivityDescriptor());
			Assert.AreEqual(originalActCount + 1, process.Activities.Count);

			WfMainStreamActivityDescriptorCollection processDespMSA = process.Descriptor.GetMainStreamActivities();
			WfMainStreamActivityDescriptorCollection processMSA = process.GetMainStreamActivities(true);

			processDespMSA.Output("流程描述中的主线活动");
			processMSA.Output("主线流程中的主线活动");

			Assert.AreEqual(process.Descriptor.Activities.Count, process.MainStream.Activities.Count);
			Assert.AreEqual(processDespMSA.Count, processMSA.Count);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("在有条件分支线的活动之前添加活动测试，重点测试条件分支线的位置")]
		public void AddActivityBeforeConditionActivityExecutorTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcessDescriptorWithTransitionCondition();

			IWfActivity targetActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			List<IWfTransitionDescriptor> originalTransitions = new List<IWfTransitionDescriptor>();

			targetActivity.Descriptor.ToTransitions.CopyTo(originalTransitions);

			WfActivityDescriptorCreateParams createParams = new WfActivityDescriptorCreateParams();

			WfAddActivityExecutor executor = new WfAddActivityExecutor(process.CurrentActivity, targetActivity, createParams);

			executor.Execute();

			Console.WriteLine("Out transitions: {0}", originalTransitions.Count);
			Assert.AreEqual(originalTransitions.Count, executor.AddedActivity.Descriptor.ToTransitions.Count);
			Assert.AreEqual(1, targetActivity.Descriptor.ToTransitions.Count);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("添加会签环节Executor的基本测试")]
		public void AddActivityWithConsignExecutorTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			int originalActCount = process.Activities.Count;

			IWfActivity targetActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			WfActivityDescriptorCreateParams createParams = new WfActivityDescriptorCreateParams();

			createParams.AllAgreeWhenConsign = true;
			createParams.Users = new OguDataCollection<IUser>();
			createParams.Users.Add((IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object);
			createParams.Users.Add((IUser)OguObjectSettings.GetConfig().Objects["approver2"].Object);

			WfAddActivityExecutor executor = new WfAddActivityExecutor(process.CurrentActivity, targetActivity, createParams);

			executor.Execute();

			Assert.AreEqual(originalActCount + 1, process.Activities.Count);

			WfMainStreamActivityDescriptorCollection processDespMSA = process.Descriptor.GetMainStreamActivities();
			WfMainStreamActivityDescriptorCollection processMSA = process.GetMainStreamActivities(true);

			processDespMSA.Output("流程描述中的主线活动");
			processMSA.Output("主线流程中的主线活动");

			Assert.AreEqual(1, executor.AddedActivity.Descriptor.BranchProcessTemplates.Count);
			Assert.AreEqual(1, executor.AddedActivity.GetMainStreamActivityDescriptor().BranchProcessTemplates.Count);

			Assert.AreEqual(process.Descriptor.Activities.Count, process.MainStream.Activities.Count);
			Assert.AreEqual(processDespMSA.Count, processMSA.Count);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("添加带两根出线的环节Executor的测试")]
		public void AddActivityWithTwoOutTransitionsExecutorTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			int originalActCount = process.Activities.Count;

			IWfActivity targetActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			targetActivity.Descriptor.ToTransitions.FirstOrDefault().Properties.SetValue("Name", "同意");

			//增加一条出线
			IWfTransitionDescriptor tReturn = targetActivity.Descriptor.ToTransitions.AddBackwardTransition(process.InitialActivity.Descriptor);

			tReturn.Properties.SetValue("Name", "不同意");

			WfActivityDescriptorCreateParams createParams = new WfActivityDescriptorCreateParams();

			createParams.AllAgreeWhenConsign = false;
			createParams.Users = new OguDataCollection<IUser>();
			createParams.Users.Add((IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object);

			WfAddActivityExecutor executor = new WfAddActivityExecutor(process.CurrentActivity, targetActivity, createParams);

			executor.Execute();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			IWfActivity addedActivity = process.Activities[executor.AddedActivity.ID];
			targetActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			Assert.AreEqual(originalActCount + 1, process.Activities.Count);
			Assert.AreEqual(1, targetActivity.Descriptor.ToTransitions.Count);
			Assert.IsFalse(targetActivity.Descriptor.ToTransitions[0].IsBackward);

			Assert.AreEqual(2, addedActivity.Descriptor.ToTransitions.Count, "新加的活动应该也有两条出线");
			Assert.AreEqual("同意", addedActivity.Descriptor.ToTransitions[0].Name);
			Assert.AreEqual("不同意", addedActivity.Descriptor.ToTransitions[1].Name);

			WfMainStreamActivityDescriptorCollection processDespMSA = process.Descriptor.GetMainStreamActivities();
			WfMainStreamActivityDescriptorCollection processMSA = process.GetMainStreamActivities(true);

			processDespMSA.Output("流程描述中的主线活动");
			processMSA.Output("主线流程中的主线活动");

			Assert.AreEqual(process.Descriptor.Activities.Count, process.MainStream.Activities.Count);
			Assert.AreEqual(processDespMSA.Count, processMSA.Count);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("删除环节Executor的基本测试")]
		public void BasicDeleteActivityExecutorTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			int originalActCount = process.Activities.Count;

			IWfActivity targetActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			WfActivityDescriptorCreateParams createParams = new WfActivityDescriptorCreateParams();

			WfAddActivityExecutor addExecutor = new WfAddActivityExecutor(process.CurrentActivity, targetActivity, createParams);

			addExecutor.Execute();

			IWfActivityDescriptor newActDesp = targetActivity.Descriptor.ToTransitions.FirstOrDefault().ToActivity;

			WfRuntime.ClearCache();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			targetActivity = process.Activities.FindActivityByDescriptorKey(newActDesp.Key);
			WfDeleteActivityExecutor deleteExecutor = new WfDeleteActivityExecutor(process.CurrentActivity, targetActivity);

			deleteExecutor.Execute();

			WfMainStreamActivityDescriptorCollection processDespMSA = process.Descriptor.GetMainStreamActivities();
			WfMainStreamActivityDescriptorCollection processMSA = process.GetMainStreamActivities(true);

			processDespMSA.Output("流程描述中的主线活动");
			processMSA.Output("主线流程中的主线活动");

			Assert.AreEqual(process.Descriptor.Activities.Count, process.MainStream.Activities.Count);
			Assert.AreEqual(processDespMSA.Count, processMSA.Count);

			Assert.IsNull(process.Activities.FindActivityByDescriptorKey(targetActivity.Descriptor.Key));
			Assert.IsNull(process.MainStream.Activities[targetActivity.MainStreamActivityKey]);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("编辑环节Executor的基本测试")]
		public void BasicEditActivityExecutorTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			IWfActivity targetActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			WfActivityDescriptorCreateParams createParams = new WfActivityDescriptorCreateParams();

			WfVariableDescriptor variable = new WfVariableDescriptor("CodeName", "HelloKitty");

			createParams.Variables = new WfVariableDescriptor[] { variable };

			WfEditActivityExecutor editExecutor = new WfEditActivityExecutor(process.CurrentActivity, targetActivity, createParams);

			editExecutor.Execute();

			Assert.AreEqual("HelloKitty",
				process.Descriptor.Activities["NormalActivity"].Properties.GetValue("CodeName", string.Empty));
			Assert.AreEqual("HelloKitty",
				process.MainStream.Activities["NormalActivity"].Properties.GetValue("CodeName", string.Empty));
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("编辑环节Executor的基本测试")]
		public void RestoreProcessExecutorTest()
		{
			IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

			MoveToExecutor(process);

			process = WfRuntime.GetProcessByProcessID(process.ID);

			WfExecutorBase executor = new WfCancelProcessExecutor(process.CurrentActivity, process);

			executor.Execute();

			Assert.AreEqual(WfProcessStatus.Aborted, process.Status);

			process = WfRuntime.GetProcessByProcessID(process.ID);

			executor = new WfRestoreProcessExecutor(process.CurrentActivity, process);

			executor.Execute();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			Assert.AreEqual(WfProcessStatus.Running, process.Status);
		}
		#endregion Executor with MainStream

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("启动分支流程的Executor测试")]
		public void StartBranchProcessExecutorTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithBranchTemplate();

			IWfActivityDescriptor normalActivityDesp = processDesp.Activities["NormalActivity"];

			//不自动启动分支流程
			normalActivityDesp.Properties.SetValue("AutoStartBranchProcesses", false);

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			//前进一步，但是没有启动流程
			MoveToExecutor(process);

			process = WfRuntime.GetProcessByProcessID(process.ID);

			IWfActivity normalActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			Assert.AreEqual(0, normalActivity.BranchProcessGroups.Count);

			WfProcessCollection processes = new WfProcessCollection();

			for (int i = 0; i < normalActivity.Descriptor.BranchProcessTemplates.Count; i++)
			{
				process = WfRuntime.GetProcessByProcessID(process.ID);
				normalActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

				WfStartBranchProcessExecutor executor = new WfStartBranchProcessExecutor(normalActivity, normalActivity, normalActivity.Descriptor.BranchProcessTemplates[i]);

				executor.Execute();

				processes.CopyFrom(executor.StartedProcesses);
			}

			process = WfRuntime.GetProcessByProcessID(process.ID);
			normalActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			Console.WriteLine(processes.Count);

			Assert.AreEqual(2, normalActivity.BranchProcessGroups.Count);

			//Assert.AreEqual(1, normalActivity.BranchProcessGroups[0].BranchProcessStatistics.Total);
			//Assert.AreEqual(0, normalActivity.BranchProcessGroups[0].BranchProcessStatistics.Completed);
			//Assert.AreEqual(0, normalActivity.BranchProcessGroups[0].BranchProcessStatistics.Aborted);
			Assert.IsTrue(normalActivity.BranchProcessGroups[0].IsBlocking());

			//Assert.AreEqual(1, normalActivity.BranchProcessGroups[1].BranchProcessStatistics.Total);
			//Assert.AreEqual(0, normalActivity.BranchProcessGroups[1].BranchProcessStatistics.Completed);
			//Assert.AreEqual(0, normalActivity.BranchProcessGroups[1].BranchProcessStatistics.Aborted);
			Assert.IsTrue(normalActivity.BranchProcessGroups[1].IsBlocking());

			Assert.AreEqual(1, normalActivity.BranchProcessGroups[0].Branches.Count);
			Assert.AreEqual(1, normalActivity.BranchProcessGroups[1].Branches.Count);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("启动分支流程的Executor，执行后解锁的测试")]
		public void StartBranchProcessAndUnlockExecutorTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithBranchTemplate();

			IWfActivityDescriptor normalActivityDesp = processDesp.Activities["NormalActivity"];

			//不自动启动分支流程
			normalActivityDesp.Properties.SetValue("AutoStartBranchProcesses", false);

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			//前进一步，但是没有启动流程
			MoveToExecutor(process);

			process = WfRuntime.GetProcessByProcessID(process.ID);

			IWfActivity normalActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			Assert.AreEqual(0, normalActivity.BranchProcessGroups.Count);

			process = WfRuntime.GetProcessByProcessID(process.ID);
			normalActivity = process.Activities.FindActivityByDescriptorKey("NormalActivity");

			WfStartBranchProcessExecutor executor = new WfStartBranchProcessExecutor(normalActivity, normalActivity, normalActivity.Descriptor.BranchProcessTemplates[0]);

			//启动一组分支流程
			executor.Execute();

			//流转每一条子流程（办结）
			foreach (IWfProcess subProcess in executor.StartedProcesses)
			{
				IWfProcess subProcessInstance = WfRuntime.GetProcessByProcessID(subProcess.ID);

				MoveToExecutor(subProcessInstance);
			}

			process = WfRuntime.GetProcessByProcessID(process.ID);

			Console.WriteLine(process.CurrentActivity.Descriptor.Key);

			Assert.IsFalse(process.CurrentActivity.BranchProcessGroups[0].IsBlocking());
		}
	}
}
