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
    public class WfClientActivityDescriptorConverter : WfClientKeyedDescriptorConverterBase<WfClientActivityDescriptor, WfActivityDescriptor>
    {
        public static readonly WfClientActivityDescriptorConverter Instance = new WfClientActivityDescriptorConverter();

        private WfClientActivityDescriptorConverter()
        {
        }

        public override void ClientToServer(WfClientActivityDescriptor client, ref WfActivityDescriptor server)
        {
            client.NullCheck("client");

            if (server == null)
                server = new WfActivityDescriptor(client.Key, client.ActivityType.ToActivityType());

            base.ClientToServer(client, ref server);

            WfConditionDescriptor serverCondition = server.Condition;
            WfClientConditionDescriptorConverter.Instance.ClientToServer(client.Condition, ref serverCondition);
            server.Condition = serverCondition;

            WfClientResourceDescriptorCollectionConverter.Instance.ClientToServer(client.Resources, server.Resources);
            WfClientResourceDescriptorCollectionConverter.Instance.ClientToServer(client.EnterEventReceivers, server.EnterEventReceivers);
            WfClientResourceDescriptorCollectionConverter.Instance.ClientToServer(client.LeaveEventReceivers, server.LeaveEventReceivers);
            WfClientVariableDescriptorCollectionConverter.Instance.ClientToServer(client.Variables, server.Variables);
            WfClientBranchProcessTemplateDescriptorCollectionConverter.Instance.ClientToServer(client.BranchProcessTemplates, server.BranchProcessTemplates);
            WfClientRelativeLinkDescriptorCollectionConverter.Instance.ClientToServer(client.RelativeLinks, server.RelativeLinks);
        }

        public override void ServerToClient(WfActivityDescriptor server, ref WfClientActivityDescriptor client)
        {
            server.NullCheck("server");

            if (client == null)
                client = new WfClientActivityDescriptor(server.ActivityType.ToClientActivityType());
            else
                client.ActivityType = server.ActivityType.ToClientActivityType();

            base.ServerToClient(server, ref client);

            WfClientConditionDescriptor clientCondition = client.Condition;
            WfClientConditionDescriptorConverter.Instance.ServerToClient(server.Condition, ref clientCondition);
            client.Condition = clientCondition;

            WfClientResourceDescriptorCollectionConverter.Instance.ServerToClient(server.Resources, client.Resources);
            WfClientResourceDescriptorCollectionConverter.Instance.ServerToClient(server.EnterEventReceivers, client.EnterEventReceivers);
            WfClientResourceDescriptorCollectionConverter.Instance.ServerToClient(server.LeaveEventReceivers, client.LeaveEventReceivers);
            WfClientVariableDescriptorCollectionConverter.Instance.ServerToClient(server.Variables, client.Variables);
            WfClientBranchProcessTemplateDescriptorCollectionConverter.Instance.ServerToClient(server.BranchProcessTemplates, client.BranchProcessTemplates);
            WfClientRelativeLinkDescriptorCollectionConverter.Instance.ServerToClient(server.RelativeLinks, client.RelativeLinks);
        }
    }

    //public class WfClientActivityDescriptorCollectionConverter : WfClientKeyedDescriptorCollectionConverterBase<WfClientActivityDescriptor, WfActivityDescriptor>
    //{
    //    public static readonly WfClientActivityDescriptorCollectionConverter Instance = new WfClientActivityDescriptorCollectionConverter();

    //    private WfClientActivityDescriptorCollectionConverter()
    //    {
    //    }

    //    protected override WfClientKeyedDescriptorConverterBase<WfClientActivityDescriptor, WfActivityDescriptor> GetItemConverter()
    //    {
    //        return WfClientActivityDescriptorConverter.Instance;
    //    }


    //}
}
