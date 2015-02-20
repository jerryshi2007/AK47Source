using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Security.AUClient.ServiceBroker
{
	public class AUServiceBrokerExtension : ServiceBrokerExtensionBase<AUServiceBrokerContext>
	{
		protected override AUServiceBrokerContext GetSerivceBrokerContext()
		{
			return AUServiceBrokerContext.Current;
		}
	}
}
