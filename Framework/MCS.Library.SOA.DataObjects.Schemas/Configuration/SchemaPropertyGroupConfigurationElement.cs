using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects.Schemas.Configuration
{
	public class SchemaPropertyGroupConfigurationElement : NamedConfigurationElement
	{
		[ConfigurationProperty("defaultTab")]
		public string DefaultTab
		{
			get
			{
				return (string)this["defaultTab"];
			}
		}

		[ConfigurationProperty("properties", IsRequired = false)]
		public SchemaPropertyDefineConfigurationElementCollection AllProperties
		{
			get
			{
				return (SchemaPropertyDefineConfigurationElementCollection)this["properties"];
			}
		}
	}

	public class SchemaPropertyGroupConfigurationElementCollection : NamedConfigurationElementCollection<SchemaPropertyGroupConfigurationElement>
	{
	}
}
