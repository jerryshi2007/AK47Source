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
    public class WfClientOrganizationResourceDescriptorConverter : WfClientResourceDescriptorConverterBase
    {
        public static readonly WfClientOrganizationResourceDescriptorConverter Instance = new WfClientOrganizationResourceDescriptorConverter();

        private WfClientOrganizationResourceDescriptorConverter()
        {
        }

        public override void ClientToServer(WfClientResourceDescriptor client, ref WfResourceDescriptor server)
        {
            if (server == null)
                server = new WfDepartmentResourceDescriptor((IOrganization)((WfClientDepartmentResourceDescriptor)client).Department.ToOguObject());
            else
                ((WfDepartmentResourceDescriptor)server).Department = (IOrganization)((WfClientDepartmentResourceDescriptor)client).Department.ToOguObject();
        }

        public override void ServerToClient(WfResourceDescriptor server, ref WfClientResourceDescriptor client)
        {
            if (client == null)
                client = new WfClientDepartmentResourceDescriptor((WfClientOrganization)((WfDepartmentResourceDescriptor)server).Department.ToClientOguObject());
            else
                ((WfClientDepartmentResourceDescriptor)client).Department = (WfClientOrganization)((WfDepartmentResourceDescriptor)server).Department.ToClientOguObject();
        }
    }
}
