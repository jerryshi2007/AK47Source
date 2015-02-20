using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;
using MCS.Library.WcfExtensions.Configuration;
using System.ServiceModel;

namespace MCS.Library.WcfExtensions
{
	public class WfServiceHostFactory : ServiceHostFactory
	{
		protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
		{
			ServiceHost host = null;

			WfServiceContractMapElement element = WfServiceContractMapSettings.GetConfig().Maps[serviceType.FullName];

			if (element == null || string.IsNullOrEmpty(element.ContractName))
			{
				//If not mapped, using tranditional host.
				host = new ServiceHost(serviceType, baseAddresses);
			}
			else
			{
				host = new WfServiceHost(serviceType, baseAddresses, element.ContractName, element.AtlasEnabled, element.BindingMode);

				if (element.Debug)
				{
					var dbgBehavior = host.Description.Behaviors.GetBehavior<ServiceDebugBehavior>();

					dbgBehavior.IncludeExceptionDetailInFaults = true;
				}
			}

			return host;
		}
	}
}
