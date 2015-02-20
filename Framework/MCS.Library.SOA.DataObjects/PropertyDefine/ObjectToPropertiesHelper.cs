using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 对象转换到属性值的帮助类
	/// </summary>
	public static class ObjectToPropertiesHelper
	{
		private class TypeAndConverter
		{
			public TypeAndConverter(Type valueType, IObjectValueToPropertyValue converter)
			{
				this.ValueType = valueType;
				this.Converter = converter;
			}

			public Type ValueType
			{
				get;
				set;
			}

			public IObjectValueToPropertyValue Converter
			{
				get;
				set;
			}
		}

		/// <summary>
		/// 初始化一个字典，类型相等时的对象值的转换器。如果传递一个类型进来，会进行Type的比较，获得转换器
		/// </summary>
		private static ObjectValueTypeAndConverterCollection _BuiltInEqualTypeObjectValueConverters = new ObjectValueTypeAndConverterCollection()
		{
			{new ObjectValueTypeAndConverter(typeof(string), DefaultObjectValueToPropertyValue.Instance)},
			{new ObjectValueTypeAndConverter(typeof(decimal), DefaultObjectValueToPropertyValue.Instance)},
			{new ObjectValueTypeAndConverter(typeof(int), DefaultObjectValueToPropertyValue.Instance)},
			{new ObjectValueTypeAndConverter(typeof(double), DefaultObjectValueToPropertyValue.Instance)},
			{new ObjectValueTypeAndConverter(typeof(float), DefaultObjectValueToPropertyValue.Instance)},
			{new ObjectValueTypeAndConverter(typeof(Int64), DefaultObjectValueToPropertyValue.Instance)},
			{new ObjectValueTypeAndConverter(typeof(Boolean), DefaultObjectValueToPropertyValue.Instance)},
			{new ObjectValueTypeAndConverter(typeof(DateTime), DateTimeObjectValueToPropertyValue.Instance)}
		};

		/// <summary>
		/// 初始化一个列表，如果对象的值类型“Is”列表中的Type，则使用对应的转换器。这是个列表，是按照顺序排列的。一般子类在前，基类在后。
		/// </summary>
		private static ObjectValueTypeAndConverterCollection _BuiltInIsTypeObjectValueConverters = new ObjectValueTypeAndConverterCollection()
		{
		};

		/// <summary>
		/// 从字典转换为属性集合
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="keyValuePairs"></param>
		/// <returns></returns>
		public static PropertyValueCollection ToProperties<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
		{
			PropertyValueCollection properties = new PropertyValueCollection();

			foreach (KeyValuePair<TKey, TValue> kp in keyValuePairs)
			{
				if (kp.Key != null)
				{
					PropertyValue pv = BuildPropertyValue(kp.Key.ToString(), string.Empty);

					if (kp.Value != null)
					{
						System.Type type = kp.Value.GetType();
						IObjectValueToPropertyValue converter = GetObjectValueConverter(type);

						if (converter != null)
						{
							pv.Definition.ReadOnly = false;
							converter.ConvertToPropertyValue(type, kp.Value, pv, kp);
						}
						else
						{
							pv.StringValue = kp.Value.ToString();
						}

						pv.Definition.DefaultValue = pv.StringValue;
					}

					properties.Add(pv);
				}
			}

			return properties;
		}

		/// <summary>
		/// 属性集合回填到字典中
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="pvs"></param>
		/// <param name="dictionary"></param>
		public static void FillDictionary<TKey, TValue>(this PropertyValueCollection pvs, IDictionary<TKey, TValue> dictionary)
		{
			if (pvs != null && dictionary != null)
			{
				foreach (PropertyValue pv in pvs)
				{
					TValue originalObjectValue = default(TValue);
					TKey key = (TKey)DataConverter.ChangeType(pv.Definition.Name, typeof(TKey));

					if (dictionary.TryGetValue(key, out originalObjectValue))
					{
						if (pv.Definition.ReadOnly == false && originalObjectValue != null)
						{
							System.Type type = originalObjectValue.GetType();

							IObjectValueToPropertyValue converter = GetObjectValueConverter(type);

							dictionary[key] =(TValue)converter.PropertyValueToObjectValue(pv, type, originalObjectValue, pvs);
						}
					}
					else
						dictionary.Add(key, (TValue)pv.GetRealValue());
				}
			}
		}

		private static PropertyValue BuildPropertyValue(string name, string category)
		{
			PropertyDefine define = new PropertyDefine();

			define.Name = name;
			define.Category = category;
			define.ReadOnly = true;

			PropertyValue result = new PropertyValue(define);

			return result;
		}

		public static IObjectValueToPropertyValue GetObjectValueConverter(Type type)
		{
			IObjectValueToPropertyValue result = null;

			result = ObjectValueToPropertyValueSettings.GetConfig().GetTypeEqualToConvertors().GetEqualToConverter(type);

			if (result == null)
				result = _BuiltInEqualTypeObjectValueConverters.GetEqualToConverter(type);

			if (result == null)
				result = ObjectValueToPropertyValueSettings.GetConfig().GetTypeIsConvertors().GetIsConverter(type);

			if (result == null)
				result = _BuiltInIsTypeObjectValueConverters.GetIsConverter(type);

			return result;
		}
	}
}