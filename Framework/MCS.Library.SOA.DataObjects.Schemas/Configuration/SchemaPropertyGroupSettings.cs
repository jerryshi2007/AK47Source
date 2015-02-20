using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;

namespace MCS.Library.SOA.DataObjects.Schemas.Configuration
{
	public sealed class SchemaPropertyGroupSettings : ConfigurationSection
	{
		public static SchemaPropertyGroupSettings GetConfig()
		{
			SchemaPropertyGroupSettings settings = (SchemaPropertyGroupSettings)ConfigurationBroker.GetSection("schemaPropertyGroupSettings");

			if (settings == null)
				settings = new SchemaPropertyGroupSettings();

			return settings;
		}

		private SchemaPropertyGroupSettings()
		{
		}

		[ConfigurationProperty("groups", IsRequired = false)]
		public SchemaPropertyGroupConfigurationElementCollection Groups
		{
			get
			{
				return (SchemaPropertyGroupConfigurationElementCollection)this["groups"];
			}
		}
	}
}
