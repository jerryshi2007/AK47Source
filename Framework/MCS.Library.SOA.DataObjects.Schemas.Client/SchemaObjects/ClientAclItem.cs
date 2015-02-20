using System;

namespace MCS.Library.SOA.DataObjects.Schemas.Client
{
	[Serializable]
	public class ClientAclItem : ClientObjectBase
	{
		public ClientAclItem()
		{
		}

		public ClientAclItem(string containerPermission, ClientSchemaObjectBase member)
		{
			// TODO: Complete member initialization
			this.ContainerPermission = containerPermission;
			this.MemberID  = member.ID ;
			this.MemberSchemaType = member.SchemaType;
		}
		public string ContainerID { get; set; }

		public string ContainerPermission { get; set; }

		public string ContainerSchemaType { get; set; }

		public string MemberID { get; set; }

		public string MemberSchemaType { get; set; }

		public int SortID { get; set; }
	}
}
