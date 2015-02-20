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
    public class WfClientDepartmentResourceDescriptorJsonConverter : WfClientResourceDescriptorJsonConverterBase
    {
        public static readonly Type[] _SupportedType = new Type[] { typeof(WfClientDepartmentResourceDescriptor) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientDepartmentResourceDescriptor deptResourceDesp = (WfClientDepartmentResourceDescriptor)base.Deserialize(dictionary, type, serializer);
            WfClientOrganization dept = JSONSerializerExecute.Deserialize<WfClientOrganization>(dictionary.GetValue("department", (object)null));

            deptResourceDesp.Department = dept;

            return deptResourceDesp;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            WfClientDepartmentResourceDescriptor deptResourceDesp = (WfClientDepartmentResourceDescriptor)obj;

            IDictionary<string, object> dictionary = base.Serialize(obj, serializer);

            dictionary.AddNonDefaultValue("department", deptResourceDesp.Department);

            return dictionary;
        }

        protected override WfClientResourceDescriptor CreateInstance(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return new WfClientDepartmentResourceDescriptor();
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
