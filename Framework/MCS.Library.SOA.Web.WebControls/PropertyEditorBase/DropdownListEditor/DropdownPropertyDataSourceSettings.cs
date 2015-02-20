using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Web.WebControls
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

	public sealed class DropdownPropertyDataSourceConfigurationElement : TypeConfigurationElement
	{
		[ConfigurationProperty("bindingValue", IsRequired = true)]
		public string BindingValue
		{
			get
			{
				return (string)this["bindingValue"];
			}
		}

		[ConfigurationProperty("bindingText", IsRequired = true)]
		public string BindingText
		{
			get
			{
				return (string)this["bindingText"];
			}
		}

		[ConfigurationProperty("method", IsRequired = true)]
		public string Method
		{
			get
			{
				return (string)this["method"];
			}
		}
	}

	public sealed class DropdownPropertyDataSourceConfigurationCollection : NamedConfigurationElementCollection<DropdownPropertyDataSourceConfigurationElement>
	{ 
	}
}
