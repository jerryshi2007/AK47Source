using MCS.Library.Core;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Json.Converters
{
    public class WfClientRelativeLinkDescriptorJsonConverter : WfClientKeyedDescriptorJsonConverterBase
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientRelativeLinkDescriptor) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientRelativeLinkDescriptor relativeLink = (WfClientRelativeLinkDescriptor)base.Deserialize(dictionary, type, serializer);

            relativeLink.Category = dictionary.GetValue("category", string.Empty);
            relativeLink.Url = dictionary.GetValue("url", string.Empty);

            return relativeLink;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            WfClientRelativeLinkDescriptor relativeLink = (WfClientRelativeLinkDescriptor)obj;

            IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

            dictionary.AddNonDefaultValue("category", relativeLink.Category);
            dictionary.AddNonDefaultValue("url", relativeLink.Url);

            return dictionary;
        }

        protected override WfClientKeyedDescriptorBase CreateInstance(string key, IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return new WfClientRelativeLinkDescriptor(key);
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
