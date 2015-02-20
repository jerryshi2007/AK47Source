using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 管理员添加连线并且设置其属性的操作
	/// </summary>
	public class WfAdminAddTransitionExecutor : WfEditTransitionPropertiesExecutor
	{
		public WfAdminAddTransitionExecutor(IWfActivity operatorActivity, IWfProcess process, IWfTransitionDescriptor transitionDesp, bool syncMSObject)
			: base(operatorActivity, process, transitionDesp, syncMSObject, WfControlOperationType.AdminAddTransition)
		{

		}

		protected override IWfTransitionDescriptor FindMainStreamObject()
		{
			IWfTransitionDescriptor result = base.FindMainStreamObject();

			if (result == null)
				result = AddMainStreamTransition();

			return result;
		}

		private IWfTransitionDescriptor AddMainStreamTransition()
		{
			IWfActivityDescriptor fromActDesp = this.Descriptor.FromActivity;
			IWfActivityDescriptor toActDesp = this.Descriptor.ToActivity;

			IWfTransitionDescriptor result = null;

			if (fromActDesp != null && toActDesp != null && fromActDesp.IsMainStreamActivity == false && toActDesp.IsMainStreamActivity == false)
			{
				IWfActivityDescriptor msFromActDesp = fromActDesp.Instance.GetMainStreamActivityDescriptor();
				IWfActivityDescriptor msToActDesp = toActDesp.Instance.GetMainStreamActivityDescriptor();

				if (msFromActDesp != null && msToActDesp != null)
				{
					result = msFromActDesp.ToTransitions.AddForwardTransition(msToActDesp);
				}
			}

			return result;
		}
	}
}
