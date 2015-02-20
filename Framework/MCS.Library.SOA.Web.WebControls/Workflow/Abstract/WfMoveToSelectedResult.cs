using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;
using MCS.Library.Core;
using MCS.Library.Globalization;

namespace MCS.Web.WebControls
{
    public class WfMoveToSelectedResult
    {
        private IWfActivityDescriptor _TargetActivityDescriptor = null;
        private IWfTransitionDescriptor _FromTransitionDescriptor = null;
        private IWfBranchProcessTemplateDescriptor _BranchTemplate = null;
        private WfAssigneeCollection _Assignees = null;
        private WfAssigneeCollection _Circulators = null;
        private WfControlOperationType _OperationType;
        private WfRejectMode _RejectMode = WfRejectMode.SelectRejectStep;
        private WfBranchProcessBlockingType _BlockingType = WfBranchProcessBlockingType.WaitAllBranchProcessesComplete;
        private WfBranchProcessExecuteSequence _SequenceType = WfBranchProcessExecuteSequence.Parallel;

        public WfMoveToSelectedResult()
        {
        }

        public WfMoveToSelectedResult(WfControlOperationType opType, WfControlNextStep nextStep)
        {
            this._OperationType = opType;
            IWfProcessDescriptor processDesp = WfClientContext.Current.CurrentActivity.Process.Descriptor;

            this._TargetActivityDescriptor = processDesp.Activities[nextStep.ActivityDescriptor.Key];

            if (nextStep.TransitionDescriptor != null)
                this._FromTransitionDescriptor = processDesp.FindTransitionByKey(nextStep.TransitionDescriptor.Key);

            this.Assignees.CopyFrom(nextStep.Candidates);
        }

        public WfAssigneeCollection Assignees
        {
            get
            {
                if (this._Assignees == null)
                    this._Assignees = new WfAssigneeCollection();

                return this._Assignees;
            }
            set
            {
                this._Assignees = value;
            }
        }

        public WfRejectMode RejectMode
        {
            get
            {

                return this._RejectMode;
            }
            set
            {
                this._RejectMode = value;
            }
        }

        public WfAssigneeCollection Circulators
        {
            get
            {
                if (this._Circulators == null)
                    this._Circulators = new WfAssigneeCollection();

                return this._Circulators;
            }
            set
            {
                this._Circulators = value;
            }
        }

        public IWfBranchProcessTemplateDescriptor BranchTemplate
        {
            get
            {
                return this._BranchTemplate;
            }
            set
            {
                this._BranchTemplate = value;
            }
        }

        public IWfTransitionDescriptor FromTransitionDescriptor
        {
            get
            {
                return this._FromTransitionDescriptor;
            }
            set
            {
                this._FromTransitionDescriptor = value;
            }
        }

        public WfControlOperationType OperationType
        {
            get
            {
                return this._OperationType;
            }
            set
            {
                this._OperationType = value;
            }
        }

        public IWfActivityDescriptor TargetActivityDescriptor
        {
            get
            {
                return this._TargetActivityDescriptor;
            }
            set
            {
                this._TargetActivityDescriptor = value;
            }
        }

        public WfBranchProcessExecuteSequence SequenceType
        {
            get { return _SequenceType; }
            set { _SequenceType = value; }
        }

        public WfBranchProcessBlockingType BlockingType
        {
            get { return _BlockingType; }
            set { _BlockingType = value; }
        }

        public WfExecutorBase CreateExecutor()
        {
            WfExecutorBase executor = null;

            switch (this.OperationType)
            {
                case WfControlOperationType.MoveTo:
                    executor = new WfMoveToExecutor(
                        WfClientContext.Current.OriginalActivity,
                        WfClientContext.Current.OriginalActivity, ToTransferParams(),
                        WfClientContext.Current.Locks);
                    break;
                case WfControlOperationType.Consign:
                    {
                        WfAssigneeCollection assignees = new WfAssigneeCollection();

                        WfAssignee assignee = new WfAssignee(WfClientContext.Current.User);

                        assignees.Add(assignee);

                        executor = new WfConsignExecutor(
                            WfClientContext.Current.OriginalActivity,
                            WfClientContext.Current.OriginalActivity,
                            assignees,
                            this.Assignees.ToUsers(),
                            this.Circulators.ToUsers(),
                            BlockingType,
                            SequenceType);
                    }
                    break;
                case WfControlOperationType.Return:
                    if (this._RejectMode == WfRejectMode.LikeAddApprover)
                        executor = new WfAddApproverExecutor(WfClientContext.Current.CurrentActivity, WfClientContext.Current.CurrentActivity.Process.Activities.FindActivityByDescriptorKey(TargetActivityDescriptor.Key));
                    else
                        executor = new WfReturnExecutor(WfClientContext.Current.CurrentActivity, WfClientContext.Current.CurrentActivity.Process.Activities.FindActivityByDescriptorKey(TargetActivityDescriptor.Key));
                    break;
                default:
                    throw new WfRuntimeException(Translator.Translate(Define.DefaultCulture, "不能处理操作类型为{0}", this.OperationType));
            }

            return executor;
        }

        private WfTransferParams ToTransferParams()
        {
            this.TargetActivityDescriptor.NullCheck("TargetActivityDescriptor");

            WfTransferParams tParams = new WfTransferParams(this.TargetActivityDescriptor);

            tParams.Assignees.CopyFrom(this.Assignees);
            tParams.FromTransitionDescriptor = this.FromTransitionDescriptor;

            if (this.BranchTemplate != null)
                tParams.BranchTransferParams.Add(new WfBranchProcessTransferParams(this.BranchTemplate));

            tParams.Operator = WfClientContext.Current.User;

            return tParams;
        }
    }
}
