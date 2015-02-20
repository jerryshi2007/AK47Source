using MCS.Library.Core;
using MCS.Library.WcfExtensions;
using MCS.Library.WF.Contracts.Operations;
using MCS.Library.WF.Contracts.Proxies.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Proxies
{
    public static class WfClientFactory
    {
        public static WfClientChannelFactory<IWfClientProcessDescriptorService> GetProcessDescriptorService()
        {
            EndpointAddress endPoint = new EndpointAddress(WfContractsProxySettings.GetConfig().ProcessDescriptorServiceUrl.ToString());

            return new WfClientChannelFactory<IWfClientProcessDescriptorService>(endPoint);
        }

        public static WfClientChannelFactory<IWfClientProcessRuntimeService> GetProcessRuntimeService()
        {
            EndpointAddress endPoint = new EndpointAddress(WfContractsProxySettings.GetConfig().ProcessRuntimeServiceUrl.ToString());

            return new WfClientChannelFactory<IWfClientProcessRuntimeService>(endPoint);
        }
    }
}
