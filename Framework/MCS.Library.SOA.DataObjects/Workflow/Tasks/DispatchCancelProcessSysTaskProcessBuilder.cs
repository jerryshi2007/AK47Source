using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow.Tasks
{
	/// <summary>
	/// 作废流程的任务流程的创建器
	/// </summary>
	public class DispatchCancelProcessSysTaskProcessBuilder : WfSysTaskProcessBuilderBase
	{
		public DispatchCancelProcessSysTaskProcessBuilder(string processID, bool cancelAllBranchProcesses)
			: this(string.Empty, processID, true, cancelAllBranchProcesses)
		{
		}

		public DispatchCancelProcessSysTaskProcessBuilder(string ownerSysActivityID, string processID, bool cancelSelf, bool cancelAllBranchProcesses)
			: base()
		{
			processID.CheckStringIsNullOrEmpty("processID");

			this.ProcessID = processID;
			this.OwnerSysActivityID = ownerSysActivityID;
			this.CancelSelf = cancelSelf;
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

		public bool CancelSelf
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
			sysTaskProcess.Name = string.Format("作废ID为{0}的流程", this.ProcessID);

			int index = 0;

			sysTaskProcess.OwnerActivityID = this.OwnerSysActivityID;

			IWfProcess process = WfRuntime.GetProcessByProcessID(this.ProcessID);

			WfProcessCurrentInfoCollection branchProcessesStatus = WfRuntime.GetProcessStatusByOwnerActivityID(process.CurrentActivity.ID, string.Empty, false);

			if (branchProcessesStatus.Count > 0 && this.CancelAllBranchProcesses)
			{
				//作废子流程
				sysTaskProcess.Activities.Add(sysTaskProcess.CreateDispatchCancelBranchesProcessActivity(process.CurrentActivity.ID, this.CancelAllBranchProcesses, index++));
			}

			if (this.CancelSelf)
				sysTaskProcess.Activities.Add(sysTaskProcess.CreateCancelProcessActivity(this.ProcessID, index++));

			sysTaskProcess.Activities.Add(sysTaskProcess.CreateExitMaintainingActivity(this.ProcessID, index++));
		}
	}
}
