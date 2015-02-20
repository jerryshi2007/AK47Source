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
    public class WfClientBranchProcessTemplateDescriptorConverter : WfClientKeyedDescriptorConverterBase<WfClientBranchProcessTemplateDescriptor, WfBranchProcessTemplateDescriptor>
    {
        public static readonly WfClientBranchProcessTemplateDescriptorConverter Instance = new WfClientBranchProcessTemplateDescriptorConverter();

        private WfClientBranchProcessTemplateDescriptorConverter()
        {
        }

        public override void ClientToServer(WfClientBranchProcessTemplateDescriptor client, ref WfBranchProcessTemplateDescriptor server)
        {
            client.NullCheck("client");

            if (server == null)
                server = new WfBranchProcessTemplateDescriptor(client.Key);

            base.ClientToServer(client, ref server);

            WfConditionDescriptor serverCondition = server.Condition;
            WfClientConditionDescriptorConverter.Instance.ClientToServer(client.Condition, ref serverCondition);
            server.Condition = serverCondition;

            WfClientResourceDescriptorCollectionConverter.Instance.ClientToServer(client.Resources, server.Resources);
            WfClientResourceDescriptorCollectionConverter.Instance.ClientToServer(client.CancelSubProcessNotifier, server.CancelSubProcessNotifier);
            WfClientRelativeLinkDescriptorCollectionConverter.Instance.ClientToServer(client.RelativeLinks, server.RelativeLinks);
        }

        public override void ServerToClient(WfBranchProcessTemplateDescriptor server, ref WfClientBranchProcessTemplateDescriptor client)
        {
            server.NullCheck("server");

            if (client == null)
                client = new WfClientBranchProcessTemplateDescriptor(server.Key);

            base.ServerToClient(server, ref client);

            WfClientConditionDescriptor clientCondition = client.Condition;
            WfClientConditionDescriptorConverter.Instance.ServerToClient(server.Condition, ref clientCondition);
            client.Condition = clientCondition;

            WfClientResourceDescriptorCollectionConverter.Instance.ServerToClient(server.Resources, client.Resources);
            WfClientResourceDescriptorCollectionConverter.Instance.ServerToClient(server.CancelSubProcessNotifier, client.CancelSubProcessNotifier);
            WfClientRelativeLinkDescriptorCollectionConverter.Instance.ServerToClient(server.RelativeLinks, client.RelativeLinks);
        }
    }

    public class WfClientBranchProcessTemplateDescriptorCollectionConverter
    {
        public static readonly WfClientBranchProcessTemplateDescriptorCollectionConverter Instance = new WfClientBranchProcessTemplateDescriptorCollectionConverter();

        private WfClientBranchProcessTemplateDescriptorCollectionConverter()
        {
        }

        public void ClientToServer(WfClientBranchProcessTemplateCollection client, WfBranchProcessTemplateCollection server)
        {
            client.NullCheck("client");
            server.NullCheck("server");

            server.Clear();

            foreach (WfClientBranchProcessTemplateDescriptor ct in client)
            {
                WfBranchProcessTemplateDescriptor st = null;

                WfClientBranchProcessTemplateDescriptorConverter.Instance.ClientToServer(ct, ref st);

                server.Add(st);
            }
        }

        public void ServerToClient(WfBranchProcessTemplateCollection server, WfClientBranchProcessTemplateCollection client)
        {
            client.NullCheck("client");
            server.NullCheck("server");

            client.Clear();

            foreach (WfBranchProcessTemplateDescriptor st in server)
            {
                WfClientBranchProcessTemplateDescriptor ct = null;

                WfClientBranchProcessTemplateDescriptorConverter.Instance.ServerToClient(st, ref ct);

                client.Add(ct);
            }
        }
    }
}
