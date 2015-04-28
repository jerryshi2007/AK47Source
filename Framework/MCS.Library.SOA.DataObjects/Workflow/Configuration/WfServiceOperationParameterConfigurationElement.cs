using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Configuration;
using System.Configuration;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 操作的参数定义
    /// </summary>
    public class WfServiceOperationParameterConfigurationElement : NamedConfigurationElement
    {
        /// <summary>
        /// 参数的数据类型
        /// </summary>
        [ConfigurationProperty("type", DefaultValue = WfSvcOperationParameterType.String, IsRequired = false)]
        public WfSvcOperationParameterType Type
        {
            get
            {
                return (WfSvcOperationParameterType)this["type"];
            }
        }

        /// <summary>
        /// 参数的默认值。当参数类型是RuntimeParameter时，参数值是流程上下文参数名称
        /// </summary>
        [ConfigurationProperty("value", DefaultValue = "", IsRequired = false)]
        public string Value
        {
            get
            {
                return (string)this["value"];
            }
        }
    }

    /// <summary>
    /// 操作的参数定义集合
    /// </summary>
    public class WfServiceOperationParameterConfigurationElementCollection : NamedConfigurationElementCollection<WfServiceOperationParameterConfigurationElement>
    {
    }
}
