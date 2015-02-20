using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Schemas.SchemaProperties
{
	public abstract class VersionedSchemaObjectBaseConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			string schemaType = DictionaryHelper.GetValue(dictionary, "schemaType", string.Empty);

			VersionedSchemaObjectBase schemaObjectBase = CreateInstance(schemaType, dictionary, type, serializer);

			schemaObjectBase.ID = DictionaryHelper.GetValue(dictionary, "id", string.Empty); ;
			schemaObjectBase.Status = DictionaryHelper.GetValue(dictionary, "status", SchemaObjectStatus.Normal);
			schemaObjectBase.VersionStartTime = DictionaryHelper.GetValue(dictionary, "versionStartTime", DateTime.MinValue);
			schemaObjectBase.VersionEndTime = DictionaryHelper.GetValue(dictionary, "versionEndTime", DateTime.MinValue);

			if (dictionary.ContainsKey("Properties") == true)
			{
				SchemaPropertyValueCollection propCollection = JSONSerializerExecute.Deserialize<SchemaPropertyValueCollection>(dictionary["Properties"]);
				schemaObjectBase.Properties.Clear();
				schemaObjectBase.Properties.CopyFrom(propCollection);
			}

			return schemaObjectBase;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			throw new NotImplementedException();
		}

		protected abstract VersionedSchemaObjectBase CreateInstance(string schemaType, IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer);
	}
}
