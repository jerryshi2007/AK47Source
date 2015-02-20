using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter
{
	public class OguGroupImportAction : OguImportAction
	{
		public OguGroupImportAction(PC.SCOrganization parent)
			: base(parent)
		{
		}

		public bool IncludeMembers { get; set; }

		public bool IncludeConditions { get; set; }

		public override void DoImport(SCObjectSet objectSet, IImportContext context)
		{
			if (objectSet.HasRelations)
			{
				int allCount = objectSet.Objects.Count;
				int count = 0;
				var pendingGroups = new System.Collections.Generic.Queue<PC.SCGroup>(allCount);
				var pendingRelations = new System.Collections.Generic.Queue<PC.SCRelationObject>(allCount);

				foreach (var r in objectSet.Relations)
				{
					if (r.ParentID == this.Parent.ID)
					{
						pendingRelations.Enqueue(r);
					}
				}

				context.SetStatus(0, pendingRelations.Count, "正在寻找当前组织内的关系。");

				if (objectSet.HasObjects)
				{
					foreach (var obj in objectSet.Objects)
					{
						if (obj.SchemaType == "Groups")
						{
							pendingGroups.Enqueue((PC.SCGroup)obj);
						}
					}
				}

				while (pendingGroups.Count > 0)
				{
					var grp = pendingGroups.Dequeue();
					try
					{
						var summaryName = grp.DisplayName;
						context.SetStatus(count, allCount, "正在导入群组:" + summaryName);

						if ((from r in pendingRelations where r.ParentID == this.Parent.ID && r.ID == grp.ID select r).Any())
						{
							PC.Executors.SCObjectOperations.InstanceWithPermissions.AddGroup(grp, this.Parent);

							context.AppendLog("已执行导入群组" + summaryName);

							if (this.IncludeMembers && objectSet.HasMembership)
							{
								context.AppendLog("正在查找群组成员");
								var members = from m in objectSet.Membership where m.ContainerID == grp.ID && m.Status == SchemaObjectStatus.Normal select m;

								foreach (var m in members)
								{
									try
									{
										var obj = (PC.SCUser)PC.Adapters.SchemaObjectAdapter.Instance.Load(m.ID);
										if (obj != null)
										{
											context.AppendLogFormat("正在添加群组成员{0}\r\n", obj.DisplayName);
											PC.Executors.SCObjectOperations.InstanceWithPermissions.AddUserToGroup(obj, grp);
										}
									}
									catch (Exception ex)
									{
										context.AppendLog("添加群组成员时遇到错误:" + ex.Message);
									}
								}
							}

							if (this.IncludeConditions && objectSet.HasConditions)
							{
								context.AppendLog("正在查找群组条件表达式");

								var conditions = new PC.Conditions.SCConditionCollection();
								conditions.CopyFrom(ImportService.Instance.FilterConditions(objectSet.Conditions, c => c.OwnerID == grp.ID && c.Type == "Default"));

								var owner = PC.Adapters.SCConditionAdapter.Instance.Load(grp.ID, "Default") ?? new PC.Conditions.SCConditionOwner() { OwnerID = grp.ID, Type = "Default" };
								owner.Conditions.ReplaceItemsWith(conditions, grp.ID, "Default");

								try
								{
									context.AppendLog("正在添加群组条件表达式");

									PC.Adapters.SCConditionAdapter.Instance.UpdateConditions(owner);

									context.AppendLog("群组条件表达式添加完毕");
								}
								catch (Exception ex)
								{
									context.AppendLog("未能添加群组条件表达式，原因是：" + ex.Message);
								}
							}
						}
					}
					catch (Exception ex)
					{
						context.AppendLog("对群组的操作失败，原因是：" + ex.Message);
					}
				}
			}
		}
	}
}