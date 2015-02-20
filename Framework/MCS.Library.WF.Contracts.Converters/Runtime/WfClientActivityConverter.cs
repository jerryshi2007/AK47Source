using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Converters.Descriptors;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Runtime
{
    public class WfClientActivityConverter
    {
        private readonly bool _ConvertDescriptor = true;

        public static readonly WfClientActivityConverter Instance = new WfClientActivityConverter();
        public static readonly WfClientActivityConverter WithoutDescriptorInstance = new WfClientActivityConverter(false);

        private WfClientActivityConverter()
        {
        }

        private WfClientActivityConverter(bool convertDescriptor)
        {
            this._ConvertDescriptor = convertDescriptor;
        }

        public WfClientActivity ServerToClient(IWfActivity server, ref WfClientActivity client)
        {
            if (server != null)
            {
                if (client == null)
                    client = new WfClientActivity();

                client.ID = server.ID;

                if (server.Descriptor != null)
                    client.DescriptorKey = server.Descriptor.Key;

                client.LoadingType = server.LoadingType.ToClientDataLoadingType();
                client.StartTime = server.StartTime;
                client.EndTime = server.EndTime;
                client.MainStreamActivityKey = server.MainStreamActivityKey;
                client.BranchProcessReturnValue = server.BranchProcessReturnValue.ToClientBranchProcessReturnType();
                client.BranchProcessGroupsCount = server.BranchProcessGroups.Count;
                client.Operator = (WfClientUser)server.Operator.ToClientOguObject();
                client.Status = server.Status.ToClientActivityStatus();

                WfClientAssigneeCollectionConverter.Instance.ServerToClient(server.Assignees, client.Assignees);
                WfClientAssigneeCollectionConverter.Instance.ServerToClient(server.Candidates, client.Candidates);

                if (this._ConvertDescriptor)
                {
                    WfClientActivityDescriptor clientActDesp = null;

                    WfClientActivityDescriptorConverter.Instance.ServerToClient((WfActivityDescriptor)server.Descriptor, ref clientActDesp);
                    client.Descriptor = clientActDesp;
                }
            }
            else
                client = null;

            return client;
        }
    }
}
