using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Web.Responsive.WebControls.Configuration
{
	public sealed class DropdownPropertyDataSourceSettings : ConfigurationSection
	{
		public static DropdownPropertyDataSourceSettings GetConfig()
		{
			DropdownPropertyDataSourceSettings settings = (DropdownPropertyDataSourceSettings)ConfigurationBroker.GetSection("dropdownPropertyDataSourceSettings");

			if (settings == null)
				settings = new DropdownPropertyDataSourceSettings();

			return settings;
		}

		private DropdownPropertyDataSourceSettings()
		{
		}

		[ConfigurationProperty("propertySources", IsRequired = false)]
		public DropdownPropertyDataSourceConfigurationCollection PropertySources
		{
			get
			{
				return (DropdownPropertyDataSourceConfigurationCollection)this["propertySources"];
			}
		}
	}
}
