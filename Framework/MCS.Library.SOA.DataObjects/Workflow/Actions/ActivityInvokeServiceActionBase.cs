using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow.Actions
{
    /// <summary>
    /// 活动进出需要调用的外部服务的Action基类
    /// </summary>
    public abstract class ActivityInvokeServiceActionBase : InvokeServiceActionBase
    {
        private IWfActivity _OriginalActivity = null;

        public IWfActivity OriginalActivity
        {
            get
            {
                return this._OriginalActivity;
            }
        }

        public override void PrepareAction(WfActionParams actionParams)
        {
            this._OriginalActivity = actionParams.Context.CurrentActivity;

            base.PrepareAction(actionParams);
        }

        protected override WfApplicationRuntimeParameters GetApplicationRuntimeParameters()
        {
            return this.OriginalActivity.Process.ApplicationRuntimeParameters;
        }
    }
}
