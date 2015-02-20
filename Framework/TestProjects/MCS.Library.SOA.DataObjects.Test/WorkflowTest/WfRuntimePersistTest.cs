using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest
{
    /// <summary>
    /// Summary description for WfRuntimePersist
    /// </summary>
    [TestClass]
    public class WfRuntimePersistTest
    {
        public WfRuntimePersistTest()
        {
        }

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

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        [TestCategory(ProcessTestHelper.ProcessBehavior_Persist)]
        public void SimpleProcessPersistTest()
        {
            IWfProcess process = WfProcessTestCommon.StartupSimpleProcessWithAssignee();

            //process.DefaultTaskTitle = "测试保存的流程";

            ProcessTestHelper.OutputExecutionTime(() => WfRuntime.PersistWorkflows(), "保存简单流程");

            try
            {
                IWfProcess loadedProcess = WfRuntime.GetProcessByProcessID(process.ID);

                Assert.AreEqual(process.ID, loadedProcess.ID);
                Assert.AreEqual(process.Status, loadedProcess.Status);
                Assert.AreEqual(DataLoadingType.External, loadedProcess.LoadingType);

                WfTransferParams transferParams = new WfTransferParams(process.Descriptor.CompletedActivity);

                transferParams.Assignees.Add(new WfAssignee((IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object));
                transferParams.Assignees.Add(new WfAssignee((IUser)OguObjectSettings.GetConfig().Objects["approver2"].Object));

                loadedProcess.MoveTo(transferParams);
                WfRuntime.PersistWorkflows();

                Assert.AreEqual(process.ID, loadedProcess.ID);
                Assert.AreEqual(WfProcessStatus.Completed, loadedProcess.Status);
                Assert.AreEqual(DataLoadingType.External, loadedProcess.LoadingType);
            }
            finally
            {
                //WfRuntime.DeleteProcessByProcessID(process.ID);	
            }
        }

        [TestMethod]
        [TestCategory(ProcessTestHelper.ProcessBehavior_Persist)]
        public void SimpleProcessPersistRepeatTest()
        {
            IWfProcess process = WfProcessTestCommon.StartupSimpleProcessWithAssignee();
            //process.DefaultTaskTitle = "测试保存的流程";
            WfRuntime.PersistWorkflows();

            IWfProcess loadedProcess = WfRuntime.GetProcessByProcessID(process.ID);
            IWfProcess loadedProcess2 = WfRuntime.GetProcessByActivityID(process.Activities[0].ID);

            Assert.AreEqual(loadedProcess, loadedProcess2);

            int n = loadedProcess.Activities.Count;
            for (int i = 0; i < n; i++)
            {

                if (loadedProcess.Activities[i].Descriptor != loadedProcess.Descriptor.InitialActivity)
                {
                    WfTransferParams transferPara = new WfTransferParams(loadedProcess.Activities[i].Descriptor);
                    loadedProcess.MoveTo(transferPara);
                    WfRuntime.PersistWorkflows();
                }

                loadedProcess = WfRuntime.GetProcessByActivityID(loadedProcess.Activities[i].ID);
            }

            IWfProcess OnceAgainloadedProcess = WfRuntime.GetProcessByProcessID(process.ID);
            Assert.AreEqual(OnceAgainloadedProcess.ID, process.ID);
        }

        [TestMethod]
        public void ProcessWithReturnLineSerializationTest()
        {
            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithReturnLine();

            XElementFormatter formatter = new XElementFormatter();

            XElement root = formatter.Serialize(processDesp);

            Console.WriteLine(root.ToString());
            processDesp = (IWfProcessDescriptor)formatter.Deserialize(root);

            IWfActivityDescriptor normalActDesp = processDesp.Activities["NormalActivity"];

            ToTransitionsDescriptorCollection transitions = normalActDesp.ToTransitions;

            foreach (string key in transitions.GetAllKeys())
            {
                Assert.IsTrue(key.IsNotEmpty());

                IWfTransitionDescriptor transition = transitions[key];

                Assert.AreEqual(key, transition.Key);
            }
        }

        [TestMethod]
        public void ExpenseProcessSerializationTest()
        {
            IWfProcessDescriptor processDesp = WfProcessTestCommon.LoadProcessDescriptorFromFile("Expense.xml");

            IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

            process.ApplicationRuntimeParameters["Amount"] = 500;

            WfRuntime.PersistWorkflows();

            process = WfRuntime.GetProcessByProcessID(process.ID);

            WfProcessTestCommon.MoveToNextDefaultActivityWithExecutor(process);

            process = WfRuntime.GetProcessByProcessID(process.ID);

            IWfActivityDescriptor currentActDesp = process.CurrentActivity.Descriptor;

            ToTransitionsDescriptorCollection transitions = currentActDesp.ToTransitions;

            foreach (string key in transitions.GetAllKeys())
            {
                Assert.IsTrue(key.IsNotEmpty());

                IWfTransitionDescriptor transition = transitions[key];

                Assert.AreEqual(key, transition.Key);
            }
        }

        [TestMethod]
        [Description("分支流程的保存测试")]
        [TestCategory(ProcessTestHelper.ProcessBehavior_Persist)]
        public void BranchProcessPersistTest()
        {
            IWfProcess process = WfProcessTestCommon.StartupSimpleProcessWithAssignee();

            WfTransferParams tp = ProcessTestHelper.GetInstanceOfWfTransferParams(process, WfBranchProcessExecuteSequence.Parallel, WfBranchProcessBlockingType.WaitAllBranchProcessesComplete);
            process.MoveTo(tp);

            Assert.AreEqual(1, process.CurrentActivity.BranchProcessGroups.Count);
            IWfBranchProcessGroup group = process.CurrentActivity.BranchProcessGroups[0];

            AssertBranchProcessesInfo(group, tp.BranchTransferParams[0].Template.BranchProcessKey);

            WfRuntime.PersistWorkflows();

            process = WfRuntime.GetProcessByProcessID(process.ID);

            group = process.CurrentActivity.BranchProcessGroups[0];

            AssertBranchProcessesInfo(group, tp.BranchTransferParams[0].Template.BranchProcessKey);

            string subProcessID = process.CurrentActivity.BranchProcessGroups[0].Branches[0].ID;

            WfRuntime.ClearCache();

            //构造子流程，反向查找主流程信息
            IWfProcess subProcess = WfRuntime.GetProcessByProcessID(subProcessID);

            AssertBranchProcessesInfo(subProcess.EntryInfo.OwnerActivity.BranchProcessGroups[0], tp.BranchTransferParams[0].Template.BranchProcessKey);
        }

        private static void AssertBranchProcessesInfo(IWfBranchProcessGroup group, string expectedProcessKey)
        {
            Assert.AreEqual(group.OwnerActivity, group.Branches[0].EntryInfo.OwnerActivity);
            Assert.AreEqual(group.OwnerActivity, group.Branches[1].EntryInfo.OwnerActivity);

            Assert.AreEqual(expectedProcessKey, group.ProcessTemplate.BranchProcessKey);
            Assert.AreEqual(2, group.Branches.Count);
            Assert.AreEqual(expectedProcessKey, group.Branches[0].Descriptor.Key);
            Assert.AreEqual(expectedProcessKey, group.Branches[1].Descriptor.Key);
            Assert.AreEqual(WfProcessStatus.Running, group.Branches[0].Status);
            Assert.AreEqual(WfProcessStatus.Running, group.Branches[1].Status);

            //以下的断言二次执行会出错，因为再次加载的时候其分支流程会存在顺序关系
            Assert.AreEqual(OguObjectSettings.GetConfig().Objects["approver1"].Object.ID, group.Branches[0].InitialActivity.Assignees[0].User.ID);
            Assert.AreEqual(OguObjectSettings.GetConfig().Objects["approver2"].Object.ID, group.Branches[1].InitialActivity.Assignees[0].User.ID);
        }

        [TestMethod]
        [Description("流程保存的执行时间测试")]
        [TestCategory(ProcessTestHelper.ExecuteTime)]
        public void ProcessPersistExecutionTimeTest()
        {
            IWfProcessDescriptor processDesc = WfProcessTestCommon.CreateSimpleProcessDescriptor();

            int totalProcesses = 40;

            //准备流程实例
            List<IWfProcess> processes = ProcessTestHelper.StartupMultiProcesses(processDesc, totalProcesses);

            ProcessTestHelper.OutputExecutionTime(() =>
            {
                WfRuntime.PersistWorkflows();
            },
            string.Format("保存{0}个流程", totalProcesses));
        }

        [TestMethod]
        [Description("子流程的待办事项测试")]
        [TestCategory(ProcessTestHelper.BranchProcess)]
        public void BranchProcessUserTasksTest()
        {
            //启支流程及其子流程
            IWfProcess process = WfProcessTestCommon.StartupSimpleProcessWithAssignee();

            WfTransferParams tp = ProcessTestHelper.GetInstanceOfWfTransferParams(process, WfBranchProcessExecuteSequence.Parallel, WfBranchProcessBlockingType.WaitAllBranchProcessesComplete);
            process.MoveTo(tp);
            WfRuntime.PersistWorkflows();

            process = WfRuntime.GetProcessByProcessID(process.ID);

            IWfBranchProcessGroup group = process.CurrentActivity.BranchProcessGroups[0];

            AssertBranchProcessesInfo(group, tp.BranchTransferParams[0].Template.BranchProcessKey);

            UserTaskCollection currentActTasks =
                    UserTaskAdapter.Instance.LoadUserTasks(builder =>
                    {
                        builder.LogicOperator = Data.Builder.LogicOperatorDefine.Or;

                        foreach (IWfProcess subProcess in group.Branches)
                        {
                            builder.AppendItem("ACTIVITY_ID", subProcess.CurrentActivity.ID);
                        }
                    });


            foreach (IWfProcess subProcess in group.Branches)
            {
                bool containsTask = false;

                group.Branches.ForEach(p =>
                {
                    containsTask = currentActTasks.Exists(u => u.ActivityID == p.CurrentActivity.ID);

                    Assert.IsTrue(containsTask);
                });
            }
        }

        [TestMethod]
        [Description("流程中活动节点的人员测试")]
        [TestCategory(ProcessTestHelper.Data)]
        public void ProcessAssigneesTest()
        {
            IWfProcess process = WfProcessTestCommon.StartupSimpleProcessDescriptorWithTransitionCondition();

            IWfActivityDescriptor nextActivityDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
            WfTransferParams transferParams = ProcessTestHelper.GetInstanceOfWfTransferParams(nextActivityDesp, OguObject.requestor);
            process.MoveTo(transferParams);

            WfRuntime.PersistWorkflows();

            process = WfRuntime.GetProcessByProcessID(process.ID);

            Assert.IsTrue(process.CurrentActivity.Assignees.Count > 0);
            //WfProcessCurrentInfoCollection coll = WfProcessCurrentInfoAdapter.Instance.Load(true, process.ID);

            //Assert.AreEqual(process.CurrentActivity.Assignees.Count, coll[0].Assignees.Count);
            //Assert.AreEqual(process.CurrentActivity.Assignees[0].User.Name, coll[0].Assignees[0].User.Name);
        }

        [TestMethod]
        [Description("流程中活动节点的人员测试")]
        [TestCategory(ProcessTestHelper.Data)]
        public void QueryUserRelativeRunningProcessesTest()
        {
            IUser testUser = (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object;

            DataTable table = WfProcessCurrentInfoAdapter.Instance.QueryUserRelativeRunningProcesses(testUser.ID);

            Console.WriteLine(table.Rows.Count);

            foreach (DataRow row in table.Rows)
            {
                Console.WriteLine("ProcessID={0}, TaskID={1}, TaskTitke={2}, isTask={3}",
                    row["INSTANCE_ID"], row["LASTEST_TASK_ID"], row["TASK_TITLE"], row["IS_TASK"]);
            }
        }

        [TestMethod]
        [Description("测试已办")]
        [TestCategory(ProcessTestHelper.Runtime)]
        public void ProcessTaskAccomplishedOfResourceTest()
        {
            IUser user = (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object;

            IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();
            ((WfProcess)process).ResourceID = UuidHelper.NewUuidString();

            IWfActivityDescriptor actDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
            WfTransferParams tp = new WfTransferParams(actDesp);
            tp.Assignees.Add(user);
            process.MoveTo(tp);        //设置 同一人
            WfRuntime.PersistWorkflows();

            process = WfRuntime.GetProcessByProcessID(process.ID);
            actDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
            tp = ProcessTestHelper.GetInstanceOfWfTransferParams(actDesp, OguObject.approver2);
            process.MoveTo(tp);
            WfRuntime.PersistWorkflows();

            process = WfRuntime.GetProcessByProcessID(process.ID);
            actDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
            tp = new WfTransferParams(actDesp);
            tp.Assignees.Add(user);   //设置 同一人
            WfRuntime.PersistWorkflows();


            process = WfRuntime.GetProcessByProcessID(process.ID);
            actDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
            tp = ProcessTestHelper.GetInstanceOfWfTransferParams(actDesp, OguObject.approver2);
            process.MoveTo(tp);
            WfRuntime.PersistWorkflows();

            process = WfRuntime.GetProcessByProcessID(process.ID);

            Assert.AreEqual(1, ProcessTestHelper.GetResourceUserTasksAccomplished(process.ResourceID, user.ID), "如果ResourceID相同，且流转过同一个人两次，他只会剩下一个已办");

        }

        public void AddProcess()
        {
            string processID = "182364d3-2e59-bc73-40e9-fb94672d21a1";
            IWfProcess process = WfRuntime.GetProcessByProcessID(processID);

            string activityKey = process.Descriptor.FindNotUsedActivityKey();
            WfActivityDescriptor actDesp = new WfActivityDescriptor(activityKey, WfActivityType.NormalActivity);

            process.CurrentActivity.Append(actDesp);
            WfRuntime.PersistWorkflows();

            activityKey = process.Descriptor.FindNotUsedActivityKey();
            actDesp = new WfActivityDescriptor(activityKey, WfActivityType.NormalActivity);

            process.CurrentActivity.Append(actDesp);


            process = WfRuntime.GetProcessByProcessID(processID);
            WfTransferParams tp = ProcessTestHelper.GetInstanceOfWfTransferParams(process, WfBranchProcessExecuteSequence.Parallel, WfBranchProcessBlockingType.WaitAnyoneBranchProcessComplete);
            process.MoveTo(tp);
            WfRuntime.PersistWorkflows();
        }

        [TestMethod]
        [Description("流程中拷贝操作")]
        [TestCategory(ProcessTestHelper.Runtime)]
        public void ActivityDespClone()
        {
            IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();
            ProcessTestHelper.MoveToAndSaveNextActivity(OguObject.approver1, process);


            IWfProcess proc = WfRuntime.GetProcessByProcessID(process.ID);
            IWfActivityDescriptor actDesp = proc.CurrentActivity.Descriptor.Clone();//拷贝一个描述
            WfRuntime.ProcessContext.ActivityChangingContext.CreatorInstanceID = proc.CurrentActivity.ID;
            WfRuntime.ProcessContext.ActivityChangingContext.AssociatedActivityKey = proc.CurrentActivity.Descriptor.Key;
            proc.CurrentActivity.Append(actDesp);
            ProcessTestHelper.MoveToAndSaveNextActivity(OguObject.approver2, proc);

            IWfProcess pro = WfRuntime.GetProcessByProcessID(process.ID);
            Assert.AreEqual(pro.Descriptor.GetMainStreamActivities().Count + 1, pro.Activities.Count);//7
            Assert.IsTrue(pro.CurrentActivity.CreatorInstanceID.IsNotEmpty(), "运行到动态添加的活动上");

            Assert.AreEqual(pro.ElapsedActivities[1].Descriptor.ToTransitions[0].ToActivity.Key, pro.CurrentActivity.Descriptor.Key);
            Assert.IsNotNull(pro.CurrentActivity.Descriptor.ToTransitions[0].ToActivity.Key);
        }

        [TestMethod]
        [Description("流程中删除操作，单一")]
        [TestCategory(ProcessTestHelper.Runtime)]
        public void SingleActivityDelete()
        {
            IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();
            ProcessTestHelper.MoveToAndSaveNextActivity(OguObject.requestor, process);

            IWfProcess proc = WfRuntime.GetProcessByProcessID(process.ID);
            proc.CurrentActivity.Delete();
            Assert.AreEqual(0, proc.ElapsedActivities.Count);
            Assert.IsNotNull(proc.CurrentActivity.Descriptor.Key);

            IWfProcess p = WfRuntime.GetProcessByProcessID(proc.ID);
            ProcessTestHelper.MoveToAndSaveNextActivity(OguObject.approver1, p);

            IWfProcess wp = WfRuntime.GetProcessByProcessID(p.ID);
            Assert.AreEqual(wp.Activities.Count, wp.Descriptor.GetMainStreamActivities().Count);
            Assert.AreEqual(wp.ElapsedActivities[wp.ElapsedActivities.Count - 1].Descriptor.Key, wp.CurrentActivity.Descriptor.FromTransitions[0].FromActivity.Key);

        }

        [TestMethod]
        [Description("流程中删除操作,组合")]
        [TestCategory(ProcessTestHelper.Runtime)]
        public void ActivityDelete()
        {
            //创建一个六个活动的流程
            IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();
            ProcessTestHelper.MoveToAndSaveNextActivity(OguObject.approver1, process);

            IWfProcess proc = WfRuntime.GetProcessByProcessID(process.ID);
            ProcessTestHelper.MoveToAndSaveNextActivity(OguObject.requestor, proc);

            //移动到了第三个活动
            //当前活动是NormalActivity1
            proc = WfRuntime.GetProcessByProcessID(process.ID);

            int orginalActivityCount = proc.Activities.Count;

            proc.CurrentActivity.Delete();

            Assert.AreEqual(orginalActivityCount - 1, proc.Descriptor.GetMainStreamActivities().Count);

            proc = WfRuntime.GetProcessByProcessID(process.ID);
            Assert.AreEqual(proc.Activities.Count, proc.Descriptor.GetMainStreamActivities().Count, "删除一个主线活动NormalActivity1");
            Assert.AreEqual(proc.ElapsedActivities[proc.ElapsedActivities.Count - 1].Descriptor.ToTransitions[0].ToActivity.Key, proc.CurrentActivity.Descriptor.Key);//NormalActivity1

            proc = WfRuntime.GetProcessByProcessID(proc.ID);
            IWfActivityDescriptor actDesp = proc.CurrentActivity.Descriptor.Clone();
            WfRuntime.ProcessContext.ActivityChangingContext.CreatorInstanceID = proc.CurrentActivity.ID;
            WfRuntime.ProcessContext.ActivityChangingContext.AssociatedActivityKey = proc.CurrentActivity.Descriptor.Key;

            proc.CurrentActivity.Append(actDesp); //添加一个活动
            ProcessTestHelper.MoveToAndSaveNextActivity(OguObject.approver2, proc);

            proc = WfRuntime.GetProcessByProcessID(process.ID);

            Assert.AreEqual(actDesp.Key, proc.CurrentActivity.Descriptor.Key);
            Assert.AreEqual(proc.Activities.Count - 1, proc.Descriptor.GetMainStreamActivities().Count);

            proc.CurrentActivity.Delete();

            proc = WfRuntime.GetProcessByProcessID(proc.ID);
            Assert.AreEqual(proc.Activities.Count, proc.Descriptor.GetMainStreamActivities().Count);
            ProcessTestHelper.MoveToAndSaveNextActivity(OguObject.approver1, proc);

            Assert.AreEqual(2, proc.ElapsedActivities.Count);
        }

        [TestMethod]
        public void PersistProcessWithTenantCode()
        {
            IWfProcess process = WfProcessTestCommon.StartupSimpleProcessWithAssignee();

            TenantContext.Current.TenantCode = "1001";
            ProcessTestHelper.OutputExecutionTime(() => WfRuntime.PersistWorkflows(), "保存简单的带TenantCode的流程");
        }
    }
}
