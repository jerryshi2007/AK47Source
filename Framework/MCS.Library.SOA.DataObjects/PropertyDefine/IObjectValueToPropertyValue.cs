using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 对象的值到属性值（PropertyValue）
	/// </summary>
	public interface IObjectValueToPropertyValue
	{
		/// <summary>
		/// 转换成属性值
		/// </summary>
		/// <param name="objectValue"></param>
		/// <param name="pv"></param>
		/// <param name="target"></param>
		void ConvertToPropertyValue(Type objectValueType, object objectValue, PropertyValue pv, object target);

		/// <summary>
		/// 属性值再转换为对象值
		/// </summary>
		/// <param name="pv"></param>
		/// <param name="originalObjectValue"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		object PropertyValueToObjectValue(PropertyValue pv, Type objectValueType, object originalObjectValue, object target);
	}

	internal class TypeToIObjectValueToPropertyValue
	{
		public System.Type Type
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
}
