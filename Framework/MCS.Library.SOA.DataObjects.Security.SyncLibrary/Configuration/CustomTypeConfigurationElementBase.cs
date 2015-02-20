using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using System.Configuration;
using System.Runtime;
using System.Collections.Specialized;
using System.Collections;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary.Configuration
{
    public abstract class CustomTypeConfigurationElementBase : CustomConfigurationElementBase
    {
        private static readonly ConfigurationProperty _propName = new ConfigurationProperty("name", typeof(string), null, null, NotEmptyStringValidator, ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty _propType = new ConfigurationProperty("type", typeof(string), "", ConfigurationPropertyOptions.IsTypeStringTransformationRequired | ConfigurationPropertyOptions.IsRequired);

        internal CustomTypeConfigurationElementBase()
            : base()
        {
        }

        public CustomTypeConfigurationElementBase(string name, string type)
            : this()
        {
            this.Name = name;
            this.Type = type;
        }

        protected override void InitializeBuildInProperties(ConfigurationPropertyCollection properties)
        {
            properties.Add(_propName);
            properties.Add(_propType);
        }

        protected override bool IsBuiltInProperty(string propertyName)
        {
            return propertyName == "name" || propertyName == "type";
        }

        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)base[_propName];
            }
            set
            {
                base[_propName] = value;
            }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get
            {
                return (string)base[_propType];
            }
            set
            {
                base[_propType] = value;
            }
        }
    }

    public class CustomTypeConfigurationElementCollection<T> : ConfigurationElementCollection where T : CustomTypeConfigurationElementBase, new()
    {
        /// <summary>
        /// 按照序号获取指定的配置元素
        /// </summary>
        /// <param name="index">序号</param>
        /// <returns>配置元素</returns>
        public virtual T this[int index] { get { return (T)base.BaseGet(index); } }

        /// <summary>
        /// 按照名称获取指定的配置元素
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>配置元素</returns>
        public new T this[string name] { get { return (T)base.BaseGet(name); } }

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
        protected override object GetElementKey(ConfigurationElement element) { return ((T)element).Name; }

        /// <summary>
        /// 生成新的配置元素实例
        /// </summary>
        /// <returns>配置元素实例</returns>
        protected override ConfigurationElement CreateNewElement() { return new T(); }

        /// <summary>
        /// 通过name在字典内查找数据，如果name不存在，不抛出异常
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>配置元素</returns>
        protected virtual T InnerGet(string name)
        {
            object element = base.BaseGet(name);

            return (T)element;
        }
    }
}
