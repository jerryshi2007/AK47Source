using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 创建活动的基类
	/// </summary>
	public abstract class WfActivityBuilderBase
	{
		public WfActivityBase CreateActivity(IWfActivityDescriptor actDesp)
		{
			WfActivityBase activity = CreateActivityInstance(actDesp);

			LoadActions(activity);

			return activity;
		}

		public static void LoadActions(IWfActivity activity)
		{
			WfActivityConfigurationElement activityElement = WfActivitySettings.GetConfig().Activities[activity.Descriptor.ActivityType.ToString()];

			if (WfRuntime.Parameters.AutoloadActions && activityElement != null)
			{
				activity.LeaveActions.CopyFrom(activityElement.GetLeaveActions());
				activity.EnterActions.CopyFrom(activityElement.GetEnterActions());
			}
		}

		/// <summary>
		/// 创建活动
		/// </summary>
		/// <param name="actDesp"></param>
		/// <returns></returns>
		protected abstract WfActivityBase CreateActivityInstance(IWfActivityDescriptor actDesp);
	}

	public class WfNormalActivityBuilder : WfActivityBuilderBase
	{
		protected override WfActivityBase CreateActivityInstance(IWfActivityDescriptor actDesp)
		{
			return new WfNormalActivity(actDesp);
		}
	}

	public class WfInitialActivityBuilder : WfActivityBuilderBase
	{
		protected override WfActivityBase CreateActivityInstance(IWfActivityDescriptor actDesp)
		{
			return new WfInitialActivity(actDesp);
		}
	}

	public class WfCompletedActivityBuilder : WfActivityBuilderBase
	{
		protected override WfActivityBase CreateActivityInstance(IWfActivityDescriptor actDesp)
		{
			return new WfCompletedActivity(actDesp);
		}
	}

	public class WfConditionalActivityBuilder : WfActivityBuilderBase
	{
		protected override WfActivityBase CreateActivityInstance(IWfActivityDescriptor actDesp)
		{
			throw new NotImplementedException();
		}
	}

	public class WfCompositeActivityBuilder : WfActivityBuilderBase
	{
		protected override WfActivityBase CreateActivityInstance(IWfActivityDescriptor actDesp)
		{
			throw new NotImplementedException();
		}
	}
}
