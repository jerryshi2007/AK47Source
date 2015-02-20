using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 管理员删除连线
	/// </summary>
	public class WfAdminDeleteTransitionExecutor : WfExecutorBase
	{
		private IWfTransitionDescriptor _TargetTransitionDescriptor = null;
		private IWfProcessDescriptor _ProcessDescriptor = null;
		private bool _SyncMainStreamObject = false;

		public WfAdminDeleteTransitionExecutor(IWfActivity operatorActivity, IWfProcessDescriptor processDescriptor, IWfTransitionDescriptor targetTransitionDescriptor, bool syncMSObject) :
			base(operatorActivity, WfControlOperationType.AdminDeleteTransition)
		{
			processDescriptor.NullCheck("processDescriptor");
			targetTransitionDescriptor.NullCheck("targetTransitionDescriptor");

			this._TargetTransitionDescriptor = targetTransitionDescriptor;
			this._ProcessDescriptor = processDescriptor;

			this._SyncMainStreamObject = syncMSObject;
		}

		protected override IWfProcess OnGetCurrentProcess()
		{
			return this._ProcessDescriptor.ProcessInstance;
		}

		protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
		{
			if (this._SyncMainStreamObject)
			{
				IWfTransitionDescriptor msTransition = FindMainStreamTransition(this._TargetTransitionDescriptor);

				if (msTransition != null)
					msTransition.FromActivity.ToTransitions.Remove(msTransition);
			}

			this._TargetTransitionDescriptor.FromActivity.ToTransitions.Remove(this._TargetTransitionDescriptor);

			WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(dataContext.CurrentProcess);
		}

		protected static IWfTransitionDescriptor FindMainStreamTransition(IWfTransitionDescriptor templateTransition)
		{
			IWfActivityDescriptor fromActDesp = templateTransition.FromActivity;
			IWfActivityDescriptor toActDesp = templateTransition.ToActivity;

			IWfTransitionDescriptor result = null;

			if (fromActDesp != null && toActDesp != null && fromActDesp.IsMainStreamActivity == false && toActDesp.IsMainStreamActivity == false)
			{
				IWfActivityDescriptor msFromActDesp = fromActDesp.Instance.GetMainStreamActivityDescriptor();
				IWfActivityDescriptor msToActDesp = toActDesp.Instance.GetMainStreamActivityDescriptor();

				if (msFromActDesp != null && msToActDesp != null)
				{
					result = msFromActDesp.ToTransitions.GetTransitionByToActivity(msToActDesp);
				}
			}

			return result;
		}
	}
}
