using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects.Workflow.Contracts;
using MCS.Library.SOA.DataObjects.Workflow.Tasks;
using MCS.Library.WcfExtensions;

namespace WfPlatformServices.Services
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class WfProcessService : IWfProcessService
	{
		#region Dispatch操作
		/// <summary>
		/// 根据分支流程模板，将模板中的资源中的每一个人员进行分解，分发出不同的启动分支流程的任务。返回分发的任务数。
		/// 对于统一流程内串行，不会启动多个流程任务。
		/// </summary>
		/// <param name="ownerActivityID">分支流程所挂接的活动</param>
		/// <param name="template">分支流程模板</param>
		/// <param name="autoAddExitMaintainingStatusTask">是否在最后补充一个退出维护模式的任务</param>
		/// <returns>分发的启动分支流程的任务个数</returns>
		[WfJsonFormatter]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public int DispatchStartBranchProcessTasks(string ownerActivityID, IWfBranchProcessTemplateDescriptor template, bool autoAddExitMaintainingStatusTask)
		{
			ownerActivityID.CheckStringIsNullOrEmpty("ownerActivityID");
			template.NullCheck("template");

			StartBranchProcessSysTaskProcessBuilder builder = new StartBranchProcessSysTaskProcessBuilder(ownerActivityID, template, autoAddExitMaintainingStatusTask);

			SysTaskProcess sysTaskProcess = builder.Build();

			SysTaskProcessRuntime.StartProcess(sysTaskProcess);

			return sysTaskProcess.Activities.Count;
		}

		/// <summary>
		/// 分发作废流程的任务
		/// </summary>
		/// <param name="ownerSysActivityID">父任务流程活动的ID，可以为空</param>
		/// <param name="processID"></param>
		/// <param name="cancelAllBranchProcesses"></param>
		/// <returns></returns>
		[WfJsonFormatter]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public int DispatchCancelProcessTask(string ownerSysActivityID, string processID, bool cancelAllBranchProcesses)
		{
			IWfProcess process = WfRuntime.GetProcessByProcessID(processID);

			IWfActivity currentActivity = process.CurrentActivity;

			process.EntrtMaintainingStatus();

			WfRuntime.PersistWorkflows();

			int result = 0;

			DispatchCancelProcessSysTaskProcessBuilder builder = new DispatchCancelProcessSysTaskProcessBuilder(ownerSysActivityID, process.ID, true, cancelAllBranchProcesses);

			SysTaskProcess sysTaskProcess = builder.Build();

			SysTaskProcessRuntime.StartProcess(sysTaskProcess);

			result += sysTaskProcess.Activities.Count;

			return result;
		}

		[WfJsonFormatter]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public int DispatchCancelBranchProcessesTasks(string ownerSysActivityID, string ownerActivityID, bool cancelAllBranchProcesses)
		{
			int result = 0;

			WfProcessCurrentInfoCollection branchProcessesStatus = WfRuntime.GetProcessStatusByOwnerActivityID(ownerActivityID, string.Empty, false);

			foreach (WfProcessCurrentInfo processInfo in branchProcessesStatus)
			{
				DispatchCancelProcessSysTaskProcessBuilder builder = new DispatchCancelProcessSysTaskProcessBuilder(ownerSysActivityID, processInfo.InstanceID, true, cancelAllBranchProcesses);

				SysTaskProcess sysTaskProcess = builder.Build();

				SysTaskProcessRuntime.StartProcess(sysTaskProcess);

				result += sysTaskProcess.Activities.Count;
			}

			return result;
		}

		/// <summary>
		/// 分发撤回流程的任务
		/// </summary>
		/// <param name="ownerSysActivityID">父任务流程活动的ID，可以为空</param>
		/// <param name="processID"></param>
		/// <param name="cancelAllBranchProcesses"></param>
		/// <returns></returns>
		[WfJsonFormatter]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public int DispatchWithdrawProcessTask(string ownerSysActivityID, string processID, bool cancelAllBranchProcesses)
		{
			IWfProcess process = WfRuntime.GetProcessByProcessID(processID);

			IWfActivity currentActivity = process.CurrentActivity;

			process.EntrtMaintainingStatus();

			WfRuntime.PersistWorkflows();

			DispatchWithdrawProcessSysTaskProcessBuilder builder = new DispatchWithdrawProcessSysTaskProcessBuilder(ownerSysActivityID, process.ID, cancelAllBranchProcesses);

			SysTaskProcess sysTaskProcess = builder.Build();

			SysTaskProcessRuntime.StartProcess(sysTaskProcess);

			return sysTaskProcess.Activities.Count;
		}
		#endregion Dispatch

		#region 流程操作
		/// <summary>
		/// 同步作废流程
		/// </summary>
		/// <param name="processID">需要作废的流程ID</param>
		/// <param name="cancelAllBranchProcesses">是否递归作废所有子流程</param>
		[WfJsonFormatter]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public void CancelProcess(string processID, bool cancelAllBranchProcesses)
		{
			processID.CheckStringIsNullOrEmpty("processID");

			IWfProcess process = WfRuntime.GetProcessByProcessID(processID);

			IWfActivity currentActivity = process.CurrentActivity;

			//仅作废当前流程
			WfCancelProcessExecutor executor = new WfCancelProcessExecutor(process.CurrentActivity, process, cancelAllBranchProcesses);

			executor.Execute();
		}

		/// <summary>
		/// 撤回流程的步骤
		/// </summary>
		/// <param name="processID">需要撤回的流程ID</param>
		/// <param name="cancelAllBranchProcesses">是否递归作废所有子流程</param>
		[WfJsonFormatter]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public void WithdrawProcess(string processID, bool cancelAllBranchProcesses)
		{
			processID.CheckStringIsNullOrEmpty("processID");

			IWfProcess process = WfRuntime.GetProcessByProcessID(processID);

			WfWithdrawExecutor executor = new WfWithdrawExecutor(process.CurrentActivity, process.CurrentActivity, cancelAllBranchProcesses);

			executor.Execute();
		}

		/// <summary>
		/// 启动分支流程
		/// </summary>
		/// <param name="ownerActivityID">分支流程所挂接的活动</param>
		/// <param name="branchTransferParams">分支流程启动参数</param>
		/// <returns>已经启动的分支流程的实例ID</returns>
		[WfJsonFormatter]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string[] StartBranchProcesses(string ownerActivityID, WfBranchProcessTransferParams branchTransferParams)
		{
			ownerActivityID.CheckStringIsNullOrEmpty("ownerActivityID");
			branchTransferParams.NullCheck("branchTransferParams");

			IWfProcess process = WfRuntime.GetProcessByActivityID(ownerActivityID);

			IWfActivity ownerActivity = process.Activities[ownerActivityID];

			WfStartBranchProcessExecutor executor = new WfStartBranchProcessExecutor(ownerActivity, ownerActivity, branchTransferParams);

			executor.Execute();

			List<string> startedProcessIDs = new List<string>();

			executor.StartedProcesses.ForEach(p => startedProcessIDs.Add(p.ID));

			return startedProcessIDs.ToArray();
		}

		/// <summary>
		/// 退出流程的维护模式
		/// </summary>
		/// <param name="processID">流程ID</param>
		/// <param name="processPendingActivity">是否处理挂起的活动</param>
		[WfJsonFormatter]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public void ExitMaintainingStatus(string processID, bool processPendingActivity)
		{
			processID.NullCheck("processID");

			IWfProcess process = WfRuntime.GetProcessByProcessID(processID);

			WfExitMaintainingStatusExecutor executor = new WfExitMaintainingStatusExecutor(process.CurrentActivity, process, processPendingActivity);

			executor.Execute();
		}
		#endregion 流程操作
	}
}
