using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Core;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 应用日志相关的设置
	/// </summary>
	public class AppLogSettings : ConfigurationSection
	{
		/// <summary>
		/// 获得和AppLog相关的设置
		/// </summary>
		/// <returns></returns>
		public static AppLogSettings GetConfig()
		{
			AppLogSettings settings = (AppLogSettings)ConfigurationBroker.GetSection("appLogSettings");

			if (settings == null)
				settings = new AppLogSettings();

			return settings;
		}

		[ConfigurationProperty("connectionName", DefaultValue = "HB2008", IsRequired = false)]
		public string ConnectionName
		{
			get
			{
				return (string)this["connectionName"];
			}
		}
	}
}
