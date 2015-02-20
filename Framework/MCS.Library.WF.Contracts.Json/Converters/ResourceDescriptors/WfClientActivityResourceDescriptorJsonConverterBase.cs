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
    public abstract class WfClientActivityResourceDescriptorJsonConverterBase : WfClientResourceDescriptorJsonConverterBase
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientActivityResourceDescriptorBase actResourceDesp = (WfClientActivityResourceDescriptorBase)base.Deserialize(dictionary, type, serializer);

            actResourceDesp.ActivityKey = dictionary.GetValue("activityKey", string.Empty);

            return actResourceDesp;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            WfClientActivityResourceDescriptorBase actResourceDesp = (WfClientActivityResourceDescriptorBase)obj;

            IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

            dictionary.AddNonDefaultValue("activityKey", actResourceDesp.ActivityKey);

            return dictionary;
        }
    }

    public class WfClientActivityAssigneesResourceDescriptorJsonConverter : WfClientActivityResourceDescriptorJsonConverterBase
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientActivityAssigneesResourceDescriptor) };

        protected override WfClientResourceDescriptor CreateInstance(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return new WfClientActivityAssigneesResourceDescriptor();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return _SupportedTypes;
            }
        }
    }

    public class WfClientActivityOperatorResourceDescriptorJsonConverter : WfClientActivityResourceDescriptorJsonConverterBase
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientActivityOperatorResourceDescriptor) };

        protected override WfClientResourceDescriptor CreateInstance(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return new WfClientActivityOperatorResourceDescriptor();
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
