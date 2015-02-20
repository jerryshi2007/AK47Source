using MCS.Library.Core;
using MCS.Web.Library.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public class SOARolePropertyRowConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(SOARolePropertyRow) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            SOARolePropertyRow row = new SOARolePropertyRow();

            row.RowNumber = dictionary.GetValue("rowNumber", 0);
            row.Operator = dictionary.GetValue("operator", string.Empty);
            row.OperatorType = dictionary.GetValue("operatorType", SOARoleOperatorType.User);

            JSONSerializerExecute.FillDeserializedCollection(dictionary["values"], row.Values);

            return row;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            SOARolePropertyRow row = (SOARolePropertyRow)obj;

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
