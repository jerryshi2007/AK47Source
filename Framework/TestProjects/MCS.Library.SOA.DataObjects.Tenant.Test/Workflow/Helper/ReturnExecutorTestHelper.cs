using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.Workflow.Helper
{
    public static class ReturnExecutorTestHelper
    {
        /// <summary>
        /// 准备专用退件流程，据有A、B、C、D、E5个活动。F为结束点
        /// B有BC和BE两条出线，IsCLine为True走BC，否则走BE。
        /// 然后流转到活动D
        /// </summary>
        /// <returns></returns>
        public static IWfActivity PrepareAndMoveToSpecialActivity()
        {
            IWfProcessDescriptor processDesp = ReturnExecutorTestHelper.PrepareSpecialReturnProcessDesp();

            IWfProcess process = ReturnExecutorTestHelper.StartSpecialReturnProcess(processDesp);

            process.MoveToNextDefaultActivity();	//To B
            process.MoveToNextDefaultActivity();	//To C
            process.MoveToNextDefaultActivity();	//To D

            return process.CurrentActivity;
        }

        /// <summary>
        /// 准备同意/不同意流程，流转到到选择同意/不同意的活动（B）
        /// </summary>
        /// <returns></returns>
        public static IWfActivity PrepareAndMoveToAgreeSelectorActivity()
        {
            //准备同意/不同意的退回流程。在B环节有两根出线BC和BA，BA是退回线，退回到A。其中BA默认是没有选择的
            IWfProcessDescriptor processDesp = ReturnExecutorTestHelper.PrepareAgreeReturnProcessDesp();

            IWfProcess process = ReturnExecutorTestHelper.StartSpecialReturnProcess(processDesp);

            process.MoveToNextDefaultActivity();	//To B

            return process.CurrentActivity;
        }

        #region Prepare Descriptor
        /// <summary>
        /// 准备专用退件流程，B有BC和BE两条出线，IsCLine为True走BC，否则走BE
        /// </summary>
        /// <returns></returns>
        public static IWfProcessDescriptor PrepareSpecialReturnProcessDesp()
        {
            WfProcessDescriptor processDesp = new WfProcessDescriptor();

            processDesp.Key = "TestProcess" + UuidHelper.NewUuidString().Substring(0, 8);
            processDesp.Name = "专用退回流程";
            processDesp.ApplicationName = "TEST_APP_NAME";
            processDesp.ProgramName = "TEST_PROGRAM_NAME";
            processDesp.Url = "/MCS_Framework/WebTestProject/defaultHandler.aspx";

            WfActivityDescriptor initActivity = new WfActivityDescriptor("A", WfActivityType.InitialActivity);
            initActivity.Name = "A";
            initActivity.CodeName = "A";
            initActivity.Resources.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object));

            processDesp.Activities.Add(initActivity);

            IWfActivityDescriptor activityB = CreateNormalDescriptor("B", "B");

            activityB.Resources.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object));
            processDesp.Activities.Add(activityB);

            IWfActivityDescriptor activityC = CreateNormalDescriptor("C", "C");
            processDesp.Activities.Add(activityC);

            IWfActivityDescriptor activityE = CreateNormalDescriptor("E", "E");
            processDesp.Activities.Add(activityE);

            IWfActivityDescriptor activityD = CreateNormalDescriptor("D", "D");
            processDesp.Activities.Add(activityD);

            WfActivityDescriptor completedActivity = new WfActivityDescriptor("F", WfActivityType.CompletedActivity);
            completedActivity.Name = "F";
            completedActivity.CodeName = "F";

            processDesp.Activities.Add(completedActivity);

            //A到B
            initActivity.ToTransitions.AddForwardTransition(activityB);

            //B有两根出线，分别是C和E
            WfTransitionDescriptor transitionBC = (WfTransitionDescriptor)activityB.ToTransitions.AddForwardTransition(activityC);
            transitionBC.Condition = new WfConditionDescriptor(transitionBC) { Expression = "IsCLine" };

            WfTransitionDescriptor transitionBE = (WfTransitionDescriptor)activityB.ToTransitions.AddForwardTransition(activityE);
            transitionBE.Condition = new WfConditionDescriptor(transitionBE) { Expression = "!IsCLine" };

            //C和E都汇聚到D
            activityC.ToTransitions.AddForwardTransition(activityD);
            activityE.ToTransitions.AddForwardTransition(activityD);

            //D到结束点
            activityD.ToTransitions.AddForwardTransition(completedActivity);

            return processDesp;
        }

        /// <summary>
        /// 准备加签退件流程
        /// </summary>
        /// <returns></returns>
        public static IWfProcessDescriptor PrepareAddApproverReturnProcessDesp()
        {
            WfProcessDescriptor processDesp = new WfProcessDescriptor();

            processDesp.Key = "TestProcess" + UuidHelper.NewUuidString().Substring(0, 8);
            processDesp.Name = "加签退回流程";
            processDesp.ApplicationName = "TEST_APP_NAME";
            processDesp.ProgramName = "TEST_PROGRAM_NAME";
            processDesp.Url = "/MCS_Framework/WebTestProject/defaultHandler.aspx";

            WfActivityDescriptor initActivity = new WfActivityDescriptor("A", WfActivityType.InitialActivity);
            initActivity.Name = "A";
            initActivity.CodeName = "A";

            processDesp.Activities.Add(initActivity);

            IWfActivityDescriptor activityB = CreateNormalDescriptor("B", "B");

            processDesp.Activities.Add(activityB);

            IWfActivityDescriptor activityC = CreateNormalDescriptor("C", "C");
            processDesp.Activities.Add(activityC);

            WfActivityDescriptor completedActivity = new WfActivityDescriptor("D", WfActivityType.CompletedActivity);
            completedActivity.Name = "D";
            completedActivity.CodeName = "D";

            processDesp.Activities.Add(completedActivity);

            //A到B
            initActivity.ToTransitions.AddForwardTransition(activityB);

            //B到C
            activityB.ToTransitions.AddForwardTransition(activityC);

            //C到结束点
            activityC.ToTransitions.AddForwardTransition(completedActivity);

            return processDesp;
        }

        /// <summary>
        /// 准备同意/不同意的退回流程。在B环节有两根出线BC和BA，BA是退回线，退回到A。其中BA默认是没有选择的
        /// </summary>
        /// <returns></returns>
        public static IWfProcessDescriptor PrepareAgreeReturnProcessDesp()
        {
            WfProcessDescriptor processDesp = new WfProcessDescriptor();

            processDesp.Key = "TestProcess" + UuidHelper.NewUuidString().Substring(0, 8);
            processDesp.Name = "专用退回流程";
            processDesp.ApplicationName = "TEST_APP_NAME";
            processDesp.ProgramName = "TEST_PROGRAM_NAME";
            processDesp.Url = "/MCS_Framework/WebTestProject/defaultHandler.aspx";

            WfActivityDescriptor initActivity = new WfActivityDescriptor("A", WfActivityType.InitialActivity);
            initActivity.Name = "A";
            initActivity.CodeName = "A";

            processDesp.Activities.Add(initActivity);

            IWfActivityDescriptor activityB = CreateNormalDescriptor("B", "B");

            processDesp.Activities.Add(activityB);

            IWfActivityDescriptor activityC = CreateNormalDescriptor("C", "C");
            processDesp.Activities.Add(activityC);

            IWfActivityDescriptor activityD = CreateNormalDescriptor("D", "D");
            processDesp.Activities.Add(activityD);

            WfActivityDescriptor completedActivity = new WfActivityDescriptor("F", WfActivityType.CompletedActivity);
            completedActivity.Name = "F";
            completedActivity.CodeName = "F";

            processDesp.Activities.Add(completedActivity);

            //A->B
            initActivity.ToTransitions.AddForwardTransition(activityB);

            //B有两根出线，分别是C和A，A是退回线
            WfTransitionDescriptor transitionBC = (WfTransitionDescriptor)activityB.ToTransitions.AddForwardTransition(activityC);
            transitionBC.Enabled = true;
            transitionBC.DefaultSelect = true;
            WfTransitionDescriptor transitionBA = (WfTransitionDescriptor)activityB.ToTransitions.AddBackwardTransition(initActivity);
            //transitionBA.Enabled = false;
            transitionBA.Enabled = true;
            transitionBC.DefaultSelect = false;
            transitionBA.IsBackward = true;

            //C->D
            activityC.ToTransitions.AddForwardTransition(activityD);

            //D到结束点
            activityD.ToTransitions.AddForwardTransition(completedActivity);

            return processDesp;
        }

        /// <summary>
        /// 生成用于复制退件活动的测试流程
        /// A为起点、两条分支，A、B、C、D和A、E、D，D为终点
        /// </summary>
        /// <returns></returns>
        public static IWfProcessDescriptor PrepareCopyTestProcessDesp()
        {
            WfProcessDescriptor processDesp = new WfProcessDescriptor();

            processDesp.Key = "TestProcess" + UuidHelper.NewUuidString().Substring(0, 8);
            processDesp.Name = "专用退回流程";
            processDesp.ApplicationName = "TEST_APP_NAME";
            processDesp.ProgramName = "TEST_PROGRAM_NAME";
            processDesp.Url = "/MCS_Framework/WebTestProject/defaultHandler.aspx";

            WfActivityDescriptor initActivity = new WfActivityDescriptor("A", WfActivityType.InitialActivity);
            initActivity.Name = "A";
            initActivity.CodeName = "A";

            processDesp.Activities.Add(initActivity);

            IWfActivityDescriptor activityB = CreateNormalDescriptor("B", "B");

            processDesp.Activities.Add(activityB);

            IWfActivityDescriptor activityC = CreateNormalDescriptor("C", "C");
            processDesp.Activities.Add(activityC);

            IWfActivityDescriptor activityE = CreateNormalDescriptor("E", "E");
            processDesp.Activities.Add(activityE);

            WfActivityDescriptor completedActivity = new WfActivityDescriptor("D", WfActivityType.CompletedActivity);
            completedActivity.Name = "D";
            completedActivity.CodeName = "D";

            processDesp.Activities.Add(completedActivity);

            //A到B
            WfTransitionDescriptor transitionAB = (WfTransitionDescriptor)initActivity.ToTransitions.AddForwardTransition(activityB);
            transitionAB.Enabled = true;

            //A到E
            WfTransitionDescriptor transitionAE = (WfTransitionDescriptor)initActivity.ToTransitions.AddBackwardTransition(activityE);
            transitionAE.Enabled = false;

            //B到C
            activityB.ToTransitions.AddForwardTransition(activityC);

            //E到C
            activityE.ToTransitions.AddForwardTransition(activityC);

            //C到结束点
            activityC.ToTransitions.AddForwardTransition(completedActivity);

            return processDesp;
        }

        /// <summary>
        /// 生成用于复制退件活动的测试流程
        /// A为起点、两条分支，A、B、C、D和A、E、D。
        /// D为终点。B和A之间有一条退回线
        /// </summary>
        /// <returns></returns>
        public static IWfProcessDescriptor PrepareCopyTestProcessWithReturnLineDesp()
        {
            IWfProcessDescriptor processDesp = PrepareCopyTestProcessDesp();

            IWfActivityDescriptor activityB = processDesp.Activities["B"];

            WfTransitionDescriptor transition = (WfTransitionDescriptor)activityB.ToTransitions.AddForwardTransition(processDesp.InitialActivity);

            transition.IsBackward = true;

            return processDesp;
        }

        /// <summary>
        /// 准备一条简单的直线流程，主要用于两次退回等场景
        /// 流程为A->B->C->D
        /// </summary>
        /// <returns></returns>
        public static IWfProcessDescriptor PrepareStraightProcessDesp()
        {
            WfProcessDescriptor processDesp = new WfProcessDescriptor();

            processDesp.Key = "TestProcess" + UuidHelper.NewUuidString().Substring(0, 8);
            processDesp.Name = "简单直线流程";
            processDesp.ApplicationName = "TEST_APP_NAME";
            processDesp.ProgramName = "TEST_PROGRAM_NAME";
            processDesp.Url = "/MCS_Framework/WebTestProject/defaultHandler.aspx";

            WfActivityDescriptor initActivity = new WfActivityDescriptor("A", WfActivityType.InitialActivity);
            initActivity.Name = "A";
            initActivity.CodeName = "A";
            initActivity.Resources.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object));

            processDesp.Activities.Add(initActivity);

            IWfActivityDescriptor activityB = CreateNormalDescriptor("B", "B");

            processDesp.Activities.Add(activityB);
            activityB.Resources.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object));

            IWfActivityDescriptor activityC = CreateNormalDescriptor("C", "C");
            processDesp.Activities.Add(activityC);

            WfActivityDescriptor completedActivity = new WfActivityDescriptor("D", WfActivityType.CompletedActivity);
            completedActivity.Name = "D";
            completedActivity.CodeName = "D";

            processDesp.Activities.Add(completedActivity);

            //A到B
            WfTransitionDescriptor transitionAB = (WfTransitionDescriptor)initActivity.ToTransitions.AddForwardTransition(activityB);
            transitionAB.Enabled = true;


            //B到C
            activityB.ToTransitions.AddForwardTransition(activityC);

            //C到结束点
            activityC.ToTransitions.AddForwardTransition(completedActivity);

            return processDesp;
        }
        #endregion Prepare Descriptor

        public static IWfProcess StartSpecialReturnProcess(IWfProcessDescriptor processDesp)
        {
            WfProcessStartupParams startupParams = new WfProcessStartupParams();
            startupParams.ProcessDescriptor = processDesp;
            startupParams.ApplicationRuntimeParameters["IsCLine"] = true;

            return WfRuntime.StartWorkflow(startupParams);
        }

        private static IWfActivityDescriptor CreateNormalDescriptor(string key, string name)
        {
            WfActivityDescriptor activityDesp = new WfActivityDescriptor(key, WfActivityType.NormalActivity);

            activityDesp.Name = name;
            activityDesp.CodeName = key;

            return activityDesp;
        }

        #region Validation
        /// <summary>
        /// 验证B的衍生点的出线是否是两根
        /// </summary>
        public static void ValidateBRelativeActivityOutTransitions(IWfActivity currentActivity)
        {
            IWfActivity relativeBActivity = FindRelativeActivityByKey(currentActivity, "B");

            Assert.IsNotNull(relativeBActivity, string.Format("不能在{0}后找到B的衍生活动", currentActivity.Descriptor.Key));

            //衍生线也是两根
            WfTransitionDescriptorCollection transitions = relativeBActivity.Descriptor.ToTransitions;

            Assert.AreEqual(2, transitions.Count);
            Assert.IsTrue(transitions.Exists(t => t.ToActivity.AssociatedActivityKey == "C"));
            Assert.IsTrue(transitions.Exists(t => t.ToActivity.AssociatedActivityKey == "E"));

            transitions = relativeBActivity.Descriptor.ToTransitions.GetAllCanTransitForwardTransitions();

            Assert.AreEqual(1, transitions.Count);
            Assert.IsTrue(transitions.Exists(t => t.ToActivity.AssociatedActivityKey == "C"));
        }
        #endregion Validation

        #region Helper
        public static IWfProcess ExecuteReturnOperation(IWfActivity currentActivity, string targetKey)
        {
            IWfActivity targetActivity = currentActivity.Process.Activities.FindActivityByDescriptorKey(targetKey);
            WfReturnExecutor executor = new WfReturnExecutor(currentActivity, targetActivity);

            IWfProcess process = executor.ExecuteNotPersist();

            return WfRuntime.GetProcessByProcessID(process.ID);
        }

        private static IWfActivity FindRelativeActivityByKey(IWfActivity currentActivity, string key)
        {
            IWfActivity result = null;

            IWfActivityDescriptor actDesp = FindAssociatedActivityByKey(currentActivity.Descriptor, key);

            if (actDesp != null)
                result = actDesp.Instance;

            return result;
        }

        private static IWfActivityDescriptor FindAssociatedActivityByKey(IWfActivityDescriptor actDesp, string key)
        {
            IWfActivityDescriptor result = null;

            if (actDesp.AssociatedActivityKey == key)
                result = actDesp;
            else
            {
                WfTransitionDescriptorCollection transitions = actDesp.ToTransitions.GetAllCanTransitForwardTransitions();

                foreach (IWfTransitionDescriptor transition in transitions)
                {
                    result = FindAssociatedActivityByKey(transition.ToActivity, key);

                    if (result != null)
                        break;
                }
            }

            return result;
        }
        #endregion Helper
    }
}
