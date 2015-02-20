using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MCS.Library.Caching;
using MCS.Library.Configuration;
using MCS.Library.Core;

namespace MCS.Library.Services.Configuration
{
	/// <summary>
	/// Web服务中每一个方法的缓存相关定义
	/// </summary>
	public sealed class ServiceMethodConfigurationElement : NamedConfigurationElement
	{
		private Dictionary<string, string> _UnrecognizedAttribute = new Dictionary<string, string>();

		/// <summary>
		/// 默认超时时间
		/// </summary>
		public static readonly TimeSpan DefaultCacheSlidingTime = new TimeSpan(23, 59, 59);

		/// <summary>
		/// 是否使用Cache
		/// </summary>
		[ConfigurationProperty("cacheEnabled", DefaultValue = false, IsRequired = false)]
		public bool CacheEnabled
		{
			get
			{
				return (bool)this["cacheEnabled"];
			}
		}

		/// <summary>
		/// Cache队列长度
		/// </summary>
		[ConfigurationProperty("queueLength", DefaultValue = QueueSetting.CacheDefaultQueueLength)]
		public int QueueLength
		{
			get
			{
				return (int)this["queueLength"];
			}
		}

		/// <summary>
		/// 缓存的滑动过期时间。指的是最后一次访问Cache项后，多长时间过期
		/// </summary>
		[ConfigurationProperty("cacheSlidingTime", DefaultValue = "23:59:59")]
		public TimeSpan CacheSlidingTime
		{
			get
			{
				return (TimeSpan)this["cacheSlidingTime"];
			}
		}

		/// <summary>
		/// 得到没有预定义好的配置属性值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="attrName"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public T GetValue<T>(string attrName, T defaultValue)
		{
			T result = defaultValue;

			string value = null;

			if (this._UnrecognizedAttribute.TryGetValue(attrName, out value))
				result = (T)DataConverter.ChangeType(value, typeof(T));

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			lock (this._UnrecognizedAttribute)
			{
				_UnrecognizedAttribute[name] = value;
			}

			return true;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public sealed class ServiceMethodConfigurationElementCollection : NamedConfigurationElementCollection<ServiceMethodConfigurationElement>
	{
	}
}