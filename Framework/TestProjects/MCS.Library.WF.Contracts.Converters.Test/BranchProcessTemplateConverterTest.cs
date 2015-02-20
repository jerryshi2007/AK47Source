using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Common.Test;
using MCS.Library.WF.Contracts.Converters.Descriptors;
using MCS.Library.WF.Contracts.Json.Converters;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Web.Library.Script;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MCS.Library.WF.Contracts.Converters.Test
{
    [TestClass]
    public class BranchProcessTemplateConverterTest
    {
        [TestMethod]
        public void ClientBranchProcessTemplateToServer()
        {
            WfClientBranchProcessTemplateDescriptor client = PrepareClientBranchProcessTemplate();

            WfBranchProcessTemplateDescriptor server = null;

            WfClientBranchProcessTemplateDescriptorConverter.Instance.ClientToServer(client, ref server);

            client.AreSame(server);
        }

        [TestMethod]
        public void ServerBranchProcessTemplateToClient()
        {
            WfBranchProcessTemplateDescriptor server = PrepareServerBranchProcessTemplate();

            WfClientBranchProcessTemplateDescriptor client = null;

            WfClientBranchProcessTemplateDescriptorConverter.Instance.ServerToClient(server, ref client);

            client.AreSame(server);
        }

        [TestMethod]
        public void ClientBranchProcessTemplateSerializationTest()
        {
            WfClientJsonConverterHelper.Instance.RegisterConverters();

            WfClientBranchProcessTemplateDescriptor client = PrepareClientBranchProcessTemplate();

            string data = JSONSerializerExecute.Serialize(client);

            Console.WriteLine(data);

            WfClientBranchProcessTemplateDescriptor deserialized = JSONSerializerExecute.Deserialize<WfClientBranchProcessTemplateDescriptor>(data);

            client.AreSame(deserialized);
        }

        internal static WfClientBranchProcessTemplateDescriptor PrepareClientBranchProcessTemplate()
        {
            WfClientBranchProcessTemplateDescriptor client = new WfClientBranchProcessTemplateDescriptor("B1");

            client.BlockingType = WfClientBranchProcessBlockingType.WaitAllBranchProcessesComplete;
            client.BranchProcessKey = "Branch1";
            client.DefaultProcessName = "Test Process";
            client.Condition.Expression = "Amount > 0";
            client.SubProcessApprovalMode = WfClientSubProcessApprovalMode.LastActivityDecide;
            client.ExecuteSequence = WfClientBranchProcessExecuteSequence.Parallel;

            client.Resources.Add(new WfClientUserResourceDescriptor(Consts.Users["Requestor"]));
            client.CancelSubProcessNotifier.Add(new WfClientUserResourceDescriptor(Consts.Users["CEO"]));
            client.RelativeLinks.Add(new WfClientRelativeLinkDescriptor("R1") { Category = "Doc Lib", Url = "http://www.baidu.com" });

            return client;
        }

        internal static WfBranchProcessTemplateDescriptor PrepareServerBranchProcessTemplate()
        {
            WfBranchProcessTemplateDescriptor server = new WfBranchProcessTemplateDescriptor("B1");

            server.BlockingType = WfBranchProcessBlockingType.WaitAllBranchProcessesComplete;
            server.BranchProcessKey = "Branch1";
            server.DefaultProcessName = "Test Process";
            server.Condition.Expression = "Amount > 0";
            server.SubProcessApprovalMode = WfSubProcessApprovalMode.LastActivityDecide;
            server.ExecuteSequence = WfBranchProcessExecuteSequence.Parallel;

            server.Resources.Add(new WfUserResourceDescriptor((IUser)Consts.Users["Requestor"].ToOguObject()));
            server.CancelSubProcessNotifier.Add(new WfUserResourceDescriptor((IUser)Consts.Users["CEO"].ToOguObject()));
            server.RelativeLinks.Add(new WfRelativeLinkDescriptor("R1") { Category = "Doc Lib", Url = "http://www.baidu.com" });

            return server;
        }
    }
}
