using MCS.Library.Configuration;
using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// 根据一组Operation Key的定义产生一组ServiceOperation类
        /// </summary>
        /// <param name="invokeWhenPersist">是否是持久化时调用</param>
        /// <param name="opKeys"></param>
        /// <returns></returns>
        public WfServiceOperationDefinitionCollection GetOperations(bool invokeWhenPersist, params string[] opKeys)
        {
            WfServiceOperationDefinitionCollection result = new WfServiceOperationDefinitionCollection();

            if (opKeys != null)
            {
                foreach (string opKey in opKeys)
                {
                    if (opKey.IsNotEmpty())
                    {
                        string key = opKey.Trim();

                        if (key.IsNotEmpty())
                        {
                            WfServiceOperationDefinitionConfigurationElement opElement = this.Operations[key];

                            if (opElement != null)
                            {
                                if (opElement.InvokeWhenPersist == invokeWhenPersist)
                                    result.Add(new WfServiceOperationDefinition(opElement));
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
