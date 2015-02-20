using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;

namespace MCS.Library.SOA.DataObjects.Security
{
	public class SchemaObjectXmlImporter
	{
		private ObjectSchemaSettings config = null;

		public SchemaObjectBase XmlToObject(string xml, string schemaType)
		{
			if (this.config == null)
				this.config = ObjectSchemaSettings.GetConfig();

			var obj = (SchemaObjectBase)this.config.Schemas[schemaType].CreateInstance(schemaType);
			obj.FromString(xml);

			return obj;
		}

		public T XmlToObject<T>(string xml, string schemaType) where T : SchemaObjectBase
		{
			return (T)this.XmlToObject(xml, schemaType);
		}
	}
}