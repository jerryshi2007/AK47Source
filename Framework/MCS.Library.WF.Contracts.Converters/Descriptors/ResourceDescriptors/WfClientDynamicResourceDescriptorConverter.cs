using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Descriptors
{
    public class WfClientDynamicResourceDescriptorConverter : WfClientResourceDescriptorConverterBase
    {
        public static readonly WfClientDynamicResourceDescriptorConverter Instance = new WfClientDynamicResourceDescriptorConverter();

        private WfClientDynamicResourceDescriptorConverter()
        {
        }

        public override void ClientToServer(WfClientResourceDescriptor client, ref WfResourceDescriptor server)
        {
            client.NullCheck("client");

            WfClientDynamicResourceDescriptor clientDynRes = (WfClientDynamicResourceDescriptor)client;

            if (server == null)
                server = new WfDynamicResourceDescriptor(clientDynRes.Name, clientDynRes.Condition.Expression);
            else
            {
                WfDynamicResourceDescriptor serverDynRes = (WfDynamicResourceDescriptor)server;

                serverDynRes.Name = clientDynRes.Name;
                serverDynRes.Condition.Expression = clientDynRes.Condition.Expression;
            }
        }

        public override void ServerToClient(WfResourceDescriptor server, ref WfClientResourceDescriptor client)
        {
            server.NullCheck("server");

            WfDynamicResourceDescriptor serverDynRes = (WfDynamicResourceDescriptor)server;

            if (client == null)
                client = new WfClientDynamicResourceDescriptor(serverDynRes.Name, serverDynRes.Condition.Expression);
            else
            {
                WfClientDynamicResourceDescriptor clientDynRes = (WfClientDynamicResourceDescriptor)client;

                clientDynRes.Name = serverDynRes.Name;
                clientDynRes.Condition.Expression = serverDynRes.Condition.Expression;
            }
        }
    }
}
