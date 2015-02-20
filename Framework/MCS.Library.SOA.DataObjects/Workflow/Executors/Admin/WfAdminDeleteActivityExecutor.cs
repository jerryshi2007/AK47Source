using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// Admin delete activity
	/// </summary>
	public class WfAdminDeleteActivityExecutor : WfExecutorBase
	{
		private IWfActivityDescriptor _TargetActivityDescriptor = null;
		private bool _SyncMainStreamObject = false;

		public WfAdminDeleteActivityExecutor(IWfActivity operatorActivity, IWfActivityDescriptor targetActivityDescriptor, bool syncMSObject) :
			base(operatorActivity, WfControlOperationType.AdminDeleteActivity)
		{
			targetActivityDescriptor.NullCheck("targetActivityDescriptor");

			this._TargetActivityDescriptor = targetActivityDescriptor;
			this._SyncMainStreamObject = syncMSObject;
		}

		public IWfActivityDescriptor TargetActivityDescriptor
		{
			get
			{
				return this._TargetActivityDescriptor;
			}
		}

		protected override IWfProcess OnGetCurrentProcess()
		{
			return this._TargetActivityDescriptor.ProcessInstance;
		}

		protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
		{
			(this.TargetActivityDescriptor.ActivityType == WfActivityType.NormalActivity).FalseThrow("只能删除正常的活动，不能删除起始或结束点");

			if (this.TargetActivityDescriptor.IsMainStreamActivity == false)
			{
				(this._TargetActivityDescriptor.Instance.Status == WfActivityStatus.NotRunning).FalseThrow("不能删除已经启动过的活动");

				if (this._SyncMainStreamObject)
				{
					IWfActivityDescriptor msActDesp = this._TargetActivityDescriptor.Instance.GetMainStreamActivityDescriptor();

					if (msActDesp != null)
						msActDesp.Remove();
				}

				this.TargetActivityDescriptor.Instance.Remove();
			}
			else
			{
				this.TargetActivityDescriptor.Remove();
			}

			WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(dataContext.CurrentProcess);
		}
	}
}
