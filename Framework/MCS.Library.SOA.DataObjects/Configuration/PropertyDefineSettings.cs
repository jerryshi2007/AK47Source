using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects
{
    public sealed class PropertyGroupSettings : ConfigurationSection
    {
        public static PropertyGroupSettings GetConfig()
        {
            PropertyGroupSettings settings = (PropertyGroupSettings)ConfigurationBroker.GetSection("propertyGroupSettings");

            if (settings == null)
                settings = new PropertyGroupSettings();

            return settings;
        }

        private PropertyGroupSettings()
        {
        }

        [ConfigurationProperty("groups", IsRequired = false)]
        public PropertyGroupConfigurationElementCollection Groups
        {
            get
            {
                return (PropertyGroupConfigurationElementCollection)this["groups"];
            }
        }
    }

    public class PropertyGroupConfigurationElement : NamedConfigurationElement
    {
        [ConfigurationProperty("properties", IsRequired = false)]
        public PropertyDefineConfigurationElementCollection AllProperties
        {
            get
            {
                return (PropertyDefineConfigurationElementCollection)this["properties"];
            }
        }
    }

    public class PropertyGroupConfigurationElementCollection : NamedConfigurationElementCollection<PropertyGroupConfigurationElement>
    {
    }

    public class PropertyDefineConfigurationElement : NamedConfigurationElement
    {
        [ConfigurationProperty("displayName", DefaultValue = "")]
        public string DisplayName
        {
            get
            {
                return (string)this["displayName"];
            }
        }

        [ConfigurationProperty("category", DefaultValue = "")]
        public string Category
        {
            get
            {
                return (string)this["category"];
            }
        }

        [ConfigurationProperty("type", DefaultValue = PropertyDataType.String)]
        public PropertyDataType DataType
        {
            get
            {
                return (PropertyDataType)this["type"];
            }
        }

        [ConfigurationProperty("defaultValue", DefaultValue = "")]
        public string DefaultValue
        {
            get
            {
                return (string)this["defaultValue"];
            }
        }

        [ConfigurationProperty("readOnly", DefaultValue = false, IsRequired = false)]
        public bool ReadOnly
        {
            get
            {
                return (bool)this["readOnly"];
            }
        }

        [ConfigurationProperty("visible", DefaultValue = true, IsRequired = false)]
        public bool Visible
        {
            get
            {
                return (bool)this["visible"];
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

        [ConfigurationProperty("editorKey", DefaultValue = "")]
        public string EditorKey
        {
            get
            {
                return (string)this["editorKey"];
            }
        }

        [ConfigurationProperty("persisterKey", DefaultValue = "")]
        public string PersisterKey
        {
            get
            {
                return (string)this["persisterKey"];
            }
        }

        [ConfigurationProperty("editorParamsSettingsKey", DefaultValue = "")]
        public string EditorParamsSettingsKey
        {
            get
            {
                return (string)this["editorParamsSettingsKey"];
            }
        }

        [ConfigurationProperty("editorParams", DefaultValue = "")]
        public string EditorParams
        {
            get
            {
                return (string)this["editorParams"];
            }
        }

        [ConfigurationProperty("sortOrder", DefaultValue = 0xFFFF)]
        public int SortOrder
        {
            get
            {
                return (int)this["sortOrder"];
            }
        }

        [ConfigurationProperty("maxLength", DefaultValue = 0xFFFF)]
        public int MaxLength
        {
            get
            {
                return (int)this["maxLength"];
            }
        }

        [ConfigurationProperty("isRequired", DefaultValue = false, IsRequired = false)]
        public bool IsRequired
        {
            get
            {
                return (bool)this["isRequired"];
            }
        }

        [ConfigurationProperty("showTitle", DefaultValue = true, IsRequired = false)]
        public bool ShowTitle
        {
            get
            {
                return (bool)this["showTitle"];
            }
        }

        [ConfigurationProperty("validators", IsRequired = false)]
        public PropertyDefineValidatorConfigurationElementCollection Validators
        {
            get
            {
                return (PropertyDefineValidatorConfigurationElementCollection)this["validators"];
            }
        }

        /// <summary>
        /// 重载之后，避免报错
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            return true;
        }
    }

    public sealed class PropertyDefineConfigurationElementCollection : NamedConfigurationElementCollection<PropertyDefineConfigurationElement>
    {
    }

    public sealed class PropertyDefineValidatorConfigurationElement : TypeConfigurationElement
    {
        [ConfigurationProperty("messageTemplate", IsRequired = false, DefaultValue = "")]
        public string MessageTemplate
        {
            get
            {
                return (string)this["messageTemplate"];
            }
        }

        [ConfigurationProperty("tag", IsRequired = false, DefaultValue = "")]
        public string Tag
        {
            get
            {
                return (string)this["tag"];
            }
        }

        [ConfigurationProperty("parameters", IsRequired = false)]
        public PropertyDefineValidatorParameterConfigurationElementCollection Parameters
        {
            get
            {
                return (PropertyDefineValidatorParameterConfigurationElementCollection)this["parameters"];
            }
        }

        /// <summary>
        /// 重载之后，避免报错
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            return true;
        }
    }

    public sealed class PropertyDefineValidatorConfigurationElementCollection : NamedConfigurationElementCollection<PropertyDefineValidatorConfigurationElement>
    {
    }

    public sealed class PropertyDefineValidatorParameterConfigurationElement : NamedConfigurationElement
    {
        [ConfigurationProperty("type", DefaultValue = PropertyDataType.String)]
        public PropertyDataType DataType
        {
            get
            {
                return (PropertyDataType)this["type"];
            }
        }

        [ConfigurationProperty("paramValue", DefaultValue = "")]
        public string ParamValue
        {
            get
            {
                return (string)this["paramValue"];
            }
        }
    }

    public sealed class PropertyDefineValidatorParameterConfigurationElementCollection : NamedConfigurationElementCollection<PropertyDefineValidatorParameterConfigurationElement>
    {
    }
}
