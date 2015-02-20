using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WrappedAUSchemaRoleConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			string schemaRoleID = DictionaryHelper.GetValue(dictionary, "SchemaRoleID", string.Empty);

			WrappedAUSchemaRole role = new WrappedAUSchemaRole(schemaRoleID);

			role.Name = DictionaryHelper.GetValue(dictionary, "Name", string.Empty);
			role.Description = DictionaryHelper.GetValue(dictionary, "Description", string.Empty);

			return role;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			WrappedAUSchemaRole role = (WrappedAUSchemaRole)obj;

			IDictionary<string, object> dictionary = new Dictionary<string, object>();

			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "SchemaRoleID", role.SchemaRoleID);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Name", role.Name);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Description", role.Description);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new System.Type[] { typeof(WrappedAUSchemaRole) };
			}
		}
	}
}

