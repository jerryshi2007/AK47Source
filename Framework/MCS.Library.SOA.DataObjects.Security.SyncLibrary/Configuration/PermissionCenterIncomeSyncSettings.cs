using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary.Configuration
{
    public class PermissionCenterIncomeSyncSettings : ConfigurationSection
    {
        public static PermissionCenterIncomeSyncSettings GetConfig()
        {
            var config = (PermissionCenterIncomeSyncSettings)ConfigurationBroker.GetSection("pcIncomeSyncSettings");
            if (config == null)
                throw new System.Configuration.ConfigurationErrorsException("没有找到配置节pcIncomeSyncSettings");

            return config;
        }

        /// <summary>
        /// 同步方案
        /// </summary>
        [ConfigurationCollection(typeof(SyncPlanConfigurationElementCollection))]
        [ConfigurationProperty("syncPlans", IsRequired = true)]
        public SyncPlanConfigurationElementCollection SyncPlans
        {
            get
            {
                return (SyncPlanConfigurationElementCollection)this["syncPlans"];
            }
        }

        /// <summary>
        /// 比较器
        /// </summary>
        [ConfigurationCollection(typeof(ComparerConfigurationElementCollection))]
        [ConfigurationProperty("comparers", IsRequired = true)]
        public ComparerConfigurationElementCollection Comparers
        {
            get
            {
                return (ComparerConfigurationElementCollection)this["comparers"];
            }
        }

        /// <summary>
        /// 属性设置器
        /// </summary>
        [ConfigurationCollection(typeof(ComparerConfigurationElementCollection))]
        [ConfigurationProperty("propertySetters", IsRequired = true)]
        public ComparerConfigurationElementCollection PropertySetters
        {
            get
            {
                return (ComparerConfigurationElementCollection)this["propertySetters"];
            }
        }
    }
}
