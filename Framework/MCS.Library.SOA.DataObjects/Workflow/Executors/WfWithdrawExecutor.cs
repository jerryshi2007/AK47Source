using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 实现业务上的撤回单步操作，实际上不一定是一个Activity
    /// </summary>
    public class WfWithdrawExecutor : WfExecutorBase
    {
        public IWfActivity OriginalActivity
        {
            get;
            private set;
        }

        /// <summary>
        /// 是否作废所有的子流程
        /// </summary>
        public bool CancelAllBranchProcesses
        {
            get;
            private set;
        }

        /// <summary>
        /// 撤回后，是否作废流程
        /// </summary>
        public bool CancelProcess
        {
            get;
            private set;
        }

        public WfWithdrawExecutor(IWfActivity operatorActivity, IWfActivity originalActivity)
            : base(operatorActivity, WfControlOperationType.Withdraw)
        {
            this.InitProperties(operatorActivity, originalActivity, true, false);
        }

        public WfWithdrawExecutor(IWfActivity operatorActivity, IWfActivity originalActivity, bool cancelAllBranchProcesses)
            : base(operatorActivity, WfControlOperationType.Withdraw)
        {
            this.InitProperties(operatorActivity, originalActivity, cancelAllBranchProcesses, false);
        }

        public WfWithdrawExecutor(IWfActivity operatorActivity, IWfActivity originalActivity, bool cancelAllBranchProcesses, bool cancelProcess)
            : base(operatorActivity, WfControlOperationType.Withdraw)
        {
            this.InitProperties(operatorActivity, originalActivity, cancelAllBranchProcesses, cancelProcess);
        }

        protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
        {
            int targetActivityIndex = FindTargetActivityIndex(this.OriginalActivity);

            (targetActivityIndex >= 0).FalseThrow<WfRuntimeException>("活动(ID={0}, Key={1})不能撤回", this.OriginalActivity.ID, this.OriginalActivity.Descriptor.Key);

            WfActivityCollection clonedElapsedActivities = new WfActivityCollection();

            IWfProcess process = this.OriginalActivity.Process;

            clonedElapsedActivities.CopyFrom(process.ElapsedActivities);

            dataContext["OriginalActivityID"] = process.CurrentActivity.ID;

            //将流程实例活动点的状态设置为未运行
            this.OriginalActivity.Process.Withdraw(clonedElapsedActivities[targetActivityIndex], this.CancelAllBranchProcesses);

            //删除掉动态添加的点
            for (int i = targetActivityIndex; i < clonedElapsedActivities.Count; i++)
            {
                IWfActivity act = clonedElapsedActivities[i];

                process.Activities.FindAll(a => a.CreatorInstanceID == act.ID).ForEach(a => a.Delete());
            }

            if (this.CancelProcess)
                process.CancelProcess(this.CancelAllBranchProcesses);
        }

        //protected override void OnPrepareNotifyTasks(WfExecutorDataContext dataContext)
        //{
        //    base.OnPrepareNotifyTasks(dataContext);

        //    if (this.CancelProcess)
        //    {
        //        dataContext.NotifyTasks.Remove(t =>
        //            t.ActivityID == dataContext.GetValue("OriginalActivityID", string.Empty)
        //                    && t.Status == TaskStatus.Yue
        //        );
        //    }
        //}

        private void InitProperties(IWfActivity operatorActivity, IWfActivity originalActivity, bool cancelAllBranchProcesses, bool cancelProcess)
        {
            originalActivity.NullCheck("originalActivity");
            (originalActivity.Process.CurrentActivity == originalActivity).FalseThrow("活动(ID={0}, Key={1})不是流程的当前活动，不能撤回", originalActivity.ID, originalActivity.Descriptor.Key);

            this.OriginalActivity = originalActivity;
            this.CancelAllBranchProcesses = cancelAllBranchProcesses;

            this.CancelProcess = cancelProcess;
        }

        private static int FindTargetActivityIndex(IWfActivity currentActivity)
        {
            int targetIndex = currentActivity.Process.ElapsedActivities.Count - 1;

            if (currentActivity.Descriptor.ActivityType == WfActivityType.CompletedActivity)
                targetIndex--;

            return targetIndex;
        }
    }
}
