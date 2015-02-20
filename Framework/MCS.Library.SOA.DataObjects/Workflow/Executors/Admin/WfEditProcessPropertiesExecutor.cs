using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 编辑流程属性的执行器
	/// </summary>
	public class WfEditProcessPropertiesExecutor : WfEditPropertiesExecutorBase<IWfProcessDescriptor>
	{
		public WfEditProcessPropertiesExecutor(IWfActivity operatorActivity, IWfProcess process, IWfProcessDescriptor processDesp, bool syncMSObject)
			: base(operatorActivity, process, processDesp, syncMSObject, WfControlOperationType.EditProcessProperties)
		{
		}

		/// <summary>
		/// 找到对应的主线流程
		/// </summary>
		/// <returns></returns>
		protected override IWfProcessDescriptor FindMainStreamObject()
		{
			IWfProcessDescriptor result = null;

			if (this.Descriptor.IsMainStream == false)
				result = this.Process.MainStream;

			return result;
		}

		protected override void MergeMainStreamProperties(IWfProcessDescriptor processDesp)
		{
			string originalKey = processDesp.Properties.GetValue("Key", string.Empty);

			bool isMainStream = processDesp.IsMainStream;
			((WfProcessDescriptor)this.Descriptor).CopyPropertiesTo((WfProcessDescriptor)processDesp);
			((WfProcessDescriptor)processDesp).IsMainStream = isMainStream;

			processDesp.Properties.SetValue("Key", originalKey);
		}
	}
}
