using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DO = MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.DataObjects.Test.SysTaskProcesses
{
	[TestClass]
	public class SysTaskProcessTest
	{
		[TestMethod]
		[TestCategory("SysTaskProcess")]
		[Description("创建任务流程和持久化")]
		public void CreateSysTaskProcessAndPersist()
		{
			SysTaskAdapter.Instance.ClearAll();

			SysTaskProcess process = PrepareSysTaskProcessData(() => PrepareInvokeServiceTask());

			SysTaskProcessRuntime.ProcessContext.AffectedProcesses.AddOrReplace(process);
			SysTaskProcessRuntime.Persist();

			SysTaskProcess loadedProcess = SysTaskProcessRuntime.GetProcessByID(process.ID);

			Assert.AreNotEqual(process, loadedProcess);
			Assert.AreEqual(process.ID, loadedProcess.ID);
			Assert.AreEqual(process.Activities.Count, loadedProcess.Activities.Count);

			Assert.AreEqual(process.Activities[0].Sequence, loadedProcess.Activities[0].Sequence);
			Assert.AreEqual(process.Activities[1].Sequence, loadedProcess.Activities[1].Sequence);

			Assert.AreEqual(process.Activities[0].Task.TaskID, loadedProcess.Activities[0].Task.TaskID);
		}

		[TestMethod]
		[TestCategory("SysTaskProcess")]
		[Description("启动任务流程的测试")]
		public void StartSysTaskProcessTest()
		{
			SysTaskAdapter.Instance.ClearAll();

			SysTaskProcess process = PrepareSysTaskProcessData(() => PrepareInvokeServiceTask());

			SysTaskProcessRuntime.StartProcess(process);

			DO.SysTask moveToTask = SysTaskAdapter.Instance.LoadByResourceID(process.Activities[0].ID).FirstOrDefault();

			Assert.IsNotNull(moveToTask);

			SysTaskCommon.ExecuteAndAssertTask(moveToTask);

			SysTaskProcessRuntime.ClearCache();

			process = SysTaskProcessRuntime.GetProcessByID(process.ID);

			Assert.AreEqual(1, process.CurrentActivityIndex);
		}

		[TestMethod]
		[TestCategory("SysTaskProcess")]
		[Description("启动任务流程然后执行到底的测试")]
		public void StartSysTaskProcessAndMoveToCompleteTest()
		{
			SysTaskAdapter.Instance.ClearAll();

			SysTaskProcess process = PrepareSysTaskProcessData(() => PrepareInvokeServiceTask());

			SysTaskProcessRuntime.StartProcess(process);

			SysTaskCommon.ExecuteAllTasks();

			SysTaskProcessRuntime.ClearCache();
			process = SysTaskProcessRuntime.GetProcessByID(process.ID);
			Assert.AreEqual(SysTaskProcessStatus.Completed, process.Status);
		}

		//
		[TestMethod]
		[TestCategory("SysTaskProcess")]
		[Description("启动带分支流程的任务流程然后执行到底的测试")]
		public void StartSysTaskProcessWithBranchesAndMoveToCompleteTest()
		{
			SysTaskAdapter.Instance.ClearAll();

			SysTaskProcess process = PrepareSysTaskProcessWithBranchData(() => PrepareInvokeServiceTask());

			SysTaskProcessRuntime.StartProcess(process);

			SysTaskCommon.ExecuteAllTasks();

			SysTaskProcessRuntime.ClearCache();
			process = SysTaskProcessRuntime.GetProcessByID(process.ID);
			Assert.AreEqual(SysTaskProcessStatus.Completed, process.Status);
		}

		[TestMethod]
		[TestCategory("SysTaskProcess")]
		[Description("启动带错误的任务流程测试，流程会终止")]
		public void StartSysTaskProcessWithErrorTest()
		{
			SysTaskAdapter.Instance.ClearAll();

			SysTaskProcess process = PrepareSysTaskProcessData(() => PrepareErrorInvokeServiceTask());

			SysTaskProcessRuntime.StartProcess(process);

			DO.SysTask moveToTask = SysTaskAdapter.Instance.LoadByResourceID(process.Activities[0].ID).FirstOrDefault();

			Assert.IsNotNull(moveToTask);

			SysTaskCommon.ExecuteTask(moveToTask);

			SysTaskProcessRuntime.ClearCache();

			process = SysTaskProcessRuntime.GetProcessByID(process.ID);

			Assert.AreEqual(0, process.CurrentActivityIndex);
			Assert.AreEqual(SysTaskProcessStatus.Aborted, process.Status);
			Assert.AreEqual(SysTaskActivityStatus.Aborted, process.CurrentActivity.Status);
		}

		private SysTaskProcess PrepareSysTaskProcessData(Func<DO.SysTask> getTask)
		{
			SysTaskProcess process = new SysTaskProcess();

			process.ID = UuidHelper.NewUuidString();
			process.Name = "测试任务流程名称";
			process.Status = SysTaskProcessStatus.Running;
			process.StartTime = DateTime.Now;

			SysTaskActivity activity1 = new SysTaskActivity(getTask());

			activity1.ID = UuidHelper.NewUuidString();
			activity1.Name = "活动1";

			process.Activities.Add(activity1);

			SysTaskActivity activity2 = new SysTaskActivity(getTask());

			activity2.ID = UuidHelper.NewUuidString();
			activity2.Name = "活动1";

			process.Activities.Add(activity2);

			return process;
		}

		/// <summary>
		/// 准备带子流程的任务流程
		/// </summary>
		/// <param name="getTask"></param>
		/// <returns></returns>
		private SysTaskProcess PrepareSysTaskProcessWithBranchData(Func<DO.SysTask> getTask)
		{
			SysTaskProcess process = PrepareSysTaskProcessData(getTask);

			process.Name = "测试带子流程的任务流程名称";

			SysTaskProcess branchA1 = PrepareSysTaskProcessData(getTask);
			branchA1.OwnerActivityID = process.Activities[0].ID;

			SysTaskProcess branchA2 = PrepareSysTaskProcessData(getTask);
			branchA2.OwnerActivityID = process.Activities[1].ID;

			SysTaskProcessRuntime.ProcessContext.AffectedProcesses.AddOrReplace(branchA1);
			SysTaskProcessRuntime.ProcessContext.AffectedProcesses.AddOrReplace(branchA2);

			SysTaskProcess branchB1 = PrepareSysTaskProcessData(getTask);
			branchB1.OwnerActivityID = process.Activities[1].ID;

			SysTaskProcess branchB2 = PrepareSysTaskProcessData(getTask);
			branchB2.OwnerActivityID = process.Activities[1].ID;

			SysTaskProcessRuntime.ProcessContext.AffectedProcesses.AddOrReplace(branchB1);
			SysTaskProcessRuntime.ProcessContext.AffectedProcesses.AddOrReplace(branchB2);

			return process;
		}

		private static InvokeServiceTask PrepareInvokeServiceTask()
		{
			string url = "http://localhost/MCSWebApp/PermissionCenterServices/services/PermissionCenterToADService.asmx";

			InvokeServiceTask task = new InvokeServiceTask()
			{
				TaskID = UuidHelper.NewUuidString(),
				TaskTitle = "新任务",
			};

			WfServiceOperationParameterCollection parameters = new WfServiceOperationParameterCollection();

			parameters.Add(new WfServiceOperationParameter("callerID", "InvokeServiceTaskPersistTest"));

			task.SvcOperationDefs.Add(new Workflow.WfServiceOperationDefinition(
					new Workflow.WfServiceAddressDefinition(WfServiceRequestMethod.Post, null, url),
						"GetVersion", parameters, "ReturnValue")
				);

			return task;
		}

		/// <summary>
		/// 准备一个带错误的任务
		/// </summary>
		/// <returns></returns>
		private static InvokeServiceTask PrepareErrorInvokeServiceTask()
		{
			string url = "http://localhost/MCSWebApp/PermissionCenterServices/services/PermissionCenterToADService.asmx-error";

			InvokeServiceTask task = new InvokeServiceTask()
			{
				TaskID = UuidHelper.NewUuidString(),
				TaskTitle = "新任务",
			};

			WfServiceOperationParameterCollection parameters = new WfServiceOperationParameterCollection();

			parameters.Add(new WfServiceOperationParameter("callerID", "InvokeServiceTaskPersistTest"));

			task.SvcOperationDefs.Add(new Workflow.WfServiceOperationDefinition(
					new Workflow.WfServiceAddressDefinition(WfServiceRequestMethod.Post, null, url),
						"GetVersion", parameters, "ReturnValue")
				);

			return task;
		}
	}
}
