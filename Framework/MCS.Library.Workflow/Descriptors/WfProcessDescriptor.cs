using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MCS.Library.Core;
using MCS.Library.Workflow.Properties;
using System.Security.Permissions;
using MCS.Library.Workflow.Engine;

namespace MCS.Library.Workflow.Descriptors
{
	/// <summary>
	/// 工作流描述
	/// </summary>
	/// <remarks>包含的工作流的所有属性，OA所特有的属性在OAWfProcessDescriptor中。
	/// Activities点属性集合、Variables变量列表属性集合、Version版本、
	/// ExtendedProperties扩展属性为今后添加新属性预留  
	/// </remarks>
	/// <scholiast>徐照雨</scholiast>
	[Serializable]
	public class WfProcessDescriptor : WfDescriptorBase, IWfProcessDescriptor
	{
		/// <summary>
		/// 
		/// </summary>
		private WfActivityDescriptorCollection _Activities = null;

		/// <summary>
		/// 
		/// </summary>
		private WfVariableDescriptorCollection _Variables = new WfVariableDescriptorCollection();

		/// <summary>
		/// 
		/// </summary>
		private string _Version = "0.0";

		/// <summary>
		/// 
		/// </summary>
		private WfExtendedPropertyDictionary _ExtendedProperties = null;

		/// <summary>
		/// 
		/// </summary>
		public WfProcessDescriptor()
		{
		}
		/// <summary>
		/// 工作流Key值
		/// </summary>
		/// <param name="key">Key值</param>
		/// <scholiast>徐照雨</scholiast>
		public WfProcessDescriptor(string key)
			: base(key)
		{
		}

