using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 暂停流程的执行器
	/// </summary>
	public class WfPauseProcessExecutor : WfExecutorBase
	{
		public IWfProcess PauseProcess
		{
			get;
			private set;
		}

		public WfPauseProcessExecutor(IWfActivity operatorActivity, IWfProcess pauseProcess)
			: base(operatorActivity, WfControlOperationType.PauseProcess)
		{
			pauseProcess.NullCheck("pauseProcess");

			this.PauseProcess = pauseProcess;

			if (OperatorActivity == null)
				OperatorActivity = PauseProcess.CurrentActivity;
		}

		protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
		{
			this.PauseProcess.PauseProcess(true);
		}

		protected override IWfProcess OnGetCurrentProcess()
		{
			IWfProcess result = WfRuntime.ProcessContext.CurrentProcess;

			if (result == null)
			{
				result = this.PauseProcess;
				WfRuntime.ProcessContext.OriginalActivity = result.CurrentActivity;
			}

			return result;
		}
	}
}
