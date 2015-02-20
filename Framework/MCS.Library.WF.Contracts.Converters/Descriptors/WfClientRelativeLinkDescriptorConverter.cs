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
    public class WfClientRelativeLinkDescriptorConverter : WfClientKeyedDescriptorConverterBase<WfClientRelativeLinkDescriptor, WfRelativeLinkDescriptor>
    {
        public static readonly WfClientRelativeLinkDescriptorConverter Instance = new WfClientRelativeLinkDescriptorConverter();

        private WfClientRelativeLinkDescriptorConverter()
        {
        }

        public override void ClientToServer(WfClientRelativeLinkDescriptor client, ref WfRelativeLinkDescriptor server)
        {
            if (server == null)
                server = new WfRelativeLinkDescriptor(client.Key);

            base.ClientToServer(client, ref server);

            server.Category = client.Category;
            server.Url = client.Url;
        }

        public override void ServerToClient(WfRelativeLinkDescriptor server, ref WfClientRelativeLinkDescriptor client)
        {
            if (client == null)
                client = new WfClientRelativeLinkDescriptor(server.Key);

            base.ServerToClient(server, ref client);

            client.Category = server.Category;
            client.Url = server.Url;
        }
    }

    public class WfClientRelativeLinkDescriptorCollectionConverter
    {
        public static readonly WfClientRelativeLinkDescriptorCollectionConverter Instance = new WfClientRelativeLinkDescriptorCollectionConverter();

        private WfClientRelativeLinkDescriptorCollectionConverter()
        {
        }

        public void ClientToServer(IEnumerable<WfClientRelativeLinkDescriptor> client, ICollection<IWfRelativeLinkDescriptor> server)
        {
            client.NullCheck("client");

            foreach (WfClientRelativeLinkDescriptor ct in client)
            {
                WfRelativeLinkDescriptor st = null;

                WfClientRelativeLinkDescriptorConverter.Instance.ClientToServer(ct, ref st);

                server.Add(st);
            }
        }

        public void ServerToClient(IEnumerable<IWfRelativeLinkDescriptor> server, ICollection<WfClientRelativeLinkDescriptor> client)
        {
            server.NullCheck("server");

            foreach (WfRelativeLinkDescriptor st in server)
            {
                WfClientRelativeLinkDescriptor ct = null;

                WfClientRelativeLinkDescriptorConverter.Instance.ServerToClient(st, ref ct);

                client.Add(ct);
            }
        }
    }
}
