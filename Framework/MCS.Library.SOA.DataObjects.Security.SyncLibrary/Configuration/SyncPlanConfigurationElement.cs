using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using System.Configuration;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary.Configuration
{
    public class SyncPlanConfigurationElement : NamedConfigurationElement
    {
        /// <summary>
        /// 批次大小，最小值是1
        /// </summary>
        [ConfigurationProperty("batchSize", IsRequired = false, DefaultValue = 1)]
        [IntegerValidator(MinValue = 1, MaxValue = 4096)]
        public int BatchSize
        {
            get { return (int)this["batchSize"]; }
        }

        /// <summary>
        /// 数据提供程序的配置
        /// </summary>
        [ConfigurationProperty("dataProvider", IsRequired = false, Options = ConfigurationPropertyOptions.IsTypeStringTransformationRequired)]
        public DataProviderConfigurationElement DataProvider
        {
            get
            {
                return (DataProviderConfigurationElement)this["dataProvider"];
            }
        }

        /// <summary>
        /// 属性映射
        /// </summary>
        [ConfigurationCollection(typeof(PropertyMappingConfigurationElementCollection))]
        [ConfigurationProperty("propertyMappings", IsRequired = true)]
        public PropertyMappingConfigurationElementCollection PropertyMappings
        {
            get
            {
                return (PropertyMappingConfigurationElementCollection)this["propertyMappings"];
            }
        }

        /// <summary>
        /// 日志记录器
        /// </summary>
        [ConfigurationCollection(typeof(LoggerConfigurationElementCollection))]
        [ConfigurationProperty("loggers", IsRequired = true)]
        public LoggerConfigurationElementCollection Loggers
        {
            get
            {
                return (LoggerConfigurationElementCollection)this["loggers"];
            }
        }

        [ConfigurationProperty("sourceKeyProperty", IsRequired = false, DefaultValue = "ID")]
        public string SourceKeyProperty
        {
            get
            {
                return (string)this["sourceKeyProperty"];
            }
        }

        protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
        {
            return false;
        }
    }

    public class SyncPlanConfigurationElementCollection : NamedConfigurationElementCollection<SyncPlanConfigurationElement>
    {

    }
}
