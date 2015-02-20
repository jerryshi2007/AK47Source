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
    public class WfClientGroupResourceDescriptorJsonConverter : WfClientResourceDescriptorJsonConverterBase
    {
        public static readonly Type[] _SupportedType = new Type[] { typeof(WfClientGroupResourceDescriptor) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientGroupResourceDescriptor groupResourceDesp = (WfClientGroupResourceDescriptor)base.Deserialize(dictionary, type, serializer);
            WfClientGroup dept = JSONSerializerExecute.Deserialize<WfClientGroup>(dictionary.GetValue("group", (object)null));

            groupResourceDesp.Group = dept;

            return groupResourceDesp;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            WfClientGroupResourceDescriptor groupResourceDesp = (WfClientGroupResourceDescriptor)obj;

            IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

            dictionary.AddNonDefaultValue("group", groupResourceDesp.Group);

            return dictionary;
        }

        protected override WfClientResourceDescriptor CreateInstance(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return new WfClientGroupResourceDescriptor();
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
