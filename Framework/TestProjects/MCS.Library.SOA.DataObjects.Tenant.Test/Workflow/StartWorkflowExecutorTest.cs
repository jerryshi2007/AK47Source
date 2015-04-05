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
    [TestClass]
    public class StartWorkflowExecutorTest
    {
        [ClassInitialize()]
        public static void ProcessDescriptorInitialize(TestContext testContext)
        {
            WfProcessDescriptorInfoAdapter.Instance.ClearAll();
            WfProcessDescriptorDimensionAdapter.Instance.ClearAll();
            WfProcessInstanceDataAdapter.Instance.ClearAll();
            UserOperationLogAdapter.Instance.ClearAll();
        }

        /// <summary>
        /// 正常的启动流程的Executor的测试
        /// </summary>
        [TestMethod]
        public void NormalStartWorkflowExecutorTest()
        {
            IWfProcess process = ProcessHelper.CreateSimpleProcessDescriptor().StartupProcessByExecutor();

            IWfProcess processLoaded = WfRuntime.GetProcessByProcessID(process.ID);

            Assert.AreEqual(WfProcessStatus.Running, processLoaded.Status);
            Assert.AreEqual(process.CurrentActivity.ID, processLoaded.CurrentActivity.ID);
            Assert.AreEqual(WfActivityStatus.Running, processLoaded.CurrentActivity.Status);
        }

        /// <summary>
        /// 启动流程并且流转到下一个活动的测试
        /// </summary>
        [TestMethod]
        public void StartWorkflowAndMoveToNextExecutorTest()
        {
            IWfProcessDescriptor processDesp = ProcessHelper.CreateFreeStepsProcessDescriptor(OguObjectSettings.GetConfig().Objects["approver1"].User);

            WfTransferParams transferParams = WfTransferParams.FromNextDefaultActivity(processDesp.InitialActivity);

            WfProcessStartupParams startupParams = processDesp.PrepareStartupParams();

            WfStartWorkflowExecutor executor = new WfStartWorkflowExecutor(null, startupParams, transferParams);

            IWfProcess process = executor.Execute();

            IWfProcess processLoaded = WfRuntime.GetProcessByProcessID(process.ID);

            Console.WriteLine("Process ID: {0}, Status: {1}", process.ID, process.Status);
            Console.WriteLine("Current ActivityKey : {0}, Status: {1}", process.CurrentActivity.Descriptor.Key, process.CurrentActivity.Status);

            Assert.AreEqual(process.CurrentActivity.ID, processLoaded.CurrentActivity.ID);
            Assert.AreEqual(WfActivityStatus.Running, processLoaded.CurrentActivity.Status);
            Assert.IsTrue(processLoaded.CurrentActivity.Assignees.Count > 0);
        }

        /// <summary>
        /// 启动流程并且流转到下一个默认活动的测试
        /// </summary>
        [TestMethod]
        public void StartWorkflowAndMoveToNextDefaultExecutorTest()
        {
            IWfProcessDescriptor processDesp = ProcessHelper.CreateFreeStepsProcessDescriptor(OguObjectSettings.GetConfig().Objects["approver1"].User);

            WfProcessStartupParams startupParams = processDesp.PrepareStartupParams();

            WfStartWorkflowExecutor executor = new WfStartWorkflowExecutor(null, startupParams, null);

            IWfProcess process = executor.Execute();

            IWfProcess processLoaded = WfRuntime.GetProcessByProcessID(process.ID);

            Console.WriteLine("Process ID: {0}, Status: {1}", process.ID, process.Status);
            Console.WriteLine("Current ActivityKey : {0}, Status: {1}", process.CurrentActivity.Descriptor.Key, process.CurrentActivity.Status);

            Assert.AreEqual(process.CurrentActivity.ID, processLoaded.CurrentActivity.ID);
            Assert.AreEqual(WfActivityStatus.Running, processLoaded.CurrentActivity.Status);
            Assert.IsTrue(processLoaded.CurrentActivity.Assignees.Count > 0);
        }
    }
}
