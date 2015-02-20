using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects.Security.Configuration
{
	public class LogCategoryConfigurationElement : NamedConfigurationElement
	{
		[ConfigurationProperty("title")]
		public string Title
		{
			get
			{
				return (string)this["title"];
			}

			set
			{
				this["title"] = value;
			}
		}
	}

	public class LogCategoryConfigurationElementCollection : NamedConfigurationElementCollection<LogCategoryConfigurationElement>
	{
	}
}
