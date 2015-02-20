using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Globalization;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 启动分支流程的执行器，用于多次在某个活动下生成子流程
	/// </summary>
	public class WfStartBranchProcessExecutor : WfExecutorBase
	{
		public WfStartBranchProcessExecutor(IWfActivity operatorActivity, IWfActivity ownerActivity, WfBranchProcessTransferParams branchTransferParams)
			: base(operatorActivity, WfControlOperationType.StartBranchProcess)
		{
			ownerActivity.NullCheck("ownerActivity");
			branchTransferParams.NullCheck("branchTransferParams");

			this.OwnerActivity = ownerActivity;
			this.BranchTransferParams = branchTransferParams;

			if (this.OperatorActivity == null)
				this.OperatorActivity = ownerActivity;
		}

		public WfStartBranchProcessExecutor(IWfActivity operatorActivity, IWfActivity ownerActivity, IWfBranchProcessTemplateDescriptor template)
			: base(operatorActivity, WfControlOperationType.StartBranchProcess)
		{
			ownerActivity.NullCheck("ownerActivity");
			template.NullCheck("template");

			this.OwnerActivity = ownerActivity;
			this.BranchTransferParams = new WfBranchProcessTransferParams(template);
		}

		public IWfActivity OwnerActivity
		{
			get;
			private set;
		}

		public WfBranchProcessTransferParams BranchTransferParams
		{
			get;
			private set;
		}

		/// <summary>
		/// 已经启动的流程
		/// </summary>
		public WfProcessCollection StartedProcesses
		{
			get;
			private set;
		}

		protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
		{
			this.StartedProcesses = ((WfActivityBase)this.OwnerActivity).InternalStartupBranchProcesses(this.BranchTransferParams, false);
		}

		protected override void OnPrepareUserOperationLogDescription(WfExecutorDataContext dataContext, UserOperationLog log)
		{
			log.Subject = Translator.Translate(Define.DefaultCulture, "启动分支流程");

			StringBuilder strB = new StringBuilder();

			foreach (IWfProcess process in this.StartedProcesses)
			{
				if (strB.Length > 0)
					strB.Append(",");

				strB.Append(process.ID);
			}

			log.OperationDescription = Translator.Translate(
				Define.DefaultCulture, 
				"根据模板{0}，启动了{1}条流程，ID为:{2}",
				this.BranchTransferParams.Template.Key,
				this.StartedProcesses.Count,
				strB.ToString());
		}
	}
}
