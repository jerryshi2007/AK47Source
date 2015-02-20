using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;

namespace MCS.Library.Logging
{
	/// <summary>
	/// 日志部分的配置信息
	/// </summary>
	public class LoggingSection : DeluxeConfigurationSection
	{
		private LoggingSection()
		{
		}

		/// <summary>
		/// 得到配置节
		/// </summary>
		/// <returns></returns>
		public static LoggingSection GetConfig()
		{
			LoggingSection settings = (LoggingSection)ConfigurationBroker.GetSection("LoggingSettings");

			if (settings == null)
				settings = new LoggingSection();

			return settings;
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("Loggers")]
		public LoggerConfigurationElementCollection Loggers
		{
			get
			{
				return (LoggerConfigurationElementCollection)this["Loggers"];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("Formatters")]
		public LoggerFormatterConfigurationElementCollection Formatters
		{
			get
			{
				return (LoggerFormatterConfigurationElementCollection)this["Formatters"];
			}
		}
	}
}
