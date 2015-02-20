using MCS.Library.Core;
using MCS.Library.WF.Contracts.PropertyDefine;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Web.Library.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Json.Converters
{
    public abstract class WfClientKeyedDescriptorJsonConverterBase : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            string key = DictionaryHelper.GetValue(dictionary, "key", string.Empty);

            WfClientKeyedDescriptorBase desp = CreateInstance(key, dictionary, type, serializer);

            desp.Name = DictionaryHelper.GetValue(dictionary, "name", string.Empty); ;
            desp.Enabled = DictionaryHelper.GetValue(dictionary, "enabled", false);
            desp.Description = DictionaryHelper.GetValue(dictionary, "description", string.Empty);

            JSONSerializerExecute.FillDeserializedCollection<ClientPropertyValue>(dictionary.GetValue("properties", (object)null), desp.Properties);

            return desp;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            WfClientKeyedDescriptorBase desp = (WfClientKeyedDescriptorBase)obj;

            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            dictionary.AddNonDefaultValue("key", desp.Key);
            dictionary.AddNonDefaultValue("name", desp.Name);

            DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "description", desp.Description);
            DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "enabled", desp.Enabled);

            dictionary.Add("properties", desp.Properties);

            return dictionary;
        }

        protected abstract WfClientKeyedDescriptorBase CreateInstance(string key, IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer);
    }
}
