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
	public class LoggerFormatterConfigurationElement : LoggerItemConfigurationElementBase
	{
		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("template", IsRequired = false)]
		public string Template
		{
			get
			{
				return (string)this["template"];
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class LoggerFormatterConfigurationElementCollection : NamedConfigurationElementCollection<LoggerFormatterConfigurationElement>
	{
	}
}
