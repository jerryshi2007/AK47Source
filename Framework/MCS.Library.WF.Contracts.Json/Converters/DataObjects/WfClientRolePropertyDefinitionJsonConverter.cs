using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.WF.Contracts.Workflow.DataObjects;

namespace MCS.Library.WF.Contracts.Json.Converters.DataObjects
{
	public class WfClientRolePropertyDefinitionJsonConverter : JavaScriptConverter
	{
		private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientRolePropertyDefinition) };

		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfClientRolePropertyDefinition pv = new WfClientRolePropertyDefinition();

			//ColumnDefinitionBase
			pv.Caption = DictionaryHelper.GetValue(dictionary, "caption", string.Empty);
			pv.Name = DictionaryHelper.GetValue(dictionary, "name", string.Empty);
			pv.DataType = DictionaryHelper.GetValue(dictionary, "dataType", ColumnDataType.String);
			pv.DefaultValue = DictionaryHelper.GetValue(dictionary, "defaultValue", (string)null);

			//WfClientRolePropertyDefinition
			pv.SortOrder = DictionaryHelper.GetValue(dictionary, "sortOrder", 0);
			pv.Description = DictionaryHelper.GetValue(dictionary, "description", string.Empty);

			return pv;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();

			WfClientRolePropertyDefinition pd = (WfClientRolePropertyDefinition)obj;

			//ColumnDefinitionBase
			DictionaryHelper.AddNonDefaultValue(dictionary, "caption", pd.Caption);
			DictionaryHelper.AddNonDefaultValue(dictionary, "name", pd.Name);
			DictionaryHelper.AddNonDefaultValue(dictionary, "dataType", pd.DataType);
			DictionaryHelper.AddNonDefaultValue(dictionary, "defaultValue", pd.DefaultValue);

			//WfClientRolePropertyDefinition
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
