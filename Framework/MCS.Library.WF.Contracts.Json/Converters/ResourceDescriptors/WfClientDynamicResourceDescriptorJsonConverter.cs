using MCS.Library.Core;
using MCS.Library.WF.Contracts.Ogu;
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
    public class WfClientDynamicResourceDescriptorJsonConverter : WfClientResourceDescriptorJsonConverterBase
    {
        public static readonly Type[] _SupportedType = new Type[] { typeof(WfClientDynamicResourceDescriptor) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientDynamicResourceDescriptor conditionResourceDesp = (WfClientDynamicResourceDescriptor)base.Deserialize(dictionary, type, serializer);

            conditionResourceDesp.Name = dictionary.GetValue("name", string.Empty);
            conditionResourceDesp.Condition = JSONSerializerExecute.Deserialize<WfClientConditionDescriptor>(dictionary.GetValue("condition", (object)null));

            return conditionResourceDesp;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            WfClientDynamicResourceDescriptor conditionResourceDesp = (WfClientDynamicResourceDescriptor)obj;

            IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

            dictionary.AddNonDefaultValue("name", conditionResourceDesp.Name);
            dictionary.AddNonDefaultValue("condition", conditionResourceDesp.Condition);

            return dictionary;
        }

        protected override WfClientResourceDescriptor CreateInstance(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return new WfClientDynamicResourceDescriptor();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return _SupportedType;
            }
        }
    }
}
