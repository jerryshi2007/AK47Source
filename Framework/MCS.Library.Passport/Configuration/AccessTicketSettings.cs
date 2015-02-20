using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.Passport
{
	/// <summary>
	/// 票据检查的配置节
	/// </summary>
	public sealed class AccessTicketSettings : DeluxeConfigurationSection
	{
		/// <summary>
		/// 获取配置信息
		/// </summary>
		/// <returns></returns>
		public static AccessTicketSettings GetConfig()
		{
			AccessTicketSettings settings = (AccessTicketSettings)ConfigurationBroker.GetSection("accessTicketSettings");

			if (settings == null)
				settings = new AccessTicketSettings();

			return settings;
		}

		private AccessTicketSettings()
		{
		}

		/// <summary>
		/// 默认是否启用票据检查
		/// </summary>
		[ConfigurationProperty("enabled", DefaultValue = true)]
		public bool Enabled
		{
			get
			{
				return (bool)this["enabled"];
			}
		}

		/// <summary>
		/// 默认是否检察Url
		/// </summary>
		[ConfigurationProperty("checkUrl", DefaultValue = true)]
		public bool CheckUrl
		{
			get
			{
				return (bool)this["checkUrl"];
			}
		}

		/// <summary>
		/// 默认检查票据Url的部分
		/// </summary>
		[ConfigurationProperty("urlCheckParts", DefaultValue = AccessTicketUrlCheckParts.All)]
		public AccessTicketUrlCheckParts UrlCheckParts
		{
			get
			{
				return (AccessTicketUrlCheckParts)this["urlCheckParts"];
			}
		}

		/// <summary>
		/// 默认检查票据的超时时间
		/// </summary>
		[ConfigurationProperty("ticketTimeout", DefaultValue = "00:00:30")]
		public TimeSpan TicketTimeout
		{
			get
			{
				return (TimeSpan)this["ticketTimeout"];
			}
		}
	}
}
