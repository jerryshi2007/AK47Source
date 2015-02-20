using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// Restored Canceled Process
	/// </summary>
	public class WfRestoreProcessExecutor : WfExecutorBase
	{
		public IWfProcess Process
		{
			get;
			private set;
		}

		public WfRestoreProcessExecutor(IWfActivity operatorActivity, IWfProcess restoreProcess)
			: base(operatorActivity, WfControlOperationType.RestoreProcess)
		{
			restoreProcess.NullCheck("restoreProcess");

			this.Process = restoreProcess;

			if (OperatorActivity == null)
				OperatorActivity = Process.CurrentActivity;
		}

		protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
		{
			this.Process.RestoreProcess();
		}

		protected override IWfProcess OnGetCurrentProcess()
		{
			IWfProcess result = WfRuntime.ProcessContext.CurrentProcess;

			if (result == null)
			{
				result = this.Process;
				WfRuntime.ProcessContext.OriginalActivity = result.CurrentActivity;
			}

			return result;
		}
	}
}
