using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfEditTransitionPropertiesExecutor : WfEditPropertiesExecutorBase<IWfTransitionDescriptor>
	{
		public WfEditTransitionPropertiesExecutor(IWfActivity operatorActivity, IWfProcess process, IWfTransitionDescriptor transitionDesp, bool syncMSObject)
			: base(operatorActivity, process, transitionDesp, syncMSObject, WfControlOperationType.EditTransitionProperties)
		{

		}

		protected WfEditTransitionPropertiesExecutor(IWfActivity operatorActivity, IWfProcess process, IWfTransitionDescriptor transitionDesp, bool syncMSObject, WfControlOperationType operationType)
			: base(operatorActivity, process, transitionDesp, syncMSObject, operationType)
		{
		}

		protected override IWfTransitionDescriptor FindMainStreamObject()
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
					result = msFromActDesp.ToTransitions.GetTransitionByToActivity(msToActDesp);
				}
			}

			return result;
		}

		protected override void MergeMainStreamProperties(IWfTransitionDescriptor targetDescriptor)
		{
			((WfTransitionDescriptor)this.Descriptor).CloneProperties((WfKeyedDescriptorBase)targetDescriptor);
		}
	}
}
