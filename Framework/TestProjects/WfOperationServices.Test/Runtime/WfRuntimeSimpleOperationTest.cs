using MCS.Library.Core;
using MCS.Library.WcfExtensions;
using MCS.Library.WF.Contracts.Common.Test;
using MCS.Library.WF.Contracts.DataObjects;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Operations;
using MCS.Library.WF.Contracts.Proxies;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WfOperationServices.Test.Runtime
{
    [TestClass]
    public class WfRuntimeSimpleOperationTest
    {
        [TestMethod]
        public void StartWorkflow()
        {
            WfClientProcessDescriptor processDesp = OperationHelper.PrepareSimpleProcess();

            WfClientProcessStartupParams clientStartupParams = ProcessRuntimeHelper.PrepareClientProcessStartupParams(processDesp.Key);

            WfClientProcessInfo processInfo = WfClientProcessRuntimeServiceProxy.Instance.StartWorkflow(clientStartupParams);

            processInfo.Output();
            AssertStartedProcess(processInfo, clientStartupParams);
        }

        [TestMethod]
        public void StartWorkflowWithoutPersist()
        {
            WfClientProcessDescriptor processDesp = OperationHelper.PrepareSimpleProcess();

            WfClientProcessStartupParams clientStartupParams = ProcessRuntimeHelper.PrepareClientProcessStartupParams(processDesp.Key);

            clientStartupParams.AutoPersist = false;

            WfClientProcessInfo processInfo = WfClientProcessRuntimeServiceProxy.Instance.StartWorkflow(clientStartupParams);

            processInfo.Output();
            AssertStartedProcess(processInfo, clientStartupParams);

            try
            {
                WfClientProcess process = WfClientProcessRuntimeServiceProxy.Instance.GetProcessByID(processInfo.ID, Consts.Users["Requestor"]);
            }
            catch (WfClientChannelException ex)
            {
                if (ex.Message.IndexOf("不能找到") == -1)
                    throw ex;
            }
        }

        [TestMethod]
        public void StartWorkflowWithOpinion()
        {
            WfClientProcessDescriptor processDesp = OperationHelper.PrepareSimpleProcess();

            WfClientProcessStartupParams clientStartupParams = ProcessRuntimeHelper.PrepareClientProcessStartupParams(processDesp.Key);
            clientStartupParams.Opinion = new WfClientOpinion() { Content = "我启动流程很高兴" };

            WfClientProcessInfo processInfo = WfClientProcessRuntimeServiceProxy.Instance.StartWorkflow(clientStartupParams);

            processInfo.Output();
            AssertStartedProcess(processInfo, clientStartupParams);

            WfClientOpinionCollection opinions = WfClientProcessRuntimeServiceProxy.Instance.GetOpinionsByResourceID(processInfo.ResourceID);

            Assert.IsTrue(opinions.Count > 0);
            Assert.AreEqual(processInfo.CurrentActivity.ID, opinions[0].ActivityID);
        }

        [TestMethod]
        public void StartWorkflowAndMoveToNext()
        {
            WfClientProcessDescriptor processDesp = OperationHelper.PrepareSimpleProcess();

            WfClientProcessStartupParams clientStartupParams = ProcessRuntimeHelper.PrepareClientProcessStartupParams(processDesp.Key);
            clientStartupParams.Opinion = new WfClientOpinion() { Content = "我启动流程很高兴" };

            WfClientProcessInfo processInfo = WfClientProcessRuntimeServiceProxy.Instance.StartWorkflowAndMoveTo(clientStartupParams, null, null);

            processInfo.Output();

            WfClientOpinionCollection opinions = WfClientProcessRuntimeServiceProxy.Instance.GetOpinionsByResourceID(processInfo.ResourceID);

            Assert.IsTrue(opinions.Count > 0);
            Assert.AreNotEqual(processInfo.CurrentActivity.ID, opinions[0].ActivityID);
        }

        [TestMethod]
        public void GetProcessByIDTest()
        {
            WfClientProcessInfo processInfo = OperationHelper.PreapreProcessWithConditionLinesInstance();
            WfClientProcess process = WfClientProcessRuntimeServiceProxy.Instance.GetProcessByID(processInfo.ID, Consts.Users["Requestor"]);

            processInfo.Output();

            Assert.AreEqual(processInfo.ID, processInfo.ID);
            Assert.AreEqual(processInfo.Status, processInfo.Status);

            Assert.AreEqual(processInfo.CurrentActivity.DescriptorKey, process.CurrentActivity.Descriptor.Key);
        }

        [TestMethod]
        public void GetProcessByResourceIDTest()
        {
            WfClientProcessInfo processInfo = OperationHelper.PreapreProcessWithConditionLinesInstance();
            WfClientProcess process = WfClientProcessRuntimeServiceProxy.Instance.GetProcessByResourceID(processInfo.ResourceID, Consts.Users["Requestor"]).First();

            Assert.AreEqual(processInfo.ID, processInfo.ID);
            Assert.AreEqual(processInfo.ResourceID, processInfo.ResourceID);
            Assert.AreEqual(processInfo.Status, processInfo.Status);

            Assert.AreEqual(processInfo.CurrentActivity.DescriptorKey, process.CurrentActivity.Descriptor.Key);
        }

        [TestMethod]
        public void GetProcessByAcvtivityIDTest()
        {
            WfClientProcessInfo processInfo = OperationHelper.PreapreProcessWithConditionLinesInstance();
            WfClientProcess process = WfClientProcessRuntimeServiceProxy.Instance.GetProcessByActivityID(processInfo.CurrentActivity.ID, Consts.Users["Requestor"]);

            Assert.AreEqual(processInfo.ID, processInfo.ID);
            Assert.AreEqual(processInfo.Status, processInfo.Status);

            Assert.AreEqual(processInfo.CurrentActivity.DescriptorKey, process.CurrentActivity.Descriptor.Key);
        }

        [TestMethod]
        public void GetProcessInfoByIDTest()
        {
            WfClientProcessInfo processInfo = OperationHelper.PreapreProcessWithConditionLinesInstance();
            WfClientProcessInfo process = WfClientProcessRuntimeServiceProxy.Instance.GetProcessInfoByID(processInfo.ID, Consts.Users["Requestor"]);

            process.Output();

            Assert.AreEqual(processInfo.ID, processInfo.ID);
            Assert.AreEqual(processInfo.Status, processInfo.Status);

            Assert.AreEqual(processInfo.CurrentActivity.DescriptorKey, process.CurrentActivity.Descriptor.Key);
        }

        [TestMethod]
        public void GetProcessInfoByActivityIDTest()
        {
            WfClientProcessInfo processInfo = OperationHelper.PreapreProcessWithConditionLinesInstance();
            WfClientProcessInfo process = WfClientProcessRuntimeServiceProxy.Instance.GetProcessInfoByActivityID(processInfo.CurrentActivity.ID, Consts.Users["Requestor"]);

            process.Output();

            Assert.AreEqual(processInfo.ID, processInfo.ID);
            Assert.AreEqual(processInfo.Status, processInfo.Status);

            Assert.AreEqual(processInfo.CurrentActivity.DescriptorKey, process.CurrentActivity.Descriptor.Key);
        }

        [TestMethod]
        public void GetProcessInfoByResourceIDTest()
        {
            WfClientProcessInfo processInfo = OperationHelper.PreapreProcessWithConditionLinesInstance();
            WfClientProcessInfo process = WfClientProcessRuntimeServiceProxy.Instance.GetProcessInfoByResourceID(processInfo.ResourceID, Consts.Users["Requestor"]).First();

            process.Output();

            Assert.AreEqual(processInfo.ID, processInfo.ID);
            Assert.AreEqual(processInfo.ResourceID, processInfo.ResourceID);
            Assert.AreEqual(processInfo.Status, processInfo.Status);

            Assert.AreEqual(processInfo.CurrentActivity.DescriptorKey, process.CurrentActivity.Descriptor.Key);
        }

        [TestMethod]
        public void MoveToNextDefaultActivity()
        {
            WfClientProcessInfo processInfo = OperationHelper.PreapreProcessWithConditionLinesInstance();

            WfClientRuntimeContext runtimeContext = new WfClientRuntimeContext();

            runtimeContext.ApplicationRuntimeParameters["Amount"] = 20000;
            runtimeContext.Operator = Consts.Users["Requestor"];

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.MoveToNextDefaultActivity(processInfo.ID, runtimeContext);
            processInfo.Output();

            Assert.AreEqual("N1", processInfo.CurrentActivity.Descriptor.Key);
            Assert.IsTrue(processInfo.CurrentActivity.Assignees.Contains(Consts.Users["CFO"].ID));
        }

        [TestMethod]
        public void MoveToOneStep()
        {
            WfClientProcessInfo processInfo = OperationHelper.PreapreProcessWithConditionLinesInstance();

            WfClientTransferParams transferParams = new WfClientTransferParams("N2");

            transferParams.Assignees.Add(Consts.Users["CEO"]);

            WfClientRuntimeContext runtimeContext = new WfClientRuntimeContext();
            runtimeContext.Operator = Consts.Users["Requestor"];

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.MoveTo(processInfo.ID, transferParams, runtimeContext);

            processInfo.Output();

            Assert.AreEqual("N2", processInfo.CurrentActivity.Descriptor.Key);
            Assert.IsTrue(processInfo.CurrentActivity.Assignees.Contains(Consts.Users["CEO"].ID));
        }

        [TestMethod]
        public void MoveToOneStepWithOpinion()
        {
            WfClientProcessInfo processInfo = OperationHelper.PreapreProcessWithConditionLinesInstance();

            WfClientTransferParams transferParams = new WfClientTransferParams("N2");

            transferParams.Assignees.Add(Consts.Users["CEO"]);

            WfClientRuntimeContext runtimeContext = new WfClientRuntimeContext();
            runtimeContext.Operator = Consts.Users["Requestor"];
            runtimeContext.Opinion = new WfClientOpinion() { Content = "我很高兴" };

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.MoveTo(processInfo.ID, transferParams, runtimeContext);

            processInfo.Output();

            Assert.AreEqual("N2", processInfo.CurrentActivity.Descriptor.Key);
            Assert.IsTrue(processInfo.CurrentActivity.Assignees.Contains(Consts.Users["CEO"].ID));

            WfClientProcessInfo prevProcessInfo = WfClientProcessRuntimeServiceProxy.Instance.GetProcessInfoByActivityID(processInfo.PreviousActivity.ID, runtimeContext.Operator);

            prevProcessInfo.Output();

            Assert.IsNotNull(prevProcessInfo.CurrentOpinion);

            WfClientOpinionCollection opinions = WfClientProcessRuntimeServiceProxy.Instance.GetOpinionsByResourceID(prevProcessInfo.ResourceID);

            Assert.IsTrue(opinions.Count > 0);
        }

        [TestMethod]
        public void MoveToOneStepWithOpinionThenSaveOpinion()
        {
            WfClientProcessInfo processInfo = OperationHelper.PreapreProcessWithConditionLinesInstance();

            WfClientTransferParams transferParams = new WfClientTransferParams("N2");

            transferParams.Assignees.Add(Consts.Users["CEO"]);

            WfClientRuntimeContext runtimeContext = new WfClientRuntimeContext();
            runtimeContext.Operator = Consts.Users["Requestor"];

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.MoveTo(processInfo.ID, transferParams, runtimeContext);

            processInfo.Output();

            Assert.AreEqual("N2", processInfo.CurrentActivity.Descriptor.Key);
            Assert.IsTrue(processInfo.CurrentActivity.Assignees.Contains(Consts.Users["CEO"].ID));

            runtimeContext.Opinion = new WfClientOpinion() { Content = "我很高兴" };

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.SaveProcess(processInfo.ID, runtimeContext);

            Assert.IsNotNull(processInfo.CurrentOpinion);
            Assert.IsTrue(processInfo.CurrentOpinion.ID.IsNotEmpty());

            runtimeContext.Opinion = processInfo.CurrentOpinion;
            runtimeContext.Opinion.Content = "我确实很高兴";

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.SaveProcess(processInfo.ID, runtimeContext);

            WfClientOpinionCollection opinions = WfClientProcessRuntimeServiceProxy.Instance.GetOpinionsByResourceID(processInfo.ResourceID);

            Assert.AreEqual(1, opinions.Count);
            Assert.AreEqual(runtimeContext.Opinion.ID, opinions.First().ID);
            Assert.AreEqual(runtimeContext.Opinion.Content, opinions.First().Content);
        }

        [TestMethod]
        public void ProcessWithWithActivityMatrixResourceTest()
        {
            WfClientProcessInfo processInfo = OperationHelper.PrepareProcessWithWithActivityMatrixResourceInstance();

            Assert.IsTrue(processInfo.NextActivities.Count > 0);
            Assert.IsTrue(processInfo.NextActivities[0].Activity.Candidates.Count > 0);
            Assert.AreEqual(Consts.Users["Approver1"].ID, processInfo.NextActivities[0].Activity.Candidates[0].User.ID);
        }

        [TestMethod]
        public void UpdateRuntimeParametersTest()
        {
            WfClientProcessInfo processInfo = OperationHelper.PreapreProcessWithConditionLinesInstance();

            WfClientRuntimeContext runtimeContext = new WfClientRuntimeContext();

            runtimeContext.ApplicationRuntimeParameters["Amount"] = 1000;
            runtimeContext.ApplicationRuntimeParameters["Users"] = new WfClientUser[] { Consts.Users["Requestor"], Consts.Users["Approver1"] };
            runtimeContext.Operator = Consts.Users["Requestor"];
            runtimeContext.ProcessContext["Context"] = "This is process context";
            runtimeContext.AutoCalculate = true;

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.UpdateRuntimeParameters(processInfo.ID, runtimeContext);
            processInfo.Output();

            Assert.AreEqual("N2", processInfo.NextActivities[0].Activity.DescriptorKey);

            IList usersInProcess = (IList)processInfo.ApplicationRuntimeParameters["Users"];

            Assert.IsTrue(usersInProcess.Count > 0);

            Assert.AreEqual(Consts.Users["Requestor"].ID, ((WfClientUser)usersInProcess[0]).ID);
            Assert.AreEqual(Consts.Users["Approver1"].ID, ((WfClientUser)usersInProcess[1]).ID);
        }

        [TestMethod]
        public void SaveProcessTest()
        {
            WfClientProcessInfo processInfo = OperationHelper.PreapreProcessWithConditionLinesInstance();

            WfClientRuntimeContext runtimeContext = new WfClientRuntimeContext();

            runtimeContext.ApplicationRuntimeParameters["Amount"] = 1000;
            runtimeContext.Operator = Consts.Users["Requestor"];
            runtimeContext.ProcessContext["Context"] = "This is process context";
            runtimeContext.AutoCalculate = true;

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.SaveProcess(processInfo.ID, runtimeContext);
            processInfo.Output();

            Assert.AreEqual("N2", processInfo.NextActivities[0].Activity.DescriptorKey);
        }

        [TestMethod]
        public void SaveProcessWithOpinionTest()
        {
            WfClientProcessInfo processInfo = OperationHelper.PreapreProcessWithConditionLinesInstance();

            WfClientRuntimeContext runtimeContext = new WfClientRuntimeContext();

            runtimeContext.ApplicationRuntimeParameters["Amount"] = 1000;
            runtimeContext.Operator = Consts.Users["Requestor"];
            runtimeContext.ProcessContext["Context"] = "This is process context";
            runtimeContext.Opinion = new WfClientOpinion() { Content = "我很高兴" };

            runtimeContext.AutoCalculate = true;

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.SaveProcess(processInfo.ID, runtimeContext);
            processInfo.Output();

            Assert.AreEqual("N2", processInfo.NextActivities[0].Activity.DescriptorKey);
            Assert.IsNotNull(processInfo.CurrentOpinion);

            WfClientOpinionCollection opinions = WfClientProcessRuntimeServiceProxy.Instance.GetOpinionsByProcessID(processInfo.ID);

            Assert.IsTrue(opinions.Count > 0);
        }

        [TestMethod]
        public void GetSimpleApplicationRuntimeParameterTest()
        {
            WfClientProcessInfo processInfo = OperationHelper.PreapreProcessWithConditionLinesInstance();

            WfClientRuntimeContext runtimeContext = new WfClientRuntimeContext();

            runtimeContext.ApplicationRuntimeParameters["Amount"] = 1000;
            runtimeContext.ApplicationRuntimeParameters["ProcessRequestor"] = Consts.Users["Requestor"];
            runtimeContext.Operator = Consts.Users["Requestor"];
            runtimeContext.ProcessContext["Context"] = "This is process context";
            runtimeContext.AutoCalculate = true;

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.UpdateRuntimeParameters(processInfo.ID, runtimeContext);
            processInfo.Output();

            int data = WfClientProcessRuntimeServiceProxy.Instance.GetApplicationRuntimeParameters(processInfo.ID, "Amount", WfClientProbeApplicationRuntimeParameterMode.Auto, 0);
            Console.WriteLine(data);
        }

        [TestMethod]
        public void WithdrawProcess()
        {
            WfClientProcessInfo processInfo = OperationHelper.PreapreProcessWithConditionLinesInstance();

            WfClientRuntimeContext runtimeContext = new WfClientRuntimeContext(Consts.Users["Requestor"]);

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.MoveToNextDefaultActivity(processInfo.ID, runtimeContext);

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.Withdraw(processInfo.ID, runtimeContext);

            processInfo.Output();

            Assert.AreEqual("Start", processInfo.CurrentActivity.Descriptor.Key);
        }

        [TestMethod]
        public void CancelProcess()
        {
            WfClientProcessInfo processInfo = OperationHelper.PreapreProcessWithConditionLinesInstance();

            WfClientRuntimeContext runtimeContext = new WfClientRuntimeContext(Consts.Users["Requestor"]);

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.MoveToNextDefaultActivity(processInfo.ID, runtimeContext);

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.Cancel(processInfo.ID, runtimeContext);

            processInfo.Output();

            Assert.AreEqual(WfClientProcessStatus.Aborted, processInfo.Status);
            Assert.AreEqual(WfClientActivityStatus.Aborted, processInfo.CurrentActivity.Status);
        }

        [TestMethod]
        public void InvalidUpdateTagTest()
        {
            WfClientProcessInfo processInfo = OperationHelper.PreapreProcessWithConditionLinesInstance();

            WfClientRuntimeContext runtimeContext = new WfClientRuntimeContext(Consts.Users["Requestor"]);

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.GetProcessInfoByID(processInfo.ID, Consts.Users["Requestor"]);

            processInfo.Output();

            runtimeContext.UpdateTag = processInfo.UpdateTag + 1; //Invalid UpdateTag

            bool thrown = false;

            try
            {
                WfClientProcessRuntimeServiceProxy.Instance.MoveToNextDefaultActivity(processInfo.ID, runtimeContext);
            }
            catch (System.Exception ex)
            {
                thrown = true;
                Assert.IsTrue(ex.Message.IndexOf("流程状态已经改变") >= 0);
            }

            Assert.IsTrue(thrown);
        }

        [TestMethod]
        public void CancelProcessWithOpinion()
        {
            WfClientProcessInfo processInfo = OperationHelper.PreapreProcessWithConditionLinesInstance();

            WfClientRuntimeContext runtimeContext = new WfClientRuntimeContext(Consts.Users["Requestor"]);

            runtimeContext.Opinion = new WfClientOpinion() { Content = "我很高兴" };

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.MoveToNextDefaultActivity(processInfo.ID, runtimeContext);

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.Cancel(processInfo.ID, runtimeContext);

            processInfo.Output();

            Assert.AreEqual(WfClientProcessStatus.Aborted, processInfo.Status);
            Assert.IsNotNull(processInfo.CurrentOpinion);
        }

        [TestMethod]
        public void RestoreProcess()
        {
            WfClientProcessInfo processInfo = OperationHelper.PreapreProcessWithConditionLinesInstance();
            WfClientRuntimeContext runtimeContext = new WfClientRuntimeContext(Consts.Users["Requestor"]);

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.MoveToNextDefaultActivity(processInfo.ID, runtimeContext);

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.Cancel(processInfo.ID, runtimeContext);

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.Restore(processInfo.ID, runtimeContext);

            processInfo.Output();

            Assert.AreEqual(WfClientProcessStatus.Running, processInfo.Status);
            Assert.AreEqual(WfClientActivityStatus.Running, processInfo.CurrentActivity.Status);
        }

        [TestMethod]
        public void PauseProcess()
        {
            WfClientProcessInfo processInfo = OperationHelper.PreapreProcessWithConditionLinesInstance();
            WfClientRuntimeContext runtimeContext = new WfClientRuntimeContext(Consts.Users["Requestor"]);

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.MoveToNextDefaultActivity(processInfo.ID, runtimeContext);

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.Pause(processInfo.ID, runtimeContext);

            processInfo.Output();

            Assert.AreEqual(WfClientProcessStatus.Paused, processInfo.Status);
        }

        [TestMethod]
        public void ResumeProcess()
        {
            WfClientProcessInfo processInfo = OperationHelper.PreapreProcessWithConditionLinesInstance();
            WfClientRuntimeContext runtimeContext = new WfClientRuntimeContext(Consts.Users["Requestor"]);

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.MoveToNextDefaultActivity(processInfo.ID, runtimeContext);

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.Pause(processInfo.ID, runtimeContext);

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.Resume(processInfo.ID, runtimeContext);

            processInfo.Output();

            Assert.AreEqual(WfClientProcessStatus.Running, processInfo.Status);
            Assert.AreEqual(WfClientActivityStatus.Running, processInfo.CurrentActivity.Status);
        }

        [TestMethod]
        public void ReplaceAssignees()
        {
            WfClientProcessInfo processInfo = OperationHelper.PreapreProcessWithConditionLinesInstance();
            WfClientRuntimeContext runtimeContext = new WfClientRuntimeContext(Consts.Users["Requestor"]);

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.MoveToNextDefaultActivity(processInfo.ID, runtimeContext);

            processInfo = WfClientProcessRuntimeServiceProxy.Instance.ReplaceAssignees(
                processInfo.CurrentActivity.ID, null, new WfClientUser[] { Consts.Users["CEO"] }, runtimeContext);

            processInfo.Output();

            Assert.AreEqual(Consts.Users["CEO"].ID, processInfo.CurrentActivity.Assignees[0].User.ID);
        }

        /// <summary>
        /// 检查
        /// </summary>
        /// <param name="processInfo"></param>
        /// <param name="clientStartupParams"></param>
        private void AssertStartedProcess(WfClientProcessInfo processInfo, WfClientProcessStartupParams clientStartupParams)
        {
            Assert.IsTrue(processInfo.ID.IsNotEmpty());
            Assert.AreEqual(clientStartupParams.ProcessDescriptorKey, processInfo.ProcessDescriptorKey);
            Assert.AreEqual(WfClientProcessStatus.Running, processInfo.Status);
            Assert.IsNotNull(processInfo.CurrentActivity);

            processInfo.Creator.AreSame(clientStartupParams.Assignees[0].User);

            Assert.IsNotNull(processInfo.CurrentActivity);
            processInfo.CurrentActivity.Assignees.AreSame(clientStartupParams.Assignees, false);

            Assert.IsNotNull(processInfo.CurrentActivity.Descriptor);

            Assert.IsTrue(processInfo.NextActivities.Count > 0);
            Assert.IsNotNull(processInfo.NextActivities[0].Transition);
            Assert.IsNotNull(processInfo.NextActivities[0].Activity);

            processInfo.ApplicationRuntimeParameters.AssertContains(clientStartupParams.ApplicationRuntimeParameters);
            processInfo.ProcessContext.AssertContains(clientStartupParams.ProcessContext);
        }


        [TestMethod()]
        public void QueryBranchProcessesTest()
        {
            //建立流程
            int pageSize = 1;

            WfClientProcessInfo process = OperationHelper.PrepareBranchProcesses();

            Assert.IsTrue(process.MainStreamActivityDescriptors["N1"].BranchProcessGroupsCount > 0);

            string id = process.CurrentActivity.ID;
            //获取子流程
            WfClientProcessCurrentInfoPageQueryResult result = WfClientProcessRuntimeServiceProxy.Instance.QueryBranchProcesses(id, string.Empty, 0, pageSize, string.Empty, -1);

            //检查
            //分页是否正确,分支流程个数是否正确
            Assert.AreEqual(pageSize, result.QueryResult.Count());
            Assert.AreEqual(process.CurrentActivity.Assignees.Count, result.TotalCount);
        }

        [TestMethod()]
        public void QueryProcessesTest()
        {
            WfClientProcessQueryCondition condition = new WfClientProcessQueryCondition();

            condition.BeginStartTime = DateTime.Now.AddHours(-1);
            condition.EndStartTime = DateTime.Now.AddHours(1);

            WfClientProcessInfo process = OperationHelper.PrepareSimpleProcessInstance();

            WfClientProcessCurrentInfoPageQueryResult result = WfClientProcessRuntimeServiceProxy.Instance.QueryProcesses(condition, 0, 1, string.Empty, -1);

            Assert.IsTrue(result.QueryResult.ContainsKey(process.ID));
        }
    }
}