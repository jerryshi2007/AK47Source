using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow.Exporters;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Descriptors
{
    public class WfClientExportProcessDescriptorParamsConverter
    {
        public static readonly WfClientExportProcessDescriptorParamsConverter Instance = new WfClientExportProcessDescriptorParamsConverter();

        private WfClientExportProcessDescriptorParamsConverter()
        {
        }

        public WfExportProcessDescriptorParams ClientToServer(WfClientExportProcessDescriptorParams client, ref WfExportProcessDescriptorParams server)
        {
            client.NullCheck("client");

            if (server == null)
                server = new WfExportProcessDescriptorParams();

            server.MatrixRoleAsPerson = client.MatrixRoleAsPerson;

            return server;
        }

        public WfClientExportProcessDescriptorParams ServerToClient(WfExportProcessDescriptorParams server, ref WfClientExportProcessDescriptorParams client)
        {
            server.NullCheck("server");

            if (client == null)
                client = new WfClientExportProcessDescriptorParams();

            client.MatrixRoleAsPerson = server.MatrixRoleAsPerson;

            return client;
        }
    }
}
