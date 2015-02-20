using System;
using MCS.Library.SOA.DataObjects.Schemas.Client;

namespace MCS.Library.SOA.DataObjects.Schemas.Client
{
	[Serializable]
	public class ClientConditionItem : ClientObjectBase
	{
		public string Condition { get; set; }

		public string Description { get; set; }

		public string OwnerID { get; set; }

		public int SortID { get; set; }

		public string Type { get; set; }
	}
}
