using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects.Workflow.Tasks;
using DO = MCS.Library.SOA.DataObjects;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Test.Executor
{
	/// <summary>
	/// 流程流转时进入到维护状态的单元测试
	/// </summary>
	[TestClass]
	public class MaintainingStatusTest
	{
		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("进入到维护模式的测试")]
		public void EnterMaintainingStatusTest()
		{
			IWfProcessDescriptor processDesp = CreateProcessWithAutoMaintainProperty();

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			MoveToExecutor(process);

			process = WfRuntime.GetProcessByProcessID(process.ID);

			Assert.AreEqual(WfProcessStatus.Maintaining, process.Status);
			Assert.AreEqual(WfActivityStatus.Pending, process.CurrentActivity.Status);

			Assert.AreEqual(0, process.CurrentActivity.BranchProcessGroups.Count);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("进入到维护模式的测试")]
		public void ExitMaintainingStatusTest()
		{
			IWfProcessDescriptor processDesp = CreateProcessWithAutoMaintainProperty();

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			MoveToExecutor(process);

			process = WfRuntime.GetProcessByProcessID(process.ID);

			process.ExitMaintainingStatus(true);

			Console.WriteLine(process.CurrentActivity.Descriptor.Key);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("进入到维护模式后启动分支流程任务的测试")]
		public void StartBranchProcessTaskTest()
		{
			IWfProcessDescriptor processDesp = CreateProcessWithAutoMaintainProperty();

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			MoveToExecutor(process);

			process = WfRuntime.GetProcessByProcessID(process.ID);
			IWfActivity ownerActivity = process.CurrentActivity;

			Console.WriteLine("OwnerActivityID: {0}", ownerActivity.ID);

			WfBranchProcessTransferParams transferParams = new WfBranchProcessTransferParams(ownerActivity.Descriptor.BranchProcessTemplates[0]);

			ExecuteAndAssertTask(() => StartBranchProcessTask.SendTask(ownerActivity.ID, transferParams));

			WfRuntime.ClearCache();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			Assert.IsTrue(process.CurrentActivity.BranchProcessGroups.Count > 0);
			Assert.IsTrue(process.CurrentActivity.BranchProcessGroups[0].Branches.Count > 0);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("进入到维护模式后执行退出维护模式任务的测试")]
		public void ExitMaintainingStatusTaskTest()
		{
			IWfProcessDescriptor processDesp = CreateProcessWithAutoMaintainProperty();

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			MoveToExecutor(process);

			process = WfRuntime.GetProcessByProcessID(process.ID);
			IWfActivity ownerActivity = process.CurrentActivity;

			WfBranchProcessTransferParams transferParams = new WfBranchProcessTransferParams(ownerActivity.Descriptor.BranchProcessTemplates[0]);

			Console.WriteLine("Process ID: {0}", process.ID);

			ExecuteAndAssertTask(() => ExitMaintainingStatusTask.SendTask(ownerActivity.ID, process.ID, true));

			WfRuntime.ClearCache();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			Assert.AreEqual(WfProcessStatus.Completed, process.Status);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("进入到维护模式后执行启动分支流程任务的测试")]
		public void DispatchStartBranchProcessTaskTest()
		{
			IWfProcessDescriptor processDesp = CreateProcessWithAutoMaintainProperty();

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			MoveToExecutor(process);

			process = WfRuntime.GetProcessByProcessID(process.ID);
			IWfActivity ownerActivity = process.CurrentActivity;

			Console.WriteLine("OwnerActivityID: {0}", ownerActivity.ID);

			//在Template设置资源为一个组织，用于后续分发分支流程的测试
			IWfBranchProcessTemplateDescriptor template = ownerActivity.Descriptor.BranchProcessTemplates[0];

			IUser userInTemplate = template.Resources.ToUsers().FirstOrDefault();

			WfDepartmentResourceDescriptor deptResourceDesp = new WfDepartmentResourceDescriptor(userInTemplate.Parent);

			template.Resources.Clear();
			template.Resources.Add(deptResourceDesp);

			SysTaskAdapter.Instance.ClearAll();

			SysTaskCommon.ExecuteAndAssertTask(DispatchStartBranchProcessTask.SendTask(ownerActivity.ID, template, true));

			WfRuntime.ClearCache();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			int startedTaskCount = (int)WfServiceInvoker.InvokeContext["returnValue"];

			Console.WriteLine("Started Branch Task Count: {0}", startedTaskCount);

			Assert.IsTrue(startedTaskCount > 0);

			int executedTaskCount = SysTaskCommon.ExecuteAllTasks();

			Assert.AreEqual(startedTaskCount, executedTaskCount);

			WfRuntime.ClearCache();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			Assert.AreEqual(WfProcessStatus.Running, process.Status);
			Assert.IsTrue(process.CurrentActivity.BranchProcessGroups.Count > 0);

			Assert.AreEqual(startedTaskCount - 1, process.CurrentActivity.BranchProcessGroups[0].Branches.Count);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("进入到维护模式后执行创建分支流程任务流程的测试")]
		public void BuildStartBranchProcessTaskProcessTest()
		{
			IWfProcessDescriptor processDesp = CreateProcessWithAutoMaintainProperty();

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			MoveToExecutor(process);

			process = WfRuntime.GetProcessByProcessID(process.ID);
			IWfActivity ownerActivity = process.CurrentActivity;

			Console.WriteLine("OwnerActivityID: {0}", ownerActivity.ID);

			//在Template设置资源为一个组织，用于后续分发分支流程的测试
			IWfBranchProcessTemplateDescriptor template = ownerActivity.Descriptor.BranchProcessTemplates[0];

			IUser userInTemplate = template.Resources.ToUsers().FirstOrDefault();

			WfDepartmentResourceDescriptor deptResourceDesp = new WfDepartmentResourceDescriptor(userInTemplate.Parent);

			template.Resources.Clear();
			template.Resources.Add(deptResourceDesp);

			SysTaskAdapter.Instance.ClearAll();

			StartBranchProcessSysTaskProcessBuilder builder = new StartBranchProcessSysTaskProcessBuilder(ownerActivity.ID, template, true);

			SysTaskProcess sysTaskProcess = builder.Build();

			SysTaskProcessRuntime.ClearCache();

			sysTaskProcess = SysTaskProcessRuntime.GetProcessByID(sysTaskProcess.ID);

			Console.WriteLine("SysTaskProcess ID: {0}", sysTaskProcess.ID);
			Console.WriteLine("SysTaskProcess Activities: {0}", sysTaskProcess.Activities.Count);

			Assert.AreEqual(template.Resources.ToUsers().Count + 1, sysTaskProcess.Activities.Count);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("作废流程任务的测试")]
		public void DispatchCancelProcessTaskTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithBranchTemplate();

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			MoveToExecutor(process);

			SysTaskAdapter.Instance.ClearAll();

			DispatchCancelProcessTask cancelTask = DispatchCancelProcessTask.SendTask(string.Empty, process.ID, true);

			//直接发布一个作废的任务流程
			SysTaskCommon.ExecuteAndAssertTask(cancelTask);

			int executedTaskCount = SysTaskCommon.ExecuteAllTasks();

			WfRuntime.ClearCache();
			process = WfRuntime.GetProcessByProcessID(process.ID);
			AssertProcessAndAllBranchesStatus(WfProcessStatus.Aborted, process);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("作废无分支流程任务的测试")]
		public void DispatchCancelNoBranchProcessTaskTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			MoveToExecutor(process);

			SysTaskAdapter.Instance.ClearAll();

			DispatchCancelProcessTask cancelTask = DispatchCancelProcessTask.SendTask(string.Empty, process.ID, true);

			//直接发布一个作废的任务流程
			SysTaskCommon.ExecuteAndAssertTask(cancelTask);

			int executedTaskCount = SysTaskCommon.ExecuteAllTasks();

			WfRuntime.ClearCache();
			process = WfRuntime.GetProcessByProcessID(process.ID);
			AssertProcessAndAllBranchesStatus(WfProcessStatus.Aborted, process);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("撤回流程任务的测试")]
		public void DispatchWithdrawProcessTaskTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithBranchTemplate();

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			MoveToExecutor(process);

			SysTaskAdapter.Instance.ClearAll();

			DispatchWithdrawProcessTask withdrawTask = DispatchWithdrawProcessTask.SendTask(string.Empty, process.ID, true);

			//直接发布一个撤回的任务流程
			SysTaskCommon.ExecuteAndAssertTask(withdrawTask);

			int executedTaskCount = SysTaskCommon.ExecuteAllTasks();

			WfRuntime.ClearCache();
			process = WfRuntime.GetProcessByProcessID(process.ID);

			Assert.AreEqual(WfProcessStatus.Running, process.Status);
			Assert.AreEqual(WfActivityType.InitialActivity, process.CurrentActivity.Descriptor.ActivityType);
			
			AssertAllBranchesStatus(WfProcessStatus.Aborted, process);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("启动分支流程后，创建作废分支流程的任务流程的测试")]
		public void BuildCancelProcessTaskProcessTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithBranchTemplate();

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			MoveToExecutor(process);

			process = WfRuntime.GetProcessByProcessID(process.ID);

			IWfActivity ownerActivity = process.CurrentActivity;

			process.EntrtMaintainingStatus();

			WfRuntime.PersistWorkflows();

			DispatchCancelProcessSysTaskProcessBuilder builder = new DispatchCancelProcessSysTaskProcessBuilder(process.ID, true);

			SysTaskProcess sysTaskProcess = builder.Build();

			Console.WriteLine(sysTaskProcess.Activities.Count);

			Assert.AreEqual(3, sysTaskProcess.Activities.Count);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("作废流程任务的测试")]
		public void CancelProcessTaskTest()
		{
			IWfProcessDescriptor processDesp = CreateProcessWithAutoMaintainProperty();

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			MoveToExecutor(process);

			ExecuteAndAssertTask(() => CancelProcessTask.SendTask(process.ID, false));

			WfRuntime.ClearCache();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			Assert.AreEqual(WfProcessStatus.Aborted, process.Status);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Executor)]
		[Description("撤回流程任务的测试")]
		public void WithdrawProcessTaskTest()
		{
			IWfProcessDescriptor processDesp = CreateProcessWithAutoMaintainProperty();

			IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

			MoveToExecutor(process);

			ExecuteAndAssertTask(() => WithdrawProcessTask.SendTask(process.ID, false));

			WfRuntime.ClearCache();

			process = WfRuntime.GetProcessByProcessID(process.ID);

			Assert.AreEqual(WfProcessStatus.Running, process.Status);

			Assert.AreEqual(WfActivityType.InitialActivity, process.CurrentActivity.Descriptor.ActivityType);
			Assert.AreEqual(WfActivityStatus.Running, process.CurrentActivity.Status);
		}

		private static void ExecuteTasks(IEnumerable<DO.SysTask> tasks)
		{
			foreach (DO.SysTask task in tasks)
			{
				ISysTaskExecutor executor = SysTaskSettings.GetSettings().GetExecutor(task.TaskType);

				executor.Execute(task);
			}
		}

		/// <summary>
		/// 执行并且验证Task的返回结果
		/// </summary>
		/// <param name="func"></param>
		private static void ExecuteAndAssertTask(Func<DO.SysTask> func)
		{
			func.NullCheck("func");

			DO.SysTask task = func();

			DO.SysTask taskLoaded = SysTaskAdapter.Instance.Load(task.TaskID);

			ISysTaskExecutor executor = SysTaskSettings.GetSettings().GetExecutor(taskLoaded.TaskType);

			executor.Execute(taskLoaded);

			SysAccomplishedTask accomplishedTask = SysAccomplishedTaskAdapter.Instance.Load(taskLoaded.TaskID);

			Assert.IsNotNull(accomplishedTask);

			Console.WriteLine(accomplishedTask.StatusText);
			Assert.AreEqual(SysTaskStatus.Completed, accomplishedTask.Status);
		}

		/// <summary>
		/// 构造设置了自动进入到维护状态的流程
		/// </summary>
		/// <returns></returns>
		private static IWfProcessDescriptor CreateProcessWithAutoMaintainProperty()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithBranchTemplate();

			processDesp.Activities["NormalActivity"].Properties.SetValue("AutoMaintain", true);

			return processDesp;
		}

		private static void MoveToExecutor(IWfProcess process)
		{
			IWfActivityDescriptor nextActivityDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
			WfTransferParams pa = ProcessTestHelper.GetInstanceOfWfTransferParams(nextActivityDesp, OguObject.approver1);
			WfMoveToExecutor moveExec = new WfMoveToExecutor(process.CurrentActivity, process.CurrentActivity, pa);

			moveExec.Execute();
		}

		private static void AssertProcessAndAllBranchesStatus(WfProcessStatus expectedStatus, IWfProcess process)
		{
			Assert.AreEqual(expectedStatus, process.Status);

			process.CurrentActivity.BranchProcessGroups.ForEach(grpup =>
				{
					grpup.Branches.ForEach(p => AssertProcessAndAllBranchesStatus(expectedStatus, p));
				}
			);
		}

		private static void AssertAllBranchesStatus(WfProcessStatus expectedStatus, IWfProcess process)
		{
			process.CurrentActivity.BranchProcessGroups.ForEach(grpup =>
			{
				grpup.Branches.ForEach(p => AssertProcessAndAllBranchesStatus(expectedStatus, p));
			}
			);
		}
	}
}
