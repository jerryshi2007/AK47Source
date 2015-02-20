using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;

namespace MCS.Library.SOA.DataObjects.Security.AUClient.ServiceBroker
{
	[AttributeUsage(AttributeTargets.Method)]
	public class AUServiceBrokerExtensionAttribute : SoapExtensionAttribute
	{
		public override Type ExtensionType
		{
			get { return typeof(AUServiceBrokerExtension); }
		}

		public override int Priority { get; set; }
	}
}
