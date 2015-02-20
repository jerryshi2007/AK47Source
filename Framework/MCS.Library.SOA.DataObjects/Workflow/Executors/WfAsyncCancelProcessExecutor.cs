using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow.Tasks;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 异步撤回流程
	/// </summary>
	public class WfAsyncCancelProcessExecutor : WfExecutorBase
	{
		public WfAsyncCancelProcessExecutor(IWfActivity operatorActivity, IWfProcess process)
			: base(operatorActivity, WfControlOperationType.AsyncCancelProcess)
		{
			process.NullCheck("process");

			this.Process = process;

			if (this.OperatorActivity == null)
				this.OperatorActivity = process.CurrentActivity;
		}

		public IWfProcess Process
		{
			get;
			private set;
		}

		protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
		{
			(this.Process.Status == WfProcessStatus.Running).FalseThrow("流程不在运行中，无法作废");

			this.Process.EntrtMaintainingStatus();
		}

		protected override void OnSaveApplicationData(WfExecutorDataContext dataContext)
		{
			DispatchCancelProcessTask cancelTask = DispatchCancelProcessTask.SendTask(string.Empty, this.Process.ID, true);

			base.OnSaveApplicationData(dataContext);
		}

		protected override void OnPrepareUserOperationLogDescription(WfExecutorDataContext dataContext, UserOperationLog log)
		{
			log.Subject = "异步作废流程";
		}
	}
}
