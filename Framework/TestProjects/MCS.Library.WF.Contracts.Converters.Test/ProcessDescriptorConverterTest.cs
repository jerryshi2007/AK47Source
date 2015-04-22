using System;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Common.Test;
using MCS.Library.WF.Contracts.Converters.Descriptors;
using MCS.Library.WF.Contracts.Json.Converters;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Web.Library.Script;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.WF.Contracts.Workflow.Builders;
using System.IO;
using MCS.Library.Office.OpenXml.Excel;

namespace MCS.Library.WF.Contracts.Converters.Test
{
    [TestClass]
    public class ProcessDescriptorConverterTest
    {
        [TestMethod]
        public void SimpleStandardClientProcessToServer()
        {
            WfClientProcessDescriptor clientProcessDesp = ProcessDescriptorHelper.CreateSimpleClientProcessWithLines();

            clientProcessDesp.Output();

            WfProcessDescriptor processDesp = null;

            WfClientProcessDescriptorConverter.Instance.ClientToServer(clientProcessDesp, ref processDesp);

            processDesp.Output();

            clientProcessDesp.AssertProcessDescriptor(processDesp);
        }

        [TestMethod]
        public void SimpleStandardServerProcessToClient()
        {
            WfProcessDescriptor processDesp = ProcessDescriptorHelper.CreateSimpleServerProcessWithLines();

            processDesp.Output();

            WfClientProcessDescriptor clientProcessDesp = null;

            WfClientProcessDescriptorConverter.Instance.ServerToClient(processDesp, ref clientProcessDesp);

            processDesp.Output();

            clientProcessDesp.AssertProcessDescriptor(processDesp);
        }

        [TestMethod]
        public void ClientProcessWithWithActivityMatrixResourceDescriptorToServerTest()
        {
            WfClientProcessDescriptor client = ProcessDescriptorHelper.CreateClientProcessWithActivityMatrixResourceDescriptor();
            WfProcessDescriptor server = null;

            WfClientProcessDescriptorConverter.Instance.ClientToServer(client, ref server);

            Assert.IsNotNull(server.Activities["N1"]);
            Assert.IsTrue(server.Activities["N1"].Resources.Count > 0);
            Assert.IsTrue(server.Activities["N1"].Resources[0] is WfActivityMatrixResourceDescriptor);
        }

        [TestMethod]
        public void ClientProcessWithWithActivityMatrixResourceDescriptorSerializationTest()
        {
            WfClientJsonConverterHelper.Instance.RegisterConverters();

            WfClientProcessDescriptor processDesp = ProcessDescriptorHelper.CreateClientProcessWithActivityMatrixResourceDescriptor();
            string data = JSONSerializerExecute.Serialize(processDesp);

            Console.WriteLine(data);

            WfClientProcessDescriptor deserialized = JSONSerializerExecute.Deserialize<WfClientProcessDescriptor>(data);

            Assert.AreEqual(processDesp.Key, deserialized.Key);
            Assert.AreEqual(processDesp.Activities.Count, deserialized.Activities.Count);
            Assert.AreEqual(processDesp.InitialActivity.Key, deserialized.InitialActivity.Key);
            Assert.AreEqual(processDesp.CompletedActivity.Key, deserialized.CompletedActivity.Key);
            Assert.AreEqual(processDesp.InitialActivity.ToTransitions.Count, deserialized.InitialActivity.ToTransitions.Count);
            Assert.AreEqual(processDesp.CancelEventReceivers.Count, deserialized.CancelEventReceivers.Count);
            Assert.AreEqual(processDesp.CompletedActivity.GetFromTransitions().Count, deserialized.CompletedActivity.GetFromTransitions().Count);

            Assert.IsNotNull(deserialized.Activities["N1"]);
            Assert.IsTrue(deserialized.Activities["N1"].Resources.Count > 0);
            Assert.IsTrue(deserialized.Activities["N1"].Resources[0] is WfClientActivityMatrixResourceDescriptor);

            ((WfClientActivityMatrixResourceDescriptor)processDesp.Activities["N1"].Resources[0]).AreSame((WfClientActivityMatrixResourceDescriptor)deserialized.Activities["N1"].Resources[0]);
        }

