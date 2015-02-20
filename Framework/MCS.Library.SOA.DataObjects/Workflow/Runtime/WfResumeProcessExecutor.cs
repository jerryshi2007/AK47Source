using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfResumeProcessExecutor : WfExecutorBase
	{
		public IWfProcess ResumeProcess
		{
			get;
			private set;
		}

		public WfResumeProcessExecutor(IWfActivity operatorActivity, IWfProcess resumeProcess)
			: base(operatorActivity, WfControlOperationType.ResumeProcess)
		{
			resumeProcess.NullCheck("resumeProcess");

			this.ResumeProcess = resumeProcess;

			if (OperatorActivity == null)
				OperatorActivity = ResumeProcess.CurrentActivity;
		}

		protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
		{
			this.ResumeProcess.ResumeProcess(true);
		}

		protected override IWfProcess OnGetCurrentProcess()
		{
			IWfProcess result = WfRuntime.ProcessContext.CurrentProcess;

			if (result == null)
			{
				result = this.ResumeProcess;
				WfRuntime.ProcessContext.OriginalActivity = result.CurrentActivity;
			}

			return result;
		}
	}
}
