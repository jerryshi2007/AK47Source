using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Common.Test;
using MCS.Library.WF.Contracts.Converters;
using MCS.Library.WF.Contracts.Converters.Runtime;
using MCS.Library.WF.Contracts.Json.Converters;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using MCS.Web.Library.Script;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.WF.Contracts.Workflow.Descriptors;

namespace MCS.Library.WF.Contracts.Converters.Test
{
    [TestClass]
    public class ProcessInstanceConverterTest
    {
        [TestMethod]
        public void SimpleProcessServerInstanceToClientTest()
        {
            WfClientUser[] users = new WfClientUser[] { Consts.Users["Requestor"] };

            IWfProcessDescriptor processDesp = ProcessHelper.CreateFreeStepsProcess(users.ToOguObjects<WfClientUser, IUser>().ToArray());

            WfProcessStartupParams startupParams = ProcessHelper.GetInstanceOfWfProcessStartupParams(processDesp);

            IWfProcess process = WfRuntime.StartWorkflow(startupParams);

            process.Context["Context"] = "This is a context";

            WfClientProcess client = null;

            WfClientProcessConverter.Instance.ServerToClient(process, ref client);

            Console.WriteLine(client.InitialActivity.ID);
            Console.WriteLine(client.CompletedActivity.ID);
            Console.WriteLine(client.Descriptor.Url);

            Assert.AreEqual(startupParams.DefaultUrl, client.Descriptor.Url);
            Assert.AreEqual(process.Context["Context"], client.ProcessContext["Context"]);
        }

        [TestMethod]
        public void SimpleProcessSerializationTest()
        {
            WfClientJsonConverterHelper.Instance.RegisterConverters();

            WfClientUser[] users = new WfClientUser[] { Consts.Users["Requestor"] };

            IWfProcess process = ProcessHelper.CreateFreeStepsProcessInstance(users.ToOguObjects<WfClientUser, IUser>().ToArray());

            WfClientProcess client = null;

            WfClientProcessConverter.InstanceWithoutActivityBindings.ServerToClient(process, ref client);

            Assert.IsNull(client.InitialActivity);
            Assert.IsNull(client.CompletedActivity);
            Assert.IsNull(client.MainStream);

            string data = JSONSerializerExecute.Serialize(client);

            Console.WriteLine(data);

            WfClientProcess deserializedProcess = JSONSerializerExecute.Deserialize<WfClientProcess>(data);

            Assert.IsNotNull(deserializedProcess.InitialActivity);
            Assert.IsNotNull(deserializedProcess.CompletedActivity);

            Assert.AreEqual(deserializedProcess.InitialActivity.ID, deserializedProcess.CurrentActivity.ID);
        }

        [TestMethod]
        public void SimpleProcessWithMainStreamSerializationTest()
        {
            WfClientJsonConverterHelper.Instance.RegisterConverters();

            WfClientUser[] users = new WfClientUser[] { Consts.Users["Requestor"] };

            IWfProcess process = ProcessHelper.CreateFreeStepsProcessInstance(users.ToOguObjects<WfClientUser, IUser>().ToArray());

            WfClientProcess client = null;

            WfClientProcessConverter.InstanceAllInfo.ServerToClient(process, ref client);

            Assert.IsNotNull(client.InitialActivity);
            Assert.IsNotNull(client.CompletedActivity);
            Assert.IsNotNull(client.MainStream);
            Assert.IsTrue(client.ApplicationRuntimeParameters.Count > 0);

            string data = JSONSerializerExecute.Serialize(client);

            Console.WriteLine(data);

            WfClientProcess deserializedProcess = JSONSerializerExecute.Deserialize<WfClientProcess>(data);

            Assert.IsNotNull(deserializedProcess.InitialActivity);
            Assert.IsNotNull(deserializedProcess.CompletedActivity);
            Assert.IsNotNull(deserializedProcess.MainStream);
            Assert.IsTrue(deserializedProcess.ApplicationRuntimeParameters.Count > 0);

            Assert.AreEqual(deserializedProcess.InitialActivity.ID, deserializedProcess.CurrentActivity.ID);
        }

        [TestMethod]
        public void SimpleProcessWithAppParametersSerializationTest()
        {
            WfClientJsonConverterHelper.Instance.RegisterConverters();

            WfClientUser[] users = new WfClientUser[] { Consts.Users["Requestor"] };

            IWfProcess process = ProcessHelper.CreateFreeStepsProcessInstance(users.ToOguObjects<WfClientUser, IUser>().ToArray());

            WfClientProcess client = null;

            new WfClientProcessConverter(WfClientProcessInfoFilter.Descriptor | WfClientProcessInfoFilter.ApplicationRuntimeParameters | WfClientProcessInfoFilter.BindActivityDescriptors).ServerToClient(process, ref client);

            Assert.IsNotNull(client.InitialActivity);
            Assert.IsNotNull(client.CompletedActivity);
            Assert.IsNull(client.MainStream);
            Assert.IsTrue(client.ApplicationRuntimeParameters.Count > 0);

            string data = JSONSerializerExecute.Serialize(client);

            Console.WriteLine(data);

            WfClientProcess deserializedProcess = JSONSerializerExecute.Deserialize<WfClientProcess>(data);

            Assert.IsNotNull(deserializedProcess.InitialActivity);
            Assert.IsNotNull(deserializedProcess.CompletedActivity);
            Assert.IsNull(deserializedProcess.MainStream);
            Assert.IsTrue(deserializedProcess.ApplicationRuntimeParameters.Count > 0);
            Assert.IsTrue(deserializedProcess.ProcessContext.Count > 0);

            Assert.AreEqual(deserializedProcess.InitialActivity.ID, deserializedProcess.CurrentActivity.ID);
        }
    }
}
