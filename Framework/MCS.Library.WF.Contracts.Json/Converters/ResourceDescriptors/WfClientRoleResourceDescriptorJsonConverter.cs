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
    public class WfClientRoleResourceDescriptorJsonConverter : WfClientResourceDescriptorJsonConverterBase
    {
        public static readonly Type[] _SupportedType = new Type[] { typeof(WfClientRoleResourceDescriptor) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientRoleResourceDescriptor roleResourceDesp = (WfClientRoleResourceDescriptor)base.Deserialize(dictionary, type, serializer);

            roleResourceDesp.FullCodeName = dictionary.GetValue("fullCodeName", string.Empty);

            return roleResourceDesp;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            WfClientRoleResourceDescriptor roleResourceDesp = (WfClientRoleResourceDescriptor)obj;

            IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

            dictionary.AddNonDefaultValue("fullCodeName", roleResourceDesp.FullCodeName);

            return dictionary;
        }

        protected override WfClientResourceDescriptor CreateInstance(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return new WfClientRoleResourceDescriptor();
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
