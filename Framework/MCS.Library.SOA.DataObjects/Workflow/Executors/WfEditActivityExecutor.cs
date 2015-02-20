using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 修改主线流程活动的Executor
	/// </summary>
	public class WfEditActivityExecutor : WfAddAndEditActivityExecutorBase
	{
		public WfEditActivityExecutor(IWfActivity operatorActivity, IWfActivity targetActivity, WfActivityDescriptorCreateParams createParams)
			: base(operatorActivity, targetActivity, createParams, WfControlOperationType.EditActivity)
		{
		}

		protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
		{
			base.OnModifyWorkflow(dataContext);

			if (WfRuntime.ProcessContext != null)
				WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(this.TargetActivity.Process);
		}

		protected override IWfActivity PrepareInstanceActivity()
		{
			return this.TargetActivity;
		}

        protected override IWfActivityDescriptor PrepareActivityDescriptor(IWfActivityDescriptor targetActDesp)
        {
            return targetActDesp;
        }
	}
}
