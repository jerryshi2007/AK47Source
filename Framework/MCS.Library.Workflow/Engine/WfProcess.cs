using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Runtime.Serialization;
using MCS.Library.Core;
using MCS.Library.Workflow.Descriptors;
using MCS.Library.Principal;
using MCS.Library.OGUPermission;
using MCS.Library.Workflow.Properties;
using MCS.Library.Workflow.Configuration;
using MCS.Library.Workflow.OguObjects;

namespace MCS.Library.Workflow.Engine
{
	/// <summary>
	/// 流程实例
	/// </summary>
	[Serializable]
	public class WfProcess : IWfProcess, ISerializable
	{
		private string _ID = UuidHelper.NewUuidString();

		private WfActivityCollection _Activities = null;
		private WfProcessContext _Context = null;
        private string _ResourceID = string.Empty;

		protected WfProcessStatus _Status = WfProcessStatus.NotRunning;
		protected DateTime _StartTime = DateTime.MinValue;
		protected DateTime _EndTime = DateTime.MinValue;
		protected IUser _Creator = null;
		protected IOrganization _OwnerDepartment;
		protected WfBranchProcessInfo _EntryInfo = null;

		private IWfFactory _Factory = null;
		internal Dictionary<string, IWfProcessDescriptor> Descriptors = new Dictionary<string, IWfProcessDescriptor>();
	
		protected DataLoadingType _LoadingType = DataLoadingType.Memory;

		#region ISerializable 成员

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ID", this._ID);
			info.AddValue("ResourceID", this._ResourceID);
			info.AddValue("Activities", this._Activities, typeof(WfActivityCollection));
			info.AddValue("Context", _Context, typeof(WfProcessContext));

			info.AddValue("Status", this._Status, typeof(WfProcessStatus));
			info.AddValue("StartTime", this._StartTime);
			info.AddValue("EndTime", this._EndTime);

			info.AddValue("Creator", this._Creator);
			info.AddValue("OwnerDepartment", this._OwnerDepartment);

