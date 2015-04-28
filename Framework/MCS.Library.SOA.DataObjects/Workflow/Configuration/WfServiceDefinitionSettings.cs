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
    /// 定义好的服务定义配置信息
    /// </summary>
    public class WfServiceDefinitionSettings : DeluxeConfigurationSection
    {
        /// <summary>
        /// 得到定义好的服务定义配置信息的配置节
        /// </summary>
        /// <returns></returns>
        public static WfServiceDefinitionSettings GetSection()
        {
            WfServiceDefinitionSettings result = (WfServiceDefinitionSettings)ConfigurationBroker.GetSection("wfServiceDefinitionSettings");

            if (result == null)
                result = new WfServiceDefinitionSettings();

            return result;
        }

        [ConfigurationProperty("addresses", IsRequired = false)]
        public WfServiceAddressDefinitionConfigurationElementCollection Addresses
        {
            get
            {
                return (WfServiceAddressDefinitionConfigurationElementCollection)this["addresses"];
            }
        }

        /// <summary>
        /// 服务操作定义
        /// </summary>
        [ConfigurationProperty("operations", IsRequired = false)]
        public WfServiceOperationDefinitionConfigurationElementCollection Operations
        {
            get
            {
                return (WfServiceOperationDefinitionConfigurationElementCollection)this["operations"];
            }
        }
    }
}
