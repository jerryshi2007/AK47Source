#region Using
using System;
using System.Configuration;
using MCS.Library.Configuration;
#endregion
namespace MCS.Library.Data.Configuration
{
    class BuilderConfigurationElement : TypeConfigurationElement 
    {
        /// <summary>
        /// Builder适用于ConnectionString的Attribute名称
        /// </summary>
        [ConfigurationProperty("attributeName")]
        public string AttributeName
        {
            get 
            {
                return (string)this["attributeName"]; 
            }
        }
    }

    [ConfigurationCollection(typeof(BuilderConfigurationElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    class BuildersConfiguratonElementCollection : NamedConfigurationElementCollection<BuilderConfigurationElement> 
    {
    }
}
