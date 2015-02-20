using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public abstract class WfDescriptorConverterBase : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			string key = DictionaryHelper.GetValue(dictionary, "Key", string.Empty);

			WfKeyedDescriptorBase desp = CreateInstance(key, dictionary, type, serializer);

			desp.Name = DictionaryHelper.GetValue(dictionary, "Name", string.Empty); ;
			desp.Enabled = DictionaryHelper.GetValue(dictionary, "Enabled", false);
			desp.Description = DictionaryHelper.GetValue(dictionary, "Description", string.Empty);

			if (dictionary.ContainsKey("Properties"))
			{
				PropertyValueCollection properties = JSONSerializerExecute.Deserialize<PropertyValueCollection>(dictionary["Properties"]);

				desp.Properties.Clear();
				desp.Properties.CopyFrom(properties);
			}

			return desp;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			WfKeyedDescriptorBase desp = (WfKeyedDescriptorBase)obj;

			Dictionary<string, object> dictionary = new Dictionary<string, object>();

			dictionary.Add("Key", desp.Key);
			dictionary.Add("Name", desp.Name);

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Description", desp.Description);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Enabled", desp.Enabled);

			dictionary.Add("Properties", desp.Properties);

			return dictionary;
		}

		protected abstract WfKeyedDescriptorBase CreateInstance(string key, IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer);
	}
}
