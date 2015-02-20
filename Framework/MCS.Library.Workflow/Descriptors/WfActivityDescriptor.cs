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
    /// Activity����
    /// </summary>
    /// <remarks>
    /// _Variables�����б�_Resources��Դ�б�_FromTransitions�õ����߼��ϡ�_ToTransitions�Ӹõ㷢�����ߵļ���
    /// _Process��������ȫ����Ϣ��ʹ�ÿ��ԴӴ˵��ҵ����̵�ȫ����Ϣ
    /// _Actions������б�
    /// </remarks>
    /// <scholiast>������</scholiast>
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
        /// ���췽��
        /// </summary>
        protected WfActivityDescriptor()
        {
        }

        /// <summary>
        /// ���췽��
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
        /// Process Key������
        /// </summary>
        /// <scholiast>������</scholiast>
        /// <remarks>Process Key������</remarks>
        /// <scholiast>������</scholiast>
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
        /// Process Value������
        /// </summary>
        /// <remarks>Process Value������</remarks>
        /// <scholiast>������</scholiast>
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
		/// �жϴӵ�ǰ���ܷ�ִ���һ����
		/// </summary>
		/// <param name="activityDespKey">Ŀ����Key</param>
		/// <returns></returns>
		public bool CanReachTo(string targetActivityDespKey)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(targetActivityDespKey, "targetActivityDespKey");

			Dictionary<IWfActivityDescriptor, IWfActivityDescriptor> history = new Dictionary<IWfActivityDescriptor, IWfActivityDescriptor>();

			return InnerCanReachTo(targetActivityDespKey, history);
		}

		/// <summary>
		/// �жϴӵ�ǰ���ܷ�ִ���һ����
		/// </summary>
		/// <param name="targetAct">Ŀ������������</param>
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
        /// ʵ����ToTransitions���Լ���
        /// </summary>
        /// <remarks>��һ�����ȥ�ߣ�ʵ����ȥ�����Լ���</remarks>
        /// <scholiast>������</scholiast>
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
        /// ʵ����FromTransitions����
        /// </summary>
        /// <remarks>��һ�����ȥ�ߣ�ʵ����ȥ�����Լ���,��toActivity�����FromTransition��</remarks>
        /// <scholiast>������</scholiast>
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
		/// ���̶�Ӧ�Ļ�������(����ڵ���ܶ�Ӧһ����������)
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
		/// ����һ�����̵Ľڵ�����������һ���µ�key
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
    /// ��������ִ�е�
    /// </summary>
    /// <remarks>���̵��Ϊ���֣���ʼ�㡢�����㡢����ִ�е㡢�����̵�</remarks>
    /// <scholiast>������</scholiast>
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
        /// ��������ִ�е��ʼ��
        /// </summary>
        /// <param name="key"></param>
        /// <remarks>���̵��Ϊ���֣���ʼ�㡢�����㡢����ִ�е㡢�����̵�</remarks>
        /// <scholiast>������</scholiast>
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
    /// ���̿�ʼ��
    /// </summary>
    /// <scholiast>������</scholiast>
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
        /// ���̿�ʼ���ʼ��
        /// </summary>
        /// <param name="key">��ʼ��Key</param>
        /// <remarks>���̵��Ϊ���֣���ʼ�㡢�����㡢����ִ�е㡢�����̵�</remarks>
        /// <scholiast>������</scholiast>
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
    /// ���̽�����
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
        /// ���̽������ʼ��
        /// </summary>
        /// <param name="key">������Key</param>
        /// <remarks>���̵��Ϊ���֣���ʼ�㡢�����㡢����ִ�е㡢�����̵�</remarks>
        /// <scholiast>������</scholiast>
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
    /// �����̵�
    /// </summary>
    /// <remarks>���̵��Ϊ���֣���ʼ�㡢�����㡢����ִ�е㡢�����̵�</remarks>
    /// <scholiast>������</scholiast>
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
    /// ������ �����Լ���
    /// </summary>
    /// <remarks>
    /// ����ΪWfDescriptorCollectionBase���û����ṩDictionary��Ա����ʾ����ֵ�ļ��ϡ�
    /// _ProcessΪȫ�����̵ļ�¼���Ա�֤���Դ��������ȫ��������Ϣ��
    /// _InitialActivity��_CompletedActivity�����̵���β��ǵ㣬Ϊ�����ṩ���㡣
    /// </remarks>
    /// <scholiast>������</scholiast>
    [Serializable]
    public class WfActivityDescriptorCollection : WfDescriptorCollectionBase<IWfActivityDescriptor>
    {
        private IWfProcessDescriptor _Process = null;
        private IWfInitialActivityDescriptor _InitialActivity = null;
        private IWfCompletedActivityDescriptor _CompletedActivity = null;

        /// <summary>
        /// ����Activity�㣬�����ʿ�����׷�ӵ����ʿ����б�
        /// </summary>
        /// <param name="item">���ӵ������</param>
        /// <remarks>��ִ��Addǰ��ִ��OnInsert</remarks>
        /// <scholiast>������</scholiast>
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
		/// �滻�ڵ�
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
        /// ���� DictionaryBase ʵ���в�����Ԫ��֮ǰִ�������Զ������
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="value">value</param>
        /// <remarks>
        /// ������̣�����WfActivityDescriptorCollection�м������ӵ�ĺϷ���ValidateActivity()��ֻ����һ����ʼ���һ�������㣩
        /// ��_Processֵ����item.Process��ʹÿ���㶼����ȫ�����̵�����ԣ��������һ�������������
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
