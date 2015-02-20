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
    public class WfClientProcessStartupParamsConverter
    {
        public static readonly WfClientProcessStartupParamsConverter Instance = new WfClientProcessStartupParamsConverter();
        public static readonly WfClientProcessStartupParamsConverter TestInstance = new WfClientProcessStartupParamsConverter(true);

        private readonly bool _TestMode = false;

        private WfClientProcessStartupParamsConverter()
        {
        }

        private WfClientProcessStartupParamsConverter(bool testMode)
        {
            this._TestMode = testMode;
        }

        public WfProcessStartupParams ClientToServer(WfClientProcessStartupParams client, ref WfProcessStartupParams server)
        {
            client.NullCheck("client");

            if (server == null)
                server = new WfProcessStartupParams();

            if (this._TestMode == false)
                server.ProcessDescriptor = WfProcessDescriptorManager.GetDescriptor(client.ProcessDescriptorKey);

            WfClientDictionaryConverter.Instance.ClientToServer(client.ApplicationRuntimeParameters, server.ApplicationRuntimeParameters);

            server.AutoCommit = client.AutoCommit;
            server.AutoStartInitialActivity = client.AutoStartInitialActivity;
            server.CheckStartProcessUserPermission = client.CheckStartProcessUserPermission;
            server.DefaultTaskTitle = client.DefaultTaskTitle;
            server.DefaultUrl = client.DefaultUrl;
            server.RelativeID = client.RelativeID;
            server.RelativeURL = client.RelativeURL;
            server.ResourceID = client.ResourceID;
            server.RuntimeProcessName = client.RuntimeProcessName;

            server.Creator = (IUser)client.Creator.ToOguObject();
            server.Department = (IOrganization)client.Department.ToOguObject();

            WfClientAssigneeCollectionConverter.Instance.ClientToServer(client.Assignees, server.Assignees);

            return server;
        }

        public WfClientProcessStartupParams ServerToClient(WfProcessStartupParams server, ref WfClientProcessStartupParams client)
        {
            server.NullCheck("server");

            if (client == null)
                client = new WfClientProcessStartupParams();

            if (server.ProcessDescriptor != null)
                client.ProcessDescriptorKey = server.ProcessDescriptor.Key;

            WfClientDictionaryConverter.Instance.ServerToClient(server.ApplicationRuntimeParameters, client.ApplicationRuntimeParameters);

            client.AutoCommit = server.AutoCommit;
            client.AutoStartInitialActivity = server.AutoStartInitialActivity;
            client.CheckStartProcessUserPermission = server.CheckStartProcessUserPermission;
            client.DefaultTaskTitle = server.DefaultTaskTitle;
            client.DefaultUrl = server.DefaultUrl;
            client.RelativeID = server.RelativeID;
            client.RelativeURL = server.RelativeURL;
            client.ResourceID = server.ResourceID;
            client.RuntimeProcessName = server.RuntimeProcessName;

            client.Creator = (WfClientUser)server.Creator.ToClientOguObject();
            client.Department = (WfClientOrganization)server.Department.ToClientOguObject();

            WfClientAssigneeCollectionConverter.Instance.ServerToClient(server.Assignees, client.Assignees);

            return client;
        }
    }
}
