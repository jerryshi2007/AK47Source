using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;

namespace AUCenter
{
	public class AUSchemaExportExecutor : AUObjectExportExecutor
	{
		public override string CategoryName
		{
			get
			{
				return "AUSchemas";
			}
		}

		public override MCS.Library.SOA.DataObjects.Security.SCObjectSet Execute(HttpRequest req)
		{
			string[] ids = req.Form.GetValues("id");

			string categoryID = req.Form["categoryId"];

			if (ids == null && categoryID == null)
				throw new HttpException("至少应指定id和categoryID其中之一。如果同时指定，则忽略categoryId");

			if (ids != null)
			{
				return AU.AUObjectExportor.ExportAUSchemas(ids);
			}
			else
			{
				return AU.AUObjectExportor.ExportAUSchemas(categoryID);
			}
		}
	}
}