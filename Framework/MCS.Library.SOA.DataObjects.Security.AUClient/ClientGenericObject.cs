using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Schemas.Client;

namespace MCS.Library.SOA.DataObjects.Security.AUClient
{
	public class ClientGenericObject : ClientSchemaObjectBase
	{
		public ClientGenericObject()
		{
		}

		public ClientGenericObject(string schemaType)
			: base(schemaType)
		{
		}
	}
}
