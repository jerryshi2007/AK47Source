using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Runtime
{
    public class WfClientTransferParamsConverter
    {
        public static readonly WfClientTransferParamsConverter Instance = new WfClientTransferParamsConverter();

        private WfClientTransferParamsConverter()
        {
        }

        public WfTransferParams ClientToServer(WfClientTransferParams client, IWfProcess process, ref WfTransferParams server)
        {
            client.NullCheck("client");

            if (server == null)
                server = new WfTransferParams();

            if (process != null)
            {
                if (client.NextActivityDescriptorKey.IsNotEmpty())
                {
                    IWfActivity nextActivity = process.Activities.FindActivityByDescriptorKey(client.NextActivityDescriptorKey);

                    if (nextActivity != null)
                        server.NextActivityDescriptor = nextActivity.Descriptor;
                }

                if (client.FromTransitionDescriptorKey.IsNotEmpty())
                {
                    IWfTransitionDescriptor fromTransition = process.Descriptor.FindTransitionByKey(client.FromTransitionDescriptorKey);

                    server.FromTransitionDescriptor = fromTransition;
                }
            }

            server.Operator = (IUser)client.Operator.ToOguObject();
            WfClientBranchProcessTransferParamsCollectionConverter.Instance.ClientToServer(client.BranchTransferParams, server.BranchTransferParams);
            WfClientAssigneeCollectionConverter.Instance.ClientToServer(client.Assignees, server.Assignees);

            return server;
        }
    }
}
