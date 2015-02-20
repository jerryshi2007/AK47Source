using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Core;

namespace MCS.Library.Configuration
{
	/// <summary>
	/// 模拟时间点的设置
	/// </summary>
	public sealed class TimePointSimulationSettings : ConfigurationSection
	{
		private TimePointSimulationSettings()
		{
		}

		/// <summary>
		/// 得到模拟时间点的设置信息
		/// </summary>
		/// <returns></returns>
		public static TimePointSimulationSettings GetConfig()
		{
			TimePointSimulationSettings result = (TimePointSimulationSettings)ConfigurationBroker.GetSection("timePointSimulationSettings");

			if (result == null)
				result = new TimePointSimulationSettings();

			return result;
		}

		/// <summary>
		/// 是否启用
		/// </summary>
		[ConfigurationProperty("enabled", DefaultValue = true, IsRequired = false)]
		public bool Enabled
		{
			get
			{
				return (bool)this["enabled"];
			}
		}

		/// <summary>
		/// 如果保存为Cookie，Cookie的Key的名称
		/// </summary>
		[ConfigurationProperty("cookieKey", DefaultValue = "HTimePointContext", IsRequired = false)]
		public string CookieKey
		{
			get
			{
				return (string)this["cookieKey"];
			}
		}

		/// <summary>
		/// 默认的模拟时间
		/// </summary>
		public DateTime DefaultSimulatedTime
		{
			get
			{
				DateTime result = DateTime.MinValue;

				DateTime.TryParse(DefaultSimulatedTimeString, out result);

				return result;
			}
		}

		/// <summary>
		/// 持久化器
		/// </summary>
		public IPersistTimePoint Persister
		{
			get
			{
				IPersistTimePoint result = null;

				if (TypeFactories.ContainsKey("persister"))
					result = (IPersistTimePoint)TypeFactories["persister"].CreateInstance();

				return result;
			}
		}

		[ConfigurationProperty("typeFactories", IsRequired = false)]
		private TypeConfigurationCollection TypeFactories
		{
			get
			{
				return (TypeConfigurationCollection)this["typeFactories"];
			}
		}

		[ConfigurationProperty("defaultSimulatedTimeString")]
		private string DefaultSimulatedTimeString
		{
			get
			{
				return (string)this["defaultSimulatedTimeString"];
			}
		}
	}
}
