using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects
{
    public sealed class PropertyFormLayoutSettings : ConfigurationSection
    {
        public static PropertyFormLayoutSettings GetConfig()
        {
            PropertyFormLayoutSettings settings = (PropertyFormLayoutSettings)ConfigurationBroker.GetSection("propertyFormLayoutSettings");

            if (settings == null)
                settings = new PropertyFormLayoutSettings();

            return settings;
        }

        private PropertyFormLayoutSettings()
        {
        }

        [ConfigurationProperty("layouts", IsRequired = false)]
        public PropertyFormLayoutConfigurationElementCollection Layouts
        {
            get
            {
                return (PropertyFormLayoutConfigurationElementCollection)this["layouts"];
            }
        }
    }

    public class PropertyFormLayoutConfigurationElement : NamedConfigurationElement
    {
        [ConfigurationProperty("sections", IsRequired = false)]
        public PropertyFormSectionConfigurationElementCollection AllSections
        {
            get
            {
                return (PropertyFormSectionConfigurationElementCollection)this["sections"];
            }
        }
    }

    public class PropertyFormLayoutConfigurationElementCollection : NamedConfigurationElementCollection<PropertyFormLayoutConfigurationElement>
    {
    }

    public class PropertyFormSectionConfigurationElement : NamedConfigurationElement
    {
		[ConfigurationProperty("displayName", DefaultValue = "")]
		public string DisplayName
		{
			get
			{
				return (string)this["displayName"];
			}
		}

        [ConfigurationProperty("columns", DefaultValue = 1)]
        public int Columns
        {
            get
            {
                return (int)this["columns"];
            }
        }

        [ConfigurationProperty("defaultRowHeight", DefaultValue = "")]
        public string  DefaultRowHeight
        {
            get
            {
                return (string)this["defaultRowHeight"];
            }
        }

        /// <summary>
        /// 属性合并的时候是否允许覆盖之前的属性
        /// </summary>
        [ConfigurationProperty("allowOverride", DefaultValue = true, IsRequired = false)]
        public bool AllowOverride
        {
            get
            {
                return (bool)this["allowOverride"];
            }
        }
    }

    public class PropertyFormSectionConfigurationElementCollection : NamedConfigurationElementCollection<PropertyFormSectionConfigurationElement>
    {
    }
}
