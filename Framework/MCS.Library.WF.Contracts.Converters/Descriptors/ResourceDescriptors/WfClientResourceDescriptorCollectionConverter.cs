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
    public class WfClientResourceDescriptorCollectionConverter
    {
        public static readonly WfClientResourceDescriptorCollectionConverter Instance = new WfClientResourceDescriptorCollectionConverter();

        private WfClientResourceDescriptorCollectionConverter()
        {
        }

        public void ClientToServer(WfClientResourceDescriptorCollection client, WfResourceDescriptorCollection server)
        {
            client.NullCheck("client");
            server.NullCheck("server");

            server.Clear();

            foreach (WfClientResourceDescriptor clientRes in client)
            {
                WfClientResourceDescriptorConverterBase converter = WfClientResourceDescriptorConverterFactory.Instance.GetConverterByClientType(clientRes.GetType());

                if (converter != null)
                {
                    WfResourceDescriptor serverRes = null;

                    converter.ClientToServer(clientRes, ref serverRes);

                    if (serverRes != null)
                        server.Add(serverRes);
                }
            }
        }

        public void ServerToClient(WfResourceDescriptorCollection server, WfClientResourceDescriptorCollection client)
        {
            client.NullCheck("client");
            server.NullCheck("server");

            client.Clear();

            foreach (WfResourceDescriptor serverRes in server)
            {
                WfClientResourceDescriptorConverterBase converter = WfClientResourceDescriptorConverterFactory.Instance.GetConverterByServerType(serverRes.GetType());

                if (converter != null)
                {
                    WfClientResourceDescriptor clientRes = null;

                    converter.ServerToClient(serverRes, ref clientRes);

                    if (clientRes != null)
                        client.Add(clientRes);
                }
            }
        }
    }
}
