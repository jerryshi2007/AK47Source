using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.OGUPermission;
using MCS.Library.Passport;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects.Tenant.Test.Workflow.Helper;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.Workflow
{
    /// <summary>
    /// 动态（模板）活动的单元测试
    /// </summary>
    [TestClass]
    public class DynamicActivityTest
    {
        [TestMethod]
        [Description("创建一个只有三个活动（开始-动态模版-结束）的流程，进行测试。动态活动上有一个人员")]
        public void BasicDynamicActivityTest()
        {
            IWfProcessDescriptor processDesp = ProcessHelper.CreateSimpleProcessDescriptor();

            processDesp.Activities["NormalActivity"].RelativeLinks.Add(new WfRelativeLinkDescriptor("DynLink") { Url = "http://localhost" });

            IWfActivityDescriptor templateActDesp = processDesp.Activities["NormalActivity"];

            FillDynamicActivitySimpleResource(templateActDesp, (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object);

            IWfProcess process = processDesp.StartupProcess();

            IWfActivityDescriptor firstDynamicActDesp = ValidateDynamicActivities(process.InitialActivity.Descriptor, templateActDesp);

            Assert.AreEqual(templateActDesp.RelativeLinks.Count, firstDynamicActDesp.RelativeLinks.Count);

            ValidateInDynamicTransitionsProperties(firstDynamicActDesp.FromTransitions, templateActDesp);

            ValidateLastDynamicTransitions(templateActDesp);

            ValidateTemplateCandidatesAndDynamicActivityCandidates(firstDynamicActDesp, templateActDesp);
        }

        [TestMethod]
        [Description("动态模版退件测试。创建一个只有三个活动（开始-动态模版-结束）的流程，进行测试。动态活动上有一个人员")]
        public void BasicDynamicActivityReturnTest()
        {
            IWfProcessDescriptor processDesp = ProcessHelper.CreateSimpleProcessDescriptor();

            IWfActivityDescriptor templateActDesp = processDesp.Activities["NormalActivity"];

            FillDynamicActivitySimpleResource(templateActDesp, (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object);

            IWfProcess process = processDesp.StartupProcess();

            process.OutputMainStream();

            process.OutputEveryActivities();

            process.MoveToNextDefaultActivity();
            ReturnExecutorTestHelper.ExecuteReturnOperation(process.CurrentActivity, processDesp.InitialActivity.Key);

            //N2(Initial)->N1->N4->N5->Completed
            process.OutputMainStream();

            //N1->N2(Initial)->N4->N5->Completed
            process.OutputEveryActivities();

            Assert.AreEqual("Initial", process.CurrentActivity.Descriptor.GetAssociatedActivity().Key);
            Assert.AreEqual(2, process.CurrentActivity.Descriptor.ToTransitions.Count);

            process.MoveToNextDefaultActivity();
            //Assert.AreEqual("N4", process.CurrentActivity.Descriptor.Key); //To N4

            //N2(Initial)->N1->N4->N5->Completed
            process.OutputMainStream();

            //N1->N2(Initial)->N4->N5->Completed
            process.OutputAndAssertEveryActivities();

            //IWfActivityDescriptor firstDynamicActDesp = ValidateDynamicActivities(process.InitialActivity.Descriptor, templateActDesp);

            //ValidateInDynamicTransitionsProperties(firstDynamicActDesp.FromTransitions, templateActDesp);

            //ValidateLastDynamicTransitions(templateActDesp);

            //ValidateTemplateCandidatesAndDynamicActivityCandidates(firstDynamicActDesp, templateActDesp);
        }

        [TestMethod]
        [Description("创建一个只有三个活动（开始-动态模版-结束）的流程，进行测试。动态活动上有两个人员，会生成两个动态活动")]
        public void BasicDynamicActivityWithTwoAssigneesTest()
        {
            IWfProcessDescriptor processDesp = ProcessHelper.CreateSimpleProcessDescriptor();

            IWfActivityDescriptor templateActDesp = processDesp.Activities["NormalActivity"];

            FillDynamicActivitySimpleResource(templateActDesp,
                (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object,
                (IUser)OguObjectSettings.GetConfig().Objects["cfo"].Object);

            IWfProcess process = processDesp.StartupProcess();

            IWfActivityDescriptor firstDynamicActDesp = ValidateDynamicActivities(process.InitialActivity.Descriptor, templateActDesp);

            ValidateInDynamicTransitionsProperties(firstDynamicActDesp.FromTransitions, templateActDesp);

            WfActivityDescriptorCollection dynamicToActivities = firstDynamicActDesp.GetToActivities();

            ValidateLastDynamicTransitions(templateActDesp);

            ValidateTemplateCandidatesAndDynamicActivityCandidates(firstDynamicActDesp, templateActDesp);
        }

        [TestMethod]
        [Description("创建一个只有三个活动（开始-动态-结束）的流程，进行测试。动态活动上没有资源")]
        public void BasicDynamicActivityWithoutResourceTest()
        {
            IWfProcessDescriptor processDesp = ProcessHelper.CreateSimpleProcessDescriptor();

            IWfActivityDescriptor templateActDesp = processDesp.Activities["NormalActivity"];
            FillDynamicActivitySimpleResource(templateActDesp);

            IWfProcess process = processDesp.StartupProcess();

            IWfActivityDescriptor firstDynamicActDesp = ValidateDynamicActivities(process.InitialActivity.Descriptor, templateActDesp);

            ValidateNoResourceTransitionsProperties(processDesp.InitialActivity.ToTransitions.FindAll(t => t.Properties.GetValue("DynamicSource", string.Empty) == templateActDesp.Key), templateActDesp);

            Assert.AreEqual(processDesp.CompletedActivity.Key, firstDynamicActDesp.Key, "起始点和终止点连接在一起");
        }

        [TestMethod]
        [Description("创建一个有七个活动（开始-A1~A2-动态-B1~B2-结束）的流程，进行测试。动态活动上没有资源")]
        public void ComplexDynamicActivityWithoutResourceTest()
        {
            IWfProcessDescriptor processDesp = CreateComplexDynamicActivityProcess();
            IWfProcess process = processDesp.StartupProcess();

            IWfActivityDescriptor templateActDesp = process.Descriptor.Activities["NormalActivity"];
            IWfActivityDescriptor a1Activity = process.Descriptor.Activities["A1"];
            IWfActivityDescriptor a2Activity = process.Descriptor.Activities["A2"];

            ValidateDynamicActivities(a1Activity, templateActDesp);
            ValidateDynamicActivities(a2Activity, templateActDesp);

            ValidateNoResourceTransitionsProperties(a1Activity.ToTransitions.FindAll(t => t.Properties.GetValue("DynamicSource", string.Empty) == templateActDesp.Key), templateActDesp);
            ValidateNoResourceTransitionsProperties(a2Activity.ToTransitions.FindAll(t => t.Properties.GetValue("DynamicSource", string.Empty) == templateActDesp.Key), templateActDesp);

            IWfActivityDescriptor b1Activity = process.Descriptor.Activities["B1"];
            IWfActivityDescriptor b2Activity = process.Descriptor.Activities["B2"];

            Assert.IsTrue(a1Activity.ToTransitions.Exists(t => t.ToActivity.Key == b1Activity.Key));
            Assert.IsTrue(a1Activity.ToTransitions.Exists(t => t.ToActivity.Key == b2Activity.Key));

            Assert.IsTrue(a2Activity.ToTransitions.Exists(t => t.ToActivity.Key == b1Activity.Key));
            Assert.IsTrue(a2Activity.ToTransitions.Exists(t => t.ToActivity.Key == b2Activity.Key));
        }

        [TestMethod]
        [Description("创建一个有七个活动（开始-A1~A2-动态-B1~B2-结束）的流程，进行测试。动态活动上有两个指派人")]
        public void ComplexDynamicActivityWithTwoAssigneesTest()
        {
            IWfProcessDescriptor processDesp = CreateComplexDynamicActivityProcess(
                (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object,
                (IUser)OguObjectSettings.GetConfig().Objects["cfo"].Object);

            IWfProcess process = processDesp.StartupProcess();

            IWfActivityDescriptor templateActDesp = process.Descriptor.Activities["NormalActivity"];
            IWfActivityDescriptor a1Activity = process.Descriptor.Activities["A1"];
            IWfActivityDescriptor a2Activity = process.Descriptor.Activities["A2"];

            IWfActivityDescriptor firstDynamicActDesp1 = ValidateDynamicActivities(a1Activity, templateActDesp);
            IWfActivityDescriptor firstDynamicActDesp2 = ValidateDynamicActivities(a2Activity, templateActDesp);

            Assert.AreEqual(firstDynamicActDesp1.Key, firstDynamicActDesp2.Key);

            IWfActivityDescriptor secondDynamicActDesp = firstDynamicActDesp1.GetToActivities().FirstOrDefault();

            //第二个动态点的出线对应活动
            WfActivityDescriptorCollection secondDynamicActDespOutDesps = secondDynamicActDesp.GetToActivities();

            Assert.AreEqual(2, secondDynamicActDespOutDesps.Count);
            Assert.IsTrue(secondDynamicActDespOutDesps.ContainsKey("B1"));
            Assert.IsTrue(secondDynamicActDespOutDesps.ContainsKey("B2"));

            ValidateLastDynamicTransitions(templateActDesp);
            ValidateTemplateCandidatesAndDynamicActivityCandidates(firstDynamicActDesp1, templateActDesp);
        }

        /// <summary>
        /// 原始测试不能通过
        /// </summary>
        [TestMethod]
        [Description("创建一个三个活动（开始-矩阵活动-结束）的流程，进行测试。矩阵活动展开成三个活动")]
        public void DynamicActivityWithActivityMatrixTest()
        {
            IRole role = PrepareSOARole();

            IWfProcessDescriptor processDesp = ProcessHelper.CreateSimpleProcessDescriptor();

            IWfActivityDescriptor templateActDesp = processDesp.Activities["NormalActivity"];

            FillDynamicActivitySimpleResource(templateActDesp, new WfResourceDescriptorCollection() { new WfRoleResourceDescriptor(role) });

            IWfProcess process = processDesp.StartupProcess(new Dictionary<string, object>() { { "CostCenter", "1001" } });

            Assert.AreEqual(6, process.Activities.Count);

            IWfActivityDescriptor firstDynamicActDesp = ValidateDynamicActivities(process.Descriptor.InitialActivity, templateActDesp);

            Assert.AreEqual(2, firstDynamicActDesp.Instance.Candidates.Count);

            foreach (WfAssignee assignee in firstDynamicActDesp.Instance.Candidates)
                Assert.IsTrue(assignee.User.LogOnName == "fanhy" || assignee.User.LogOnName == "yangrui1");

            IWfActivityDescriptor limingActDesp = firstDynamicActDesp.GetToActivities().FirstOrDefault();

            Assert.AreEqual(1, limingActDesp.Instance.Candidates.Count);
            Assert.IsTrue(limingActDesp.Instance.Candidates[0].User.LogOnName == "liming");

            IWfActivityDescriptor quymActDesp = limingActDesp.GetToActivities().FirstOrDefault();

            Assert.AreEqual(1, quymActDesp.Instance.Candidates.Count);
            Assert.IsTrue(quymActDesp.Instance.Candidates[0].User.LogOnName == "quym");
        }

        [TestMethod]
        [Description("创建一个三个活动（开始-矩阵活动-结束）的流程，进行测试。没有流程上下文，矩阵活动0个活动")]
        public void DynamicActivityActivityMatrixWithoutProcessContextTest()
        {
            IRole role = PrepareSOARole();

            IWfProcessDescriptor processDesp = ProcessHelper.CreateSimpleProcessDescriptor();

            IWfActivityDescriptor templateActDesp = processDesp.Activities["NormalActivity"];

            FillDynamicActivitySimpleResource(templateActDesp, new WfResourceDescriptorCollection() { new WfRoleResourceDescriptor(role) });

            IWfProcess process = processDesp.StartupProcess();

            Assert.AreEqual(3, process.Activities.Count);

            IWfActivityDescriptor firstDynamicActDesp = ValidateDynamicActivities(process.InitialActivity.Descriptor, templateActDesp);

            ValidateNoResourceTransitionsProperties(processDesp.InitialActivity.ToTransitions.FindAll(t => t.Properties.GetValue("DynamicSource", string.Empty) == templateActDesp.Key), templateActDesp);

            Assert.AreEqual(processDesp.CompletedActivity.Key, firstDynamicActDesp.Key, "起始点和终止点连接在一起");
        }

        [TestMethod]
        [Description("创建一个三个活动（开始-矩阵活动-结束）的流程，进行测试。矩阵活动展开成三个活动，然后在动态活动内部退回")]
        public void DynamicActivityWithActivityMatrixReturnToInnerTest()
        {
            IRole role = PrepareSOARole();

            IWfProcessDescriptor processDesp = ProcessHelper.CreateSimpleProcessDescriptor();

            IWfActivityDescriptor templateActDesp = processDesp.Activities["NormalActivity"];

            FillDynamicActivitySimpleResource(templateActDesp, new WfResourceDescriptorCollection() { new WfRoleResourceDescriptor(role) });

            IWfProcess process = processDesp.StartupProcess(new Dictionary<string, object>() { { "CostCenter", "1001" } });

            //到第一个动态活动
            process = process.MoveToDefaultActivityByExecutor();

            string targetActKey = process.CurrentActivity.Descriptor.Key;

            //到第二个动态活动
            process = process.MoveToDefaultActivityByExecutor();

            //退回到第一个动态活动
            ReturnExecutorTestHelper.ExecuteReturnOperation(process.CurrentActivity, targetActKey);

            process = WfRuntime.GetProcessByProcessID(process.ID);

            Console.WriteLine("Current Activity Key: {0}", process.CurrentActivity.Descriptor.Key);

            process.OutputMainStream();
            process.OutputAndAssertEveryActivities();

            Console.WriteLine("Next Default Activity Key: {0}",
                process.CurrentActivity.Descriptor.ToTransitions.GetAllCanTransitForwardTransitions().First().ToActivity.Key);

            Assert.AreEqual(targetActKey, process.CurrentActivity.Descriptor.AssociatedActivityKey);
        }

        [TestMethod]
        [Description("创建一个三个活动（开始-矩阵活动-结束）的流程，进行测试。矩阵活动展开成三个活动，然后退回到起始点")]
        public void DynamicActivityWithActivityMatrixReturnToInitialTest()
        {
            IRole role = PrepareSOARole();

            IWfProcessDescriptor processDesp = ProcessHelper.CreateSimpleProcessDescriptor();

            IWfActivityDescriptor templateActDesp = processDesp.Activities["NormalActivity"];

            FillDynamicActivitySimpleResource(templateActDesp, new WfResourceDescriptorCollection() { new WfRoleResourceDescriptor(role) });

            IWfProcess process = processDesp.StartupProcess(new Dictionary<string, object>() { { "CostCenter", "1001" } });

            //到第一个动态活动
            process = process.MoveToDefaultActivityByExecutor();

            //到第二个动态活动
            process = process.MoveToDefaultActivityByExecutor();

            //退回到起始点
            ReturnExecutorTestHelper.ExecuteReturnOperation(process.CurrentActivity, process.InitialActivity.Descriptor.Key);

            Console.WriteLine(process.CurrentActivity.Descriptor.Key);
            Console.WriteLine(process.CompletedActivity.Descriptor.FromTransitions.GetAllCanTransitForwardTransitions().Count);

            process.OutputMainStream();
            process.OutputAndAssertEveryActivities();

            Assert.AreEqual(1, process.CompletedActivity.Descriptor.FromTransitions.GetAllCanTransitForwardTransitions().Count,
                "结束点应该只有一条有效的入线s");

            Assert.AreEqual(process.InitialActivity.Descriptor.Key, process.CurrentActivity.Descriptor.AssociatedActivityKey);
        }

        [TestMethod]
        [Description("创建一个三个活动（开始-矩阵活动-结束）的流程，进行测试。矩阵活动展开成三个活动，然后退回到起始点两次测试")]
        public void DynamicActivityWithActivityMatrixReturnToInitialTwiceTest()
        {
            IRole role = PrepareSOARole();

            IWfProcessDescriptor processDesp = ProcessHelper.CreateSimpleProcessDescriptor();

            IWfActivityDescriptor templateActDesp = processDesp.Activities["NormalActivity"];

            FillDynamicActivitySimpleResource(templateActDesp, new WfResourceDescriptorCollection() { new WfRoleResourceDescriptor(role) });

            IWfProcess process = processDesp.StartupProcess(new Dictionary<string, object>() { { "CostCenter", "1001" } });

            //到第一个动态活动
            process = process.MoveToDefaultActivityByExecutor();

            //到第二个动态活动
            process = process.MoveToDefaultActivityByExecutor();

            //第一次退回到起始点
            process = OutputAndAssertProcessInfo(process, "第一次退回到起始点",
                (p) => ReturnExecutorTestHelper.ExecuteReturnOperation(p.CurrentActivity, p.InitialActivity.Descriptor.Key));

            //再到第一个动态活动
            process = process.MoveToDefaultActivityByExecutor();

            //再到第二个动态活动
            process = process.MoveToDefaultActivityByExecutor();

            //第二次退回到起始点
            process = OutputAndAssertProcessInfo(process, "第二次退回到起始点",
                (p) => ReturnExecutorTestHelper.ExecuteReturnOperation(p.CurrentActivity, p.InitialActivity.Descriptor.Key));

            Assert.AreEqual(1, process.CompletedActivity.Descriptor.FromTransitions.GetAllCanTransitForwardTransitions().Count,
                "结束点应该只有一条有效的入线s");

            Assert.AreEqual(process.InitialActivity.Descriptor.Key, process.CurrentActivity.Descriptor.AssociatedActivityKey);
        }

        [TestMethod]
        [Description("创建一个三个活动（开始-矩阵活动-结束）的流程，进行测试。矩阵活动展开成三个活动，流转到最后一个动态活动，然后退回到起始点")]
        public void DynamicActivityWithActivityMatrixLastDynamicActivityReturnToInitialTest()
        {
            //在最后一个动态活动上添加一个默认的退回线
            IRole role = PrepareSOARole((row, pds) =>
            {
                if (row.Values.GetValue(SOARolePropertyDefinition.ActivitySNColumn, string.Empty) == "30")
                {
                    row.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.TransitionsColumn])
                    {
                        Value =
                          "[{\"Name\":\"同意\",\"DefaultSelect\":true}, {\"Name\":\"不同意\",\"ToActivityKey\":\"FirstActivity\",\"DefaultSelect\":false,\"IsReturn\":true}]"
                    });
                }
            });

            IWfProcessDescriptor processDesp = ProcessHelper.CreateSimpleProcessDescriptor();

            IWfActivityDescriptor templateActDesp = processDesp.Activities["NormalActivity"];

            FillDynamicActivitySimpleResource(templateActDesp, new WfResourceDescriptorCollection() { new WfRoleResourceDescriptor(role) });

            IWfProcess process = processDesp.StartupProcess(new Dictionary<string, object>() { { "CostCenter", "1001" } });

            //到第一个动态活动
            process = process.MoveToDefaultActivityByExecutor();

            //到第二个动态活动
            process = process.MoveToDefaultActivityByExecutor();

            //到第三个动态活动
            process = process.MoveToDefaultActivityByExecutor();

            //退回到起始点
            process = process.MoveToReturnActivityByExecutor();

            Console.WriteLine(process.CurrentActivity.Descriptor.Key);
            Console.WriteLine(process.CompletedActivity.Descriptor.FromTransitions.GetAllCanTransitForwardTransitions().Count);

            process.OutputMainStream();
            process.OutputAndAssertEveryActivities();

            Assert.AreEqual(1, process.CompletedActivity.Descriptor.FromTransitions.GetAllCanTransitForwardTransitions().Count,
                "结束点应该只有一条有效的入线s");

            Assert.AreEqual(process.InitialActivity.Descriptor.Key, process.CurrentActivity.Descriptor.AssociatedActivityKey);
        }

        [TestMethod]
        [Description("创建一个三个活动（开始-矩阵活动-结束）的流程，进行测试。矩阵活动展开成三个活动，流转一步，退回，再流转，再退回，流转两步，再退回一步")]
        public void DynamicActivityWithActivityMatrixLastDynamicActivityReturnThriceTest()
        {
            #region 调整线属性
            //在最后一个动态活动上添加一个默认的退回线
            IRole role = PrepareSOARole((row, pds) =>
            {
                if (row.Values.GetValue(SOARolePropertyDefinition.ActivitySNColumn, string.Empty) == "10")
                {
                    row.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.TransitionsColumn])
                    {
                        Value =
                          "[{\"Name\":\"同意\",\"DefaultSelect\":true}, {\"Name\":\"不同意\",\"ToActivityKey\":\"FirstActivity\",\"DefaultSelect\":false,\"IsReturn\":true}]"
                    });
                }
                else
                    if (row.Values.GetValue(SOARolePropertyDefinition.ActivitySNColumn, string.Empty) == "20")
                    {
                        row.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.TransitionsColumn])
                        {
                            Value =
                              "[{\"Name\":\"同意\",\"DefaultSelect\":true}, {\"Name\":\"不同意\",\"ToActivityKey\":\"10\",\"DefaultSelect\":false,\"IsReturn\":true}]"
                        });
                    }
            });
            #endregion

            IWfProcessDescriptor processDesp = ProcessHelper.CreateSimpleProcessDescriptor();

            IWfActivityDescriptor templateActDesp = processDesp.Activities["NormalActivity"];

            FillDynamicActivitySimpleResource(templateActDesp, new WfResourceDescriptorCollection() { new WfRoleResourceDescriptor(role) });

            IWfProcess process = processDesp.StartupProcess(new Dictionary<string, object>() { { "CostCenter", "1001" } });

            //到第一个动态活动
            process = process.MoveToDefaultActivityByExecutor();

            process = OutputAndAssertProcessInfo(process, "第一次退回到起始点", (p) => p.MoveToReturnActivityByExecutor());

            //再流转一步，到第一个动态活动的人
            process = process.MoveToDefaultActivityByExecutor();

            process = OutputAndAssertProcessInfo(process, "第二次退回到起始点", (p) => p.MoveToReturnActivityByExecutor());

            //再流转一步，再回到第一个动态活动的人
            process = OutputAndAssertProcessInfo(process, "再流转到第一个动态活动的人", (p) => p.MoveToDefaultActivityByExecutor());

            //流转一步，到第二个动态活动的人
            process = OutputAndAssertProcessInfo(process, "再流转到第二个动态活动的人", (p) => p.MoveToDefaultActivityByExecutor());

            process = OutputAndAssertProcessInfo(process, "退回到第一个动态活动", (p) => p.MoveToReturnActivityByExecutor());

            //Assert.AreEqual(1, process.CompletedActivity.Descriptor.FromTransitions.GetAllCanTransitForwardTransitions().Count,
            //    "结束点应该只有一条有效的入线s");

            //Assert.AreEqual(process.InitialActivity.Descriptor.Key, process.CurrentActivity.Descriptor.AssociatedActivityKey);
        }

        [TestMethod]
        [Description("资源为活动矩阵的动态活动测试")]
        public void ActiveMatrixResourceTest()
        {
            IWfProcessDescriptor processDesp = ProcessHelper.GetDynamicProcessDesp();
            IWfProcess process = processDesp.StartupProcess(new Dictionary<string, object>()
				{
					{ "CostCenter", "1001" },
					{ "PayMethod", "1" },
					{ "Age", 30 }
				});

            Console.WriteLine(process.Activities.Count);

            process.OutputMainStream();
            process.OutputAndAssertEveryActivities();

            Assert.AreEqual(5, process.Activities.Count);
        }

        #region 辅助方法
        private IWfProcessDescriptor CreateComplexDynamicActivityProcess(params IUser[] users)
        {
            IWfProcessDescriptor processDesp = ProcessHelper.CreateSimpleProcessDescriptor();

            IWfActivityDescriptor templateActDesp = processDesp.Activities["NormalActivity"];
            FillDynamicActivitySimpleResource(templateActDesp, users);

            processDesp.InitialActivity.ToTransitions.RemoveByToActivity(templateActDesp);
            templateActDesp.FromTransitions.RemoveByFromActivity(processDesp.InitialActivity);
            templateActDesp.ToTransitions.RemoveByToActivity(processDesp.CompletedActivity);
            processDesp.CompletedActivity.FromTransitions.RemoveByFromActivity(templateActDesp);

            IWfActivityDescriptor a1Activity = ProcessHelper.CreateNormalActivity("A1");
            processDesp.Activities.Add(a1Activity);

            IWfActivityDescriptor a2Activity = ProcessHelper.CreateNormalActivity("A2");
            processDesp.Activities.Add(a2Activity);

            IWfActivityDescriptor b1Activity = ProcessHelper.CreateNormalActivity("B1");
            processDesp.Activities.Add(b1Activity);

            IWfActivityDescriptor b2Activity = ProcessHelper.CreateNormalActivity("B2");
            processDesp.Activities.Add(b2Activity);

            processDesp.InitialActivity.ToTransitions.AddForwardTransition(a1Activity);
            processDesp.InitialActivity.ToTransitions.AddForwardTransition(a2Activity);

            a1Activity.ToTransitions.AddForwardTransition(templateActDesp);
            a2Activity.ToTransitions.AddForwardTransition(templateActDesp);

            templateActDesp.ToTransitions.AddForwardTransition(b1Activity);
            templateActDesp.ToTransitions.AddForwardTransition(b2Activity);

            b1Activity.ToTransitions.AddForwardTransition(processDesp.CompletedActivity);
            b2Activity.ToTransitions.AddForwardTransition(processDesp.CompletedActivity);

            return processDesp;
        }

        private static void FillDynamicActivitySimpleResource(IWfActivityDescriptor actDesp, params IUser[] users)
        {
            WfUserResourceDescriptor[] resources = new WfUserResourceDescriptor[users.Length];

            for (int i = 0; i < users.Length; i++)
                resources[i] = new WfUserResourceDescriptor(users[i]);

            FillDynamicActivitySimpleResource(actDesp, resources);
        }

        private static void FillDynamicActivitySimpleResource(IWfActivityDescriptor actDesp, IEnumerable<WfResourceDescriptor> resources)
        {
            actDesp.Properties.SetValue("IsDynamic", true);

            foreach (WfResourceDescriptor resource in resources)
                actDesp.Resources.Add(resource);
        }

        /// <summary>
        /// 从动态节点的起始点验证动态节点相关属性的正确性
        /// </summary>
        /// <param name="startActDesp"></param>
        /// <param name="templateActDesp">动态活动模版</param>
        private static IWfActivityDescriptor ValidateDynamicActivities(IWfActivityDescriptor startActDesp, IWfActivityDescriptor templateActDesp)
        {
            IWfActivityDescriptor firstDynamicActDesp = null;

            Assert.IsTrue(startActDesp.ToTransitions.Count > 1, "起始点不止一条出线");

            IWfTransitionDescriptor dynTransition = startActDesp.ToTransitions.Find(t => t.Properties.GetValue("DynamicSource", string.Empty) == templateActDesp.Key);

            Assert.IsNotNull(dynTransition, "起始点有一条出线为生成的指向动态路径的线");

            foreach (IWfTransitionDescriptor transition in templateActDesp.FromTransitions)
            {
                Assert.IsFalse(transition.Enabled, "模版活动的进线都是Disabled");
                //Assert.IsTrue(transition.IsDynamicActivityTransition, "模版活动的进线的IsDynamicActivityTransition都是True");
            }

            foreach (IWfTransitionDescriptor transition in templateActDesp.ToTransitions)
            {
                Assert.IsFalse(transition.Enabled, "模版活动的出线都是Disabled");
                //Assert.IsTrue(transition.IsDynamicActivityTransition, "模版活动的出线的IsDynamicActivityTransition都是True");
            }

            firstDynamicActDesp = dynTransition.ToActivity;

            return firstDynamicActDesp;
        }

        private static void ValidateInDynamicTransitionsProperties(IEnumerable<IWfTransitionDescriptor> dynamicTransitions, IWfActivityDescriptor templateActDesp)
        {
            Assert.AreEqual(templateActDesp.FromTransitions.Count, dynamicTransitions.Count());

            foreach (IWfTransitionDescriptor transition in dynamicTransitions)
            {
                Assert.IsTrue(transition.Enabled, "动态线应该是Enabled");
                Assert.AreEqual(templateActDesp.Key, transition.Properties.GetValue("DynamicSource", string.Empty), "动态线的DynamicSource与模版的Key相同");
            }
        }

        /// <summary>
        /// 验证模版上没有资源的动态活动的动态线和模版线之间的数量匹配和属性值
        /// </summary>
        /// <param name="dynamicTransitions"></param>
        /// <param name="templateActDesp"></param>
        private static void ValidateNoResourceTransitionsProperties(IEnumerable<IWfTransitionDescriptor> dynamicTransitions, IWfActivityDescriptor templateActDesp)
        {
            Assert.AreEqual(templateActDesp.FromTransitions.Count, dynamicTransitions.Count());

            foreach (IWfTransitionDescriptor transition in dynamicTransitions)
            {
                Assert.IsTrue(transition.Enabled, "动态线应该是Enabled");
                Assert.AreEqual(templateActDesp.Key, transition.Properties.GetValue("DynamicSource", string.Empty), "动态线的DynamicSource与模版的Key相同");
            }
        }

        /// <summary>
        /// 查找最后一个动态点的出线
        /// </summary>
        /// <param name="templateActDesp"></param>
        private static void ValidateLastDynamicTransitions(IWfActivityDescriptor templateActDesp)
        {
            WfActivityDescriptorCollection toActivities = templateActDesp.GetToActivities();

            List<IWfTransitionDescriptor> dynamicTransitions = new List<IWfTransitionDescriptor>();

            foreach (IWfActivityDescriptor actDesp in toActivities)
            {
                foreach (IWfTransitionDescriptor transition in actDesp.FromTransitions)
                {
                    if (transition.Properties.GetValue("DynamicSource", string.Empty) == templateActDesp.Key)
                        dynamicTransitions.Add(transition);
                }
            }

            Assert.AreEqual(templateActDesp.ToTransitions.Count, dynamicTransitions.Count, "动态出线应该和模版出线的数量相同");
            dynamicTransitions.ForEach(t => Assert.IsTrue(t.Enabled, "动态出线应该都是Enabled"));
        }

        /// <summary>
        /// 检查模版的候选人和生成的动态角色候选人是否一样
        /// </summary>
        /// <param name="firstDynamicActDesp"></param>
        /// <param name="templateActDesp"></param>
        private static void ValidateTemplateCandidatesAndDynamicActivityCandidates(IWfActivityDescriptor firstDynamicActDesp, IWfActivityDescriptor templateActDesp)
        {
            IWfActivityDescriptor currentActDesp = firstDynamicActDesp;

            foreach (WfAssignee assignee in templateActDesp.Instance.Candidates)
            {
                Assert.IsTrue(currentActDesp.Instance.Candidates.Contains(assignee.User), "检查模版的候选人和生成的动态角色候选人是否一样");
                IWfTransitionDescriptor transition = currentActDesp.ToTransitions.GetAllCanTransitForwardTransitions().FirstOrDefault();

                Assert.IsNotNull(transition, "动态活动的数量必须和模版资源匹配");

                currentActDesp = transition.ToActivity;
            }
        }

        private static IWfProcess OutputAndAssertProcessInfo(IWfProcess process, string description, Func<IWfProcess, IWfProcess> func)
        {
            OutputAndAssertProcessInfo(process, description + "之前");

            IWfProcess result = func(process);

            OutputAndAssertProcessInfo(result, description + "之后");

            return result;
        }

        private static void OutputAndAssertProcessInfo(IWfProcess process, string description)
        {
            Console.WriteLine(description);
            Console.WriteLine("Current Activity: {0}", process.CurrentActivity.Descriptor.Key);
            Console.WriteLine("Next Default Activity Key: {0}",
                process.CurrentActivity.Descriptor.ToTransitions.GetAllCanTransitForwardTransitions().First().ToActivity.Key);

            process.OutputMainStream();
            process.OutputAndAssertEveryActivities();
        }

        #endregion 辅助方法

        #region 动态角色方法
        /// <summary>
        /// 1001应该匹配出3个活动。其中第一个活动两个人
        /// </summary>
        /// <returns></returns>
        private static SOARole PrepareSOARole(Action<SOARolePropertyRow, SOARolePropertyDefinitionCollection> action = null)
        {
            IRole originalRole = GetTestRole();

            SOARolePropertyDefinitionAdapter.Instance.Delete(originalRole);

            SOARolePropertyDefinitionCollection pds = UpdateRolePropertiesDefinition(originalRole);

            SOARole role = new SOARole(originalRole);

            role.Rows.Clear();

            AddSOARoleRow(role, SOARoleOperatorType.User, "fanhy", "1001", "10", action);
            AddSOARoleRow(role, SOARoleOperatorType.User, "yangrui1", "1001", "10", action);
            AddSOARoleRow(role, SOARoleOperatorType.User, "liming", "1001", "20", action);
            AddSOARoleRow(role, SOARoleOperatorType.User, "quym", "1001", "30", action);

            SOARolePropertiesAdapter.Instance.Update(role);

            return role;
        }

        private static SOARolePropertyRow AddSOARoleRow(SOARole role, SOARoleOperatorType operatorType, string opUser, string constCenter, string activitySN, Action<SOARolePropertyRow, SOARolePropertyDefinitionCollection> action = null)
        {
            SOARolePropertyRow row = new SOARolePropertyRow(role) { RowNumber = role.Rows.Count + 1, OperatorType = operatorType, Operator = opUser };

            SOARolePropertyDefinitionCollection pds = role.PropertyDefinitions;

            row.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = constCenter });
            row.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.ActivitySNColumn]) { Value = activitySN });
            row.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorTypeColumn]) { Value = operatorType.ToString() });
            row.Values.Add(new SOARolePropertyValue(pds[SOARolePropertyDefinition.OperatorColumn]) { Value = opUser });

            if (action != null)
                action(row, pds);

            role.Rows.Add(row);

            return row;
        }

        private static SOARolePropertyDefinitionCollection UpdateRolePropertiesDefinition(IRole role)
        {
            SOARolePropertyDefinitionCollection propertiesDefinition = PreparePropertiesDefinition(role);

            SOARolePropertyDefinitionAdapter.Instance.Update(role, propertiesDefinition);

            return propertiesDefinition;
        }

        private static SOARolePropertyDefinitionCollection PreparePropertiesDefinition(IRole role)
        {
            SOARolePropertyDefinitionCollection propertiesDefinition = new SOARolePropertyDefinitionCollection();

            propertiesDefinition.Add(new SOARolePropertyDefinition(role) { Name = "CostCenter", SortOrder = 1 });
            propertiesDefinition.Add(new SOARolePropertyDefinition(role) { Name = SOARolePropertyDefinition.ConditionColumn, SortOrder = 2 });
            propertiesDefinition.Add(new SOARolePropertyDefinition(role) { Name = SOARolePropertyDefinition.ActivitySNColumn, SortOrder = 3 });
            propertiesDefinition.Add(new SOARolePropertyDefinition(role) { Name = SOARolePropertyDefinition.ActivityPropertiesColumn, SortOrder = 4 });
            propertiesDefinition.Add(new SOARolePropertyDefinition(role) { Name = SOARolePropertyDefinition.IsMergeableColumn, SortOrder = 5, DataType = ColumnDataType.Boolean });
            propertiesDefinition.Add(new SOARolePropertyDefinition(role) { Name = SOARolePropertyDefinition.TransitionsColumn, SortOrder = 5, DataType = ColumnDataType.String });
            propertiesDefinition.Add(new SOARolePropertyDefinition(role) { Name = SOARolePropertyDefinition.OperatorTypeColumn, SortOrder = 7, DataType = ColumnDataType.String });
            propertiesDefinition.Add(new SOARolePropertyDefinition(role) { Name = SOARolePropertyDefinition.OperatorColumn, SortOrder = 8, DataType = ColumnDataType.String });


            return propertiesDefinition;
        }

        private static IRole GetTestRole()
        {
            IRole[] roles = DeluxePrincipal.GetRoles(RolesDefineConfig.GetConfig().RolesDefineCollection["testRole"].Roles);

            return roles[0];
        }
        #endregion 动态角色方法
    }
}
