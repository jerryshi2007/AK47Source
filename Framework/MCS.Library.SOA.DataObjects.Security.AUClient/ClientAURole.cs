using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Web.Script.Serialization;

namespace MCS.Library.SOA.DataObjects.Security.AUClient
{
	public class ClientAURole : ClientGenericObject
	{
		public ClientAURole()
			: base("AURoles")
		{
		}

		public ClientAURole(string schemaType)
			: base(schemaType)
		{
		}

		[XmlIgnore]
		[ScriptIgnore]
		public string SchemaRoleID
		{
			get { return this.Properties.GetValue("SchemaRoleID", null); }
			set { this.Properties.AddOrSetValue("SchemaRoleID", Schemas.Client.ClientPropertyDataType.String, value); }
		}
	}
}
