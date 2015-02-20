using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Web.Script.Serialization;

namespace MCS.Library.SOA.DataObjects.Security.AUClient
{
	public class ClientAUAdminScope : ClientGenericObject
	{
		public ClientAUAdminScope()
			: base("AUAdminScopes")
		{
		}

		public ClientAUAdminScope(string schemaType)
			: base(schemaType)
		{
		}

		[XmlIgnore]
		[ScriptIgnore]
		public string ScopeSchemaType
		{
			get { return this.Properties.GetValue("ScopeSchemaType", null); }
			set { this.Properties.AddOrSetValue("ScopeSchemaType", Schemas.Client.ClientPropertyDataType.String, value); }
		}
	}
}
