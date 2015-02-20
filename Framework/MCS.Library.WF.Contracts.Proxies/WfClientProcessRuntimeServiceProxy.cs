using MCS.Library.Core;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Operations;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using MCS.Web.Library.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Proxies
{
    public class WfClientProcessRuntimeServiceProxy
    {
        public static readonly WfClientProcessRuntimeServiceProxy Instance = new WfClientProcessRuntimeServiceProxy();

        private WfClientProcessRuntimeServiceProxy()
        {
        }

        public WfClientProcessInfo Cancel(string processID, WfClientRuntimeContext runtimeContext)
        {
            WfClientProcessInfo processInfo = null;

            ServiceProxy.SingleCall<IWfClientProcessRuntimeService>(WfClientFactory.GetProcessRuntimeService(), action =>
            {
                processInfo = action.Cancel(processID, runtimeContext);
            });

            return processInfo;
        }

        public WfClientProcessInfo MoveTo(string processID, WfClientTransferParams clientTransferParams, WfClientRuntimeContext runtimeContext)
        {
            WfClientProcessInfo processInfo = null;

            ServiceProxy.SingleCall<IWfClientProcessRuntimeService>(WfClientFactory.GetProcessRuntimeService(), action =>
            {
                processInfo = action.MoveTo(processID, clientTransferParams, runtimeContext);
            });

            return processInfo;
        }

        public WfClientProcessInfo MoveToNextDefaultActivity(string processID, WfClientRuntimeContext runtimeContext)
        {
            WfClientProcessInfo processInfo = null;

            ServiceProxy.SingleCall<IWfClientProcessRuntimeService>(WfClientFactory.GetProcessRuntimeService(), action =>
            {
                processInfo = action.MoveToNextDefaultActivity(processID, runtimeContext);
            });

            return processInfo;
        }

        public WfClientProcessInfo SaveProcess(string processID, WfClientRuntimeContext runtimeContext)
        {
            WfClientProcessInfo processInfo = null;

            ServiceProxy.SingleCall<IWfClientProcessRuntimeService>(WfClientFactory.GetProcessRuntimeService(), action =>
            {
                processInfo = action.SaveProcess(processID, runtimeContext);
            });

            return processInfo;
        }

        public WfClientProcessInfo UpdateRuntimeParameters(string processID, WfClientRuntimeContext runtimeContext)
        {
            WfClientProcessInfo processInfo = null;

            ServiceProxy.SingleCall<IWfClientProcessRuntimeService>(WfClientFactory.GetProcessRuntimeService(), action =>
            {
                processInfo = action.UpdateRuntimeParameters(processID, runtimeContext);
            });

            return processInfo;
        }

        public WfClientProcessInfo Pause(string processID, WfClientRuntimeContext runtimeContext)
        {
            WfClientProcessInfo processInfo = null;

            ServiceProxy.SingleCall<IWfClientProcessRuntimeService>(WfClientFactory.GetProcessRuntimeService(), action =>
            {
                processInfo = action.Pause(processID, runtimeContext);
            });

            return processInfo;
        }

        public WfClientProcessInfo Restore(string processID, WfClientRuntimeContext runtimeContext)
        {
            WfClientProcessInfo processInfo = null;

            ServiceProxy.SingleCall<IWfClientProcessRuntimeService>(WfClientFactory.GetProcessRuntimeService(), action =>
            {
                processInfo = action.Restore(processID, runtimeContext);
            });

            return processInfo;
        }

        public WfClientProcessInfo Resume(string processID, WfClientRuntimeContext runtimeContext)
        {
            WfClientProcessInfo processInfo = null;

            ServiceProxy.SingleCall<IWfClientProcessRuntimeService>(WfClientFactory.GetProcessRuntimeService(), action =>
            {
                processInfo = action.Resume(processID, runtimeContext);
            });

            return processInfo;
        }

        public WfClientProcessInfo StartWorkflow(WfClientProcessStartupParams clientStartupParams)
        {
            WfClientProcessInfo processInfo = null;

            ServiceProxy.SingleCall<IWfClientProcessRuntimeService>(WfClientFactory.GetProcessRuntimeService(), action =>
            {
                processInfo = action.StartWorkflow(clientStartupParams);
            });

            return processInfo;
        }

        public WfClientProcessInfo Withdraw(string processID, WfClientRuntimeContext runtimeContext)
        {
            WfClientProcessInfo processInfo = null;

            ServiceProxy.SingleCall<IWfClientProcessRuntimeService>(WfClientFactory.GetProcessRuntimeService(), action =>
            {
                processInfo = action.Withdraw(processID, runtimeContext);
            });

            return processInfo;
        }

        public WfClientProcess GetProcessByID(string processID, WfClientUser user)
        {
            return this.GetProcessByID(processID, user, WfClientProcessInfoFilter.Default);
        }

        public WfClientProcess GetProcessByID(string processID, WfClientUser user, WfClientProcessInfoFilter filter)
        {
            WfClientProcess process = null;

            ServiceProxy.SingleCall<IWfClientProcessRuntimeService>(WfClientFactory.GetProcessRuntimeService(), action =>
            {
                process = action.GetProcessByID(processID, user, filter);
            });

            return process;
        }

        public WfClientProcess GetProcessByActivityID(string activityID, WfClientUser user)
        {
            return this.GetProcessByActivityID(activityID, user, WfClientProcessInfoFilter.Default);
        }

        public WfClientProcess GetProcessByActivityID(string activityID, WfClientUser user, WfClientProcessInfoFilter filter)
        {
            WfClientProcess process = null;

            ServiceProxy.SingleCall<IWfClientProcessRuntimeService>(WfClientFactory.GetProcessRuntimeService(), action =>
            {
                process = action.GetProcessByActivityID(activityID, user, filter);
            });

            return process;
        }

        public WfClientProcessInfo GetProcessInfoByID(string processID, WfClientUser user)
        {
            WfClientProcessInfo processInfo = null;

            ServiceProxy.SingleCall<IWfClientProcessRuntimeService>(WfClientFactory.GetProcessRuntimeService(), action =>
            {
                processInfo = action.GetProcessInfoByID(processID, user);
            });

            return processInfo;
        }

        public WfClientProcessInfo GetProcessInfoByActivityID(string activityID, WfClientUser user)
        {
            WfClientProcessInfo processInfo = null;

            ServiceProxy.SingleCall<IWfClientProcessRuntimeService>(WfClientFactory.GetProcessRuntimeService(), action =>
            {
                processInfo = action.GetProcessInfoByActivityID(activityID, user);
            });

            return processInfo;
        }

        public WfClientProcessInfoCollection GetProcessInfoByResourceID(string resourceID, WfClientUser user)
        {
            WfClientProcessInfoCollection result = new WfClientProcessInfoCollection();

            ServiceProxy.SingleCall<IWfClientProcessRuntimeService>(WfClientFactory.GetProcessRuntimeService(), action =>
            {
                List<WfClientProcessInfo> processes = action.GetProcessesInfoByResourceID(resourceID, user);

                processes.ForEach(process => result.Add(process));
            });

            return result;
        }

        public WfClientProcessCollection GetProcessByResourceID(string resourceID, WfClientUser user)
        {
            return this.GetProcessByResourceID(resourceID, user, WfClientProcessInfoFilter.Default);
        }

        public WfClientProcessCollection GetProcessByResourceID(string resourceID, WfClientUser user, WfClientProcessInfoFilter filter)
        {
            WfClientProcessCollection result = new WfClientProcessCollection();

            ServiceProxy.SingleCall<IWfClientProcessRuntimeService>(WfClientFactory.GetProcessRuntimeService(), action =>
            {
                List<WfClientProcess> processes = action.GetProcessesByResourceID(resourceID, user, filter);

                processes.ForEach(process => result.Add(process));
            });

            return result;
        }

        public WfClientOpinionCollection GetOpinionsByResourceID(string resourceID)
        {
            WfClientOpinionCollection result = null;

            ServiceProxy.SingleCall<IWfClientProcessRuntimeService>(WfClientFactory.GetProcessRuntimeService(), action =>
            {
                result = new WfClientOpinionCollection(action.GetOpinionsByResourceID(resourceID));
            });

            return result;
        }

        public WfClientOpinionCollection GetOpinionsByProcessID(string processID)
        {
            WfClientOpinionCollection result = null;

            ServiceProxy.SingleCall<IWfClientProcessRuntimeService>(WfClientFactory.GetProcessRuntimeService(), action =>
            {
                result = new WfClientOpinionCollection(action.GetOpinionsByProcessID(processID));
            });

            return result;
        }

        public T GetApplicationRuntimeParameters<T>(string processID, string key, WfClientProbeApplicationRuntimeParameterMode probeMode, T defaultValue)
        {
            T result = defaultValue;

            ServiceProxy.SingleCall<IWfClientProcessRuntimeService>(WfClientFactory.GetProcessRuntimeService(), action =>
            {
                string serializedData = action.GetApplicationRuntimeParameters(processID, key, probeMode);

                Dictionary<string, object> data = JSONSerializerExecute.Deserialize<Dictionary<string, object>>(serializedData);

                result = data.GetValue("d", defaultValue);
            });

            return result;
        }

        public WfClientProcessCurrentInfoPageQueryResult QueryBranchProcesses(string ownerActivityID, string ownerTemplateKey, int startRowIndex, int maximumRows, string orderBy, int totalCount)
        {
            WfClientProcessCurrentInfoPageQueryResult result = null;

            ServiceProxy.SingleCall<IWfClientProcessRuntimeService>(WfClientFactory.GetProcessRuntimeService(), action =>
            {
                result = action.QueryBranchProcesses(ownerActivityID, ownerTemplateKey, startRowIndex, maximumRows, orderBy, totalCount);
            });

            return result;
        }
    }
}
