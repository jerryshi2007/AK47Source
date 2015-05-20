using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Workflow.Actions
{
    /// <summary>
    /// 活动撤回时调用服务的Action
    /// </summary>
    public class WithdrawInvokeServiceAction : ActivityInvokeServiceActionBase
    {
        protected override WfServiceOperationDefinitionCollection GetOperationsBeforePersist()
        {
            return this.OriginalActivity.Descriptor.WithdrawExecuteServices.GetServiceOperationsBeforePersist();
        }

        protected override WfServiceOperationDefinitionCollection GetOperationsWhenPersist()
        {
            return this.OriginalActivity.Descriptor.WithdrawExecuteServices.GetServiceOperationsWhenPersist();
        }

        protected override string GetInvokeServiceKeys()
        {
            return this.OriginalActivity.Descriptor.WithdrawExecuteServiceKeys;
        }
    }
}
