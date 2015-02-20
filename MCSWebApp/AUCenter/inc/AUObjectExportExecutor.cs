using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects.Security;

namespace AUCenter
{
	public abstract class AUObjectExportExecutor
	{
		public virtual string CategoryName
		{
			get { return "对象集合"; }
		}

		public static AUObjectExportExecutor CreateAction(string cate)
		{
			switch (cate)
			{
				case "SchemaList":
					return new AUSchemaExportExecutor();
				case "SchemaRoleList":
					return new AUSchemaRoleExportExecutor();
				case "AdminUnitList":
					return new AdminUnitExportExecutor();

				default:
					throw new HttpException("无法识别的上下文参数");
			}
		}

		public abstract SCObjectSet Execute(HttpRequest req);
	}
}