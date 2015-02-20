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
    public class WfClientGroupResourceDescriptorConverter : WfClientResourceDescriptorConverterBase
    {
        public static readonly WfClientGroupResourceDescriptorConverter Instance = new WfClientGroupResourceDescriptorConverter();

        private WfClientGroupResourceDescriptorConverter()
        {
        }

        public override void ClientToServer(WfClientResourceDescriptor client, ref WfResourceDescriptor server)
        {
            if (server == null)
                server = new WfGroupResourceDescriptor((IGroup)((WfClientGroupResourceDescriptor)client).Group.ToOguObject());
            else
                ((WfGroupResourceDescriptor)server).Group = (IGroup)((WfClientGroupResourceDescriptor)client).Group.ToOguObject();
        }

        public override void ServerToClient(WfResourceDescriptor server, ref WfClientResourceDescriptor client)
        {
            if (client == null)
                client = new WfClientGroupResourceDescriptor((WfClientGroup)((WfGroupResourceDescriptor)server).Group.ToClientOguObject());
            else
                ((WfClientGroupResourceDescriptor)client).Group = (WfClientGroup)((WfGroupResourceDescriptor)server).Group.ToClientOguObject();
        }
    }
}
