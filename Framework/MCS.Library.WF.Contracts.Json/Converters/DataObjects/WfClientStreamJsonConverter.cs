using MCS.Library.Core;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
namespace MCS.Library.WF.Contracts.Json.Converters.DataObjects
{
    public class WfClientStreamJsonConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(System.IO.Stream) };
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            string strBase64 = dictionary.GetValue("__stream", string.Empty);
            byte[] rawBody = Convert.FromBase64String(strBase64);

            return new MemoryStream(rawBody);
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            System.IO.Stream stream = (System.IO.Stream)obj;

            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            byte[] buffer = stream.ToBytes();
            dictionary.AddNonDefaultValue("__stream", Convert.ToBase64String(buffer));

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
