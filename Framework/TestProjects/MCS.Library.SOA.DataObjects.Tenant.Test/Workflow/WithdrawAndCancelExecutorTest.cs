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
    public class WithdrawAndCancelExecutorTest
    {
        [TestMethod]
        public void NormalWithdrawTest()
        {
            IWfProcess process = ProcessHelper.CreateFreeStepsProcessDescriptor(OguObjectSettings.GetConfig().Objects["approver1"].User).StartupProcess();

            process.MoveToDefaultActivityByExecutor();

            process = WfRuntime.GetProcessByProcessID(process.ID);

            string activityIDBeforeWithdraw = process.CurrentActivity.ID;

            Assert.AreEqual(WfActivityType.NormalActivity, process.CurrentActivity.Descriptor.ActivityType);

            Console.WriteLine("ActivityID Before Withdraw = {0}", activityIDBeforeWithdraw);

            process = process.WithdrawByExecutor();

            Assert.AreEqual(WfProcessStatus.Running, process.Status);
            Assert.AreEqual(WfActivityType.InitialActivity, process.CurrentActivity.Descriptor.ActivityType);

            UserTaskCollection notifies = UserTaskAdapter.Instance.GetUserTasks(UserTaskIDType.ActivityID, UserTaskFieldDefine.ActivityID | UserTaskFieldDefine.Status, activityIDBeforeWithdraw);

            Assert.AreEqual(1, notifies.Count);
            Assert.AreEqual(TaskStatus.Yue, notifies[0].Status);
        }

        [TestMethod]
        public void WithdrawAndCancelTest()
        {
            IWfProcess process = ProcessHelper.CreateFreeStepsProcessDescriptor(OguObjectSettings.GetConfig().Objects["approver1"].User).StartupProcess();

            process.MoveToDefaultActivityByExecutor();

            process = WfRuntime.GetProcessByProcessID(process.ID);

            string activityIDBeforeWithdraw = process.CurrentActivity.ID;

            Assert.AreEqual(WfActivityType.NormalActivity, process.CurrentActivity.Descriptor.ActivityType);

            Console.WriteLine("ActivityID Before Withdraw = {0}, Process ID = {1}", activityIDBeforeWithdraw, process.ID);

            process = process.WithdrawByExecutor(true);

            Assert.AreEqual(WfActivityType.InitialActivity, process.CurrentActivity.Descriptor.ActivityType);

            Assert.AreEqual(WfProcessStatus.Aborted, process.Status);

            UserTaskCollection notifies = UserTaskAdapter.Instance.GetUserTasks(UserTaskIDType.ActivityID, UserTaskFieldDefine.ActivityID | UserTaskFieldDefine.Status, activityIDBeforeWithdraw);

            Assert.AreEqual(1, notifies.Count);

            UserTaskCollection tasks = UserTaskAdapter.Instance.GetUserTasks(UserTaskIDType.ActivityID, UserTaskFieldDefine.ActivityID | UserTaskFieldDefine.Status, process.InitialActivity.ID);

            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual(TaskStatus.Yue, tasks[0].Status);

            UserTaskCollection accomplishedTasks = UserTaskAdapter.Instance.GetUserAccomplishedTasks(UserTaskIDType.ActivityID, UserTaskFieldDefine.ActivityID | UserTaskFieldDefine.Status, false, activityIDBeforeWithdraw);

            Assert.AreEqual(1, tasks.Count);
        }
    }
}
