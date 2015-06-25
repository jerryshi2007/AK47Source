using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects.Workflow.Builders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.Workflow.Helper
{
    public static class ProcessHelper
    {
        public static IWfActivityDescriptor CreateNormalActivity(string key)
        {
            WfActivityDescriptor normalAct = new WfActivityDescriptor(key, WfActivityType.NormalActivity);
            normalAct.Name = key;
            normalAct.CodeName = key;

            return normalAct;
        }

        /// <summary>
        /// 创建一个简单的流程定义
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

        public static IWfProcessDescriptor CreateInitAndCompletedProcessDescriptor()
        {
            return CreateFreeStepsProcessDescriptor(new IUser[] { });
        }

        public static IWfProcessDescriptor CreateFreeStepsProcessDescriptor(params IUser[] approvers)
        {
            string processKey = UuidHelper.NewUuidString();

            WfFreeStepsProcessBuilder builder = new WfFreeStepsProcessBuilder(
                Define.DefaultApplicationName,
                Define.DefaultProgramName,
                approvers);

            return builder.Build(processKey, "测试流程");
        }

        /// <summary>
        /// 创建一个带动态活动，且内嵌矩阵的流程
        /// </summary>
        /// <param name="externalMatrixID"></param>
        /// <returns></returns>
        public static IWfProcessDescriptor GetDynamicProcessDesp(string externalMatrixID = null)
        {
            return GetDynamicProcessDesp(ActivityMatrixHelper.PrepareActivityMatrixResourceDescriptor(), externalMatrixID);
        }

        public static IWfProcessDescriptor GetDynamicProcessDesp(WfActivityMatrixResourceDescriptor activityMatrixResource, string externalMatrixID = null)
        {
            IWfProcessDescriptor processDesp = CreateSimpleProcessDescriptor();

            IWfActivityDescriptor normalActDesp = processDesp.Activities["NormalActivity"];

            normalActDesp.Properties.SetValue("IsDynamic", true);

            WfActivityMatrixResourceDescriptor resource = activityMatrixResource;

            normalActDesp.Resources.Add(resource);

            resource.ExternalMatrixID = externalMatrixID;

            return processDesp;
        }

        /// <summary>
        /// 启动流程
        /// </summary>
        /// <param name="processDesp"></param>
        /// <returns></returns>
        public static IWfProcess StartupProcess(this IWfProcessDescriptor processDesp)
        {
            return StartupProcess(processDesp, new Dictionary<string, object>());
        }

        /// <summary>
        /// 带上下文参数的启动流程
        /// </summary>
        /// <param name="processDesp"></param>
        /// <returns></returns>
        public static IWfProcess StartupProcessByExecutor(this IWfProcessDescriptor processDesp, Dictionary<string, object> runtimeParameters = null)
        {
            WfProcessStartupParams startupParams = processDesp.PrepareStartupParams(runtimeParameters);

            WfStartWorkflowExecutor executor = new WfStartWorkflowExecutor(startupParams);

            return executor.Execute();
        }

        /// <summary>
        /// 带上下文参数的启动流程
        /// </summary>
        /// <param name="processDesp"></param>
        /// <param name="runtimeParameters"></param>
        /// <returns></returns>
        public static IWfProcess StartupProcess(this IWfProcessDescriptor processDesp, Dictionary<string, object> runtimeParameters = null)
        {
            WfProcessStartupParams startupParams = PrepareStartupParams(processDesp, runtimeParameters);

            return WfRuntime.StartWorkflow(startupParams);
        }

        public static WfProcessStartupParams PrepareStartupParams(this IWfProcessDescriptor processDesp, Dictionary<string, object> runtimeParameters = null)
        {
            WfProcessStartupParams startupParams = new WfProcessStartupParams();
            startupParams.ResourceID = UuidHelper.NewUuidString();
            startupParams.ProcessDescriptor = processDesp;
            startupParams.Assignees.Add(OguObjectSettings.GetConfig().Objects["admin"].User);

            if (runtimeParameters != null)
                runtimeParameters.ForEach(kp => startupParams.ApplicationRuntimeParameters.Add(kp.Key, kp.Value));

            return startupParams;
        }

        public static IWfProcess MoveToDefaultActivityByExecutor(this IWfProcess process, bool persist = true)
        {
            WfTransferParams transferParams = WfTransferParams.FromNextDefaultActivity(process);

            WfMoveToExecutor executor = new WfMoveToExecutor(process.CurrentActivity, process.CurrentActivity, transferParams);

            IWfProcess result = process;

            if (persist)
            {
                executor.Execute();
                result = WfRuntime.GetProcessByProcessID(process.ID);
            }
            else
                executor.ExecuteNotPersist();

            return result;
        }

        public static IWfProcess MoveToReturnActivityByExecutor(this IWfProcess process, bool persist = true)
        {
            return MoveToNextActivityByExecutor(process, (toTransitions) => toTransitions.Find(t => t.IsBackward), persist);
        }

        public static IWfProcess MoveToNextActivityByExecutor(this IWfProcess process, Func<WfTransitionDescriptorCollection, IWfTransitionDescriptor> predicate, bool persist = true)
        {
            WfTransferParams transferParams = WfTransferParams.FromNextActivity(process.CurrentActivity.Descriptor, predicate);

            WfMoveToExecutor executor = new WfMoveToExecutor(process.CurrentActivity, process.CurrentActivity, transferParams);

            IWfProcess result = process;

            if (persist)
            {
                executor.Execute();
                result = WfRuntime.GetProcessByProcessID(process.ID);
            }
            else
                executor.ExecuteNotPersist();

            return result;
        }

        public static IWfProcess WithdrawByExecutor(this IWfProcess process, bool cancelProcess = false)
        {
            WfWithdrawExecutor executor = new WfWithdrawExecutor(process.CurrentActivity, process.CurrentActivity, true, cancelProcess);

            executor.Execute();

            return WfRuntime.GetProcessByProcessID(process.ID); ;
        }

        public static IWfProcess CancelByExecutor(this IWfProcess process)
        {
            WfCancelProcessExecutor executor = new WfCancelProcessExecutor(process.CurrentActivity, process);

            executor.Execute();

            return process;
        }

        public static void OutputExecutionTime(this Action action, string description)
        {
            Console.WriteLine("执行操作: {0}", description);

            DateTime startTime = DateTime.Now;

            Console.WriteLine("开始时间: {0:yyyy-MM-dd HH:mm:ss}", startTime);
            action();

            DateTime endTime = DateTime.Now;
            Console.WriteLine("结束时间: {0:yyyy-MM-dd HH:mm:ss}", endTime);
            Console.WriteLine("经过时间: {0:#,##0.00}ms", (endTime - startTime).TotalMilliseconds);
        }

        /// <summary>
        /// 验证主活动点的Key
        /// </summary>
        /// <param name="process"></param>
        /// <param name="expectedActKeys"></param>
        public static void ValidateMainStreamActivities(this IWfProcess process, params string[] expectedActKeys)
        {
            ValidateMainStreamActivities(process, false, expectedActKeys);
        }

        /// <summary>
        /// 验证主活动点的Key
        /// </summary>
        /// <param name="activityKeys"></param>
        public static void ValidateMainStreamActivities(this IWfProcess process, bool includeAllElapsedActivities, params string[] expectedActKeys)
        {
            WfMainStreamActivityDescriptorCollection mainStreamActivities = process.GetMainStreamActivities(includeAllElapsedActivities);

            List<string> mainStreamKeys = new List<string>();

            foreach (WfMainStreamActivityDescriptor msActDesp in mainStreamActivities)
            {
                string actKey = msActDesp.Activity.Instance.MainStreamActivityKey;

                mainStreamKeys.Add(actKey);
            }

            Assert.AreEqual(expectedActKeys.Length, mainStreamActivities.Count, "主活动点和预期的个数不符");

            for (int i = 0; i < expectedActKeys.Length; i++)
                Assert.AreEqual(expectedActKeys[i], mainStreamKeys[i], string.Format("第{0}个主活动的Key不一致", i));
        }
    }
}
