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
    [Obsolete("暂时不实现，服务器端接口有问题")]
    public class WfClientAURoleResourceDescriptorConverter : WfClientResourceDescriptorConverterBase
    {
        public override void ClientToServer(WfClientResourceDescriptor client, ref WfResourceDescriptor server)
        {
            //client.NullCheck("client");

            //WfClientAURoleResourceDescriptor clientAURes = (WfClientAURoleResourceDescriptor)client;

            ////这里覆盖传入的对象
            //server = new WfAURoleResourceDescriptor(clientAURes.FullCodeName);
        }

        public override void ServerToClient(WfResourceDescriptor server, ref WfClientResourceDescriptor client)
        {
            //server.NullCheck("server");

            //WfAURoleResourceDescriptor serverAURes = (WfAURoleResourceDescriptor)server;

            //if (serverAURes.AUSchemaRole != null)
            //    client = new WfClientAURoleResourceDescriptor(serverAURes.AUSchemaRole.)
            //client = new WfClientAURoleResourceDescriptor(serverAURes.RoleFullCodeName);
        }
    }
}
