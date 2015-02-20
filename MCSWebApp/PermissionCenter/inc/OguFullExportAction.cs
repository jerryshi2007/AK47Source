using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace PermissionCenter
{
	public class OguFullExportAction : ExportAction
	{
		public override string CategoryName
		{
			get
			{
				return "深度组织机构";
			}
		}

		public override SCObjectSet Execute(HttpRequest req)
		{
			string parentId = req.Form["ParentId"];
			if (string.IsNullOrEmpty(parentId))
				throw new HttpException("当获取深度组织机构对象时，必须提供ParentId参数");

			PC.SCOrganization parentObj = parentId == PC.SCOrganization.RootOrganizationID ? PC.SCOrganization.GetRoot() : (PC.SCOrganization)PC.Adapters.SchemaObjectAdapter.Instance.Load(parentId);
			if (parentObj == null || parentObj.Status != SchemaObjectStatus.Normal)
				throw new HttpException("指定的父级对象不存在或者已删除");

			List<string> ids;
			string[] requestIds = req.Form.GetValues("id");
			if (requestIds == null || requestIds.Length == 0)
			{
				PC.SCSimpleRelationObjectCollection allChildren = PC.Adapters.SchemaRelationObjectAdapter.Instance.LoadAllChildrenRelationsRecursively(parentObj);
				ids = new List<string>(allChildren.Count);
				allChildren.ForEach(so => ids.Add(so.ID));
			}
			else
			{
				PC.SCSimpleRelationObjectCollection allChildren = new PC.SCSimpleRelationObjectCollection();
				ids = new List<string>();

				var directRelations = ExportQueryHelper.LoadRelations(parentId, requestIds);

				foreach (var item in directRelations)
				{
					ids.Add(item.ID);
				}

				var selectedObjects = DbUtil.LoadObjects(ids.ToArray());
				foreach (var item in selectedObjects)
				{
					var subRelations = PC.Adapters.SchemaRelationObjectAdapter.Instance.LoadAllChildrenRelationsRecursively(item);
					subRelations.ForEach(m => ids.Add(m.ID));
				}
			}

			SCObjectSet set = new SCObjectSet();

			if (ids.Count > 0)
			{
				set.Objects = DbUtil.LoadObjects(ids.ToArray());
				set.Objects.Distinct((m, n) => m.ID == n.ID);

				var objIds = set.Objects.ToIDArray();

				set.Relations = ExportQueryHelper.LoadFullRelations(objIds);
				set.Relations.Distinct((a, b) => a.ID == b.ID && a.ParentID == b.ParentID);

				set.Membership = ExportQueryHelper.LoadFullMemberships(objIds);
				set.Membership.Distinct((a, b) => a.ID == b.ID && a.ContainerID == b.ContainerID);

				set.Acls = ExportQueryHelper.LoadAclsFor(objIds);

				set.Conditions = ExportQueryHelper.LoadConditions(objIds);
			}

			set.Scope = "OguFull";
			return set;
		}
	}
}