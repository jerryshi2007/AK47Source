using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects.Security.Configuration
{
	public class LogCategoryConfigSection : ConfigurationSection
	{
		public static LogCategoryConfigSection GetConfig()
		{
			LogCategoryConfigSection settings = (LogCategoryConfigSection)ConfigurationBroker.GetSection("schemaLogCategorySettings");

			return settings;
		}

		[ConfigurationProperty("categories", IsRequired = false)]
		public LogCategoryConfigurationElementCollection Categories
		{
			get
			{
				return (LogCategoryConfigurationElementCollection)this["categories"];
			}
		}
	}
}
