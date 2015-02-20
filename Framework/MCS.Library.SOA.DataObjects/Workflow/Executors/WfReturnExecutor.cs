using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public class WfReturnExecutor : WfMoveToExecutorBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operatorActivity">当前操作人的Activity</param>
        /// <param name="targetActivity">退件到的目标Activity</param>
        public WfReturnExecutor(IWfActivity operatorActivity, IWfActivity targetActivity)
            : base(operatorActivity, targetActivity, WfControlOperationType.Return)
        {
        }

        protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
        {
            this.TargetActivity.Process.Committed = true;

            IWfProcess process = this.TargetActivity.Process;

            IWfActivity activity = process.ElapsedActivities.FindActivityByDescriptorKey(TargetActivity.Descriptor.Key);
            activity.NullCheck("targetActivity");

            WfRuntime.ProcessContext.BeginChangeActivityChangingContext();
            try
            {
                WfRuntime.ProcessContext.ActivityChangingContext.CreatorInstanceID = process.CurrentActivity.ID;
                process.CurrentActivity.CopyMainStreamActivities(process.CurrentActivity, 
                    TargetActivity,
                    process.CurrentActivity.Descriptor.GetAssociatedActivity().Instance,
                    null,
                    WfControlOperationType.Return);

                if (process.CurrentActivity.Descriptor.ToTransitions.Count == 0)
                    throw new ApplicationException(string.Format("退回时，不能找到当前活动({0})的出线", process.CurrentActivity.Descriptor.Key));

                IWfActivityDescriptor nextActivityDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
                WfTransferParams tp = new WfTransferParams(nextActivityDesp);
                IWfActivity act = process.Activities.FindActivityByDescriptorKey(nextActivityDesp.Key);

                if (act.Assignees.Count > 0)
                    tp.Assignees.CopyFrom(act.Assignees);
                else
                    tp.Assignees.CopyFrom(activity.Assignees);

                process.MoveTo(tp);
            }
            finally
            {
                WfRuntime.ProcessContext.RestoreChangeActivityChangingContext();
            }
        }
    }
}
