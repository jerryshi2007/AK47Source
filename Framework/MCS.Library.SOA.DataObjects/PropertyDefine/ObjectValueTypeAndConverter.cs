using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	public class ObjectValueTypeAndConverter
	{
		public ObjectValueTypeAndConverter(Type valueType, IObjectValueToPropertyValue converter)
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

	public class ObjectValueTypeAndConverterCollection : EditableKeyedDataObjectCollectionBase<Type, ObjectValueTypeAndConverter>
	{
		protected override Type GetKeyForItem(ObjectValueTypeAndConverter item)
		{
			return item.ValueType;
		}

		public IObjectValueToPropertyValue GetEqualToConverter(Type type)
		{
			IObjectValueToPropertyValue result = null;

			if (this.ContainsKey(type))
				result = this[type].Converter;

			return result;
		}

		public IObjectValueToPropertyValue GetIsConverter(Type type)
		{
			IObjectValueToPropertyValue result = null;

			foreach (ObjectValueTypeAndConverter ovtc in this)
			{
				if (ovtc.ValueType.IsAssignableFrom(type))
				{
					result = ovtc.Converter;
					break;
				}
			}

			return result;
		}
	}
}
