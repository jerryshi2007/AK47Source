using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Schemas.Client.ServiceBroker;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.AUClient.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.Client.Configuration;

namespace MCS.Library.SOA.DataObjects.Security.AUClient.ServiceBroker
{
	[Serializable]
	[ActionContextDescription("AUUpdateServiceBrokerContext")]
	public class AUUpdateServiceBrokerContext : SchemaServiceBrokerContextBase<AUUpdateServiceBrokerContext>
	{
		private bool _WithLock;

		public bool WithLock
		{
			get
			{
				return _WithLock;
			}

			set
			{
				this._WithLock = value;
			}
		}

		protected override void InitProperties()
		{
			base.InitProperties();

			this.WithLock = AUServiceClientSettings.GetConfig().WithLock;
		}

		protected override SchemaObjectServiceSettingsBase GetSettings()
		{
			return AUServiceClientSettings.GetConfig();
		}
	}
}
