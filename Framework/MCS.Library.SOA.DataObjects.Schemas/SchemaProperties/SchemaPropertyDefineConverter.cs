using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Schemas.SchemaProperties
{
	public sealed class SchemaPropertyDefineConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			SchemaPropertyDefine sechemaPropertyDefine = new SchemaPropertyDefine();

			#region "PropertyDefine"
			sechemaPropertyDefine.Name = DictionaryHelper.GetValue(dictionary, "name", string.Empty);
			sechemaPropertyDefine.DisplayName = DictionaryHelper.GetValue(dictionary, "displayName", string.Empty);
			sechemaPropertyDefine.Category = DictionaryHelper.GetValue(dictionary, "category", string.Empty);
			sechemaPropertyDefine.DefaultValue = DictionaryHelper.GetValue(dictionary, "defaultValue", string.Empty);
			sechemaPropertyDefine.DataType = DictionaryHelper.GetValue(dictionary, "dataType", PropertyDataType.String);
			sechemaPropertyDefine.Description = DictionaryHelper.GetValue(dictionary, "description", string.Empty);
			sechemaPropertyDefine.ReadOnly = DictionaryHelper.GetValue(dictionary, "readOnly", false);
			sechemaPropertyDefine.Visible = DictionaryHelper.GetValue(dictionary, "visible", true);
			sechemaPropertyDefine.EditorKey = DictionaryHelper.GetValue(dictionary, "editorKey", string.Empty);
			sechemaPropertyDefine.EditorParams = DictionaryHelper.GetValue(dictionary, "editorParams", string.Empty);
			sechemaPropertyDefine.SortOrder = DictionaryHelper.GetValue(dictionary, "sortOrder", 0xFFFF);
			
			if (dictionary.ContainsKey("validators") == true)
			{
				PropertyValidatorDescriptorCollection validators = JSONSerializerExecute.Deserialize<PropertyValidatorDescriptorCollection>(dictionary["validators"]);
				sechemaPropertyDefine.Validators.Clear();
				sechemaPropertyDefine.Validators.CopyFrom(validators);
			}
			#endregion

			sechemaPropertyDefine.Tab = DictionaryHelper.GetValue(dictionary, "tab",string.Empty);
			sechemaPropertyDefine.SnapshotMode = DictionaryHelper.GetValue(dictionary, "snapshotMode", default(SnapshotModeDefinition));
			sechemaPropertyDefine.SnapshotFieldName = DictionaryHelper.GetValue(dictionary, "snapshotFieldName", string.Empty);
			
			return sechemaPropertyDefine;
		}
		
		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			SchemaPropertyDefine sechemaPropertyDefine = (SchemaPropertyDefine)obj;

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "name", sechemaPropertyDefine.Name);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "displayName", sechemaPropertyDefine.DisplayName);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "category", sechemaPropertyDefine.Category);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "defaultValue", sechemaPropertyDefine.DefaultValue);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "dataType", sechemaPropertyDefine.DataType);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "description", sechemaPropertyDefine.Description);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "readOnly", sechemaPropertyDefine.ReadOnly);

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "visible", sechemaPropertyDefine.Visible);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "editorKey", sechemaPropertyDefine.EditorKey);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "editorParams", sechemaPropertyDefine.EditorParams);

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "validators", sechemaPropertyDefine.Validators);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "clientVdtData", sechemaPropertyDefine.GetPropertyValidator());

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "sortOrder", sechemaPropertyDefine.SortOrder);

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "tab", sechemaPropertyDefine.Tab);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "snapshotMode", sechemaPropertyDefine.SnapshotMode);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "snapshotFieldName", sechemaPropertyDefine.SnapshotFieldName);
			
			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new System.Type[] { typeof(SchemaPropertyDefine) }; }
		}
	}
}
