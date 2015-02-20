using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow.Tasks
{
	/// <summary>
	/// 撤回流程的任务流程的创建器
	/// </summary>
	public class DispatchWithdrawProcessSysTaskProcessBuilder : WfSysTaskProcessBuilderBase
	{
		public DispatchWithdrawProcessSysTaskProcessBuilder(string processID, bool cancelAllBranchProcesses)
			: this(string.Empty, processID, cancelAllBranchProcesses)
		{
		}

		public DispatchWithdrawProcessSysTaskProcessBuilder(string ownerSysActivityID, string processID, bool cancelAllBranchProcesses)
			: base()
		{
			processID.CheckStringIsNullOrEmpty("processID");

			this.ProcessID = processID;
			this.OwnerSysActivityID = ownerSysActivityID;
			this.CancelAllBranchProcesses = cancelAllBranchProcesses;
		}

		public string OwnerSysActivityID
		{
			get;
			private set;
		}

		public string ProcessID
		{
			get;
			private set;
		}

		public bool CancelAllBranchProcesses
		{
			get;
			private set;
		}

		protected override void AfterCreateProcessInstance(SysTaskProcess sysTaskProcess)
		{
			sysTaskProcess.Name = string.Format("撤回ID为{0}的流程", this.ProcessID);

			int index = 0;

			IWfProcess process = WfRuntime.GetProcessByProcessID(this.ProcessID);

			sysTaskProcess.OwnerActivityID = this.OwnerSysActivityID;

			WfProcessCurrentInfoCollection branchProcessesStatus = WfRuntime.GetProcessStatusByOwnerActivityID(process.CurrentActivity.ID, string.Empty, false);

			if (branchProcessesStatus.Count > 0 && this.CancelAllBranchProcesses)
			{
				sysTaskProcess.Activities.Add(sysTaskProcess.CreateDispatchCancelBranchesProcessActivity(process.CurrentActivity.ID, true, index++));
			}

			sysTaskProcess.Activities.Add(sysTaskProcess.CreateWithdrawProcessActivity(this.ProcessID, index++));
			sysTaskProcess.Activities.Add(sysTaskProcess.CreateExitMaintainingActivity(this.ProcessID, index++));
		}
	}
}
