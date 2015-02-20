using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Descriptors
{
    public class WfClientActivityResourceDescriptorConverterBase<TClient, TServer> : WfClientResourceDescriptorConverterBase
        where TClient : WfClientActivityResourceDescriptorBase, new()
        where TServer : WfActivityResourceDescriptorBase, new()
    {
        public WfClientActivityResourceDescriptorConverterBase()
        {
        }

        public override void ClientToServer(WfClientResourceDescriptor client, ref WfResourceDescriptor server)
        {
            if (server == null)
                server = new TServer();

            ((TServer)server).ActivityKey = ((TClient)client).ActivityKey;
        }

        public override void ServerToClient(WfResourceDescriptor server, ref WfClientResourceDescriptor client)
        {
            if (client == null)
                client = new TClient();

            ((TClient)client).ActivityKey = ((TServer)server).ActivityKey;
        }
    }

    public class WfClientActivityAssigneesResourceDescriptorConverter :
        WfClientActivityResourceDescriptorConverterBase<WfClientActivityAssigneesResourceDescriptor, WfActivityAssigneesResourceDescriptor>
    {
        public static readonly WfClientActivityAssigneesResourceDescriptorConverter Instance = new WfClientActivityAssigneesResourceDescriptorConverter();

        public WfClientActivityAssigneesResourceDescriptorConverter()
            : base()
        {
        }
    }

    public class WfClientActivityOperatorResourceDescriptorConverter : WfClientActivityResourceDescriptorConverterBase<WfClientActivityOperatorResourceDescriptor, WfActivityOperatorResourceDescriptor>
    {
        public static readonly WfClientActivityOperatorResourceDescriptorConverter Instance = new WfClientActivityOperatorResourceDescriptorConverter();

        public WfClientActivityOperatorResourceDescriptorConverter()
        {
        }
    }
}
