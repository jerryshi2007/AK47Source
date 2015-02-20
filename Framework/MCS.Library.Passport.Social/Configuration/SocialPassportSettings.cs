using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Core;
using MCS.Library.Configuration;

namespace MCS.Library.Passport.Social.Configuration
{
	/// <summary>
	/// 社交网络登录的配置信息
	/// </summary>
	public class SocialPassportSettings : DeluxeConfigurationSection
	{
		private SocialPassportSettings()
		{
		}

		/// <summary>
		/// 得到社交连接的配置信息
		/// </summary>
		/// <returns></returns>
		public static SocialPassportSettings GetConfig()
		{
			SocialPassportSettings settings = (SocialPassportSettings)ConfigurationBroker.GetSection("socialPassportSettings");

			if (settings == null)
				settings = new SocialPassportSettings();

			return settings;
		}

		[ConfigurationProperty("connections", IsRequired = false)]
		public SocialConnectionConfigurationElementCollection Connections
		{
			get
			{
				return (SocialConnectionConfigurationElementCollection)this["connections"];
			}
		}
	}
}
