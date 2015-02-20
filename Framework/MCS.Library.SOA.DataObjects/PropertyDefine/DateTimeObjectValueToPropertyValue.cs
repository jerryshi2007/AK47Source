using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	public class DateTimeObjectValueToPropertyValue : IObjectValueToPropertyValue
	{
		public static readonly IObjectValueToPropertyValue Instance = new DateTimeObjectValueToPropertyValue();

		private DateTimeObjectValueToPropertyValue()
		{
		}

		#region IObjectValueToPropertyValue Members

		public void ConvertToPropertyValue(Type objectValueType, object objectValue, PropertyValue pv, object target)
		{
			pv.Definition.DataType = PropertyDataType.DateTime;

			if (objectValue != null && objectValue is DateTime)
				pv.StringValue = string.Format("{0:yyyy-MM-dd HH:mm:ss}", objectValue);
		}

		public object PropertyValueToObjectValue(PropertyValue pv, Type objectValueType, object originalObjectValue, object target)
		{
			return DataConverter.ChangeType(pv.GetRealValue(), objectValueType);
		}

		#endregion
	}
}
