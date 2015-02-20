using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow.Builders
{
	/// <summary>
	/// 自由流程的创建器，根据一组人员创建流程，每个人员默认是流程中一个步骤
	/// </summary>
	public class WfFreeStepsProcessBuilder : WfProcessBuilderBase
	{
		private IUser[] _StepUsers = null;

		public WfFreeStepsProcessBuilder(string appName, string progName, params IUser[] stepUsers)
			: base(appName, progName)
		{
			stepUsers.NullCheck("stepUsers");

			this._StepUsers = stepUsers;
		}

		public override IWfProcessDescriptor Build(string processKey, string processName)
		{
			processKey.CheckStringIsNullOrEmpty("processKey");
			processName.CheckStringIsNullOrEmpty("processName");

			IWfProcessDescriptor processDesp = base.Build(processKey, processName);

			IWfActivityDescriptor currentActDesp = processDesp.InitialActivity;

			for (int i = 0; i < _StepUsers.Length; i++)
			{
				WfActivityDescriptor actDesp = new WfActivityDescriptor(processDesp.FindNotUsedActivityKey(), WfActivityType.NormalActivity);
				actDesp.ActivityType = WfActivityType.NormalActivity;

				actDesp.Name = _StepUsers[i].DisplayName;
				actDesp.Resources.Add(new WfUserResourceDescriptor(_StepUsers[i]));

				processDesp.Activities.Add(actDesp);

				WfTransitionDescriptor transitionDesp = (WfTransitionDescriptor)currentActDesp.ToTransitions.AddForwardTransition(actDesp);

				transitionDesp.Name = "审核";

				currentActDesp = actDesp;
			}

			currentActDesp.ToTransitions.AddForwardTransition(processDesp.CompletedActivity);

			return processDesp;
		}

		public IUser[] StepUsers
		{
			get
			{
				if (this._StepUsers == null)
					this._StepUsers = new IUser[0];

				return _StepUsers;
			}
		}
	}
}
