using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;

namespace AUCenter
{
	public class AUSchemaRoleExportExecutor : AUObjectExportExecutor
	{
		public override string CategoryName
		{
			get
			{
				return "AUSchemaRoles";
			}
		}

		public override MCS.Library.SOA.DataObjects.Security.SCObjectSet Execute(HttpRequest req)
		{
			string categoryID = req.Form["auSchemaId"];

			string[] roleIds = req.Form.GetValues("roleIds"); //如果没有，则提取全部角色

			return AU.AUObjectExportor.ExportAUSchemaRoles(categoryID, roleIds);
		}
	}
}