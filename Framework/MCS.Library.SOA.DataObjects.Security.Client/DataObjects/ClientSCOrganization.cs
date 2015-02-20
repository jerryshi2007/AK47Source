using System;

namespace MCS.Library.SOA.DataObjects.Security.Client
{
	[Serializable]
	public class ClientSCOrganization : ClientSCBase
	{
		public ClientSCOrganization()
			: base("Organizations")
		{
		}

		public ClientSCOrganization(string schemaType)
			: base(schemaType)
		{
		}
	}
}
