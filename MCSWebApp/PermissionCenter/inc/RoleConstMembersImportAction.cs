using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter
{
	public class RoleConstMembersImportAction : ImportAction
	{
		private string roleID;

		public RoleConstMembersImportAction(string roleID)
		{
			if (string.IsNullOrEmpty(roleID))
				throw new ArgumentNullException("roleId");
			this.roleID = roleID;
		}

		public override void DoImport(SCObjectSet objectSet, IImportContext context)
		{
			if (objectSet.HasMembership)
			{
				try
				{
					var actor = PC.Executors.SCObjectOperations.InstanceWithPermissions;
					ImportService.Instance.WithEffectObject<PC.SCRole>(this.roleID, role =>
					{
						var pendingMembership = ImportService.Instance.FilterMembership(objectSet.Membership, m => m.ContainerID == this.roleID).ToList();

						int allCount = pendingMembership.Count;
						context.SetStatus(0, allCount, "正在寻找当前角色的固定成员。");

						int count = 0;
						foreach (var r in pendingMembership)
						{
							count++;
							try
							{
								ImportService.Instance.WithEffectObject<PC.SCBase>(r.ID, o =>
								{
									string msg = "正在导入成员： " + o.ToDescription();
									context.SetStatus(count, allCount, msg);
									context.AppendLog(msg);
									actor.AddMemberToRole(o, role);
								}, () =>
								{
									string msg = string.Format("跳过了不存在的对象 {0} \r\n", r.ID);
									context.AppendLog(msg);
									context.SetStatus(count, allCount, msg);
								});
							}
							catch (Exception ex)
							{
								context.AppendLog("导入角色成员时出错：" + ex.Message);
							}
						}
					}, () =>
					{
						context.AppendLog("指定的角色无效，导入终止。");
					});
				}
				catch (Exception ex)
				{
					context.AppendLog("导入遇到错误，已经终止：" + ex.Message.ToString() + Environment.NewLine);
				}
			}
		}
	}
}