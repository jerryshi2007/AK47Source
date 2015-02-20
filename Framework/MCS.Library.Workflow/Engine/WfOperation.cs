using System;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using MCS.Library.OGUPermission;
using MCS.Library.Core;
using MCS.Library.Workflow.Engine;
using MCS.Library.Workflow.Descriptors;
using MCS.Library.Workflow.Configuration;

namespace MCS.Library.Workflow.Engine
{
    [Obsolete]
    public enum OpState
    {
        Added,
        Modified,
        Unchanged
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class WfOperation : IWfOperation
    {
		private string _ID = UuidHelper.NewUuidString();
        private IWfOperationDescriptor _Descriptor = null;
        private string _WfOperationDescKey;
        private IWfAnchorActivity _AnchorActivity = null;
        private BranchesOperationalType _OperationalType = BranchesOperationalType.Parallel;
        private WfBranchProcessInfoCollection _Branches = null;
        private State _OpState = State.Added;
        private string _AnchorActivityID;
        private WfOperationContext _Context = null;

        private DataLoadingType _LoadingType = DataLoadingType.Memory;

        public WfOperation(IWfAnchorActivity ownerActivity)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(ownerActivity != null,
                "ownerActivity");

            this.AnchorActivity = ownerActivity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ownerActivity"></param>
        /// <param name="transferParams"></param>
        internal protected WfOperation(IWfAnchorActivity ownerActivity, WfBranchesTransferParams transferParams)
            : this(ownerActivity)
        {
            this.Descriptor = transferParams.OperationDescriptor;
            this.OperationalType = transferParams.OperationalType;
            this.OpState = State.Added;

            //根据分支流程的相关启动参数 启动分支流程 , 形成WfBranchProcessInfo
            InitializeBranchProcesses(ownerActivity, transferParams);
            MoveToBranchProcessesFirstActivity();
        }

        private void MoveToBranchProcessesFirstActivity()
        {
            if (this.OperationalType == BranchesOperationalType.Serial)
            {
                if (this.Branches.Count > 0)
                    MoveToOneBranchProcessFirstActivity(this.Branches[0]);
            }
            else
            {   //并行
                foreach (WfBranchProcessInfo processInfo in this.Branches)
                    MoveToOneBranchProcessFirstActivity(processInfo);
            }
        }

        private void MoveToOneBranchProcessFirstActivity(WfBranchProcessInfo processInfo)
        {
            WfTransferParams initialTransferParams =
                      new WfTransferParams(processInfo.ProcessDescriptor.InitialActivity);

			((WfActivityDescriptor)processInfo.ProcessDescriptor.InitialActivity).LevelName = processInfo.OwnerOperation.AnchorActivity.Descriptor.LevelName;

            initialTransferParams.Receivers.CopyFrom(processInfo.BranchProcessReceiver);

			WfProcess process = ((WfProcess)processInfo.Process);
			process.MoveTo(initialTransferParams);
        }

        private void InitializeBranchProcesses(IWfAnchorActivity ownerActivity, WfBranchesTransferParams transferParams)
        {
            for (int i = 0; i < transferParams.BranchParams.Count; i++)
            {
                WfBranchStartupParams branchParam = (WfBranchStartupParams)transferParams.BranchParams[i];

                InitializeBranchProcess(transferParams.Operator, i, branchParam);
            }
        }

        private WfBranchProcessInfo InitializeBranchProcess(IUser user, int i, WfBranchStartupParams branchParam)
        {
            WfBranchProcessInfo processInfo = new WfBranchProcessInfo(CreateBranchProcess(branchParam.Descriptor));

            processInfo.OwnerOperation = this;
            processInfo.ProcessDescriptor = branchParam.Descriptor;
            processInfo.IsSpecificProcess = branchParam.IsSpecificProcess;
            processInfo.Sequence = i;
            processInfo.BranchProcessReceiver.CopyFrom(branchParam.BranchReceiverResource);
            processInfo.BranchInfoState = State.Added;

            //注意：~~~~~~
            processInfo.Process.ResourceID = this.AnchorActivity.Process.ResourceID;
			processInfo.Process.OwnerDepartment = branchParam.Department;

            ((WfProcess)processInfo.Process).EntryInfo = processInfo;
            ((WfProcess)processInfo.Process).Creator = user;

            this.Branches.Add(processInfo);

            return processInfo;
        }

        protected IWfProcess CreateBranchProcess(IWfProcessDescriptor descriptor)
        {
            WfProcessStartupParams branchProcessStartupParam = new WfProcessStartupParams(descriptor);

            return WfRuntime.StartWorkflow(this.AnchorActivity.Process.GetType(), branchProcessStartupParam);
        }

        public WfOperationContext Context
        {
            get
            {
				if (this._Context == null)
					if (this.LoadingType == DataLoadingType.External)
						this._Context = LoadContext();
					else
						this._Context = new WfOperationContext();

				return this._Context;
            }
        }

        private WfOperationContext LoadContext()
        {
            WfOperationContext operationContext = null;
            IWorkflowReader persistProcess = WorkflowSettings.GetConfig().Reader;

            Dictionary<string, string> context = persistProcess.GetWfContext(this.ID);

			string strContext = string.Empty;

			if (context.TryGetValue(this.ID, out strContext))
                operationContext = SerializationHelper.DeserializeStringToObject<WfOperationContext>(strContext, SerializationFormatterType.Binary);
            else
                operationContext = new WfOperationContext();

            return operationContext;
        }

        public void DoNextBranchProcess(WfBranchProcessInfo nextBranchProcessInfo)
        {
			lock (this)
			{
				CreateBranchProcess(nextBranchProcessInfo.ProcessDescriptor);

				MoveToOneBranchProcessFirstActivity(nextBranchProcessInfo);
			}
        }

        public bool Completed()
        {
            bool flag = false;

            if (this.OperationalType == BranchesOperationalType.Parallel)
                flag = ParallelOperationCompleted();
            else
                flag = this.Branches.AllBranchesCompleted();

            return flag;
        }

        private bool ParallelOperationCompleted()
        {
            bool flag = false;

            switch (this.Descriptor.CompleteCondition)
            {
                case AnchorOperationCompleteCondition.WaitAllBranchProcessesComplete:
                    flag = this.Branches.AllBranchesCompleted();
                    break;

                case AnchorOperationCompleteCondition.WaitAnyoneBranchProcessComplete:
                    flag = this.Branches.AnyoneBranchProcessCompleted();
                    break;

                case AnchorOperationCompleteCondition.WaitNoneOfBranchProcessComplete:
                    flag = true;
                    break;

                case AnchorOperationCompleteCondition.WaitSpecificBranchProcessesComplete:
                    flag = this.Branches.SpecificBranchesProcessCompleted();
                    break;
            }

            return flag;
        }

        public bool CanAutoTransfer()
        {
            return this.Descriptor.AutoTransferWhenCompleted &&
                         this.AnchorActivity.Descriptor.ToTransitions.Count == 1;
        }

        public void AdjustBranches(WfAdjustBranchesParams adjustParams)
        {
			lock (this)
			{
				WfBranchProcessInfoCollection deletedBranchProcesses;

				AddBranchProcess(adjustParams.User, adjustParams.AddedBranchesParamsCollection);

				deletedBranchProcesses = DeleteBranchProcesses(adjustParams.DeletedBranchIDs);

				UpdateBranchesSort(adjustParams.SortBranchParamsList);

				FinalInitBranches();

				this.OpState = State.Modified;

				WorkflowSettings.GetConfig().EnqueueWorkItemExecutor.EnqueueAdjustBranchesWorkItem(this, deletedBranchProcesses);
			}
        }

        private void FinalInitBranches()
        {
            if (OperationalType == BranchesOperationalType.Serial)
            {
                //判断没有正在流转的子流程
                if (HasRunningBranchProcess() == false)
                {
                    //得到 notrunning
                    List<WfBranchProcessInfo> notRunningBranchCollection = GetNotRunningBranchProcesses();

                    if (notRunningBranchCollection.Count > 0)
                    {
                        WfBranchProcessInfo leastBranchInfo = GetLeastBranchInfo(notRunningBranchCollection);

                        MoveToOneBranchProcessFirstActivity(leastBranchInfo);
                    }
                }
            }
        }

        private WfBranchProcessInfo GetLeastBranchInfo(List<WfBranchProcessInfo> notRunningBranchCollection)
        {
            notRunningBranchCollection.Sort();

            return notRunningBranchCollection[0];
        }

        private List<WfBranchProcessInfo> GetNotRunningBranchProcesses()
        {
            List<WfBranchProcessInfo> resultCollection = new List<WfBranchProcessInfo>();

            foreach (WfBranchProcessInfo branchInfo in Branches)
            {
                if (branchInfo.Process.Status == WfProcessStatus.NotRunning)
                    resultCollection.Add(branchInfo);
            }

            return resultCollection;
        }

        private bool HasRunningBranchProcess()
        {
            bool result = false;

            foreach (WfBranchProcessInfo branchInfo in Branches)
            {
                if (branchInfo.Process.Status == WfProcessStatus.Running)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        private WfBranchProcessInfoCollection DeleteBranchProcesses(List<string> DeletedBranchIDs)
        {
            WfBranchProcessInfoCollection deletedBranchProcesses = new WfBranchProcessInfoCollection();
            WfBranchProcessInfo branchInfo;

            for (int i = 0; i < DeletedBranchIDs.Count; i++)
            {
                branchInfo = this.Branches[DeletedBranchIDs[i]];

                branchInfo.BranchInfoState = State.Deleted;

                deletedBranchProcesses.Add(branchInfo);

                this.Branches.Remove(branchInfo);
            }

            return deletedBranchProcesses;
        }

        private void AddBranchProcess(IUser user, WfBranchStartupParamsCollection<WfBranchStartupParams> branchStartupParamses)
        {
            WfBranchProcessInfo processInfo;

            foreach (WfBranchStartupParams branchStartup in branchStartupParamses)
            {
                processInfo = InitializeBranchProcess(user, branchStartup.Sequence, branchStartup);

                if (OperationalType == BranchesOperationalType.Parallel)
                    MoveToOneBranchProcessFirstActivity(processInfo);
            }
        }

        private void UpdateBranchesSort(List<WfSortBranchParams> sortBranchParamsList)
        {
            WfBranchProcessInfo branchInfo;

            foreach (WfSortBranchParams sortBranchParams in sortBranchParamsList)
            {
                branchInfo = this.Branches[sortBranchParams.ProcessID];

                branchInfo.Sequence = sortBranchParams.Sequence;
                branchInfo.BranchInfoState = State.Modified;
            }
        }
        #region Properties

        protected string WfOperationDescKey
        {
            get
            {
                return _WfOperationDescKey;
            }
            set
            {
                _WfOperationDescKey = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public string ID
        {
            get
            {
                return _ID;
            }
            set
            {
                this._ID = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IWfOperationDescriptor Descriptor
        {
            get
            {
				if (this._Descriptor == null)
					this._Descriptor = ((IWfAnchorActivityDescriptor)this.AnchorActivity.Descriptor).Operations[this._WfOperationDescKey];

				return _Descriptor;
            }
            set
            {
                _Descriptor = value;
				if (_Descriptor != null)
					this._WfOperationDescKey = _Descriptor.Key;
				else
					this._WfOperationDescKey = string.Empty;
            }
        }

        public State OpState
        {
            get
            {
                return _OpState;
            }
            set
            {
                _OpState = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IWfAnchorActivity AnchorActivity
        {
            get
            {
				if (_AnchorActivity == null && LoadingType == DataLoadingType.External)
					_AnchorActivity = LoadAnchorActivity();

				return _AnchorActivity;
            }
            set
            {
                _AnchorActivity = value;
            }
        }

        private IWfAnchorActivity LoadAnchorActivity()
        {
            return (IWfAnchorActivity)WfRuntime.GetWfActivity(this._AnchorActivityID);
        }

        /// <summary>
        /// 
        /// </summary>
        public BranchesOperationalType OperationalType
        {
            get
            {
                return _OperationalType;
            }
            set
            {
                _OperationalType = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public WfBranchProcessInfoCollection Branches
        {
            get
            {
                if (this._Branches == null)
                    if (LoadingType == DataLoadingType.External)
                        this._Branches = LoadBranches();
                    else
                        this._Branches = new WfBranchProcessInfoCollection();

                return _Branches;
            }
        }

        public WfAssigneeCollection AutoTransferReceivers
        {
            get
            {
                WfAssigneeCollection receivers = (WfAssigneeCollection)this.Context["AutoTransferReceivers"];

                if (receivers == null)
                {
                    receivers = new WfAssigneeCollection();
                    this.Context["AutoTransferReceivers"] = receivers;
                }

                return receivers;
            }
        }

        private WfBranchProcessInfoCollection LoadBranches()
        {
            WfBranchProcessInfoCollection resultCollection = new WfBranchProcessInfoCollection();

            IWorkflowReader persistProcess = WorkflowSettings.GetConfig().Reader;
            List<string> branchProcessIDs = persistProcess.GetBranchProcessIDsByOperationID(this.ID);

            WfProcessCollection processes = WfRuntime.GetWfProcesses(branchProcessIDs.ToArray());

            IWfProcess[] processesArray = new IWfProcess[processes.Count];

            int i = 0;
            foreach (IWfProcess process in processes)
            {
                ((WfBranchProcessInfo)process.EntryInfo).OwnerOperation = this;
                processesArray[i++] = process;
            }

            //按照子流程的序号排序
            Array.Sort<IWfProcess>(processesArray,
                delegate(IWfProcess process1, IWfProcess process2)
                {
                    return Math.Sign(process1.EntryInfo.Sequence - process2.EntryInfo.Sequence);
                });

            foreach (IWfProcess process in processesArray)
                resultCollection.Add(process.EntryInfo);

            return resultCollection;
        }

        public DataLoadingType LoadingType
        {
            get
            {
                return _LoadingType;
            }
            protected set
            {
                _LoadingType = value;
            }
        }

        protected string AnchorActivityID
        {
            get
            {
                return this._AnchorActivityID;
            }
            set
            {
                this._AnchorActivityID = value;
            }
        }

        #endregion

    }
}
