using System;

namespace MCS.Library.SOA.DataObjects.Security.Client
{
	[Serializable]
	public class ClientSCGroup : ClientSCBase
	{
		public ClientSCGroup()
			: base("Groups")
		{
		}

		public ClientSCGroup(string schemaType)
			: base(schemaType)
		{
		}
	}
}
