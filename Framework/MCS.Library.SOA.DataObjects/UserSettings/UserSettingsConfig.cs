using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 用户个人设置的配置信息
	/// </summary>
	public sealed class UserSettingsConfig : ConfigurationSection
	{
		public static UserSettingsConfig GetConfig()
		{
			UserSettingsConfig settings = (UserSettingsConfig)ConfigurationBroker.GetSection("userSettings");

			if (settings == null)
				settings = new UserSettingsConfig();

			return settings;
		}

		[ConfigurationProperty("categories", IsRequired = false)]
		public PropertyGroupConfigurationElementCollection Categories
		{
			get
			{
				return (PropertyGroupConfigurationElementCollection)this["categories"];
			}
		}
	}

	///// <summary>
	///// 个人设置的类别的集合
	///// </summary>
	//public class UserSettingsCategoryConfigurationElementCollection : NamedConfigurationElementCollection<UserSettingsCategoryConfigurationElement>
	//{
	//}

	///// <summary>
	///// 个人设置的类别
	///// </summary>
	//public class UserSettingsCategoryConfigurationElement : NamedConfigurationElement
	//{
	//    /// <summary>
	//    /// 每个个人设置类别的属性
	//    /// </summary>
	//    [ConfigurationProperty("properties", IsRequired = false)]
	//    public PropertyGroupConfigurationElement SettingProperties
	//    {
	//        get
	//        {
	//            return (PropertyGroupConfigurationElement)this["properties"];
	//        }
	//    }
	//}
}
