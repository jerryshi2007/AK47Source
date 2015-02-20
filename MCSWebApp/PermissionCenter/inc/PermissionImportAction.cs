using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter
{
	public class PermissionImportAction : ImportAction
	{
		public PermissionImportAction(string appId)
		{
			this.ApplicationId = appId;
		}

		public string ApplicationId { get; set; }

		public bool CopyMode { get; set; }

		public override void DoImport(SCObjectSet objectSet, IImportContext context)
		{
			if (string.IsNullOrEmpty(this.ApplicationId))
				throw new HttpException("没有指定ApplicationId的情况下无法导入。");

			var app = (PC.SCApplication)PC.Adapters.SchemaObjectAdapter.Instance.Load(this.ApplicationId);
			if (app == null)
				throw new HttpException("指定的应用并不存在");

			if (objectSet.HasObjects)
			{
				int count = 0;
				int allCount = 0;
				var exec = PC.Executors.SCObjectOperations.InstanceWithPermissions;

				IEnumerable<PC.SCSimpleRelationBase> permissionRelations = null;

				if (this.CopyMode)
				{
					permissionRelations = from r in objectSet.Membership where r.MemberSchemaType == "Permissions" orderby r.InnerSort ascending select r;
				}
				else
				{
					permissionRelations = from r in objectSet.Membership where r.MemberSchemaType == "Permissions" && r.ContainerID == app.ID orderby r.InnerSort ascending select r;
				}

				var permissions = (from o in objectSet.Objects join p in permissionRelations on o.ID equals p.ID select (PC.SCPermission)o).ToArray();

				allCount = permissions.Length;

				foreach (var p in permissions)
				{
					count++;
					PC.SCPermission fun = this.CopyMode ? AppImportAction.MakeCopy(p) : p;

					// 只导入明确的
					context.SetStatus(count, allCount, "正在导入权限:" + fun.DisplayName);
					context.AppendLogFormat("正在导入权限 {0} \r\n", fun.DisplayName ?? fun.Name);
					exec.AddPermission(fun, app);
				}
			}
		}
	}
}