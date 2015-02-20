using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.AUClient
{
	/// <summary>
	/// 表示客户端的管理单元角色
	/// </summary>
	public class ClientAURoleDisplayItem
	{
		public string SchemaRoleID { get; set; }

		public string SchemaRoleName { get; set; }

		public string AURoleID { get; set; }
	}
}
