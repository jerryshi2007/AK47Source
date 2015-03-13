using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Converters.DataObjects;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Descriptors;

namespace MCS.Library.WF.Contracts.Converters.Descriptors
{
    public class WfClientActivityMatrixResourceDescriptorConverter : WfClientResourceDescriptorConverterBase
    {
        public static readonly WfClientActivityMatrixResourceDescriptorConverter Instance = new WfClientActivityMatrixResourceDescriptorConverter();

        private WfClientActivityMatrixResourceDescriptorConverter()
        {
        }

        public override void ClientToServer(WfClientResourceDescriptor client, ref WfResourceDescriptor server)
        {
            client.NullCheck("client");

            if (server == null)
                server = new WfActivityMatrixResourceDescriptor();

            WfActivityMatrixResourceDescriptor amr = (WfActivityMatrixResourceDescriptor)server;
            WfClientActivityMatrixResourceDescriptor cmr = (WfClientActivityMatrixResourceDescriptor)client;

            amr.ExternalMatrixID = cmr.ExternalMatrixID;

            foreach (WfClientRolePropertyDefinition cpd in cmr.PropertyDefinitions)
            {
                SOARolePropertyDefinition spd = null;

                WfClientRolePropertyDefinitionConverter.Instance.ClientToServer(cpd, ref spd);

                amr.PropertyDefinitions.Add(spd);
            }

            foreach (WfClientRolePropertyRow cRow in cmr.Rows)
            {
                SOARolePropertyRow sRow = null;

                WfClientRolePropertyRowConverter.Instance.ClientToServer(cRow, amr.PropertyDefinitions, ref sRow);

                amr.Rows.Add(sRow);
            }
        }

        public override void ServerToClient(WfResourceDescriptor server, ref WfClientResourceDescriptor client)
        {
            server.NullCheck("server");

            if (client == null)
                client = new WfClientActivityMatrixResourceDescriptor();

            WfClientActivityMatrixResourceDescriptor cmr = (WfClientActivityMatrixResourceDescriptor)client;
            WfActivityMatrixResourceDescriptor amr = (WfActivityMatrixResourceDescriptor)server;

            cmr.ExternalMatrixID = amr.ExternalMatrixID;

            foreach (SOARolePropertyDefinition spd in amr.PropertyDefinitions)
            {
                WfClientRolePropertyDefinition cpd = null;

                WfClientRolePropertyDefinitionConverter.Instance.ServerToClient(spd, ref cpd);

                cmr.PropertyDefinitions.Add(cpd);
            }

            foreach (SOARolePropertyRow sRow in amr.Rows)
            {
                WfClientRolePropertyRow cRow = null;

                WfClientRolePropertyRowConverter.Instance.ServerToClient(sRow, cmr.PropertyDefinitions, ref cRow);

                cmr.Rows.Add(cRow);
            }
        }
    }
}
