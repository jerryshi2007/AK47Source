using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfCancelProcessExecutor : WfExecutorBase
	{
		public IWfProcess CancelProcess
		{
			get;
			private set;
		}

		/// <summary>
		/// 是否递归作废所有子流程
		/// </summary>
		public bool CancelAllBranchProcesses
		{
			get;
			private set;
		}

		public WfCancelProcessExecutor(IWfActivity operatorActivity, IWfProcess cancelProcess)
			: base(operatorActivity, WfControlOperationType.CancelProcess)
		{
			cancelProcess.NullCheck("cancelProcess");

			this.CancelProcess = cancelProcess;
			this.CancelAllBranchProcesses = true;

			if (OperatorActivity == null)
				OperatorActivity = CancelProcess.CurrentActivity;
		}

		public WfCancelProcessExecutor(IWfActivity operatorActivity, IWfProcess cancelProcess, bool cancelAllBranchProcesses)
			: base(operatorActivity, WfControlOperationType.CancelProcess)
		{
			cancelProcess.NullCheck("cancelProcess");

			this.CancelProcess = cancelProcess;
			this.CancelAllBranchProcesses = cancelAllBranchProcesses;

			if (OperatorActivity == null)
				OperatorActivity = CancelProcess.CurrentActivity;
		}

		protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
		{
			this.CancelProcess.CancelProcess(this.CancelAllBranchProcesses);

			IWfProcess process = dataContext.CurrentProcess;

			while ((process.Status == WfProcessStatus.Completed || process.Status == WfProcessStatus.Aborted) &&
				process.EntryInfo != null)
			{
				process.EntryInfo.OwnerActivity.Process.ProcessPendingActivity();
				process = process.EntryInfo.OwnerActivity.Process;
			}
		}

		protected override IWfProcess OnGetCurrentProcess()
		{
			IWfProcess result = WfRuntime.ProcessContext.CurrentProcess;

			if (result == null)
			{
				result = this.CancelProcess;
				WfRuntime.ProcessContext.OriginalActivity = result.CurrentActivity;
			}

			return result;
		}
	}
}
