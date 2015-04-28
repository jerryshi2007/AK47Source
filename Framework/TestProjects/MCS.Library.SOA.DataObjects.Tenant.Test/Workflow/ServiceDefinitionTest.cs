using MCS.Library.SOA.DataObjects.Tenant.Test.Workflow.Helper;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.Workflow
{
    /// <summary>
    /// 服务定义的测试
    /// </summary>
    [TestClass]
    public class ServiceDefinitionTest
    {
        [TestMethod]
        public void ServiceDefinitionSettingsTest()
        {
            WfServiceOperationDefinitionConfigurationElement operationElement =
                WfServiceDefinitionSettings.GetSection().Operations["PCGetVersion"];

            Assert.IsNotNull(operationElement);

            WfServiceOperationDefinition operation = new WfServiceOperationDefinition(operationElement);

            operation.AreEqual(operationElement);
        }

        [TestMethod]
        public void CallServiceTest()
        {
            WfServiceOperationDefinitionConfigurationElement operationElement =
                WfServiceDefinitionSettings.GetSection().Operations["PCGetVersion"];

            Assert.IsNotNull(operationElement);

            WfServiceOperationDefinition operation = new WfServiceOperationDefinition(operationElement);

            WfServiceInvoker.InvokeContext["Version"] = string.Empty;
            WfServiceInvoker.InvokeContext["callerID"] = "Zheng Shen";

            WfServiceInvoker invoker = new WfServiceInvoker(operation);

            invoker.Invoke();

            string result = WfServiceInvoker.InvokeContext.GetValue("Version", string.Empty);

            Console.WriteLine(result);

            Assert.IsTrue(result.IndexOf(WfServiceInvoker.InvokeContext.GetValue("callerID", string.Empty)) >= 0);
        }
    }
}
