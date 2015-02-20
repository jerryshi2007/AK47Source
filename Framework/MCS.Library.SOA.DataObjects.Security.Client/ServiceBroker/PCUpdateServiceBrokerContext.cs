using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Schemas.Client.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.Client.ServiceBroker;

namespace MCS.Library.SOA.DataObjects.Security.Client
{
	/// <summary>
	/// 用于更新服务的ServiceContext
	/// </summary>
	[Serializable]
	[ActionContextDescription("PCUpdateServiceBrokerContext")]
	public class PCUpdateServiceBrokerContext : SchemaServiceBrokerContextBase<PCUpdateServiceBrokerContext>
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

			this.WithLock = PCServiceClientSettings.GetConfig().WithLock;
		}

		protected override SchemaObjectServiceSettingsBase GetSettings()
		{
			return PCServiceClientSettings.GetConfig();
		}
	}
}
