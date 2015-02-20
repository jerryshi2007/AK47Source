using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Descriptors
{
    public class WfClientVariableDescriptorConverter : WfClientKeyedDescriptorConverterBase<WfClientVariableDescriptor, WfVariableDescriptor>
    {
        public static readonly WfClientVariableDescriptorConverter Instance = new WfClientVariableDescriptorConverter();

        private WfClientVariableDescriptorConverter()
        {
        }

        public override void ClientToServer(WfClientVariableDescriptor client, ref WfVariableDescriptor server)
        {
            if (server == null)
                server = new WfVariableDescriptor(client.Key);

            base.ClientToServer(client, ref server);

            server.OriginalType = client.OriginalType.ToVariableDataType();
            server.OriginalValue = client.OriginalValue;
        }

        public override void ServerToClient(WfVariableDescriptor server, ref WfClientVariableDescriptor client)
        {
            if (client == null)
                client = new WfClientVariableDescriptor(server.Key);

            base.ServerToClient(server, ref client);

            client.OriginalType = server.OriginalType.ToClientVariableDataType();
            client.OriginalValue = server.OriginalValue;
        }
    }

    public class WfClientVariableDescriptorCollectionConverter : WfClientKeyedDescriptorCollectionConverterBase<WfClientVariableDescriptor, WfVariableDescriptor>
    {
        public static readonly WfClientVariableDescriptorCollectionConverter Instance = new WfClientVariableDescriptorCollectionConverter();

        private WfClientVariableDescriptorCollectionConverter()
        {
        }

        protected override WfClientVariableDescriptor CreateClientItem()
        {
            return new WfClientVariableDescriptor();
        }

        protected override WfVariableDescriptor CreateServerItem()
        {
            return new WfVariableDescriptor();
        }

        protected override WfClientKeyedDescriptorConverterBase<WfClientVariableDescriptor, WfVariableDescriptor> GetItemConverter()
        {
            return WfClientVariableDescriptorConverter.Instance;
        }
    }
}
