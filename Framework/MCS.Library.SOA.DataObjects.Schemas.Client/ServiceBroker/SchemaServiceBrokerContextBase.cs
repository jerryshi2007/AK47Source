using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Schemas.Client.Configuration;

namespace MCS.Library.SOA.DataObjects.Schemas.Client.ServiceBroker
{
	public abstract class SchemaServiceBrokerContextBase<T> : ServiceBrokerContextBase<T> where T : SchemaServiceBrokerContextBase<T>, new()
	{
		protected override void InitProperties()
		{
			SchemaObjectServiceSettingsBase settings = GetSettings();

			this.Timeout = settings.Timeout;
			this.UseLocalCache = settings.UseLocalCache;
			this.UseServerCache = settings.UseServerCache;

			foreach (OguConnectionMappingElement cm in settings.ConnectionMappings)
				this.ConnectionMappings[cm.Name] = cm.Destination;
		}

		protected abstract SchemaObjectServiceSettingsBase GetSettings();
	}
}
