using MCS.Library.Core;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Web.Library.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Json.Converters.DataObjects
{
    public class WfClientApprovalMatrixConverterJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientApprovalMatrix) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientApprovalMatrix resource = new WfClientApprovalMatrix();

            resource.ID = dictionary.GetValue("id", string.Empty);

            JSONSerializerExecute.FillDeserializedCollection(dictionary["definitions"], resource.PropertyDefinitions);
            JSONSerializerExecute.FillDeserializedCollection(dictionary["rows"], resource.Rows);

            return resource;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            IDictionary<string, object> dictionary = new Dictionary<string, object>();

            WfClientApprovalMatrix resource = (WfClientApprovalMatrix)obj;

            dictionary["id"] = resource.ID;
            dictionary["definitions"] = resource.PropertyDefinitions;
            dictionary["rows"] = resource.Rows;

            resource.Rows.FillColumnInfoToRowValues(resource.PropertyDefinitions);

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
