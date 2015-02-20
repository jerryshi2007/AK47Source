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
    public class WfClientExportProcessDescriptorParamsJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientExportProcessDescriptorParams) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientExportProcessDescriptorParams data = new WfClientExportProcessDescriptorParams();

            data.MatrixRoleAsPerson = dictionary.GetValue("matrixRoleAsPerson", false);

            return data;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            WfClientExportProcessDescriptorParams data = (WfClientExportProcessDescriptorParams)obj;

            dictionary["matrixRoleAsPerson"] = data.MatrixRoleAsPerson;

            return dictionary;
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
