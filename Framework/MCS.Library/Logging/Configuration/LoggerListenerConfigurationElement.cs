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
	public class LoggerListenerConfigurationElement : LoggerItemConfigurationElementBase
	{
		/// <summary>
		/// LogFormatter的名称
		/// </summary>
		[ConfigurationProperty("formatter", IsRequired = false)]
		public string LogFormatterName
		{
			get
			{
				return (string)this["formatter"];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("logName", IsRequired = false)]
		public string LogName
		{
			get
			{
				return (string)this["formatter"];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("source", IsRequired = false)]
		public string Source
		{
			get
			{
				return (string)this["source"];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("fileName", IsRequired = false)]
		public string FileName
		{
			get
			{
				return (string)this["fileName"];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("header", IsRequired = false)]
		public string Header
		{
			get
			{
				return (string)this["header"];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("footer", IsRequired = false)]
		public string Footer
		{
			get
			{
				return (string)this["footer"];
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class LoggerListenerConfigurationElementCollection : NamedConfigurationElementCollection<LoggerListenerConfigurationElement>
	{
	}
}
