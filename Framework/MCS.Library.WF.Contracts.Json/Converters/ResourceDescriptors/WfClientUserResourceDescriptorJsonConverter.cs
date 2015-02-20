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
    public class WfClientUserResourceDescriptorJsonConverter : WfClientResourceDescriptorJsonConverterBase
    {
        public static readonly Type[] _SupportedType = new Type[] { typeof(WfClientUserResourceDescriptor) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientUserResourceDescriptor userResourceDesp = (WfClientUserResourceDescriptor)base.Deserialize(dictionary, type, serializer);
            WfClientUser user = JSONSerializerExecute.Deserialize<WfClientUser>(dictionary.GetValue("user", (object)null));

            userResourceDesp.User = user;

            return userResourceDesp;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            WfClientUserResourceDescriptor userResourceDesp = (WfClientUserResourceDescriptor)obj;

            IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

            dictionary.AddNonDefaultValue("user", userResourceDesp.User);

            return dictionary;
        }

        protected override WfClientResourceDescriptor CreateInstance(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return new WfClientUserResourceDescriptor();
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
