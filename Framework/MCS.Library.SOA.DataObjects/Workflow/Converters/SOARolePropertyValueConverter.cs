using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public class SOARolePropertyValueConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(SOARolePropertyValue) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            string columnName = dictionary.GetValue("columnName", string.Empty);

            SOARolePropertyValue pv = new SOARolePropertyValue(new SOARolePropertyDefinition() { Name = columnName });

            pv.Value = dictionary.GetValue("value", string.Empty);

            return pv;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            SOARolePropertyValue pv = (SOARolePropertyValue)obj;

            DictionaryHelper.AddNonDefaultValue(dictionary, "columnName", pv.Column.Name);
            DictionaryHelper.AddNonDefaultValue(dictionary, "value", pv.Value);

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
