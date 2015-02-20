using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Converters.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Runtime
{
    public class WfClientProcessConverter
    {
        private readonly WfClientProcessInfoFilter _Filter = WfClientProcessInfoFilter.Descriptor;

        public static readonly WfClientProcessConverter Instance = new WfClientProcessConverter(WfClientProcessInfoFilter.Descriptor | WfClientProcessInfoFilter.BindActivityDescriptors);
        public static readonly WfClientProcessConverter InstanceWithoutActivityBindings = new WfClientProcessConverter(WfClientProcessInfoFilter.Descriptor);
        public static readonly WfClientProcessConverter InstanceAllInfo = new WfClientProcessConverter(WfClientProcessInfoFilter.All);

        private WfClientProcessConverter()
        {
        }

        public WfClientProcessConverter(WfClientProcessInfoFilter filter)
        {
            this._Filter = filter;
        }

        public WfClientProcess ServerToClient(IWfProcess process, ref WfClientProcess client)
        {
            WfClientProcessDescriptor clientDescriptor = null;
            WfClientProcessDescriptor clientMainStream = null;

            if ((this._Filter & WfClientProcessInfoFilter.Descriptor) != WfClientProcessInfoFilter.InstanceOnly)
                WfClientProcessDescriptorConverter.Instance.ServerToClient((WfProcessDescriptor)process.Descriptor, ref clientDescriptor);

            if ((this._Filter & WfClientProcessInfoFilter.MainStream) != WfClientProcessInfoFilter.InstanceOnly)
                WfClientProcessDescriptorConverter.Instance.ServerToClient((WfProcessDescriptor)process.MainStream, ref clientMainStream);

            if (client == null)
                client = new WfClientProcess(clientDescriptor, clientMainStream);

            WfClientProcessInfoBaseConverter.Instance.ServerToClient(process, client);

            ServerActivitiesToClient(process, client);

            if ((this._Filter & WfClientProcessInfoFilter.BindActivityDescriptors) != WfClientProcessInfoFilter.InstanceOnly)
                client.NormalizeActivities();

            return client;
        }

        private static void ServerActivitiesToClient(IWfProcess server, WfClientProcess client)
        {
            foreach (IWfActivity activity in server.Activities)
            {
                WfClientActivity clientActivity = null;

                WfClientActivityConverter.WithoutDescriptorInstance.ServerToClient(activity, ref clientActivity);

                client.Activities.Add(clientActivity);
            }
        }
    }
}
