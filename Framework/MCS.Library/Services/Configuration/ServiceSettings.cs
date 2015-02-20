using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MCS.Library.Configuration;
using MCS.Library.Services.Configuration;

namespace MCS.Library.Services.Configuration
{
	/// <summary>
	/// 服务方法的配置信息
	/// </summary>
	public sealed class ServiceSettings : ConfigurationSection
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static ServiceSettings GetConfig()
		{
			ServiceSettings result = (ServiceSettings)ConfigurationBroker.GetSection("serviceSettings");

			if (result == null)
				result = new ServiceSettings();

			return result;
		}

		private ServiceSettings()
		{
		}

		/// <summary>
		/// 所有方法的默认设置
		/// </summary>
		[ConfigurationProperty("methodDefaultSettings", IsRequired = false)]
		public ServiceMethodConfigurationElement MethodDefaultSettings
		{
			get
			{
				return (ServiceMethodConfigurationElement)this["methodDefaultSettings"];
			}
		}

		/// <summary>
		/// 服务的配置集合
		/// </summary>
		[ConfigurationProperty("services", IsRequired = false)]
		public ServiceConfigurationElementCollection Services
		{
			get
			{
				return (ServiceConfigurationElementCollection)this["services"];
			}
		}
	}
}