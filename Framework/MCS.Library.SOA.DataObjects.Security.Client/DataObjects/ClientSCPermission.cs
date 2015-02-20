using System;

namespace MCS.Library.SOA.DataObjects.Security.Client
{
	[Serializable]
	public class ClientSCPermission : ClientSCBase
	{
		public ClientSCPermission()
			: base("Permissions")
		{
		}

		public ClientSCPermission(string schemaType)
			: base(schemaType)
		{
		}
	}
}
