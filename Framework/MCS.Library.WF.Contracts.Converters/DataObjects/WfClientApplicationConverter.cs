using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.DataObjects
{
    public class WfClientApplicationConverter
    {
        public static readonly WfClientApplicationConverter Instance = new WfClientApplicationConverter();

        private WfClientApplicationConverter()
        {
        }

        public WfClientApplication ServerToClient(WfApplication server, ref WfClientApplication client)
        {
            server.NullCheck("server");

            if (client == null)
                client = new WfClientApplication();

            client.CodeName = server.CodeName;
            client.Name = server.Name;
            client.Sort = server.Sort;

            return client;
        }

        public WfClientApplicationCollection ServerToClient(WfApplicationCollection server)
        {
            server.NullCheck("server");

            WfClientApplicationCollection client = new WfClientApplicationCollection();

            foreach (WfApplication serverItem in server)
            {
                WfClientApplication clientItem = null;

                this.ServerToClient(serverItem, ref clientItem);

                client.Add(clientItem);
            }

            return client;
        }
    }
}
