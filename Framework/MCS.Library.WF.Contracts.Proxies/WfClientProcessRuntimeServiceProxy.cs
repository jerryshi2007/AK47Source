using MCS.Library.Core;
using MCS.Library.WcfExtensions;
using MCS.Library.WF.Contracts.DataObjects;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Operations;
using MCS.Library.WF.Contracts.Proxies.Configuration;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using MCS.Web.Library.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Proxies
{
    public class WfClientProcessRuntimeServiceProxy : WfClientServiceProxyBase<IWfClientProcessRuntimeService>
    {
        public static readonly WfClientProcessRuntimeServiceProxy Instance = new WfClientProcessRuntimeServiceProxy();

        private WfClientProcessRuntimeServiceProxy()
        {
        }

        public WfClientProcessInfo Cancel(string processID, WfClientRuntimeContext runtimeContext)
        {
            return this.SingleCall(action => action.Cancel(processID, runtimeContext));
        }

        public WfClientProcessInfo MoveTo(string processID, WfClientTransferParams clientTransferParams, WfClientRuntimeContext runtimeContext)
        {
            return this.SingleCall(action => action.MoveTo(processID, clientTransferParams, runtimeContext));
        }

        public WfClientProcessInfo MoveToNextDefaultActivity(string processID, WfClientRuntimeContext runtimeContext)
        {
            return this.SingleCall(action => action.MoveToNextDefaultActivity(processID, runtimeContext));
        }

        public WfClientProcessInfo SaveProcess(string processID, WfClientRuntimeContext runtimeContext)
        {
            return this.SingleCall(action => action.SaveProcess(processID, runtimeContext));
        }

        public WfClientProcessInfo UpdateRuntimeParameters(string processID, WfClientRuntimeContext runtimeContext)
        {
            return this.SingleCall(action => action.UpdateRuntimeParameters(processID, runtimeContext));
        }

        public WfClientProcessInfo Pause(string processID, WfClientRuntimeContext runtimeContext)
        {
            return this.SingleCall(action => action.Pause(processID, runtimeContext));
        }

        public WfClientProcessInfo Restore(string processID, WfClientRuntimeContext runtimeContext)
        {
            return this.SingleCall(action => action.Restore(processID, runtimeContext));
        }

        public WfClientProcessInfo Resume(string processID, WfClientRuntimeContext runtimeContext)
        {
            return this.SingleCall(action => action.Resume(processID, runtimeContext));
        }

        public WfClientProcessInfo StartWorkflow(WfClientProcessStartupParams clientStartupParams)
        {
            return this.SingleCall(action => action.StartWorkflow(clientStartupParams));
        }

        public WfClientProcessInfo Withdraw(string processID, WfClientRuntimeContext runtimeContext)
        {
            return this.SingleCall(action => action.Withdraw(processID, runtimeContext));
        }

        public WfClientProcess GetProcessByID(string processID, WfClientUser user)
        {
            return this.GetProcessByID(processID, user, WfClientProcessInfoFilter.Default);
        }

        public WfClientProcess GetProcessByID(string processID, WfClientUser user, WfClientProcessInfoFilter filter)
        {
            return this.SingleCall(action => action.GetProcessByID(processID, user, filter));
        }

        public WfClientProcess GetProcessByActivityID(string activityID, WfClientUser user)
        {
            return this.GetProcessByActivityID(activityID, user, WfClientProcessInfoFilter.Default);
        }

        public WfClientProcess GetProcessByActivityID(string activityID, WfClientUser user, WfClientProcessInfoFilter filter)
        {
            return this.SingleCall(action => action.GetProcessByActivityID(activityID, user, filter));
        }

        public WfClientProcessInfo GetProcessInfoByID(string processID, WfClientUser user)
        {
            return this.SingleCall(action => action.GetProcessInfoByID(processID, user));
        }

        public WfClientProcessInfo GetProcessInfoByActivityID(string activityID, WfClientUser user)
        {
            return this.SingleCall(action => action.GetProcessInfoByActivityID(activityID, user));
        }

        public WfClientProcessInfoCollection GetProcessInfoByResourceID(string resourceID, WfClientUser user)
        {
            List<WfClientProcessInfo> processes = this.SingleCall(action => action.GetProcessesInfoByResourceID(resourceID, user));

            WfClientProcessInfoCollection result = new WfClientProcessInfoCollection();

            result.CopyFrom(processes);

            return result;
        }

        public WfClientProcessCollection GetProcessByResourceID(string resourceID, WfClientUser user)
        {
            return this.GetProcessByResourceID(resourceID, user, WfClientProcessInfoFilter.Default);
        }

        public WfClientProcessCollection GetProcessByResourceID(string resourceID, WfClientUser user, WfClientProcessInfoFilter filter)
        {
            List<WfClientProcess> processes = this.SingleCall(action => action.GetProcessesByResourceID(resourceID, user, filter));

            WfClientProcessCollection result = new WfClientProcessCollection();

            result.CopyFrom(processes);

            return result;
        }

        public WfClientOpinionCollection GetOpinionsByResourceID(string resourceID)
        {
            return this.SingleCall(action => new WfClientOpinionCollection(action.GetOpinionsByResourceID(resourceID)));
        }

        public WfClientOpinionCollection GetOpinionsByProcessID(string processID)
        {
            return this.SingleCall(action => new WfClientOpinionCollection(action.GetOpinionsByProcessID(processID)));
        }

        public T GetApplicationRuntimeParameters<T>(string processID, string key, WfClientProbeApplicationRuntimeParameterMode probeMode, T defaultValue)
        {
            return this.SingleCallWithScalar(action => action.GetApplicationRuntimeParameters(processID, key, probeMode), defaultValue);
        }

        public WfClientProcessCurrentInfoPageQueryResult QueryBranchProcesses(string ownerActivityID, string ownerTemplateKey, int startRowIndex, int maximumRows, string orderBy, int totalCount)
        {
            return this.SingleCall(action => action.QueryBranchProcesses(ownerActivityID, ownerTemplateKey, startRowIndex, maximumRows, orderBy, totalCount));
        }

        public WfClientProcessCurrentInfoPageQueryResult QueryProcesses(WfClientProcessQueryCondition condition, int startRowIndex, int maximumRows, string orderBy, int totalCount)
        {
            return this.SingleCall(action => action.QueryProcesses(condition, startRowIndex, maximumRows, orderBy, totalCount));
        }

        protected override WfClientChannelFactory<IWfClientProcessRuntimeService> GetService()
        {
            EndpointAddress endPoint = new EndpointAddress(WfContractsProxySettings.GetConfig().ProcessRuntimeServiceUrl.ToString());

            return new WfClientChannelFactory<IWfClientProcessRuntimeService>(endPoint);
        }
    }
}
