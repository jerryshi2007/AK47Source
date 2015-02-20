using System;
using System.Configuration;
using MCS.Library.Configuration;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Schemas.Client.Configuration;

namespace MCS.Library.SOA.DataObjects.Security.Client
{
	public sealed class PCServiceClientSettings : SchemaObjectServiceSettingsBase
	{
		public static PCServiceClientSettings GetConfig()
		{
			PCServiceClientSettings settings = (PCServiceClientSettings)ConfigurationBroker.GetSection("pcServiceClientSettings");

			ConfigurationExceptionHelper.CheckSectionNotNull(settings, "pcServiceClientSettings");

			return settings;
		}

		private PCServiceClientSettings()
		{
		}

		/// <summary>
		/// 机构人员管理的WebService的地址
		/// </summary>
		public Uri QueryServiceAddress
		{
			get
			{
				return this.Paths["queryServiceAddress"].Uri;
			}
		}

		/// <summary>
		/// 更新的WebService的地址
		/// </summary>
		public Uri UpdateServiceAddress
		{
			get
			{
				return this.Paths["updateServiceAddress"].Uri;
			}
		}
	}
}
