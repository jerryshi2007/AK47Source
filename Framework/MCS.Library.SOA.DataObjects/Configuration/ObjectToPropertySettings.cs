using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Core;
using MCS.Library.Configuration;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 对象的值映射到属性值的配置信息
	/// </summary>
	public sealed class ObjectValueToPropertyValueSettings : ConfigurationSection
	{
		public static ObjectValueToPropertyValueSettings GetConfig()
		{
			ObjectValueToPropertyValueSettings settings = (ObjectValueToPropertyValueSettings)ConfigurationBroker.GetSection("objectValueToPropertyValueSettings");

			if (settings == null)
				settings = new ObjectValueToPropertyValueSettings();

			return settings;
		}

		private ObjectValueToPropertyValueSettings()
		{
		}

		public ObjectValueTypeAndConverterCollection GetTypeEqualToConvertors()
		{
			return GetConverters("ObjectValueToPropertyValueSettings.TypeEqualToConverters", this.TypeEqualToConverters);
		}

		public ObjectValueTypeAndConverterCollection GetTypeIsConvertors()
		{
			return GetConverters("ObjectValueToPropertyValueSettings.TypeIsConverters", this.TypeIsConverters);
		}

		private ObjectValueTypeAndConverterCollection GetConverters(string cacheKey, ObjectValueToPropertyValueConfigurationElementCollection configElements)
		{
			ObjectValueTypeAndConverterCollection result = (ObjectValueTypeAndConverterCollection)ObjectContextCache.Instance.GetOrAddNewValue(
					cacheKey,
					(cache, key) =>
					{
						ObjectValueTypeAndConverterCollection instances = new ObjectValueTypeAndConverterCollection();

						foreach (ObjectValueToPropertyValueConfigurationElement elem in configElements)
						{
							try
							{
								instances.Add(new ObjectValueTypeAndConverter(elem.GetObjectValueType(), (IObjectValueToPropertyValue)elem.CreateInstance()));
							}
							catch (System.Exception)
							{
							}
						}
						cache.Add(key, instances);

						return instances;
					});

			return result;
		}

		[ConfigurationProperty("typeEqualToConverters", IsRequired = false)]
		private ObjectValueToPropertyValueConfigurationElementCollection TypeEqualToConverters
		{
			get
			{
				return (ObjectValueToPropertyValueConfigurationElementCollection)this["typeEqualToConverters"];
			}
		}

		[ConfigurationProperty("typeIsConverters", IsRequired = false)]
		private ObjectValueToPropertyValueConfigurationElementCollection TypeIsConverters
		{
			get
			{
				return (ObjectValueToPropertyValueConfigurationElementCollection)this["typeIsConverters"];
			}
		}
	}
}
