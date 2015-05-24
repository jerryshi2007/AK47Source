using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 流程实例的实现类
    /// </summary>
    [Serializable]
    [XElementSerializable]
    public class WfProcess : IWfProcess, ISimpleXmlSerializer
    {
        private string _ID = UuidHelper.NewUuidString();
        private IWfProcessDescriptor _Descriptor = null;
        private WfProcessStatus _Status = WfProcessStatus.NotRunning;
        private DataLoadingType _LoadingType = DataLoadingType.Memory;
        private WfProcessContext _Context = null;
        private WfProcessContext _StatusContext = null;
        private WfApplicationRuntimeParameters _ApplicationRuntimeParameters = null;
        private WfActivityCollection _Activities = null;
        private DateTime _StartTime = DateTime.MinValue;
        private DateTime _EndTime = DateTime.MinValue;

        private bool _Committed = false;

        private IUser _Creator = null;
        private IOrganization _OwnerDepartment = null;

        [NonSerialized]
        private IWfBranchProcessGroup _EntryInfo = null;

        [NonSerialized]
        private IWfProcess _RootProcess = null;

        [NonSerialized]
        private IWfProcess _SameResourceRootProcess = null;

        [NonSerialized]
        private IWfProcess _ApprovalRootProcess = null;

        [NonSerialized]
        private IWfProcess _ScheduleRootProcess = null;

        [NonSerialized]
        private IWfProcess _OpinionRootProcess = null;

        [NonSerialized]
        private WfActionCollection _CancelProcessActions = new WfActionCollection();

        [NonSerialized]
        private WfActionCollection _RestoreProcessctions = new WfActionCollection();

        [NonSerialized]
        private WfActionCollection _WithdrawActions = new WfActionCollection();

        [NonSerialized]
        private WfActionCollection _ProcessStatusChangeActions = new WfActionCollection();

        private string _OwnerActivityID = null;
        private string _OwnerTemplateKey = null;
        private int _Sequence = -1;
        private WfBranchProcessStartupParams _BranchStartupParams = null;
        private NameValueCollection _RelativeParams = null;

        private IWfProcessDescriptor _MainStream = null;

        private WfProcess()
        {
            this.LoadActions();
        }

        public WfProcess(IWfProcessDescriptor processDesp)
        {
            this.LoadActions();

            this._MainStream = InitializeMainStream(processDesp);

            this._Descriptor = processDesp;
            this._StartTime = DateTime.Now;
            ((WfProcessDescriptor)processDesp).SetProcessInstance(this);

            processDesp.Activities.ForEach(actDesp =>
            {
                WfActivityBase act = WfActivitySettings.GetConfig().GetActivityBuilder(actDesp).CreateActivity(actDesp);

                act.Process = this;
                act.MainStreamActivityKey = actDesp.Key;

                Activities.Add(act);

                switch (actDesp.ActivityType)
                {
                    case WfActivityType.InitialActivity:
                        this.InitialActivity = act;
                        break;
                    case WfActivityType.CompletedActivity:
                        this.CompletedActivity = act;
                        break;
                }
            });
        }

        private void LoadActions()
        {
            this.CancelProcessActions.CopyFrom(WfActivitySettings.GetConfig().GetCancelProcessActions());
            this.RestoreProcessActions.CopyFrom(WfActivitySettings.GetConfig().GetRestoreProcessActions());
            this.WithdrawActions.CopyFrom(WfActivitySettings.GetConfig().GetWithdrawActions());
            this.ProcessStatusChangeActions.CopyFrom(WfActivitySettings.GetConfig().GetProcessStatusChangeActions());
        }

        #region IWfProcess Members

        public string ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
            }
        }

        public string SearchID
        {
            get
            {
                string result = this.ResourceID;

                switch (this.Descriptor.Properties.GetValue("CreateSearchIDMode", WfSearchIDMode.SameAsResourceID))
                {
                    case WfSearchIDMode.SameAsResourceID:
                        {
                            break;
                        }
                    case WfSearchIDMode.SameAsProcessID:
                        {
                            result = this.ID;
                            break;
                        }
                }

                return result;
            }
        }

        public string ResourceID
        {
            get;
            set;
        }

        public string RelativeID
        {
            get;
            set;
        }

        public string RelativeURL
        {
            get;
            set;
        }

        public IWfProcessDescriptor MainStream
        {
            get
            {
                return this._MainStream;
            }
        }

        public IWfProcessDescriptor Descriptor
        {
            get
            {
                return this._Descriptor;
            }
            set
            {
                IWfProcessDescriptor processDesp = value;

                this._MainStream = InitializeMainStream(processDesp);

                this._Descriptor = processDesp;
                ((WfProcessDescriptor)_Descriptor).SetProcessInstance(this);

                this.Activities.ForEach(actInst =>
                {
                    IWfActivityDescriptor msActivityDesp = processDesp.Activities.SingleOrDefault(actDesp => actDesp.Key == actInst.Descriptor.Key);

                    if (msActivityDesp != null)
                    {
                        actInst.Descriptor = msActivityDesp;
                        ((WfActivityBase)actInst).MainStreamActivityKey = actInst.Descriptor.Key;
                    }
                });

                this.GenerateCandidatesFromResources();
            }
        }

        public WfProcessStatus Status
        {
            get
            {
                return this._Status;
            }
            set
            {
                ChangeProcessStatus(value);
            }
        }

        public DateTime StartTime
        {
            get
            {
                return this._StartTime;
            }
            set
            {
                this._StartTime = value;
            }
        }

        public DateTime EndTime
        {
            get
            {
                return this._EndTime;
            }
            set
            {
                this._EndTime = value;
            }
        }

        /// <summary>
        /// 流程逻辑上是否可以撤回。包括流程的状态判断以及活动点的状态判断（不含业务逻辑和管理逻辑）
        /// </summary>
        public bool CanWithdraw
        {
            get
            {
                bool result = (this.Status == WfProcessStatus.Running || this.Status == WfProcessStatus.Completed || this.Status == WfProcessStatus.Maintaining);

                if (result)
                    result = this.ElapsedActivities.Count > 0;

                return result;
            }
        }

        /// <summary>
        /// 更新标记
        /// </summary>
        public int UpdateTag
        {
            get;
            set;
        }

        /// <summary>
        /// 流程是否是提交的。如果为False，则表示是用户打开表单，启动了流程，但是没有保存和流转
        /// </summary>
        public bool Committed
        {
            get
            {
                return this._Committed;
            }
            set
            {
                this._Committed = value;
            }
        }

        /// <summary>
        /// 运行时的流程名称
        /// </summary>
        public string RuntimeProcessName
        {
            get
            {
                string result = this.Context.GetValue("RuntimeProcessName", string.Empty);
                    
                if (result.IsNullOrEmpty() && this.Descriptor != null)
                    result = this.Descriptor.Name;

                return result;

            }
            set
            {
                this.Context["RuntimeProcessName"] = value;
            }
        }

        public IUser Creator
        {
            get
            {
                return this._Creator;
            }
            set
            {
                this._Creator = (IUser)OguUser.CreateWrapperObject(value);
            }
        }

        public IOrganization OwnerDepartment
        {
            get
            {
                return this._OwnerDepartment;
            }
            set
            {
                this._OwnerDepartment = (IOrganization)OguOrganization.CreateWrapperObject(value);
            }
        }

        public WfProcessContext Context
        {
            get
            {
                if (this._Context == null)
                    this._Context = new WfProcessContext();

                return this._Context;
            }
        }

        /// <summary>
        /// 内部的状态上下文
        /// </summary>
        public WfProcessContext StatusContext
        {
            get
            {
                if (this._StatusContext == null)
                    this._StatusContext = new WfProcessContext();

                return this._StatusContext;
            }
        }

        public WfApplicationRuntimeParameters ApplicationRuntimeParameters
        {
            get
            {
                if (this._ApplicationRuntimeParameters == null)
                    this._ApplicationRuntimeParameters = new WfApplicationRuntimeParameters();

                this._ApplicationRuntimeParameters.ProcessInstance = this;

                return this._ApplicationRuntimeParameters;
            }
        }

        public DataLoadingType LoadingType
        {
            get
            {
                return this._LoadingType;
            }
            set
            {
                this._LoadingType = value;
            }
        }

        public WfActivityCollection Activities
        {
            get
            {
                if (this._Activities == null)
                    this._Activities = new WfActivityCollection();

                return this._Activities;
            }
        }

        public WfBranchProcessStartupParams BranchStartupParams
        {
            get
            {
                return this._BranchStartupParams;
            }
            set
            {
                this._BranchStartupParams = value;
            }
        }

        private WfActivityCollection _ElapsedActivities = new WfActivityCollection();

        /// <summary>
        /// 顺序返回流程中已执行过的一组活动节点集合
        /// </summary>
        public WfActivityCollection ElapsedActivities
        {
            get
            {
                return _ElapsedActivities;
            }
        }

        public IWfActivity InitialActivity
        {
            get;
            set;
        }

        public IWfActivity CurrentActivity
        {
            get;
            set;
        }

        public IWfActivity CompletedActivity
        {
            get;
            set;
        }

        public int Sequence
        {
            get
            {
                return this._Sequence;
            }
            set
            {
                this._Sequence = value;
            }
        }

        public NameValueCollection RelativeParams
        {
            get
            {
                if (this._RelativeParams == null)
                    this._RelativeParams = new NameValueCollection(StringComparer.OrdinalIgnoreCase);

                return this._RelativeParams;
            }
            set
            {
                this._RelativeParams = value;
            }
        }

        /// <summary>
        /// 取消流程的相关动作
        /// </summary>
        public WfActionCollection CancelProcessActions
        {
            get
            {
                return this._CancelProcessActions;
            }
        }

        /// <summary>
        /// 恢复流程的相关操作
        /// </summary>
        public WfActionCollection RestoreProcessActions
        {
            get
            {
                return this._RestoreProcessctions;
            }
        }

        /// <summary>
        /// 撤回流程的相关动作
        /// </summary>
        public WfActionCollection WithdrawActions
        {
            get
            {
                return this._WithdrawActions;
            }
        }

        /// <summary>
        /// 流程状态变化的相关动作
        /// </summary>
        public WfActionCollection ProcessStatusChangeActions
        {
            get
            {
                return this._ProcessStatusChangeActions;
            }
        }

        public bool HasParentProcess
        {
            get
            {
                return this.OwnerActivityID.IsNotEmpty() && this.OwnerTemplateKey.IsNotEmpty();
            }
        }

        public IWfBranchProcessGroup EntryInfo
        {
            get
            {
                if (this._EntryInfo == null)
                {
                    if (LoadingType == DataLoadingType.External)
                    {
                        if (HasParentProcess)
                            this._EntryInfo = LoadEntryInfo(this.OwnerActivityID, this.OwnerTemplateKey);
                    }
                }

                return this._EntryInfo;
            }
            set
            {
                this._EntryInfo = value;
            }
        }

        public IWfProcess RootProcess
        {
            get
            {
                if (this._RootProcess == null)
                {
                    IWfProcess currentProcess = this;

                    while (currentProcess.EntryInfo != null)
                    {
                        currentProcess = currentProcess.EntryInfo.OwnerActivity.Process;
                    }

                    this._RootProcess = currentProcess;
                }

                return this._RootProcess;
            }
        }

        public IWfProcess SameResourceRootProcess
        {
            get
            {
                if (this._SameResourceRootProcess == null)
                {
                    IWfProcess currentProcess = this;

                    while (currentProcess.EntryInfo != null &&
                        currentProcess.EntryInfo.OwnerActivity.Process.ResourceID == this.ResourceID)
                    {
                        currentProcess = currentProcess.EntryInfo.OwnerActivity.Process;
                    }

                    this._SameResourceRootProcess = currentProcess;
                }

                return this._SameResourceRootProcess;
            }
        }

        public bool IsApprovalRootProcess
        {
            get
            {
                return ApprovalRootProcess == this;
            }
        }

        public IWfProcess ApprovalRootProcess
        {
            get
            {
                if (this._ApprovalRootProcess == null)
                {
                    IWfProcess currentProcess = this;

                    while (currentProcess.EntryInfo != null &&
                        currentProcess.Descriptor.Properties.GetValue("Independent", true) == false &&
                        currentProcess.EntryInfo.OwnerActivity.Process.Descriptor.ProcessType == WfProcessType.Approval)
                    {
                        currentProcess = currentProcess.EntryInfo.OwnerActivity.Process;
                    }

                    this._ApprovalRootProcess = currentProcess;
                }

                return this._ApprovalRootProcess;
            }
        }

        public IWfProcess ScheduleRootProcess
        {
            get
            {
                if (this._ScheduleRootProcess == null)
                {
                    IWfProcess currentProcess = this;

                    if (currentProcess.Descriptor.ProcessType != WfProcessType.Schedule)
                    {
                        while (currentProcess.EntryInfo != null)
                        {
                            currentProcess = currentProcess.EntryInfo.OwnerActivity.Process;

                            if (currentProcess.Descriptor.ProcessType == WfProcessType.Schedule)
                                break;
                        }
                    }

                    this._ScheduleRootProcess = currentProcess;
                }

                return this._ScheduleRootProcess;
            }
        }

        //todo:ydz 2012-07-21
        /// <summary>
        /// 意见控件显示流程
        /// </summary>
        public IWfProcess OpinionRootProcess
        {
            get
            {
                if (this._OpinionRootProcess == null)
                {
                    if (this.EntryInfo == null)
                        this._OpinionRootProcess = this.ApprovalRootProcess;
                    else
                    {
                        WfOpinionMode currentOpinionMode = this.EntryInfo.ProcessTemplate.Properties.GetValue("IndependentOpinion", WfOpinionMode.Default);
                        switch (currentOpinionMode)
                        {
                            case WfOpinionMode.Default:
                                this._OpinionRootProcess = this.ApprovalRootProcess;
                                break;
                            case WfOpinionMode.Independent:
                                this._OpinionRootProcess = this;
                                break;
                            case WfOpinionMode.Merged:
                                this._OpinionRootProcess = this.EntryInfo.OwnerActivity.Process;
                                break;
                        }
                    }
                }

                return this._OpinionRootProcess;
            }
        }

        /// <summary>
        /// 恢复已经中止（取消）的流程
        /// </summary>
        public void RestoreProcess()
        {
            if (this.Status == WfProcessStatus.Aborted)
            {
                WfProcessActionContext actionContext = WfRuntime.ProcessContext;

                WfProcessActionContextState state = actionContext.SaveDifferentProcessInfo(this);
                try
                {
                    actionContext.ResetContextByProcess(this);

                    this.Status = this.StatusContext.GetValue("OriginalStatus", WfProcessStatus.Running);
                    this.StatusContext.Remove("OriginalStatus");
                    this.EndTime = DateTime.MinValue;

                    if (this.CurrentActivity != null)
                    {
                        WfActivityBase currentActivity = ((WfActivityBase)this.CurrentActivity);

                        currentActivity.Status = currentActivity.StatusContext.GetValue("OriginalStatus", WfActivityStatus.Running);
                        currentActivity.StatusContext.Remove("OriginalStatus");
                        currentActivity.EndTime = DateTime.MinValue;
                    }

                    WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(this);
                    RegisterAndPrepareActions(this.RestoreProcessActions);
                }
                finally
                {
                    actionContext.RestoreSavedProcessInfo(state);
                }
            }
        }

        /// <summary>
        /// 中止流程
        /// </summary>
        /// <param name="cancelAllBranchProcesses">中止流程时，是否中止所有的分支流程</param>
        public void CancelProcess(bool cancelAllBranchProcesses)
        {
            if (this.Status != WfProcessStatus.Aborted)
            {
                InternalCancelProcess(cancelAllBranchProcesses);

                ExecuteNextSerialProcess(this.EntryInfo);
            }
        }

        internal void InternalCancelProcess(bool cancelAllBranchProcesses)
        {
            if (this.Status != WfProcessStatus.Aborted)
            {
                if (cancelAllBranchProcesses)
                    this.Activities.ForEach(act => act.CancelBranchProcesses(cancelAllBranchProcesses));

                WfProcessActionContext actionContext = WfRuntime.ProcessContext;

                WfProcessActionContextState state = actionContext.SaveDifferentProcessInfo(this);
                try
                {
                    actionContext.ResetContextByProcess(this);

                    this.StatusContext["OriginalStatus"] = this.Status;
                    this.Status = WfProcessStatus.Aborted;
                    this.EndTime = DateTime.Now;

                    if (this.CurrentActivity != null)
                    {
                        WfActivityBase currentActivity = ((WfActivityBase)this.CurrentActivity);

                        currentActivity.StatusContext["OriginalStatus"] = currentActivity.Status;
                        currentActivity.Status = WfActivityStatus.Aborted;
                    }

                    WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(this);
                    RegisterAndPrepareActions(this.CancelProcessActions);
                }
                finally
                {
                    actionContext.RestoreSavedProcessInfo(state);
                }
            }
        }

        /// <summary>
        /// 暂停流程
        /// </summary>
        /// <param name="pauseAllBranchProcesses">暂停流程时，暂停中止所有的分支流程</param>
        public void PauseProcess(bool pauseAllBranchProcesses)
        {
            if (this.Status == WfProcessStatus.Running)
            {
                InternalPauseProcess(pauseAllBranchProcesses);
            }
        }

        internal void InternalPauseProcess(bool pauseAllBranchProcesses)
        {
            if (this.Status == WfProcessStatus.Running)
            {
                if (pauseAllBranchProcesses)
                    this.Activities.ForEach(act => act.PauseBranchProcesses(pauseAllBranchProcesses));

                WfProcessActionContext actionContext = WfRuntime.ProcessContext;

                WfProcessActionContextState state = actionContext.SaveDifferentProcessInfo(this);
                try
                {
                    actionContext.ResetContextByProcess(this);
                    this.Status = WfProcessStatus.Paused;

                    WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(this);
                }
                finally
                {
                    actionContext.RestoreSavedProcessInfo(state);
                }
            }
        }

        /// <summary>
        /// 恢复暂停的流程
        /// </summary>
        /// <param name="resumeAllBranchProcesses"></param>
        public void ResumeProcess(bool resumeAllBranchProcesses)
        {
            if (this.Status == WfProcessStatus.Paused)
            {
                InternalResumeProcess(resumeAllBranchProcesses);
            }
        }

        internal void InternalResumeProcess(bool resumeAllBranchProcesses)
        {
            if (this.Status == WfProcessStatus.Paused)
            {
                if (resumeAllBranchProcesses)
                    this.Activities.ForEach(act => act.PauseBranchProcesses(resumeAllBranchProcesses));

                WfProcessActionContext actionContext = WfRuntime.ProcessContext;

                WfProcessActionContextState state = actionContext.SaveDifferentProcessInfo(this);
                try
                {
                    actionContext.ResetContextByProcess(this);
                    this.Status = WfProcessStatus.Running;

                    WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(this);
                }
                finally
                {
                    actionContext.RestoreSavedProcessInfo(state);
                }
            }
        }

        /// <summary>
        /// 结束流程
        /// </summary>
        /// <param name="completeAllBranchProcesses">是否结束分支流程</param>
        public void CompleteProcess(bool completeAllBranchProcesses)
        {
            (this.Status == WfProcessStatus.Running).FalseThrow<WfRuntimeException>("流程{0}不在运行状态，只有正在运行的流程才能直接结束。");

            if (completeAllBranchProcesses)
                this.Activities.ForEach(act => act.CompleteBranchProcesses(completeAllBranchProcesses));

            WfTransferParams transferParams = new WfTransferParams(this.CompletedActivity.Descriptor);

            MoveTo(transferParams);
        }

        public void Withdraw(IWfActivity destinationActivity, bool cancelAllBranchProcesses)
        {
            destinationActivity.NullCheck("destinationActivity");

            ElapsedActivities.ContainsKey(destinationActivity.ID).FalseThrow<WfRuntimeException>(
                "不能撤回到活动'{0}'，这个Key对应的活动不存在", destinationActivity.Descriptor.Key);

            WfProcessActionContext actionContext = WfRuntime.ProcessContext;

            WfProcessActionContextState state = actionContext.SaveDifferentProcessInfo(this);
            try
            {
                actionContext.ResetContextByProcess(this);

                this.Status = WfProcessStatus.Running;

                RegisterAndPrepareActions(this.CurrentActivity.BeWithdrawnActions);

                actionContext.OriginalActivity.CancelBranchProcesses(cancelAllBranchProcesses);

                ((WfActivityBase)actionContext.OriginalActivity).Status = WfActivityStatus.NotRunning;

                while (this.ElapsedActivities.Count > 0)
                {
                    WfActivityBase activity = (WfActivityBase)this.ElapsedActivities[this.ElapsedActivities.Count - 1];

                    this.ElapsedActivities.RemoveAt(this.ElapsedActivities.Count - 1);

                    if (string.Compare(activity.ID, destinationActivity.ID, true) == 0)
                    {
                        activity.Status = WfActivityStatus.Running;
                        break;
                    }
                    else
                    {
                        activity.CancelBranchProcesses(cancelAllBranchProcesses);

                        activity.Status = WfActivityStatus.NotRunning;
                    }
                }

                this.CurrentActivity = Activities[destinationActivity.ID];

                RegisterAndPrepareActions(this.CurrentActivity.WithdrawActions);

                WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(this);
                RegisterAndPrepareActions(this.WithdrawActions);
            }
            finally
            {
                actionContext.RestoreSavedProcessInfo(state);
            }
        }

        /// <summary>
        /// 进入到维护模式
        /// </summary>
        public void EntrtMaintainingStatus()
        {
            if (this.Status != WfProcessStatus.Maintaining)
            {
                this.StatusContext["OriginalStatus"] = this.Status;
                this.Status = WfProcessStatus.Maintaining;

                WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(this);
            }
        }

        /// <summary>
        /// 如果当前活动处于挂起，那么处理其挂起状态
        /// </summary>
        public void ProcessPendingActivity()
        {
            if (this.CurrentActivity != null && this.CurrentActivity.Status == WfActivityStatus.Pending && this.Status == WfProcessStatus.Running)
            {
                WfProcessActionContext actionContext = WfRuntime.ProcessContext;

                WfProcessActionContextState state = actionContext.SaveDifferentProcessInfo(this);
                try
                {
                    actionContext.ResetContextByProcess(this);

                    if (this.CurrentActivity.CanMoveTo)
                    {
                        WfRuntime.ProcessContext.ReleasedPendingActivities.Add(this.CurrentActivity);

                        ((WfActivityBase)this.CurrentActivity).Status = WfActivityStatus.Running;

                        string valueParamName = this.CurrentActivity.Descriptor.Properties.GetValue("BranchProcessReturnValueParamName", string.Empty);

                        if (valueParamName.IsNotEmpty())
                        {
                            this.ApplicationRuntimeParameters[valueParamName] = this.CurrentActivity.BranchProcessReturnValue;
                        }

                        //如果自动向下流转
                        if (this.CurrentActivity.Descriptor.Properties.GetValue("AutoMoveAfterPending", false))
                        {
                            this.MoveToNextDefaultActivity();
                        }
                        else
                        {
                            RegisterAndPrepareActions(this.CurrentActivity.EnterActions);
                        }
                    }

                    WfRuntime.ProcessContext.NormalizeTaskTitles();

                    this.Committed = true;
                    actionContext.AffectedProcesses.AddOrReplace(this);
                }
                finally
                {
                    actionContext.RestoreSavedProcessInfo(state);
                }
            }
        }

        /// <summary>
        /// 退出维护模式
        /// </summary>
        /// <param name="processPendingActivity">是否随之执行ProcessPendingActivity</param>
        public void ExitMaintainingStatus(bool processPendingActivity)
        {
            if (this.Status == WfProcessStatus.Maintaining)
            {
                this.Status = this.Status = this.StatusContext.GetValue("OriginalStatus", WfProcessStatus.Running);
                this.StatusContext.Remove("OriginalStatus");

                if (processPendingActivity)
                    ProcessPendingActivity();

                WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(this);
            }
        }

        /// <summary>
        /// 流程流转
        /// </summary>
        /// <param name="transferParams"></param>
        /// <returns></returns>
        public IWfActivity MoveTo(WfTransferParams transferParams)
        {
            (transferParams.NextActivityDescriptor != null).FalseThrow<ArgumentNullException>("transferParams.NextActivityDescriptor");

            IWfActivity result = InternalMoveTo(transferParams);

            if (result != null && result.Descriptor.Properties.GetValue("AutoMoveTo", false))
                result = MoveToNextDefaultActivity();

            return result;
        }

        /// <summary>
        /// 自动流转到下一个默认的活动点
        /// </summary>
        /// <returns></returns>
        public IWfActivity MoveToNextDefaultActivity()
        {
            if (this.CurrentActivity != null)
            {
                WfTransitionDescriptorCollection toTransitions =
                                this.CurrentActivity.Descriptor.ToTransitions.GetAllCanTransitTransitions();

                //找到缺省的线
                IWfTransitionDescriptor transition = toTransitions.FindDefaultSelectTransition();

                if (transition != null)
                {
                    WfTransferParams transferParams = new WfTransferParams(transition.ToActivity);

                    transferParams.FromTransitionDescriptor = transition;

                    IWfActivity nextActivity = this.Activities.FindActivityByDescriptorKey(transferParams.NextActivityDescriptor.Key);

                    //自动流转点的自动退件
                    if (nextActivity.Status != WfActivityStatus.NotRunning)
                    {
                        WfRuntime.ProcessContext.ActivityChangingContext.CreatorInstanceID = this.CurrentActivity.ID;

                        this.CurrentActivity.CopyMainStreamActivities(nextActivity, null, WfControlOperationType.Return);

                        //ydz: 2013 修改添加完新克隆活动点线的下一个活动点
                        IWfActivity nextCloneActivity = this.Activities.Find(act => string.Equals(act.Descriptor.ClonedKey, transition.ToActivity.Key) == true);

                        if (nextCloneActivity != null)
                            transferParams.NextActivityDescriptor = nextCloneActivity.Descriptor;
                        else
                            transferParams.NextActivityDescriptor = this.CurrentActivity.Descriptor.ToTransitions.FindDefaultSelectTransition().ToActivity;
                    }

                    if (nextActivity.Candidates.Count > 0)
                        transferParams.Assignees.CopyFrom(nextActivity.Candidates);
                    else
                        transferParams.Assignees.Add(transferParams.NextActivityDescriptor.Resources.ToUsers());

                    this.MoveTo(transferParams);
                }
            }

            return this.CurrentActivity;
        }

        private IWfActivity InternalMoveTo(WfTransferParams transferParams)
        {
            WfProcessActionContext actionContext = WfRuntime.ProcessContext;

            WfProcessActionContextState state = actionContext.SaveDifferentProcessInfo(this);
            try
            {
                actionContext.ResetContextByProcess(this);
                WfActivityBase nextActivity = GetNextActivityFromTransferParams(transferParams);
                AdjustTransferParamsFromTemplates(nextActivity.Descriptor, transferParams);

                this.Status = WfProcessStatus.Running;
                nextActivity.StartTime = DateTime.Now;
                nextActivity.FromTransitionDescriptor = transferParams.FromTransitionDescriptor;

                //Set Original Activity Status
                if (this.CurrentActivity != null)
                {
                    ((WfActivityBase)this.CurrentActivity).ToTransitionDescriptor = transferParams.FromTransitionDescriptor;
                    ExecuteOriginalActivityActionsAndSetStatus(this.CurrentActivity, transferParams.Operator);
                }

                WfMoveToEventArgs movetoEventArgs = new WfMoveToEventArgs(this.CurrentActivity, nextActivity, transferParams);

                actionContext.FireBeforeMoveTo(movetoEventArgs);

                //Set CurrentActivity to Next Activity 
                this.CurrentActivity = nextActivity;
                actionContext.ResetContextByProcess(this);

                //是否进入到维护状态
                if (IfIntoMaintainingStatus(nextActivity) == false)
                {
                    //设置下一个节点的状态，包括启动分支流程、启动同类型的串行子流程
                    ExecuteNextActivityActionsAndSetStatus(nextActivity, transferParams);
                }

                //设置流程结束返回状态
                if (transferParams.FromTransitionDescriptor != null)
                    if (transferParams.FromTransitionDescriptor.AffectProcessReturnValue)
                        this.Descriptor.DefaultReturnValue = transferParams.FromTransitionDescriptor.AffectedProcessReturnValue;

                //自动生成每一个点候选人
                if (this.Descriptor.AutoGenerateResourceUsers)
                    this.GenerateCandidatesFromResources();

                actionContext.FireAfterMoveTo(movetoEventArgs);

                WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(this);

                WfSimulator.WriteSimulationInfo(this, WfSimulationOperationType.MoveTo);

                actionContext.ResetContextByProcess(this);

                return nextActivity;
            }
            finally
            {
                actionContext.RestoreSavedProcessInfo(state);
            }
        }

        /// <summary>
        /// 是否进入到维护状态
        /// </summary>
        /// <param name="nextActivity"></param>
        /// <returns></returns>
        private bool IfIntoMaintainingStatus(IWfActivity nextActivity)
        {
            bool result = nextActivity.Descriptor.Properties.GetValue("AutoMaintain", false);

            if (result)
            {
                ((WfActivityBase)nextActivity).Status = WfActivityStatus.Pending;
                this.Status = WfProcessStatus.Maintaining;
            }

            return result;
        }

        public void GenerateCandidatesFromResources()
        {
            PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration(string.Format("GenerateProcessCandidatesFromResources({0})", this.ID),
                () => InnerGenerateCandidatesFromResources());

            PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration(string.Format("GenerateProcessCandidatesFromResources({0})", this.ID),
                () => WfRuntime.DecorateProcess(this));
        }

        /// <summary>
        /// 得到主线的活动点
        /// </summary>
        /// <param name="includeAllElapsedActivities">是否包含所有经过的活动</param>
        public WfMainStreamActivityDescriptorCollection GetMainStreamActivities(bool includeAllElapsedActivities)
        {
            WfMainStreamActivityDescriptorCollection result = null;

            if (this.MainStream != null)
                result = GetProcessInstanceMainStreamActivities(includeAllElapsedActivities);
            else
                result = this.Descriptor.GetMainStreamActivities();

            return result;
        }

        private void InnerGenerateCandidatesFromResources()
        {
            WfProcessActionContext actionContext = WfRuntime.ProcessContext;

            WfProcessActionContextState state = actionContext.SaveDifferentProcessInfo(this);

            try
            {
                actionContext.ResetContextByProcess(this);

                WfMatrix matrix = GetMatrix();

                List<IWfActivity> dynamicActivities = new List<IWfActivity>();

                foreach (IWfActivity activity in this.Activities)
                {
                    if (activity.Status == WfActivityStatus.NotRunning)
                    {
                        if (activity.Descriptor.Properties.GetValue("AutoGenerateCadidates", true))
                        {
                            activity.GenerateCandidatesFromResources();

                            if (activity.Descriptor.Properties.GetValue("IsDynamic", false))
                                dynamicActivities.Add(activity);

                            if (matrix != null)
                                ((WfActivityBase)activity).GenerateCandidatesFromMatrix(matrix);
                        }
                    }
                }

                //生成动态活动
                dynamicActivities.ForEach(activity => ((WfActivityBase)activity).GenerateDynamicActivities());
            }
            finally
            {
                actionContext.RestoreSavedProcessInfo(state);
            }
        }

        /// <summary>
        /// 得到矩阵
        /// </summary>
        /// <returns></returns>
        public WfMatrix GetMatrix()
        {
            WfMatrix matrix = null;

            if (this.Descriptor.Properties.GetValue("UseMatrix", true))
            {
                matrix = WfMatrixAdapter.Instance.GetByProcessKey(this.Descriptor.Key);

                if (matrix != null)
                {
                    if (IsApplicationParametersAvailableForMatrix(matrix))
                    {
                        WfMatrixQueryParamCollection queryParams = new WfMatrixQueryParamCollection(matrix.MatrixID);

                        foreach (WfMatrixDimensionDefinition dd in matrix.Definition.Dimensions)
                        {
                            if (dd.DimensionKey != "OperatorType" && dd.DimensionKey != "Operator" && dd.DimensionKey != "ProcessKey" && dd.DimensionKey != "ActivityKey")
                            {
                                WfMatrixQueryParam qp = new WfMatrixQueryParam();

                                qp.QueryName = dd.DimensionKey;
                                qp.QueryValue = this.ApplicationRuntimeParameters.GetValueRecursively(dd.DimensionKey, string.Empty);

                                queryParams.Add(qp);
                            }
                        }

                        matrix = WfMatrixAdapter.Instance.Query(queryParams);
                    }
                }
            }

            return matrix;
        }

        private bool IsApplicationParametersAvailableForMatrix(WfMatrix matrix)
        {
            bool result = true;

            foreach (WfMatrixDimensionDefinition dd in matrix.Definition.Dimensions)
            {
                if (dd.DimensionKey != "OperatorType" && dd.DimensionKey != "Operator" && dd.DimensionKey != "ProcessKey" && dd.DimensionKey != "ActivityKey")
                {
                    if (this.ApplicationRuntimeParameters.GetValueRecursively(dd.DimensionKey, (string)null) == null)
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 如果自动启动分支流程，则调整分支流程的参数
        /// </summary>
        /// <param name="nextActDesp"></param>
        /// <param name="transferParams"></param>
        private static void AdjustTransferParamsFromTemplates(IWfActivityDescriptor nextActDesp, WfTransferParams transferParams)
        {
            if (transferParams.BranchTransferParams.Count == 0)
            {
                IEnumerable<IUser> candidates = nextActDesp.Instance.Candidates.ToUsers();

                //自动启动子流程
                if (nextActDesp.Properties.GetValue("AutoStartBranchProcesses", false))
                {
                    transferParams.BranchTransferParams.InitFromTemplates(nextActDesp.BranchProcessTemplates);

                    foreach (var btp in transferParams.BranchTransferParams)
                    {
                        if (btp.BranchParams.Count == 0)
                            btp.BranchParams.Add(new WfBranchProcessStartupParams());
                    }
                }

                //自动启动审批流程
                if (nextActDesp.Properties.GetValue("AutoStartBranchApprovalProcess", false))
                {
                    if (nextActDesp.BranchProcessTemplates.ContainsKey(WfTemplateBuilder.AutoStartSubProcessTemplateKey))
                        nextActDesp.BranchProcessTemplates.Remove(t => t.Key == WfTemplateBuilder.AutoStartSubProcessTemplateKey);

                    if (transferParams.BranchTransferParams.ContainsKey(WfTemplateBuilder.AutoStartSubProcessTemplateKey))
                        transferParams.BranchTransferParams.Remove(t => t.Template.Key == WfTemplateBuilder.AutoStartSubProcessTemplateKey);

                    //RoleProperties: 此处待修改，应该从活动点的Resource来生成
                    //沈峥：2012/8/21，改为从活动点定义获取默认审批流的信息
                    IWfBranchProcessTemplateDescriptor template = WfTemplateBuilder.CreateDefaultApprovalTemplate(
                                    WfTemplateBuilder.AutoStartSubProcessTemplateKey,
                                    nextActDesp.Properties.GetValue("AutoStartBranchProcessKey", "DefaultApprovalProcess"),
                                    nextActDesp.Properties.GetValue("AutoStartBranchProcessExecuteSequence", WfBranchProcessExecuteSequence.SerialInSameProcess),
                                    nextActDesp.Properties.GetValue("AutoStartBranchProcessBlockingType", WfBranchProcessBlockingType.WaitAllBranchProcessesComplete),
                                    candidates);

                    template.SubProcessApprovalMode = nextActDesp.Properties.GetValue("SubProcessApprovalMode", WfSubProcessApprovalMode.NoActivityDecide);

                    transferParams.BranchTransferParams.Add(new WfBranchProcessTransferParams(template));
                }
            }
        }

        private WfActivityBase GetNextActivityFromTransferParams(WfTransferParams transferParams)
        {
            WfActivityBase nextActivity = (WfActivityBase)Activities.Find(act => act.Descriptor.Key == transferParams.NextActivityDescriptor.Key);

            (nextActivity != null).FalseThrow<WfDescriptorException>(
                Translator.Translate(WfHelper.CultureCategory, "不能在当前流程中找到下一步的活动点定义{0}",
                transferParams.NextActivityDescriptor.Key));

            return nextActivity;
        }

        /// <summary>
        /// 流转时，对当前流程环节的状态设置和操作执行
        /// </summary>
        private void ExecuteOriginalActivityActionsAndSetStatus(IWfActivity originalActivity, IUser opUser)
        {
            //执行离开活动时的Action
            RegisterAndPrepareActions(originalActivity.LeaveActions);

            ((WfActivityBase)originalActivity).Status = WfActivityStatus.Completed;
            ((WfActivityBase)originalActivity).EndTime = DateTime.Now;

            //设置操作人
            IUser op = opUser;

            //如果不是自动流转才对OP赋值
            if (op == null && DeluxePrincipal.IsAuthenticated && originalActivity.Descriptor.Properties.GetValue("AutoMoveAfterPending", false) == false)
                op = DeluxeIdentity.CurrentUser;

            ((WfActivityBase)originalActivity).Operator = op;

            //记录流程中已经过的节点Key的集合
            if (this.ElapsedActivities.ContainsKey(originalActivity.ID) == false)
                this._ElapsedActivities.Add(originalActivity);
        }

        /// <summary>
        /// 流转时，设置目标节点的状态，包括启动分支流程等操作。
        /// </summary>
        /// <param name="nextActivity"></param>
        /// <param name="transferParams"></param>
        private void ExecuteNextActivityActionsAndSetStatus(WfActivityBase nextActivity, WfTransferParams transferParams)
        {
            PrepareActivityAssigneesFromTransferParams(nextActivity, transferParams);

            CalculateActivityCompleteTime(nextActivity.Descriptor);

            FillAclInContext(this.SearchID, nextActivity);

            //启动分支流程
            transferParams.BranchTransferParams.ForEach(b => nextActivity.StartupBranchProcesses(b));

            if (transferParams.BranchTransferParams.Count == 0 &&
                    WfRuntime.ProcessContext.TargetActivityCanMoveTo &&
                    nextActivity.Descriptor.IsConditionActivity == false)
            {
                nextActivity.Status = WfActivityStatus.Running;
            }
            else
            {
                nextActivity.Status = WfActivityStatus.Pending;
            }

            WfRuntime.ProcessContext.ResetContextByProcess(this);
            RegisterAndPrepareActions(nextActivity.EnterActions);

            if (nextActivity.Descriptor.ActivityType == WfActivityType.CompletedActivity)
            {
                nextActivity.Status = WfActivityStatus.Completed;

                this.Status = WfProcessStatus.Completed;
                this.EndTime = DateTime.Now;

                WfRuntime.ProcessContext.ResetContextByProcess(this);

                this._ElapsedActivities.Add(nextActivity);

                RegisterAndPrepareActions(nextActivity.LeaveActions);

                ExecuteNextSerialProcess(this.EntryInfo);
            }

            SyncUrlsInAssigneesFromTasks(nextActivity, WfRuntime.ProcessContext.MoveToUserTasks);

            this.ProcessPendingActivity();
        }

        /// <summary>
        /// 同步待办中的Url到Assignee的Url中
        /// </summary>
        /// <param name="nextActivity"></param>
        /// <param name="tasks"></param>
        private static void SyncUrlsInAssigneesFromTasks(IWfActivity nextActivity, UserTaskCollection tasks)
        {
            foreach (WfAssignee assignee in nextActivity.Assignees)
            {
                if (assignee.User != null)
                {
                    UserTask task = tasks.Find(t => t.SendToUserID == assignee.User.ID && t.ActivityID == nextActivity.ID);

                    if (task != null)
                        assignee.Url = task.Url;
                }
            }
        }

        /// <summary>
        /// 准备下一步的Assignees
        /// </summary>
        /// <param name="nextActivity"></param>
        /// <param name="transferParams"></param>
        private void PrepareActivityAssigneesFromTransferParams(WfActivityBase nextActivity, WfTransferParams transferParams)
        {
            nextActivity.Assignees.Clear();
            nextActivity.Assignees.CopyFrom(transferParams.Assignees);
            nextActivity.Assignees.CopyFrom(GetDelegatedAssignees(transferParams.Assignees, this));
            nextActivity.Assignees.Distinct((a1, a2) => string.Compare(a1.User.ID, a2.User.ID, true) == 0 && a1.AssigneeType == a2.AssigneeType);
        }

        /// <summary>
        /// 将Activity中的Assignees填充到Acl中
        /// </summary>
        /// <param name="resourceID"></param>
        /// <param name="nextActivity"></param>
        private static void FillAclInContext(string resourceID, WfActivityBase nextActivity)
        {
            if (resourceID.IsNotEmpty())
            {
                WfAclItemCollection acl = nextActivity.Assignees.ToAcl(resourceID, nextActivity.ID);
                WfRuntime.ProcessContext.Acl.CopyFrom(acl);
            }
        }

        /// <summary>
        /// 自动计算活动的完成时间
        /// </summary>
        /// <param name="iWfActivityDescriptor"></param>
        private void CalculateActivityCompleteTime(IWfActivityDescriptor actDesp)
        {
            DateTime endTime = actDesp.EstimateEndTime;

            if (endTime == DateTime.MinValue)
            {
                Decimal duration = actDesp.EstimateDuration;

                if (duration != 0)
                {
                    DateTime startTime = actDesp.EstimateStartTime;

                    if (startTime == DateTime.MinValue)
                        startTime = DateTime.Now;

                    endTime = startTime.AddHours((double)duration);

                    ((WfActivityDescriptor)actDesp).EstimateEndTime = endTime;
                }
            }
        }

        /// <summary>
        /// 得到被委托的人员
        /// </summary>
        /// <param name="sourceAssignees"></param>
        /// <returns></returns>
        private static WfAssigneeCollection GetDelegatedAssignees(WfAssigneeCollection sourceAssignees, IWfProcess process)
        {
            WfAssigneeCollection result = new WfAssigneeCollection();

            foreach (WfAssignee sourceAssignee in sourceAssignees)
            {
                WfDelegationCollection delegations = WfDelegationSettings.GetConfig().Reader.GetUserActiveDelegations(sourceAssignee.User, process);

                foreach (WfDelegation delegation in delegations)
                {
                    OguUser user = new OguUser(delegation.DestinationUserID);

                    user.DisplayName = delegation.DestinationUserName;
                    user.Name = delegation.DestinationUserName;

                    OguUser delegator = new OguUser(delegation.SourceUserID);
                    delegator.DisplayName = delegation.SourceUserName;
                    delegator.Name = delegation.SourceUserName;

                    WfAssignee assignee = new WfAssignee(user);

                    assignee.AssigneeType = WfAssigneeType.Delegated;
                    assignee.Delegator = delegator;

                    result.Add(assignee);
                }
            }

            return result;
        }

        /// <summary>
        /// 执行group中下一个需要顺序执行的流程
        /// </summary>
        /// <param name="group"></param>
        private static void ExecuteNextSerialProcess(IWfBranchProcessGroup group)
        {
            if (group != null && group.ProcessTemplate.ExecuteSequence == WfBranchProcessExecuteSequence.Serial)
            {
                IWfProcess process = group.Branches.Find(p => p.Status == WfProcessStatus.NotRunning);

                if (process != null && process.BranchStartupParams != null)
                {
                    WfTransferParams transferParams = new WfTransferParams(process.Descriptor.InitialActivity);

                    transferParams.Assignees.CopyFrom(process.BranchStartupParams.Assignees);

                    process.MoveTo(transferParams);
                }
            }
        }

        private static void RegisterAndPrepareActions(WfActionCollection actions)
        {
            WfActionParams actionParams = new WfActionParams(WfRuntime.ProcessContext);

            actions.PrepareActions(actionParams);
            WfRuntime.ProcessContext.AffectedActions.CopyFrom(actions);
        }
        #endregion

        #region 非接口属性
        public string OwnerActivityID
        {
            get { return this._OwnerActivityID; }
            set { this._OwnerActivityID = value; }
        }

        public string OwnerTemplateKey
        {
            get { return this._OwnerTemplateKey; }
            set { this._OwnerTemplateKey = value; }
        }
        #endregion

        #region 私有方法
        private static IWfBranchProcessGroup LoadEntryInfo(string ownerActID, string templateKey)
        {
            IWfProcess process = WfRuntime.GetProcessByActivityID(ownerActID);

            IWfActivity activity = process.Activities[ownerActID];

            WfBranchProcessGroup group = null;

            if (activity != null)
            {
                group = (WfBranchProcessGroup)activity.BranchProcessGroups[templateKey];

                if (group != null)
                    group.OwnerActivity = activity;
            }

            return group;
        }
        #endregion

        #region ISimpleXmlSerializer Members

        void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
        {
            element.SetAttributeValue("ID", this.ID);
            element.SetAttributeValue("Status", this.Status.ToString());

            this.Creator.ToSimpleXElement(element, "Creator");
            this.OwnerDepartment.ToSimpleXElement(element, "OwnerDepartment");

            if (this.StartTime != DateTime.MinValue)
                element.SetAttributeValue("StartTime", this.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));

            if (this.EndTime != DateTime.MinValue)
                element.SetAttributeValue("EndTime", this.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));

            ((ISimpleXmlSerializer)this.Descriptor.Properties).ToXElement(element, refNodeName);

            ((ISimpleXmlSerializer)this.Activities).ToXElement(element, refNodeName);

            if (this.ScheduleRootProcess != null)
            {
                element.SetAttributeValue("ScheduleRootProcessID", ScheduleRootProcess.ID);

                if (this.InitialActivity != null)
                {
                    element.SetAttributeValue("ScheduleRootActivityID", InitialActivity.ScheduleRootActivity.ID);
                    element.SetAttributeValue("ScheduleRootActivityName", InitialActivity.ScheduleRootActivity.Descriptor.Name);
                }
            }

            RuntimeParametersToXElement(element);
        }

        #endregion

        private void RuntimeParametersToXElement(XElement element)
        {
            if (this.ApplicationRuntimeParameters.Count > 0)
            {
                XElement rpNode = element.AddChildElement("RuntimeParameters");

                Dictionary<string, string> outputParameterNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                InnerRuntimeParametersToXElement(rpNode, outputParameterNames);
            }
        }

        private void InnerRuntimeParametersToXElement(XElement element, Dictionary<string, string> outputParameterNames)
        {
            foreach (KeyValuePair<string, object> kp in this.ApplicationRuntimeParameters)
            {
                if (outputParameterNames.ContainsKey(kp.Key) == false)
                {
                    if (kp.Value != null)
                    {
                        if (kp.Value is ISimpleXmlSerializer)
                            ((ISimpleXmlSerializer)kp.Value).ToXElement(element, kp.Key);
                        else
                            SetXElementStringValue(element, kp.Key, kp.Value);
                    }

                    outputParameterNames.Add(kp.Key, kp.Key);
                }
            }

            if (this.Descriptor.Properties.GetValue("ProbeParentProcessParams", false) && this.HasParentProcess)
                ((WfProcess)this.EntryInfo.OwnerActivity.Process).InnerRuntimeParametersToXElement(element, outputParameterNames);
        }

        private static void SetXElementStringValue(XElement element, string key, object data)
        {
            try
            {
                element.SetAttributeValue(key, DataConverter.ChangeType(data, typeof(string)));
            }
            catch (System.Exception)
            {
            }
        }

        private IWfProcessDescriptor InitializeMainStream(IWfProcessDescriptor processDesp)
        {
            WfProcessDescriptor clonedProcessDesp = (WfProcessDescriptor)processDesp.Clone();
            clonedProcessDesp.SetProcessInstance(this);
            clonedProcessDesp.Variables.SetValue("MainStream", "True", DataType.Boolean);

            return clonedProcessDesp;
        }

        /// <summary>
        /// 得到流程实例中的主线活动
        /// </summary>
        /// <param name="includeAllElapsedActivities">是否包含所有经过的活动</param>
        /// <returns></returns>
        private WfMainStreamActivityDescriptorCollection GetProcessInstanceMainStreamActivities(bool includeAllElapsedActivities)
        {
            WfMainStreamActivityDescriptorCollection msActivities = new WfMainStreamActivityDescriptorCollection();
            IList<IWfTransitionDescriptor> matchedTransitions = null;

            this.MainStream.ProbeAllActivities(actArgs =>
            {
                matchedTransitions = FindMatchedTransitions(actArgs.ActivityDescriptor, includeAllElapsedActivities);

                WfMainStreamActivityDescriptor msActivity = new WfMainStreamActivityDescriptor(actArgs.ActivityDescriptor);

                msActivity.Level = actArgs.Level;

                //当后面有同样的环节后，用后面的环节覆盖前面已经存在的主线活动
                IWfMainStreamActivityDescriptor existedMSActivity = msActivities[actArgs.ActivityDescriptor.Key];
                if (existedMSActivity != null)
                {
                    if (existedMSActivity.Level < msActivity.Level)
                    {
                        msActivities.Remove(msa => msa.Activity.Key == msActivity.Activity.Key);
                        msActivities.Add(msActivity);
                    }
                }
                else
                    msActivities.Add(msActivity);

                return true;
            },
            transition =>
                matchedTransitions.Exists(t => t == transition)
            );

            //按照遍历的级别深度排序
            msActivities.Sort(new Comparison<IWfMainStreamActivityDescriptor>((msa1, msa2) => msa1.Level - msa2.Level));

            WfMainStreamActivityDescriptorCollection result = ChangeMainStreamActivitiesToInstanceActivities(msActivities);

            FillAsscioatedActivities(result);

            return result;
        }

        private IList<IWfTransitionDescriptor> FindMatchedTransitions(IWfActivityDescriptor actDesp, bool includeAllElapsedActivities)
        {
            List<IWfTransitionDescriptor> result = new List<IWfTransitionDescriptor>();

            //先看看有没有经过的线
            if (includeAllElapsedActivities)
            {
                result.AddRange(actDesp.ToTransitions.FindAll(t => t.IsBackward == false &&
                    this.Activities.Find(act => act.MainStreamActivityKey == t.ToActivity.Key && act.Status != WfActivityStatus.NotRunning) != null));
            }

            IWfTransitionDescriptor exclusiveTransition = GetExclusiveNextTransition(actDesp);

            //如果找到了符合条件的下一条线，且不是已经经过的线，则添加到结果中
            if (exclusiveTransition != null && result.Exists(t => t == exclusiveTransition) == false)
                result.Add(exclusiveTransition);

            return result;
        }

        /// <summary>
        /// 得到唯一的能够流转的前进线。如果没有得到，则获取默认线。
        /// </summary>
        /// <param name="actDesp"></param>
        /// <returns></returns>
        private static IWfTransitionDescriptor GetExclusiveNextTransition(IWfActivityDescriptor actDesp)
        {
            WfTransitionDescriptorCollection forwardCanTrsitTransitions = actDesp.ToTransitions.GetAllCanTransitForwardTransitions();

            IWfTransitionDescriptor result = forwardCanTrsitTransitions.FindDefaultSelectTransition();

            //如果没有前进且能流转的线，查找前进线中的默认线
            if (result == null)
                result = actDesp.ToTransitions.GetAllForwardTransitions().FindDefaultSelectTransition();

            //如果没有前进线中的默认线，则查找全部线（含退回线）中的默认线
            if (result == null)
                result = actDesp.ToTransitions.FindDefaultSelectTransition();

            return result;
        }

        private static bool TransitionCanTransit(IWfTransitionDescriptor transition)
        {
            bool result = false;

            try
            {
                result = transition.CanTransit();
            }
            catch (WfTransitionEvaluationException ex)
            {
                ex.WriteToLog();
            }

            return result;
        }

        /// <summary>
        /// 将主线活动转换为实例活动
        /// </summary>
        /// <param name="msActivities"></param>
        /// <returns></returns>
        private WfMainStreamActivityDescriptorCollection ChangeMainStreamActivitiesToInstanceActivities(WfMainStreamActivityDescriptorCollection msActivities)
        {
            WfMainStreamActivityDescriptorCollection result = new WfMainStreamActivityDescriptorCollection();

            foreach (WfMainStreamActivityDescriptor msActivity in msActivities)
            {
                IList<IWfActivity> matchedActivities = this.Activities.FindAll(activity => activity.MainStreamActivityKey == msActivity.Activity.Key);

                IWfActivity matchedActivity = FindMatchedInstanceActivity(matchedActivities);

                if (matchedActivity != null)
                    result.Add(new WfMainStreamActivityDescriptor(matchedActivity.Descriptor));
            }

            return result;
        }

        /// <summary>
        /// 查找匹配的实例活动，首先是未执行的，其次按照时间
        /// </summary>
        /// <param name="matchedActivities"></param>
        /// <returns></returns>
        private static IWfActivity FindMatchedInstanceActivity(IList<IWfActivity> matchedActivities)
        {
            IWfActivity result = null;
            DateTime maxStartTime = DateTime.MinValue;

            foreach (IWfActivity activity in matchedActivities)
            {
                //不是关联活动或者不是Clone的Key
                if (activity.Descriptor.AssociatedActivityKey.IsNullOrEmpty() || activity.Descriptor.ClonedKey.IsNotEmpty())
                {
                    if (activity.Status == WfActivityStatus.NotRunning)
                    {
                        result = activity;
                        break;
                    }
                    else
                    {
                        if (activity.StartTime >= maxStartTime)
                        {
                            result = activity;
                            maxStartTime = activity.StartTime;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 填充关联活动
        /// </summary>
        /// <param name="matchedActivities"></param>
        private void FillAsscioatedActivities(WfMainStreamActivityDescriptorCollection matchedActivities)
        {
            foreach (IWfMainStreamActivityDescriptor msActivity in matchedActivities)
            {
                IList<IWfActivityDescriptor> actDesps = this.Descriptor.Activities.FindAll(actDesp => actDesp.AssociatedActivityKey == msActivity.Activity.Key);

                msActivity.AssociatedActivities.CopyFrom(actDesps);
            }
        }

        /// <summary>
        /// 设置流程的状态，同时更新父流程中关于子流程的状态信息
        /// </summary>
        /// <param name="status"></param>
        private void ChangeProcessStatus(WfProcessStatus status)
        {
            if (this._Status != status)
            {
                if (this.EntryInfo != null)
                {
                    if (((WfBranchProcessGroup)this.EntryInfo).ChangeProcessStatus(this.ID, this._Status, status))
                        WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(this.EntryInfo.OwnerActivity.Process);
                }

                this._Status = status;

                WfRuntime.ProcessContext.StatusChangedProcesses.AddOrReplace(this);
                RegisterAndPrepareActions(this.ProcessStatusChangeActions);
            }
        }
    }
}
