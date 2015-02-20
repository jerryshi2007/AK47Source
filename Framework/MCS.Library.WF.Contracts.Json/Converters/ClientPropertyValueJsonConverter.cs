using MCS.Library.Core;
using MCS.Library.WF.Contracts.PropertyDefine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Json.Converters
{
    public class ClientPropertyValueJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(ClientPropertyValue) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            ClientPropertyValue pv = new ClientPropertyValue();

            pv.Key = DictionaryHelper.GetValue(dictionary, "key", string.Empty);
            pv.DataType = DictionaryHelper.GetValue(dictionary, "dataType", ClientPropertyDataType.String);
            pv.StringValue = DictionaryHelper.GetValue(dictionary, "stringValue", string.Empty);

            return pv;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            ClientPropertyValue pv = (ClientPropertyValue)obj;

            DictionaryHelper.AddNonDefaultValue(dictionary, "key", pv.Key);
            DictionaryHelper.AddNonDefaultValue(dictionary, "dataType", pv.DataType);
            DictionaryHelper.AddNonDefaultValue(dictionary, "stringValue", pv.StringValue);

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
