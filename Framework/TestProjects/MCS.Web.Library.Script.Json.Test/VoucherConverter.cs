using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Web.Library.Script.Json.Test
{
    public class VoucherConverter : JavaScriptConverter
    {
        public static readonly Type[] _SupportedTypes = new Type[] { typeof(VoucherEntity) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            VoucherEntity data = new VoucherEntity();

            data.Name = dictionary.GetValue("name", string.Empty);
            data.Items = JSONSerializerExecute.Deserialize<VoucherItemCollection>(dictionary["items"]);

            return data;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            VoucherEntity data = (VoucherEntity)obj;

            dictionary.AddNonDefaultValue("name", data.Name);
            dictionary["items"] = data.Items;

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
