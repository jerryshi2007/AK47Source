using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// OguObject的属性转换器
	/// </summary>
	public class OguObjectValueToPropertyValue : IObjectValueToPropertyValue
	{
		#region IObjectValueToPropertyValue Members

		public void ConvertToPropertyValue(Type objectValueType, object objectValue, PropertyValue pv, object target)
		{
			(objectValue is IOguObject).FalseThrow("objectValue必须是IOguObject类型");

			pv.Definition.DataType = PropertyDataType.DataObject;

			if (objectValue != null)
			{
				pv.StringValue = JSONSerializerExecute.Serialize(objectValue);
			}
		}

		public object PropertyValueToObjectValue(PropertyValue pv, Type objectValueType, object originalObjectValue, object target)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
