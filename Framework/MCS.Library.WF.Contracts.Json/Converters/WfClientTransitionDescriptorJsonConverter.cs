using MCS.Library.Core;
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
    public class WfClientTransitionDescriptorJsonConverter : WfClientKeyedDescriptorJsonConverterBase
    {
        private static readonly Type[] _Supported = new Type[] { typeof(WfClientTransitionDescriptor) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientTransitionDescriptor transition = (WfClientTransitionDescriptor)base.Deserialize(dictionary, type, serializer);

            transition.FromActivityKey = dictionary.GetValue("fromActivityKey", string.Empty);
            transition.ToActivityKey = dictionary.GetValue("toActivityKey", string.Empty);
            transition.Condition = JSONSerializerExecute.Deserialize<WfClientConditionDescriptor>(dictionary.GetValue("condition", (object)null));

            return transition;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = (Dictionary<string, object>)base.Serialize(obj, serializer);
            WfClientTransitionDescriptor transition = (WfClientTransitionDescriptor)obj;

            dictionary.AddNonDefaultValue("fromActivityKey", transition.FromActivityKey);
            dictionary.AddNonDefaultValue("toActivityKey", transition.ToActivityKey);
            dictionary.AddNonDefaultValue("condition", transition.Condition);

            return dictionary;
        }

        protected override WfClientKeyedDescriptorBase CreateInstance(string key, IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return new WfClientTransitionDescriptor();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return _Supported;
            }
        }
    }
}
