using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Schemas.Client.ServiceBroker;

namespace MCS.Library.SOA.DataObjects.Security.Client
{
	public class PCUpdateServiceBrokerExtension : ServiceBrokerExtensionBase<PCUpdateServiceBrokerContext>
	{
		protected override PCUpdateServiceBrokerContext GetSerivceBrokerContext()
		{
			return PCUpdateServiceBrokerContext.Current;
		}

		public override void ProcessMessage(SoapMessage message)
		{
			base.ProcessMessage(message);
			if (message.Stage == SoapMessageStage.BeforeSerialize)
			{
				message.Headers.Add(new SchemaServiceUpdatableClientSoapHeader() { WithLock = this.GetSerivceBrokerContext().WithLock });
			}
		}
	}
}
