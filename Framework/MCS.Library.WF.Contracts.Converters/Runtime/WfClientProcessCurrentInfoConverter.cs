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
    /// <summary>
    /// WfProcessCurrentInfo向WfClientProcessCurrentInfo转换
    /// </summary>
    public class WfClientProcessCurrentInfoConverter
    {
        public static readonly WfClientProcessCurrentInfoConverter Instance = new WfClientProcessCurrentInfoConverter();

        private WfClientProcessCurrentInfoConverter()
        {
        }

        public WfProcessCurrentInfo ClientToServer(WfClientProcessCurrentInfo client, ref WfProcessCurrentInfo server)
        {
            client.NullCheck("client");

            if (server == null)
                server = new WfProcessCurrentInfo();

            server.InstanceID = client.InstanceID;
            server.ResourceID = client.ResourceID;

            server.ApplicationName = client.ApplicationName;
            server.ProgramName = client.ProgramName;
            server.ProcessName = client.ProcessName;
            server.DescriptorKey = client.DescriptorKey;
            server.OwnerActivityID = client.OwnerActivityID;
            server.OwnerTemplateKey = client.OwnerTemplateKey;
            server.CurrentActivityID = client.CurrentActivityID;

            server.Sequence = client.Sequence;
            server.Committed = client.Committed;

            server.CreateTime = client.CreateTime;
            server.StartTime = client.StartTime;
            server.EndTime = client.EndTime;
            server.Creator = (IUser)client.Creator.ToOguObject();
            server.Department = (IOrganization)client.Department.ToOguObject();
            server.Status = client.Status.ToProcessStatus();
            server.UpdateTag = client.UpdateTag;

            return server;
        }

        public WfClientProcessCurrentInfo ServerToClient(WfProcessCurrentInfo server, ref WfClientProcessCurrentInfo client)
        {
            server.NullCheck("server");

            if (client == null)
                client = new WfClientProcessCurrentInfo();

            client.InstanceID = server.InstanceID;
            client.ResourceID = server.ResourceID;

            client.ApplicationName = server.ApplicationName;
            client.ProgramName = server.ProgramName;
            client.ProcessName = server.ProcessName;
            client.DescriptorKey = server.DescriptorKey;
            client.OwnerActivityID = server.OwnerActivityID;
            client.OwnerTemplateKey = server.OwnerTemplateKey;
            client.CurrentActivityID = server.CurrentActivityID;

            client.Sequence = server.Sequence;
            client.Committed = server.Committed;

            client.CreateTime = server.CreateTime;
            client.StartTime = server.StartTime;
            client.EndTime = server.EndTime;
            client.Creator = (WfClientUser)server.Creator.ToClientOguObject();
            client.Department = (WfClientOrganization)server.Department.ToClientOguObject();
            client.Status = server.Status.ToClientProcessStatus();
            client.UpdateTag = server.UpdateTag;

            return client;
        }

        public void ServerToClient(WfProcessCurrentInfoCollection server, WfClientProcessCurrentInfoCollection client)
        {
            server.NullCheck("server");
            client.NullCheck("client");

            client.Clear();

            foreach (WfProcessCurrentInfo processInfo in server)
            {
                WfClientProcessCurrentInfo clientInfo = null;

                this.ServerToClient(processInfo, ref clientInfo);

                client.Add(clientInfo);
            }
        }
    }
}
