using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.Builder;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace PermissionCenter
{
	public class RoleExportAction : ExportAction
	{
		public override string CategoryName
		{
			get
			{
				return "角色";
			}
		}

		public override SCObjectSet Execute(HttpRequest req)
		{
			string[] ids = req.Form.GetValues("id");

			if (ids == null && ids.Length == 0)
				throw new HttpException("当获取角色对象时，必须提供ID参数");

			string appId = req.Form["appId"];

			if (string.IsNullOrEmpty(appId))
				throw new HttpException("当获取角色对象时，必须提供appId参数");

			SCObjectSet objectSet = new SCObjectSet();
			objectSet.Scope = "AppRoles";

			PC.SCApplication appObj = (PC.SCApplication)PC.Adapters.SchemaObjectAdapter.Instance.Load(appId);
			if (appObj == null || appObj.Status != SchemaObjectStatus.Normal)
				throw new HttpException("指定的应用不存在或者已删除");

			objectSet.Objects = ExportQueryHelper.LoadObjects(ids, null);

			objectSet.Membership = ExportQueryHelper.LoadFullMemberships(ids); // 应用与角色的关系 & 应用的成员关系

			objectSet.Conditions = ExportQueryHelper.LoadConditions(ids); // 保存对象的条件

			objectSet.Relations = ExportQueryHelper.LoadFullRelations(ids); // 与权限的绑定关系

			return objectSet;
		}
	}
}