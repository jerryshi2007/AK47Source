using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace PermissionCenter
{
	public class GroupConstMembersExportAction : ExportAction
	{
		public override string CategoryName
		{
			get
			{
				return "群组固定人员";
			}
		}

		public override SCObjectSet Execute(HttpRequest req)
		{
			string[] ids = req.Form.GetValues("memberIds");

			string groupId = req.Form["groupId"];

			if (ids == null || ids.Length < 0)
				throw new HttpException("当获取角色固定成员数据时，必须提供memberIds参数");

			if (string.IsNullOrEmpty(groupId))
				throw new HttpException("当获取角色固定成员数据时，必须提供group");

			SCObjectSet objectSet = new SCObjectSet();
			objectSet.Scope = "GroupConstMembers";

			objectSet.Membership = ExportQueryHelper.LoadMembershipFor(ids, groupId);

			return objectSet;
		}
	}
}