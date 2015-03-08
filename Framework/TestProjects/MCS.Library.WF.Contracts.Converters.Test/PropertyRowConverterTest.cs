using MCS.Library.Office.OpenXml.Excel;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Common.Test;
using MCS.Library.WF.Contracts.Converters.DataObjects;
using MCS.Library.WF.Contracts.Converters.Descriptors;
using MCS.Library.WF.Contracts.Json.Converters;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Web.Library.Script;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Test
{
    [TestClass]
    public class PropertyRowConverterTest
    {
        [TestMethod]
        public void SimpleClientActivityMatrixResourceDescriptorToServer()
        {
            WfClientActivityMatrixResourceDescriptor client = ProcessDescriptorHelper.GetClientActivityMatrixResourceDescriptor();

            WfResourceDescriptor server = null;

            WfClientActivityMatrixResourceDescriptorConverter.Instance.ClientToServer(client, ref server);

            client.AreSame((WfActivityMatrixResourceDescriptor)server);
        }

        [TestMethod]
        public void SimpleServerActivityMatrixResourceDescriptorToClient()
        {
            WfActivityMatrixResourceDescriptor server = ProcessDescriptorHelper.GetServerActivityMatrixResourceDescriptor();

            WfClientResourceDescriptor client = null;

            WfClientActivityMatrixResourceDescriptorConverter.Instance.ServerToClient(server, ref client);

            ((WfClientActivityMatrixResourceDescriptor)client).AreSame(server);
        }

        [TestMethod]
        public void SimpleClientApprovalMatrixToServer()
        {
            WfClientApprovalMatrix client = ApprovalMatrixHelper.PrepareClientApprovalMatrix();

            WfApprovalMatrix server = null;

            WfClientApprovalMatrixConverter.Instance.ClientToServer(client, ref server);

            client.AreSame(server);
        }

        [TestMethod]
        public void SimpleServerApprovalMatrixToClient()
        {
            WfApprovalMatrix server = ApprovalMatrixHelper.PrepareServerApprovalMatrix();

            WfClientApprovalMatrix client = null;

            WfClientApprovalMatrixConverter.Instance.ServerToClient(server, ref client);

            client.AreSame(server);
        }

        [TestMethod]
        public void ClientActivityMatrixResourceDescriptorSerializationTest()
        {
            WfClientJsonConverterHelper.Instance.RegisterConverters();

            WfClientActivityMatrixResourceDescriptor resource = ProcessDescriptorHelper.GetClientActivityMatrixResourceDescriptor();

            string data = JSONSerializerExecute.Serialize(resource);

            Console.WriteLine(data);

            WfClientActivityMatrixResourceDescriptor deserializedResource = JSONSerializerExecute.Deserialize<WfClientActivityMatrixResourceDescriptor>(data);

            resource.AreSame(deserializedResource);
        }

        [TestMethod]
        public void ClientApprovalMatrixDescriptorSerializationTest()
        {
            WfClientJsonConverterHelper.Instance.RegisterConverters();

            WfClientApprovalMatrix matrix = ApprovalMatrixHelper.PrepareClientApprovalMatrix();

            string data = JSONSerializerExecute.Serialize(matrix);

            Console.WriteLine(data);

            WfClientApprovalMatrix deserialized = JSONSerializerExecute.Deserialize<WfClientApprovalMatrix>(data);

            matrix.AreSame(deserialized);
        }

        [TestMethod]
        public void ClientApprovalMatrixToExcelTest()
        {
            WfClientApprovalMatrix matrix = ApprovalMatrixHelper.PrepareClientApprovalMatrix();

            WorkBook workBook = matrix.ToWorkBook();

            workBook.Save("ClientApprovalMatrixToExcelTest.xlsx");

            WfClientApprovalMatrix deserialized = workBook.ToApprovalMatrix();

            deserialized.ID = matrix.ID;

            matrix.AreSame(deserialized);
        }
    }
}
