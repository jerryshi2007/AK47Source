using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Converters.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Runtime
{
    public class WfClientBranchProcessTransferParamsConverter
    {
        public static readonly WfClientBranchProcessTransferParamsConverter Instance = new WfClientBranchProcessTransferParamsConverter();

        private WfClientBranchProcessTransferParamsConverter()
        {
        }

        public WfBranchProcessTransferParams ClientToServer(WfClientBranchProcessTransferParams client, ref WfBranchProcessTransferParams server)
        {
            client.NullCheck("client");
            client.Template.NullCheck("Template");

            if (server == null)
                server = new WfBranchProcessTransferParams();

            WfBranchProcessTemplateDescriptor serverTemplate = null;

            WfClientBranchProcessTemplateDescriptorConverter.Instance.ClientToServer(client.Template, ref serverTemplate);
            server.Template = serverTemplate;

            WfClientBranchProcessStartupParamsCollectionConverter.Instance.ClientToServer(client.BranchParams, server.BranchParams);

            return server;
        }
    }

    public class WfClientBranchProcessTransferParamsCollectionConverter
    {
        public static readonly WfClientBranchProcessTransferParamsCollectionConverter Instance = new WfClientBranchProcessTransferParamsCollectionConverter();

        private WfClientBranchProcessTransferParamsCollectionConverter()
        {
        }

        public void ClientToServer(IEnumerable<WfClientBranchProcessTransferParams> client, ICollection<WfBranchProcessTransferParams> server)
        {
            client.NullCheck("client");

            foreach (WfClientBranchProcessTransferParams ct in client)
            {
                WfBranchProcessTransferParams st = null;

                WfClientBranchProcessTransferParamsConverter.Instance.ClientToServer(ct, ref st);

                server.Add(st);
            }
        }
    }
}
