using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Principal;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfStartWorkflowExecutor : WfExecutorBase
	{
		public WfProcessStartupParams StartupParams
		{
			get;
			private set;
		}

		public WfStartWorkflowExecutor(IWfActivity operatorActivity, WfProcessStartupParams startupParams)
			: base(operatorActivity, WfControlOperationType.Startup)
		{
			startupParams.NullCheck("startupParams");

			StartupParams = startupParams;
		}

		public WfStartWorkflowExecutor(WfProcessStartupParams startupParams)
			: base(null, WfControlOperationType.Startup)
		{
			startupParams.NullCheck("startupParams");

			StartupParams = startupParams;
		}

		protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
		{
			IWfProcess process = WfRuntime.StartWorkflow(StartupParams);

			if (OperatorActivity == null)
			{
				OperatorActivity = process.CurrentActivity;
			}

			WfRuntime.ProcessContext.ResetContextByProcess(process);
		}
	}
}
