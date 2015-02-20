using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	public class AUSchemaImportAction : IImportAction
	{
		public void DoImport(MCS.Library.SOA.DataObjects.Security.SCObjectSet objectSet, IImportContext context)
		{
			var exec = this.IgnorePermission ? Operations.Facade.DefaultInstance : Operations.Facade.InstanceWithPermissions;

			IEnumerable<SchemaObjectBase> schemas;
			if (string.IsNullOrEmpty(TargetCategory))
				schemas = objectSet.Objects.Where(m => m.SchemaType == AUCommon.SchemaAUSchema && m.Status == Schemas.SchemaProperties.SchemaObjectStatus.Normal);
			else
				schemas = objectSet.Objects.Where(m => m.SchemaType == AUCommon.SchemaAUSchema && m.Status == Schemas.SchemaProperties.SchemaObjectStatus.Normal && ((AUSchema)m).CategoryID == TargetCategory);

			int allCount = schemas.Count();
			int count = 0;

			foreach (AUSchema item in schemas)
			{
				count++;

				try
				{
					context.SetStatusAndLog(count, allCount, "正在导入对象：" + item.GetQualifiedName());
					exec.AddAdminSchema(item);

					ImportSchemaRoles(objectSet, context, exec, item);
				}
				catch (Exception ex)
				{
					context.ErrorCount++;
					context.AppendLog("对项的操作失败，原因是：" + ex.Message);
				}
			}
		}

		private void ImportSchemaRoles(MCS.Library.SOA.DataObjects.Security.SCObjectSet objectSet, IImportContext context, Operations.IFacade exec, AUSchema item)
		{
			if (this.IncludeSchemaRoles)
			{
				var roles = (from p in objectSet.Membership where p.ContainerID == item.ID && p.MemberSchemaType == AUCommon.SchemaAUSchemaRole join q in objectSet.Objects on p.ID equals q.ID select q);
				int total = roles.Count();
				int roleCount = 0;
				foreach (AUSchemaRole role in roles)
				{
					roleCount++;
					try
					{
						context.SetSubStatusAndLog(roleCount, total, "正在添加角色：" + role.GetQualifiedName());
						exec.AddAdminSchemaRole(role, item);
					}
					catch (Exception ex)
					{
						context.ErrorCount++;
						context.AppendLog("对角色的操作失败，原因是：" + ex.Message);
					}
				}
			}
		}

		public bool IncludeSchemaRoles { get; set; }

		/// <summary>
		/// 不为null时，过滤categoryID
		/// </summary>
		public string TargetCategory { get; set; }

		/// <summary>
		/// 是否忽略权限检查
		/// </summary>
		public bool IgnorePermission { get; set; }
	}
}
