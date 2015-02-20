using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Schemas.Client
{
	[Serializable]
	public class ClientSchemaMember : ClientSchemaObjectBase
	{
		public string MemberSchemaType { get; set; }

		public string ContainerSchemaType { get; set; }

		public string ContainerID { get; set; }

		public int InnerSort { get; set; }
	}
}
