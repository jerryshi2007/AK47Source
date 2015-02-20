using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using MCS.Library.Core;
using MCS.Library.Workflow.Properties;
using System.Security.Permissions;

namespace MCS.Library.Workflow.Descriptors
{
    /// <summary>
    /// Activity属性
    /// </summary>
    /// <remarks>
    /// _Variables变量列表、_Resources资源列表、_FromTransitions该点来线集合、_ToTransitions从该点发出的线的集合
    /// _Process包含流程全部信息，使得可以从此点找到流程的全部信息
    /// _Actions活动操作列表
    /// </remarks>
    /// <scholiast>徐照宇</scholiast>
    [Serializable]
    public abstract class WfActivityDescriptor : WfDescriptorBase, IWfActivityDescriptor
    {
        private WfVariableDescriptorCollection _Variables = new WfVariableDescriptorCollection();
        private WfResourceDescriptorCollection _Resources = null;
        private FromTransitionsDescriptorCollection _FromTransitions = null;
        private ToTransitionsDescriptorCollection _ToTransitions = null;
        private IWfProcessDescriptor _Process = null;
        private WfExtendedPropertyDictionary _ExtendedProperties = new WfExtendedPropertyDictionary();

        /// <summary>
        /// 构造方法
        /// </summary>
        protected WfActivityDescriptor()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="key"></param>
        protected WfActivityDescriptor(string key)
            : base(key)
        {
        }

        #region ISerializable Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected WfActivityDescriptor(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this._ToTransitions = (ToTransitionsDescriptorCollection)info.GetValue("ToTransitions", typeof(ToTransitionsDescriptorCollection));
            this._FromTransitions = (FromTransitionsDescriptorCollection)info.GetValue("FromTransitions", typeof(FromTransitionsDescriptorCollection));
            
			this._Process = (WfProcessDescriptor)info.GetValue("Process", typeof(WfProcessDescriptor));
            this._Resources = (WfResourceDescriptorCollection)info.GetValue("Resources", typeof(WfResourceDescriptorCollection));
            this._Variables = (WfVariableDescriptorCollection)info.GetValue("Variables", typeof(WfVariableDescriptorCollection));
            this._ExtendedProperties = (WfExtendedPropertyDictionary)info.GetValue("ExtendedProperty", typeof(WfExtendedPropertyDictionary));
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

			info.AddValue("ToTransitions", this._ToTransitions, typeof(ToTransitionsDescriptorCollection));
            info.AddValue("FromTransitions", this._FromTransitions, typeof(FromTransitionsDescriptorCollection));
			info.AddValue("Process", this._Process, typeof(WfProcessDescriptor));
            info.AddValue("Resources", _Resources, typeof(WfResourceDescriptorCollection));
            info.AddValue("Variables", _Variables, typeof(WfVariableDescriptorCollection));
            info.AddValue("ExtendedProperty", _ExtendedProperties, typeof(WfExtendedPropertyDictionary));
        }
        #endregion

        /// <summary>
        /// Process Key属性器
        /// </summary>
        /// <scholiast>徐照宇</scholiast>
        /// <remarks>Process Key属性器</remarks>
        /// <scholiast>徐照宇</scholiast>
        public override string Key
        {
            get
            {
                return base.Key;
            }
            set
            {
                ExceptionHelper.TrueThrow<WfDescriptorException>(_Process != null, Resource.CannotModifyActivityKeyInProcess);
                base.Key = value;
            }
        }

        /// <summary>
        /// Process Value属性器
        /// </summary>
        /// <remarks>Process Value属性器</remarks>
        /// <scholiast>徐照宇</scholiast>
        public IWfProcessDescriptor Process
        {
            get 
            { 
                return _Process; 
            }
            internal set 
            { 
                _Process = value; 
            }
        }

		/// <summary>
		/// 判断从当前点能否抵达另一个点
		/// </summary>
		/// <param name="activityDespKey">目标点的Key</param>
		/// <returns></returns>
		public bool CanReachTo(string targetActivityDespKey)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(targetActivityDespKey, "targetActivityDespKey");

			Dictionary<IWfActivityDescriptor, IWfActivityDescriptor> history = new Dictionary<IWfActivityDescriptor, IWfActivityDescriptor>();

