using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Core;
using MCS.Web.Library.Script;
using MCS.Library.Validation;

namespace MCS.Library.SOA.DataObjects
{
	public class PropertyValueConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			PropertyDefine pd = new PropertyDefine();

			pd.Name = DictionaryHelper.GetValue(dictionary, "name", string.Empty);
			pd.DisplayName = DictionaryHelper.GetValue(dictionary, "displayName", string.Empty);
			pd.Category = DictionaryHelper.GetValue(dictionary, "category", string.Empty);
			pd.DefaultValue = DictionaryHelper.GetValue(dictionary, "defaultValue", string.Empty);
			pd.DataType = DictionaryHelper.GetValue(dictionary, "dataType", PropertyDataType.String);
			pd.Description = DictionaryHelper.GetValue(dictionary, "description", string.Empty);
			pd.ReadOnly = DictionaryHelper.GetValue(dictionary, "readOnly", false);
			pd.Visible = DictionaryHelper.GetValue(dictionary, "visible", true);
			pd.EditorKey = DictionaryHelper.GetValue(dictionary, "editorKey", string.Empty);
			pd.PersisterKey = DictionaryHelper.GetValue(dictionary, "persisterKey", string.Empty);
            pd.EditorParamsSettingsKey = DictionaryHelper.GetValue(dictionary, "editorParamsSettingsKey", string.Empty);
            pd.EditorParams = DictionaryHelper.GetValue(dictionary, "editorParams", string.Empty);
			pd.SortOrder = DictionaryHelper.GetValue(dictionary, "sortOrder", 0xFFFF);
			pd.MaxLength = DictionaryHelper.GetValue(dictionary, "maxLength", 0xFFFF);
			pd.IsRequired = DictionaryHelper.GetValue(dictionary, "isRequired", false);
            pd.ShowTitle = DictionaryHelper.GetValue(dictionary, "showTitle", true);

			if (dictionary.ContainsKey("validators") == true)
			{
				PropertyValidatorDescriptorCollection validators = JSONSerializerExecute.Deserialize<PropertyValidatorDescriptorCollection>(dictionary["validators"]);
				pd.Validators.Clear();
				pd.Validators.CopyFrom(validators);
			}

			PropertyValue pv = new PropertyValue(pd);

			pv.StringValue = DictionaryHelper.GetValue(dictionary, "value", (string)null);

			return pv;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			PropertyValue prop = (PropertyValue)obj;

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "name", prop.Definition.Name);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "displayName", prop.Definition.DisplayName);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "category", prop.Definition.Category);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "defaultValue", prop.Definition.DefaultValue);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "dataType", prop.Definition.DataType);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "description", prop.Definition.Description);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "readOnly", prop.Definition.ReadOnly);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "visible", prop.Definition.Visible);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "editorKey", prop.Definition.EditorKey);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "persisterKey", prop.Definition.PersisterKey);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "isRequired", prop.Definition.IsRequired);
            DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "showTitle", prop.Definition.ShowTitle);

            DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "editorParamsSettingsKey", prop.Definition.EditorParamsSettingsKey);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "editorParams", prop.Definition.EditorParams);
			if (prop.Definition.Validators.Count > 0)
			{
				string jsonStr = JSONSerializerExecute.Serialize(prop.Definition.Validators);
				DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "validators", jsonStr);
				DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "clientVdtData", prop.Definition.GetPropertyValidator());
			}

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "maxLength", prop.Definition.MaxLength);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "sortOrder", prop.Definition.SortOrder);

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
