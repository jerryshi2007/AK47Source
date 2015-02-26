using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Web.Library.Script.Json.Test
{
    public class VoucherItemCollectionConverter : JavaScriptConverter
    {
        public static readonly Type[] _SupportedTypes = new Type[] { typeof(VoucherItemCollection) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            VoucherItemCollection data = new VoucherItemCollection();

            data.CollectioName = dictionary.GetValue("collectioName", string.Empty);
            JSONSerializerExecute.FillDeserializedCollection(dictionary["items"], data);

            return data;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            VoucherItemCollection data = (VoucherItemCollection)obj;

            dictionary.AddNonDefaultValue("collectioName", data.CollectioName);
            dictionary["items"] = data.ToArray();

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
