using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Common.Test;
using MCS.Library.WF.Contracts.Converters.Descriptors;
using MCS.Library.WF.Contracts.Converters.Test;
using MCS.Library.WF.Contracts.Json.Converters;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Web.Library.Script;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Descriptors.Tests
{
    [TestClass()]
    public class WfClientProcessDescriptorInfoConverterTests
    {
        [TestMethod]
        public void WfProcessDescriptorInfoToClientTest()
        {
            WfProcessDescriptorInfo server = PrepareServerData();
            ClearServerData(server.ProcessKey);

            WfClientProcessDescriptorInfo client = null;

            WfClientProcessDescriptorInfoConverter.Instance.ServerToClient(server, ref client);

            client.AreSame(server);
        }

        [TestMethod]
        public void WfProcessDescriptorInfoSerializationTest()
        {
            WfClientJsonConverterHelper.Instance.RegisterConverters();

            WfProcessDescriptorInfo server = PrepareServerData();
            ClearServerData(server.ProcessKey);

            WfClientProcessDescriptorInfo client = null;

            WfClientProcessDescriptorInfoConverter.Instance.ServerToClient(server, ref client);

            string data = JSONSerializerExecute.Serialize(client);

            Console.WriteLine(data);

            WfClientProcessDescriptorInfo deserialized = JSONSerializerExecute.Deserialize<WfClientProcessDescriptorInfo>(data);

            client.AreSame(deserialized);
        }

        private static WfProcessDescriptorInfo PrepareServerData()
        {

            WfClientUser[] users = new WfClientUser[] { Consts.Users["Requestor"] };

            IWfProcessDescriptor processDesp = ProcessHelper.CreateFreeStepsProcess(users.ToOguObjects<WfClientUser, IUser>().ToArray());

            WfSqlProcessDescriptorManager manager = new WfSqlProcessDescriptorManager();
            manager.SaveDescriptor(processDesp);
            WfProcessDescriptorInfo server = WfProcessDescriptorInfoAdapter.Instance.Load(processDesp.Key);

            return server;
        }

        private static bool ClearServerData(string key)
        {
            WfSqlProcessDescriptorManager manager = new WfSqlProcessDescriptorManager();
            manager.DeleteDescriptor(key);

            return true;
        }
    }
}
