using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.WF.Contracts.Converters.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Json.Converters;
using MCS.Web.Library.Script;
using MCS.Library.WF.Contracts.Common.Test;

namespace MCS.Library.WF.Contracts.Converters.DataObjects.Tests
{
    [TestClass()]
    public class DelegationConverterTests
    {
        [TestMethod()]
        public void ClientToServerTest()
        {
            WfClientDelegation client = PrepareClientDelegation();

            WfDelegation server = null;

            WfClientDelegationConverter.Instance.ClientToServer(client, ref server);

            client.AreSame(server);
        }

        [TestMethod()]
        public void ServerToClientTest()
        {
            WfDelegation server = PrepareServerDelegation();

            WfClientDelegation client = null;

            WfClientDelegationConverter.Instance.ServerToClient(server, ref client);

            client.AreSame(server);
        }

        [TestMethod]
        public void SimpleDelegationSerializationTest()
        {
            WfClientJsonConverterHelper.Instance.RegisterConverters();

            WfClientDelegation client = PrepareClientDelegation();

            string data = JSONSerializerExecute.Serialize(client);

            Console.WriteLine(data);

            WfClientDelegation deserializedDelegation = JSONSerializerExecute.Deserialize<WfClientDelegation>(data);

            client.AreSame(deserializedDelegation);
        }

        private static WfClientDelegation PrepareClientDelegation()
        {
            WfClientDelegation delegation = new WfClientDelegation();

            delegation.SourceUserID = UuidHelper.NewUuidString();
            delegation.DestinationUserID = UuidHelper.NewUuidString();

            delegation.SourceUserName = "Source UserName";
            delegation.DestinationUserName = "Destination UserName";
            delegation.Enabled =true;
            delegation.StartTime = new DateTime(2014, 10, 9, 0, 0, 0,DateTimeKind.Utc);
            delegation.EndTime = new DateTime(2014, 10, 9, 0, 0, 0, DateTimeKind.Local);

            delegation.ApplicationName = "WF ApplicationName";
            delegation.ProgramName = "WF ProgramName";
            delegation.TenantCode = UuidHelper.NewUuidString();

            return delegation;
        }

        private static WfDelegation PrepareServerDelegation()
        {
             WfDelegation delegation = new WfDelegation();

            delegation.SourceUserID = UuidHelper.NewUuidString();
            delegation.DestinationUserID = UuidHelper.NewUuidString();

            delegation.SourceUserName = "Source UserName";
            delegation.DestinationUserName = "Destination UserName";
            delegation.Enabled =true;
            delegation.StartTime = new DateTime(2014, 10, 9, 0, 0, 0, DateTimeKind.Local);
            delegation.EndTime = new DateTime(2014, 10, 10, 0, 0, 0, DateTimeKind.Local); 

            return delegation;
        }
 
    }
}
