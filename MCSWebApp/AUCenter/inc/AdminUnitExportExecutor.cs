using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;

namespace AUCenter
{
	public class AdminUnitExportExecutor : AUObjectExportExecutor
	{
		public override string CategoryName
		{
			get
			{
				return "AdminUnits";
			}
		}

		public override MCS.Library.SOA.DataObjects.Security.SCObjectSet Execute(HttpRequest req)
		{
			var ids = req.Form.GetValues("ids");
			var auSchemaID = req.Form["auSchemaId"];
			if (string.IsNullOrEmpty(auSchemaID))
				throw new HttpException("必须提供auSchemaId");
			bool deep = req.Form["deep"] == "true";

			return AU.AUObjectExportor.ExportAdminUnits(auSchemaID, ids, deep);
		}
	}
}