using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.OGUPermission;
using MCS.Library.Passport;
using MCS.Library.SOA.DataObjects.Test.WorkflowTest;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace MCS.Library.SOA.DataObjects.Test
{
    public static class WfProcessTestCommon
    {
        public static IWfProcessDescriptor LoadProcessDescriptorFromFile(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                return WfProcessDescriptorManager.LoadDescriptor(stream);
            }
        }

        /// <summary>
        /// 创建一个基本流程描述。只有3个节点. "Initial"->"NormalActivity"->"Completed"
        /// </summary>
        /// <returns></returns>
        public static IWfProcessDescriptor CreateSimpleProcessDescriptor()
        {
            WfProcessDescriptor processDesp = new WfProcessDescriptor();

            processDesp.Key = "TestProcess" + UuidHelper.NewUuidString().Substring(0, 8);
            processDesp.Name = "测试流程";
            processDesp.ApplicationName = "TEST_APP_NAME";
            processDesp.ProgramName = "TEST_PROGRAM_NAME";
            processDesp.Url = "/MCS_Framework/WebTestProject/defaultHandler.aspx";

            WfActivityDescriptor initAct = new WfActivityDescriptor("Initial", WfActivityType.InitialActivity);
            initAct.Name = "Initial";
            initAct.CodeName = "Initial Activity";

            processDesp.Activities.Add(initAct);

            WfActivityDescriptor normalAct = new WfActivityDescriptor("NormalActivity", WfActivityType.NormalActivity);

            normalAct.Name = "Normal";
            normalAct.CodeName = "Normal Activity";

            normalAct.Properties.SetValue("AutoMoveAfterPending", true);

            processDesp.Activities.Add(normalAct);

            processDesp.RelativeLinks.Add(new WfRelativeLinkDescriptor("TestLink") { Name = "测试链接", Url = "/MCSWebApp/Sample.htm" });

            WfActivityDescriptor completedAct = new WfActivityDescriptor("Completed", WfActivityType.CompletedActivity);
            completedAct.Name = "Completed";
            completedAct.CodeName = "Completed Activity";

            completedAct.RelativeLinks.Add(new WfRelativeLinkDescriptor("TestLink") { Name = "测试链接", Url = "/MCSWebApp/Sample.htm" });

            processDesp.Activities.Add(completedAct);

            initAct.ToTransitions.AddForwardTransition(normalAct);
            normalAct.ToTransitions.AddForwardTransition(completedAct);

            return processDesp;
        }

        /// <summary>
        /// 创建一条有三个节点的简单流程，并且第二个点带一根退回线。
        /// </summary>
        /// <returns></returns>
        public static IWfProcessDescriptor CreateSimpleProcessDescriptorWithReturnLine()
        {
            IWfProcessDescriptor processDesp = CreateSimpleProcessDescriptor();

            IWfActivityDescriptor normalActivity = processDesp.Activities["NormalActivity"];

            normalActivity.ToTransitions.AddBackwardTransition(processDesp.InitialActivity);

            return processDesp;
        }

        /// <summary>
        /// 创建一个只有开始和结束点的流程描述
        /// </summary>
        /// <returns></returns>
        public static IWfProcessDescriptor CreateInitAndCompletedProcessDescriptor()
        {
            WfProcessDescriptor processDesp = new WfProcessDescriptor();

            processDesp.Key = "TestProcess" + UuidHelper.NewUuidString().Substring(0, 8);
            processDesp.Name = "测试流程";
            processDesp.ApplicationName = "TEST_APP_NAME";
            processDesp.ProgramName = "TEST_PROGRAM_NAME";
            processDesp.Url = "/MCS_Framework/WebTestProject/defaultHandler.aspx";

            WfActivityDescriptor initAct = new WfActivityDescriptor("Initial", WfActivityType.InitialActivity);
            initAct.Name = "Initial";
            initAct.CodeName = "Initial Activity";

            processDesp.Activities.Add(initAct);

            WfActivityDescriptor completedAct = new WfActivityDescriptor("Completed", WfActivityType.CompletedActivity);
            completedAct.Name = "Completed";
            completedAct.CodeName = "Completed Activity";

            completedAct.RelativeLinks.Add(new WfRelativeLinkDescriptor("TestLink") { Name = "测试链接", Url = "/MCSWebApp/Sample.htm" });

            processDesp.Activities.Add(completedAct);

            initAct.ToTransitions.AddForwardTransition(completedAct);

            return processDesp;
        }

        public static IWfProcessDescriptor CreateSimpleProcessDescriptorWithSecretary()
        {
            IWfProcessDescriptor processDesp = CreateSimpleProcessDescriptor();

            processDesp.Activities["NormalActivity"].Resources.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["ceo"].Object));
            processDesp.Activities["NormalActivity"].Properties.SetValue("AutoAppendSecretary", true);

            return processDesp;
        }

        /// <summary>
        /// 创建一个带秘书的分支流程，且存在自动创建的同意和不同意线
        /// </summary>
        /// <returns></returns>
        public static IWfProcessDescriptor CreateBranchProcessWithSecretaryAndAgreeLine()
        {
            IWfProcessDescriptor processDesp = CreateSimpleProcessDescriptor();

            IWfActivityDescriptor normalActDesp = processDesp.Activities["NormalActivity"];

            //normalActDesp.Resources.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object));
            normalActDesp.Resources.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["ceo"].Object));

            normalActDesp.Properties.TrySetValue("AutoMoveAfterPending", true);
            normalActDesp.Properties.TrySetValue("AutoStartBranchProcessExecuteSequence", WfBranchProcessExecuteSequence.SerialInSameProcess);
            normalActDesp.Properties.TrySetValue("AutoStartBranchProcessKey", "DefaultApprovalProcess");
            normalActDesp.Properties.TrySetValue("AutoStartBranchApprovalProcess", true);
            normalActDesp.Properties.TrySetValue("SubProcessApprovalMode", WfSubProcessApprovalMode.LastActivityDecide);
            normalActDesp.Properties.TrySetValue("BlockingType", WfBranchGroupBlockingType.WaitAllBranchGroupsComplete);

            return processDesp;
        }

        /// <summary>
        /// 创建一个带秘书的活动，且该活动有一根指向首节点的退回线
        /// </summary>
        /// <returns></returns>
        public static IWfProcessDescriptor CreateSimpleProcessDescriptorWithSecretaryAndReturnLine()
        {
            IWfProcessDescriptor processDesp = CreateSimpleProcessDescriptor();

            IWfActivityDescriptor normalActivity = processDesp.Activities["NormalActivity"];

            normalActivity.Resources.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["ceo"].Object));
            normalActivity.Properties.SetValue("AutoAppendSecretary", true);
            normalActivity.ToTransitions.AddBackwardTransition(processDesp.InitialActivity);

            return processDesp;
        }

        public static IWfActivityDescriptor CreateNormalActivity(string key)
        {
            WfActivityDescriptor normalAct = new WfActivityDescriptor(key, WfActivityType.NormalActivity);
            normalAct.Name = key;
            normalAct.CodeName = key;

            return normalAct;
        }

        /// <summary>
        /// 带分支流程模板的简单流程。总共两个模板。分支流程模板加在第二个点上，是并行分支流程，且等待所有子流程结束。
        /// </summary>
        /// <returns></returns>
        public static IWfProcessDescriptor CreateSimpleProcessDescriptorWithBranchTemplate()
        {
            IWfProcessDescriptor processDesp = CreateSimpleProcessDescriptor();

            IWfActivityDescriptor normalActivity = processDesp.Activities["NormalActivity"];

            IWfBranchProcessTemplateDescriptor template = CreateTemplate("Consign",
                (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object);

            normalActivity.BranchProcessTemplates.Add(template);

            template = CreateTemplate("Distribute",
               (IUser)OguObjectSettings.GetConfig().Objects["approver2"].Object);

            normalActivity.BranchProcessTemplates.Add(template);

            return processDesp;
        }

        /// <summary>
        /// 带分支流程模板的简单流程。总共两个模板。且每一个活动上都有人
        /// </summary>
        /// <returns></returns>
        public static IWfProcessDescriptor CreateProcessDescriptorWithBranchTemplateAndUsers()
        {
            IWfProcessDescriptor processDesp = CreateSimpleProcessDescriptor();

            processDesp.InitialActivity.Resources.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object));
            processDesp.InitialActivity.Resources.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object));

            IWfActivityDescriptor normalActivity = processDesp.Activities["NormalActivity"];

            IWfBranchProcessTemplateDescriptor template = CreateTemplate("Consign",
                (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object);

            normalActivity.BranchProcessTemplates.Add(template);

            template = CreateTemplate("Distribute",
               (IUser)OguObjectSettings.GetConfig().Objects["approver2"].Object);

            normalActivity.BranchProcessTemplates.Add(template);

            return processDesp;
        }

        /// <summary>
        /// 创建带分支流程的模版。为了测试XElement序列化，里面需要包含角色、资源、条件等内容
        /// </summary>
        /// <returns></returns>
        public static IWfProcessDescriptor CreateProcessDescriptorForXElementSerialization()
        {
            IWfProcessDescriptor processDesp = CreateSimpleProcessDescriptorWithBranchTemplate();

            IWfActivityDescriptor normalActivity = processDesp.Activities["NormalActivity"];

            string roleDesp = RolesDefineConfig.GetConfig().RolesDefineCollection["testRole"].Roles;

            OguRole role = new OguRole(roleDesp);

            WfRoleResourceDescriptor roleResource = new WfRoleResourceDescriptor(role);
            normalActivity.EnterEventReceivers.Add(roleResource);

            WfDynamicResourceDescriptor dynResource = new WfDynamicResourceDescriptor();
            dynResource.Condition.Expression = "Leader";
            normalActivity.LeaveEventReceivers.Add(dynResource);

            processDesp.CancelEventReceivers.Add(dynResource);

            WfRelativeLinkDescriptor relLink = new WfRelativeLinkDescriptor("TestUrl");

            relLink.Category = "Test";
            relLink.Url = "http://localhost/www.baidu.com";

            processDesp.RelativeLinks.Add(relLink);

            return processDesp;
        }

        /// <summary>
        /// 流转到默认的下一个活动
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static IWfActivity MoveToNextDefaultActivity(IWfProcess process)
        {
            IWfTransitionDescriptor transition = process.CurrentActivity.Descriptor.ToTransitions.GetAllCanTransitTransitions(true).FirstOrDefault();

            Assert.IsNotNull(transition,
                string.Format("活动{0}没有能够使用的出线", process.CurrentActivity.Descriptor.Key));

            WfTransferParams transferParams = new WfTransferParams(transition.ToActivity);

            process.MoveTo(transferParams);

            return process.CurrentActivity;
        }

        /// <summary>
        /// 通过Executor流转到默认的下一个活动
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static IWfActivity MoveToNextDefaultActivityWithExecutor(IWfProcess process)
        {
            IWfTransitionDescriptor transition = process.CurrentActivity.Descriptor.ToTransitions.GetAllCanTransitTransitions(true).FirstOrDefault();

            Assert.IsNotNull(transition,
                string.Format("活动{0}没有能够使用的出线", process.CurrentActivity.Descriptor.Key));

            WfTransferParams transferParams = new WfTransferParams(transition.ToActivity);

            transferParams.FromTransitionDescriptor = transition;

            WfMoveToExecutor executor = new WfMoveToExecutor(process.CurrentActivity, process.CurrentActivity, transferParams);

            executor.Execute();

            return process.CurrentActivity;
        }

        /// <summary>
        /// 通过Executor流转到默认的下一个活动，但是不保存
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static IWfActivity MoveToNextDefaultActivityWithNoPersistExecutor(IWfProcess process)
        {
            IWfTransitionDescriptor transition = process.CurrentActivity.Descriptor.ToTransitions.GetAllCanTransitTransitions(true).FirstOrDefault();

            Assert.IsNotNull(transition,
                string.Format("活动{0}没有能够使用的出线", process.CurrentActivity.Descriptor.Key));

            WfTransferParams transferParams = new WfTransferParams(transition.ToActivity);

            transferParams.FromTransitionDescriptor = transition;

            WfMoveToExecutor executor = new WfMoveToExecutor(process.CurrentActivity, process.CurrentActivity, transferParams);

            executor.ExecuteNotPersist();

            return process.CurrentActivity;
        }

        public static IWfBranchProcessTemplateDescriptor CreateTemplate(string key, IUser user)
        {
            WfBranchProcessTemplateDescriptor template = new WfBranchProcessTemplateDescriptor(key);

            template.BranchProcessKey = WfProcessDescriptorManager.DefaultConsignProcessKey;
            template.ExecuteSequence = WfBranchProcessExecuteSequence.Parallel;
            template.BlockingType = WfBranchProcessBlockingType.WaitAllBranchProcessesComplete;
            template.Resources.Add(new WfUserResourceDescriptor(user));

            return template;
        }

        /// <summary>
        /// 创建一个有四个节点的带条件流程，Init，Complete，Normal和Manager
        /// </summary>
        /// <returns></returns>
        public static IWfProcessDescriptor CreateSimpleProcessDescriptorWithCondition()
        {
            IWfProcessDescriptor processDesp = CreateSimpleProcessDescriptor();

            //直接到结束点
            ((IWfForwardTransitionDescriptor)processDesp.Activities["NormalActivity"].ToTransitions[0]).Condition.Expression = "Amount < 5000";

            WfActivityDescriptor mgrAct = new WfActivityDescriptor("ManagerActivity", WfActivityType.NormalActivity);

            mgrAct.Name = "Manager";
            mgrAct.CodeName = "Manager";

            processDesp.Activities.Add(mgrAct);

            //经过管理员点
            IWfForwardTransitionDescriptor transition = processDesp.Activities["NormalActivity"].ToTransitions.AddForwardTransition(mgrAct);

            transition.Condition.Expression = "Amount >= 5000";

            mgrAct.ToTransitions.AddForwardTransition(processDesp.CompletedActivity);

            return processDesp;
        }

        /// <summary>
        /// 创建一个有多个节点的流程
        /// </summary>
        /// <returns></returns>
        public static IWfProcessDescriptor CreateProcessDescriptor()
        {
            WfProcessDescriptor processDesp = new WfProcessDescriptor();

            processDesp.Key = "TestProcess" + UuidHelper.NewUuidString().Substring(0, 8);
            processDesp.Name = "测试流程";
            processDesp.ApplicationName = "TEST_APP_NAME";
            processDesp.ProgramName = "TEST_PROGRAM_NAME";
            processDesp.Url = "/MCS_Framework/WebTestProject/defaultHandler.aspx";

            WfActivityDescriptor initAct = new WfActivityDescriptor("Initial", WfActivityType.InitialActivity);
            initAct.Name = "Initial";
            initAct.CodeName = "Initial Activity";

            processDesp.Activities.Add(initAct);

            WfActivityDescriptor normalAct = new WfActivityDescriptor("NormalActivity", WfActivityType.NormalActivity);
            normalAct.Name = "Normal";
            normalAct.CodeName = "Normal Activity";

            processDesp.Activities.Add(normalAct);

            WfActivityDescriptor normalAct1 = new WfActivityDescriptor("NormalActivity1", WfActivityType.NormalActivity);
            normalAct1.Name = "Normal";
            normalAct1.CodeName = "Normal Activity1";

            processDesp.Activities.Add(normalAct1);

            WfActivityDescriptor normalAct2 = new WfActivityDescriptor("NormalActivity2", WfActivityType.NormalActivity);
            normalAct2.Name = "Normal";
            normalAct2.CodeName = "Normal Activity2";

            processDesp.Activities.Add(normalAct2);

            WfActivityDescriptor normalAct3 = new WfActivityDescriptor("NormalActivity3", WfActivityType.NormalActivity);
            normalAct3.Name = "Normal";
            normalAct3.CodeName = "Normal Activity3";

            processDesp.Activities.Add(normalAct3);

            WfActivityDescriptor completedAct = new WfActivityDescriptor("Completed", WfActivityType.CompletedActivity);
            completedAct.Name = "Completed";
            completedAct.CodeName = "Completed Activity";

            processDesp.Activities.Add(completedAct);

            initAct.ToTransitions.AddForwardTransition(normalAct);
            normalAct.ToTransitions.AddForwardTransition(normalAct1);
            normalAct1.ToTransitions.AddForwardTransition(normalAct2);
            normalAct2.ToTransitions.AddForwardTransition(normalAct3);
            normalAct3.ToTransitions.AddForwardTransition(completedAct);

            return processDesp;
        }

        public static IWfProcess StartupProcess(IWfProcessDescriptor processDesp)
        {
            return StartupProcess(processDesp, new Dictionary<string, object>());
        }

        public static IWfProcess StartupProcess(IWfProcessDescriptor processDesp, Dictionary<string, object> runtimeParameters)
        {
            ProcessContextAction();

            WfProcessStartupParams startupParams = new WfProcessStartupParams();
            startupParams.ResourceID = UuidHelper.NewUuidString();
            startupParams.ProcessDescriptor = processDesp;

            runtimeParameters.ForEach(kp => startupParams.ApplicationRuntimeParameters.Add(kp.Key, kp.Value));

            return WfRuntime.StartWorkflow(startupParams);
        }

        public static IWfProcess StartupSimpleProcess()
        {
            ProcessContextAction();

            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

            WfProcessStartupParams startupParams = new WfProcessStartupParams();
            startupParams.ResourceID = UuidHelper.NewUuidString();
            startupParams.ProcessDescriptor = processDesp;

            return WfRuntime.StartWorkflow(startupParams);
        }

        public static IWfProcess StartupSimpleProcessWithAssignee()
        {
            ProcessContextAction();

            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();
            processDesp.InitialActivity.Resources.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object));

            WfProcessStartupParams startupParams = GetInstanceOfWfProcessStartupParams(processDesp);

            IWfProcess process = WfRuntime.StartWorkflow(startupParams);

            process.ApplicationRuntimeParameters["CostCenter"] = "1001";
            process.ApplicationRuntimeParameters["Requestor"] = process.Creator;

            return process;
        }

        /// <summary>
        /// 将服务定义加入流程每个节点上
        /// </summary>
        /// <param name="enter"></param>
        /// <param name="leave"></param>
        /// <returns></returns>
        public static IWfProcess StartupProcessWithServiceDefinition(WfServiceOperationDefinition enter,
            WfServiceOperationDefinition leave)
        {
            ProcessContextAction();

            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateProcessDescriptor();

            foreach (var item in processDesp.Activities)
            {
                item.EnterEventExecuteServices.Add(enter);
                item.LeaveEventExecuteServices.Add(leave);
            }

            processDesp.InitialActivity.Resources.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object));

            WfProcessStartupParams startupParams = GetInstanceOfWfProcessStartupParams(processDesp);

            return WfRuntime.StartWorkflow(startupParams);
        }

        public static IWfProcess StartupProcessWithAssignee()
        {
            ProcessContextAction();

            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateProcessDescriptor();
            processDesp.InitialActivity.Resources.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object));

            WfProcessStartupParams startupParams = GetInstanceOfWfProcessStartupParams(processDesp);

            return WfRuntime.StartWorkflow(startupParams);
        }

        public static IWfProcess StartupSimpleProcessDescriptorWithTransitionCondition()
        {
            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithCondition();
            processDesp.InitialActivity.Resources.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object));

            WfProcessStartupParams startupParams = GetInstanceOfWfProcessStartupParams(processDesp);

            return WfRuntime.StartWorkflow(startupParams);
        }

        public static IWfProcess StartupSimpleProcessDescriptorWithActivityCondition()
        {
            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithCondition();

            processDesp.InitialActivity.Resources.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object));

            processDesp.Activities["NormalActivity"].Condition.Expression = "Amount >= 5000";

            WfProcessStartupParams startupParams = GetInstanceOfWfProcessStartupParams(processDesp);
            startupParams.DefaultTaskTitle = "测试流程节点带条件";

            return WfRuntime.StartWorkflow(startupParams);
        }

        public static WfProcessStartupParams GetInstanceOfWfProcessStartupParams(IWfProcessDescriptor procDesp)
        {
            WfProcessStartupParams startupParams = new WfProcessStartupParams();

            IUser requestor = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object;

            startupParams.Creator = requestor;
            startupParams.Department = requestor.TopOU;
            startupParams.Assignees.Add(new WfAssignee(requestor));
            startupParams.ResourceID = UuidHelper.NewUuidString();
            startupParams.DefaultTaskTitle = "测试保存的流程";
            startupParams.ProcessDescriptor = procDesp;
            startupParams.AutoCommit = true;

            return startupParams;
        }

        public static IWfProcessDescriptor GetDynamicProcessDesp()
        {
            IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

            IWfActivityDescriptor normalActDesp = processDesp.Activities["NormalActivity"];

            normalActDesp.Properties.SetValue("IsDynamic", true);

            WfActivityMatrixResourceDescriptor resource = PrepareActivityMatrixResourceDescriptor();

            normalActDesp.Resources.Add(resource);

            return processDesp;
        }

        public static WfActivityMatrixResourceDescriptor PrepareActivityMatrixResourceDescriptor()
        {
            WfActivityMatrixResourceDescriptor resource = new WfActivityMatrixResourceDescriptor();

            resource.PropertyDefinitions.CopyFrom(PreparePropertiesDefinition());
            resource.Rows.CopyFrom(PrepareRows(resource.PropertyDefinitions));

            return resource;
        }

        private static SOARolePropertyDefinitionCollection PreparePropertiesDefinition()
        {
            SOARolePropertyDefinitionCollection propertiesDefinition = new SOARolePropertyDefinitionCollection();

            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "CostCenter", SortOrder = 0 });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "PayMethod", SortOrder = 1 });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "Age", SortOrder = 2, DataType = ColumnDataType.Integer });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "OperatorType", SortOrder = 3, DataType = ColumnDataType.String });
            propertiesDefinition.Add(new SOARolePropertyDefinition() { Name = "Operator", SortOrder = 4, DataType = ColumnDataType.String });

            return propertiesDefinition;
        }

        private static SOARolePropertyRowCollection PrepareRows(SOARolePropertyDefinitionCollection pds)
        {
            SOARolePropertyRowCollection rows = new SOARolePropertyRowCollection();

            SOARolePropertyRow row1 = new SOARolePropertyRow() { RowNumber = 1, OperatorType = SOARoleOperatorType.User, Operator = "fanhy" };

            row1.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1001" });
            row1.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "1" });
            row1.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "30" });

            SOARolePropertyRow row2 = new SOARolePropertyRow() { RowNumber = 2, OperatorType = SOARoleOperatorType.User, Operator = "wangli5" };

            row2.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1002" });
            row2.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "2" });
            row2.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "40" });

            SOARolePropertyRow row3 = new SOARolePropertyRow() { RowNumber = 3, OperatorType = SOARoleOperatorType.Role, Operator = RolesDefineConfig.GetConfig().RolesDefineCollection["nestedRole"].Roles };

            row3.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1002" });
            row3.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "2" });
            row3.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "60" });

            rows.Add(row1);
            rows.Add(row2);
            rows.Add(row3);

            return rows;
        }

        private static void ProcessContextAction()
        {
            WfRuntime.ProcessContext.LeaveActivityPrepareAction += new WfActionHandler(ProcessContext_LeaveActivityPrepareAction);
            WfRuntime.ProcessContext.EnterActivityPrepareAction += new WfActionHandler(ProcessContext_EnterActivityPrepareAction);
        }

        private static void ProcessContext_EnterActivityPrepareAction()
        {
            /*
            if (WfRuntime.ProcessContext.CurrentActivity != null)
                Console.WriteLine("Enter Activity: {0}", WfRuntime.ProcessContext.CurrentActivity.Descriptor.Key);
             */
        }

        private static void ProcessContext_LeaveActivityPrepareAction()
        {
            /*
            if (WfRuntime.ProcessContext.OriginalActivity != null)
                Console.WriteLine("Leave Activity: {0}", WfRuntime.ProcessContext.OriginalActivity.Descriptor.Key);
             */
        }


        /// <summary>
        /// 找出两文本中某一处不同的区域
        /// </summary>
        /// <param name="s1">字符串1</param>
        /// <param name="s2">字符串2</param>
        /// <returns></returns>
        public static List<string> FindStrEqual(string s1, string s2)
        {
            //间断空
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2)) return new List<string>();

            if (string.Compare(s1, s2) == 0) return new List<string>();


            int s = 0;
            int e = s1.Length / 2;
            List<string> strings = new List<string>();
            LockStrArea(s1, s2, s, e, out strings, false);

            return strings;
        }

        /// <summary>
        /// 两文本比较,取出不相等的区域
        /// </summary>
        /// <param name="s1">字符串</param>
        /// <param name="s2">被比较字符串</param>
        /// <param name="s">起始点位置</param>
        /// <param name="e">结束点位置</param>
        /// <param name="list">返回两字符串不相等的列表</param>
        /// <param name="f">标识值</param>
        /// zling 2011.1.24
        private static void LockStrArea(string s1, string s2, int s, int e, out List<string> list, bool f)
        {
            string sub1 = s1.ToString().Substring(s, e);
            string sub2 = s2.ToString().Substring(s, e);

            if (string.Compare(sub1, sub2) == 0)
            {
                sub1 = s1.ToString().Substring(e);
                sub2 = s2.ToString().Substring(e);

                if (f)
                {
                    List<string> strings = new List<string>();
                    strings.Add(sub1);
                    strings.Add(sub2);

                    list = strings;
                }
                else
                {
                    LockStrArea(sub1, sub2, s, sub1.Length / 2, out list, f);
                }
            }
            else
            {
                f = true;
                LockStrArea(sub1, sub2, s, sub1.Length / 2, out list, f);
            }

        }
    }
}
