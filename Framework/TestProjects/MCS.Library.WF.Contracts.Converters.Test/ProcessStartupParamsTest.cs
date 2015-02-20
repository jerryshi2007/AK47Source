using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Common.Test;
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

namespace MCS.Library.WF.Contracts.Converters.Test
{
    [TestClass]
    public class ProcessStartupParamsTest
    {
        [TestMethod]
        public void StandardStartupParamsClientToServer()
        {
            WfClientProcessStartupParams client = ProcessRuntimeHelper.PrepareClientProcessStartupParams(UuidHelper.NewUuidString());

            WfProcessStartupParams server = null;

            WfClientProcessStartupParamsConverter.TestInstance.ClientToServer(client, ref server);

            client.AreSame(server);
        }

        [TestMethod]
        public void SimpleStartupParamsSerializationTest()
        {
            WfClientJsonConverterHelper.Instance.RegisterConverters();

            WfClientProcessStartupParams startupParams = ProcessRuntimeHelper.PrepareClientProcessStartupParams(UuidHelper.NewUuidString());

            string data = JSONSerializerExecute.Serialize(startupParams);

            Console.WriteLine(data);

            WfClientProcessStartupParams deserializedParams = JSONSerializerExecute.Deserialize<WfClientProcessStartupParams>(data);

            startupParams.AreSame(deserializedParams);
        }
    }
}
