using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow.Actions
{
    [Serializable]
    public class CancelExecuteInvokeServiceAction : InvokeServiceActionBase
    {
        private IWfProcess _Process = null;

        public override void PrepareAction(WfActionParams actionParams)
        {
            this._Process = WfRuntime.ProcessContext.CurrentProcess;

            base.PrepareAction(actionParams);
        }

        protected override WfServiceOperationDefinitionCollection GetOperationsBeforePersist()
        {
            WfServiceOperationDefinitionCollection result = this._Process.Descriptor.CancelBeforeExecuteServices.GetServiceOperationsBeforePersist();

            result.CopyFrom(this._Process.Descriptor.CancelAfterExecuteServices.GetServiceOperationsBeforePersist());

            return result;
        }

        protected override WfServiceOperationDefinitionCollection GetOperationsWhenPersist()
        {
            WfServiceOperationDefinitionCollection result = this._Process.Descriptor.CancelBeforeExecuteServices.GetServiceOperationsWhenPersist();

            result.CopyFrom(this._Process.Descriptor.CancelAfterExecuteServices.GetServiceOperationsWhenPersist());

            return result;
        }

        protected override string GetInvokeServiceKeys()
        {
            return this._Process.Descriptor.CancelExecuteServiceKeys;
        }

        protected override WfApplicationRuntimeParameters GetApplicationRuntimeParameters()
        {
            return this._Process.ApplicationRuntimeParameters;
        }
    }
}
