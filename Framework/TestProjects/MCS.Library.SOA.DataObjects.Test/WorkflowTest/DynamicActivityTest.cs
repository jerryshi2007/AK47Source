using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.OGUPermission;
using MCS.Library.Passport;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects.Test.Executor;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.Script;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest
{
    [TestClass]
    public class DynamicActivityTest
    {
        [TestMethod]
        [TestCategory(ProcessTestHelper.DynamicActivity)]
        [Description("创建一个只有三个活动（开始-动态模版-结束）的流程，进行测试。动态活动上有一个人员")]
        public void BasicDynamicActivityTest()
        {
            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

            processDesp.Activities["NormalActivity"].RelativeLinks.Add(new WfRelativeLinkDescriptor("DynLink") { Url = "http://localhost" });

            IWfActivityDescriptor templateActDesp = processDesp.Activities["NormalActivity"];

            FillDynamicActivitySimpleResource(templateActDesp, (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object);

            IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

            IWfActivityDescriptor firstDynamicActDesp = ValidateDynamicActivities(process.InitialActivity.Descriptor, templateActDesp);

            Assert.AreEqual(templateActDesp.RelativeLinks.Count, firstDynamicActDesp.RelativeLinks.Count);

            ValidateInDynamicTransitionsProperties(firstDynamicActDesp.FromTransitions, templateActDesp);

            ValidateLastDynamicTransitions(templateActDesp);

            ValidateTemplateCandidatesAndDynamicActivityCandidates(firstDynamicActDesp, templateActDesp);
        }

        [TestMethod]
        [TestCategory(ProcessTestHelper.DynamicActivity)]
        [Description("动态模版退件测试。创建一个只有三个活动（开始-动态模版-结束）的流程，进行测试。动态活动上有一个人员")]
        public void BasicDynamicActivityReturnTest()
        {
            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

            IWfActivityDescriptor templateActDesp = processDesp.Activities["NormalActivity"];

            FillDynamicActivitySimpleResource(templateActDesp, (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object);

            IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

            ReturnExecutorTestHelper.OutputMainStream(process);

            ReturnExecutorTestHelper.OutputEveryActivities(process);

            WfProcessTestCommon.MoveToNextDefaultActivity(process);
            ReturnExecutorTestHelper.ExecuteReturnOperation(process.CurrentActivity, processDesp.InitialActivity.Key);

            //N2(Initial)->N1->N4->N5->Completed
            ReturnExecutorTestHelper.OutputMainStream(process);

            //N1->N2(Initial)->N4->N5->Completed
            ReturnExecutorTestHelper.OutputEveryActivities(process);

            Assert.AreEqual("Initial", process.CurrentActivity.Descriptor.GetAssociatedActivity().Key);
            Assert.AreEqual(2, process.CurrentActivity.Descriptor.ToTransitions.Count);

            WfProcessTestCommon.MoveToNextDefaultActivity(process);
            //Assert.AreEqual("N4", process.CurrentActivity.Descriptor.Key); //To N4

            //N2(Initial)->N1->N4->N5->Completed
            ReturnExecutorTestHelper.OutputMainStream(process);

            //N1->N2(Initial)->N4->N5->Completed
            ReturnExecutorTestHelper.OutputEveryActivities(process);

            //IWfActivityDescriptor firstDynamicActDesp = ValidateDynamicActivities(process.InitialActivity.Descriptor, templateActDesp);

            //ValidateInDynamicTransitionsProperties(firstDynamicActDesp.FromTransitions, templateActDesp);

            //ValidateLastDynamicTransitions(templateActDesp);

            //ValidateTemplateCandidatesAndDynamicActivityCandidates(firstDynamicActDesp, templateActDesp);
        }

        [TestMethod]
        [TestCategory(ProcessTestHelper.DynamicActivity)]
        [Description("创建一个只有三个活动（开始-动态模版-结束）的流程，进行测试。动态活动上有两个人员，会生成两个动态活动")]
        public void BasicDynamicActivityWithTwoAssigneesTest()
        {
            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

            IWfActivityDescriptor templateActDesp = processDesp.Activities["NormalActivity"];

            FillDynamicActivitySimpleResource(templateActDesp,
                (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object,
                (IUser)OguObjectSettings.GetConfig().Objects["approver2"].Object);

            IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

            IWfActivityDescriptor firstDynamicActDesp = ValidateDynamicActivities(process.InitialActivity.Descriptor, templateActDesp);

            ValidateInDynamicTransitionsProperties(firstDynamicActDesp.FromTransitions, templateActDesp);

            WfActivityDescriptorCollection dynamicToActivities = firstDynamicActDesp.GetToActivities();

            ValidateLastDynamicTransitions(templateActDesp);

            ValidateTemplateCandidatesAndDynamicActivityCandidates(firstDynamicActDesp, templateActDesp);
        }

        [TestMethod]
        [TestCategory(ProcessTestHelper.DynamicActivity)]
        [Description("创建一个只有三个活动（开始-动态-结束）的流程，进行测试。动态活动上没有资源")]
        public void BasicDynamicActivityWithoutResourceTest()
        {
            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

            IWfActivityDescriptor templateActDesp = processDesp.Activities["NormalActivity"];
            FillDynamicActivitySimpleResource(templateActDesp);

            IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

            IWfActivityDescriptor firstDynamicActDesp = ValidateDynamicActivities(process.InitialActivity.Descriptor, templateActDesp);

            ValidateNoResourceTransitionsProperties(processDesp.InitialActivity.ToTransitions.FindAll(t => t.Properties.GetValue("DynamicSource", string.Empty) == templateActDesp.Key), templateActDesp);

            Assert.AreEqual(processDesp.CompletedActivity.Key, firstDynamicActDesp.Key, "起始点和终止点连接在一起");
        }

        [TestMethod]
        [TestCategory(ProcessTestHelper.DynamicActivity)]
        [Description("创建一个有七个活动（开始-A1~A2-动态-B1~B2-结束）的流程，进行测试。动态活动上没有资源")]
        public void ComplexDynamicActivityWithoutResourceTest()
        {
            IWfProcessDescriptor processDesp = CreateComplexDynamicActivityProcess();
            IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

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
        [TestCategory(ProcessTestHelper.DynamicActivity)]
        [Description("创建一个有七个活动（开始-A1~A2-动态-B1~B2-结束）的流程，进行测试。动态活动上有两个指派人")]
        public void ComplexDynamicActivityWithTwoAssigneesTest()
        {
            IWfProcessDescriptor processDesp = CreateComplexDynamicActivityProcess(
                (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object,
                (IUser)OguObjectSettings.GetConfig().Objects["approver2"].Object);

            IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

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

        [TestMethod]
        [TestCategory(ProcessTestHelper.DynamicActivity)]
        [Description("创建一个三个活动（开始-矩阵活动-结束）的流程，进行测试。矩阵活动展开成三个活动")]
        public void DynamicActivityWithActivityMatrixTest()
        {
            IRole role = PrepareSOARole();

            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

            IWfActivityDescriptor templateActDesp = processDesp.Activities["NormalActivity"];

            FillDynamicActivitySimpleResource(templateActDesp, new WfResourceDescriptorCollection() { new WfRoleResourceDescriptor(role) });

            IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp, new Dictionary<string, object>() { { "CostCenter", "1001" } });

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
        [TestCategory(ProcessTestHelper.DynamicActivity)]
        [Description("创建一个三个活动（开始-矩阵活动-结束）的流程，进行测试。没有流程上下文，矩阵活动0个活动")]
        public void DynamicActivityActivityMatrixWithoutProcessContextTest()
        {
            IRole role = PrepareSOARole();

            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

            IWfActivityDescriptor templateActDesp = processDesp.Activities["NormalActivity"];

            FillDynamicActivitySimpleResource(templateActDesp, new WfResourceDescriptorCollection() { new WfRoleResourceDescriptor(role) });

            IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp);

            Assert.AreEqual(3, process.Activities.Count);

            IWfActivityDescriptor firstDynamicActDesp = ValidateDynamicActivities(process.InitialActivity.Descriptor, templateActDesp);

            ValidateNoResourceTransitionsProperties(processDesp.InitialActivity.ToTransitions.FindAll(t => t.Properties.GetValue("DynamicSource", string.Empty) == templateActDesp.Key), templateActDesp);

            Assert.AreEqual(processDesp.CompletedActivity.Key, firstDynamicActDesp.Key, "起始点和终止点连接在一起");
        }

        [TestMethod]
        [TestCategory(ProcessTestHelper.DynamicActivity)]
        [Description("创建一个三个活动（开始-矩阵活动-结束）的流程，进行测试。矩阵活动展开成三个活动，然后在动态活动内部退回")]
        public void DynamicActivityWithActivityMatrixReturnToInnerTest()
        {
            IRole role = PrepareSOARole();

            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

            IWfActivityDescriptor templateActDesp = processDesp.Activities["NormalActivity"];

            FillDynamicActivitySimpleResource(templateActDesp, new WfResourceDescriptorCollection() { new WfRoleResourceDescriptor(role) });

            IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp, new Dictionary<string, object>() { { "CostCenter", "1001" } });

            //到第一个动态活动
            WfProcessTestCommon.MoveToNextDefaultActivityWithExecutor(process);

            process = WfRuntime.GetProcessByProcessID(process.ID);

            string targetActKey = process.CurrentActivity.Descriptor.Key;

            //到第二个动态活动
            WfProcessTestCommon.MoveToNextDefaultActivityWithExecutor(process);

            process = WfRuntime.GetProcessByProcessID(process.ID);

            //退回到第一个动态活动
            ReturnExecutorTestHelper.ExecuteReturnOperation(process.CurrentActivity, targetActKey);

            process = WfRuntime.GetProcessByProcessID(process.ID);

            Console.WriteLine(process.CurrentActivity.Descriptor.Key);

            ReturnExecutorTestHelper.OutputMainStream(process);
            ReturnExecutorTestHelper.OutputEveryActivities(process);

            Assert.AreEqual(targetActKey, process.CurrentActivity.Descriptor.AssociatedActivityKey);
        }

        [TestMethod]
        [TestCategory(ProcessTestHelper.DynamicActivity)]
        [Description("创建一个三个活动（开始-矩阵活动-结束）的流程，进行测试。矩阵活动展开成三个活动，然后退回到起始点")]
        public void DynamicActivityWithActivityMatrixReturnToInitialTest()
        {
            IRole role = PrepareSOARole();

            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

            IWfActivityDescriptor templateActDesp = processDesp.Activities["NormalActivity"];

            FillDynamicActivitySimpleResource(templateActDesp, new WfResourceDescriptorCollection() { new WfRoleResourceDescriptor(role) });

            IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp, new Dictionary<string, object>() { { "CostCenter", "1001" } });

            //到第一个动态活动
            WfProcessTestCommon.MoveToNextDefaultActivityWithExecutor(process);

            process = WfRuntime.GetProcessByProcessID(process.ID);

            //到第二个动态活动
            WfProcessTestCommon.MoveToNextDefaultActivityWithExecutor(process);

            process = WfRuntime.GetProcessByProcessID(process.ID);

            //退回到起始点
            ReturnExecutorTestHelper.ExecuteReturnOperation(process.CurrentActivity, process.InitialActivity.Descriptor.Key);

            process = WfRuntime.GetProcessByProcessID(process.ID);

            Console.WriteLine(process.CurrentActivity.Descriptor.Key);
            Console.WriteLine(process.CompletedActivity.Descriptor.FromTransitions.GetAllCanTransitForwardTransitions().Count);

            ReturnExecutorTestHelper.OutputMainStream(process);
            ReturnExecutorTestHelper.OutputEveryActivities(process);

            Assert.AreEqual(1, process.CompletedActivity.Descriptor.FromTransitions.GetAllCanTransitForwardTransitions().Count,
                "结束点应该只有一条有效的入线s");

            Assert.AreEqual(process.InitialActivity.Descriptor.Key, process.CurrentActivity.Descriptor.AssociatedActivityKey);
        }

        [TestMethod]
        [TestCategory(ProcessTestHelper.DynamicActivity)]
        [Description("资源为活动矩阵的动态活动测试")]
        public void ActiveMatrixResourceTest()
        {
            IWfProcessDescriptor processDesp = WfProcessTestCommon.GetDynamicProcessDesp();
            IWfProcess process = WfProcessTestCommon.StartupProcess(processDesp, new Dictionary<string, object>()
				{
					{ "CostCenter", "1001" },
					{ "PayMethod", "1" },
					{ "Age", 30 }
				});

            Console.WriteLine(process.Activities.Count);

            ReturnExecutorTestHelper.OutputMainStream(process);
            ReturnExecutorTestHelper.OutputEveryActivities(process);

            Assert.AreEqual(4, process.Activities.Count);
        }

        [TestMethod]
        [TestCategory(ProcessTestHelper.DynamicActivity)]
        [Description("资源为活动矩阵的动态序列化测试")]
        public void ActiveMatrixResourceSerializationTest()
        {
            WfActivityMatrixResourceDescriptor expected = WfProcessTestCommon.PrepareActivityMatrixResourceDescriptor();

            string serializedData = SerializationHelper.SerializeObjectToString(expected, SerializationFormatterType.Binary);

            WfActivityMatrixResourceDescriptor actual = SerializationHelper.DeserializeStringToObject<WfActivityMatrixResourceDescriptor>(serializedData, SerializationFormatterType.Binary);

            Assert.AreEqual(expected.PropertyDefinitions.Count, actual.PropertyDefinitions.Count);
            Assert.AreEqual(expected.Rows.Count, actual.Rows.Count);

            Assert.AreEqual(expected.PropertyDefinitions.GetAllKeys().Count(), actual.PropertyDefinitions.GetAllKeys().Count());
        }

        [TestMethod]
        [TestCategory(ProcessTestHelper.DynamicActivity)]
        [Description("资源为活动矩阵的Json序列化测试")]
        public void ActiveMatrixResourceJsonSerializationTest()
        {
            WfActivityMatrixResourceDescriptor expected = WfProcessTestCommon.PrepareActivityMatrixResourceDescriptor();

            WfConverterHelper.RegisterConverters();

            string serializedData = JSONSerializerExecute.Serialize(expected);

            Console.WriteLine(serializedData);

            WfActivityMatrixResourceDescriptor actual = JSONSerializerExecute.Deserialize<WfActivityMatrixResourceDescriptor>(serializedData);

            Assert.AreEqual(expected.PropertyDefinitions.Count, actual.PropertyDefinitions.Count);
            Assert.AreEqual(expected.Rows.Count, actual.Rows.Count);

            Assert.AreEqual(expected.PropertyDefinitions.GetAllKeys().Count(), actual.PropertyDefinitions.GetAllKeys().Count());
        }

        #region 辅助方法
        private IWfProcessDescriptor CreateComplexDynamicActivityProcess(params IUser[] users)
        {
            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

            IWfActivityDescriptor templateActDesp = processDesp.Activities["NormalActivity"];
            FillDynamicActivitySimpleResource(templateActDesp, users);

            processDesp.InitialActivity.ToTransitions.RemoveByToActivity(templateActDesp);
            templateActDesp.FromTransitions.RemoveByFromActivity(processDesp.InitialActivity);
            templateActDesp.ToTransitions.RemoveByToActivity(processDesp.CompletedActivity);
            processDesp.CompletedActivity.FromTransitions.RemoveByFromActivity(templateActDesp);

            IWfActivityDescriptor a1Activity = WfProcessTestCommon.CreateNormalActivity("A1");
            processDesp.Activities.Add(a1Activity);

            IWfActivityDescriptor a2Activity = WfProcessTestCommon.CreateNormalActivity("A2");
            processDesp.Activities.Add(a2Activity);

            IWfActivityDescriptor b1Activity = WfProcessTestCommon.CreateNormalActivity("B1");
            processDesp.Activities.Add(b1Activity);

            IWfActivityDescriptor b2Activity = WfProcessTestCommon.CreateNormalActivity("B2");
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
        #endregion 辅助方法

        #region 动态角色方法
        private static SOARole PrepareSOARole()
        {
            IRole originalRole = GetTestRole();

            SOARolePropertyDefinitionAdapter.Instance.Delete(originalRole);

            SOARolePropertyDefinitionCollection pds = UpdateRolePropertiesDefinition(originalRole);

            SOARole role = new SOARole(originalRole);

            role.Rows.Clear();

            AddSOARoleRow(role, SOARoleOperatorType.User, "fanhy", "1001", "10");
            AddSOARoleRow(role, SOARoleOperatorType.User, "yangrui1", "1001", "10");
            AddSOARoleRow(role, SOARoleOperatorType.User, "liming", "1001", "20");
            AddSOARoleRow(role, SOARoleOperatorType.User, "quym", "1001", "30");

            SOARolePropertiesAdapter.Instance.Update(role);

            return role;
        }

        private static SOARolePropertyRow AddSOARoleRow(SOARole role, SOARoleOperatorType operatorType, string opUser, string constCenter, string activitySN)
        {
            SOARolePropertyRow row = new SOARolePropertyRow(role) { RowNumber = role.Rows.Count + 1, OperatorType = operatorType, Operator = opUser };

            SOARolePropertyDefinitionCollection pds = role.PropertyDefinitions;

            row.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = constCenter });
            row.Values.Add(new SOARolePropertyValue(pds["ActivitySN"]) { Value = activitySN });
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

            propertiesDefinition.Add(new SOARolePropertyDefinition(role) { Name = "CostCenter", SortOrder = 0 });
            propertiesDefinition.Add(new SOARolePropertyDefinition(role) { Name = "Condition", SortOrder = 3 });
            propertiesDefinition.Add(new SOARolePropertyDefinition(role) { Name = "ActivitySN", SortOrder = 4 });
            propertiesDefinition.Add(new SOARolePropertyDefinition(role) { Name = "ActivityProperties", SortOrder = 5 });
            propertiesDefinition.Add(new SOARolePropertyDefinition(role) { Name = "IsMergeable", SortOrder = 6, DataType = ColumnDataType.Boolean });

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
