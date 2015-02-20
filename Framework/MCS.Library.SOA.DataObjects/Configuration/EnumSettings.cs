using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects
{
	public sealed class EnumSettings : ConfigurationSection
	{
		public static EnumSettings GetConfig()
		{
			EnumSettings settings = (EnumSettings)ConfigurationBroker.GetSection("enumSettings");

			if (settings == null)
				settings = new EnumSettings();
			
			return settings;
		}

		private EnumSettings()
		{
		}

		[ConfigurationProperty("groups", IsRequired = false)]
		public EnumGroupConfigurationElementCollection Groups
		{
			get
			{
				return (EnumGroupConfigurationElementCollection)this["groups"];
			}
		}
	}

	public class EnumGroupConfigurationElement : NamedConfigurationElement
	{
		[ConfigurationProperty("items", IsRequired = false)]
		public EnumConfigurationElementCollection Items
		{
			get
			{
				return (EnumConfigurationElementCollection)this["items"];
			}
		}
	}

	public class EnumGroupConfigurationElementCollection : NamedConfigurationElementCollection<EnumGroupConfigurationElement>
	{
	}

	public class EnumConfigurationElementCollection : NamedConfigurationElementCollection<EnumConfigurationElement>
	{
	}

	public class EnumConfigurationElement : NamedConfigurationElement
	{
	}
}
