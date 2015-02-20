using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Schemas.Client.Configuration;

namespace MCS.Library.SOA.DataObjects.Security.AUClient.Configuration
{
	public sealed class AUServiceClientSettings : SchemaObjectServiceSettingsBase
	{
		public static AUServiceClientSettings GetConfig()
		{
			AUServiceClientSettings settings = (AUServiceClientSettings)ConfigurationBroker.GetSection("auServiceClientSettings");

			ConfigurationExceptionHelper.CheckSectionNotNull(settings, "auServiceClientSettings");

			return settings;
		}

		private AUServiceClientSettings()
		{
		}

		/// <summary>
		/// 查询接口的WebService的地址
		/// </summary>
		public Uri QueryServiceAddress
		{
			get
			{
				return this.Paths["queryServiceAddress"].Uri;
			}
		}

		/// <summary>
		/// 更新接口的WebService的地址
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
