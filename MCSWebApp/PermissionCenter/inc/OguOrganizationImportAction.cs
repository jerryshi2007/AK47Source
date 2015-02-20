using System;
using System.Collections.Generic;
using MCS.Library.SOA.DataObjects.Security;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace PermissionCenter
{
	public class OguOrganizationImportAction : OguImportAction
	{
		public OguOrganizationImportAction(PC.SCOrganization parent)
			: base(parent)
		{
		}

		public override void DoImport(SCObjectSet objectSet, IImportContext context)
		{
			if (objectSet.HasRelations)
			{
				int allCount = objectSet.Objects.Count;
				int count = 0;

				var pendingOrgs = new List<SchemaObjectBase>(objectSet.Objects.Count);

				foreach (SCOrganization item in ImportService.Instance.FilterNormalObjectsBySchemaCategories(objectSet.Objects, "Organizations"))
				{
					// 进行过滤，保留当前组织中的组织
					if (objectSet.Relations.Exists(r => r.ParentID == this.Parent.ID && r.Status == SchemaObjectStatus.Normal && r.ID == item.ID))
					{
						pendingOrgs.Add(item);
					}
				}

				allCount = pendingOrgs.Count;
				if (allCount > 0)
				{
					context.SetStatus(0, 1, "正在寻找当前组织内的关系。");

					foreach (SCOrganization item in pendingOrgs)
					{
						count++;

						try
						{
							var summaryName = item.ToDescription();
							context.SetStatus(count, allCount, "正在导入对象:" + summaryName);

							PC.Executors.SCObjectOperations.InstanceWithPermissions.AddOrganization((PC.SCOrganization)item, this.Parent);

							context.AppendLog("已执行导入项目" + summaryName);
						}
						catch (Exception ex)
						{
							context.AppendLog("对项的操作失败，原因是：" + ex.Message);
						}
					}
				}
				else
				{
					context.SetStatus(0, 1, "没有找到符合条件的组织关系。");
				}
			}
		}
	}
}