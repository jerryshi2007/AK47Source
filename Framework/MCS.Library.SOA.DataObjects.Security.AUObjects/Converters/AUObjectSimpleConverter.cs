using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Converters
{
	public class AUObjectSimpleConverter : JavaScriptConverter
	{

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();

			SchemaObjectBase data = (SchemaObjectBase)obj;

			dictionary.AddNonDefaultValue("id", data.ID);
			dictionary.AddNonDefaultValue("name", data.Properties.GetValue("Name", string.Empty));
			dictionary.AddNonDefaultValue("codeName", data.Properties.GetValue("CodeName", string.Empty));
			dictionary.AddNonDefaultValue("displayName", data.Properties.GetValue("DisplayName", string.Empty));
			dictionary.AddNonDefaultValue("description", data.Properties.GetValue("Description", string.Empty));
			dictionary.AddNonDefaultValue("schemaType", data.SchemaType);

			return dictionary;
		}

		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			string schemaType = dictionary.GetValue("schemaType", string.Empty);

			SchemaObjectBase data = SchemaExtensions.CreateObject(schemaType);

			data.ID = dictionary.GetValue("id", string.Empty);
			data.Properties.TrySetValue("Name", dictionary.GetValue("name", string.Empty));
			data.Properties.TrySetValue("CodeName", dictionary.GetValue("codeName", string.Empty));
			data.Properties.TrySetValue("DisplayName", dictionary.GetValue("displayName", string.Empty));
			data.Properties.TrySetValue("Description", dictionary.GetValue("description", string.Empty));

			return data;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new Type[] { 
					typeof(SchemaObjectBase),
					typeof(AUAdminScope),
					typeof(AUAdminScopeItem),
					typeof(AdminUnit),
					typeof(AUSchemaCategory),
					typeof(AURole) ,
					typeof(AUSchemaRole),
				};
			}
		}
	}
}
