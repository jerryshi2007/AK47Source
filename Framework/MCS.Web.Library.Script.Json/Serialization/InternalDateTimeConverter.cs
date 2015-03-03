using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace MCS.Web.Library.Script
{
    internal class InternalDateTimeConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(DateTime) };

        public static readonly JavaScriptConverter Instance = new InternalDateTimeConverter();
        public static readonly JavaScriptConverter[] Instances = new JavaScriptConverter[] { InternalDateTimeConverter.Instance };

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            return DataConverter.ChangeType<DateTime, IDictionary<string, object>>((DateTime)obj);
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return DataConverter.ChangeType<IDictionary<string, object>, DateTime>(dictionary);
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
