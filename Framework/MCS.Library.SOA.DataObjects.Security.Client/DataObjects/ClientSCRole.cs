using System;

namespace MCS.Library.SOA.DataObjects.Security.Client
{
	[Serializable]
	public class ClientSCRole : ClientSCBase
	{
		public ClientSCRole()
			: base("Roles")
		{
		}

		public ClientSCRole(string schemaType)
			: base(schemaType)
		{
		}
	}
}
