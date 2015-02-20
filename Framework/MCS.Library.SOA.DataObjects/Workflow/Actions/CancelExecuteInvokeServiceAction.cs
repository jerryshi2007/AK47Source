using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow.Actions
{
	[Serializable]
	public class CancelExecuteInvokeServiceAction : IWfAction
	{
		public void PrepareAction(WfActionParams actionParams)
		{
			if (WfRuntime.ProcessContext.EnableServiceCall)
			{
				if (WfRuntime.ProcessContext.CurrentProcess != null)
				{
					foreach (WfServiceOperationDefinition svcDefinition in WfRuntime.ProcessContext.CurrentProcess.Descriptor.CancelBeforeExecuteServices)
					{
						WfServiceInvoker svcInvoker = new WfServiceInvoker(svcDefinition);
						object result = svcInvoker.Invoke();

						if (svcDefinition.RtnXmlStoreParamName.IsNotEmpty())
							WfRuntime.ProcessContext.CurrentProcess.ApplicationRuntimeParameters[svcDefinition.RtnXmlStoreParamName] = result;
					}
				}
			}
		}

		public void PersistAction(WfActionParams actionParams)
		{
			if (WfRuntime.ProcessContext.EnableServiceCall)
			{
				if (WfRuntime.ProcessContext.CurrentProcess != null)
				{
					foreach (WfServiceOperationDefinition svcDefinition in WfRuntime.ProcessContext.CurrentProcess.Descriptor.CancelAfterExecuteServices)
					{
						WfServiceInvoker svcInvoker = new WfServiceInvoker(svcDefinition);
						object result = svcInvoker.Invoke();

						if (svcDefinition.RtnXmlStoreParamName.IsNotEmpty())
							WfRuntime.ProcessContext.CurrentProcess.ApplicationRuntimeParameters[svcDefinition.RtnXmlStoreParamName] = result;
					}
				}
			}
		}

		public void ClearCache()
		{

		}
	}
}
