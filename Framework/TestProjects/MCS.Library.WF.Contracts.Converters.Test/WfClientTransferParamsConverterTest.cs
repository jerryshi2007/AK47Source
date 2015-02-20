using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Common.Test;
using MCS.Library.WF.Contracts.Converters.Runtime;
using MCS.Library.WF.Contracts.Json.Converters;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using MCS.Web.Library.Script;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MCS.Library.WF.Contracts.Converters.Test
{
    [TestClass]
    public class WfClientTransferParamsConverterTest
    {
        [TestMethod]
        public void StandardClientTransferParamsToServer()
        {
            WfClientTransferParams client = PrepareClientTransferParams();

            WfTransferParams server = null;

            WfClientTransferParamsConverter.Instance.ClientToServer(client, null, ref server);

            client.AreSame(server);
        }

        [TestMethod]
        public void ClientTransferParamsSerializationTest()
        {
            WfClientJsonConverterHelper.Instance.RegisterConverters();

            WfClientTransferParams client = PrepareClientTransferParams();

            string data = JSONSerializerExecute.Serialize(client);

            Console.WriteLine(data);

            WfClientTransferParams deserialized = JSONSerializerExecute.Deserialize<WfClientTransferParams>(data);

            client.AreSame(deserialized);
        }

        private static WfClientTransferParams PrepareClientTransferParams()
        {
            WfClientTransferParams transferParams = new WfClientTransferParams("N1");

            transferParams.Operator = Consts.Users["Requestor"];
            transferParams.FromTransitionDescriptorKey = "L1";
            transferParams.Assignees.Add(Consts.Users["Approver1"]);

            WfClientBranchProcessTemplateDescriptor template = BranchProcessTemplateConverterTest.PrepareClientBranchProcessTemplate();
            WfClientBranchProcessTransferParams branchTransferParams = new WfClientBranchProcessTransferParams(template);

            WfClientBranchProcessStartupParams branchStartupParams = new WfClientBranchProcessStartupParams(Consts.Users["CFO"]);

            branchStartupParams.DefaultTaskTitle = "Hello Branch";
            branchStartupParams.Department = Consts.Departments["RequestorOrg"];
            branchStartupParams.ApplicationRuntimeParameters["Amount"] = 1024;
            branchStartupParams.ResourceID = UuidHelper.NewUuidString();
            branchStartupParams.StartupContext = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            branchStartupParams.RelativeParams["Relative"] = "Hello world";

            branchTransferParams.BranchParams.Add(branchStartupParams);

            transferParams.BranchTransferParams.Add(branchTransferParams);

            return transferParams;
        }
    }
}
