using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace MCS.Library.Configuration
{
    /// <summary>
    /// 租户上下文配置信息
    /// </summary>
    public class TenantContextSettings : ConfigurationSection
    {
        private static readonly TenantContextSettings DefaultSettings = new TenantContextSettings();

        /// <summary>
        /// 从配置信息中获取租户上下文相关的配置信息
        /// </summary>
        /// <returns></returns>
        public static TenantContextSettings GetConfig()
        {
            TenantContextSettings settings = (TenantContextSettings)ConfigurationBroker.GetSection("tenantContextSettings");

            if (settings == null)
                settings = TenantContextSettings.DefaultSettings;

            return settings;
        }

        private TenantContextSettings()
        {
        }

        /// <summary>
        /// 是否启用多租户模式
        /// </summary>
        [ConfigurationProperty("enabled", IsRequired = false, DefaultValue = false)]
        public bool Enabled
        {
            get
            {
                return (bool)this["enabled"];
            }
        }

        /// <summary>
        /// 默认的租户代码
        /// </summary>
        [ConfigurationProperty("defaultTenantCode", IsRequired = false, DefaultValue = "D5561180-7617-4B67-B68B-1F0EA604B509")]
        public string DefaultTenantCode
        {
            get
            {
                return (string)this["defaultTenantCode"];
            }
        }
    }
}
