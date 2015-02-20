using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web.Services.Protocols;
using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Schemas.Client.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.Client.ServiceBroker;

namespace MCS.Library.SOA.DataObjects.Security.Client
{
	/// <summary>
	/// 调用服务的上下文
	/// </summary>
	[Serializable]
	[ActionContextDescription("PCServiceBrokerContext")]
	public class PCServiceBrokerContext : SchemaServiceBrokerContextBase<PCServiceBrokerContext>
	{
		protected override SchemaObjectServiceSettingsBase GetSettings()
		{
			return PCServiceClientSettings.GetConfig();
		}
	}
}