			info.AddValue("EntryInfo", this._EntryInfo);
			info.AddValue("LoadingType", this._LoadingType);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected WfProcess(SerializationInfo info, StreamingContext context)
		{
			this._ID = info.GetString("ID");
			this._ResourceID = info.GetString("ResourceID");
			this._Activities = (WfActivityCollection)info.GetValue("Activities", typeof(WfActivityCollection));
			this._Context = (WfProcessContext)info.GetValue("Context", typeof(WfProcessContext));

			this._Status = (WfProcessStatus)info.GetValue("Status", typeof(WfProcessStatus));
			this._StartTime = info.GetDateTime("StartTime");
			this._EndTime = info.GetDateTime("EndTime");
			this._Creator = (IUser)info.GetValue("Creator", typeof(IUser));
			this._OwnerDepartment = (IOrganization)info.GetValue("OwnerDepartment", typeof(IOrganization));

			this._EntryInfo = (WfBranchProcessInfo)info.GetValue("EntryInfo", typeof(WfBranchProcessInfo));
			this._LoadingType = (DataLoadingType)info.GetValue("LoadingType", typeof(DataLoadingType));

			WfProcessContextCache.Instance[this._ID] = this;
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		public WfProcess()
		{
			this._StartTime = DateTime.Now;
		}

		#region 公共属性

		/// <summary>
		/// 流程的ID
		/// </summary>
		public string ID
		{
			get
			{
				return _ID;
			}
			protected set
			{
				_ID = value;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        public string ResourceID
        {
            get
            {
                return this._ResourceID;
            }
            set
            {
                this._ResourceID = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		public WfProcessStatus Status
		{
			get
			{
				return _Status;
			}
			internal set
			{
				this._Status = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public DateTime StartTime
		{
			get
			{
				return this._StartTime;
			}
			internal set
			{
				this._StartTime = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public DateTime EndTime
		{
			get
			{
				return this._EndTime;
			}
			internal set
			{
				this._EndTime = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public IUser Creator
		{
			get
			{
				return _Creator;
			}
			set
			{
				if (value != null)
				{
					if ((value is WfOguUser) == false)
						this._Creator = new WfOguUser(value);
					else
						this._Creator = value;
				}
				else
					this._Creator = value;
			}
		}

		/// <summary>
		/// 创建流程的部门
		/// </summary>
		public IOrganization OwnerDepartment
		{
			get
			{
				return this._OwnerDepartment;
			}
			set
			{
				if (value != null)
				{
					if ((value is WfOguOrganization) == false)
						this._OwnerDepartment = new WfOguOrganization(value);
					else
						this._OwnerDepartment = value;
				}
				else
					this._OwnerDepartment = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public IWfActivity FirstActivity
		{
			get
			{
				IWfActivity activity = null;

				if (Activities.Count > 0)
					activity = Activities[0];

				return activity;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public IWfActivity LastActivity
		{
			get
			{
				IWfActivity activity = null;

				if (Activities.Count > 0)
					activity = Activities[Activities.Count - 1];

				return activity;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public IWfActivity CurrentActivity
		{
			get
			{
				return LastActivity;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public WfActivityCollection Activities
		{
			get
			{
				if (_Activities == null)
					if (LoadingType == DataLoadingType.External)
						_Activities = LoadActivities();
					else
						_Activities = new WfActivityCollection();

				return _Activities;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public WfProcessContext Context
		{
			get
			{
				if (_Context == null)
					if (LoadingType == DataLoadingType.External)
						_Context = LoadContext();
					else
						_Context = new WfProcessContext();

				return _Context;
			}
		}

		public DataLoadingType LoadingType
		{
			get
			{
				return _LoadingType;
			}
		}

		private WfActivityCollection LoadActivities()
		{
			IWorkflowReader persistProcess = WorkflowSettings.GetConfig().Reader;

			return persistProcess.LoadProcessActivities(this);
		}

		private WfProcessContext LoadContext()
		{
			WfProcessContext processContext = null;

			IWorkflowReader persistProcess = WorkflowSettings.GetConfig().Reader;

			Dictionary<string, string> context = persistProcess.GetWfContext(this.ID);

			string strContext = string.Empty;

			if (context.TryGetValue(this.ID, out strContext))
				processContext = SerializationHelper.DeserializeStringToObject<WfProcessContext>(strContext, SerializationFormatterType.Binary);
			else
				processContext = new WfProcessContext();

			return processContext;
		}

		/// <summary>
		/// 根流程信息
		/// </summary>
		public IWfProcess RootProcess
		{
			get
			{
				IWfProcess result = this;

				while (result.EntryInfo != null)
					result = result.EntryInfo.OwnerOperation.AnchorActivity.Process;

				return result;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual IWfFactory Factory
		{
			get
			{
				lock (this)
				{
					if (_Factory == null)
						_Factory = new WfFactory();

					return _Factory;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public WfBranchProcessInfo EntryInfo
		{
			get
			{
				return _EntryInfo;
			}
			internal set
			{
				_EntryInfo = value;
			}
		}
		#endregion 公共属性

		/// <summary>
		/// 
		/// </summary>
		/// <param name="transferParams"></param>
		/// <returns></returns>
		public IWfActivity MoveTo(WfTransferParamsBase transferParams)
		{
			lock (this)
			{
				IWfActivity activity = InnerMoveTo(transferParams);

				if (this.CurrentActivity.FromTransition == null)
					WorkflowSettings.GetConfig().EnqueueWorkItemExecutor.EnqueueMoveToWorkItem(
						null, this.CurrentActivity);
				else
					WorkflowSettings.GetConfig().EnqueueWorkItemExecutor.EnqueueMoveToWorkItem(
						this.CurrentActivity.FromTransition.FromActivity, this.CurrentActivity);

				return activity;
			}
		}

		internal IWfActivity InnerMoveTo(WfTransferParamsBase transferParams)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(transferParams != null, "transferParams");

			//如果流程没有启动，且一个点都没有，则自动设为运行状态
			if (this.Status == WfProcessStatus.NotRunning && this.Activities.Count == 0)
				this.Status = WfProcessStatus.Running;
			else
				if (this.Status == WfProcessStatus.Completed) //如果流程已经结束，那么流程状态应该变为运行状态
					this.Status = WfProcessStatus.Running;

			CheckProcessRunningStatus();
			CheckActivityMoveToCondition();

			IWfActivity activity = null;

			//生成下一个新的活动点
			if (transferParams.NextActivityDescriptor is IWfAnchorActivityDescriptor)
				activity = this.Factory.CreateAnchorActivity(this, (WfBranchesTransferParams)transferParams);
			else
				activity = this.Factory.CreateActivity(this, transferParams.NextActivityDescriptor);

			activity.Operator = transferParams.Operator;

			//当前点与新的活动点连接,并设置好相关指针
			if (CurrentActivity != null)
			{
				((WfActivityBase)CurrentActivity).ConnectToNextActivity(activity);
				((WfActivityBase)CurrentActivity).Status = WfActivityStatus.Completed;
			}

			//加入点集合并设置好新的活动点
			Activities.Add(activity);

			activity.Assignees.CopyFrom(transferParams.Receivers);

			///设置的
			if (transferParams.FromTransitionDescriptor != null)
			{
				activity.Context["FromTransitionDescriptor"] = transferParams.FromTransitionDescriptor.Key;
			}

			DoComplete(activity);

			return activity;
		}

		public void Withdraw(IWfActivity destinationActivity)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(destinationActivity != null, "destinationActivity");

			lock (this)
			{
				ExceptionHelper.FalseThrow<WfEngineException>(this.Activities.ContainsKey(destinationActivity.ID),
					Resource.InvalidWithdrawActivityID, destinationActivity.ID);

				if (this.CurrentActivity != destinationActivity
					&& string.Compare(this.CurrentActivity.ID, destinationActivity.ID, true) != 0)
				{
					WfActivityCollection deletedActivities = DoWithdraw(destinationActivity);
					WfProcessCollection deletedProcesses = GetDeletedProcesses(deletedActivities);

					((WfActivityBase)destinationActivity).Status = WfActivityStatus.Running;
					this.Status = WfProcessStatus.Running;

					WorkflowSettings.GetConfig().EnqueueWorkItemExecutor.EnqueueWithdrawWorkItem(
						destinationActivity,
						deletedActivities,
						deletedProcesses);
				}
			}
		}

		public void CancelProcess()
		{
			lock (this)
			{
				ExceptionHelper.TrueThrow(Status == WfProcessStatus.Aborted, Resource.CanNotCacelProcess);

				Status = WfProcessStatus.Aborted;

				if (this.CurrentActivity != null)
					((WfActivityBase)this.CurrentActivity).Status = WfActivityStatus.Aborted;

				WorkflowSettings.GetConfig().EnqueueWorkItemExecutor.EnqueueCancelProcessWorkItem(this);
			}
		}

		public WfActivityLevelGroupCollection GetAllLevels(bool autoCalcaulatePath)
		{
			return ((WfProcessDescriptor)this.FirstActivity.Descriptor.Process).GetAllLevels(autoCalcaulatePath, this);
		}

		private WfActivityCollection DoWithdraw(IWfActivity destinationActivity)
		{
			WfActivityCollection deletedActivities = new WfActivityCollection();

			while (string.Compare(this.CurrentActivity.ID, destinationActivity.ID, true) != 0)
			{
				deletedActivities.Add(this.LastActivity);

				Activities.Remove(this.LastActivity);
			}

			return deletedActivities;
		}

		private WfProcessCollection GetDeletedProcesses(WfActivityCollection deletedActivities)
        {
			WfProcessCollection deletedProcesses = new WfProcessCollection();

            foreach (IWfActivity activity in deletedActivities)
            {
                if (activity is IWfAnchorActivity)
                {
                    foreach (IWfOperation operation in ((IWfAnchorActivity)activity).Operations)
                    {
                        foreach (WfBranchProcessInfo branchProcessInfo in operation.Branches)
                        {
                            deletedProcesses.Add(branchProcessInfo.Process);

							WfProcessCollection deletedSubProcesses = 
								GetDeletedProcesses(branchProcessInfo.Process.Activities);

							foreach (IWfProcess subProcess in deletedSubProcesses)
								deletedProcesses.Add(subProcess);
                        }
                    }
                }
            }

			return deletedProcesses;
        }

		private void DoComplete(IWfActivity activity)
		{
			//下一个点为结束点，进行特殊处理
			if (activity.Descriptor is IWfCompletedActivityDescriptor)
			{
				((WfActivityBase)activity).Status = WfActivityStatus.Completed;
				this.Status = WfProcessStatus.Completed;

				if (this.EntryInfo != null)
				{
					IWfOperation ownerOperation = this.EntryInfo.OwnerOperation;
					WfBranchProcessInfo currentProcessInfo = ownerOperation.Branches[this.ID];

					if (ownerOperation.OperationalType == BranchesOperationalType.Serial)
					{
						if (currentProcessInfo.Sequence + 1 < ownerOperation.Branches.Count)
						{
							WfBranchProcessInfo nextProcessInfo = ownerOperation.Branches[currentProcessInfo.Sequence + 1];
							((WfOperation)ownerOperation).DoNextBranchProcess(nextProcessInfo);
						}
					}
				}
			}
		}

		private void CheckProcessRunningStatus()
		{
			ExceptionHelper.TrueThrow<WfEngineException>(this.Status == WfProcessStatus.NotRunning, Resource.ProcessIsNotRunning);
			ExceptionHelper.TrueThrow<WfEngineException>(this.Status == WfProcessStatus.Aborted, Resource.ProcessIsAborted);
		}

		private void CheckActivityMoveToCondition()
		{
			if (this.CurrentActivity != null)
				ExceptionHelper.FalseThrow<WfEngineException>(this.CurrentActivity.AbleToMoveTo(),
					Resource.ActivityNotCompleted);
		}
	}
}
