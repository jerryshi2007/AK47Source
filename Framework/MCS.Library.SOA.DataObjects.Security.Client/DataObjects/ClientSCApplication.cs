using System;

namespace MCS.Library.SOA.DataObjects.Security.Client
{
	[Serializable]
	public class ClientSCApplication : ClientSCBase
	{
		public ClientSCApplication()
			: base("Applications")
		{
		}

		public ClientSCApplication(string schemaType)
			: base(schemaType)
		{
		}
	}
}
