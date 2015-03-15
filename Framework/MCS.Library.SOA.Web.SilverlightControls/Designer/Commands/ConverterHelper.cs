using System;
using System.Net;
using System.Windows;
using System.ComponentModel;

namespace Designer.Commands
{
	public static class ConverterHelper
	{
		public static object ConvertToType(object value, Type type)
		{
			if (value == null)
			{
				return null;
			}
			if (type.IsAssignableFrom(value.GetType()))
			{
				return value;
			}
			TypeConverter typeConverter = ConverterHelper.GetTypeConverter(type);
			if (typeConverter != null && typeConverter.CanConvertFrom(value.GetType()))
			{
				value = typeConverter.ConvertFrom(value);
				return value;
			}
			return null;
		}
		public static TypeConverter GetTypeConverter(Type type)
		{
			TypeConverterAttribute typeConverterAttribute = (TypeConverterAttribute)Attribute.GetCustomAttribute(type, typeof(TypeConverterAttribute), false);
			if (typeConverterAttribute != null)
			{
				try
				{
					Type type2 = Type.GetType(typeConverterAttribute.ConverterTypeName, false);
					if (type2 != null)
					{
						return Activator.CreateInstance(type2) as TypeConverter;
					}
				}
				catch
				{
				}
			}
			return new ConvertFromStringConverter(type);
		}
	}
}
