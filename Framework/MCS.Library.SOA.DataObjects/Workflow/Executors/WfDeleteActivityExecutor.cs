using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 删除主线流程活动的Executor
    /// </summary>
    public class WfDeleteActivityExecutor : WfActivityRelativeExecutorBase
    {
        public WfDeleteActivityExecutor(IWfActivity operatorActivity, IWfActivity targetActivity) :
            base(operatorActivity, targetActivity, WfControlOperationType.DeleteActivity)
        {
        }

        protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
        {
            this.TargetActivity.Delete();

            IWfActivityDescriptor msActDesp = this.TargetActivity.GetMainStreamActivityDescriptor();

            if (msActDesp != null)
                msActDesp.Delete();
        }
    }
}
