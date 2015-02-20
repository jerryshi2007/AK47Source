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
    public class WfClientProcessDescriptorJsonConverter : WfClientKeyedDescriptorJsonConverterBase
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientProcessDescriptor) };

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = (Dictionary<string, object>)base.Serialize(obj, serializer);

            WfClientProcessDescriptor processDesp = (WfClientProcessDescriptor)obj;

            dictionary.Add("activities", processDesp.Activities);
            dictionary.Add("variables", processDesp.Variables);
            dictionary.Add("relativeLinks", processDesp.RelativeLinks);

            return dictionary;
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientProcessDescriptor processDesp = (WfClientProcessDescriptor)base.Deserialize(dictionary, type, serializer);

            JSONSerializerExecute.FillDeserializedCollection<WfClientActivityDescriptor>(dictionary.GetValue("activities", (object)null), processDesp.Activities);

            processDesp.NormalizeAllTransitions();

            JSONSerializerExecute.FillDeserializedCollection<WfClientVariableDescriptor>(dictionary.GetValue("variables", (object)null), processDesp.Variables);
            JSONSerializerExecute.FillDeserializedCollection<WfClientRelativeLinkDescriptor>(dictionary.GetValue("relativeLinks", (object)null), processDesp.RelativeLinks);

            return processDesp;
        }

        protected override WfClientKeyedDescriptorBase CreateInstance(string key, IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return new WfClientProcessDescriptor() { Key = key };
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return _SupportedTypes;
            }
        }
    }
}
