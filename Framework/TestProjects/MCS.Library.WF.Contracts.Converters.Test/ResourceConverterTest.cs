using MCS.Library.WF.Contracts.Common.Test;
using MCS.Library.WF.Contracts.Json.Converters;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Web.Library.Script;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MCS.Library.WF.Contracts.Converters.Test
{
    [TestClass]
    public class ResourceConverterTest
    {
        [TestMethod]
        public void DynamicResourceJsonConverterTest()
        {
            WfClientJsonConverterHelper.Instance.RegisterConverters();

            WfClientDynamicResourceDescriptor resource = new WfClientDynamicResourceDescriptor("ConditionResource", "Amount > 1000");

            string data = JSONSerializerExecute.Serialize(resource);

            Console.WriteLine(data);

            WfClientDynamicResourceDescriptor deserialized = JSONSerializerExecute.Deserialize<WfClientDynamicResourceDescriptor>(data);

            resource.AreSame(deserialized);
        }
    }
}
