using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	public class AUSchemaRoleImportAction : IImportAction
	{
		public void DoImport(MCS.Library.SOA.DataObjects.Security.SCObjectSet objectSet, IImportContext context)
		{
			var exec = Operations.Facade.InstanceWithPermissions;

			if (string.IsNullOrEmpty(this.AUSchemaID))
				throw new InvalidOperationException("操作前必须对SchemaID进行赋值");

			var item = (AUSchema)Adapters.AUSnapshotAdapter.Instance.LoadAUSchema(this.AUSchemaID, true, DateTime.MinValue).FirstOrDefault();
			if (item == null)
				throw new AUObjectValidationException("不存在指定的管理架构，或已删除。");

			var roles = (from p in objectSet.Membership where p.ContainerID == item.ID && p.MemberSchemaType == AUCommon.SchemaAUSchemaRole join q in objectSet.Objects on p.ID equals q.ID where q.SchemaType == AUCommon.SchemaAUSchemaRole select q);
			int total = roles.Count();
			int roleCount = 0;
			foreach (AUSchemaRole role in roles)
			{
				roleCount++;
				try
				{
					context.SetStatusAndLog(roleCount, total, "正在添加角色：" + role.GetQualifiedName());
					exec.AddAdminSchemaRole(role, item);
				}
				catch (Exception ex)
				{
					context.AppendLog("对角色的操作失败，原因是：" + ex.Message);
				}
			}
		}

		public string AUSchemaID { get; set; }
	}
}
