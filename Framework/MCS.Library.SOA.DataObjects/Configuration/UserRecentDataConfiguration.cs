using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// 表示用户最近的数据的配置元素
    /// </summary>
    public sealed class UserRecentDataCategoryConfigurationElement : PropertyGroupConfigurationElement
    {
        [ConfigurationProperty("defaultRecentCount", DefaultValue = 10)]
        public int DefaultRecentCount
        {
            get
            {
                return (int)this["defaultRecentCount"];
            }
            set
            {
                this["defaultRecentCount"] = value;
            }
        }
    }

    /// <summary>
    /// 表示<see cref="UserRecentDataCategoryConfigurationElement"/>配置元素的集合。
    /// </summary>
    public sealed class UserRecentDataCategoryConfigurationElementCollection : NamedConfigurationElementCollection<UserRecentDataCategoryConfigurationElement>
    {
    }

    /// <summary>
    /// 表示用户最近的数据的配置节
    /// </summary>
    public class UserRecentDataConfigurationSection : ConfigurationSection
    {
        public static UserRecentDataConfigurationSection GetConfig()
        {
            UserRecentDataConfigurationSection settings = (UserRecentDataConfigurationSection)ConfigurationBroker.GetSection("userRecentDataSettings");

            if (settings == null)
                settings = new UserRecentDataConfigurationSection();

            return settings;
        }

        [ConfigurationProperty("categories", IsRequired = false)]
        public UserRecentDataCategoryConfigurationElementCollection Categories
        {
            get
            {
                return (UserRecentDataCategoryConfigurationElementCollection)this["categories"];
            }
        }
    }
}
