using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Descriptors
{
    public class WfClientProcessDescriptorInfoConverter
    {
        public static readonly WfClientProcessDescriptorInfoConverter Instance = new WfClientProcessDescriptorInfoConverter();

        private WfClientProcessDescriptorInfoConverter()
        {
        }

        public WfClientProcessDescriptorInfo ServerToClient(WfProcessDescriptorInfo server, ref WfClientProcessDescriptorInfo client)
        {
            server.NullCheck("server");

            if (client == null)
                client = new WfClientProcessDescriptorInfo();

            client.ProcessKey = server.ProcessKey;
            client.ApplicationName = server.ApplicationName;
            client.ProgramName = server.ProgramName;
            client.ProcessName = server.ProcessName;
            client.CreateTime = server.CreateTime;
            client.Creator = (WfClientUser)server.Creator.ToClientOguObject();
            client.Modifier = (WfClientUser)server.Modifier.ToClientOguObject();
            client.ModifyTime = server.ModifyTime;
            client.Data = server.Data;
            client.Enabled = server.Enabled;
            client.ImportTime = server.ImportTime;
            client.ImportUser = (WfClientUser)server.ImportUser.ToClientOguObject();

            return client;
        }

        public WfProcessDescriptorInfo ClientToServer(WfClientProcessDescriptorInfo client, ref WfProcessDescriptorInfo server)
        {
            client.NullCheck("client");

            if (server == null)
                server = new WfProcessDescriptorInfo();

            server.ProcessKey = client.ProcessKey;
            server.ApplicationName = client.ApplicationName;
            server.ProgramName = client.ProgramName;
            server.ProcessName = client.ProcessName;
            server.CreateTime = client.CreateTime;
            server.Creator = (IUser)client.Creator.ToOguObject();
            server.Modifier = (IUser)client.Modifier.ToOguObject();
            server.ModifyTime = client.ModifyTime;
            server.Data = client.Data;
            server.Enabled = client.Enabled;
            server.ImportTime = client.ImportTime;
            server.ImportUser = (IUser)client.ImportUser.ToOguObject();

            return server;
        }

        public void ServerToClient(WfProcessDescriptorInfoCollection server, WfClientProcessDescriptorInfoCollection client)
        {
            server.NullCheck("server");
            client.NullCheck("client");

            client.Clear();

            foreach (WfProcessDescriptorInfo processInfo in server)
            {
                WfClientProcessDescriptorInfo clientInfo = null;

                this.ServerToClient(processInfo, ref clientInfo);

                client.Add(clientInfo);
            }
        }
    }
}
