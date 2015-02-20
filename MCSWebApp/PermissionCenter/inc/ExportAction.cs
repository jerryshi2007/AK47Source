using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.Builder;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter
{
	public abstract class ExportAction
	{
		public virtual string CategoryName
		{
			get { return "对象集合"; }
		}

		public static ExportAction CreateAction(string cate)
		{
			switch (cate)
			{
				case "AllUsers":
					return new AllUserExportAction();
				case "AllGroups":
					return new AllGroupsExportAction();
				case "OguObjects":
					return new OguExportActions();
				case "OguObjectsFull":
					return new OguFullExportAction();
				case "AppPermissions":
					return new PermissionExportAction();
				case "AppRoles":
					return new RoleExportAction();
				case "AllApps":
					return new AppExportAction();
				case "AppRoleMembers":
					return new RoleConstMembersExportAction();
				case "GroupConstMembers":
					return new GroupConstMembersExportAction();
				default:
					throw new HttpException("无法识别的上下文参数");
			}
		}

		public abstract SCObjectSet Execute(HttpRequest req);

		protected PC.SCRelationObjectCollection ToNormalRelation(PC.SCParentsRelationObjectCollection src)
		{
			PC.SCRelationObjectCollection result = null;
			if (src != null)
			{
				result = new PC.SCRelationObjectCollection();
				result.CopyFrom(src);
			}

			return result;
		}

		protected PC.SCRelationObjectCollection FilterChildren(PC.SCChildrenRelationObjectCollection src, string[] ids)
		{
			PC.SCRelationObjectCollection result = null;
			if (src != null)
			{
				result = new PC.SCRelationObjectCollection();
				foreach (string key in ids)
				{
					result.Add(src[key]);
				}
			}

			return result;
		}
	}
}