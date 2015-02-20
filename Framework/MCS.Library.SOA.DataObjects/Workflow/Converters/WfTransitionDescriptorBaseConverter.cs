using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Web.Script.Serialization;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public abstract class WfTransitionDescriptorConverterBase : WfDescriptorConverterBase
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfTransitionDescriptor transitionDesp = (WfTransitionDescriptor)base.Deserialize(dictionary, type, serializer);
			
			transitionDesp.ToActivityKey = DictionaryHelper.GetValue(dictionary, "ToActivityKey", string.Empty);
			transitionDesp.FromActivityKey = DictionaryHelper.GetValue(dictionary, "FromActivityKey", string.Empty);

			if (dictionary.ContainsKey("Condition"))
			{ 
				transitionDesp.Condition = JSONSerializerExecute.Deserialize<WfConditionDescriptor>(dictionary["Condition"]);
				transitionDesp.Condition.Owner = transitionDesp;
			}

			WfVariableDescriptorCollection variables = JSONSerializerExecute.Deserialize<WfVariableDescriptorCollection>(dictionary["Variables"]);
			transitionDesp.Variables.Clear();
			transitionDesp.Variables.CopyFrom(variables);

			return transitionDesp;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			WfTransitionDescriptor transitionDesp = (WfForwardTransitionDescriptor)obj;

			IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Priority", transitionDesp.Priority);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "DefaultSelect", transitionDesp.DefaultSelect);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "__type", transitionDesp.GetType().AssemblyQualifiedName);

			dictionary.Add("ToActivityKey", transitionDesp.ToActivityKey);
			dictionary.Add("FromActivityKey", transitionDesp.FromActivityKey);

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Condition", ((WfForwardTransitionDescriptor)transitionDesp).Condition);

			dictionary.Add("Variables", transitionDesp.Variables);
			return dictionary;
		}
	}
}
