using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;

namespace MCS.Library.Logging
{
	/// <summary>
	/// 
	/// </summary>
	public class LoggerFilterConfigurationElement : LoggerItemConfigurationElementBase
	{
		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("minPriority", IsRequired = false, DefaultValue = LogPriority.BelowNormal)]
		public LogPriority MinPriority
		{
			get
			{
				return (LogPriority)this["minPriority"];
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class LoggerFilterConfigurationElementCollection : NamedConfigurationElementCollection<LoggerFilterConfigurationElement>
	{
	}
}
