using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Converters.Runtime;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Descriptors
{
    public class WfClientMainStreamActivityDescriptorConverter
    {
        public static readonly WfClientMainStreamActivityDescriptorConverter Instance = new WfClientMainStreamActivityDescriptorConverter();

        private WfClientMainStreamActivityDescriptorConverter()
        {
        }

        public void ServerToClient(IWfMainStreamActivityDescriptor server, ref WfClientMainStreamActivityDescriptor client)
        {
            server.NullCheck("server");

            WfClientActivityDescriptor clientActDesp = null;

            WfClientActivityDescriptorConverter.Instance.ServerToClient((WfActivityDescriptor)server.Activity, ref clientActDesp);

            if (client == null)
                client = new WfClientMainStreamActivityDescriptor(clientActDesp);

            client.Level = server.Level;

            foreach (WfActivityDescriptor actDesp in server.AssociatedActivities)
            {
                WfClientActivityDescriptor cad = null;

                WfClientActivityDescriptorConverter.Instance.ServerToClient(actDesp, ref cad);

                client.AssociatedActivities.Add(cad);
            }

            FillStatusAndAssignees(client, server);
        }

        private static void FillStatusAndAssignees(WfClientMainStreamActivityDescriptor client, IWfMainStreamActivityDescriptor server)
        {
            IWfActivity actInstance = server.Activity.ProcessInstance.Activities.
                FindActivityByDescriptorKey(server.Activity.Key);

            if (actInstance != null)
            {
                client.Status = actInstance.Status.ToClientActivityStatus();
                client.Elapsed = server.Elapsed;
                client.ActivityInstanceID = actInstance.ID;
                client.Operator = (WfClientUser)actInstance.Operator.ToClientOguObject();
                client.BranchProcessGroupsCount = actInstance.BranchProcessGroups.Count;

                client.FromTransitionDescriptor = ConvertServerTransitionToClient(actInstance.FromTransitionDescriptor);
                client.ToTransitionDescriptor = ConvertServerTransitionToClient(actInstance.ToTransitionDescriptor);

                if (actInstance.Assignees.Count != 0)
                {
                    WfClientAssigneeCollectionConverter.Instance.ServerToClient(actInstance.Assignees, client.Assignees);
                }
                else
                {
                    WfAssigneeCollection candidates = actInstance.Candidates.GetSelectedAssignees();

                    WfClientAssigneeCollectionConverter.Instance.ServerToClient(candidates, client.Assignees);
                }
            }
        }

        private static WfClientTransitionDescriptor ConvertServerTransitionToClient(IWfTransitionDescriptor server)
        {
            WfClientTransitionDescriptor clientTransition = null;

            if (server != null)
                WfClientTransitionDescriptorConverter.Instance.ServerToClient((WfTransitionDescriptor)server, ref clientTransition);

            return clientTransition;
        }
    }

    public class WfClientMainStreamActivityDescriptorCollectionConverter
    {
        public static readonly WfClientMainStreamActivityDescriptorCollectionConverter Instance = new WfClientMainStreamActivityDescriptorCollectionConverter();

        private WfClientMainStreamActivityDescriptorCollectionConverter()
        {
        }

        public void ServerToClient(IEnumerable<IWfMainStreamActivityDescriptor> server, WfClientMainStreamActivityDescriptorCollection client)
        {
            server.NullCheck("server");
            client.NullCheck("client");

            client.Clear();

            foreach (IWfMainStreamActivityDescriptor msActDesp in server)
            {
                WfClientMainStreamActivityDescriptor clientMSActDesp = null;

                WfClientMainStreamActivityDescriptorConverter.Instance.ServerToClient(msActDesp, ref clientMSActDesp);

                client.Add(clientMSActDesp);
            }
        }
    }
}
