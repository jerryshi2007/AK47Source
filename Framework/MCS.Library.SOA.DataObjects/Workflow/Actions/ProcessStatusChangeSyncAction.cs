using MCS.Library.SOA.DataObjects.UserTaskPlugin;
using MCS.Library.SOA.DataObjects.UserTaskSync;
using MCS.Web.Library.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.SOA.DataObjects.Workflow.Actions
{
    public class ProcessStatusChangeSyncAction : IWfAction
    {

        public void PrepareAction(WfActionParams actionParams)
        {

        }

        public void PersistAction(WfActionParams actionParams)
        {
            if (actionParams.Context.StatusChangedProcesses != null)
            {
                List<object> result = new List<object>();

                actionParams.Context.StatusChangedProcesses.ForEach(p =>
                {
                    result.Add(new
                    {
                        ProcessId = p.ID,
                        ProcessKey = p.Descriptor.Key,
                        ProcessName = p.Descriptor.Name,
                        Status = p.Status.ToString(),
                        CreatorId = p.Creator.ID,
                        CreatorName = p.Creator.DisplayName,
                        Created = p.StartTime,
                        TenantCode = p.ApplicationRuntimeParameters.GetValue(UserTaskServicePlugin._TenantCode, string.Empty),
                        EmailCollector = p.ApplicationRuntimeParameters.GetValue(UserTaskServicePlugin._MailCollector, string.Empty)
                    });
                });

                if (result.Count >= 0)
                    UserTaskServicePluginBroker.Instance.SyncProcess(JSONSerializerExecute.Serialize(result));
            }
        }

        public void AfterWorkflowPersistAction(WfActionParams actionParams)
        {
        }

        public void ClearCache()
        {
        }
    }
}
