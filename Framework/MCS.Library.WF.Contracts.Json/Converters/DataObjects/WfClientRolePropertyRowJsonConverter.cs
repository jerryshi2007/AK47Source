using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Web.Library.Script;

namespace MCS.Library.WF.Contracts.Json.Converters.DataObjects
{
    public class WfClientRolePropertyRowJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientRolePropertyRow) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            WfClientRolePropertyRow row = new WfClientRolePropertyRow();

            row.RowNumber = dictionary.GetValue("rowNumber", 0);
            row.Operator = dictionary.GetValue("operator", string.Empty);
            row.OperatorType = dictionary.GetValue("operatorType", WfClientRoleOperatorType.User);
            JSONSerializerExecute.FillDeserializedCollection(dictionary["values"], row.Values);

            return row;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            WfClientRolePropertyRow row = (WfClientRolePropertyRow)obj;

            dictionary.AddNonDefaultValue("rowNumber", row.RowNumber);
            dictionary.AddNonDefaultValue("operatorType", row.OperatorType);
            dictionary.AddNonDefaultValue("operator", row.Operator);

            dictionary["values"] = JSONSerializerExecute.Serialize(row.Values);

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
