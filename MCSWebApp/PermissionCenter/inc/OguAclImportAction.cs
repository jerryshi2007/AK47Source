using System;
using System.Collections.Generic;
using MCS.Library.SOA.DataObjects.Security;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace PermissionCenter
{
	public class OguAclImportAction : OguImportAction
	{
		public OguAclImportAction(PC.SCOrganization parent)
			: base(parent)
		{
		}

		public override void DoImport(SCObjectSet objectSet, IImportContext context)
		{
			if (objectSet.HasAcls)
			{
				context.SetStatus(0, 1, "正在寻找当前组织内组织的ACL。");

				var pendingOrgs = new List<SCOrganization>(objectSet.Objects.Count);

				foreach (SCOrganization item in ImportService.Instance.FilterNormalObjects<SCOrganization>(objectSet.Objects))
				{
					// 进行过滤，保留当前组织中的组织
					if (objectSet.Relations.Exists(r => r.ParentID == this.Parent.ID && r.Status == SchemaObjectStatus.Normal && r.ID == item.ID))
					{
						pendingOrgs.Add(item);
					}
				}

				int allCount = pendingOrgs.Count;
				int count = 0;

				foreach (SCOrganization item in pendingOrgs)
				{
					var summaryName = item.ToDescription();

					count++;

					var pendingAcls = ImportService.Instance.FilterAcls(objectSet.Acls, acl => acl.ContainerID == item.ID && acl.Status == SchemaObjectStatus.Normal);

					try
					{
						var newContainer = new PC.Permissions.SCAclContainer(item);

						PC.Permissions.SCAclMemberCollection members = new PC.Permissions.SCAclMemberCollection();

						foreach (var acl in pendingAcls)
						{
							ImportService.Instance.WithEffectObject<SchemaObjectBase>(acl.MemberID, role =>
							{
								members.Add(acl.ContainerPermission, role);
							}, null);
						}

						var oldMembers = PC.Adapters.SCAclAdapter.Instance.LoadByContainerID(item.ID, DateTime.MinValue);

						if (oldMembers != null)
						{
							members.MergeChangedItems(oldMembers);
						}

						newContainer.Members.CopyFrom(members);

						context.SetStatus(count, allCount, "正在替换对象ACL:" + summaryName);

						PC.Adapters.SCAclAdapter.Instance.Update(newContainer);
					}
					catch (Exception ex)
					{
						context.AppendLogFormat("对项 {0} 的ACL操作失败，原因是：{1}\r\n", summaryName, ex.Message);
					}
				}
			}
		}
	}
}