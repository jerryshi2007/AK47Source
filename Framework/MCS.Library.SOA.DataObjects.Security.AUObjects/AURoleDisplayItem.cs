using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	[Serializable]
	public class AURoleDisplayItem
	{
		public string ID { get; set; }

		public string SchemaRoleID { get; set; }

		public string CodeName { get; set; }

		public string Name { get; set; }

		public string DisplayName { get; set; }

		public List<IUser> Users { get; set; }
	}
}
