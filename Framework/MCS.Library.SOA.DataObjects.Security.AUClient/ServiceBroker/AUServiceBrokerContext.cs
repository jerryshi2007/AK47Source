using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Schemas.Client.ServiceBroker;
using MCS.Library.SOA.DataObjects.Security.AUClient.Configuration;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.AUClient.ServiceBroker
{
	[Serializable]
	[ActionContextDescription("AUServiceBrokerContext")]
	public class AUServiceBrokerContext : SchemaServiceBrokerContextBase<AUServiceBrokerContext>
	{
		protected override Schemas.Client.Configuration.SchemaObjectServiceSettingsBase GetSettings()
		{
			return AUServiceClientSettings.GetConfig();
		}
	}
}
