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
    public class WfClientProgramConverter
    {
        public static readonly WfClientProgramConverter Instance = new WfClientProgramConverter();

        private WfClientProgramConverter()
        {
        }

        public WfClientProgram ServerToClient(WfProgram server, ref WfClientProgram client)
        {
            server.NullCheck("server");

            if (client == null)
                client = new WfClientProgram();

            client.ApplicationCodeName = server.ApplicationCodeName;
            client.CodeName = server.CodeName;
            client.Name = server.Name;
            client.Sort = server.Sort;

            return client;
        }

        public WfClientProgramInApplicationCollection ServerToClient(WfProgramInApplicationCollection server)
        {
            server.NullCheck("server");

            WfClientProgramInApplicationCollection client = new WfClientProgramInApplicationCollection();

            foreach (WfProgram serverItem in server)
            {
                WfClientProgram clientItem = null;

                this.ServerToClient(serverItem, ref clientItem);

                client.Add(clientItem);
            }

            return client;
        }
    }
}
