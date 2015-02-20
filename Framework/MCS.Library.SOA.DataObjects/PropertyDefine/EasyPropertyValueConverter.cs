using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
    public class EasyPropertyValueConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            PropertyDefine pd = new PropertyDefine();

            pd.Name = DictionaryHelper.GetValue(dictionary, "name", string.Empty);

            PropertyValue pv = new PropertyValue(pd);

            pv.StringValue = DictionaryHelper.GetValue(dictionary, "value", (string)null);

            return pv;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            PropertyValue prop = (PropertyValue)obj;

            DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "name", prop.Definition.Name);

            if (string.IsNullOrEmpty(prop.StringValue))
                DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "defaultValue", prop.Definition.DefaultValue);
            else
                dictionary.Add("value", prop.StringValue);

            return dictionary;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new System.Type[] { typeof(PropertyValue) };
            }
        }
    }
}
