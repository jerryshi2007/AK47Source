﻿using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Tenant.Test.Workflow.Helper;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.Workflow
{
    /// <summary>
    /// 流程运行时的单元测试（不包含Executor）
    /// </summary>
    [TestClass]
    public class ProcessRuntimeTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [Description("运行时活动上的条件判断")]
        [TestMethod]
        public void ActivityConditionTest()
        {
            WfPendingActivityInfoAdapter.Instance.ClearAll();

            TestContext.Properties["Amount"] = 1000;

            IWfProcessDescriptor processDesp = ProcessHelper.CreateSimpleProcessDescriptor();

            processDesp.Activities["NormalActivity"].Condition.Expression = "Amount > 5000";

            WfRuntime.ProcessContext.EvaluateActivityCondition += new Expression.CalculateUserFunction(ProcessContext_EvaluateTransition);

            IWfProcess process = processDesp.StartupProcess();

            IWfActivity normalActivity = process.MoveToNextDefaultActivity();

            Console.WriteLine(normalActivity.Status);

            Assert.AreEqual(WfActivityStatus.Pending, normalActivity.Status);

            WfRuntime.PersistWorkflows();

            string activityID = process.CurrentActivity.ID;

            WfPendingActivityInfoCollection pendingActivities = WfPendingActivityInfoAdapter.Instance.Load(builder => builder.AppendItem("ACTIVITY_ID", process.CurrentActivity.ID));

            Assert.IsTrue(pendingActivities.Count > 0);

            TestContext.Properties["Amount"] = 8000;

            pendingActivities.ForEach(pa => WfRuntime.ProcessPendingActivity(pa));

            process = WfRuntime.GetProcessByProcessID(process.ID);

            Assert.AreEqual(WfActivityStatus.Completed, process.Activities[activityID].Status);
        }

        [Description("流程办结时的通知测试")]
        [TestMethod]
        public void CompleteProcessNotifyTest()
        {
            IWfProcessDescriptor processDesp = ProcessHelper.CreateSimpleProcessDescriptor();

            processDesp.CompleteEventReceivers.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object));

            IWfProcess process = processDesp.StartupProcess();

            process = process.MoveToDefaultActivityByExecutor();

            process = process.MoveToDefaultActivityByExecutor();

            Assert.AreEqual(WfProcessStatus.Completed, process.Status);

            UserTask notify = UserTaskAdapter.Instance.LoadUserTasks(builder => builder.AppendItem("ACTIVITY_ID", process.CompletedActivity.ID)).FirstOrDefault();

            Assert.IsNotNull(notify);

            Assert.AreEqual(TaskStatus.Yue, notify.Status);
            Assert.AreEqual(OguObjectSettings.GetConfig().Objects["requestor"].Object.ID, notify.SendToUserID);
        }

        private object ProcessContext_EvaluateTransition(string funcName, Expression.ParamObjectCollection arrParams, object callerContext)
        {
            object result = null;

            switch (funcName.ToLower())
            {
                case "amount":
                    result = TestContext.Properties["Amount"];
                    break;
            }

            return result;
        }
    }
}
