using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Descriptors
{
    public class WfClientUserResourceDescriptorConverter : WfClientResourceDescriptorConverterBase
    {
        public static readonly WfClientUserResourceDescriptorConverter Instance = new WfClientUserResourceDescriptorConverter();

        private WfClientUserResourceDescriptorConverter()
        {
        }

        public override void ClientToServer(WfClientResourceDescriptor client, ref WfResourceDescriptor server)
        {
            if (server == null)
                server = new WfUserResourceDescriptor((IUser)((WfClientUserResourceDescriptor)client).User.ToOguObject());
            else
                ((WfUserResourceDescriptor)server).User = (IUser)((WfClientUserResourceDescriptor)client).User.ToOguObject();
        }

        public override void ServerToClient(WfResourceDescriptor server, ref WfClientResourceDescriptor client)
        {
            if (client == null)
                client = new WfClientUserResourceDescriptor((WfClientUser)((WfUserResourceDescriptor)server).User.ToClientOguObject());
            else
                ((WfClientUserResourceDescriptor)client).User = (WfClientUser)((WfUserResourceDescriptor)server).User.ToClientOguObject();
        }
    }
}
