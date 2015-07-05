using MCS.Library.SOA.DataObjects.Tenant.Test.Workflow.Helper;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.Workflow
{
    /// <summary>
    /// 流程办结的有关测试
    /// </summary>
    [TestClass]
    public class ProcessParameterTest
    {
        [TestMethod]
        public void AgreeProcessTest()
        {
            IWfProcessDescriptor processDesp = PrepareProcessDescriptor(true);

            IWfProcess process = processDesp.StartupProcess();

            process = process.MoveToDefaultActivityByExecutor();

            process = process.MoveToDefaultActivityByExecutor();

            Assert.AreEqual(WfProcessStatus.Completed, process.Status);
            Assert.AreEqual(true, process.ApplicationRuntimeParameters["CurrentProcessReturnValue"]);
        }

        [TestMethod]
        public void DisagreeProcessTest()
        {
            IWfProcessDescriptor processDesp = PrepareProcessDescriptor(false);

            processDesp.CompletedActivity.EnterEventReceivers.Add(new WfUserResourceDescriptor(OguObjectSettings.GetConfig().Objects["approver1"].User));

            IWfProcess process = processDesp.StartupProcess();

            process = process.MoveToDefaultActivityByExecutor();

            process = process.MoveToDefaultActivityByExecutor();

            Assert.AreEqual(WfProcessStatus.Completed, process.Status);
            Assert.AreEqual(false, process.ApplicationRuntimeParameters["CurrentProcessReturnValue"]);
        }

        [TestMethod]
        public void TransitionNameTest()
        {
            IWfProcessDescriptor processDesp = PrepareProcessDescriptor(true);

            IWfProcess process = processDesp.StartupProcess();

            process = process.MoveToDefaultActivityByExecutor();

            process = process.MoveToDefaultActivityByExecutor();

            Console.WriteLine(process.ApplicationRuntimeParameters["CurrentTransitionName"]);

            IWfActivityDescriptor normalActivity = processDesp.Activities["NormalActivity"];

            Assert.AreEqual(normalActivity.ToTransitions[0].Name, process.ApplicationRuntimeParameters["CurrentTransitionName"]);
        }

        private static IWfProcessDescriptor PrepareProcessDescriptor(bool selectAgree)
        {
            IWfProcessDescriptor processDesp = ProcessHelper.CreateSimpleProcessDescriptor();

            IWfActivityDescriptor normalActivity = processDesp.Activities["NormalActivity"];

            IWfTransitionDescriptor agreeTransition = normalActivity.ToTransitions[0];

            agreeTransition.AffectedProcessReturnValue = true;
            agreeTransition.AffectProcessReturnValue = true;
            agreeTransition.DefaultSelect = selectAgree;
            ((WfTransitionDescriptor)agreeTransition).Name = "同意";

            IWfTransitionDescriptor disagreeTransition = normalActivity.ToTransitions.AddForwardTransition(processDesp.CompletedActivity);

            disagreeTransition.AffectedProcessReturnValue = false;
            disagreeTransition.AffectProcessReturnValue = true;
            disagreeTransition.DefaultSelect = selectAgree == false;
            ((WfTransitionDescriptor)disagreeTransition).Name = "不同意";

            return processDesp;
        }
    }
}
