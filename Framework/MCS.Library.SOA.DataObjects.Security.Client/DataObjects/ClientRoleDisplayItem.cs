using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MCS.Library.SOA.DataObjects.Security.Client
{
	[Serializable]
	public class ClientRoleDisplayItem
	{
		public ClientRoleDisplayItem() { }

		[XmlAttribute]
		public string ApplicationDisplayName { get; set; }
		[XmlAttribute]
		public string ApplicationID { get; set; }
		[XmlAttribute]
		public string ApplicationName { get; set; }
		[XmlAttribute]
		public string RoleCodeName { get; set; }
		[XmlAttribute]
		public string RoleDisplayName { get; set; }
		[XmlAttribute]
		public string RoleID { get; set; }
		[XmlAttribute]
		public string RoleName { get; set; }
	}
}
