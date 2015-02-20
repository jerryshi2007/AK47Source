using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Common.Test;
using MCS.Library.WF.Contracts.Converters;
using MCS.Library.WF.Contracts.Converters.Runtime;
using MCS.Library.WF.Contracts.Json.Converters;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Runtime;
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
    public class ProcessCurrentInfoConverterTest
    {
        [TestMethod]
        public void ProcessCurrentInfoToClientTest()
        {
            IWfProcess process = PrepareProcess();

            WfProcessCurrentInfo server = WfProcessCurrentInfoAdapter.Instance.LoadByProcessID(process.ID).FirstOrDefault();

            WfClientProcessCurrentInfo client = null;

            WfClientProcessCurrentInfoConverter.Instance.ServerToClient(server, ref client);

            client.AreSame(server);
        }

        [TestMethod]
        public void ProcessCurrentInfoSerializationTest()
        {
            WfClientJsonConverterHelper.Instance.RegisterConverters();

            IWfProcess process = PrepareProcess();

            WfProcessCurrentInfo server = WfProcessCurrentInfoAdapter.Instance.LoadByProcessID(process.ID).FirstOrDefault();

            WfClientProcessCurrentInfo client = null;

            WfClientProcessCurrentInfoConverter.Instance.ServerToClient(server, ref client);

            string data = JSONSerializerExecute.Serialize(client);

            Console.WriteLine(data);

            WfClientProcessCurrentInfo deserialized = JSONSerializerExecute.Deserialize<WfClientProcessCurrentInfo>(data);

            client.AreSame(deserialized);
        }

        private static IWfProcess PrepareProcess()
        {
            WfClientUser[] users = new WfClientUser[] { Consts.Users["Requestor"] };

            IWfProcessDescriptor processDesp = ProcessHelper.CreateFreeStepsProcess(users.ToOguObjects<WfClientUser, IUser>().ToArray());

            WfProcessStartupParams startupParams = ProcessHelper.GetInstanceOfWfProcessStartupParams(processDesp);

            IWfProcess process = WfRuntime.StartWorkflow(startupParams);

            WfRuntime.PersistWorkflows();

            return process;
        }
    }
}
