using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfControlNextStepConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfControlNextStep nextStep = new WfControlNextStep();

			if (dictionary.ContainsKey("ActivityDescriptor"))
				nextStep.ActivityDescriptor = JSONSerializerExecute.Deserialize<IWfActivityDescriptor>(dictionary["ActivityDescriptor"]);

			if (dictionary.ContainsKey("Candidates"))
				nextStep.Candidates = JSONSerializerExecute.Deserialize<WfAssigneeCollection>(dictionary["Candidates"]);

			if (dictionary.ContainsKey("TransitionDescriptor"))
				nextStep.TransitionDescriptor = JSONSerializerExecute.Deserialize<IWfTransitionDescriptor>(dictionary["TransitionDescriptor"]);

			return nextStep;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			WfControlNextStep nextStep = (WfControlNextStep)obj;
			IDictionary<string, object> dictionary = new Dictionary<string, object>();

			DictionaryHelper.AddNonDefaultValue(dictionary, "ActivityDescriptor", nextStep.ActivityDescriptor);
			DictionaryHelper.AddNonDefaultValue(dictionary, "Candidates", nextStep.Candidates);
			DictionaryHelper.AddNonDefaultValue(dictionary, "TransitionDescriptor", nextStep.TransitionDescriptor);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new System.Type[] { typeof(WfControlNextStep) }; }
		}
	}
}
