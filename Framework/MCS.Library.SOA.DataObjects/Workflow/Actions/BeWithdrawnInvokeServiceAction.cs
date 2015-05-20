using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Workflow.Actions
{
    /// <summary>
    /// 活动被撤回时调用服务的Action
    /// </summary>
    public class BeWithdrawnInvokeServiceAction : ActivityInvokeServiceActionBase
    {
        protected override WfServiceOperationDefinitionCollection GetOperationsBeforePersist()
        {
            return this.OriginalActivity.Descriptor.BeWithdrawnExecuteServices.GetServiceOperationsBeforePersist();
        }

        protected override WfServiceOperationDefinitionCollection GetOperationsWhenPersist()
        {
            return this.OriginalActivity.Descriptor.BeWithdrawnExecuteServices.GetServiceOperationsWhenPersist();
        }

        protected override string GetInvokeServiceKeys()
        {
            return this.OriginalActivity.Descriptor.BeWithdrawnExecuteServiceKeys;
        }
    }
}
