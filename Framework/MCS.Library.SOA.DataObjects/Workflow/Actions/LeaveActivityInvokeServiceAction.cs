using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow.Actions
{
    [Serializable]
    public class LeaveActivityInvokeServiceAction : ActivityInvokeServiceActionBase
    {
        protected override WfServiceOperationDefinitionCollection GetOperationsBeforePersist()
        {
            return this.OriginalActivity.Descriptor.LeaveEventExecuteServices.GetServiceOperationsBeforePersist();
        }

        protected override WfServiceOperationDefinitionCollection GetOperationsWhenPersist()
        {
            return this.OriginalActivity.Descriptor.LeaveEventExecuteServices.GetServiceOperationsWhenPersist();
        }

        protected override string GetInvokeServiceKeys()
        {
            return this.OriginalActivity.Descriptor.LeaveEventExecuteServiceKeys;
        }
    }
}
