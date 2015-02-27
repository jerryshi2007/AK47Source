using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Library.WF.Contracts.DataObjects;
using MCS.Library.WF.Contracts.Ogu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.DataObjects
{
    public class WfClientUserOperationLogConverter
    {
        public static readonly WfClientUserOperationLogConverter Instance = new WfClientUserOperationLogConverter();

        private WfClientUserOperationLogConverter()
        {
        }

        public UserOperationLog ClientToServer(WfClientUserOperationLog client, ref UserOperationLog server)
        {
            client.NullCheck("client");

            if (server == null)
                server = new UserOperationLog();

            server.ID = client.ID;
            server.Subject = client.Subject;
            server.ResourceID = client.ResourceID;
            server.ApplicationName = client.ApplicationName;
            server.ProgramName = client.ProgramName;
            server.ProcessID = client.ProcessID;
            server.ActivityID = client.ActivityID;
            server.ActivityName = client.ActivityName;
            server.OperationName = client.OperationName;
            server.OperationType = client.OperationType.ToOperationType();
            server.OperationDescription = client.OperationDescription;
            server.OperationDateTime = client.OperationDateTime;
            server.Operator = (IUser)client.Operator.ToOguObject();
            server.RealUser = (IUser)client.RealUser.ToOguObject();
            server.TopDepartment = (IOrganization)client.TopDepartment.ToOguObject();

            return server;
        }

        public WfClientUserOperationLogCollection ServerToClient(IEnumerable<UserOperationLog> server)
        {
            server.NullCheck("server");

            WfClientUserOperationLogCollection client = new WfClientUserOperationLogCollection();

            foreach (UserOperationLog serverItem in server)
            {
                WfClientUserOperationLog clientItem = null;

                this.ServerToClient(serverItem, ref clientItem);

                client.Add(clientItem);
            }

            return client;
        }

        public WfClientUserOperationLog ServerToClient(UserOperationLog server, ref WfClientUserOperationLog client)
        {
            server.NullCheck("server");

            if (client == null)
                client = new WfClientUserOperationLog();

            client.ID = server.ID;
            client.Subject = server.Subject;
            client.ResourceID = server.ResourceID;
            client.ApplicationName = server.ApplicationName;
            client.ProgramName = server.ProgramName;
            client.ProcessID = server.ProcessID;
            client.ActivityID = server.ActivityID;
            client.ActivityName = server.ActivityName;
            client.OperationName = server.OperationName;
            client.OperationType = server.OperationType.ToClientOperationType();
            client.OperationDescription = server.OperationDescription;
            client.OperationDateTime = server.OperationDateTime;
            client.Operator = (WfClientUser)server.Operator.ToClientOguObject();
            client.RealUser = (WfClientUser)server.RealUser.ToClientOguObject();
            client.TopDepartment = (WfClientOrganization)server.TopDepartment.ToClientOguObject();
            client.HttpContextString = server.HttpContextString;
            client.CorrelationID = server.CorrelationID;

            return client;
        }

        public UserOperationLogCollection ClientToServer(IEnumerable<WfClientUserOperationLog> client)
        {
            client.NullCheck("client");

            UserOperationLogCollection server = new UserOperationLogCollection();

            foreach (WfClientUserOperationLog clientItem in client)
            {
                UserOperationLog serverItem = null;

                this.ClientToServer(clientItem, ref serverItem);

                server.Add(serverItem);
            }

            return server;
        }
    }
}
