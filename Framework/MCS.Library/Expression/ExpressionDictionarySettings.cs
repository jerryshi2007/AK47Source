using MCS.Library.Configuration;
using MCS.Library.Data.DataObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Expression
{
    /// <summary>
    /// 表达式中的预定义字典的配置信息
    /// </summary>
    [Serializable]
    public sealed class ExpressionDictionarySettings : ConfigurationSection
    {
        /// <summary>
        /// 获得表达式预定义字典的配置信息
        /// </summary>
        /// <returns></returns>
        public static ExpressionDictionarySettings GetConfig()
        {
            ExpressionDictionarySettings settings = (ExpressionDictionarySettings)ConfigurationBroker.GetSection("expressionDictionarySettings");

            if (settings == null)
                settings = new ExpressionDictionarySettings();

            return settings;
        }

        /// <summary>
        /// 字典项的配置信息集合
        /// </summary>
        [ConfigurationProperty("dictionaries", IsRequired = false)]
        public ExpressionDictionaryConfigurationElementCollection Dictionaries
        {
            get
            {
                return (ExpressionDictionaryConfigurationElementCollection)this["dictionaries"];
            }
        }
    }

    /// <summary>
    /// 表达式字典配置项
    /// </summary>
    public sealed class ExpressionDictionaryConfigurationElement : TypeConfigurationElement
    {
        /// <summary>
        /// 字典描述项集合
        /// </summary>
        [ConfigurationProperty("items", IsRequired = false)]
        public ExpressionDictionaryItemConfigurationElementCollection Items
        {
            get
            {
                return (ExpressionDictionaryItemConfigurationElementCollection)this["items"];
            }
        }
    }

    /// <summary>
    /// 表达式字典配置项集合
    /// </summary>
    public sealed class ExpressionDictionaryConfigurationElementCollection : NamedConfigurationElementCollection<ExpressionDictionaryConfigurationElement>
    {
    }

    /// <summary>
    /// 表达式字典项说明的配置项
    /// </summary>
    public sealed class ExpressionDictionaryItemConfigurationElement : NamedConfigurationElement
    {
        /// <summary>
        /// 数据类型
        /// </summary>
        [ConfigurationProperty("dataType", IsRequired = false, DefaultValue = "String")]
        public ExpressionDataType DataType
        {
            get
            {
                return (ExpressionDataType)this["dataType"];
            }
        }

        /// <summary>
        /// 默认值
        /// </summary>
        [ConfigurationProperty("defaultValue", IsRequired = false, DefaultValue = "")]
        public string DefaultValue
        {
            get
            {
                return (string)this["defaultValue"];
            }
        }
    }

    /// <summary>
    /// 表达式字典项说明的配置项集合
    /// </summary>
    public sealed class ExpressionDictionaryItemConfigurationElementCollection : NamedConfigurationElementCollection<ExpressionDictionaryItemConfigurationElement>
    {
    }
}
