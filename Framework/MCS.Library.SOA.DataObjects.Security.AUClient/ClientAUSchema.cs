using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Web.Script.Serialization;

namespace MCS.Library.SOA.DataObjects.Security.AUClient
{
	public class ClientAUSchema : ClientNamedObject
	{
		public ClientAUSchema()
			: base("AUSchemas")
		{
		}

		public ClientAUSchema(string schemaType)
			: base(schemaType)
		{
		}

		[XmlIgnore]
		[ScriptIgnore]
		public string CategoryID
		{
			get { return this.Properties.GetValue("CategoryID", null); }
			set { this.Properties.AddOrSetValue("CategoryID", Schemas.Client.ClientPropertyDataType.String, value); }
		}

		[XmlIgnore]
		[ScriptIgnore]
		public string Scopes
		{
			get { return this.Properties.GetValue("Scopes", string.Empty); }
			set { this.Properties.AddOrSetValue("Scopes", Schemas.Client.ClientPropertyDataType.String, value); }
		}

		[XmlIgnore]
		[ScriptIgnore]
		public string MasterRole
		{
			get { return this.Properties.GetValue("MasterRole", string.Empty); }
			set { this.Properties.AddOrSetValue("MasterRole", Schemas.Client.ClientPropertyDataType.String, value); }
		}
	}
}
