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
    public class WfClientTransitionDescriptorConverter : WfClientKeyedDescriptorConverterBase<WfClientTransitionDescriptor, WfTransitionDescriptor>
    {
        public static readonly WfClientTransitionDescriptorConverter Instance = new WfClientTransitionDescriptorConverter();

        private WfClientTransitionDescriptorConverter()
        {
        }

        public override void ClientToServer(WfClientTransitionDescriptor client, ref WfTransitionDescriptor server)
        {
            client.NullCheck("client");

            if (server == null)
                server = new WfForwardTransitionDescriptor(client.Key);

            base.ClientToServer(client, ref server);

            server.ToActivityKey = client.ToActivityKey;
            server.FromActivityKey = client.FromActivityKey;

            WfConditionDescriptor serverCondition = server.Condition;
            WfClientConditionDescriptorConverter.Instance.ClientToServer(client.Condition, ref serverCondition);
            server.Condition = serverCondition;

            WfClientVariableDescriptorCollectionConverter.Instance.ClientToServer(client.Variables, server.Variables);
        }

        public override void ServerToClient(WfTransitionDescriptor server, ref WfClientTransitionDescriptor client)
        {
            server.NullCheck("server");

            if (client == null)
                client = new WfClientTransitionDescriptor(server.Key);

            base.ServerToClient(server, ref client);

            client.ToActivityKey = server.ToActivityKey;
            client.FromActivityKey = server.FromActivityKey;

            WfClientConditionDescriptor clientCondition = client.Condition;
            WfClientConditionDescriptorConverter.Instance.ServerToClient(server.Condition, ref clientCondition);
            client.Condition = clientCondition;

            WfClientVariableDescriptorCollectionConverter.Instance.ServerToClient(server.Variables, client.Variables);
        }
    }

    public class WfClientTransitionDescriptorCollectionConverter
    {
        public static readonly WfClientTransitionDescriptorCollectionConverter Instance = new WfClientTransitionDescriptorCollectionConverter();

        private WfClientTransitionDescriptorCollectionConverter()
        {
        }

        public void ClientToServer(IEnumerable<WfClientTransitionDescriptor> client, ICollection<IWfTransitionDescriptor> server)
        {
            client.NullCheck("client");

            foreach (WfClientTransitionDescriptor ct in client)
            {
                WfTransitionDescriptor st = null;

                WfClientTransitionDescriptorConverter.Instance.ClientToServer(ct, ref st);

                server.Add(st);
            }
        }

        public void ServerToClient(IEnumerable<IWfTransitionDescriptor> server, ICollection<WfClientTransitionDescriptor> client)
        {
            server.NullCheck("server");

            foreach (WfTransitionDescriptor st in server)
            {
                WfClientTransitionDescriptor ct = null;

                WfClientTransitionDescriptorConverter.Instance.ServerToClient(st, ref ct);

                client.Add(ct);
            }
        }
    }
}
