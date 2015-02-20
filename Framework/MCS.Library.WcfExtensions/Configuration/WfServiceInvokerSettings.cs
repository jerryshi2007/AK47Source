using MCS.Library.Configuration;
using MCS.Library.Data.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WcfExtensions.Configuration
{
    /// <summary>
    /// 流程以及后台作业服务调用的设置
    /// </summary>
    public class WfServiceInvokerSettings : DeluxeConfigurationSection
    {
        private WfServiceInvokerSettings()
        {
        }

        public static WfServiceInvokerSettings GetConfig()
        {
            WfServiceInvokerSettings settings = (WfServiceInvokerSettings)ConfigurationBroker.GetSection("wfServiceInvokerSettings");

            if (settings == null)
                settings = new WfServiceInvokerSettings();

            return settings;
        }

        /// <summary>
        /// 客户端传递到Web服务的连接串映射信息集合
        /// </summary>
        [ConfigurationProperty("connectionMappings", IsRequired = false)]
        public ConnectionMappingElementCollection ConnectionMappings
        {
            get
            {
                return (ConnectionMappingElementCollection)this["connectionMappings"];
            }
        }
    }
}
