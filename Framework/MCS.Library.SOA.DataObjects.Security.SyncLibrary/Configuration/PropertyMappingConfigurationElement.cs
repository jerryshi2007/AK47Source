using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary.Configuration
{
    public class PropertyMappingConfigurationElement : CustomConfigurationElementBase
    {
        private readonly ConfigurationProperty _propSrcProperty = new ConfigurationProperty("sourceProperty", typeof(string), null, null, NotEmptyStringValidator, ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);
        private readonly ConfigurationProperty _propTargetProperty = new ConfigurationProperty("targetProperty", typeof(string), null, null, NotEmptyStringValidator, ConfigurationPropertyOptions.IsRequired);
        private readonly ConfigurationProperty _propSetterName = new ConfigurationProperty("comparerName", typeof(string), null, null, NotEmptyStringValidator, ConfigurationPropertyOptions.IsRequired);
        private readonly ConfigurationProperty _propComparerName = new ConfigurationProperty("setterName", typeof(string), null, null, NotEmptyStringValidator, ConfigurationPropertyOptions.IsRequired);

        public PropertyMappingConfigurationElement()
            : base()
        {
        }

        protected override void InitializeBuildInProperties(ConfigurationPropertyCollection properties)
        {
            base.InitializeBuildInProperties(properties);
            properties.Add(_propSrcProperty);
            properties.Add(_propTargetProperty);
            properties.Add(_propComparerName);
            properties.Add(_propSetterName);
        }

        protected override bool IsBuiltInProperty(string propertyName)
        {
            return propertyName == "sourceProperty" || propertyName == "targetProperty" || propertyName == "comparerName" || propertyName == "setterName";
        }

        [ConfigurationProperty("sourceProperty", IsKey = true, IsRequired = true)]
        public string SourceProperty
        {
            get
            {
                return (string)this["sourceProperty"];
            }
        }

        [ConfigurationProperty("targetProperty", IsRequired = true)]
        public string TargetProperty
        {
            get
            {
                return (string)this["targetProperty"];
            }
        }

        [ConfigurationProperty("comparerName", IsRequired = true)]
        public string ComparerName
        {
            get
            {
                return (string)this["comparerName"];
            }
        }

        [ConfigurationProperty("setterName", IsRequired = true)]
        public string SetterName
        {
            get
            {
                return (string)this["setterName"];
            }
        }
    }

    public class PropertyMappingConfigurationElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// 按照序号获取指定的配置元素
        /// </summary>
        /// <param name="index">序号</param>
        /// <returns>配置元素</returns>
        public virtual PropertyMappingConfigurationElement this[int index] { get { return (PropertyMappingConfigurationElement)base.BaseGet(index); } }

        /// <summary>
        /// 按照名称获取指定的配置元素
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>配置元素</returns>
        public new PropertyMappingConfigurationElement this[string name] { get { return (PropertyMappingConfigurationElement)base.BaseGet(name); } }

        /// <summary>
        /// 是否包含指定的配置元素
        /// </summary>
        /// <param name="name">配置元素名称</param>
        /// <returns>是否包含</returns>
        public bool ContainsKey(string name) { return BaseGet(name) != null; }

        /// <summary>
        /// 得到元素的Key值
        /// </summary>
        /// <param name="element">配置元素</param>
        /// <returns>配置元素所对应的配置元素</returns>
        protected override object GetElementKey(ConfigurationElement element) { return ((PropertyMappingConfigurationElement)element).SourceProperty; }

        /// <summary>
        /// 生成新的配置元素实例
        /// </summary>
        /// <returns>配置元素实例</returns>
        protected override ConfigurationElement CreateNewElement() { return new PropertyMappingConfigurationElement(); }
    }
}
