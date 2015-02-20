using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using MCS.Library.Core;
using MCS.Library.Workflow.Properties;
using System.Security.Permissions;
using System.Diagnostics;


namespace MCS.Library.Workflow.Descriptors
{
	/// <summary>
	/// 线基本属性
	/// </summary>
	[Serializable]
	public abstract class WfTransitionDescriptor : WfDescriptorBase, IWfTransitionDescriptor
	{
		private IWfActivityDescriptor _FromActivity;
		private IWfActivityDescriptor _ToActivity;
		private int _Priority;
		private WfVariableDescriptorCollection _Variables = new WfVariableDescriptorCollection();

		protected WfTransitionDescriptor()
			: base()
		{
		}
		/// <summary>
		/// 给线赋予Key值
		/// </summary>
		/// <param name="key">Key值</param>
		/// <scholiast>徐照雨</scholiast>
		protected WfTransitionDescriptor(string key)
			: base(key)
		{
		}
		/// <summary>
		/// Priority属性访问器
		/// </summary>
		/// <remarks>Priority属性访问器</remarks>
		/// <scholiast>徐照雨</scholiast>
		public int Priority
		{
			get
			{
				return this._Priority;
			}
			set
			{
				this._Priority = value;
			}
		}
		/// <summary>
		/// ToActivity属性访问器
		/// </summary>
		/// <remarks>ToActivity属性访问器</remarks>
		/// <scholiast>徐照雨</scholiast>
		public IWfActivityDescriptor ToActivity
		{
			get
			{
				return _ToActivity;
			}
			set
			{
				_ToActivity = value;
			}
		}

		/// <summary>
		/// FromActivity属性访问器
		/// </summary>
		/// <remarks>FromActivity属性访问器</remarks>
		/// <scholiast>徐照雨</scholiast>
		public IWfActivityDescriptor FromActivity
		{
			get
			{
				return _FromActivity;
			}
			set
			{
				_FromActivity = value;
			}
		}

		public WfVariableDescriptorCollection Variables
		{
			get
			{
				return _Variables;
			}
		}

		public bool DefaultSelect
		{
			get
			{
				return Variables.GetValue("DefaultSelect", false);
			}
			set
			{
				WfVariableDescriptor variable = this.Variables["DefaultSelect"];

				if (variable != null)
					variable.OriginalValue = value.ToString();
				else
				{
					variable = new WfVariableDescriptor("DefaultSelect", value.ToString(), DataType.Boolean);
					this.Variables.Add(variable);
				}
			}
		}

		public virtual bool CanTransit()
		{
			return this.Enabled;
		}

		internal protected virtual void JoinActivity(IWfActivityDescriptor fromActivity, IWfActivityDescriptor toActivity)
		{
			_FromActivity = fromActivity;
			_ToActivity = toActivity;
		}

		#region ISerializable Members
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected WfTransitionDescriptor(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this._Priority = info.GetInt32("Priority");
			this._FromActivity = (IWfActivityDescriptor)info.GetValue("FromActivity", typeof(WfActivityDescriptor));
			this._ToActivity = (IWfActivityDescriptor)info.GetValue("ToActivity", typeof(WfActivityDescriptor));
			this._Variables = (WfVariableDescriptorCollection)info.GetValue("Variables", typeof(WfVariableDescriptorCollection));
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

			info.AddValue("Priority", this._Priority, typeof(int));
			info.AddValue("FromActivity", this._FromActivity, typeof(WfActivityDescriptor));
			info.AddValue("ToActivity", this._ToActivity, typeof(WfActivityDescriptor));
			info.AddValue("Variables", _Variables, typeof(WfVariableDescriptorCollection));
		}

		#endregion

		#region IComparable<IWfTransitionDescriptor> Members

		public int CompareTo(IWfTransitionDescriptor other)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(other != null, "other");

			return this._Priority.CompareTo(other.Priority);
		}

