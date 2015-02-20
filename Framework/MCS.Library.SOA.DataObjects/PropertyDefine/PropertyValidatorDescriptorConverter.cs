using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects
{
	public class PropertyValidatorDescriptorConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			PropertyValidatorDescriptor propValiDesp = new PropertyValidatorDescriptor();
			propValiDesp.Name = DictionaryHelper.GetValue(dictionary, "name", string.Empty);
			propValiDesp.TypeDescription = DictionaryHelper.GetValue(dictionary, "typeDescription", string.Empty);
			propValiDesp.MessageTemplate = DictionaryHelper.GetValue(dictionary, "messageTemplate", string.Empty);
			propValiDesp.Tag = DictionaryHelper.GetValue(dictionary, "tag", string.Empty);

			if (dictionary.ContainsKey("validatorParameters") == true)
			{
				PropertyValidatorParameterDescriptorCollection parameters = JSONSerializerExecute.Deserialize<PropertyValidatorParameterDescriptorCollection>(dictionary["validatorParameters"]);
				propValiDesp.Parameters.Clear();
				propValiDesp.Parameters.CopyFrom(parameters);
			}

			return propValiDesp;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			PropertyValidatorDescriptor propValiDesp = (PropertyValidatorDescriptor)obj;

			dictionary.Add("name", propValiDesp.Name);
			dictionary.Add("typeDescription", propValiDesp.TypeDescription);
			dictionary.Add("messageTemplate", propValiDesp.MessageTemplate);
			dictionary.Add("tag", propValiDesp.Tag);

			dictionary.Add("validatorParameters", propValiDesp.Parameters);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new System.Type[] { typeof(PropertyValidatorDescriptor) }; }
		}
	}
}
