using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Web.Script.Serialization;

namespace MCS.Library.SOA.DataObjects.Security.AUClient
{
	public class ClientAdminUnit : ClientNamedObject
	{
		public ClientAdminUnit()
			: base("AdminUnits")
		{
		}

		public ClientAdminUnit(string schemaType)
			: base(schemaType)
		{
		}

		[XmlIgnore]
		[ScriptIgnore]
		public string AUSchemaID
		{
			get { return this.Properties["AUSchemaID"].StringValue; }
			set { this.Properties.AddOrSetValue("AUSchemaID", Schemas.Client.ClientPropertyDataType.String, value); }
		}
	}
}
