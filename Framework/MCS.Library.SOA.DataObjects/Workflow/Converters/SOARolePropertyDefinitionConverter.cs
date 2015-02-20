using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public class SOARolePropertyDefinitionConverter : JavaScriptConverter
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(SOARolePropertyDefinition) };

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            SOARolePropertyDefinition pv = new SOARolePropertyDefinition();

            //ColumnDefinitionBase
            pv.Caption = DictionaryHelper.GetValue(dictionary, "caption", string.Empty);
            pv.Name = DictionaryHelper.GetValue(dictionary, "name", string.Empty);
            pv.DataType = DictionaryHelper.GetValue(dictionary, "dataType", ColumnDataType.String);
            pv.DefaultValue = DictionaryHelper.GetValue(dictionary, "defaultValue", (string)null);

            //SOARolePropertyDefinition
            pv.RoleID = DictionaryHelper.GetValue(dictionary, "roleID", string.Empty);
            pv.SortOrder = DictionaryHelper.GetValue(dictionary, "sortOrder", 0);
            pv.Description = DictionaryHelper.GetValue(dictionary, "description", string.Empty);

            return pv;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            SOARolePropertyDefinition pd = (SOARolePropertyDefinition)obj;

            //ColumnDefinitionBase
            DictionaryHelper.AddNonDefaultValue(dictionary, "caption", pd.Caption);
            DictionaryHelper.AddNonDefaultValue(dictionary, "name", pd.Name);
            DictionaryHelper.AddNonDefaultValue(dictionary, "dataType", pd.DataType);
            DictionaryHelper.AddNonDefaultValue(dictionary, "defaultValue", pd.DefaultValue);

            //SOARolePropertyDefinition
            DictionaryHelper.AddNonDefaultValue(dictionary, "roleID", pd.RoleID);
            DictionaryHelper.AddNonDefaultValue(dictionary, "sortOrder", pd.SortOrder);
            DictionaryHelper.AddNonDefaultValue(dictionary, "description", pd.Description);

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
