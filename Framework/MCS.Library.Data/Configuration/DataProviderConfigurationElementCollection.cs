using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

using MCS.Library.Configuration;

namespace MCS.Library.Data.Configuration
{
	//class DataProviderConfigurationSection : ConfigurationSection
	//{
	//    [ConfigurationProperty("dataProviders", IsRequired = true)]
	//    public DataProviderConfigurationElementCollection DataProviders
	//    {
	//        get
	//        {
	//            return (DataProviderConfigurationElementCollection)base["dataProviders"];
	//        }
	//    }
	//}
	[ConfigurationCollection(typeof(DataProviderConfigurationElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	sealed class DataProviderConfigurationElementCollection : NamedConfigurationElementCollection<DataProviderConfigurationElement>
	{

	}

	sealed class DataProviderConfigurationElement : TypeConfigurationElement
	{

	}
}
