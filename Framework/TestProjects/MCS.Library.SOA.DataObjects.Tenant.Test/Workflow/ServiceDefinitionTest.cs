using MCS.Library.Core;
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

        [TestMethod]
        public void EnterActivityServiceTest()
        {
            IWfProcessDescriptor processDesp = ProcessHelper.CreateSimpleProcessDescriptor();

            ((WfActivityDescriptor)processDesp.InitialActivity).EnterEventExecuteServiceKeys = "PCGetVersion";
            IWfProcess process = processDesp.StartupProcessByExecutor(new Dictionary<string, object>() { { "callerID", "EnterActivity" } });

            string result = process.ApplicationRuntimeParameters.GetValue("Version", string.Empty);

            Console.WriteLine(result);

            Assert.IsTrue(result.IndexOf("EnterActivity") >= 0);
        }

        [TestMethod]
        public void LeaveActivityServiceTest()
        {
            IWfProcessDescriptor processDesp = ProcessHelper.CreateSimpleProcessDescriptor();

            ((WfActivityDescriptor)processDesp.InitialActivity).LeaveEventExecuteServiceKeys = "PCGetVersion";
            IWfProcess process = processDesp.StartupProcessByExecutor(new Dictionary<string, object>() { { "callerID", "LeaveActivity" } });

            process = WfRuntime.GetProcessByProcessID(process.ID);

            process = process.MoveToDefaultActivityByExecutor();

            string result = process.ApplicationRuntimeParameters.GetValue("Version", string.Empty);

            Console.WriteLine(result);

            Assert.IsTrue(result.IndexOf("LeaveActivity") >= 0);
        }

        [TestMethod]
        public void CancelTest()
        {
            IWfProcessDescriptor processDesp = ProcessHelper.CreateSimpleProcessDescriptor();

            ((WfProcessDescriptor)processDesp).CancelExecuteServiceKeys = "PCGetVersion";
            IWfProcess process = processDesp.StartupProcessByExecutor(new Dictionary<string, object>() { { "callerID", "CancelProcess" } });

            process = WfRuntime.GetProcessByProcessID(process.ID);

            TenantContext.Current.TenantCode = "Test1";
            process.CancelByExecutor();

            process = WfRuntime.GetProcessByProcessID(process.ID);

            string result = process.ApplicationRuntimeParameters.GetValue("Version", string.Empty);

            Console.WriteLine(result);

            Assert.IsTrue(result.IndexOf("CancelProcess") >= 0);
        }
    }
}
