using MCS.Library.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 服务操作的配置信息
    /// </summary>
    public class WfServiceOperationDefinitionConfigurationElement : NamedConfigurationElement
    {
        /// <summary>
        /// 地址的Key
        /// </summary>
        [ConfigurationProperty("addressKey")]
        public string AddressKey
        {
            get
            {
                return (string)this["addressKey"];
            }
        }

        /// <summary>
        /// 操作（方法）的名称
        /// </summary>
        [ConfigurationProperty("operationName")]
        public string OperationName
        {
            get
            {
                return (string)this["operationName"];
            }
        }

        /// <summary>
        /// 返回值的参数名称
        /// </summary>
        [ConfigurationProperty("returnParamName")]
        public string ReturnParamName
        {
            get
            {
                return (string)this["returnParamName"];
            }
        }

        /// <summary>
        /// 服务调用的超时时间
        /// </summary>
        [ConfigurationProperty("timeout", IsRequired = false, DefaultValue = "00:00:30")]
        public TimeSpan Timeout
        {
            get
            {
                return (TimeSpan)this["timeout"];
            }
        }

        /// <summary>
        /// 是否在流程持久化时调用。
        /// </summary>
        [ConfigurationProperty("invokeWhenPersist", IsRequired = false, DefaultValue = true)]
        public bool InvokeWhenPersist
        {
            get
            {
                return (bool)this["invokeWhenPersist"];
            }
        }

        /// <summary>
        /// 参数的定义
        /// </summary>
        [ConfigurationProperty("parameters", IsRequired = false)]
        public WfServiceOperationParameterConfigurationElementCollection Parameters
        {
            get
            {
                return (WfServiceOperationParameterConfigurationElementCollection)this["parameters"];
            }
        }
    }

    /// <summary>
    /// 服务操作的配置集合
    /// </summary>
    public class WfServiceOperationDefinitionConfigurationElementCollection : NamedConfigurationElementCollection<WfServiceOperationDefinitionConfigurationElement>
    {
    }
}
