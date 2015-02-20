using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.AUClient
{
	public class ClientAUSchemaRole : ClientNamedObject
	{
		public ClientAUSchemaRole()
			: base("AUSchemaRoles")
		{
		}

		public ClientAUSchemaRole(string schemaType)
			: base(schemaType)
		{
		}
	}
}
