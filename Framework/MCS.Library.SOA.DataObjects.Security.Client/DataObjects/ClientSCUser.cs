using System;

namespace MCS.Library.SOA.DataObjects.Security.Client
{
	[Serializable]
	public class ClientSCUser : ClientSCBase
	{
		public ClientSCUser()
			: base("Users")
		{
		}

		public ClientSCUser(string schemaType)
			: base(schemaType)
		{
		}
	}
}
