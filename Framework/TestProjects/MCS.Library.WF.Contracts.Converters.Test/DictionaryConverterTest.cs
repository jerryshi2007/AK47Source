using System;
using System.Collections;
using System.Collections.Generic;
using MCS.Library.SOA.DataObjects;
using MCS.Library.WF.Contracts.Common.Test;
using MCS.Library.WF.Contracts.Converters.Runtime;
using MCS.Library.WF.Contracts.Ogu;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.WF.Contracts.Converters.Test
{
    [TestClass]
    public class DictionaryConverterTest
    {
        [TestMethod]
        public void ClientGenericListTypeToServerTest()
        {
            List<string> clientData = new List<string>();

            clientData.Add("Shen Zheng");

            Dictionary<string, object> client = new Dictionary<string, object>();

            client["Data"] = clientData;

            Dictionary<string, object> server = new Dictionary<string, object>();

            WfClientDictionaryConverter.Instance.ClientToServer(client, server);

            Assert.IsTrue(server.ContainsKey("Data"));

            IList serverData = (IList)server["Data"];

            Assert.AreEqual(clientData[0], serverData[0]);
        }

        [TestMethod]
        public void ClientUserListTypeToServerTest()
        {
            List<WfClientUser> clientData = new List<WfClientUser>();

            clientData.Add(Consts.Users["Requestor"]);

            Dictionary<string, object> client = new Dictionary<string, object>();

            client["Data"] = clientData;

            Dictionary<string, object> server = new Dictionary<string, object>();

            WfClientDictionaryConverter.Instance.ClientToServer(client, server);

            Assert.IsTrue(server.ContainsKey("Data"));

            IList serverData = (IList)server["Data"];

            Assert.AreEqual(clientData[0].ID, ((OguUser)serverData[0]).ID);
        }

        [TestMethod]
        public void ServerGenericListTypeToClientTest()
        {
            List<string> serverData = new List<string>();

            serverData.Add("Shen Zheng");

            Dictionary<string, object> server = new Dictionary<string, object>();

            server["Data"] = serverData;

            Dictionary<string, object> client = new Dictionary<string, object>();

            WfClientDictionaryConverter.Instance.ServerToClient(server, client);

            Assert.IsTrue(client.ContainsKey("Data"));

            IList clientData = (IList)server["Data"];

            Assert.AreEqual(serverData[0], clientData[0]);
        }

        [TestMethod]
        public void ServerUserListTypeToClientTest()
        {
            List<OguUser> serverData = new List<OguUser>();

            serverData.Add((OguUser)Consts.Users["Requestor"].ToOguObject());

            Dictionary<string, object> server = new Dictionary<string, object>();

            server["Data"] = serverData;

            Dictionary<string, object> client = new Dictionary<string, object>();

            WfClientDictionaryConverter.Instance.ServerToClient(server, client);

            Assert.IsTrue(client.ContainsKey("Data"));

            IList clientData = (IList)client["Data"];

            Assert.AreEqual(serverData[0].ID, ((WfClientUser)clientData[0]).ID);
        }
    }
}