        [TestMethod]
        public void SimpleProcessSerializationTest()
        {
            WfClientJsonConverterHelper.Instance.RegisterConverters();

            WfClientProcessDescriptor processDesp = ProcessDescriptorHelper.CreateSimpleClientProcessWithLines();

            string data = JSONSerializerExecute.Serialize(processDesp);

            Console.WriteLine(data);

            WfClientProcessDescriptor deserialized = JSONSerializerExecute.Deserialize<WfClientProcessDescriptor>(data);

            Assert.AreEqual(processDesp.Key, deserialized.Key);
            Assert.AreEqual(processDesp.Activities.Count, deserialized.Activities.Count);
            Assert.AreEqual(processDesp.InitialActivity.Key, deserialized.InitialActivity.Key);
            Assert.AreEqual(processDesp.CompletedActivity.Key, deserialized.CompletedActivity.Key);
            Assert.AreEqual(processDesp.InitialActivity.ToTransitions.Count, deserialized.InitialActivity.ToTransitions.Count);
            Assert.AreEqual(processDesp.CancelEventReceivers.Count, deserialized.CancelEventReceivers.Count);
            Assert.AreEqual(processDesp.CompletedActivity.GetFromTransitions().Count, deserialized.CompletedActivity.GetFromTransitions().Count);
        }

        [TestMethod]
        public void WfCreateClientDynamicProcessParamsSerializationTest()
        {
            WfClientJsonConverterHelper.Instance.RegisterConverters();

            WfCreateClientDynamicProcessParams createParams = ProcessDescriptorHelper.CreateClientDynamicProcessParams();

            string data = JSONSerializerExecute.Serialize(createParams);

            Console.WriteLine(data);

            WfCreateClientDynamicProcessParams deserialized = JSONSerializerExecute.Deserialize<WfCreateClientDynamicProcessParams>(data);

            createParams.AreSame(deserialized);
        }

        [TestMethod]
        public void WfClientDynamicProcessBuilderTest()
        {
            WfCreateClientDynamicProcessParams createParams = ProcessDescriptorHelper.CreateClientDynamicProcessParams();

            WfClientDynamicProcessBuilder builder = new WfClientDynamicProcessBuilder(createParams);

            WfClientProcessDescriptor client = builder.Build(createParams.Key, createParams.Name);

            WfProcessDescriptor server = null;

            WfClientProcessDescriptorConverter.Instance.ClientToServer(client, ref server);

            Assert.IsTrue(server.Variables.GetValue("ClientDynamicProcess", false));
            Assert.IsNotNull(server.Activities["N1"]);
            Assert.IsTrue(server.Activities["N1"].Resources.Count > 0);
            Assert.IsTrue(server.Activities["N1"].Resources[0] is WfActivityMatrixResourceDescriptor);
        }

        [TestMethod]
        public void WfClientDynamicProcessBuilderInstanceTest()
        {
            WfCreateClientDynamicProcessParams createParams = ProcessDescriptorHelper.CreateClientDynamicProcessParams();

            WfClientDynamicProcessBuilder builder = new WfClientDynamicProcessBuilder(createParams);

            WfClientProcessDescriptor client = builder.Build(createParams.Key, createParams.Name);

            IWfProcess process = ProcessHelper.CreateProcessInstance(client);

            Assert.IsTrue(process.Activities.Count > client.Activities.Count);
        }

        [TestMethod()]
        public void ClientDynamicProcessToExcelStreamTest()
        {
            WfCreateClientDynamicProcessParams createParams = ProcessDescriptorHelper.CreateClientDynamicProcessParams();
            WfClientDynamicProcessBuilder builder = new WfClientDynamicProcessBuilder(createParams);
            WfClientProcessDescriptor client = builder.Build(createParams.Key, createParams.Name);

            string processKey = createParams.Key;
            System.Data.DataTable processTable = new System.Data.DataTable();
            System.Data.DataTable matrixTable = new System.Data.DataTable();

            using (Stream stream = WfClientProcessDescriptorConverter.Instance.ClientDynamicProcessToExcelStream(client))
            {
                processTable = DocumentHelper.GetRangeValuesAsTable(stream, "Process", "A3");
                matrixTable = DocumentHelper.GetRangeValuesAsTable(stream, "Matrix", "A3");
            }
            Assert.IsTrue(processTable.Rows.Count > 0);
            Assert.IsTrue(matrixTable.Rows.Count == 2);
            Assert.IsTrue(matrixTable.Rows[0]["CostCenter"].ToString() == "1001");
            Assert.IsTrue(matrixTable.Rows[1]["Age"].ToString() == "40");
        }


    }
}