		#region ISerializable Members
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("Activities", this._Activities, typeof(WfActivityDescriptorCollection));
			info.AddValue("Variables", this._Variables, typeof(WfVariableDescriptorCollection));
			info.AddValue("Version", this._Version);
			info.AddValue("ExtendedProperties", _ExtendedProperties, typeof(WfExtendedPropertyDictionary));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected WfProcessDescriptor(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this._Activities = (WfActivityDescriptorCollection)info.GetValue("Activities", typeof(WfActivityDescriptorCollection));
			this._Variables = (WfVariableDescriptorCollection)info.GetValue("Variables", typeof(WfVariableDescriptorCollection));
			this._Version = info.GetString("Version");
			this._ExtendedProperties = (WfExtendedPropertyDictionary)info.GetValue("ExtendedProperties", typeof(WfExtendedPropertyDictionary));
		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		public string Version
		{
			get
			{
				return this._Version;
			}
			set
			{
				this._Version = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public WfExtendedPropertyDictionary ExtendedProperties
		{
			get
			{
				if (_ExtendedProperties == null)
					_ExtendedProperties = new WfExtendedPropertyDictionary();

				return _ExtendedProperties;
			}
		}

		/// <summary>
		/// 结束点发出的线
		/// </summary>
		/// <remarks>
		/// 所有的线都包含在点的属性内
		/// 还可以通过Activities["Completed"]访问到CompletedActivity
		/// </remarks>
		/// <scholiast>徐照雨</scholiast>
		public IWfCompletedActivityDescriptor CompletedActivity
		{
			get
			{
				return Activities.CompletedActivity;
			}
		}

		/// <summary>
		/// 初始点发出的线
		/// </summary>
		/// <remarks>
		/// 所有的线都包含在点的属性内
		/// 还可以通过Activities["Initial"]访问到InitialActivity
		/// </remarks>
		/// <scholiast>徐照雨</scholiast>
		public IWfInitialActivityDescriptor InitialActivity
		{
			get
			{
				return Activities.InitialActivity;
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
		/// <summary>
		/// 实例化流程点属性集合
		/// </summary>
		/// <remarks>如果是第一次添加流程点，实例化流程点属性集合。再添加点直接返回_Activites</remarks>
		/// <scholiast>徐照雨</scholiast>
		public WfActivityDescriptorCollection Activities
		{
			get
			{
				if (_Activities == null)
					_Activities = CreateActivityCollection();

				return _Activities;
			}
		}

		public WfActivityLevelGroupCollection GetAllBranchesLevels()
		{
			return GetAllBranchesLevels(true);
		}

		internal WfActivityLevelGroupCollection GetAllBranchesLevels(bool autoCalcaulatePath, IWfProcess process)
		{
			List<IWfActivityDescriptor> alreadyScanedActivities = new List<IWfActivityDescriptor>();

			FindAllNextStepActivityDescriptor(this.InitialActivity, autoCalcaulatePath, process, alreadyScanedActivities);

			WfActivityLevelGroupCollection result = new WfActivityLevelGroupCollection(alreadyScanedActivities);

			return result;
		}

		public WfActivityLevelGroupCollection GetAllBranchesLevels(bool autoCalcaulatePath)
		{
			return GetAllBranchesLevels(autoCalcaulatePath, null);
		}

		internal WfActivityLevelGroupCollection GetAllLevels(bool autoCalcaulatePath, IWfProcess process)
		{
			List<IWfActivityDescriptor> alreadyScanedActivities = new List<IWfActivityDescriptor>();

			FindNextStepActivityDescriptor(this.InitialActivity, autoCalcaulatePath, process, alreadyScanedActivities);

			WfActivityLevelGroupCollection result = new WfActivityLevelGroupCollection(alreadyScanedActivities);

			//寻找那些在线上不能达到，但是又属于某个环节的
			foreach (IWfActivityDescriptor actDesp in this.Activities)
			{
				try
				{
					WfActivityLevelGroup group = result[actDesp.LevelName];

					if (group.Data.Exists(actInGroup => actInGroup.Key == actDesp.Key) == false)
						group.Data.Add(actDesp);
				}
				catch (KeyNotFoundException)
				{
				}
			}

			return result;
		}

		public WfActivityLevelGroupCollection GetAllLevels()
		{
			return GetAllLevels(true);
		}

		public WfActivityLevelGroupCollection GetAllLevels(bool autoCalcaulatePath)
		{
			return GetAllLevels(autoCalcaulatePath, null);
		}

		/// <summary>
		/// 自动计算出一个没有用过的ActivityKey
		/// </summary>
		/// <returns></returns>
		public string FindNotUsedActivityKey()
		{
			int i = 0;

			string result = string.Empty;

			while (true)
			{
				result = "N" + i;

				if (this.Activities[result] == null)
					break;

				i++;
			}

			return result;
		}

		public string FindNotUsedLevelName()
		{
			int i = 0;

			string result = string.Empty;

			while (true)
			{
				result = "L" + i;

				if (this.Activities.Exists(actDesp => actDesp.LevelName == result) == false)
					break;

				i++;
			}

			return result;
		}

		/// <summary>
		/// 自动计算出一个没有用过的TransitionKey
		/// </summary>
		/// <returns></returns>
		public string FindNotUsedTransitionKey()
		{
			int i = 0;

			string result = string.Empty;

			while (true)
			{
				result = "L" + i;

				bool found = false;

				foreach (IWfActivityDescriptor actDesp in this.Activities)
				{
					if (actDesp.ToTransitions[result] != null || actDesp.FromTransitions[result] != null)
					{
						found = true;
						break;
					}
				}

				if (found == false)
					break;

				i++;
			}

			return result;
		}

		/// <summary>
		/// 根据Key查找Transition
		/// </summary>
		/// <param name="transitionKey"></param>
		/// <returns></returns>
		public IWfTransitionDescriptor FindTransitionByKey(string transitionKey)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(transitionKey, "transitionKey");

			IWfTransitionDescriptor result = null;

			foreach (IWfActivityDescriptor actDesp in this.Activities)
			{
				result = actDesp.ToTransitions[transitionKey];

				if (result != null)
					break;
			}

			return result;
		}

		/// <summary>
		/// 创建Activity集合
		/// </summary>
		/// <returns></returns>
		protected virtual WfActivityDescriptorCollection CreateActivityCollection()
		{
			return new WfActivityDescriptorCollection(this);
		}

		private static void FindAllNextStepActivityDescriptor(
			IWfActivityDescriptor currentAct,
			bool autoCalcaulatePath,
			IWfProcess process,
			List<IWfActivityDescriptor> alreadyScanedActivities)
		{
			try
			{
				foreach (IWfTransitionDescriptor transition in currentAct.ToTransitions)
				{
					if (autoCalcaulatePath == false || transition.CanTransit())
					{
						AddTransitionToScanedActivities(transition, autoCalcaulatePath, process, alreadyScanedActivities);
					}
				}
			}
			catch (WfTransitionEvaluationException ex)
			{
				IWfTransitionDescriptor transition = FindNearestPassedTransitionDescriptor(process, currentAct.ToTransitions);

				if (transition != null)
				{
					AddTransitionToScanedActivities(transition, autoCalcaulatePath, process, alreadyScanedActivities);
				}
				else
					throw ex;
			}
		}

		private static void AddTransitionToScanedActivities(IWfTransitionDescriptor transition, bool autoCalcaulatePath, IWfProcess process, List<IWfActivityDescriptor> alreadyScanedActivities)
		{
			IWfActivityDescriptor target = transition.ToActivity;

			if ((target is IWfInitialActivityDescriptor == false) && (target is IWfCompletedActivityDescriptor == false))
			{
				if (alreadyScanedActivities.Exists(a => a.Key == target.Key) == false)
				{
					if (string.IsNullOrEmpty(target.LevelName) == false)
					{
						alreadyScanedActivities.Add(target);
						FindAllNextStepActivityDescriptor(target, autoCalcaulatePath, process, alreadyScanedActivities);
					}
				}
			}
		}

		/// <summary>
		/// 在线中，查找最接近的经过的线
		/// </summary>
		/// <param name="process"></param>
		/// <param name="transitionDesps"></param>
		/// <returns></returns>
		private static IWfTransitionDescriptor FindNearestPassedTransitionDescriptor(IWfProcess process, WfTransitionDescriptorCollection transitionDesps)
		{
			IWfTransitionDescriptor result = null;

			if (process != null)
			{
				IWfActivity currentActivity = process.LastActivity;

				while (currentActivity != null)
				{
					IWfTransitionDescriptor fromTransitionDescriptor = currentActivity.FromTransitionDescriptor;

					if (fromTransitionDescriptor != null)
					{
						if (transitionDesps.Exists(t => t.Key == fromTransitionDescriptor.Key))
						{
							result = fromTransitionDescriptor;
							break;
						}
					}

					if (currentActivity.FromTransition != null)
						currentActivity = currentActivity.FromTransition.FromActivity;
					else
						currentActivity = null;
				}
			}

			return result;
		}

		private static void FindNextStepActivityDescriptor(
			IWfActivityDescriptor currentAct,
			bool autoCalcaulatePath,
			IWfProcess process,
			List<IWfActivityDescriptor> alreadyScanedActivities)
		{
			IWfActivityDescriptor act = null;

			int maxPriority = 255;

			WfTransitionDescriptorCollection transitions = null;

			if (autoCalcaulatePath == false)
				transitions = currentAct.ToTransitions;
			else
				transitions = FindCanTransitTransitions(process, currentAct.ToTransitions);

			IWfActivityDescriptor target = GetTransitionValidTarget(transitions.FindDefaultSelectTransition());

			if (target != null)
			{
				if (alreadyScanedActivities.Exists(a => a.Key == target.Key) == false)
					act = target;
			}

			if (act == null)
			{
				foreach (IWfTransitionDescriptor transition in transitions)
				{
					target = GetTransitionValidTarget(transition);

					if (target != null)
					{
						if (alreadyScanedActivities.Exists(a => a.Key == target.Key) == false)
						{
							if (string.IsNullOrEmpty(target.LevelName) == false)
							{
								if (transition.Priority < maxPriority)
								{
									act = target;
									maxPriority = transition.Priority;
								}
							}
						}
					}
				}
			}

			if (act != null)
			{
				if (act is IWfCompletedActivityDescriptor == false)
				{
					alreadyScanedActivities.Add(act);
					FindNextStepActivityDescriptor(act, autoCalcaulatePath, process, alreadyScanedActivities);
				}
			}
		}

		private static WfTransitionDescriptorCollection FindCanTransitTransitions(IWfProcess process, WfTransitionDescriptorCollection transitions)
		{
			WfTransitionDescriptorCollection result = null;

			if (WfContext.Current.AutoFindPassedTransitions)
			{
				IWfTransitionDescriptor transition = FindNearestPassedTransitionDescriptor(process, transitions);

				result = transitions.CreateNewCollection();

				if (transition != null)
				{
					result = transitions.CreateNewCollection();
					result.Add(transition);
				}
				else
					result = transitions.GetAllCanTransitTransitions();
			}
			else
			{
				try
				{
					result = transitions.GetAllCanTransitTransitions();
				}
				catch (WfTransitionEvaluationException ex)
				{
					IWfTransitionDescriptor transition = FindNearestPassedTransitionDescriptor(process, transitions);

					if (transition != null)
					{
						result = transitions.CreateNewCollection();
						result.Add(transition);
					}
					else
						throw ex;
				}
			}

			return result;
		}

		private static IWfActivityDescriptor GetTransitionValidTarget(IWfTransitionDescriptor transition)
		{
			IWfActivityDescriptor actDesp = null;

			if (transition != null)
			{
				actDesp = transition.ToActivity;

				//if ((actDesp is IWfInitialActivityDescriptor) || (actDesp is IWfCompletedActivityDescriptor))
				//	actDesp = null;
				if ((actDesp is IWfInitialActivityDescriptor))
					actDesp = null;
			}

			return actDesp;
		}
	}
}
