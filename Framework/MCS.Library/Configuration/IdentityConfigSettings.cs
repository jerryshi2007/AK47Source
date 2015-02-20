using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;

namespace MCS.Library.Configuration
{
	/// <summary>
	/// 登录身份的配置信息
	/// </summary>
	public class IdentityConfigSettings : ConfigurationSection
	{
		private IdentityConfigSettings()
		{
		}

		/// <summary>
		/// 从配置文件中得到服务器配置信息
		/// </summary>
		/// <returns></returns>
		public static IdentityConfigSettings GetConfig()
		{
			IdentityConfigSettings result = (IdentityConfigSettings)ConfigurationBroker.GetSection("identityConfigSettings");

			if (result == null)
				result = new IdentityConfigSettings();

			return result;
		}

		/// <summary>
		/// 服务器配置信息集合
		/// </summary>
		[ConfigurationProperty("identities")]
		public IdentityConfigurationElementCollection Identities
		{
			get
			{
				return (IdentityConfigurationElementCollection)this["identities"];
			}
		}
	}
}
