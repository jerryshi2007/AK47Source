using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace MCS.Web.Library.Script
{
    internal class InternalDateTimeConverter : JavaScriptConverter
    {
        public static readonly JavaScriptConverter Instance = new InternalDateTimeConverter();

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict["DateValue"] = ((DateTime)obj).Ticks;
            dict["DateKind"] = ((DateTime)obj).Kind;

            return dict;
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            long ticks = long.Parse(dictionary["DateValue"].ToString());
            DateTimeKind kind = (DateTimeKind)dictionary["DateKind"];

            DateTime result = new DateTime(ticks, kind);

            if (result != DateTime.MinValue && result.Kind == DateTimeKind.Utc)
                result = result.ToLocalTime();

            return result;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new Type[] { typeof(DateTime) }; }
        }
    }
}
