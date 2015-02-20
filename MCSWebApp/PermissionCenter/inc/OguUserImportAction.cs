using System.Collections.Generic;
using MCS.Library.SOA.DataObjects.Security;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace PermissionCenter
{
	public class OguUserImportAction : OguImportAction
	{
		public OguUserImportAction(PC.SCOrganization parent)
			: base(parent)
		{
		}

		public bool IncludeSecretaries { get; set; }

		public override void DoImport(SCObjectSet objectSet, IImportContext context)
		{
			var pendingUsers = new List<SchemaObjectBase>(objectSet.Objects.Count);

			foreach (SCUser item in ImportService.Instance.FilterNormalObjectsBySchemaCategories(objectSet.Objects, "Users"))
			{
				// 进行过滤，保留当前组织中的人员
				if (objectSet.Relations.Exists(r => r.ParentID == this.Parent.ID && r.Status == SchemaObjectStatus.Normal && r.ID == item.ID))
				{
					pendingUsers.Add(item);
				}
			}

			// 这些人属于组织
			ImportService.Instance.ImportUsers(pendingUsers, objectSet, context, true);

			if (this.IncludeSecretaries && objectSet.HasMembership)
			{
				ImportService.Instance.ImportSecretaries(pendingUsers, objectSet, context);
				ImportService.Instance.ImportBosses(pendingUsers, objectSet, context);
			}
		}
	}
}