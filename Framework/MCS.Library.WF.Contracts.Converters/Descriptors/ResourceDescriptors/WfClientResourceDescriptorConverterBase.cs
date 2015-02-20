using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Descriptors
{
    public abstract class WfClientResourceDescriptorConverterBase
    {
        public abstract void ClientToServer(WfClientResourceDescriptor client, ref WfResourceDescriptor server);

        public abstract void ServerToClient(WfResourceDescriptor server, ref WfClientResourceDescriptor client);
    }
}
