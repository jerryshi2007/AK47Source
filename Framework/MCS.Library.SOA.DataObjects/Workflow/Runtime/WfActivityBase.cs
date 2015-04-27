using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Expression;
using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects.Workflow.Builders;
using System.Xml.Linq;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    [Serializable]
    [XElementSerializable]
    public abstract partial class WfActivityBase : IWfActivity, ISimpleXmlSerializer
    {
        private string _ID = UuidHelper.NewUuidString();
        private IWfActivityDescriptor _Descriptor;

        private WfActivityContext _Context = null;
        private WfActivityContext _StatusContext = null;
        private WfActivityStatus _Status = WfActivityStatus.NotRunning;
        private IWfProcess _Process = null;

        private DateTime _StartTime = DateTime.MinValue;
        private DateTime _EndTime = DateTime.MinValue;
        private WfAssigneeCollection _Assignees = null;
        private WfAssigneeCollection _Candidates = null;
        private IUser _Operator = null;
        private string _ProcessDescKey = string.Empty;
        private string _ActivityDescKey = string.Empty;
        private DataLoadingType _LoadingType = DataLoadingType.Memory;
        private string _CreatorInstanceID = string.Empty;

        [NonSerialized]
        private WfActionCollection _EnterActions = new WfActionCollection();

        [NonSerialized]
        private WfActionCollection _LeaveActions = new WfActionCollection();

        protected WfActivityBase()
        {
        }

        protected WfActivityBase(IWfActivityDescriptor descriptor)
        {
            _Descriptor = descriptor;

            this._ProcessDescKey = descriptor.Process.Key;
            this._ActivityDescKey = descriptor.Key;
            this._StartTime = DateTime.Now;
        }

        #region IWfActivity Members

        public string ID
        {
            get { return this._ID; }
            set { this._ID = value; }
        }

        public IWfActivityDescriptor Descriptor
        {
            get { return this._Descriptor; }
            set { this._Descriptor = value; }
        }

        public IWfProcess Process
        {
            get { return _Process; }
            set { _Process = value; }
        }

        public WfActivityStatus Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        public DateTime StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }

        public DateTime EndTime
        {
            get { return _EndTime; }
            set { _EndTime = value; }
        }

        public DataLoadingType LoadingType
        {
            get { return _LoadingType; }
            set { _LoadingType = value; }
        }

        public WfActivityContext Context
        {
            get
            {
                if (_Context == null)
                    _Context = new WfActivityContext();

                return _Context;
            }
        }

        public WfActivityContext StatusContext
        {
            get
            {
                if (this._StatusContext == null)
                    this._StatusContext = new WfActivityContext();

                return this._StatusContext;
            }
        }

        /// <summary>
        /// 该环节的候选人
        /// </summary>
        public WfAssigneeCollection Candidates
        {
            get
            {
                if (this._Candidates == null)
                    this._Candidates = new WfAssigneeCollection();

                return this._Candidates;
            }
        }

        public WfAssigneeCollection Assignees
        {
            get
            {
                if (_Assignees == null)
                    _Assignees = new WfAssigneeCollection();

                return _Assignees;
            }
        }

        public IUser Operator
        {
            get
            {
                return _Operator;
            }
            set
            {
                if (value != null)
                {
                    if ((value is OguUser) == false)
                        this._Operator = (IUser)OguUser.CreateWrapperObject(value);
                    else
                        this._Operator = value;
                }
                else
                    this._Operator = value;
            }
        }

        /// <summary>
        /// 是否可以流转走
        /// </summary>
        public bool CanMoveTo
        {
            get
            {
                bool result = true;

                if (WorkflowSettings.GetConfig().UseActivityPlanTime && WfGlobalParameters.GetValueRecursively(
                    this.Descriptor.Process.ApplicationName,
                    this.Descriptor.Process.ProgramName,
                    "UseActivityPlanTime", false))
                {
                    DateTime estimateStartTime = this.Descriptor.Properties.GetValue("EstimateStartTime", DateTime.MinValue);

                    if (estimateStartTime != DateTime.MinValue)
                        result = (DateTime.Compare(DateTime.Now, estimateStartTime) >= 0);
                }

                if (result)
                {
                    result = EvaluateCondition();

                    if (result)
                        result = this.BranchProcessGroups.IsBlocking(
                            this.Descriptor.Properties.GetValue("BlockingType", WfBranchGroupBlockingType.WaitAllBranchGroupsComplete)
                            ) == false;
                }

                return result;
            }
        }

        /// <summary>
        /// 本活动对应着主线活动的描述的Key
        /// </summary>
        public string MainStreamActivityKey
        {
            get;
            set;
        }

        /// <summary>
        /// 得到对应的主线流程活动的描述。如果没有找到，则返回False
        /// </summary>
        /// <returns></returns>
        public IWfActivityDescriptor GetMainStreamActivityDescriptor()
        {
            IWfActivityDescriptor result = null;

            if (this.Process.MainStream != null && this.MainStreamActivityKey.IsNotEmpty())
                result = this.Process.MainStream.Activities[this.MainStreamActivityKey];

            return result;
        }

        /// <summary>
        /// 流程活动的创建者的ID，默认为空。通常是其它活动实例的ID
        /// </summary>
        public string CreatorInstanceID
        {
            get
            {
                return this._CreatorInstanceID;
            }
            set
            {
                this._CreatorInstanceID = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public WfActionCollection EnterActions
        {
            get
            {
                return this._EnterActions;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public WfActionCollection LeaveActions
        {
            get
            {
                return this._LeaveActions;
            }
        }

        public BranchProcessReturnType BranchProcessReturnValue
        {
            get
            {
                return GetBranchProcessReturnValue();
            }
        }

        ///// <summary>
        ///// 取消所有的分支流程
        ///// </summary>
        ///// <param name="recursive">是否递归</param>
        //public void CancelBranchProcesses(bool recursive)
        //{
        //    this.BranchProcessGroups.ForEach(group =>
        //    {
        //        ((WfBranchProcessGroup)group).CheckProcessesStatusInBranches();
        //        ProcessProgress.Current.MaxStep += group.Branches.Count;
        //        ProcessProgress.Current.Response();

        //        group.Branches.ForEach(p =>
        //        {
        //            ((WfProcess)p).InternalCancelProcess(recursive);

        //            ProcessProgress.Current.Increment();
        //            ProcessProgress.Current.Response();
        //        });
        //    });
        //}

        /// <summary>
        /// 取消所有的分支流程
        /// </summary>
        /// <param name="recursive">是否递归</param>
        public void CancelBranchProcesses(bool recursive)
        {
            this.BranchProcessGroups.ForEach(group =>
            {
                ((WfBranchProcessGroup)group).CheckProcessesStatusInBranches();

                IList<string> processIDs = ((WfBranchProcessGroup)group).InternalBranches.FindProcessIDsByStatus(status => status != WfProcessStatus.Aborted);

                ProcessProgress.Current.MaxStep += processIDs.Count;
                ProcessProgress.Current.Response();

                int i = 1;
                processIDs.ForEach(pID =>
                {
                    WfProcess p = (WfProcess)WfRuntime.GetProcessByProcessID(pID);

                    ProcessProgress.Current.StatusText = string.Format("正在作废分支流程{0}/{1}", i, processIDs.Count);
                    ProcessProgress.Current.Response();

                    p.InternalCancelProcess(recursive);

                    i++;
                    ProcessProgress.Current.Increment();
                    ProcessProgress.Current.Response();
                });

                ProcessProgress.Current.StatusText = string.Empty;
                ProcessProgress.Current.Response();
            });
        }

        /// <summary>
        /// 暂停所有分支流程
        /// </summary>
        /// <param name="recursive"></param>
        public void PauseBranchProcesses(bool recursive)
        {
            this.BranchProcessGroups.ForEach(group =>
            {
                group.Branches.ForEach(p =>
                {
                    ((WfProcess)p).InternalPauseProcess(recursive);
                });
            });
        }

        /// <summary>
        /// 恢复所有暂停的分支流程
        /// </summary>
        /// <param name="recursive"></param>
        public void ResumeBranchProcesses(bool recursive)
        {
            this.BranchProcessGroups.ForEach(group =>
            {
                group.Branches.ForEach(p =>
                {
                    ((WfProcess)p).InternalResumeProcess(recursive);
                });
            });
        }

        private BranchProcessReturnType GetBranchProcessReturnValue()
        {
            BranchProcessReturnType returnType = BranchProcessReturnType.AllFalse;

            //是否有真值
            bool hasTrue = false;
            //所有值进行“与”操作
            bool sumflag = true;

            if (this.BranchProcessGroups.Count > 0)
            {
                foreach (IWfBranchProcessGroup group in this.BranchProcessGroups)
                {
                    foreach (IWfProcess branch in group.Branches)
                    {
                        //Added by Fenglilei,20120906
                        if (branch.Status == WfProcessStatus.Aborted || branch.Status == WfProcessStatus.Maintaining)
                            continue;

                        bool value = branch.Descriptor.DefaultReturnValue;

                        if (hasTrue == false)
                            hasTrue = value;

                        if (sumflag)
                            sumflag = value && sumflag;
                    }
                }
            }

            if (sumflag)
            {
                //所有返回值都是真
                returnType = BranchProcessReturnType.AllTrue;
            }
            else
            {
                //存在某些值为真
                if (hasTrue)
                    returnType = BranchProcessReturnType.PartialTrue;
            }

            return returnType;
        }

        /// <summary>
        /// 结束所有分支流程
        /// </summary>
        /// <param name="recursive"></param>
        public void CompleteBranchProcesses(bool recursive)
        {
            this.BranchProcessGroups.ForEach(group =>
            {
                group.Branches.ForEach(p =>
                {
                    if (p.Status == WfProcessStatus.Running)
                        p.CompleteProcess(recursive);
                });
            });
        }

        /// <summary>
        /// 启动分支流程
        /// </summary>
        /// <param name="branchTransferParams">启动分支流程的参数，里面包括一个分支流程模板</param>
        public void StartupBranchProcesses(WfBranchProcessTransferParams branchTransferParams)
        {
            InternalStartupBranchProcesses(branchTransferParams, true);
        }

        /// <summary>
        /// 内部启动分支流程
        /// </summary>
        /// <param name="branchTransferParams">分支流程启动参数</param>
        /// <param name="addToOwnerBranches"></param>
        /// <returns></returns>
        internal WfProcessCollection InternalStartupBranchProcesses(WfBranchProcessTransferParams branchTransferParams, bool addToOwnerBranches)
        {
            WfProcessCollection processes = null;

            if (CanStartBranchProcessFromTemplate(branchTransferParams.Template))
            {
                IWfBranchProcessGroup group = this.BranchProcessGroups[branchTransferParams.Template.Key];

                //以模版Key为分支流程实例的分组标准
                if (group == null)
                {
                    group = new WfBranchProcessGroup(this, branchTransferParams.Template);
                    this.BranchProcessGroups.Add(group);
                }

                WfRuntime.ProcessContext.FirePrepareBranchProcessParams(group, branchTransferParams.BranchParams);

                ProcessProgress.Current.MaxStep += branchTransferParams.BranchParams.Count;
                ProcessProgress.Current.Response();

                //根据模板内的分支流程参数，逐一启动具体流程
                for (int i = 0; i < branchTransferParams.BranchParams.Count; i++)
                {
                    //processes = StartupBranchProcess(branchTransferParams, group, group.BranchProcessStatistics.MaxSequence++);
                    processes = StartupBranchProcess(branchTransferParams, group, i);

                    processes.ForEach(p =>
                    {
                        if (addToOwnerBranches)
                        {
                            if (group.Branches.Contains(p) == false)
                            {
                                group.Branches.Add(p);
                            }
                        }

                        WfRuntime.ProcessContext.FireAfterStartupBranchProcess(p);
                    });

                    ProcessProgress.Current.MaxStep = Math.Max(ProcessProgress.Current.MaxStep, ProcessProgress.Current.CurrentStep + processes.Count);
                    ProcessProgress.Current.IncrementBy(processes.Count);
                    ProcessProgress.Current.Response();
                }

                WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(this.Process);
            }

            if (processes == null)
                processes = new WfProcessCollection();

            return processes;
        }

        private static bool CanStartBranchProcessFromTemplate(IWfBranchProcessTemplateDescriptor template)
        {
            bool result = false;

            try
            {
                result = template.CanStart();
            }
            catch (WfConditionEvaluationException ex)
            {
                ex.WriteToLog();
            }

            return result;
        }

        /// <summary>
        /// 启动一个模版（组）下的一组分支流程，在服务模式下，可能返回多个流程。其它情况返回单个流程
        /// </summary>
        /// <param name="branchTransferParams"></param>
        /// <param name="group"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private WfProcessCollection StartupBranchProcess(WfBranchProcessTransferParams branchTransferParams, IWfBranchProcessGroup group, int index)
        {
            WfProcessCollection processes = null;

            if (branchTransferParams.Template.OperationDefinition == null)
            {
                WfProcessStartupParams startupParams = PrepareOneBranchProcessStartupParams(branchTransferParams, group, index);

                startupParams.CheckStartProcessUserPermission = false;

                processes = new WfProcessCollection();
                processes.Add(WfRuntime.StartWorkflow(startupParams));
            }
            else
            {
                if (WfRuntime.ProcessContext.EnableServiceCall)
                {
                    WfServiceStartupProcessParams startupParams = PrepareOneServiceStartupProcessParams(branchTransferParams, group, index);

                    processes = InvokeBranchProcess(startupParams, branchTransferParams.Template);
                }
                else
                    processes = new WfProcessCollection();
            }

            return processes;
        }

        /// <summary>
        /// 调用外部服务。对方会返回一个或一组流程ID。本函数返回这组ID对应的流程。
        /// </summary>
        /// <param name="startupParams"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        private WfProcessCollection InvokeBranchProcess(WfServiceStartupProcessParams startupParams, IWfBranchProcessTemplateDescriptor template)
        {
            this.Process.ApplicationRuntimeParameters["serviceOP_Paramas"] = startupParams;

            WfServiceInvoker svcInvoker = new WfServiceInvoker(template.OperationDefinition);

            object obj = svcInvoker.Invoke();

            WfProcessCollection processes = new WfProcessCollection();
            WfBranchProcessTemplateDescriptor temp = template as WfBranchProcessTemplateDescriptor;

            IList array = obj as IList;

            if (array != null)
            {
                IWfProcess process = null;
                foreach (string processID in array)
                {
                    process = WfRuntime.GetProcessByProcessID(processID);
                    processes.Add(process);
                }

                temp.BranchProcessKey = process.Descriptor.Key;
            }
            else
            {
                IWfProcess process = WfRuntime.GetProcessByProcessID(obj.ToString());
                processes.Add(process);
                temp.BranchProcessKey = process.Descriptor.Key;
            }

            return processes;
        }

        private WfProcessStartupParams PrepareOneBranchProcessStartupParams(WfBranchProcessTransferParams branchTransferParams, IWfBranchProcessGroup group, int index)
        {
            IWfBranchProcessTemplateDescriptor template = branchTransferParams.Template;
            WfBranchProcessStartupParams branchStartupParams = branchTransferParams.BranchParams[index];

            //准备启动子流程的参数
            //先检查有没有预定义好的分支流程
            IWfProcessDescriptor subProcessDesp = null;

            using (WfApplicationParametersContext apContext = WfApplicationParametersContext.CreateContext(branchStartupParams.ApplicationRuntimeParameters))
            {
                subProcessDesp = template.GetBranchProcessDescriptor();
            }

            if (template.DefaultProcessName.IsNotEmpty())
            {
                string runtimeProcessName = this.Process.ApplicationRuntimeParameters.GetMatchedString(template.DefaultProcessName);

                if (runtimeProcessName.IsNullOrEmpty())
                    runtimeProcessName = template.DefaultProcessName;

                if (runtimeProcessName.IsNullOrEmpty())
                    ((WfProcessDescriptor)subProcessDesp).Name = runtimeProcessName;

                if (subProcessDesp.InitialActivity != null && subProcessDesp.InitialActivity.Name.IsNullOrEmpty())
                    ((WfActivityDescriptor)subProcessDesp.InitialActivity).Name = runtimeProcessName;
            }

            if (template.DefaultTaskTitle.IsNotEmpty())
                subProcessDesp.DefaultTaskTitle = template.DefaultTaskTitle;

            if (template.DefaultUrl.IsNotEmpty())
                subProcessDesp.Url = template.DefaultUrl;

            if (subProcessDesp.Url.IsNullOrEmpty())
                subProcessDesp.Url = this.Descriptor.Url.IsNotEmpty() ? this.Descriptor.Url : this.Descriptor.Process.Url;

            WfProcessStartupParams startupParams = new WfProcessStartupParams();

            startupParams.Creator = this.Operator;

            if (branchStartupParams.Department == null)
                startupParams.Department = this.Process.OwnerDepartment;
            else
                startupParams.Department = branchStartupParams.Department;

            startupParams.Assignees.CopyFrom(branchStartupParams.Assignees);
            startupParams.ProcessDescriptor = subProcessDesp;

            if (branchStartupParams.DefaultTaskTitle.IsNullOrEmpty())
            {
                if (startupParams.DefaultTaskTitle.IsNullOrEmpty())
                {
                    if (subProcessDesp.DefaultTaskTitle.IsNotEmpty())
                        startupParams.DefaultTaskTitle = subProcessDesp.DefaultTaskTitle;
                    else
                        startupParams.DefaultTaskTitle = this.Process.Descriptor.DefaultTaskTitle;
                }
                else
                    startupParams.DefaultTaskTitle = this.Descriptor.TaskTitle;
            }
            else
                startupParams.DefaultTaskTitle = branchStartupParams.DefaultTaskTitle;

            if (branchStartupParams.ResourceID.IsNullOrEmpty())
            {
                WfSubProcessResourceMode mode = template.Properties.GetValue("CreateResourceMode", WfSubProcessResourceMode.DependsOnProcess);
                switch (mode)
                {
                    case WfSubProcessResourceMode.DependsOnProcess:
                        if (subProcessDesp.Properties.GetValue("Independent", false))
                            startupParams.ResourceID = UuidHelper.NewUuidString();
                        else
                            startupParams.ResourceID = this.Process.ResourceID;
                        break;
                    case WfSubProcessResourceMode.SameWithRoot:
                        startupParams.ResourceID = this.Process.ResourceID;
                        break;
                    case WfSubProcessResourceMode.NewCreate:
                        startupParams.ResourceID = UuidHelper.NewUuidString();
                        break;
                }

            }
            else
                startupParams.ResourceID = branchStartupParams.ResourceID;

            if (branchStartupParams.RelativeParams.Count == 0)
                startupParams.RelativeParams.CopyFrom(this.Process.RelativeParams);
            else
                startupParams.RelativeParams.CopyFrom(branchStartupParams.RelativeParams);

            startupParams.BranchStartupParams = branchStartupParams;
            startupParams.Group = group;
            startupParams.OwnerActivityID = this.ID;
            startupParams.OwnerTemplateKey = template.Key;
            startupParams.Sequence = index;
            startupParams.AutoCommit = this.Process.Committed;  //子流程的提交属性默认等于主流程的

            branchStartupParams.ApplicationRuntimeParameters.ForEach(kp => startupParams.ApplicationRuntimeParameters.Add(kp.Key, kp.Value));

            //如果是串行执行，且不是第一个流程，则不启动第一个活动，子流程处于未启动状态
            if (template.ExecuteSequence == WfBranchProcessExecuteSequence.Serial && index > 0)
                startupParams.AutoStartInitialActivity = false;

            return startupParams;
        }

        private WfServiceStartupProcessParams PrepareOneServiceStartupProcessParams(WfBranchProcessTransferParams branchParams, IWfBranchProcessGroup group, int index)
        {
            IWfBranchProcessTemplateDescriptor template = branchParams.Template;
            WfBranchProcessStartupParams branchStartupParams = branchParams.BranchParams[index];

            WfServiceStartupProcessParams startupParams = new WfServiceStartupProcessParams();

            startupParams.Creator = this.Process.Creator;

            if (branchStartupParams.Department == null)
                startupParams.Department = this.Process.OwnerDepartment;
            else
                startupParams.Department = branchStartupParams.Department;

            startupParams.Assignees = branchStartupParams.Assignees.ToUsers();

            if (branchStartupParams.DefaultTaskTitle.IsNullOrEmpty())
            {
                if (startupParams.DefaultTaskTitle.IsNullOrEmpty())
                    startupParams.DefaultTaskTitle = this.Process.Descriptor.DefaultTaskTitle;
                else
                    startupParams.DefaultTaskTitle = this.Descriptor.TaskTitle;
            }
            else
                startupParams.DefaultTaskTitle = branchStartupParams.DefaultTaskTitle;

            if (branchStartupParams.ResourceID.IsNullOrEmpty())
            {
                WfSubProcessResourceMode mode = template.Properties.GetValue("CreateResourceMode", WfSubProcessResourceMode.DependsOnProcess);
                switch (mode)
                {
                    //这里的规则是什么？
                    case WfSubProcessResourceMode.DependsOnProcess:
                        startupParams.ResourceID = this.Process.ResourceID;
                        break;
                    case WfSubProcessResourceMode.SameWithRoot:
                        startupParams.ResourceID = this.Process.ResourceID;
                        break;
                    case WfSubProcessResourceMode.NewCreate:
                        startupParams.ResourceID = UuidHelper.NewUuidString();
                        break;
                }

            }
            else
                startupParams.ResourceID = branchStartupParams.ResourceID;

            startupParams.OwnerActivityID = this.ID;
            startupParams.OwnerTemplateKey = template.Key;
            startupParams.Sequence = index;
            startupParams.RelativeParams = new Dictionary<object, object>();

            foreach (var strpar in branchStartupParams.RelativeParams.AllKeys)
            {
                startupParams.RelativeParams.Add(strpar, this.Process.ApplicationRuntimeParameters[branchStartupParams.RelativeParams[strpar]]);
            }

            if (!string.IsNullOrEmpty(template.DefaultUrl))
                startupParams.DefaultURL = template.DefaultUrl;
            else if (!string.IsNullOrEmpty(this.Descriptor.Url))
                startupParams.DefaultURL = this.Descriptor.Url;
            else
                startupParams.DefaultURL = this.Process.Descriptor.Url;

            //如果是串行执行，且不是第一个流程，则不启动第一个活动，子流程处于未启动状态
            if (template.ExecuteSequence == WfBranchProcessExecuteSequence.Serial && index > 0)
                startupParams.AutoStartInitialActivity = false;

            return startupParams;
        }

        public void GenerateCandidatesFromResources()
        {
            WfAssigneeCollection candidates = this.Descriptor.Resources.ToAssignees();

            //根据是否允许多个候选人，设置Selected属性
            if (this.Descriptor.Properties.GetValue("AllowAssignToMultiUsers", true) == false)
            {
                //先查找原来选中的
                WfAssignee originalSelected = this.Candidates.Find(a => a.Selected);

                candidates.ForEach(a => a.Selected = false);

                WfAssignee matchedSelected = null;

                if (originalSelected != null)
                    matchedSelected = candidates.Find(a => string.Compare(a.User.ID, originalSelected.User.ID, true) == 0);

                if (matchedSelected != null)
                {
                    matchedSelected.Selected = true;
                }
                else
                {
                    if (candidates.Count > 0)
                        candidates[0].Selected = true;
                }
            }

            this.Candidates.Clear();
            this.Candidates.CopyFrom(candidates);

            this.Candidates.Distinct((a1, a2) => string.Compare(a1.User.ID, a2.User.ID, true) == 0 && a1.AssigneeType == a2.AssigneeType);
        }

        internal void GenerateCandidatesFromMatrix(WfMatrix matrix)
        {
            WfMatrixRowCollection rows = FilterRowsByActivity(matrix);

            WfMatrixRowUsersCollection rowsUsers = rows.GenerateRowsUsers();

            foreach (WfMatrixRowUsers ru in rowsUsers)
            {
                this.Candidates.Add(ru.Users);
            }

            Candidates.Distinct((a1, a2) => string.Compare(a1.User.ID, a2.User.ID, true) == 0 && a1.AssigneeType == a2.AssigneeType);
        }

        private WfMatrixRowCollection FilterRowsByActivity(WfMatrix matrix)
        {
            WfMatrixRowCollection result = new WfMatrixRowCollection();

            if (matrix.Definition.Dimensions.ContainsKey("ActivityKey"))
            {
                foreach (WfMatrixRow row in matrix.Rows)
                {
                    if (string.Compare(row.Cells.GetValue("ActivityKey", string.Empty), this.Descriptor.Key, true) == 0)
                    {
                        result.Add(row);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 流转时该活动从哪一根线进入的
        /// </summary>
        public IWfTransitionDescriptor FromTransitionDescriptor
        {
            get
            {
                IWfTransitionDescriptor result = null;

                string transitionKey = this.Context.GetValue("FromTransitionDescriptor", string.Empty);

                if (transitionKey.IsNotEmpty())
                    result = this.Descriptor.Process.FindTransitionByKey(transitionKey);

                return result;
            }
            set
            {
                string transitionKey = string.Empty;

                if (value != null)
                    transitionKey = value.Key;

                this.Context["FromTransitionDescriptor"] = transitionKey;
            }
        }

        /// <summary>
        /// 从哪条线流转出去。有可能为null，例如结束点或者没有走到的点。如果活动被撤回，这个属性保持最后一次流转的线
        /// </summary>
        public IWfTransitionDescriptor ToTransitionDescriptor
        {
            get
            {
                IWfTransitionDescriptor result = null;

                string transitionKey = this.Context.GetValue("ToTransitionDescriptor", string.Empty);

                if (transitionKey.IsNotEmpty())
                    result = this.Descriptor.Process.FindTransitionByKey(transitionKey);

                return result;
            }
            set
            {
                string transitionKey = string.Empty;

                if (value != null)
                    transitionKey = value.Key;

                this.Context["ToTransitionDescriptor"] = transitionKey;
            }
        }

        private WfBranchProcessGroupCollection _BranchProcessGroups = null;

        public WfBranchProcessGroupCollection BranchProcessGroups
        {
            get
            {
                if (this._BranchProcessGroups == null)
                    this._BranchProcessGroups = new WfBranchProcessGroupCollection();
                else
                    this._BranchProcessGroups.ForEach(group => (group as WfBranchProcessGroup).OwnerActivity = this);

                return this._BranchProcessGroups;
            }
        }

        public IWfActivity RootActivity
        {
            get
            {
                IWfActivity result = this;

                while (result.Process.EntryInfo != null)
                {
                    result = result.Process.EntryInfo.OwnerActivity;
                }

                return result;
            }
        }

        public IWfActivity SameResourceRootActivity
        {
            get
            {
                IWfActivity result = this;
                string resourceID = this.Process.ResourceID;

                while (result.Process.EntryInfo != null &&
                    result.Process.EntryInfo.OwnerActivity.Process.ResourceID == resourceID)
                {
                    result = result.Process.EntryInfo.OwnerActivity;
                }

                return result;
            }
        }

        /// <summary>
        /// 审批流的根活动
        /// </summary>
        public IWfActivity ApprovalRootActivity
        {
            get
            {
                IWfActivity result = this;

                while (result.Process.EntryInfo != null &&
                    result.Process.Descriptor.Properties.GetValue("Independent", true) == false
                    &&
                    result.Process.EntryInfo.OwnerActivity.Process.Descriptor.ProcessType == WfProcessType.Approval)
                {
                    result = result.Process.EntryInfo.OwnerActivity;
                }

                return result;
            }
        }

        /// <summary>
        /// 计划（业务）流程的根活动。如果当前流程是计划（业务）流程，则立即返回。如果都不是，则返回根流程的活动
        /// </summary>
        public IWfActivity ScheduleRootActivity
        {
            get
            {
                IWfActivity result = this;

                if (this.Process.Descriptor.ProcessType != WfProcessType.Schedule)
                {
                    while (result.Process.EntryInfo != null)
                    {
                        result = result.Process.EntryInfo.OwnerActivity;

                        if (result.Process.Descriptor.ProcessType == WfProcessType.Schedule)
                            break;
                    }
                }

                return result;
            }
        }

        public IWfActivity OpinionRootActivity
        {
            get
            {
                IWfActivity result = this;

                if (this.Process.EntryInfo != null)
                {
                    WfOpinionMode currentOpinionMode = this.Process.EntryInfo.ProcessTemplate.Properties.GetValue("IndependentOpinion", WfOpinionMode.Default);
                    switch (currentOpinionMode)
                    {
                        case WfOpinionMode.Default:
                            result = this.ApprovalRootActivity;
                            break;
                        case WfOpinionMode.Independent:
                            result = this;
                            break;
                        case WfOpinionMode.Merged:
                            result = this.Process.EntryInfo.OwnerActivity;
                            break;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// 在当前活动之前插入活动。当前活动不能是起始点。
        /// </summary>
        /// <param name="actDesp"></param>
        public IWfActivity InsertBefore(IWfActivityDescriptor actDesp)
        {
            this.Descriptor.InsertBefore(actDesp);

            IWfActivity newActivity = CreateActivityInstance(actDesp, this.Process);

            if (newActivity.Descriptor.ActivityType == WfActivityType.InitialActivity)
                ((WfProcess)this.Process).InitialActivity = newActivity;

            return newActivity;
        }

        /// <summary>
        /// 在当前活动实例后，添加一个新的活动
        /// </summary>
        /// <param name="actDesp"></param>
        /// <returns></returns>
        public IWfActivity Append(IWfActivityDescriptor newActDesp)
        {
            return Append(newActDesp, false);
        }

        /// <summary>
        /// 在当前活动之后添加活动。当前活动不能是结束点。
        /// 自动创建一条当前点和新节点的连线，而当前点的向前的出线，将作为新点的出线。
        /// </summary>
        /// <param name="newActDesp">需要添加的新活动</param>
        /// <param name="moveReturnLine">是否将当前点的退回线也复制到新的活动之后</param>
        public IWfActivity Append(IWfActivityDescriptor newActDesp, bool moveReturnLine)
        {
            this.Descriptor.Append(newActDesp, moveReturnLine);

            return CreateActivityInstance(newActDesp, this.Process);
        }

        public static WfActivityBase CreateActivityInstance(IWfActivityDescriptor actDesp, IWfProcess process)
        {
            WfActivityBase act = WfActivitySettings.GetConfig().GetActivityBuilder(actDesp).CreateActivity(actDesp);
            act.Process = process;
            process.Activities.Add(act);

            act.CreatorInstanceID = WfRuntime.ProcessContext.ActivityChangingContext.CreatorInstanceID;

            return act;
        }

        private IWfActivityDescriptor LastCloneActivity(IWfActivityDescriptor activity)
        {
            var acts = this.Process.Descriptor.Activities;

            IWfActivityDescriptor result = activity;

            foreach (var act in acts)
            {
                if (activity.Key == act.AssociatedActivityKey)
                {
                    result = act;
                    //if (act.Instance.Status == WfActivityStatus.NotRunning)
                    //    break;
                }
            }

            return result;
        }

        //private bool CanTransit(IWfActivity targetActivity)
        //{
        //    //广度遍历。。。找最近的目标点。
        //    bool result = false;
        //    string key= targetActivity.Descriptor.Key;
        //    if (targetActivity.Descriptor.AssociatedActivityKey != null)
        //        key = targetActivity.Descriptor.AssociatedActivityKey;
        //    foreach (var trans in this.Descriptor.ToTransitions)
        //    {

        //        if (trans.ToActivity.Key == key)
        //        {
        //            result = true;
        //            break;
        //        }
        //    }
        //    return result;
        //}

        private static IEnumerable<WfResourceDescriptor> GetResources(WfAssigneeCollection wfAssignees)
        {
            List<WfResourceDescriptor> list = new List<WfResourceDescriptor>();

            foreach (WfAssignee item in wfAssignees)
            {
                list.Add(new WfUserResourceDescriptor(item.User));
            }

            return list;
        }

        /// <summary>
        /// 删除自身实例，同时删除所有进出线
        /// </summary>
        public void Remove()
        {
            (this.Descriptor.ActivityType != WfActivityType.InitialActivity).FalseThrow("起始活动不可被删除");
            (this.Descriptor.ActivityType != WfActivityType.CompletedActivity).FalseThrow("结束活动不可被删除");

            this.InternalRemove();
        }

        /// <summary>
        /// 删除自身实例，同时删除所有进出线，不进行校验
        /// </summary>
        internal void InternalRemove()
        {
            this.CancelBranchProcesses(true);

            this.Descriptor.Remove();

            this.Process.Activities.Remove(act => string.Compare(act.ID, this.ID, true) == 0);

            if (this._Process.CurrentActivity == this)
            {
                ((WfProcess)this._Process).CurrentActivity = this._Process.ElapsedActivities[this._Process.ElapsedActivities.Count - 1];
                this._Process.ElapsedActivities.RemoveAt(this._Process.ElapsedActivities.Count - 1);
            }
            else
            {
                this._Process.ElapsedActivities.Remove(act => string.Compare(act.ID, this.ID, true) == 0);
            }
        }

        /// <summary>
        /// 删除自身实例
        /// </summary>
        public void Delete()
        {
            (this.Descriptor.ActivityType != WfActivityType.InitialActivity).FalseThrow("起始活动不可被删除");
            (this.Descriptor.ActivityType != WfActivityType.CompletedActivity).FalseThrow("结束活动不可被删除");

            this.CancelBranchProcesses(true);

            this.Descriptor.Delete();

            this.Process.Activities.Remove(act => string.Compare(act.ID, this.ID, true) == 0);

            if (this._Process.CurrentActivity == this)
            {
                ((WfProcess)this._Process).CurrentActivity = this._Process.ElapsedActivities[this._Process.ElapsedActivities.Count - 1];
                this._Process.ElapsedActivities.RemoveAt(this._Process.ElapsedActivities.Count - 1);
            }
            else
            {
                this._Process.ElapsedActivities.Remove(act => string.Compare(act.ID, this.ID, true) == 0);
            }
        }

        #endregion

        #region Private
        private bool EvaluateCondition()
        {
            bool result = true;

            if (this.Descriptor.Condition.IsEmpty == false)
            {
                try
                {
                    result = this.Descriptor.Condition.Evaluate(new CalculateUserFunction(WfRuntime.ProcessContext.FireEvaluateActivityCondition));
                }
                catch (System.Exception ex)
                {
                    throw new WfActivityEvaluationException(string.Format("判断活动{0}的条件，{1}", this.Descriptor.Key, ex.Message), this.Descriptor.Condition);
                }
            }

            return result;
        }
        #endregion

        #region ISimpleXmlSerializer Members

        void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
        {
            element.SetAttributeValue("ID", this.ID);
            element.SetAttributeValue("Status", this.Status.ToString());

            if (this.StartTime != DateTime.MinValue)
                element.SetAttributeValue("StartTime", this.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));

            if (this.EndTime != DateTime.MinValue)
                element.SetAttributeValue("EndTime", this.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));

            ((ISimpleXmlSerializer)this.Descriptor).ToXElement(element, refNodeName);

            this.Assignees.ToSimpleXElement(element, "Assignees");
            this.Candidates.ToSimpleXElement(element, "Candidates");

            IWfActivity sceduleRootAct = this.ScheduleRootActivity;

            if (sceduleRootAct != null)
            {
                element.SetAttributeValue("ScheduleRootProcessID", sceduleRootAct.Process.ID);

                element.SetAttributeValue("ScheduleRootActivityID", sceduleRootAct.ID);
                element.SetAttributeValue("ScheduleRootActivityName", sceduleRootAct.Descriptor.Name);
            }
        }

        #endregion
    }
}
