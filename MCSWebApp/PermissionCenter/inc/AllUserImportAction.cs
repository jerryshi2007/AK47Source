using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects.Security;
using PC = MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter
{
	/// <summary>
	/// 仅导入人员的操作
	/// </summary>
	public class AllUserImportAction : ImportAction
	{
		/// <summary>
		/// 获取或设置一个值，表示是否同时导入组织关系
		/// </summary>
		public bool IncludeOrganizationRelation { get; set; }

		/// <summary>
		/// 获取或设置一个值，表示是否同时导入秘书
		/// </summary>
		public bool IncludeSecretaries { get; set; }

		/// <summary>
		/// 获取或设置一个值，表示是否同时导入群组固定成员
		/// </summary>
		public bool IncludeGroupConstMembers { get; set; }

		public override void DoImport(SCObjectSet objectSet, IImportContext context)
		{
			context.AppendLog("正在查找人员");

			var actor = PC.Executors.SCObjectOperations.InstanceWithPermissions;

			var pendingUsers = ImportService.Instance.FilterNormalObjectsBySchemaCategories(objectSet.Objects, "Users").ToList();

			int allCount = pendingUsers.Count;

			ImportService.Instance.ImportUsers(pendingUsers, objectSet, context, objectSet.HasRelations && this.IncludeOrganizationRelation);

			if (this.IncludeSecretaries && objectSet.HasMembership)
			{
				ImportService.Instance.ImportSecretaries(pendingUsers, objectSet, context);
				ImportService.Instance.ImportBosses(pendingUsers, objectSet, context);
			}

			if (this.IncludeGroupConstMembers && objectSet.HasMembership)
			{
				ImportService.Instance.AddUsersToGroups(pendingUsers, objectSet, context);
			}
		}
	}
}