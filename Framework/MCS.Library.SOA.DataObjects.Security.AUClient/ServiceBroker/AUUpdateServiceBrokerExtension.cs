using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using System.Web.Services.Protocols;
using MCS.Library.SOA.DataObjects.Schemas.Client.ServiceBroker;

namespace MCS.Library.SOA.DataObjects.Security.AUClient.ServiceBroker
{
	public class AUUpdateServiceBrokerExtension : ServiceBrokerExtensionBase<AUUpdateServiceBrokerContext>
	{
		protected override AUUpdateServiceBrokerContext GetSerivceBrokerContext()
		{
			return AUUpdateServiceBrokerContext.Current;
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
