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
            if (null != actionParams.Context.StatusChangedProcesses)
            {
                List<object> result = new List<object>();
                actionParams.Context.StatusChangedProcesses.ForEach(p =>
                {
                    string tenantCode = p.ApplicationRuntimeParameters.GetValue(UserTaskServicePlugin._TenantCode, string.Empty);
                    string mailCollector = p.ApplicationRuntimeParameters.GetValue(UserTaskServicePlugin._MailCollector, string.Empty);
                    result.Add(new
                    {
                        ProcessId = p.ID,
                        ProcessKey = p.Descriptor.Key,
                        ProcessName = p.Descriptor.Name,
                        Status = p.Status.ToString(),
                        CreatorId = p.Creator.ID,
                        CreatorName = p.Creator.DisplayName,
                        Created = p.StartTime,
                        TenantCode = tenantCode,
                        EmailCollector = mailCollector
                    });
                });

                if (result.Count >= 0)
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    UserTaskServicePluginBroker.Instance.SyncProcess(serializer.Serialize(result));
                }
                    
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
