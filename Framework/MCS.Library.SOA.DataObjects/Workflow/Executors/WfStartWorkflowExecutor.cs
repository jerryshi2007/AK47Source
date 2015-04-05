using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Principal;
using MCS.Library.Globalization;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public class WfStartWorkflowExecutor : WfExecutorBase
    {
        private bool _AutoMoveTo = false;

        public WfProcessStartupParams StartupParams
        {
            get;
            private set;
        }

        /// <summary>
        /// 启动后流转到下一步的参数
        /// </summary>
        public WfTransferParams TransferParams
        {
            get;
            private set;
        }

        /// <summary>
        /// 如果AutoMoveTo为True，则自动向下一步流转，流转的目标取决于TransferParams。如果TransferParams为null，则流转到默认下一个活动
        /// </summary>
        public bool AutoMoveTo
        {
            get
            {
                return this._AutoMoveTo;
            }
        }

        public WfStartWorkflowExecutor(IWfActivity operatorActivity, WfProcessStartupParams startupParams, WfTransferParams transferParams, bool autoMoveTo = true)
            : base(operatorActivity, WfControlOperationType.Startup)
        {
            startupParams.NullCheck("startupParams");

            this.StartupParams = startupParams;
            this.TransferParams = transferParams;
            this._AutoMoveTo = autoMoveTo;
        }

        public WfStartWorkflowExecutor(IWfActivity operatorActivity, WfProcessStartupParams startupParams)
            : this(operatorActivity, startupParams, null, false)
        {
            startupParams.NullCheck("startupParams");

            this.StartupParams = startupParams;
        }

        public WfStartWorkflowExecutor(WfProcessStartupParams startupParams)
            : this(null, startupParams)
        {
        }

        protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
        {
            IWfProcess process = WfRuntime.StartWorkflow(StartupParams);

            if (this.OperatorActivity == null)
                this.OperatorActivity = process.CurrentActivity;

            if (this.AutoMoveTo)
            {
                if (this.TransferParams == null)
                    this.TransferParams = WfTransferParams.FromNextDefaultActivity(process);

                //如果流转参数中没有下一步的人员，则从下一步活动的候选人中复制
                if (this.TransferParams.Assignees.Count == 0)
                {
                    IWfActivity nextActivity = process.Activities.FindActivityByDescriptorKey(this.TransferParams.NextActivityDescriptor.Key);

                    if (nextActivity != null)
                        this.TransferParams.Assignees.CopyFrom(nextActivity.Candidates);
                }

                WfMoveToExecutor.DoMoveToOperation(process, this.TransferParams);
            }

            WfRuntime.ProcessContext.ResetContextByProcess(process);
        }

        protected override void OnPrepareUserOperationLogDescription(WfExecutorDataContext dataContext, UserOperationLog log)
        {
            bool dealed = false;

            if (this.AutoMoveTo)
                dealed = WfMoveToExecutor.PrepareUserOperationLogDescriptionByTransferParams(dataContext, this.TransferParams, log);

            if (dealed == false)
                base.OnPrepareUserOperationLogDescription(dataContext, log);
        }

        protected override void OnPrepareMoveToTasks(WfExecutorDataContext dataContext)
        {
            base.OnPrepareMoveToTasks(dataContext);

            WfMoveToExecutor.SyncUrlsInAssigneesFromTasks(dataContext.MoveToTasks);
        }
    }
}
