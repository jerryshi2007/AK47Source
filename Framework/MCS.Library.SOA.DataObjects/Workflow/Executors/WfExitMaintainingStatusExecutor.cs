using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 退出维护模式的执行器
	/// </summary>
	public class WfExitMaintainingStatusExecutor : WfExecutorBase
	{
		public WfExitMaintainingStatusExecutor(IWfActivity operatorActivity, IWfProcess process)
			: this(operatorActivity, process, true)
		{
		}

		public WfExitMaintainingStatusExecutor(IWfActivity operatorActivity, IWfProcess process, bool autoProcessPendingActivity)
			: base(operatorActivity, WfControlOperationType.ExitMaintainingStatus)
		{
			process.NullCheck("process");

			this.Process = process;
			this.AutoProcessPendingActivity = autoProcessPendingActivity;

			if (this.OperatorActivity == null)
				this.OperatorActivity = process.CurrentActivity;
		}

		public IWfProcess Process
		{
			get;
			private set;
		}

		public bool AutoProcessPendingActivity
		{
			get;
			private set;
		}

		protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
		{
			this.Process.ExitMaintainingStatus(this.AutoProcessPendingActivity);
		}

		protected override void OnPrepareUserOperationLogDescription(WfExecutorDataContext dataContext, UserOperationLog log)
		{
			log.Subject = "退出运维模式";
		}
	}
}
