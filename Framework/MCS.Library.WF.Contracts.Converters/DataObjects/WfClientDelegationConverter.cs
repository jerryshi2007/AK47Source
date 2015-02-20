using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Ogu;

namespace MCS.Library.WF.Contracts.Converters.DataObjects
{
    public class WfClientDelegationConverter
    {
        public static readonly WfClientDelegationConverter Instance = new WfClientDelegationConverter();

        private WfClientDelegationConverter()
        {
        }

        public WfDelegation ClientToServer(WfClientDelegation client, ref WfDelegation server)
        {
            client.NullCheck("client");

            if (server == null)
                server = new WfDelegation();

            server.SourceUserID = client.SourceUserID;
            server.DestinationUserID = client.DestinationUserID;
            server.DestinationUserName = client.DestinationUserName;
            server.SourceUserName = client.SourceUserName;
            server.StartTime = client.StartTime;
            server.EndTime = client.EndTime;
            server.Enabled = client.Enabled;

            return server;
        }

        public List<WfClientDelegation> ServerToClient(WfDelegationCollection server)
        {
            List<WfClientDelegation> client = new List<WfClientDelegation>();

            foreach (WfDelegation opinion in server)
            {
                WfClientDelegation clientOpinion = null;

                ServerToClient(opinion, ref clientOpinion);

                client.Add(clientOpinion);
            }

            return client;
        }

        public WfClientDelegation ServerToClient(WfDelegation server, ref WfClientDelegation client)
        {
            server.NullCheck("server");

            if (client == null)
                client = new WfClientDelegation();

            client.SourceUserID = server.SourceUserID;
            client.DestinationUserID = server.DestinationUserID;
            client.DestinationUserName = server.DestinationUserName;
            client.SourceUserName = server.SourceUserName;
            client.StartTime = server.StartTime;
            client.EndTime = server.EndTime;
            client.Enabled = server.Enabled;

            return client;
        }
    }
}
