using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 添加主线流程活动
	/// </summary>
	public class WfAddActivityExecutor : WfAddAndEditActivityExecutorBase
	{
		public WfAddActivityExecutor(IWfActivity operatorActivity, IWfActivity targetActivity, WfActivityDescriptorCreateParams createParams)
			: base(operatorActivity, targetActivity, createParams, WfControlOperationType.AddActivity)
		{
		}

		/// <summary>
		/// 新增的活动
		/// </summary>
		public IWfActivity AddedActivity
		{
			get;
			private set;
		}

		protected override IWfActivity PrepareInstanceActivity()
		{
			ExceptionHelper.TrueThrow(this.TargetActivity.Descriptor.ActivityType == WfActivityType.CompletedActivity, "不能在结束活动后添加活动");

			IWfProcess process = this.TargetActivity.Process;
			IWfActivityDescriptor newActDesp = PrepareActivityDescriptor(this.TargetActivity.Descriptor);

			IWfActivity newActivity = WfActivityBase.CreateActivityInstance(newActDesp, process);

			if (WfRuntime.ProcessContext != null)
				WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(process);

			this.AddedActivity = newActivity;

			return newActivity;
		}

		protected override IWfActivityDescriptor PrepareActivityDescriptor(IWfActivityDescriptor targetActDesp)
		{
			IWfProcessDescriptor processDesp = targetActDesp.Process;

			string activityKey = processDesp.FindNotUsedActivityKey();

			WfActivityDescriptor newActDesp = new WfActivityDescriptor(activityKey);
			newActDesp.ActivityType = WfActivityType.NormalActivity;

			//主线活动不应该有关联Key
			newActDesp.AssociatedActivityKey = WfRuntime.ProcessContext.ActivityChangingContext.AssociatedActivityKey;
			newActDesp.FromTransitions.Clear();
			newActDesp.ToTransitions.Clear();

			processDesp.Activities.Add(newActDesp);
			newActDesp.Properties.SetValue("AutoMoveAfterPending", true);

			IWfTransitionDescriptor defaultSelectTran = targetActDesp.ToTransitions.FindDefaultSelectTransition();

			List<IWfTransitionDescriptor> movedTransitions = new List<IWfTransitionDescriptor>();

			foreach (WfTransitionDescriptor t in targetActDesp.ToTransitions)
			{
				WfTransitionDescriptor cloneTransition = (WfTransitionDescriptor)t.Clone();
				cloneTransition.Key = processDesp.FindNotUsedTransitionKey();

				if (t.IsBackward == false)
				{
					cloneTransition.JoinActivity(newActDesp, t.ToActivity);
					newActDesp.ToTransitions.Add(cloneTransition);

					movedTransitions.Add(t);

					t.ToActivity.FromTransitions.Remove(td => td.Key == t.Key);
					t.ToActivity.FromTransitions.Add(cloneTransition);

					if (t.Key == defaultSelectTran.Key)
					{
						t.Key = processDesp.FindNotUsedTransitionKey();
						t.JoinActivity(targetActDesp, newActDesp);
						newActDesp.FromTransitions.Add(t);
					}
				}
				else
				{
					cloneTransition.JoinActivity(newActDesp, t.ToActivity);
					newActDesp.ToTransitions.Add(cloneTransition);

					movedTransitions.Add(t);
				}
			}

			foreach (WfTransitionDescriptor t in movedTransitions)
				targetActDesp.ToTransitions.Remove(t);

			SetDynamicActivityProperties(targetActDesp, newActDesp);

			IWfTransitionDescriptor newTransition = targetActDesp.ToTransitions.AddForwardTransition(newActDesp);

			SetDynamicTransitionProperties(targetActDesp, newTransition);

			return newActDesp;
		}

		/// <summary>
		/// 设置动态活动的属性
		/// </summary>
		/// <param name="targetActDesp"></param>
		/// <param name="newActDesp"></param>
		private static void SetDynamicActivityProperties(IWfActivityDescriptor targetActDesp, IWfActivityDescriptor newActDesp)
		{
			((WfActivityDescriptor)newActDesp).GeneratedByTemplate = targetActDesp.GeneratedByTemplate;
			((WfActivityDescriptor)newActDesp).TemplateKey = targetActDesp.TemplateKey;
		}

		/// <summary>
		/// 设置动态活动产生的线属性
		/// </summary>
		/// <param name="targetActDesp"></param>
		/// <param name="newTransition"></param>
		private static void SetDynamicTransitionProperties(IWfActivityDescriptor targetActDesp, IWfTransitionDescriptor newTransition)
		{
			((WfTransitionDescriptor)newTransition).GeneratedByTemplate = targetActDesp.GeneratedByTemplate;
			((WfTransitionDescriptor)newTransition).TemplateKey = targetActDesp.TemplateKey;
		}
	}
}
