using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfSimulationParametersConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfSimulationParameters result = new WfSimulationParameters();

			result.EnableServiceCall = DictionaryHelper.GetValue(dictionary, "EnableServiceCall", true);

			if (dictionary.ContainsKey("Variables"))
				result.Variables.CopyFrom(JSONSerializerExecute.Deserialize<WfVariableDescriptor[]>(dictionary["Variables"]));

			if (dictionary.ContainsKey("Creator"))
				result.Creator = JSONSerializerExecute.Deserialize<OguUser>(dictionary["Creator"]);

			return result;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			WfSimulationParameters simulationParameters = (WfSimulationParameters)obj;

			IDictionary<string, object> dictionary = new Dictionary<string, object>();

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "EnableServiceCall", simulationParameters.EnableServiceCall);
			dictionary.Add("Variables", simulationParameters.Variables);

			if (OguBase.IsNotNullOrEmpty(simulationParameters.Creator))
				dictionary.Add("Creator", simulationParameters.Creator);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new System.Type[] { typeof(WfSimulationParameters) }; }
		}
	}
}
