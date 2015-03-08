using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.DataObjects
{
    public class WfClientApprovalMatrixConverter
    {
        public static readonly WfClientApprovalMatrixConverter Instance = new WfClientApprovalMatrixConverter();

        private WfClientApprovalMatrixConverter()
        {
        }

        public WfClientApprovalMatrix ServerToClient(WfApprovalMatrix server, ref WfClientApprovalMatrix client)
        {
            server.NullCheck("server");

            if (client == null)
                client = new WfClientApprovalMatrix();

            client.ID = server.ID;

            foreach (SOARolePropertyDefinition spd in server.PropertyDefinitions)
            {
                WfClientRolePropertyDefinition cpd = null;

                WfClientRolePropertyDefinitionConverter.Instance.ServerToClient(spd, ref cpd);

                client.PropertyDefinitions.Add(cpd);
            }

            foreach (SOARolePropertyRow sRow in server.Rows)
            {
                WfClientRolePropertyRow cRow = null;

                WfClientRolePropertyRowConverter.Instance.ServerToClient(sRow, client.PropertyDefinitions, ref cRow);

                client.Rows.Add(cRow);
            }

            return client;
        }

        public WfApprovalMatrix ClientToServer(WfClientApprovalMatrix client, ref WfApprovalMatrix server)
        {
            client.NullCheck("client");

            if (server == null)
                server = new WfApprovalMatrix();

            server.ID = client.ID;

            foreach (WfClientRolePropertyDefinition cpd in client.PropertyDefinitions)
            {
                SOARolePropertyDefinition spd = null;

                WfClientRolePropertyDefinitionConverter.Instance.ClientToServer(cpd, ref spd);

                server.PropertyDefinitions.Add(spd);
            }

            foreach (WfClientRolePropertyRow cRow in client.Rows)
            {
                SOARolePropertyRow sRow = null;

                WfClientRolePropertyRowConverter.Instance.ClientToServer(cRow, server.PropertyDefinitions, ref sRow);

                server.Rows.Add(sRow);
            }

            return server;
        }
    }
}
