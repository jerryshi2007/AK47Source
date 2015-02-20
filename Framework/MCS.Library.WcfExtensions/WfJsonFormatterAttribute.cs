using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;

namespace MCS.Library.WcfExtensions
{
	public class WfJsonFormatterAttribute : Attribute, IOperationBehavior
	{
		public void AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
		{
			
		}

		public void ApplyClientBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.ClientOperation clientOperation)
		{
			
		}

		public void ApplyDispatchBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.DispatchOperation dispatchOperation)
		{
			dispatchOperation.Invoker = new WfServerContextInvoker(dispatchOperation.Invoker);
		}

		public void Validate(OperationDescription operationDescription)
		{
			
		}
	}
}
