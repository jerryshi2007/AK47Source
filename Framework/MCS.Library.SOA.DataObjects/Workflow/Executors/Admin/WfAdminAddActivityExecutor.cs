using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfAdminAddActivityExecutor : WfEditActivityPropertiesExecutor
	{
		private IWfActivityDescriptor _FromActivityDescriptor = null;
		private IWfActivityDescriptor _NewActivityDescriptor = null;

		public WfAdminAddActivityExecutor(IWfActivity operatorActivity,
			IWfProcess process,
			IWfActivityDescriptor fromActivityDesp,
			IWfActivityDescriptor newActivityDesp,
			bool syncMSObject)
			: base(operatorActivity, process, newActivityDesp, syncMSObject, WfControlOperationType.AdminAddActivity)
		{
			this._FromActivityDescriptor = fromActivityDesp;

			newActivityDesp.NullCheck("newActivityDesp");

			this._NewActivityDescriptor = newActivityDesp;
		}

		protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
		{
			if (this._FromActivityDescriptor != null)
				this._FromActivityDescriptor.ToTransitions.AddForwardTransition(this._NewActivityDescriptor);

			base.OnModifyWorkflow(dataContext);
		}

		protected override IWfActivityDescriptor FindMainStreamObject()
		{
			return AddMainStreamActivity();
		}

		private IWfActivityDescriptor AddMainStreamActivity()
		{
			WfActivityDescriptor newMSActDesp = null;

			if (this._NewActivityDescriptor.IsMainStreamActivity == false)
			{
				newMSActDesp = new WfActivityDescriptor(this.Process.MainStream.FindNotUsedActivityKey(),
					WfActivityType.NormalActivity);

				string originalKey = newMSActDesp.Key;

				((WfActivityDescriptor)this._NewActivityDescriptor).CloneProperties(newMSActDesp);
				newMSActDesp.Key = originalKey;

				((WfActivityBase)(this._NewActivityDescriptor.Instance)).MainStreamActivityKey = newMSActDesp.Key;

				IWfActivityDescriptor fromActDesp = this._FromActivityDescriptor;
				IWfActivityDescriptor msFromActDesp = null;

				if (fromActDesp != null)
					msFromActDesp = fromActDesp.Instance.GetMainStreamActivityDescriptor();

				this.Process.MainStream.Activities.Add(newMSActDesp);

				if (msFromActDesp != null)
					msFromActDesp.ToTransitions.AddForwardTransition(newMSActDesp);
			}

			return newMSActDesp;
		}
	}
}
