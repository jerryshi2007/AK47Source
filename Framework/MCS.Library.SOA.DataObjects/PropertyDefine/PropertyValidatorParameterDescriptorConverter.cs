using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	public class PropertyValidatorParameterDescriptorConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			PropertyValidatorParameterDescriptor propValiParamDesp = new PropertyValidatorParameterDescriptor();

			propValiParamDesp.ParamName = DictionaryHelper.GetValue(dictionary, "paramName", string.Empty);

			propValiParamDesp.DataType = DictionaryHelper.GetValue(dictionary, "dataType", PropertyDataType.String);

			propValiParamDesp.ParamValue = DictionaryHelper.GetValue(dictionary, "paramValue", string.Empty);

			return propValiParamDesp;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();

			PropertyValidatorParameterDescriptor propValiParamDesp = (PropertyValidatorParameterDescriptor)obj;

			dictionary.Add("paramName", propValiParamDesp.ParamName);
			dictionary.Add("dataType", propValiParamDesp.DataType);
			dictionary.Add("paramValue", propValiParamDesp.ParamValue);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new System.Type[] { typeof(PropertyValidatorParameterDescriptor) }; }
		}
	}
}
