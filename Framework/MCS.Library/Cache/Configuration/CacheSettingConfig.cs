#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	CacheSettingsSection.cs
// Remark	：	Cache的配置信息类
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    万振龙	    20070430		创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

using MCS.Library.Configuration;

namespace MCS.Library.Caching
{
	/// <summary>
	/// Cache的配置信息
	/// </summary>
	public sealed class CacheSettingsSection : DeluxeConfigurationSection
	{
		/// <summary>
		/// 获取Cache的配置信息
		/// </summary>
		/// <returns>Cache的配置信息</returns>
		public static CacheSettingsSection GetConfig()
		{
			CacheSettingsSection result = (CacheSettingsSection)ConfigurationBroker.GetSection("cacheSettings");

			if (result == null)
				result = new CacheSettingsSection();

			return result;
		}

		private CacheSettingsSection()
		{
		}

		/// <summary>
		/// 缺省的队列长度
		/// </summary>
		[ConfigurationProperty("defaultQueueLength", DefaultValue = 100)]
		public int DefaultQueueLength
		{
			get
			{
				return (int)this["defaultQueueLength"];
			}
		}

		/// <summary>
		/// 是否启用网页中显示Cache信息
		/// </summary>
		[ConfigurationProperty("enableCacheInfoPage", DefaultValue = false, IsRequired = false)]
		public bool EnableCacheInfoPage
		{
			get
			{
				return (bool)this["enableCacheInfoPage"];
			}
		}

		/// <summary>
		/// 清理间隔
		/// </summary>
		public TimeSpan ScanvageInterval
		{
			get
			{
				return TimeSpan.FromSeconds(this.ScanvageIntervalSeconds);
			}
		}

		[ConfigurationProperty("scanvageInterval", DefaultValue = 60)]
		private int ScanvageIntervalSeconds
		{
			get
			{
				return (int)this["scanvageInterval"];
			}
		}

		/*
        /// <summary>
        /// 如果Cache队列没有定义性能计数器实例名称，可以使用的缺省的Cache队列的性能计数器的实例名称。
        /// 如果DefaultInstanceName也没有定义，可以使用Cache队列的类型名称
        /// </summary>
        [ConfigurationProperty("defaultInstanceName", DefaultValue = "")]
        public string DefaultInstanceName
        {
            get
            {
                return (string)this["defaultInstanceName"];
            }
        }
		*/

		/// <summary>
		/// 具体每个Cache队列的设置
		/// </summary>
		[ConfigurationProperty("queueSettings")]
		public QueueSettingCollection QueueSettings
		{
			get
			{
				return (QueueSettingCollection)this["queueSettings"];
			}
		}
	}

	/// <summary>
	/// 每个Cache队列的设置集合
	/// </summary>
	public sealed class QueueSettingCollection : ConfigurationElementCollection
	{
		private QueueSettingCollection()
		{
		}

		/// <summary>
		/// 获取配置元素的键值
		/// </summary>
		/// <param name="element">配置元素</param>
		/// <returns>键值</returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((QueueSetting)element).TypeName;
		}

		/// <summary>
		/// 创建配置元素
		/// </summary>
		/// <returns></returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new QueueSetting();
		}

		/// <summary>
		/// 获取指定类型的队列设置
		/// </summary>
		/// <param name="type">类型</param>
		/// <returns>队列设置</returns>
		public QueueSetting this[System.Type type]
		{
			get
			{
				return (QueueSetting)BaseGet(type.FullName);
			}
		}
	}

	/// <summary>
	/// 每个Cache队列的设置
	/// </summary>
	public sealed class QueueSetting : ConfigurationElement
	{
		/// <summary>
		/// 默认Cache队列长度
		/// </summary>
		public const int CacheDefaultQueueLength = 100;

		internal QueueSetting()
		{
		}

		/// <summary>
		/// 对象的类型名称
		/// </summary>
		[ConfigurationProperty("typeName", Options = ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired)]
		public string TypeName
		{
			get
			{
				return (string)this["typeName"];
			}
		}

		/// <summary>
		/// 队列的深度
		/// </summary>
		[ConfigurationProperty("queueLength", DefaultValue = CacheDefaultQueueLength)]
		public int QueueLength
		{
			get
			{
				return (int)this["queueLength"];
			}
		}

		/*
        /// <summary>
        /// 该Cache队列所对应的性能计数器的实例名称
        /// </summary>
        [ConfigurationProperty("instanceName", DefaultValue = "")]
        public string InstanceName
        {
            get
            {
                return (string)this["instanceName"];
            }
        }
		 * */
	}
}
