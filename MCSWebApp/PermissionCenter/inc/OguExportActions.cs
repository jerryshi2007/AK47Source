using System;
using System.Linq;
using System.Web;
using MCS.Library.Data.Builder;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace PermissionCenter
{
	public class OguExportActions : ExportAction
	{
		public override string CategoryName
		{
			get
			{
				return "组织机构";
			}
		}

		public override SCObjectSet Execute(HttpRequest req)
		{
			string[] ids = req.Form.GetValues("id");

			if (ids == null && ids.Length < 0)
				throw new HttpException("当获组织机构对象时，必须提供ID参数");

			SCObjectSet objectSet = new SCObjectSet();
			objectSet.Scope = "OguObjects";

			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendItem("Status", (int)SchemaObjectStatus.Normal);

			ConnectiveSqlClauseCollection conditios = new ConnectiveSqlClauseCollection(where, VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder());

			{
				string parentId = req.Form["ParentId"];
				if (string.IsNullOrEmpty(parentId))
					throw new HttpException("当获取组织机构对象时，必须提供ParentId参数");

				PC.SCOrganization parentObj = parentId == PC.SCOrganization.RootOrganizationID ? PC.SCOrganization.GetRoot() : (PC.SCOrganization)PC.Adapters.SchemaObjectAdapter.Instance.Load(parentId);
				if (parentObj == null || parentObj.Status != SchemaObjectStatus.Normal)
					throw new HttpException("指定的父级对象不存在或者已删除");

				InSqlClauseBuilder idIn = new InSqlClauseBuilder("ID");
				idIn.AppendItem(ids);
				conditios.Add(idIn);

				objectSet.Objects = PC.Adapters.SchemaObjectAdapter.Instance.Load(conditios);

				objectSet.Relations = this.FilterChildren(PC.Adapters.SchemaRelationObjectAdapter.Instance.LoadByParentID(parentId), ids);

				// 保存对象的下级关系（如果有）
				objectSet.Membership = ExportQueryHelper.LoadFullMemberships(ids);

				// 如果含有群组，导出群组的条件
				string[] groupIds = (from PC.SchemaObjectBase o in objectSet.Objects where o is PC.SCGroup select o.ID).ToArray();

				if (groupIds.Length > 0)
				{
					var conditions = ExportQueryHelper.LoadConditions(groupIds);
					if (conditions.Count > 0)
						objectSet.Conditions = conditions;
				}

				// 如果含有组织，导出组织的Acl
				string[] orgIds = (from PC.SchemaObjectBase o in objectSet.Objects where o is PC.SCOrganization select o.ID).ToArray();

				objectSet.Acls = ExportQueryHelper.LoadAclsFor(orgIds);
			}

			return objectSet;
		}
	}
}