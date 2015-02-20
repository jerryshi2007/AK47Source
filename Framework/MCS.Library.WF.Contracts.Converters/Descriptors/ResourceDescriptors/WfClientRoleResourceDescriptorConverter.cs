using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Descriptors
{
    public class WfClientRoleResourceDescriptorConverter : WfClientResourceDescriptorConverterBase
    {
        public static readonly WfClientRoleResourceDescriptorConverter Instance = new WfClientRoleResourceDescriptorConverter();

        private WfClientRoleResourceDescriptorConverter()
        {
        }

        public override void ClientToServer(WfClientResourceDescriptor client, ref WfResourceDescriptor server)
        {
            SOARole role = new SOARole(((WfClientRoleResourceDescriptor)client).FullCodeName);

            if (server == null)
                server = new WfRoleResourceDescriptor(role);
            else
                ((WfRoleResourceDescriptor)server).Role = role;
        }

        public override void ServerToClient(WfResourceDescriptor server, ref WfClientResourceDescriptor client)
        {
            WfRoleResourceDescriptor roleDesp = (WfRoleResourceDescriptor)server;

            if (roleDesp.Role != null)
            {
                if (client == null)
                    client = new WfClientRoleResourceDescriptor(roleDesp.Role.FullCodeName);
                else
                    ((WfClientRoleResourceDescriptor)client).FullCodeName = roleDesp.Role.FullCodeName;
            }
        }
    }
}
