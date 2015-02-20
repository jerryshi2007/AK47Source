using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects
{
    public sealed class PropertyEditorParamsSettings : ConfigurationSection
    {
        public static PropertyEditorParamsSettings GetConfig()
        {
            PropertyEditorParamsSettings settings = (PropertyEditorParamsSettings)ConfigurationBroker.GetSection("propertyEditorParamsSettings");

            if (settings == null)
                settings = new PropertyEditorParamsSettings();

            return settings;
        }

        private PropertyEditorParamsSettings()
        {
        }

        [ConfigurationProperty("editorParams", IsRequired = false)]
        public PropertyEditorParamsConfigurationElementCollection EditorParams
        {
            get
            {
                return (PropertyEditorParamsConfigurationElementCollection)this["editorParams"];
            }
        }
    }

    public class PropertyEditorParamsConfigurationElement : NamedConfigurationElement
    {
        [ConfigurationProperty("editorParamsValue", IsRequired = true)]
        public  EditorParamsValueElement EditorParamsValue
        {
            get
            {
                return (EditorParamsValueElement)this["editorParamsValue"];
            }
        }
    }

    public class PropertyEditorParamsConfigurationElementCollection : NamedConfigurationElementCollection<PropertyEditorParamsConfigurationElement>
    {
    }

    public class EditorParamsValueElement : ConfigurationElement
    {
        protected override void DeserializeElement(System.Xml.XmlReader reader, bool serializeCollectionKey)
        {
            ValueText = reader.ReadElementContentAs(typeof(string), null) as string;
        }
        protected override bool SerializeElement(System.Xml.XmlWriter writer, bool serializeCollectionKey)
        {
            if (writer != null)
                writer.WriteCData(ValueText);

            return true;
        }

        [ConfigurationProperty("data", IsRequired = false)]
        public string ValueText
        {
            get { return this["data"].ToString(); }
            set { this["data"] = value; }
        }
    }
}
