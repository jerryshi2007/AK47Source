using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Principal;
using MCS.Library.Globalization;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 一般流转的Executor
    /// </summary>
    public class WfMoveToExecutor : WfMoveToExecutorBase
    {
        public WfTransferParams TransferParams
        {
            get;
            private set;
        }

        public IEnumerable<Lock> Locks
        {
            get;
            set;
        }

        private Stopwatch MoveToStopwatch = new Stopwatch();
        private Stopwatch TransactionStopwatch = new Stopwatch();

        public WfMoveToExecutor(IWfActivity operatorActivity, IWfActivity targetActivity, WfTransferParams transferParams)
            : base(operatorActivity, targetActivity, WfControlOperationType.MoveTo)
        {
            transferParams.NullCheck("transferParams");

            TransferParams = transferParams;
        }

        public WfMoveToExecutor(IWfActivity operatorActivity, IWfActivity targetActivity, WfTransferParams transferParams, IEnumerable<Lock> locks)
            : base(operatorActivity, targetActivity, WfControlOperationType.MoveTo)
        {
            transferParams.NullCheck("transferParams");

            TransferParams = transferParams;

            this.Locks = locks;
        }

        protected override void OnSaveApplicationData(WfExecutorDataContext dataContext)
        {
            base.OnSaveApplicationData(dataContext);

            if (this.Locks != null)
            {
                foreach (Lock wfLock in this.Locks)
                    LockAdapter.Unlock(wfLock);
            }
        }

        protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
        {
            DoMoveToOperation(this.TargetActivity.Process, this.TransferParams);
        }

        internal static void DoMoveToOperation(IWfProcess process, WfTransferParams transferParams)
        {
            process.Committed = true;

            WfRuntime.ProcessContext.BeginChangeActivityChangingContext();

            try
            {
                IWfActivity nextActivity = process.Activities.FindActivityByDescriptorKey(transferParams.NextActivityDescriptor.Key);

                (nextActivity != null).FalseThrow("不能找到Key为{0}的活动", transferParams.NextActivityDescriptor.Key);

                if (nextActivity.Status != WfActivityStatus.NotRunning)
                {
                    WfRuntime.ProcessContext.ActivityChangingContext.CreatorInstanceID = process.CurrentActivity.ID;

                    IWfTransitionDescriptor returnTransition = process.CurrentActivity.CopyMainStreamActivities(nextActivity, transferParams.FromTransitionDescriptor, WfControlOperationType.Return);

                    IWfActivity nextCloneActivity = nextActivity;

                    if (returnTransition != null)
                    {
                        transferParams.FromTransitionDescriptor = returnTransition;
                        nextCloneActivity = returnTransition.ToActivity.Instance;
                    }

                    if (nextCloneActivity != null)
                        transferParams.NextActivityDescriptor = nextCloneActivity.Descriptor;
                    else
                        transferParams.NextActivityDescriptor = process.CurrentActivity.Descriptor.ToTransitions.FindDefaultSelectTransition().ToActivity;

                    IEnumerable<IUser> candidates = null;

                    if (nextActivity != null)
                        candidates = nextActivity.Candidates.ToUsers();

                    if (nextCloneActivity != null)
                    {
                        if (candidates.Count() == 0)
                            candidates = nextCloneActivity.Candidates.ToUsers();

                        if (candidates.Count() == 0)
                            candidates = nextCloneActivity.Descriptor.Resources.ToUsers();
                    }

                    transferParams.Assignees.Add(candidates);
                }

                process.MoveTo(transferParams);

                while ((process.Status == WfProcessStatus.Completed || process.Status == WfProcessStatus.Aborted)
                    && process.EntryInfo != null)
                {
                    process.EntryInfo.OwnerActivity.Process.ProcessPendingActivity();
                    process = process.EntryInfo.OwnerActivity.Process;
                }
            }
            finally
            {
                WfRuntime.ProcessContext.RestoreChangeActivityChangingContext();
            }
        }

        private static IWfActivity FindClonedRootActivity(IWfActivity clondedActivity)
        {
            IWfActivity result = clondedActivity;
            IWfProcess process = clondedActivity.Process;

            while (result != null && result.Descriptor.ClonedKey.IsNotEmpty())
            {
                result = process.Activities.Find(act => string.Compare(clondedActivity.Descriptor.ClonedKey, act.Descriptor.Key, true) == 0);
            }

            return result;
        }

        protected override void OnBeforeExecute(WfExecutorDataContext dataContext)
        {
            MoveToStopwatch.Start();

            base.OnBeforeExecute(dataContext);
        }

        protected override void OnBeforeTransaction(WfExecutorDataContext dataContext)
        {
            TransactionStopwatch.Start();
            base.OnBeforeTransaction(dataContext);
        }

        protected override void OnAfterTransaction(WfExecutorDataContext dataContext)
        {
            TransactionStopwatch.Stop();

            WfPerformanceCounters.GlobalInstance.AverageWithTxWFBase.Increment();
            WfPerformanceCounters.GlobalInstance.AverageWithTxWFDuration.IncrementBy(TransactionStopwatch.ElapsedMilliseconds / 100);

            WfPerformanceCounters.AppInstance.AverageWithTxWFBase.Increment();
            WfPerformanceCounters.AppInstance.AverageWithTxWFDuration.IncrementBy(TransactionStopwatch.ElapsedMilliseconds / 100);

            base.OnAfterTransaction(dataContext);
        }

        protected override void OnAfterExecute(WfExecutorDataContext dataContext)
        {
            MoveToStopwatch.Stop();

            WfPerformanceCounters.GlobalInstance.AverageWFBase.Increment();
            WfPerformanceCounters.GlobalInstance.AverageWFDuration.IncrementBy(MoveToStopwatch.ElapsedMilliseconds / 100);

            WfPerformanceCounters.AppInstance.AverageWFBase.Increment();
            WfPerformanceCounters.AppInstance.AverageWFDuration.IncrementBy(MoveToStopwatch.ElapsedMilliseconds / 100);

            WfPerformanceCounters.GlobalInstance.TotalWFCount.Increment();
            WfPerformanceCounters.AppInstance.TotalWFCount.Increment();

            WfPerformanceCounters.GlobalInstance.MoveToCountPerSecond.Increment();
            WfPerformanceCounters.AppInstance.MoveToCountPerSecond.Increment();

            base.OnAfterExecute(dataContext);
        }

        protected override void OnAfterSaveApplicationData(WfExecutorDataContext dataContext)
        {
            WfPerformanceCounters.GlobalInstance.SuccessWFCount.Increment();
            WfPerformanceCounters.AppInstance.SuccessWFCount.Increment();

            base.OnAfterSaveApplicationData(dataContext);
        }

        protected override void OnError(Exception ex, WfExecutorDataContext dataContext, ref bool autoThrow)
        {
            WfPerformanceCounters.GlobalInstance.FailWFCount.Increment();
            WfPerformanceCounters.AppInstance.FailWFCount.Increment();

            base.OnError(ex, dataContext, ref autoThrow);
        }

        internal static bool PrepareUserOperationLogDescriptionByTransferParams(WfExecutorDataContext dataContext, WfTransferParams transferParams, UserOperationLog log)
        {
            bool dealed = false;

            if (log.RealUser != null && log.OperationDescription.IsNullOrEmpty())
            {
                string transitionName = string.Empty;

                if (transferParams.FromTransitionDescriptor != null)
                {
                    transitionName = transferParams.FromTransitionDescriptor.Name;

                    if (transitionName.IsNullOrEmpty())
                        transitionName = transferParams.FromTransitionDescriptor.Description;
                }

                if (transitionName.IsNotEmpty())
                {
                    log.OperationDescription = string.Format("{0}:{1}->{2}, '{3}' {4:yyyy-MM-dd HH:mm:ss}",
                            log.OperationName, log.RealUser.DisplayName,
                            Translator.Translate(Define.DefaultCulture, transitionName),
                            log.Subject, DateTime.Now);

                    dealed = true;
                }
            }

            return dealed;
        }

        protected override void OnPrepareUserOperationLogDescription(WfExecutorDataContext dataContext, UserOperationLog log)
        {
            if (PrepareUserOperationLogDescriptionByTransferParams(dataContext, this.TransferParams, log) == false)
                base.OnPrepareUserOperationLogDescription(dataContext, log);
        }
    }
}
