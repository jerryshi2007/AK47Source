using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.Core;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	public static class AUObjectExportor
	{
		public static SCObjectSet ExportAUSchemas(string category)
		{
			SCObjectSet objectSet = new SCObjectSet();
			objectSet.Scope = "AUSchemas";

			var schemas = Adapters.AUSnapshotAdapter.Instance.LoadAUSchemaByCategory(category, true, DateTime.MinValue);

			AUCommon.DoDbAction(() =>
			{
				if (schemas.Count > 0)
				{
					var members = ExportQueryHelper.LoadMembershipOf(schemas.ToIDArray()); //查找Schema中的角色关系
					var roles = ExportQueryHelper.LoadObjects(members.Select(m => m.ID).ToArray(), AUCommon.SchemaAUSchemaRole);
					objectSet.Objects = schemas + roles;
					objectSet.Membership = members;
				}
			});

			return objectSet;
		}

		public static SCObjectSet ExportAUSchemas(string[] auSchemaIDs)
		{
			auSchemaIDs.NullCheck("auSchemaIDs");

			if (auSchemaIDs.Length == 0)
				throw new ArgumentException("至少应包含一个AUSchemaID", "auSchemaIDs");

			SCObjectSet objectSet = new SCObjectSet();
			objectSet.Scope = "AUSchemas";

			AUCommon.DoDbAction(() =>
			{
				var schemas = ExportQueryHelper.LoadObjects(auSchemaIDs, AUCommon.SchemaAUSchema);
				if (schemas.Count > 0)
				{
					var members = ExportQueryHelper.LoadMembershipOf(schemas.ToIDArray()); //查找Schema中的角色关系
					var roles = ExportQueryHelper.LoadObjects(members.Select(m => m.ID).ToArray(), AUCommon.SchemaAUSchemaRole);
					objectSet.Objects = schemas + roles;
					objectSet.Membership = members;
				}
			});

			return objectSet;
		}

		/// <summary>
		/// 导出指定AUSchemaID
		/// </summary>
		/// <param name="auSchemaID"></param>
		/// <param name="roleIDs">为null或长度为0时，取出所有角色</param>
		/// <returns></returns>
		public static SCObjectSet ExportAUSchemaRoles(string auSchemaID, string[] roleIDs)
		{
			auSchemaID.NullCheck("auSchemaID");

			var auSchema = AU.Adapters.AUSnapshotAdapter.Instance.LoadAUSchema(auSchemaID, true, DateTime.MinValue).FirstOrDefault();

			if (auSchema == null)
				throw new AUObjectException("指定的管理架构ID无效");

			SCObjectSet objectSet = new SCObjectSet();
			objectSet.Scope = "AUSchemaRoles";
			AU.AUCommon.DoDbAction(() =>
			{
				if (roleIDs != null && roleIDs.Length > 0)
					objectSet.Objects = new SchemaObjectCollection(AU.Adapters.AUSnapshotAdapter.Instance.LoadAUSchemaRoles(auSchema.ID, roleIDs, false, DateTime.MinValue)); //包含删除的
				else
					objectSet.Objects = new SchemaObjectCollection(AU.Adapters.AUSnapshotAdapter.Instance.LoadAUSchemaRoles(auSchema.ID, false, DateTime.MinValue)); //包含删除的

				objectSet.Membership = ExportQueryHelper.LoadFullMemberships(objectSet.Objects.ToIDArray());
			});

			return objectSet;
		}

		public static SCObjectSet ExportAdminUnits(string auSchemaID, string[] unitIDs, bool deep)
		{
			auSchemaID.NullCheck("auSchemaID");

			SCObjectSet objectSet = new SCObjectSet();
			objectSet.Scope = "AdminUnits";

			objectSet.Objects = AU.Adapters.AUSnapshotAdapter.Instance.LoadAUBySchemaID(auSchemaID, unitIDs, true, DateTime.MinValue);

			if (objectSet.Objects.Count > 0)
			{
				var ids = objectSet.Objects.ToIDArray();

				if (deep)
				{
					var allSubUnits = Adapters.AUSnapshotAdapter.Instance.LoadCurrentSubUnitsDeeply(ids, DateTime.MinValue);
					objectSet.Objects.Merge(allSubUnits);
				}

				AUCommon.DoDbAction(() =>
				{
					objectSet.Relations = ExportQueryHelper.LoadFullRelations(ids);

					objectSet.Membership = ExportQueryHelper.LoadFullMemberships(ids, false); //包含删除的

					objectSet.Acls = ExportQueryHelper.LoadAclsFor(ids);

					//查找角色
					var roleIDs = objectSet.Membership.Where(m => m.MemberSchemaType == AU.AUCommon.SchemaAdminUnitRole).Select(m => m.ID).ToArray();
					var scopeIDs = objectSet.Membership.Where(m => m.MemberSchemaType == AU.AUCommon.SchemaAUAdminScope).Select(m => m.ID).ToArray();

					LookupMemberObjects(objectSet, roleIDs, AUCommon.SchemaAdminUnitRole);

					LookupMemberObjects(objectSet, scopeIDs, AUCommon.SchemaAUAdminScope);

					objectSet.Conditions = ExportQueryHelper.LoadConditions(scopeIDs);
				});
			}

			return objectSet;
		}

		private static void LookupMemberObjects(SCObjectSet objectSet, string[] memberIDs, string schemaType)
		{
			if (memberIDs != null && memberIDs.Length > 0)
			{
				var members = ExportQueryHelper.LoadObjects(memberIDs, schemaType, false);

				objectSet.Objects.Merge(members);

				//查找角色的固定成员
				var roleMembers = ExportQueryHelper.LoadMembershipOf(memberIDs);

				objectSet.Membership.AddRange(roleMembers);
			}
		}
	}
}
