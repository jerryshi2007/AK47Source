using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Runtime
{
    public class WfClientBranchProcessStartupParamsConverter
    {
        public static readonly WfClientBranchProcessStartupParamsConverter Instance = new WfClientBranchProcessStartupParamsConverter();

        private WfClientBranchProcessStartupParamsConverter()
        {
        }

        public WfBranchProcessStartupParams ClientToServer(WfClientBranchProcessStartupParams client, ref WfBranchProcessStartupParams server)
        {
            client.NullCheck("client");

            if (server == null)
                server = new WfBranchProcessStartupParams();

            Dictionary<string, object> appParams = server.ApplicationRuntimeParameters;

            client.ApplicationRuntimeParameters.ForEach(kp => appParams[kp.Key] = kp.Value);

            server.DefaultTaskTitle = client.DefaultTaskTitle;
            server.ResourceID = client.ResourceID;
            server.Department = (IOrganization)client.Department.ToOguObject();
            server.StartupContext = client.StartupContext;
            server.RelativeParams.CopyFrom(client.RelativeParams);

            WfClientAssigneeCollectionConverter.Instance.ClientToServer(client.Assignees, server.Assignees);

            return server;
        }

        public WfClientBranchProcessStartupParams ServerToClient(WfBranchProcessStartupParams server, ref WfClientBranchProcessStartupParams client)
        {
            server.NullCheck("server");

            if (client == null)
                client = new WfClientBranchProcessStartupParams();

			Dictionary<string, object> appParams = server.ApplicationRuntimeParameters;

            client.ApplicationRuntimeParameters.ForEach(kp => appParams[kp.Key] = kp.Value);

            client.DefaultTaskTitle = server.DefaultTaskTitle;
            client.ResourceID = server.ResourceID;

            client.Department = (WfClientOrganization)server.Department.ToClientOguObject();
            client.RelativeParams.CopyFrom(server.RelativeParams);
            client.StartupContext = client.StartupContext;

            WfClientAssigneeCollectionConverter.Instance.ServerToClient(server.Assignees, client.Assignees);

            return client;
        }
    }

    public class WfClientBranchProcessStartupParamsCollectionConverter
    {
        public static readonly WfClientBranchProcessStartupParamsCollectionConverter Instance = new WfClientBranchProcessStartupParamsCollectionConverter();

        private WfClientBranchProcessStartupParamsCollectionConverter()
        {
        }

        public void ClientToServer(IEnumerable<WfClientBranchProcessStartupParams> client, ICollection<WfBranchProcessStartupParams> server)
        {
            client.NullCheck("client");

            foreach (WfClientBranchProcessStartupParams ct in client)
            {
                WfBranchProcessStartupParams st = null;

                WfClientBranchProcessStartupParamsConverter.Instance.ClientToServer(ct, ref st);

                server.Add(st);
            }
        }

        public void ServerToClient(IEnumerable<WfBranchProcessStartupParams> server, ICollection<WfClientBranchProcessStartupParams> client)
        {
            server.NullCheck("server");

            foreach (WfBranchProcessStartupParams st in server)
            {
                WfClientBranchProcessStartupParams ct = null;

                WfClientBranchProcessStartupParamsConverter.Instance.ServerToClient(st, ref ct);

                client.Add(ct);
            }
        }
    }
}
