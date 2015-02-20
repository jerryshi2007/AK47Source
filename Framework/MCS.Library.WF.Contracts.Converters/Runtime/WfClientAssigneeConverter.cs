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
    public class WfClientAssigneeConverter
    {
        public static readonly WfClientAssigneeConverter Instance = new WfClientAssigneeConverter();

        private WfClientAssigneeConverter()
        {
        }

        public void ClientToServer(WfClientAssignee client, ref WfAssignee server)
        {
            client.NullCheck("client");

            if (server == null)
                server = new WfAssignee();

            server.AssigneeType = client.AssigneeType.ToAssigneeType();
            server.Delegator = (IUser)client.Delegator.ToOguObject();
            server.Selected = client.Selected;
            server.User = (IUser)client.User.ToOguObject();
            server.Url = client.Url;
        }

        public void ServerToClient(WfAssignee server, ref WfClientAssignee client)
        {
            server.NullCheck("server");

            if (client == null)
                client = new WfClientAssignee();

            client.AssigneeType = server.AssigneeType.ToClientAssigneeType();
            client.Delegator = (WfClientUser)server.Delegator.ToClientOguObject();
            client.Selected = server.Selected;
            client.User = (WfClientUser)server.User.ToClientOguObject();
            client.Url = server.Url;
        }
    }

    public class WfClientAssigneeCollectionConverter
    {
        public static readonly WfClientAssigneeCollectionConverter Instance = new WfClientAssigneeCollectionConverter();

        private WfClientAssigneeCollectionConverter()
        {
        }

        public void ClientToServer(WfClientAssigneeCollection client, WfAssigneeCollection server)
        {
            client.NullCheck("client");
            server.NullCheck("server");

            server.Clear();

            foreach (WfClientAssignee c in client)
            {
                WfAssignee s = null;

                WfClientAssigneeConverter.Instance.ClientToServer(c, ref s);

                server.Add(s);
            }
        }

        public void ServerToClient(WfAssigneeCollection server, WfClientAssigneeCollection client)
        {
            server.NullCheck("server");
            client.NullCheck("client");

            client.Clear();

            foreach (WfAssignee s in server)
            {
                WfClientAssignee c = null;

                WfClientAssigneeConverter.Instance.ServerToClient(s, ref c);

                client.Add(c);
            }
        }
    }
}
