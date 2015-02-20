using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;

namespace MCS.Library.SOA.DataObjects.Security.AUClient.ServiceBroker
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	sealed class AUUpdateServiceBrokerExtensionAttribute : SoapExtensionAttribute
	{
		private int priority;

		public override Type ExtensionType
		{
			get { return typeof(AUUpdateServiceBrokerExtension); }
		}

		public override int Priority
		{
			get
			{
				return priority;
			}
			set
			{
				this.priority = value;
			}
		}
	}
}
