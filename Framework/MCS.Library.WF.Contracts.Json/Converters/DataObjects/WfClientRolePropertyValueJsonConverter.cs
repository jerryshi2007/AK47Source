using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.WF.Contracts.Workflow.DataObjects;

namespace MCS.Library.WF.Contracts.Json.Converters.DataObjects
{
	public class WfClientRolePropertyValueJsonConverter : JavaScriptConverter
	{
		private static readonly Type[] _SupportedTypes = new Type[] { typeof(WfClientRolePropertyValue) };

		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			string columnName = dictionary.GetValue("columnName", string.Empty);

			WfClientRolePropertyValue pv = new WfClientRolePropertyValue(new WfClientRolePropertyDefinition() { Name = columnName });

			pv.Value = dictionary.GetValue("value", string.Empty);

			return pv;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();

			WfClientRolePropertyValue pv = (WfClientRolePropertyValue)obj;

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
