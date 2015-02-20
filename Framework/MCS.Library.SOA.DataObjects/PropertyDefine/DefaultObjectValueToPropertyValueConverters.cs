using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	public class DefaultObjectValueToPropertyValue : IObjectValueToPropertyValue
	{
		public static readonly IObjectValueToPropertyValue Instance = new DefaultObjectValueToPropertyValue();

		private DefaultObjectValueToPropertyValue()
		{
		}

		#region IObjectValueToPropertyValue Members

		public void ConvertToPropertyValue(Type objectValueType, object objectValue, PropertyValue pv, object target)
		{
			pv.Definition.DataType = objectValueType.ToPropertyDataType();

			if (objectValue != null)
				pv.StringValue = objectValue.ToString();
		}

		public object PropertyValueToObjectValue(PropertyValue pv, Type objectValueType, object originalObjectValue, object target)
		{
			return DataConverter.ChangeType(pv.GetRealValue(), objectValueType);
		}

		#endregion
	}
}
