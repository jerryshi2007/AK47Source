using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Converters.PropertyDefine;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Descriptors
{
    public abstract class WfClientKeyedDescriptorConverterBase<TClient, TServer>
        where TClient : WfClientKeyedDescriptorBase
        where TServer : WfKeyedDescriptorBase
    {
        public virtual void ClientToServer(TClient client, ref TServer server)
        {
            server.NullCheck("client");
            server.NullCheck("server");

            ClientPropertyValueCollectionConverter.Instance.ClientToServer(client.Properties, server.Properties);
        }

        public virtual void ServerToClient(TServer server, ref TClient client)
        {
            server.NullCheck("client");
            server.NullCheck("server");

            ClientPropertyValueCollectionConverter.Instance.ServerToClient(server.Properties, client.Properties);
        }
    }
}
