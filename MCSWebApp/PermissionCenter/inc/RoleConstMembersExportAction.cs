using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace PermissionCenter
{
	public class RoleConstMembersExportAction : ExportAction
	{
		public override string CategoryName
		{
			get
			{
				return "角色固定成员";
			}
		}

		public override SCObjectSet Execute(HttpRequest req)
		{
			string[] ids = req.Form.GetValues("memberIds");

			string roleId = req.Form["roleId"];

			if (ids == null || ids.Length < 0)
				throw new HttpException("当获取角色固定成员数据时，必须提供memberIds参数");

			if (string.IsNullOrEmpty(roleId))
				throw new HttpException("当获取角色固定成员数据时，必须提供roleID");

			SCObjectSet objectSet = new SCObjectSet();
			objectSet.Scope = "RoleConstMembers";

			objectSet.Membership = ExportQueryHelper.LoadMembershipFor(ids, roleId);

			return objectSet;
		}
	}
}