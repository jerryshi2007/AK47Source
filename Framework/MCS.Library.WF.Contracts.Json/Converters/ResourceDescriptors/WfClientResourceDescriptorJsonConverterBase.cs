using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Json.Converters
{
    public abstract class WfClientResourceDescriptorJsonConverterBase : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientResourceDescriptor resource = CreateInstance(dictionary, type, serializer);

            return resource;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            dictionary.Add("shortType", obj.GetType().Name);
            dictionary.Add("__type", obj.GetType().AssemblyQualifiedName);

            return dictionary;
        }

        protected abstract WfClientResourceDescriptor CreateInstance(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer);
    }
}
