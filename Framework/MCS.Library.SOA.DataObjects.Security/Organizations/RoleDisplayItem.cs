using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Security
{
	[Serializable]
	public class RoleDisplayItem
	{
		[ORFieldMapping("ID")]
		public string RoleID { get; set; }

		[ORFieldMapping("Name")]
		public string RoleName { get; set; }

		[ORFieldMapping("DisplayName")]
		public string RoleDisplayName { get; set; }

		[ORFieldMapping("CodeName")]
		public string RoleCodeName { get; set; }

		[ORFieldMapping("AppID")]
		public string ApplicationID { get; set; }

		[ORFieldMapping("AppName")]
		public string ApplicationName { get; set; }

		[ORFieldMapping("AppDisplayName")]
		public string ApplicationDisplayName { get; set; }
	}

	[Serializable]
	public class RoleDisplayItemCollection : List<RoleDisplayItem>
	{
		public RoleDisplayItemCollection()
		{
		}

		public RoleDisplayItemCollection(IEnumerable<RoleDisplayItem> roles)
		{
			this.AddRange(roles);
		}
	}
}
