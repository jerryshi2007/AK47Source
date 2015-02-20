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
	public class AppExportAction : ExportAction
	{
		public override string CategoryName
		{
			get
			{
				return "应用";
			}
		}

		public override SCObjectSet Execute(HttpRequest req)
		{
			string[] ids = req.Form.GetValues("id");

			if (ids == null || ids.Length < 0)
				throw new HttpException("当获取应用数据时，必须提供ID参数");

			SCObjectSet objectSet = new SCObjectSet();
			objectSet.Scope = "AllApps";

			objectSet.Objects = ExportQueryHelper.LoadObjects(ids, "Applications");

			// 查找应用下的角色和功能
			objectSet.Membership = ExportQueryHelper.LoadMembershipOf(ids, "Applications");

			var memberObjects = ExportQueryHelper.LoadObjects((from obj in objectSet.Membership select obj.ID).ToArray(), null);

			objectSet.Acls = ExportQueryHelper.LoadAclsFor(ids); // ACL

			foreach (var obj in memberObjects)
			{
				if (objectSet.Objects.ContainsKey(obj.ID) == false)
					objectSet.Objects.Add(obj); // 成员对象(角色和功能)
			}

			var roleIds = (from m in memberObjects where m is PC.SCRole select m.ID).ToArray();

			if (roleIds.Length > 0)
			{
				var roleMembers = ExportQueryHelper.LoadMembershipOf(roleIds);

				objectSet.Membership.CopyFrom(roleMembers); // 角色→固定成员

				objectSet.Conditions = ExportQueryHelper.LoadConditions(roleIds); // 角色→条件

				objectSet.Relations = ExportQueryHelper.LoadFullRelations(roleIds);
			}

			return objectSet;
		}
	}
}