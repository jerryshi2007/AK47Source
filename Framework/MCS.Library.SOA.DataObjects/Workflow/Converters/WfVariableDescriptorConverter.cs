using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfVariableDescriptorConverter : WfDescriptorConverterBase
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, System.Web.Script.Serialization.JavaScriptSerializer serializer)
		{
			WfVariableDescriptor variableDesp = (WfVariableDescriptor)base.Deserialize(dictionary, type, serializer);

			variableDesp.OriginalType = DictionaryHelper.GetValue(dictionary, "OriginalType", DataType.String);
			variableDesp.OriginalValue = DictionaryHelper.GetValue(dictionary, "OriginalValue", string.Empty);

			return variableDesp;
		}

		public override IDictionary<string, object> Serialize(object obj, System.Web.Script.Serialization.JavaScriptSerializer serializer)
		{
			WfVariableDescriptor variDesp = (WfVariableDescriptor)obj;

			IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "OriginalType", variDesp.OriginalType);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "OriginalValue", variDesp.OriginalValue);

			return dictionary;
		}

		protected override WfKeyedDescriptorBase CreateInstance(string key, IDictionary<string, object> dictionary, Type type, System.Web.Script.Serialization.JavaScriptSerializer serializer)
		{
			return new WfVariableDescriptor(key);
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new System.Type[] { typeof(WfVariableDescriptor) };
			}
		}
	}
}
