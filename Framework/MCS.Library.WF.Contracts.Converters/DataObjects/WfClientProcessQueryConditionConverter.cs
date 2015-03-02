using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow.Conditions;
using MCS.Library.WF.Contracts.DataObjects;
using MCS.Library.WF.Contracts.Ogu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.DataObjects
{
    public class WfClientProcessQueryConditionConverter
    {
        public static readonly WfClientProcessQueryConditionConverter Instance = new WfClientProcessQueryConditionConverter();

        private WfClientProcessQueryConditionConverter()
        {
        }

        public WfClientProcessQueryCondition ServerToClient(WfProcessQueryCondition server, ref WfClientProcessQueryCondition client)
        {
            server.NullCheck("server");

            if (client == null)
                client = new WfClientProcessQueryCondition();

            client.ApplicationName = server.ApplicationName;
            client.ProcessName = server.ProcessName;
            client.AssigneeExceptionFilterType = server.AssigneeExceptionFilterType.ToClientAssigneeExceptionFilterType();
            client.AssigneesSelectType = server.AssigneesSelectType.ToClientAssigneesFilterType();

            client.AssigneesUserName = server.AssigneesUserName;
            client.BeginStartTime = server.BeginStartTime;
            client.EndStartTime = server.EndStartTime;
            client.ProcessStatus = server.ProcessStatus;
            client.DepartmentName = server.DepartmentName;

            foreach (IUser user in server.CurrentAssignees)
                client.CurrentAssignees.Add((WfClientUser)(user.ToClientOguObject()));

            return client;
        }

        public WfProcessQueryCondition ClientToServer(WfClientProcessQueryCondition client, ref WfProcessQueryCondition server)
        {
            client.NullCheck("client");

            if (server == null)
                server = new WfProcessQueryCondition();

            server.ApplicationName = client.ApplicationName;
            server.ProcessName = client.ProcessName;
            server.AssigneeExceptionFilterType = client.AssigneeExceptionFilterType.ToAssigneeExceptionFilterType();
            server.AssigneesSelectType = client.AssigneesSelectType.ToAssigneesFilterType();

            server.AssigneesUserName = client.AssigneesUserName;
            server.BeginStartTime = client.BeginStartTime;
            server.EndStartTime = client.EndStartTime;
            server.ProcessStatus = client.ProcessStatus;
            server.DepartmentName = client.DepartmentName;

            foreach (WfClientUser user in client.CurrentAssignees)
                server.CurrentAssignees.Add((IUser)(user.ToOguObject()));

            return server;
        }
    }
}