		#endregion
	}
	/// <summary>
	/// 前向线属性
	/// </summary>
	[Serializable]
	public class WfForwardTransitionDescriptor : WfTransitionDescriptor, IWfForwardTransitionDescriptor
	{
		/// <summary>
		/// 
		/// </summary>
		public WfForwardTransitionDescriptor()
			: base()
		{
		}

		/// <summary>
		/// 前向线Key
		/// </summary>
		/// <param name="key">Key</param>
		/// <remarks>得到前向线的Key值</remarks>
		/// <scholiast>徐照雨</scholiast>
		public WfForwardTransitionDescriptor(string key)
			: base(key)
		{
		}

		private WfConditionDescriptor _Condition = null;

		/// <summary>
		/// 
		/// </summary>
		public WfConditionDescriptor Condition
		{
			get { return _Condition; }
			set { _Condition = value; }
		}

		/// <summary>
		/// 是否可以移动（Enabled且表达式分析成立）
		/// </summary>
		/// <returns></returns>
		public override bool CanTransit()
		{
			try
			{
				return this.Enabled && (this._Condition != null && this._Condition.Evaluate());
			}
			catch (System.Exception ex)
			{
				throw new WfTransitionEvaluationException(string.Format("判断线{0}:{1}的条件，{2}", this.Key, this.Name, ex.Message));
			}
		}

		#region ISerializable Members
		protected WfForwardTransitionDescriptor(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this._Condition = (WfConditionDescriptor)info.GetValue("Condition", typeof(WfConditionDescriptor));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("Condition", _Condition, typeof(WfConditionDescriptor));
		}
		#endregion
	}

	/// <summary>
	/// 后向线属性
	/// </summary>
	[Serializable]
	public class WfBackwardTransitionDescriptor : WfTransitionDescriptor, IWfBackwardTransitionDescriptor
	{
		/// <summary>
		/// 
		/// </summary>
		public WfBackwardTransitionDescriptor()
			: base()
		{
		}
		/// <summary>
		/// 后向线Key
		/// </summary>
		/// <param name="key">Key</param>
		public WfBackwardTransitionDescriptor(string key)
			: base(key)
		{
		}

		#region ISerializable Members
		protected WfBackwardTransitionDescriptor(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
		#endregion
	}

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public abstract class WfTransitionDescriptorCollection : WfDescriptorCollectionBase<IWfTransitionDescriptor>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="transition"></param>
		internal protected void Add(IWfTransitionDescriptor transition)
		{
			InnerAdd(transition);
		}

		/// <summary>
		/// 创建一个新的集合
		/// </summary>
		/// <returns></returns>
		internal protected abstract WfTransitionDescriptorCollection CreateNewCollection();

		public IWfTransitionDescriptor FindDefaultSelectTransition()
		{
			IWfTransitionDescriptor transition = null;

			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].DefaultSelect)
				{
					transition = this[i];
					break;
				}
			}

			return transition;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="transition"></param>
		public void Remove(IWfTransitionDescriptor transition)
		{
			InnerRemove(transition);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		public void Remove(string key)
		{
			InnerRemove(key);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="actKey"></param>
		public void RemoveByFromActivity(string actKey)
		{
			List<IWfTransitionDescriptor> list = new List<IWfTransitionDescriptor>();

			this.RWLock.AcquireWriterLock(lockTimeout);
			try
			{
				foreach (IWfTransitionDescriptor transition in base.List)
					if (string.Compare(transition.FromActivity.Key, actKey, true) == 0)
						list.Add(transition);

				foreach (IWfTransitionDescriptor transition in list)
					Remove(transition);
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
		public void RemoveByFromActivity(IWfActivityDescriptor actDesp)
		{
			if (actDesp != null)
				RemoveByFromActivity(actDesp.Key);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="actKey"></param>
		public void RemoveByToActivity(string actKey)
		{
			List<IWfTransitionDescriptor> list = new List<IWfTransitionDescriptor>();

			this.RWLock.AcquireWriterLock(lockTimeout);
			try
			{
				foreach (IWfTransitionDescriptor transition in base.List)
					if (string.Compare(transition.ToActivity.Key, actKey, true) == 0)
						list.Add(transition);

				foreach (IWfTransitionDescriptor transition in list)
					Remove(transition);
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
		public void RemoveByToActivity(IWfActivityDescriptor actDesp)
		{
			if (actDesp != null)
				RemoveByToActivity(actDesp.Key);
		}

		/// <summary>
		/// 查找没有使用的优先级(从8以后开始)
		/// </summary>
		/// <returns></returns>
		public int FindNotUsedPriority()
		{
			int result = 8;

			while (true)
			{
				bool notUsed = true;

				foreach (IWfTransitionDescriptor transition in base.List)
				{
					if (transition.Priority == result)
					{
						notUsed = false;
						result++;
						break;
					}
				}

				if (notUsed)
					break;
			}

			return result;
		}

		/// <summary>
		/// 得到可以抵达的线
		/// </summary>
		/// <returns></returns>
		public WfTransitionDescriptorCollection GetAllCanTransitTransitions()
		{
			WfTransitionDescriptorCollection result = CreateNewCollection();

			foreach (WfTransitionDescriptor transition in this)
			{
				if (transition.CanTransit())
					result.Add(transition);
			}

			return result;
		}
	}

	/// <summary>
	/// 线属性集合
	/// </summary>
	/// <remarks>记录下从每一点所发出的线，提供线的增加、根据ToActivity查找线</remarks>
	/// <scholiast>徐照雨</scholiast>
	[Serializable]
	public class ToTransitionsDescriptorCollection : WfTransitionDescriptorCollection
	{
		private IWfActivityDescriptor _FromActivity = null;

		internal ToTransitionsDescriptorCollection(IWfActivityDescriptor fromActivity)
		{
			_FromActivity = fromActivity;
		}

		/// <summary>
		/// 获取添加的线所指向的点toActivity，并添加线。
		/// </summary>
		/// <param name="activityKey">线指向的点（Key）</param>
		/// <param name="transition">添加的线</param>
		/// <remarks>
		/// 通过_FromActivity和activityKey得到toActivity点，是否可以直接传toActivity而不是activityKey?
		/// </remarks>
		/// <scholiast>徐照雨</scholiast>
		public void AddTransition(string activityKey, IWfTransitionDescriptor transition)
		{
			IWfActivityDescriptor toActivity = _FromActivity.Process.Activities[activityKey];
			ExceptionHelper.FalseThrow<WfDescriptorException>(
				toActivity != null,
				Resource.CanNotFoundActivityDescriptorByKey,
				activityKey);

			AddTransition(toActivity, transition);
		}

		/// <summary>
		/// 添加线
		/// </summary>
		/// <param name="toActivity">线所指向的点</param>
		/// <param name="transition">添加的线</param>
		/// <remarks>
		/// 处理过程：检验数据的有效性，将FromActivity和toActivity传给类WfTransitionDescriptor
		/// </remarks>
		/// <scholiast>徐照雨</scholiast>
		public void AddTransition(IWfActivityDescriptor toActivity, IWfTransitionDescriptor transition)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(toActivity != null, "toActivity");
			ExceptionHelper.FalseThrow<ArgumentNullException>(transition != null, "transition");

			ExceptionHelper.FalseThrow<WfDescriptorException>(
					_FromActivity.Process.Activities.Contains(toActivity),
					Resource.ActivityMustInProcessDescriptor);

			this.RWLock.AcquireWriterLock(lockTimeout);
			try
			{
				((WfTransitionDescriptor)transition).JoinActivity(_FromActivity, toActivity);

				base.Add(transition);
				toActivity.FromTransitions.Add(transition);
			}
			finally
			{
				this.RWLock.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// 找到优先级最高的线
		/// </summary>
		/// <param name="includeDisabled"></param>
		/// <returns></returns>
		public IWfTransitionDescriptor FindHighestPriorityTransition(bool includeDisabled)
		{
			IWfTransitionDescriptor result = null;

			int maxPriority = int.MaxValue;

			foreach (IWfTransitionDescriptor transition in this)
			{
				bool needCheck = includeDisabled || transition.Enabled;

				if (needCheck)
				{
					if (transition.Priority < maxPriority)
					{
						result = transition;
						maxPriority = transition.Priority;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="actKey"></param>
		/// <returns></returns>
		public IWfTransitionDescriptor GetTransitionByToActivity(string actKey)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(actKey, "actKey");

			WfTransitionDescriptor result = null;

			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				foreach (WfTransitionDescriptor transition in base.List)
				{
					if (transition.ToActivity.Key == actKey)
					{
						result = transition;
						break;
					}
				}

				return result;
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="actDesp"></param>
		/// <returns></returns>
		public IWfTransitionDescriptor GetTransitionByToActivity(IWfActivityDescriptor actDesp)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(actDesp != null, "actDesp");

			return GetTransitionByToActivity(actDesp.Key);
		}

		/// <summary>
		/// 得到可以流转的出线
		/// </summary>
		/// <returns></returns>
		public IList<IWfTransitionDescriptor> GetCanMovableTransitions()
		{
			List<IWfTransitionDescriptor> transitions = new List<IWfTransitionDescriptor>();

			foreach (IWfTransitionDescriptor t in this)
			{
				if (t.CanTransit())
					transitions.Add(t);
			}

			return transitions;
		}

		internal protected override WfTransitionDescriptorCollection CreateNewCollection()
		{
			return new ToTransitionsDescriptorCollection(this._FromActivity);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class FromTransitionsDescriptorCollection : WfTransitionDescriptorCollection
	{
		private IWfActivityDescriptor _ToActivity = null;

		internal FromTransitionsDescriptorCollection(IWfActivityDescriptor toActivity)
		{
			_ToActivity = toActivity;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="activityKey"></param>
		/// <param name="transition"></param>
		public void AddTransition(string activityKey, IWfTransitionDescriptor transition)
		{
			IWfActivityDescriptor fromActivity = _ToActivity.Process.Activities[activityKey];
			ExceptionHelper.FalseThrow<WfDescriptorException>(
				fromActivity != null,
				Resource.CanNotFoundActivityDescriptorByKey,
				activityKey);

			AddTransition(fromActivity, transition);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fromActivity"></param>
		/// <param name="transition"></param>
		public void AddTransition(IWfActivityDescriptor fromActivity, IWfTransitionDescriptor transition)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(fromActivity != null, "fromActivity");
			ExceptionHelper.FalseThrow<ArgumentNullException>(transition != null, "transition");
			ExceptionHelper.FalseThrow<WfDescriptorException>(
				_ToActivity.Process.Activities.Contains(fromActivity),
				Resource.ActivityMustInProcessDescriptor);

			this.RWLock.AcquireWriterLock(lockTimeout);
			try
			{
				((WfTransitionDescriptor)transition).JoinActivity(fromActivity, _ToActivity);

				base.Add(transition);
				fromActivity.ToTransitions.Add(transition);
			}
			finally
			{
				this.RWLock.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// 根据FromActivity(线的发出点)得到线
		/// </summary>
		/// <param name="actKey">FromActivity Key值</param>
		/// <returns>result</returns>
		/// <remarks>根据FromActivity(线的发出点)得到线</remarks>
		/// <scholiast>徐照雨</scholiast>
		public IWfTransitionDescriptor GetTransitionByFromActivity(string actKey)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(actKey, "actKey");

			IWfTransitionDescriptor result = null;

			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				foreach (IWfTransitionDescriptor transition in base.List)
				{
					if (transition.FromActivity.Key == actKey)
					{
						result = transition;
						break;
					}
				}

				return result;
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// 根据FromActivity(线的发出点)得到线
		/// </summary>
		/// <param name="actDesp">FromActivity</param>
		/// <returns>GetTransitionByFromActivity</returns>
		/// <remarks>根据FromActivity(线的发出点)得到线</remarks>
		/// <scholiast>徐照雨</scholiast>
		public IWfTransitionDescriptor GetTransitionByFromActivity(IWfActivityDescriptor actDesp)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(actDesp != null, "actDesp");

			return GetTransitionByFromActivity(actDesp.Key);
		}

		internal protected override WfTransitionDescriptorCollection CreateNewCollection()
		{
			return new FromTransitionsDescriptorCollection(this._ToActivity);
		}
	}

	/// <summary>
	/// 线上的条件计算所产生的异常
	/// </summary>
	[Serializable]
	public class WfTransitionEvaluationException : SystemSupportException
	{
		/// <summary>
		/// 
		/// </summary>
		public WfTransitionEvaluationException()
			: base()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public WfTransitionEvaluationException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public WfTransitionEvaluationException(string message, System.Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
