using System;
using System.Text;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Collections.Specialized;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Workflow.Descriptors;
using MCS.Library.Workflow.Configuration;
using MCS.Library.Workflow.OguObjects;

namespace MCS.Library.Workflow.Engine
{
    [Serializable]
    public abstract class WfActivityBase : IWfActivity, ISerializable
    {
		private string _ID = UuidHelper.NewUuidString();
        private IWfActivityDescriptor _Descriptor;
        private IWfTransition _FromTransition = null;
        private IWfTransition _ToTransition = null;
        protected WfActivityContext _Context = null;
        protected WfActivityStatus _Status = WfActivityStatus.Running;
        private IWfProcess _Process = null;
        private bool _IsAborted = false;
        protected DateTime _StartTime = DateTime.MinValue;
        protected DateTime _EndTime = DateTime.MinValue;
		private WfAssigneeCollection _Assignees = null;
        private IUser _Operator = null;
        protected string _ProcessDescKey = string.Empty;
		protected string _ActivityDescKey = string.Empty;
		protected DataLoadingType _LoadingType = DataLoadingType.Memory;

        #region ISerializable 成员

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
			info.AddValue("ID", this._ID);

			if (WorkflowSettings.GetConfig().IsSerializeDesc)
				info.AddValue("Descriptor", this._Descriptor, typeof(IWfActivityDescriptor));

			info.AddValue("ProcessDescKey", this._ProcessDescKey);
			info.AddValue("ActivityDescKey", this._ActivityDescKey);
            info.AddValue("FromTransition", this._FromTransition, typeof(IWfTransition));
            info.AddValue("ToTransition", this._ToTransition, typeof(IWfTransition));
			info.AddValue("Context", this._Context);
			info.AddValue("Status", this._Status);

			info.AddValue("Process", this._Process);
			info.AddValue("IsAborted", this._IsAborted);
			info.AddValue("StartTime", this._StartTime);
			info.AddValue("EndTime", this._EndTime);

			info.AddValue("Assignees", this._Assignees);

			info.AddValue("Operator", this._Operator);
			info.AddValue("LoadingType", _LoadingType);
        }

        protected WfActivityBase(SerializationInfo info, StreamingContext context)
        {
			this._ID = info.GetString("ID");

			if (WorkflowSettings.GetConfig().IsSerializeDesc)
				this._Descriptor = (IWfActivityDescriptor)info.GetValue("Descriptor", typeof(IWfActivityDescriptor));

			this._ProcessDescKey = info.GetString("ProcessDescKey");
			this._ActivityDescKey = info.GetString("ActivityDescKey");
            this._FromTransition = (IWfTransition)info.GetValue("FromTransition", typeof(IWfTransition));
            this._ToTransition = (IWfTransition)info.GetValue("ToTransition", typeof(IWfTransition));
			this._Context = (WfActivityContext)info.GetValue("Context", typeof(WfActivityContext));
			this._Status = (WfActivityStatus)info.GetValue("Status", typeof(WfActivityStatus));

			this._Process = (IWfProcess)info.GetValue("Process", typeof(IWfProcess));
			this._IsAborted = info.GetBoolean("IsAborted");
			this._StartTime = info.GetDateTime("StartTime");
			this._EndTime = info.GetDateTime("EndTime");

			this._Assignees = (WfAssigneeCollection)info.GetValue("Assignees", typeof(WfAssigneeCollection));

			this._Operator = (IUser)info.GetValue("Operator", typeof(IUser));
			this._LoadingType = (DataLoadingType)info.GetValue("LoadingType", typeof(DataLoadingType));
        }

        #endregion

        protected WfActivityBase(IWfActivityDescriptor descriptor)
        {
            _Descriptor = descriptor;

			this._ProcessDescKey = descriptor.Process.Key;
			this._ActivityDescKey = descriptor.Key;
            this._StartTime = DateTime.Now;
        }

        protected WfActivityBase()
        {
        }

