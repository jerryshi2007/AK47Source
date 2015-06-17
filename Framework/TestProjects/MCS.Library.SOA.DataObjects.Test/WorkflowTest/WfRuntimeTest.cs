using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Core;
using System.Xml.Linq;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Test
{
    /// <summary>
    /// Summary description for WfRuntimeTest
    /// </summary>
    [TestClass]
    public class WfRuntimeTest
    {
        public WfRuntimeTest()
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
        [Description("启动流程时，查看初始节点的状态")]
        [TestCategory(ProcessTestHelper.ProcessBehavior_Start)]
        public void SimpleProcessStartTest()
        {
            IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

            Assert.AreEqual(WfActivityStatus.Running, process.InitialActivity.Status);
        }

        [TestMethod]
        [Description("移动到结束点，查看流程及节点状态")]
        [TestCategory(ProcessTestHelper.ProcessBehavior_Moveto)]
        public void SimpleProcessMoveTest()
        {
            IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

            WfTransferParams transferParams = new WfTransferParams(process.Descriptor.CompletedActivity);

            process.MoveTo(transferParams);

            Assert.IsTrue(process.InitialActivity.Status == WfActivityStatus.Completed);
            Assert.IsTrue(process.Status == WfProcessStatus.Completed);
            Assert.IsTrue(process.CompletedActivity.Status == WfActivityStatus.Completed);
        }

        [TestMethod]
        [Description("将第二个点配置成自动流转点")]
        [TestCategory(ProcessTestHelper.ProcessBehavior_Moveto)]
        public void NormalActivityAutoMoveTest()
        {
            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

            processDesp.Activities["NormalActivity"].Properties.SetValue("AutoMoveTo", true);

            IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

            IWfActivity currentActivity = process.MoveToNextDefaultActivity();

            Console.WriteLine(currentActivity.Descriptor.ActivityType);

            Assert.AreEqual(WfActivityType.CompletedActivity, currentActivity.Descriptor.ActivityType);
        }

        [TestMethod]
        [Description("将第二个点配置成自动流转点")]
        [TestCategory(ProcessTestHelper.ProcessBehavior_Moveto)]
        public void InitialActivityAutoMoveTest()
        {
            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

            processDesp.InitialActivity.Properties.SetValue("AutoMoveTo", true);
            processDesp.Activities["NormalActivity"].Properties.SetValue("AutoMoveTo", true);

            IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

            IWfActivity currentActivity = process.MoveToNextDefaultActivity();

            Console.WriteLine(currentActivity.Descriptor.ActivityType);

            Assert.AreEqual(WfActivityType.CompletedActivity, currentActivity.Descriptor.ActivityType);
        }

        [TestMethod]
        [Description("经过的流程节点的测试")]
        [TestCategory(ProcessTestHelper.Runtime)]
        public void ActivitiesTrackTest()
        {
            IWfProcess process = WfProcessTestCommon.StartupSimpleProcess();

            for (int i = 0; i < process.Activities.Count; i++)
            {
                if (process.Activities[i].Descriptor != process.Descriptor.InitialActivity)
                {
                    WfTransferParams transferPara = new WfTransferParams(process.Activities[i].Descriptor);
                    process.MoveTo(transferPara);
                }
            }

            int k = process.ElapsedActivities.Count;

            string[] keys = new string[] { "Initial", "NormalActivity", "Completed" };

            for (int j = 0; j < process.ElapsedActivities.Count; j++)
            {
                Assert.AreEqual(process.ElapsedActivities[j].Descriptor.Key, process.ElapsedActivities[j].Descriptor.Key, keys[j]);
            }

        }

        [TestMethod]
        [Description("运行时线上的条件判断")]
        [TestCategory(ProcessTestHelper.Runtime)]
        public void TransitionConditionTest()
        {
            TestContext.Properties["Amount"] = 7500;

            WfRuntime.ProcessContext.EvaluateTransitionCondition += new Expression.CalculateUserFunction(ProcessContext_EvaluateTransition);
            IWfProcess process = WfProcessTestCommon.StartupSimpleProcessDescriptorWithTransitionCondition();

            WfTransferParams transferParams = new WfTransferParams(process.Descriptor.InitialActivity.ToTransitions[0].ToActivity);

            process.MoveTo(transferParams);

            WfTransitionDescriptorCollection transitions =
                process.CurrentActivity.Descriptor.ToTransitions.GetAllCanTransitTransitions();

            Assert.AreEqual(1, transitions.Count);

            Console.WriteLine(((IWfForwardTransitionDescriptor)transitions[0]).Condition.Expression);

            Assert.AreEqual("Amount >= 5000", ((IWfForwardTransitionDescriptor)transitions[0]).Condition.Expression);
        }

        /// <summary>
        /// 已经迁移到MCS.Library.SOA.DataObjects.Tenant.Test
        /// </summary>
        [TestMethod]
        [Description("运行时活动上的条件判断")]
        [TestCategory(ProcessTestHelper.Runtime)]
        public void ActivityConditionTest()
        {
            TestContext.Properties["Amount"] = 2500;	//不满足条件
            WfRuntime.ProcessContext.EvaluateActivityCondition += new Expression.CalculateUserFunction(ProcessContext_EvaluateTransition);

            IWfProcess process = WfProcessTestCommon.StartupSimpleProcessDescriptorWithActivityCondition();

            Assert.AreEqual(WfActivityStatus.Running, process.InitialActivity.Status);

            IWfActivityDescriptor nextActivityDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
            WfTransferParams transferParams = ProcessTestHelper.GetInstanceOfWfTransferParams(nextActivityDesp, OguObject.requestor);
            process.MoveTo(transferParams);

            Assert.AreEqual(WfActivityStatus.Completed, process.InitialActivity.Status);

            Assert.AreEqual(WfActivityStatus.Pending, process.CurrentActivity.Status);

            IWfActivity activity = WfRuntime.ProcessContext.OriginalActivity;
            WfRuntime.PersistWorkflows();

            TestContext.Properties["Amount"] = 7500;

            WfPendingActivityInfoCollection pendingActivities = WfPendingActivityInfoAdapter.Instance.Load(builder => builder.AppendItem("ACTIVITY_ID", process.CurrentActivity.ID));

            pendingActivities.ForEach(pai => WfRuntime.ProcessPendingActivity(pai));
        }

        [TestMethod]
        [Description("检测活动点上多个出线的排序")]
        [TestCategory(ProcessTestHelper.Runtime)]
        public void ActivityToTransitionsSort()
        {
            IWfProcessDescriptor desp = WfProcessTestCommon.CreateProcessDescriptor();

            AddActivitiesDespToSameActivityDesp(desp.Activities[1], 4);

            ToTransitionsDescriptorCollection transDespColl = desp.Activities[1].ToTransitions;
            Assert.AreEqual(4, transDespColl.Count);

            SetTransitionCondition(transDespColl[0], false, 4);
            SetTransitionCondition(transDespColl[1], true, 0);
            SetTransitionCondition(transDespColl[2], false, 2);
            SetTransitionCondition(transDespColl[3], true, 1);

            WfTransitionDescriptorCollection transitions = desp.Activities[1].ToTransitions.GetAllCanTransitTransitions();

            Assert.IsTrue(transitions[0].DefaultSelect);
            Assert.AreEqual(transDespColl[1].DefaultSelect, transitions[0].DefaultSelect);

            Assert.IsTrue(transitions[1].DefaultSelect);
            Assert.AreEqual(transDespColl[3].DefaultSelect, transitions[1].DefaultSelect);

            Assert.IsFalse(transitions[2].DefaultSelect);
            Assert.AreEqual(transDespColl[2].Priority, transitions[2].Priority);

            Assert.IsFalse(transitions[3].DefaultSelect);
            Assert.AreEqual(transDespColl[0].Priority, transitions[3].Priority);
        }

        private void SetTransitionCondition(IWfTransitionDescriptor transitionDesp, bool isDefSelect, int priority)
        {
            WfTransitionDescriptor transDesp = (WfTransitionDescriptor)transitionDesp;
            transDesp.DefaultSelect = isDefSelect;
            transDesp.Priority = priority;
        }

        [TestMethod]
        [Description("检查通过任意已执行过的节点ID加载流程")]
        [TestCategory(ProcessTestHelper.Runtime)]
        public void LoadProcessByActivityIDTest()
        {
            string actId = string.Empty;
            IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();
            ((WfProcess)process).ResourceID = "resource2";

            actId = process.CurrentActivity.ID;
            ProcessTestHelper.MoveToAndSaveNextActivity(OguObject.approver2, process);

            process = WfRuntime.GetProcessByProcessID(process.ID);
            ProcessTestHelper.MoveToAndSaveNextActivity(OguObject.requestor, process);

            process = WfRuntime.GetProcessByProcessID(process.ID);
            ProcessTestHelper.MoveToAndSaveNextActivity(OguObject.approver2, process);

            process = WfRuntime.GetProcessByProcessID(process.ID);
            IWfProcess proc = WfRuntime.GetProcessByActivityID(actId);

            Assert.AreEqual(process.ID, proc.ID);
            Assert.AreEqual(process.CurrentActivity.ID, proc.CurrentActivity.ID);
        }

        [TestMethod]
        [TestCategory(ProcessTestHelper.Runtime)]
        [Description("基本流程描述时的流程线路")]
        public void BasicMainStreamActivities()
        {
            //主流程描述点
            IWfProcessDescriptor desp = WfProcessTestCommon.CreateProcessDescriptor();

            WfMainStreamActivityDescriptorCollection coll = desp.GetMainStreamActivities();

            Assert.AreEqual(desp.Activities.Count, coll.Count);

            //添加动态点
            string activityKey = desp.FindNotUsedActivityKey();
            WfActivityDescriptor actDesp = new WfActivityDescriptor(activityKey, WfActivityType.NormalActivity);
            desp.InitialActivity.Append(actDesp);

            coll = desp.GetMainStreamActivities();
            Assert.AreEqual(desp.Activities.Count, coll.Count, "流程描述时，添加节点描述也为主线流程中的活动");
        }

        [TestMethod]
        [TestCategory(ProcessTestHelper.Runtime)]
        [Description("FindElapsedTransition方法，在出错的时，需要正常抛出异常")]
        [ExpectedException(typeof(WfRuntimeException))]
        public void FindElapsedTransitionTest()
        {
            //流程运行时
            IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();

            //新增加活动描述，没有为此添加实例
            string activityKey = process.Descriptor.FindNotUsedActivityKey();
            WfActivityDescriptor actDesp = new WfActivityDescriptor(activityKey, WfActivityType.NormalActivity);

            process.CurrentActivity.Descriptor.Append(actDesp);

            //使用GetMainStreamActivities方法时，会出错,因为只添加了描述,没添加实例。这里需要正常抛出异常
            WfMainStreamActivityDescriptorCollection coll = process.Descriptor.GetMainStreamActivities();
            Assert.IsNotNull(coll);
        }

        [TestMethod]
        [TestCategory(ProcessTestHelper.Runtime)]
        [Description("查看基本流程描述时的流程线路，新增加动态点")]
        public void MainStreamActivitiesWithDynamicActivity()
        {
            IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();

            string currentActivityID = process.CurrentActivity.ID;
            //新增点
            WfActivityDescriptor actDesp = AddActivity(process);

            WfTransferParams tp = new WfTransferParams(actDesp);
            IUser user = (IUser)OguObjectSettings.GetConfig().Objects[OguObject.requestor.ToString()].Object;
            tp.Assignees.Add(user);

            process.MoveTo(tp);

            ProcessTestHelper.MoveToAndSaveNextActivity(OguObject.approver2, process);

            WfMainStreamActivityDescriptorCollection coll = process.Descriptor.GetMainStreamActivities();
            Assert.AreEqual(6, coll.Count, "动态创建的点不在主流程里头");
            Assert.AreEqual(7, process.Activities.Count, "流程中有7个点，其中一个是动态创建的点");
            Assert.AreEqual(process.Activities[0].Descriptor.ToTransitions[0].ToActivity.Key, actDesp.Key);
            Assert.AreEqual(actDesp.ToTransitions[0].ToActivity.Key, process.CurrentActivity.Descriptor.Key);

        }

        private static WfActivityDescriptor AddActivity(IWfProcess process)
        {
            string activityKey = process.Descriptor.FindNotUsedActivityKey();
            WfActivityDescriptor actDesp = new WfActivityDescriptor(activityKey, WfActivityType.NormalActivity);

            WfRuntime.ProcessContext.ActivityChangingContext.CreatorInstanceID = process.CurrentActivity.ID;
            WfRuntime.ProcessContext.ActivityChangingContext.AssociatedActivityKey =
                process.CurrentActivity.Descriptor.AssociatedActivityKey.IsNotEmpty() ?
                    process.CurrentActivity.Descriptor.AssociatedActivityKey : process.CurrentActivity.Descriptor.Key;

            process.CurrentActivity.Append(actDesp);
            return actDesp;
        }

        [TestMethod]
        [TestCategory(ProcessTestHelper.Runtime)]
        [Description("查看基本流程描述时的流程线路，新增加会签点")]
        public void MainStreamActivitiesWithConsignActivity()
        {
            IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();

            //会签
            string currentActivityID = process.CurrentActivity.ID;
            WfActivityDescriptor actDesp = AddActivity(process);

            WfTransferParams tp = new WfTransferParams(actDesp);
            IUser user = (IUser)OguObjectSettings.GetConfig().Objects[OguObject.requestor.ToString()].Object;
            tp.Assignees.Add(user);

            tp.BranchTransferParams.Add(new WfBranchProcessTransferParams(
                    ProcessTestHelper.CreateConsignTemplate(WfBranchProcessExecuteSequence.Parallel, WfBranchProcessBlockingType.WaitAllBranchProcessesComplete),
                    new IUser[] {(IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object,
						(IUser)OguObjectSettings.GetConfig().Objects["approver2"].Object}));

            process.MoveTo(tp);

            WfMainStreamActivityDescriptorCollection coll = process.Descriptor.GetMainStreamActivities();
            Assert.AreEqual(process.Activities.Count - 1, coll.Count);
        }

        [TestMethod]
        [Description("查看基本流程描述时的流程线路，存在退件时")]
        [TestCategory(ProcessTestHelper.Runtime)]
        public void MainStreamActivitiesWithReturn()
        {
            IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();

            ProcessTestHelper.MoveToAndSaveNextActivityWithDefSelectTranstion(OguObject.approver1, process);

            process = WfRuntime.GetProcessByProcessID(process.ID);
            ProcessTestHelper.MoveToAndSaveNextActivityWithDefSelectTranstion(OguObject.approver2, process);

            process = WfRuntime.GetProcessByProcessID(process.ID);
            ProcessTestHelper.MoveToAndSaveNextActivityWithDefSelectTranstion(OguObject.requestor, process);

            process = WfRuntime.GetProcessByProcessID(process.ID);
            int i = process.Activities.Count;
            Assert.AreEqual(6, i);
            Assert.AreEqual(6, process.Descriptor.Activities.Count);
            WfRuntime.ProcessContext.ActivityChangingContext.CreatorInstanceID = process.CurrentActivity.ID;
            process.CurrentActivity.CopyMainStreamActivities(process.Activities[1], null, WfControlOperationType.Return);

            int j = process.Activities.Count;
            Assert.AreEqual(9, j);
            Assert.AreEqual(9, process.Descriptor.Activities.Count);

            //此时还在当前节点，如果流转，也是流转到process.Activities[1]中
            ProcessTestHelper.MoveToAndSaveNextActivityWithDefSelectTranstion(OguObject.requestor, process);

            process = WfRuntime.GetProcessByProcessID(process.ID);
            Assert.IsTrue(process.CurrentActivity.CreatorInstanceID.IsNotEmpty());
            Assert.AreEqual(process.ElapsedActivities[process.ElapsedActivities.Count - 1].ID, process.CurrentActivity.CreatorInstanceID);

            WfMainStreamActivityDescriptorCollection coll = process.Descriptor.GetMainStreamActivities();
            Assert.AreEqual(6, coll.Count);
        }

        [TestMethod]
        [Description("查看基本流程描述的流程线路，存在条件判断时")]
        [TestCategory(ProcessTestHelper.Runtime)]
        public void MainStreamActivitiesWithConditionActivityPassed()
        {
            IWfProcessDescriptor desp = WfProcessTestCommon.CreateProcessDescriptor();
            desp.InitialActivity.Resources.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object));

            IWfActivityDescriptor actDesp = desp.Activities[1];
            AddActivitiesDespToSameActivityDesp(actDesp, 2);
            ToTransitionsDescriptorCollection transDespColl = actDesp.ToTransitions;
            SetTransitionCondition(transDespColl[0], false, 1);
            SetTransitionCondition(transDespColl[1], true, 3);

            WfProcessStartupParams startupParams = WfProcessTestCommon.GetInstanceOfWfProcessStartupParams(desp);

            IWfProcess process = WfRuntime.StartWorkflow(startupParams);

            ProcessTestHelper.MoveToAndSaveNextActivityWithDefSelectTranstion(OguObject.approver1, process);

            process = WfRuntime.GetProcessByProcessID(process.ID);
            ProcessTestHelper.MoveToAndSaveNextActivityWithDefSelectTranstion(OguObject.approver2, process);

            process = WfRuntime.GetProcessByProcessID(process.ID);
            ProcessTestHelper.MoveToAndSaveNextActivityWithDefSelectTranstion(OguObject.requestor, process);

            process = WfRuntime.GetProcessByProcessID(process.ID);
            Assert.AreEqual(3, process.ElapsedActivities.Count);

            WfMainStreamActivityDescriptorCollection coll = process.Descriptor.GetMainStreamActivities();
            Assert.AreEqual(process.Activities.Count - 1, coll.Count);

            Assert.AreEqual(transDespColl[1].ToActivity.Key, coll[2].Activity.Key, "此处为动态添加的活动");
        }

        [TestMethod]
        [Description("查看基本流程描述的流程线路，存在金额条件判断时")]
        [TestCategory(ProcessTestHelper.Runtime)]
        public void MainStreamActivitiesWithAmountCondition()
        {
            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithCondition();

            IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

            process.ApplicationRuntimeParameters["Amount"] = 2500;

            WfMainStreamActivityDescriptorCollection mainActs = process.GetMainStreamActivities(true);

            mainActs.Output("不经过领导的主线活动");

            Assert.IsTrue(mainActs.ContainsKey("NormalActivity"));
            Assert.IsFalse(mainActs.ContainsKey("ManagerActivity"));

            //改变条件
            process.ApplicationRuntimeParameters["Amount"] = 10000;

            mainActs = process.GetMainStreamActivities(true);

            mainActs.Output("经过领导的主线活动");

            Assert.IsTrue(mainActs.ContainsKey("NormalActivity"));
            Assert.IsTrue(mainActs.ContainsKey("ManagerActivity"));
        }

        [TestMethod]
        [Description("退件操作。带分支的活动进行退件时,且流程当前活动在分支上")]
        [TestCategory(ProcessTestHelper.ProcessBehavior_Return)]
        public void ReturnTestWithBranchActivity()
        {
            IWfProcessDescriptor desp = WfProcessTestCommon.CreateProcessDescriptor();
            desp.InitialActivity.Resources.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object));

            GetProcDespWithBranchActivities(desp.Activities[1], 2);

            WfProcessStartupParams startupParams = WfProcessTestCommon.GetInstanceOfWfProcessStartupParams(desp);
            IWfProcess process = WfRuntime.StartWorkflow(startupParams);
            ProcessTestHelper.MoveToAndSaveNextActivityWithDefSelectTranstion(OguObject.approver1, process);

            process = WfRuntime.GetProcessByProcessID(process.ID);
            ProcessTestHelper.MoveToAndSaveNextActivityWithDefSelectTranstion(OguObject.approver2, process);

            process = WfRuntime.GetProcessByProcessID(process.ID);
            Assert.AreEqual(2, process.ElapsedActivities.Count);
            WfRuntime.ProcessContext.ActivityChangingContext.CreatorInstanceID = process.CurrentActivity.ID;
            process.CurrentActivity.CopyMainStreamActivities(process.Activities[1], null, WfControlOperationType.Return); //退件
            ProcessTestHelper.MoveToAndSaveNextActivityWithDefSelectTranstion(OguObject.requestor, process);

            process = WfRuntime.GetProcessByProcessID(process.ID);

            Assert.IsTrue(process.CurrentActivity.CreatorInstanceID.IsNotEmpty());
            Assert.AreEqual(process.ElapsedActivities[2].ID, process.CurrentActivity.CreatorInstanceID);

            Assert.AreEqual(10, process.Activities.Count);
        }

        [TestMethod]
        [Description("退件操作。带分支的活动进行退件时,且流程当前活动在主干线上")]
        [TestCategory(ProcessTestHelper.ProcessBehavior_Return)]
        public void ReturnTestWithMainProcess()
        {
            IWfProcessDescriptor desp = WfProcessTestCommon.CreateProcessDescriptor();
            desp.InitialActivity.Resources.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object));

            IWfActivityDescriptor act = desp.Activities[1];
            GetProcDespWithBranchActivities(act, 2);

            Assert.AreEqual(8, desp.Activities.Count);

            WfProcessStartupParams startupParams = WfProcessTestCommon.GetInstanceOfWfProcessStartupParams(desp);
            IWfProcess process = WfRuntime.StartWorkflow(startupParams);
            ProcessTestHelper.MoveToAndSaveNextActivityWithDefSelectTranstion(OguObject.approver1, process);

            process = WfRuntime.GetProcessByProcessID(process.ID);
            ProcessTestHelper.MoveToAndSaveNextActivityWithDefSelectTranstion(OguObject.approver2, process);

            process = WfRuntime.GetProcessByProcessID(process.ID);
            ProcessTestHelper.MoveToAndSaveNextActivityWithDefSelectTranstion(OguObject.requestor, process);

            process = WfRuntime.GetProcessByProcessID(process.ID);
            ProcessTestHelper.MoveToAndSaveNextActivityWithDefSelectTranstion(OguObject.approver2, process);

            process = WfRuntime.GetProcessByProcessID(process.ID);
            Assert.AreEqual(4, process.ElapsedActivities.Count);
            WfRuntime.ProcessContext.ActivityChangingContext.CreatorInstanceID = process.CurrentActivity.ID;
            process.CurrentActivity.CopyMainStreamActivities(process.Activities.FindActivityByDescriptorKey(act.Key), null, WfControlOperationType.Return);//退件 +3+1
            ProcessTestHelper.MoveToAndSaveNextActivityWithDefSelectTranstion(OguObject.approver1, process);

            process = WfRuntime.GetProcessByProcessID(process.ID);
            Assert.AreEqual(12, process.Activities.Count);
            Assert.AreEqual(process.Activities.FindActivityByDescriptorKey(act.Key).Descriptor.Key, process.CurrentActivity.Descriptor.AssociatedActivityKey);
            Assert.AreEqual(process.ElapsedActivities[4].ID, process.CurrentActivity.CreatorInstanceID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activity">需要添加活动的活动点</param>
        /// <param name="n">添加的个数</param>
        /// <returns></returns>
        private void GetProcDespWithBranchActivities(IWfActivityDescriptor activityDesp, int n)
        {
            AddActivitiesDespToSameActivityDesp(activityDesp, n);

            ToTransitionsDescriptorCollection transDespColl = activityDesp.ToTransitions;
            bool def = false;
            int i = transDespColl.Count - 1;
            foreach (IWfTransitionDescriptor item in transDespColl)
            {
                SetTransitionCondition(item, def, i--);
                SetTransitionCondition(item, !def, i--);//设置线的条件
            }
        }

        [TestMethod]
        [Description("分支流程的启动测试")]
        [TestCategory(ProcessTestHelper.ProcessBehavior_Start)]
        public void BranchProcessStartTest()
        {
            IWfProcess process = WfProcessTestCommon.StartupSimpleProcessWithAssignee();

            WfTransferParams tp = ProcessTestHelper.GetInstanceOfWfTransferParams(process, WfBranchProcessExecuteSequence.Parallel, WfBranchProcessBlockingType.WaitAllBranchProcessesComplete);
            process.MoveTo(tp);

            Assert.AreEqual(1, process.CurrentActivity.BranchProcessGroups.Count);
            IWfBranchProcessGroup group = process.CurrentActivity.BranchProcessGroups[0];

            Assert.AreEqual(tp.BranchTransferParams[0].Template.BranchProcessKey, group.ProcessTemplate.BranchProcessKey);
            Assert.AreEqual(2, group.Branches.Count);
            Assert.AreEqual(tp.BranchTransferParams[0].Template.BranchProcessKey, group.Branches[0].Descriptor.Key);
            Assert.AreEqual(tp.BranchTransferParams[0].Template.BranchProcessKey, group.Branches[1].Descriptor.Key);
            Assert.AreEqual(WfProcessStatus.Running, group.Branches[0].Status);
            Assert.AreEqual(WfProcessStatus.Running, group.Branches[1].Status);

            Assert.AreEqual(OguObjectSettings.GetConfig().Objects["approver1"].Object.ID, group.Branches[0].InitialActivity.Assignees[0].User.ID);
            Assert.AreEqual(OguObjectSettings.GetConfig().Objects["approver2"].Object.ID, group.Branches[1].InitialActivity.Assignees[0].User.ID);
        }

        [TestMethod]
        [Description("带分支流程的MoveToExecutor的测试")]
        [TestCategory(ProcessTestHelper.BranchProcess)]
        public void MoveToWithBranchProcessesExecutorTest()
        {
            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithBranchTemplate();

            IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

            IWfActivityDescriptor normalActivity = processDesp.InitialActivity.ToTransitions[0].ToActivity;

            WfTransferParams transferParams0 = new WfTransferParams(normalActivity);

            //初始化后，流转到有分支流程的活动
            process.MoveTo(transferParams0);

            Assert.AreEqual(normalActivity.BranchProcessTemplates.Count, process.CurrentActivity.BranchProcessGroups.Count);
        }

        [TestMethod]
        [Description("测试运行时活动点上，定义了前面的活动操作人的资源")]
        [TestCategory(ProcessTestHelper.Resource)]
        public void ActivityOperatorResourceTest()
        {
            IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();

            IWfActivityDescriptor nextActivityDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
            WfTransferParams tp = ProcessTestHelper.GetInstanceOfWfTransferParams(nextActivityDesp, OguObject.approver1);
            IUser requestor = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object;
            tp.Operator = requestor;

            process.MoveTo(tp);

            WfActivityOperatorResourceDescriptor resource = new WfActivityOperatorResourceDescriptor();
            resource.ActivityKey = process.InitialActivity.Descriptor.Key;
            process.CurrentActivity.Descriptor.Resources.Add(resource);
            OguDataCollection<IUser> users = process.CurrentActivity.Descriptor.Resources.ToUsers();

            Assert.IsTrue(users.Count > 0);
            Assert.AreEqual(requestor.ID, users[0].ID, "验证资源中的人员是否是之前活动的操作人");

            WfRuntime.PersistWorkflows();
            process = WfRuntime.GetProcessByProcessID(process.ID);

            users = process.CurrentActivity.Descriptor.Resources.ToUsers();

            Assert.IsTrue(users.Count > 0, "重新加载后资源中应该有人");
            Assert.AreEqual(requestor.ID, users[0].ID, "重新加载后验证资源中的人员是否是之前活动的操作人");
        }

        [TestMethod]
        [Description("测试运行时为活动点的指定被指派人")]
        [TestCategory(ProcessTestHelper.Resource)]
        public void WfActivityAssigneesResourceTest()
        {
            IWfProcess process = GetProcessInstanceWithAssigneesResource();

            Assert.AreEqual(process.CurrentActivity.Assignees[0].User.ID, process.CurrentActivity.Descriptor.Resources.ToUsers()[0].ID);

            IWfActivityDescriptor actDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
            WfTransferParams par = new WfTransferParams(actDesp);
            par.Assignees.Add(actDesp.FromTransitions[0].FromActivity.Resources.ToUsers()[0]);
            process.MoveTo(par);

            WfRuntime.PersistWorkflows();
            process = WfRuntime.GetProcessByProcessID(process.ID);
            Assert.IsTrue(ProcessTestHelper.ExistsActivityUserTasks(process.CurrentActivity.ID, TaskStatus.Ban));
        }

        [TestMethod]
        [Description("测试多分支流程启动执行的时间")]
        [TestCategory(ProcessTestHelper.ExecuteTime)]
        public void BranchProcessRunTimeTest()
        {
            IWfProcess process = WfProcessTestCommon.StartupSimpleProcessWithAssignee();

            IWfActivityDescriptor nextActivityDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
            WfTransferParams tp = ProcessTestHelper.GetInstanceOfWfTransferParams(nextActivityDesp, OguObject.requestor);

            tp.BranchTransferParams.Add(new WfBranchProcessTransferParams(
                    ProcessTestHelper.CreateConsignTemplate(WfBranchProcessExecuteSequence.Parallel, WfBranchProcessBlockingType.WaitAllBranchProcessesComplete)));

            for (int i = 0; i < 20; i++)
            {
                IUser obj = (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object;
                IUser obj2 = (IUser)OguObjectSettings.GetConfig().Objects["approver2"].Object;

                tp.BranchTransferParams[0].BranchParams.Add(obj, obj2);
            }
            int branchProcessNum = tp.BranchTransferParams[0].BranchParams.Count;

            ProcessTestHelper.OutputExecutionTime(() => process.MoveTo(tp), "流转并启动子流程");
            ProcessTestHelper.OutputExecutionTime(() => WfRuntime.PersistWorkflows(), "保存流程状态");
            ProcessTestHelper.OutputExecutionTime(() => WfRuntime.GetProcessByProcessID(process.ID), "重新加载主流程");
        }

        [TestMethod]
        [Description("启动带有权限矩阵的流程实例")]
        [TestCategory(ProcessTestHelper.Runtime)]
        public void ProcessWithMatrix()
        {
            try
            {
                var wfDescriptor = WfProcessDescriptorManager.GetDescriptor("workflowmatrixtest");
                WfProcessStartupParams startParam = new WfProcessStartupParams();
                startParam.ProcessDescriptor = wfDescriptor;

                var processInstance = WfRuntime.StartWorkflow(startParam);
                processInstance.ApplicationRuntimeParameters.Add("支付方式", "网银");
                processInstance.ApplicationRuntimeParameters.Add("成本中心", "成1");
                processInstance.ApplicationRuntimeParameters.Add("费用类型", "差旅费");
                processInstance.ApplicationRuntimeParameters.Add("金额", "100");
                processInstance.ApplicationRuntimeParameters.Add("部门", "商务部");

                WfTransferParams transferParams = new WfTransferParams(processInstance.Descriptor.Activities["N2"]);

                var currentAct = processInstance.MoveTo(transferParams);
                currentAct.Candidates.ForEach(p =>
                {
                    Console.WriteLine(p.User.DisplayName);
                });
            }
            catch
            {
            }
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

        private static IWfProcess GetProcessInstanceWithAssigneesResource()
        {
            IWfProcess process = WfProcessTestCommon.StartupProcessWithAssignee();

            IWfActivityDescriptor nextActivityDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
            WfTransferParams tp = ProcessTestHelper.GetInstanceOfWfTransferParams(nextActivityDesp, OguObject.approver2);
            process.MoveTo(tp);

            WfRuntime.PersistWorkflows();
            process = WfRuntime.GetProcessByProcessID(process.ID);

            IWfActivityDescriptor actDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
            WfActivityAssigneesResourceDescriptor actAssResDesp = new WfActivityAssigneesResourceDescriptor();
            actAssResDesp.ActivityKey = actDesp.Key;
            actDesp.Resources.Add(actAssResDesp);

            nextActivityDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
            WfTransferParams tpa = ProcessTestHelper.GetInstanceOfWfTransferParams(nextActivityDesp, OguObject.approver1);
            process.MoveTo(tpa);

            WfRuntime.PersistWorkflows();
            process = WfRuntime.GetProcessByProcessID(process.ID);
            return process;
        }

        /// <summary>
        /// 在某个活动下 的同级添加一组活动
        /// </summary>
        /// <param name="desp"></param>
        /// <param name="n">添加的个数</param>
        private static void AddActivitiesDespToSameActivityDesp(IWfActivityDescriptor actDesp, int n)
        {
            IWfActivityDescriptor toActDesp = actDesp.ToTransitions[0].ToActivity;
            actDesp.ToTransitions.Clear();

            for (int i = 7; i < 7 + n; i++)
            {
                WfActivityDescriptor normalAct = new WfActivityDescriptor("NormalActivity" + i, WfActivityType.NormalActivity);
                normalAct.Name = "Normal" + i;
                normalAct.CodeName = "Normal Activity" + i;

                actDesp.Process.Activities.Add(normalAct);

                actDesp.ToTransitions.AddForwardTransition(normalAct);
                normalAct.ToTransitions.AddForwardTransition(toActDesp);
            }
        }
    }
}
