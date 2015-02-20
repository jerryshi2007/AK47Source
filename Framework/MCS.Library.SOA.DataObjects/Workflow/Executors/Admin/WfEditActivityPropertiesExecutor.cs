using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfEditActivityPropertiesExecutor : WfEditPropertiesExecutorBase<IWfActivityDescriptor>
	{
		public WfEditActivityPropertiesExecutor(IWfActivity operatorActivity, IWfProcess process, IWfActivityDescriptor actDesp, bool syncMSObject)
			: base(operatorActivity, process, actDesp, syncMSObject, WfControlOperationType.EditActivityProperties)
		{
		}

		protected WfEditActivityPropertiesExecutor(IWfActivity operatorActivity, IWfProcess process, IWfActivityDescriptor actDesp, bool syncMSObject, WfControlOperationType operationType)
			: base(operatorActivity, process, actDesp, syncMSObject, operationType)
		{
		}

		/// <summary>
		/// 找到主线活动中对应的活动
		/// </summary>
		/// <returns></returns>
		protected override IWfActivityDescriptor FindMainStreamObject()
		{
			IWfActivityDescriptor result = null;

			if (this.Descriptor.IsMainStreamActivity == false)
				result = this.Descriptor.Instance.GetMainStreamActivityDescriptor();

			return result;
		}

		protected override void MergeMainStreamProperties(IWfActivityDescriptor targetDescriptor)
		{
			string originalKey = targetDescriptor.Properties.GetValue("Key", string.Empty);

			IWfProcessDescriptor originalProcessDesp = targetDescriptor.Process;

			((WfActivityDescriptor)this.Descriptor).CopyPropertiesTo((WfActivityDescriptor)targetDescriptor);
			((WfActivityDescriptor)targetDescriptor).Process = originalProcessDesp;

			targetDescriptor.Properties.SetValue("Key", originalKey);
		}
	}
}