        #region 公共属性
        public string ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }

        public IWfActivityDescriptor Descriptor
        {
            get
            {
				if (_Descriptor == null)
				{
					Dictionary<string, IWfProcessDescriptor> processDesps = ((WfProcess)_Process).Descriptors;
					IWfProcessDescriptor processDesp = null;

					lock (processDesps)
					{
						if (processDesps.TryGetValue(this._ProcessDescKey, out processDesp) == false)
						{
							processDesp = WorkflowSettings.GetConfig().Reader.LoadProcessDescriptor(this._Process.ID, this._ProcessDescKey);
							processDesps.Add(this._ProcessDescKey, processDesp);
						}
					}

					_Descriptor = processDesp.Activities[this._ActivityDescKey];
				}

				return _Descriptor;
            }
        }
        
        public WfActivityStatus Status
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

        public bool IsAborted
        {
            get
            {
                return _IsAborted;
            }
        }

        public IWfTransition FromTransition
        {
            get
            {
                return _FromTransition;
            }
            set
            {
                this._FromTransition = value;
            }
        }

        public IWfTransition ToTransition
        {
            get
            {
                return _ToTransition;
            }
            set
            {
                this._ToTransition = value;
            }
        }

        public WfActivityContext Context
        {
            get
            {
				if (_Context == null)
					if (LoadingType == DataLoadingType.External)
						_Context = LoadContext();
					else
						_Context = new WfActivityContext();

                return _Context;
            }
        }

		/// <summary>
		/// 当前点是不是根流程实例的第一个点
		/// </summary>
		public bool IsFirstActivity
		{
			get
			{
				bool result = false;

				if (_Process != null)
					result = (_Process.EntryInfo == null) && (_Process.FirstActivity) == this;

				return result;
			}
		}

		/// <summary>
		/// 当前点是不是流程实例的第一个点，如果不是分支流程，则与IsFirstActivity相同
		/// </summary>
		public bool IsCurrentProcessFirstActivity
		{
			get
			{
				bool result = false;

				if (_Process != null)
					result = _Process.FirstActivity == this;

				return result;
			}
		}

		public bool IsLastActivity
		{
			get
			{
				bool result = false;

				if (_Process != null)
					result = _Process.LastActivity == this;

				return result;
			}
		}

		public DataLoadingType LoadingType
		{
			get
			{
				return _LoadingType;
			}
		}

        public IWfProcess Process
        {
            get
            {
                return _Process;
            }
            set
            {
                _Process = value;
            }
        }

		public IWfActivity RootActivity
		{
			get
			{
				IWfActivity result = this;

				while (result.Process != null && result.Process.EntryInfo != null)
					result = result.Process.EntryInfo.OwnerOperation.AnchorActivity;

				return result;
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public WfAssigneeCollection Assignees
        {
            get
            {
				if (_Assignees == null)
					if (LoadingType == DataLoadingType.External)
						_Assignees = LoadAssignees();
					else
						_Assignees = new WfAssigneeCollection();
             
                return _Assignees;
            }
        }

        /// <summary>
        /// 
        /// </summary>
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
					if ((value is WfOguUser) == false)
						this._Operator = new WfOguUser(value);
					else
						this._Operator = value;
				}
				else
					this._Operator = value;
            } 
        }

		/// <summary>
		/// 从哪条线定义产生的活动点。有可能为null，例如起始点。
		/// </summary>
		public IWfTransitionDescriptor FromTransitionDescriptor
		{
			get
			{
				IWfTransitionDescriptor result = null;

				string transitionKey = this.Context.GetValue("FromTransitionDescriptor", string.Empty);

				if (string.IsNullOrEmpty(transitionKey) == false)
					result = this.Descriptor.Process.FindTransitionByKey(transitionKey);

				return result;
			}
		}
        #endregion

        public virtual bool AbleToMoveTo()
        {
			//当流程运行中，移动的前提是点必须是活动点。如果流程已经结束，仅有结束点可以移动
			return this.Status == WfActivityStatus.Running ||
					(this.Status == WfActivityStatus.Completed && (this.Descriptor is IWfCompletedActivityDescriptor));
		}

		#region 私有方法
		internal void ConnectToNextActivity(IWfActivity nextActivity)
		{
			IWfTransitionDescriptor transDescriptor =
				this.Descriptor.ToTransitions.GetTransitionByToActivity(nextActivity.Descriptor.Key);

			WfTransition transition = new WfTransition(transDescriptor);

			transition.FromActivity = this;
			transition.ToActivity = nextActivity;

			this._ToTransition = transition;
			((WfActivityBase)nextActivity)._FromTransition = transition;
		}

		private WfActivityContext LoadContext()
		{
			WfActivityContext activityContext = null;

			IWorkflowReader persistProcess = WorkflowSettings.GetConfig().Reader;

			Dictionary<string, string> context = persistProcess.GetWfContext(this.ID);

			string strContext = string.Empty;

			if (context.TryGetValue(this.ID, out strContext))
				activityContext = SerializationHelper.DeserializeStringToObject<WfActivityContext>(strContext, SerializationFormatterType.Binary);
			else
				activityContext = new WfActivityContext();

			return activityContext;
		}

		private WfAssigneeCollection LoadAssignees()
		{
			IWorkflowReader persistProcess = WorkflowSettings.GetConfig().Reader;

			return persistProcess.LoadAssignees(this.ID)[this.ID];
		}

		#endregion
	}
}
