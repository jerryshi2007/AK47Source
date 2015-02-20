using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.Builder;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace PermissionCenter
{
	public class AllGroupsExportAction : ExportAction
	{
		public override string CategoryName
		{
			get
			{
				return "所有群组";
			}
		}

		public override SCObjectSet Execute(HttpRequest req)
		{
			string[] ids = req.Form.GetValues("id");

			if (ids == null || ids.Length < 0)
				throw new HttpException("当获取群组数据时，必须提供ID参数");

			SCObjectSet objectSet = new SCObjectSet();
			objectSet.Scope = "AllGroups";

			objectSet.Objects = ExportQueryHelper.LoadObjects(ids, "Groups");

			objectSet.Relations = ExportQueryHelper.LoadFullRelations(ids);

			objectSet.Membership = ExportQueryHelper.LoadFullMemberships(ids);

			objectSet.Conditions = ExportQueryHelper.LoadConditions(ids);

			return objectSet;
		}
	}
}