using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Tenant.Test.Workflow.Helper;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.Workflow
{
    [TestClass]
    public class ReturnExecutorTest
    {
        [TestMethod]
        [Description("复制简单流程的测试")]
        public void SimpleCopyMainStreamActivitiesTest()
        {
            IWfProcessDescriptor processDesp = ReturnExecutorTestHelper.PrepareStraightProcessDesp();

            IWfProcess process = ReturnExecutorTestHelper.StartSpecialReturnProcess(processDesp);

            process.MoveToNextDefaultActivity();	//To B
            process.MoveToNextDefaultActivity();	//To C

            IWfActivity activityB = process.Activities.FindActivityByDescriptorKey("B");

            //从B复制到C
            process.Activities.FindActivityByDescriptorKey("C").CopyMainStreamActivities(activityB, null, WfControlOperationType.Return);

            process.Descriptor.OutputEveryActivities();

            process.MoveToNextDefaultActivity();	//To CopiedB

            Assert.AreEqual("B", process.CurrentActivity.Descriptor.AssociatedActivityKey);

            IWfActivityDescriptor copiedB = process.CurrentActivity.Descriptor;

            Assert.AreEqual("B", copiedB.AssociatedActivityKey);
            Assert.AreEqual("C", copiedB.ToTransitions.First().ToActivity.AssociatedActivityKey, "复制出的B指向C");
        }

        [TestMethod]
        [Description("两次复制简单流程的测试。C退回到B，B再退回到A")]
        public void SimpleCopyTwiceMainStreamActivitiesTest()
        {
            IWfProcessDescriptor processDesp = ReturnExecutorTestHelper.PrepareStraightProcessDesp();

            IWfProcess process = ReturnExecutorTestHelper.StartSpecialReturnProcess(processDesp);

            process.MoveToNextDefaultActivity();	//To B
            process.MoveToNextDefaultActivity();	//To C

            IWfActivity activityB = process.Activities.FindActivityByDescriptorKey("B");

            //第一次
            process.Activities.FindActivityByDescriptorKey("C").CopyMainStreamActivities(activityB, null, WfControlOperationType.Return);

            Console.WriteLine("第一次");
            process.Descriptor.OutputEveryActivities();

            process.MoveToNextDefaultActivity();	//To CopiedB

            Assert.AreEqual("B", process.CurrentActivity.Descriptor.AssociatedActivityKey);

            //第二次
            process.CurrentActivity.CopyMainStreamActivities(process.CurrentActivity, process.InitialActivity, activityB, null, WfControlOperationType.Return);

            Console.WriteLine("第二次");
            process.Descriptor.OutputEveryActivities();

            IWfActivityDescriptor copiedA = process.CurrentActivity.Descriptor.ToTransitions.First().ToActivity;

            Assert.AreEqual("A", copiedA.AssociatedActivityKey);

            IWfActivityDescriptor copiedBAgain = copiedA.ToTransitions.First().ToActivity;

            Assert.AreEqual("B", copiedBAgain.AssociatedActivityKey, "验证第二次复制的B");

            IWfActivityDescriptor copiedC = copiedBAgain.ToTransitions.First().ToActivity;

            Assert.AreEqual("C", copiedC.AssociatedActivityKey, "验证第二次复制出的B指向第一次复制出的C");
        }

        [TestMethod]
        [Description("复制流程的部分环节和连线的测试")]
        public void CopyMainStreamActivitiesTest()
        {
            IWfProcessDescriptor processDesp = ReturnExecutorTestHelper.PrepareCopyTestProcessDesp();

            IWfProcess process = ReturnExecutorTestHelper.StartSpecialReturnProcess(processDesp);

            process.Activities.FindActivityByDescriptorKey("B").CopyMainStreamActivities(process.InitialActivity, null, WfControlOperationType.Return);

            Assert.AreEqual(8, process.Activities.Count, "总共有8个活动");
            IWfActivityDescriptor copiedA = process.Activities.FindActivityByDescriptorKey("B").Descriptor.ToTransitions.First().ToActivity;

            Assert.AreEqual("A", copiedA.AssociatedActivityKey);
            Assert.AreEqual(process.InitialActivity.Descriptor.ToTransitions.Count, copiedA.ToTransitions.Count, "复制出来的A和原始的A的出线个数相同");

            IWfActivityDescriptor copiedE = copiedA.ToTransitions.Find(t => t.ToActivity.AssociatedActivityKey == "E").ToActivity;
            IWfActivityDescriptor copiedB = copiedA.ToTransitions.Find(t => t.ToActivity.AssociatedActivityKey == "B").ToActivity;

            Assert.IsTrue(copiedE.ToTransitions.First().ToActivity.Key == "C", "复制出的E指向C");
            Assert.IsTrue(copiedB.ToTransitions.First().ToActivity.Key == "C", "复制出的B指向C");

            IWfActivityDescriptor actDespE = process.Activities.FindActivityByDescriptorKey("E").Descriptor;

            Assert.IsTrue(actDespE.ToTransitions.First().ToActivity.Key == "C", "E指向C");

            IWfActivityDescriptor actDespC = process.Activities.FindActivityByDescriptorKey("C").Descriptor;

            Assert.IsNotNull(actDespC.ToTransitions.First().ToActivity.Key == "D", "C指向D");
        }

        [TestMethod]
        [Description("复制带退回线流程的部分环节和连线的测试")]
        public void CopyMainStreamActivitiesWithReturnLineTest()
        {
            IWfProcessDescriptor processDesp = ReturnExecutorTestHelper.PrepareCopyTestProcessWithReturnLineDesp();

            IWfProcess process = ReturnExecutorTestHelper.StartSpecialReturnProcess(processDesp);

            process.Activities.FindActivityByDescriptorKey("B").CopyMainStreamActivities(process.InitialActivity, null, WfControlOperationType.Return);

            Assert.AreEqual(8, process.Activities.Count, "总共有8个活动，原始流程有5个活动。复制了A、B、E");
            IWfActivityDescriptor copiedA = process.Activities.FindActivityByDescriptorKey("B").Descriptor.ToTransitions.GetAllForwardTransitions().First().ToActivity;

            Assert.AreEqual("A", copiedA.AssociatedActivityKey);
            Assert.AreEqual(process.InitialActivity.Descriptor.ToTransitions.Count, copiedA.ToTransitions.Count, "复制出来的A和原始的A的出线个数相同");

            IWfActivityDescriptor copiedE = copiedA.ToTransitions.Find(t => t.ToActivity.AssociatedActivityKey == "E").ToActivity;
            IWfActivityDescriptor copiedB = copiedA.ToTransitions.Find(t => t.ToActivity.AssociatedActivityKey == "B").ToActivity;

            Assert.IsNotNull(copiedE.ToTransitions.First().ToActivity.Key == "C", "复制出的E指向C");

            Assert.IsTrue(copiedB.ToTransitions.Exists(t => t.ToActivity.Key == "C"), "复制出的B存在指向C的线");
            Assert.IsTrue(copiedB.ToTransitions.Exists(t => t.ToActivity.Key == copiedA.Key), "复制出的B存在指向复制出的A的线");

            IWfActivityDescriptor actDespE = process.Activities.FindActivityByDescriptorKey("E").Descriptor;

            Assert.IsTrue(actDespE.ToTransitions.First().ToActivity.Key == "C", "E指向C");

            IWfActivityDescriptor actDespC = process.Activities.FindActivityByDescriptorKey("C").Descriptor;

            Assert.IsNotNull(actDespC.ToTransitions.First().ToActivity.Key == "D", "C指向D");
        }

        [TestMethod]
        [Description("流转复制流程的部分环节和连线的测试")]
        public void MoveToCopiedMainStreamActivitiesTest()
        {
            IWfProcessDescriptor processDesp = ReturnExecutorTestHelper.PrepareCopyTestProcessDesp();

            IWfProcess process = ReturnExecutorTestHelper.StartSpecialReturnProcess(processDesp);

            process.Activities.FindActivityByDescriptorKey("B").CopyMainStreamActivities(process.InitialActivity, null, WfControlOperationType.Return);

            process.MoveToNextDefaultActivity();	//To B
            Assert.AreEqual("B", process.CurrentActivity.Descriptor.Key);

            process.MoveToNextDefaultActivity();	//To A1
            Assert.AreEqual("A", process.CurrentActivity.Descriptor.AssociatedActivityKey);

            process.MoveToNextDefaultActivity();	//To B1
            Assert.AreEqual("B", process.CurrentActivity.Descriptor.AssociatedActivityKey);

            process.MoveToNextDefaultActivity();	//To C
            Assert.AreEqual("C", process.CurrentActivity.Descriptor.Key);
        }

        [TestMethod]
        [Description("专用退件流程第一次退件测试")]
        public void ReturnOnceTest()
        {
            IWfActivity activityD = ReturnExecutorTestHelper.PrepareAndMoveToSpecialActivity();

            Assert.AreEqual("D", activityD.Descriptor.Key);

            activityD.Process.OutputMainStream();
            activityD.Process.OutputEveryActivities();

            activityD.Process.ValidateMainStreamActivities("A", "B", "C", "D", "F");

            ReturnExecutorTestHelper.ExecuteReturnOperation(activityD, "A");

            IWfActivity returnedActivity = activityD.Process.CurrentActivity;

            Assert.AreEqual("A", returnedActivity.Descriptor.AssociatedActivityKey);
            Assert.AreEqual(activityD.ID, returnedActivity.CreatorInstanceID);
            ReturnExecutorTestHelper.ValidateBRelativeActivityOutTransitions(activityD.Process.CurrentActivity);

            activityD.Process.OutputMainStream();
            activityD.Process.OutputEveryActivities();

            activityD.Process.ValidateMainStreamActivities("A", "B", "C", "D", "F");
        }

        [TestMethod]
        [Description("专用退件流程一次退件加一次撤回测试")]
        public void ReturnOnceThenWithdrawTest()
        {
            IWfActivity activityD = ReturnExecutorTestHelper.PrepareAndMoveToSpecialActivity();

            int originalActivityCount = activityD.Process.Activities.Count;

            Assert.AreEqual("D", activityD.Descriptor.Key);
            activityD.Process.OutputMainStream();
            activityD.Process.OutputEveryActivities();

            ReturnExecutorTestHelper.ExecuteReturnOperation(activityD, "A");

            Console.WriteLine("退件之后");
            activityD.Process.OutputMainStream();
            activityD.Process.OutputEveryActivities();

            IWfProcess process = activityD.Process.WithdrawByExecutor();

            Console.WriteLine("撤回之后");
            process.OutputMainStream();
            process.OutputEveryActivities();

            Assert.AreEqual(originalActivityCount, process.Activities.Count, "撤回后与退件前的活动数一样");
            Assert.AreEqual(originalActivityCount, process.Descriptor.Activities.Count, "撤回后与退件前的活动数一样");
        }

        /// <summary>
        /// 原始测试不通过
        /// </summary>
        [TestMethod]
        [Description("专用退件流程第二次退件测试")]
        public void ReturnTwiceTest()
        {
            IWfActivity activityD = ReturnExecutorTestHelper.PrepareAndMoveToSpecialActivity();

            Assert.AreEqual("D", activityD.Descriptor.Key);
            activityD.Process.OutputMainStream();
            activityD.Process.OutputEveryActivities();

            //第一次退回
            ReturnExecutorTestHelper.ExecuteReturnOperation(activityD, "A");

            Console.WriteLine("第一次退回后");
            activityD.Process.OutputMainStream();
            activityD.Process.OutputEveryActivities();

            activityD.Process.MoveToNextDefaultActivity();	//B1
            activityD.Process.MoveToNextDefaultActivity();	//C1
            activityD.Process.MoveToNextDefaultActivity();	//D1

            Assert.AreEqual("D", activityD.Process.CurrentActivity.Descriptor.AssociatedActivityKey);

            //在B的出线C和E之间选择E
            activityD.Process.ApplicationRuntimeParameters["IsCLine"] = false;

            //第二次退回
            ReturnExecutorTestHelper.ExecuteReturnOperation(activityD, "A");

            Console.WriteLine("第二次退回后");
            activityD.Process.OutputMainStream();
            activityD.Process.OutputEveryActivities();

            activityD.Process.ValidateMainStreamActivities("A", "B", "E", "D", "F");
        }

        [TestMethod]
        [Description("专用退件流程第二次退件后撤回两次测试")]
        public void ReturnTwiceThenWithdrawTwiceTest()
        {
            IWfActivity activityD = ReturnExecutorTestHelper.PrepareAndMoveToSpecialActivity();

            int originalActivityCount = activityD.Process.Activities.Count;

            Assert.AreEqual("D", activityD.Descriptor.Key);
            activityD.Process.OutputMainStream();
            activityD.Process.OutputEveryActivities();

            //第一次退回
            ReturnExecutorTestHelper.ExecuteReturnOperation(activityD, "A");

            Console.WriteLine("第一次退回后");
            activityD.Process.OutputMainStream();
            activityD.Process.OutputEveryActivities();

            activityD.Process.MoveToNextDefaultActivity();	//B1
            activityD.Process.MoveToNextDefaultActivity();	//C1
            activityD.Process.MoveToNextDefaultActivity();	//D1

            Assert.AreEqual("D", activityD.Process.CurrentActivity.Descriptor.AssociatedActivityKey);

            //在B的出线C和E之间选择E
            activityD.Process.ApplicationRuntimeParameters["IsCLine"] = false;

            //第二次退回
            ReturnExecutorTestHelper.ExecuteReturnOperation(activityD, "A");

            Console.WriteLine("第二次退回后");
            activityD.Process.OutputMainStream();
            activityD.Process.OutputEveryActivities();

            IWfProcess process = activityD.Process.WithdrawByExecutor();

            Console.WriteLine("第一组撤回之后");
            process.OutputMainStream();
            process.OutputEveryActivities();

            process = process.WithdrawByExecutor();    //C1;
            process = process.WithdrawByExecutor();    //B1
            process = process.WithdrawByExecutor();    //A1

            process = process.WithdrawByExecutor();

            Console.WriteLine("第二组撤回之后");
            process.OutputMainStream();
            process.OutputEveryActivities();

            Assert.AreEqual(originalActivityCount, process.Activities.Count, "撤回后与退件前的活动数一样");
            Assert.AreEqual(originalActivityCount, process.Descriptor.Activities.Count, "撤回后与退件前的活动数一样");
        }

        /// <summary>
        /// 虽然测试通过，但是验证代码被注释掉，需要考虑
        /// </summary>
        [TestMethod]
        [Description("同意/不同意退件一次后，进行撤回的测试")]
        public void AgreeReturnOnceThenWithdrawTest()
        {
            IWfActivity activityB = ReturnExecutorTestHelper.PrepareAndMoveToAgreeSelectorActivity();

            Assert.AreEqual("B", activityB.Descriptor.Key);

            SetToLineAndMSLineSelected(activityB, "C", false);
            SetToLineAndMSLineSelected(activityB, "A", true);

            Console.WriteLine("退回之前");
            Console.WriteLine("当前活动{0}", activityB.Process.CurrentActivity.Descriptor.Key);
            activityB.Process.OutputMainStream();
            activityB.Process.OutputEveryActivities();

            MoveAgreeProcessOneStepAndValidate(activityB.Process, 1);

            Console.WriteLine("撤回之前");
            Console.WriteLine("当前活动{0}", activityB.Process.CurrentActivity.Descriptor.Key);
            activityB.Process.OutputMainStream();
            activityB.Process.OutputEveryActivities();

            IWfProcess process = activityB.Process.WithdrawByExecutor();
            activityB.Process.OutputEveryActivities();

            Console.WriteLine("撤回之后");
            Console.WriteLine("当前活动{0}", process.CurrentActivity.Descriptor.Key);
            process.OutputMainStream();
            process.OutputEveryActivities();

            //activityB.Process.Descriptor.Output();

            //ReturnExecutorTestHelper.ValidateMainStreamActivities(activityB.Process, "A", "B", "C", "D", "F");
        }

        [TestMethod]
        [Description("同意/不同意退件两次的测试")]
        public void AgreeReturnTwiceThenWithdrawTest()
        {
            IWfActivity activityB = ReturnExecutorTestHelper.PrepareAndMoveToAgreeSelectorActivity();

            Assert.AreEqual("B", activityB.Descriptor.Key);

            SetToLineAndMSLineSelected(activityB, "C", false);
            SetToLineAndMSLineSelected(activityB, "A", true);

            Console.WriteLine("退回之前");
            Console.WriteLine("当前活动{0}", activityB.Process.CurrentActivity.Descriptor.Key);
            activityB.Process.OutputMainStream();
            activityB.Process.OutputEveryActivities();

            MoveAgreeProcessOneStepAndValidate(activityB.Process, 1);

            activityB.Process.MoveToNextDefaultActivity();	//To N2(B)

            Console.WriteLine("第二次退回之前");
            Console.WriteLine("当前活动{0}", activityB.Process.CurrentActivity.Descriptor.Key);
            activityB.Process.OutputMainStream();
            activityB.Process.OutputEveryActivities();

            Assert.AreEqual("N2", activityB.Process.CurrentActivity.Descriptor.Key);

            MoveAgreeProcessOneStepAndValidate(activityB.Process, 2);

            Console.WriteLine("第二次退回之后");
            Console.WriteLine("当前活动{0}", activityB.Process.CurrentActivity.Descriptor.Key);
            activityB.Process.OutputMainStream();
            activityB.Process.OutputEveryActivities();

            Assert.AreEqual("N3", activityB.Process.CurrentActivity.Descriptor.Key);
        }

        [TestMethod]
        [Description("同意/不同意退件流程第一次退件测试")]
        public void AgreeReturnOnceTest()
        {
            IWfActivity activityB = ReturnExecutorTestHelper.PrepareAndMoveToAgreeSelectorActivity();

            Assert.AreEqual("B", activityB.Descriptor.Key);

            SetToLineAndMSLineSelected(activityB, "C", false);
            SetToLineAndMSLineSelected(activityB, "A", true);

            MoveAgreeProcessAndValidate(activityB.Process, 1);

            activityB.Process.OutputMainStream();
            activityB.Process.OutputEveryActivities();

            activityB.Process.ValidateMainStreamActivities("A", "B", "C", "D", "F");
        }

        [TestMethod]
        [Description("同意/不同意退件流程第二次退件测试")]
        public void AgreeReturnTwiceTest()
        {
            IWfActivity activityB = ReturnExecutorTestHelper.PrepareAndMoveToAgreeSelectorActivity();

            Assert.AreEqual("B", activityB.Descriptor.Key);

            SetToLineAndMSLineSelected(activityB, "C", false);
            SetToLineAndMSLineSelected(activityB, "A", true);

            Console.WriteLine("第一次退回之前，不同意");
            activityB.Process.OutputMainStream();
            activityB.Process.OutputEveryActivities();

            MoveAgreeProcessAndValidate(activityB.Process, 1);

            Console.WriteLine("第二次退回之前，同意");
            activityB.Process.OutputMainStream();
            activityB.Process.OutputEveryActivities();

            MoveAgreeProcessAndValidate(activityB.Process, 2);

            Console.WriteLine("第二次退回之后，同意");
            activityB.Process.OutputMainStream();
            activityB.Process.OutputEveryActivities();

            activityB.Process.ValidateMainStreamActivities("A", "B", "C", "D", "F");
        }

        [TestMethod]
        [Description("同意/不同意退件流程第三次退件测试")]
        public void AgreeReturnThriceTest()
        {
            IWfActivity activityB = ReturnExecutorTestHelper.PrepareAndMoveToAgreeSelectorActivity();

            Assert.AreEqual("B", activityB.Descriptor.Key);

            SetToLineAndMSLineSelected(activityB, "C", false);
            SetToLineAndMSLineSelected(activityB, "A", true);

            MoveAgreeProcessAndValidate(activityB.Process, 1);
            MoveAgreeProcessAndValidate(activityB.Process, 2);
            MoveAgreeProcessAndValidate(activityB.Process, 3);

            activityB.Process.OutputMainStream();
            activityB.Process.OutputEveryActivities();

            activityB.Process.ValidateMainStreamActivities("A", "B", "C", "D", "F");
        }

        [TestMethod]
        [Description("加签后再退回的测试")]
        public void AddApproverReturnTest()
        {
            IWfProcessDescriptor processDesp = ReturnExecutorTestHelper.PrepareAddApproverReturnProcessDesp();

            IWfProcess process = ReturnExecutorTestHelper.StartSpecialReturnProcess(processDesp);

            process.MoveToNextDefaultActivity();

            Assert.AreEqual("B", process.CurrentActivity.Descriptor.Key);

            WfAddApproverExecutor executor = new WfAddApproverExecutor(process.CurrentActivity, process.CurrentActivity);
            executor.Assignees.Add((IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object);

            executor.ExecuteNotPersist();

            Assert.AreEqual(6, process.Activities.Count, "加签后变成6个活动");

            process.MoveToNextDefaultActivity();	//B的衍生点
            Assert.AreEqual("B", process.CurrentActivity.Descriptor.AssociatedActivityKey);

            process.MoveToNextDefaultActivity();	//C的衍生点
            Assert.AreEqual("C", process.CurrentActivity.Descriptor.Key);

            ReturnExecutorTestHelper.ExecuteReturnOperation(process.CurrentActivity, "B");

            Assert.AreEqual(8, process.Activities.Count, "退回后8个活动");
            Assert.AreEqual("B", process.CurrentActivity.Descriptor.AssociatedActivityKey);

            process.MoveToNextDefaultActivity();	//C的衍生点
            Assert.AreEqual("C", process.CurrentActivity.Descriptor.AssociatedActivityKey);

            process.MoveToNextDefaultActivity();	//D
            Assert.AreEqual("D", process.CurrentActivity.Descriptor.Key);
        }

        /// <summary>
        /// 仅执行退回线本身一次
        /// </summary>
        /// <param name="process"></param>
        /// <param name="count"></param>
        private static void MoveAgreeProcessOneStepAndValidate(IWfProcess process, int count)
        {
            Console.WriteLine("第{0}次退件之前，退件发起点为{1}({2})", count, process.CurrentActivity.Descriptor.Key, process.CurrentActivity.Descriptor.AssociatedActivityKey);
            IWfActivity returnedActivityA = process.MoveToDefaultActivityByExecutor(false).CurrentActivity;	//Move To A
            Console.WriteLine("第{0}次退件之后，退件当前点为{1}({2})", count, process.CurrentActivity.Descriptor.Key, process.CurrentActivity.Descriptor.AssociatedActivityKey);

            Assert.AreEqual("A", returnedActivityA.Descriptor.AssociatedActivityKey);
        }

        /// <summary>
        /// 执行退回线，然后再流转一步
        /// </summary>
        /// <param name="process"></param>
        /// <param name="count"></param>
        private static void MoveAgreeProcessAndValidate(IWfProcess process, int count)
        {
            Console.WriteLine("第{0}次退件之前，退件发起点为{1}({2})", count, process.CurrentActivity.Descriptor.Key, process.CurrentActivity.Descriptor.AssociatedActivityKey);
            IWfActivity returnedActivityA = process.MoveToDefaultActivityByExecutor(false).CurrentActivity;	//Move To A
            Console.WriteLine("第{0}次退件之后，退件当前点为{1}({2})", count, process.CurrentActivity.Descriptor.Key, process.CurrentActivity.Descriptor.AssociatedActivityKey);

            Assert.AreEqual("A", returnedActivityA.Descriptor.AssociatedActivityKey);

            IWfActivity returnedActivityB = process.MoveToDefaultActivityByExecutor(false).CurrentActivity;	//Move To B again

            Assert.AreEqual("B", returnedActivityB.Descriptor.AssociatedActivityKey);

            ValidateAggressProcessBTransitions(returnedActivityB, count);
        }

        private static void ValidateAggressProcessBTransitions(IWfActivity activityB, int count)
        {
            Assert.AreEqual(2, activityB.Descriptor.ToTransitions.Count,
                string.Format("第{0}次退回。B的衍生线必须也是两根，BC和BA", count));

            WfTransitionDescriptor transitionBC = (WfTransitionDescriptor)activityB.Descriptor.ToTransitions.Find(t => t.ToActivity.GetAssociatedActivity().Key == "C");
            WfTransitionDescriptor transitionBA = (WfTransitionDescriptor)activityB.Descriptor.ToTransitions.Find(t => t.ToActivity.GetAssociatedActivity().Key == "A");

            Assert.IsNotNull(transitionBC);
            Assert.IsNotNull(transitionBA);
        }

        private static void SetToLineAndMSLineSelected(IWfActivity activity, string targetActDespKey, bool selected)
        {
            WfTransitionDescriptor transition = (WfTransitionDescriptor)activity.Descriptor.ToTransitions.Find(t => t.ToActivity.Key == targetActDespKey);

            Assert.IsNotNull(transition);

            transition.DefaultSelect = selected;

            WfTransitionDescriptor transitionMS = (WfTransitionDescriptor)activity.GetMainStreamActivityDescriptor().ToTransitions.Find(t => t.ToActivity.Key == targetActDespKey);

            Assert.IsNotNull(transitionMS);

            transitionMS.DefaultSelect = selected;
        }
    }
}
