using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Schemas.SchemaProperties
{
	public sealed class SchemaTabDefineConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			SchemaTabDefine schemaTabDefine = new SchemaTabDefine();

			schemaTabDefine.Name = DictionaryHelper.GetValue(dictionary, "name", string.Empty);
			schemaTabDefine.Description = DictionaryHelper.GetValue(dictionary, "description", string.Empty);

			return schemaTabDefine;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			SchemaTabDefine schemaTabDefine = (SchemaTabDefine)obj;

			IDictionary<string, object> dictionary = new Dictionary<string, object>();
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "name", schemaTabDefine.Name);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "description", schemaTabDefine.Description);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new System.Type[] { typeof(SchemaTabDefine) }; }
		}
	}
}
