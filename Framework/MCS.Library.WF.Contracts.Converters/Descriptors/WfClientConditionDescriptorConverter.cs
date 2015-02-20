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
    public class WfClientConditionDescriptorConverter
    {
        public static readonly WfClientConditionDescriptorConverter Instance = new WfClientConditionDescriptorConverter();

        private WfClientConditionDescriptorConverter()
        {
        }

        public void ClientToServer(WfClientConditionDescriptor client, ref WfConditionDescriptor server)
        {
            client.NullCheck("client");

            if (server == null)
                server = new WfConditionDescriptor(null);

            server.Expression = client.Expression;
        }

        public void ServerToClient(WfConditionDescriptor server, ref WfClientConditionDescriptor client)
        {
            server.NullCheck("server");

            if (client == null)
                client = new WfClientConditionDescriptor();

            client.Expression = server.Expression;
        }
    }
}
