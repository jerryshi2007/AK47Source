using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
    public class PropertyLayoutConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            PropertyLayoutSectionDefine plsd = new PropertyLayoutSectionDefine();
			plsd.Name = DictionaryHelper.GetValue(dictionary, "name", string.Empty);
			plsd.Columns = DictionaryHelper.GetValue(dictionary, "columns", default(int));
            plsd.DisplayName = DictionaryHelper.GetValue(dictionary, "displayName", string.Empty);
            plsd.DefaultRowHeight = DictionaryHelper.GetValue(dictionary, "defaultRowHeight", string.Empty);

            return new PropertyLayout(plsd);
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            PropertyLayout pl = (PropertyLayout)obj;

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "name", pl.LayoutSection.Name);
            DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "columns", pl.LayoutSection.Columns);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "displayName", pl.LayoutSection.DisplayName);
            DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "defaultRowHeight", pl.LayoutSection.DefaultRowHeight);

            return dictionary;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new System.Type[] { typeof(PropertyLayout) }; }
        }
    }
}