			return InnerCanReachTo(targetActivityDespKey, history);
		}

		/// <summary>
		/// 判断从当前点能否抵达另一个点
		/// </summary>
		/// <param name="targetAct">目标点的描述对象</param>
		/// <returns></returns>
		public bool CanReachTo(IWfActivityDescriptor targetAct)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(targetAct != null, "targetAct");
			bool result = false;

			if (this.Process == targetAct.Process)
				result = CanReachTo(targetAct.Key);

			return result;
		}

        /// <summary>
        /// 实例化ToTransitions属性集合
        /// </summary>
        /// <remarks>第一次添加去线，实例化去线属性集合</remarks>
        /// <scholiast>徐照宇</scholiast>
        public ToTransitionsDescriptorCollection ToTransitions
        {
            get
            {
                if (_ToTransitions == null)
                    _ToTransitions = new ToTransitionsDescriptorCollection(this);

                return _ToTransitions;
            }
        }

        /// <summary>
        /// 实例化FromTransitions属性
        /// </summary>
        /// <remarks>第一次添加去线，实例化去线属性集合,向toActivity点添加FromTransition点</remarks>
        /// <scholiast>徐照宇</scholiast>
        public FromTransitionsDescriptorCollection FromTransitions
        {
            get
            {
                if (_FromTransitions == null)
                    _FromTransitions = new FromTransitionsDescriptorCollection(this);

                return _FromTransitions;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public WfResourceDescriptorCollection Resources
        {
            get 
            {
				if (_Resources == null)
					_Resources = CreateResourceCollection();

                return _Resources; 
            }
        }

		/// <summary>
		///
		/// </summary>
        public WfVariableDescriptorCollection Variables
        {
            get 
            {
                return _Variables; 
            }
        }

        public WfExtendedPropertyDictionary ExtendedProperties
        {
            get
            {
                return _ExtendedProperties;
            }
        }

		/// <summary>
		/// 流程对应的环节名称(多个节点可能对应一个环节名称)
		/// </summary>
		public string LevelName
		{
			get
			{
				return Variables.GetValue("LevelName", string.Empty);
			}
			set
			{
				WfVariableDescriptor variable = Variables["LevelName"];

				if (variable == null)
				{
					variable = new WfVariableDescriptor("LevelName", value);
					Variables.Add(variable);
				}
				else
					variable.OriginalValue = value;
			}
		}

		/// <summary>
		/// 复制一个流程的节点描述，生成一个新的key
		/// </summary>
		/// <returns></returns>
		public virtual IWfActivityDescriptor CloneDescriptor()
		{
			WfActivityDescriptor result = CreateNewDescriptor();

			result.Key = this.Process.FindNotUsedActivityKey();
			result.Name = this.Name;
			result.Description = this.Description;
			
			this.Variables.ForEach(variable=>result.Variables.Add(variable));
			this.Resources.ForEach(resource=>result.Resources.Add(resource));

			foreach (KeyValuePair<string, object> kp in this.ExtendedProperties)
				result.ExtendedProperties.Add(kp.Key, kp.Value);
			
			return result;
		}

		protected abstract WfActivityDescriptor CreateNewDescriptor();

		protected virtual WfResourceDescriptorCollection CreateResourceCollection()
		{
			return new WfResourceDescriptorCollection();
		}

		private bool InnerCanReachTo(string targetActivityDespKey, Dictionary<IWfActivityDescriptor, IWfActivityDescriptor> history)
		{
			bool result = false;

			if (history.ContainsKey(this) == false)
			{
				result = string.Compare(this.Key, targetActivityDespKey, true) == 0;

				if (result == false)
				{
					history.Add(this, this);

					foreach (IWfTransitionDescriptor transition in this.ToTransitions)
					{
						if (transition is IWfForwardTransitionDescriptor)
						{
							result = ((WfActivityDescriptor)transition.ToActivity).InnerCanReachTo(targetActivityDespKey, history);

							if (result)
								break;
						}
					}
				}
			}

			return result;
		}
	}

    /// <summary>
    /// 正常流程执行点
    /// </summary>
    /// <remarks>流程点分为四种，开始点、结束点、正常执行点、子流程点</remarks>
    /// <scholiast>徐照宇</scholiast>
    [Serializable]
    public class WfRegularActivityDescriptor : WfActivityDescriptor, IWfRegularActivityDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        public WfRegularActivityDescriptor()
            : base()
        {
        }

        /// <summary>
        /// 正常流程执行点初始化
        /// </summary>
        /// <param name="key"></param>
        /// <remarks>流程点分为四种，开始点、结束点、正常执行点、子流程点</remarks>
        /// <scholiast>徐照宇</scholiast>
        public WfRegularActivityDescriptor(string key)
            : base(key)
        {
        }

        #region ISerializable Members
        public WfRegularActivityDescriptor(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion

		protected override WfActivityDescriptor CreateNewDescriptor()
		{
			return new WfRegularActivityDescriptor();
		}
    }

    /// <summary>
    /// 流程开始点
    /// </summary>
    /// <scholiast>徐照宇</scholiast>
    [Serializable]
    public class WfInitialActivityDescriptor : WfActivityDescriptor, IWfInitialActivityDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        public WfInitialActivityDescriptor()
            : base()
        {
        }

        /// <summary>
        /// 流程开始点初始化
        /// </summary>
        /// <param name="key">开始点Key</param>
        /// <remarks>流程点分为四种，开始点、结束点、正常执行点、子流程点</remarks>
        /// <scholiast>徐照宇</scholiast>
        public WfInitialActivityDescriptor(string key)
            : base(key)
        {

        }

        #region ISerializable Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected WfInitialActivityDescriptor(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
       
        #endregion

		protected override WfActivityDescriptor CreateNewDescriptor()
		{
			return new WfRegularActivityDescriptor();
		}
    }
    /// <summary>
    /// 流程结束点
    /// </summary>
    [Serializable]
    public class WfCompletedActivityDescriptor : WfActivityDescriptor, IWfCompletedActivityDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        public WfCompletedActivityDescriptor()
            : base()
        {
        }

        /// <summary>
        /// 流程结束点初始化
        /// </summary>
        /// <param name="key">结束点Key</param>
        /// <remarks>流程点分为四种，开始点、结束点、正常执行点、子流程点</remarks>
        /// <scholiast>徐照宇</scholiast>
        public WfCompletedActivityDescriptor(string key)
            : base(key)
        {
        }

        #region ISerializable Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public WfCompletedActivityDescriptor(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        #endregion

		protected override WfActivityDescriptor CreateNewDescriptor()
		{
			return new WfCompletedActivityDescriptor();
		}
    }

    /// <summary>
    /// 子流程点
    /// </summary>
    /// <remarks>流程点分为四种，开始点、结束点、正常执行点、子流程点</remarks>
    /// <scholiast>徐照宇</scholiast>
    [Serializable]
    public class WfAnchorActivityDescriptor : WfActivityDescriptor, IWfAnchorActivityDescriptor
    {
        private WfOperationDescriptorCollection _Operations = null;

        /// <summary>
        /// 
        /// </summary>
        public WfAnchorActivityDescriptor()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public WfAnchorActivityDescriptor(string key)
            : base(key)
        {
        }

        #region ISerializable Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected WfAnchorActivityDescriptor(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this._Operations = (WfOperationDescriptorCollection)info.GetValue("Operations", typeof(WfOperationDescriptorCollection));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Operations", this._Operations, typeof(WfOperationDescriptorCollection));
        }
        #endregion

        public WfOperationDescriptorCollection Operations
        {
            get 
            {
                if (_Operations == null)
                    _Operations = CreateOperationCollection();

                return _Operations;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual WfOperationDescriptorCollection CreateOperationCollection()
        {
            return new WfOperationDescriptorCollection();
        }

		protected override WfActivityDescriptor CreateNewDescriptor()
		{
			return new WfAnchorActivityDescriptor();
		}
    }

    /// <summary>
    /// 工作流 点属性集合
    /// </summary>
    /// <remarks>
    /// 基类为WfDescriptorCollectionBase。该基类提供Dictionary成员，表示键和值的集合。
    /// _Process为全部流程的纪录，以保证可以从任意点获得全部流程信息。
    /// _InitialActivity、_CompletedActivity是流程的首尾标记点，为查找提供方便。
    /// </remarks>
    /// <scholiast>徐照宇</scholiast>
    [Serializable]
    public class WfActivityDescriptorCollection : WfDescriptorCollectionBase<IWfActivityDescriptor>
    {
        private IWfProcessDescriptor _Process = null;
        private IWfInitialActivityDescriptor _InitialActivity = null;
        private IWfCompletedActivityDescriptor _CompletedActivity = null;

        /// <summary>
        /// 增加Activity点，将访问控制项追加到访问控制列表。
        /// </summary>
        /// <param name="item">增加点的属性</param>
        /// <remarks>在执行Add前先执行OnInsert</remarks>
        /// <scholiast>徐照宇</scholiast>
        public void Add(IWfActivityDescriptor item)
        {
            this.InnerAdd(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
			this.RWLock.AcquireWriterLock(lockTimeout);
			try
			{
				IWfActivityDescriptor actDesp = this[key];

				Remove(actDesp);
			}
			finally
			{
				this.RWLock.ReleaseWriterLock();
			}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actDesp"></param>
        public void Remove(IWfActivityDescriptor actDesp)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(actDesp != null, "actDesp");

			this.RWLock.AcquireWriterLock(lockTimeout);
			try
			{
				foreach (WfTransitionDescriptor transition in actDesp.ToTransitions)
					transition.ToActivity.FromTransitions.RemoveByFromActivity(actDesp);

				foreach (WfTransitionDescriptor transition in actDesp.FromTransitions)
					transition.FromActivity.ToTransitions.RemoveByToActivity(actDesp);

				List.Remove(actDesp);
			}
			finally
			{
				this.RWLock.ReleaseWriterLock();
			}
        }

		public void Replace(string srcKey, string destKey)
		{
			this.RWLock.AcquireWriterLock(lockTimeout);
			try
			{
				IWfActivityDescriptor srcDesp = this[srcKey];
				IWfActivityDescriptor destDesp = this[destKey];

				Replace(srcDesp, destDesp);
			}
			finally
			{
				this.RWLock.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// 替换节点
		/// </summary>
		/// <param name="srcDesp"></param>
		/// <param name="destDesp"></param>
		public void Replace(IWfActivityDescriptor srcDesp, IWfActivityDescriptor destDesp)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(srcDesp != null, "srcDesp");
			ExceptionHelper.FalseThrow<ArgumentNullException>(destDesp != null, "destDesp");

			this.RWLock.AcquireWriterLock(lockTimeout);
			try
			{
				foreach (WfTransitionDescriptor transition in srcDesp.FromTransitions)
				{
					transition.ToActivity = destDesp;
					destDesp.FromTransitions.Add(transition);
				}

				foreach (WfTransitionDescriptor transition in srcDesp.ToTransitions)
				{
					transition.FromActivity = destDesp;
					destDesp.ToTransitions.Add(transition);
				}

				List.Remove(srcDesp);
				List.Add(destDesp);
			}
			finally
			{
				this.RWLock.ReleaseWriterLock();
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        internal protected WfActivityDescriptorCollection(IWfProcessDescriptor process)
        {
            _Process = process;
        }

        internal IWfInitialActivityDescriptor InitialActivity
        {
            get { return _InitialActivity; }
        }

        internal IWfCompletedActivityDescriptor CompletedActivity
        {
            get { return _CompletedActivity; }
        }

        /// <summary>
        /// 在向 DictionaryBase 实例中插入新元素之前执行其他自定义进程
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="value">value</param>
        /// <remarks>
        /// 处理过程：先在WfActivityDescriptorCollection中检验所加点的合法性ValidateActivity()（只能由一个开始点和一个结束点）
        /// 将_Process值赋给item.Process，使每个点都包含全部流程点的属性，方便从任一点遍历整个流程
        /// </remarks>
        protected override void OnInsert(int index, object value)
        {
            WfActivityDescriptor item = (WfActivityDescriptor)value;
            ValidateActivity(item);

            item.Process = _Process;
            base.OnInsert(index, value);

            if (item is IWfInitialActivityDescriptor)
                _InitialActivity = (IWfInitialActivityDescriptor)item;
            else
                if (item is IWfCompletedActivityDescriptor)
                    _CompletedActivity = (IWfCompletedActivityDescriptor)item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        protected override void OnRemoveComplete(int index, object value)
        {
            base.OnRemoveComplete(index, value);

            if (value is IWfInitialActivityDescriptor)
                _InitialActivity = null;
            else
                if (value is IWfCompletedActivityDescriptor)
                    _CompletedActivity = null;
        }

        private void ValidateActivity(IWfActivityDescriptor item)
        {
            CheckActivityType<IWfInitialActivityDescriptor>(item, Resource.OneProcessOnlyHasOneInitialActivity);
            CheckActivityType<IWfCompletedActivityDescriptor>(item, Resource.OneProcessOnlyHasOneCompletedActivity);
        }

        private void CheckActivityType<T>(IWfActivityDescriptor item, string errorMessage)
        {
			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				if (item is T)
					foreach (IWfActivityDescriptor act in base.List)
						if (act is T)
							throw new WfDescriptorException(errorMessage);
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
        }
	}
}
