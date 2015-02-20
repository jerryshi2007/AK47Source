using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml.Serialization;
using MCS.Library.SOA.DataObjects.Schemas.Client;
using MCS.Library.Services;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Adapters;
using MCS.Library.SOA.DataObjects.Security.AUClient;

namespace AUCenterServices.Services
{
	/// <summary>
	/// Summary description for AUCenterQueryService
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	[XmlInclude(typeof(ClientSchemaObjectBase))]
	[XmlInclude(typeof(ClientGenericObject))]
	[XmlInclude(typeof(ClientAdminUnit))]
	[XmlInclude(typeof(ClientAUAdminScope))]
	[XmlInclude(typeof(ClientAUAdminScopeItem))]
	[XmlInclude(typeof(ClientAURole))]
	[XmlInclude(typeof(ClientAURoleDisplayItem))]
	[XmlInclude(typeof(ClientAUSchema))]
	[XmlInclude(typeof(ClientAUSchemaRole))]
	[XmlInclude(typeof(ClientNamedObject))]
	[XmlInclude(typeof(ClientConditionItem))]
	[XmlInclude(typeof(ClientAclItem))]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	[System.Web.Script.Services.ScriptService]
	public class AUCenterQueryService : System.Web.Services.WebService, IAUCenterQueryService
	{
		/// <summary>
		/// 根据ID和Schema类型获取对象
		/// </summary>
		/// <param name="ids">ID的列表，空数组将返回符合SchemaType的任何数据。</param>
		/// <param name="objectSchemaTypes">用于过滤的Schema类型，空数组表示将不进行过滤</param>
		/// <param name="normalOnly">为true时，仅含正常对象。</param>
		/// <returns></returns>
		/// <remarks><paramref name="ids"/>和<paramref name="objectSchemaTypes"/>同时为空数组时，不会反回任何数据。</remarks>
		[WebMethod(Description = "根据ID和Schema类型获取对象")]
		public ClientSchemaObjectBase[] GetObjectsByIDs(string[] ids, string[] objectSchemaTypes, bool normalOnly)
		{
			objectSchemaTypes.NullCheck("objectSchemaTypes");

			return GetObjectByIDInner(ids, objectSchemaTypes, normalOnly);
		}

		private static ClientSchemaObjectBase[] GetObjectByIDInner(string[] ids, string[] objectSchemaTypes, bool normalOnly)
		{
			ClientSchemaObjectBase[] result;
			objectSchemaTypes.NullCheck("objectSchemaTypes");

			bool noIDMatch = ids != null && ids.Length == 0;

			if (noIDMatch)
			{
				result = new ClientSchemaObjectBase[0];
			}
			else
			{
				ConnectiveSqlClauseCollection builder = new ConnectiveSqlClauseCollection();
				if (ids != null)
				{
					InSqlClauseBuilder idInBuilder = new InSqlClauseBuilder("ID");
					idInBuilder.AppendItem(ids);

					builder.Add(idInBuilder);
				}

				if (objectSchemaTypes != null)
				{
					InSqlClauseBuilder typeInBuilder = new InSqlClauseBuilder("SchemaType");
					typeInBuilder.AppendItem(objectSchemaTypes);
					builder.Add(typeInBuilder);
				}

				if (normalOnly)
					builder.Add((IConnectiveSqlClause)new WhereSqlClauseBuilder().AppendItem("Status", (int)SchemaObjectStatus.Normal));

				if (builder.IsEmpty == false)
					result = AUCommon.DoDbProcess(() => SchemaObjectAdapter.Instance.Load(builder, SimpleRequestSoapMessage.Current.TimePoint).ToClientSchemaObjectBaseObjectArray());
				else
					result = new ClientSchemaObjectBase[0];
			}

			return result;
		}

		/// <summary>
		/// 根据CodeName查询对象
		/// </summary>
		/// <param name="codeNames"></param>
		/// <param name="objectSchemaTypes"></param>
		/// <param name="normalOnly">为true时，仅含正常对象。</param>
		/// <returns></returns>
		[WebMethod(Description = "根据代码名称和Schema类型获取对象")]
		public ClientSchemaObjectBase[] GetObjectsByCodeNames(string[] codeNames, string[] objectSchemaTypes, bool normalOnly)
		{
			codeNames.NullCheck("codeNames");
			objectSchemaTypes.NullCheck("objectsSchemaTypes");

			if (codeNames.Length > 0)
			{
				SchemaObjectCollection pcObjects = AUCommon.DoDbProcess(() =>
					SchemaObjectAdapter.Instance.LoadByCodeNameAndSchema(objectSchemaTypes, codeNames, normalOnly, false, SimpleRequestSoapMessage.Current.TimePoint));

				return pcObjects.ToClientSchemaObjectBaseObjectArray();
			}
			else
			{
				return new ClientSchemaObjectBase[0];
			}
		}

		/// <summary>
		/// 查找某个对象所有父对象
		/// </summary>
		/// <param name="id"></param>
		/// <param name="parentSchemaTypes">用于过滤的Schema类型，为空数组时不过滤。</param>
		/// <param name="normalOnly">为true时，仅含正常对象。</param>
		/// <returns></returns>
		[WebMethod(Description = "根据ID和父级Schema类型获取所有父对象")]
		public ClientSchemaObjectBase[] GetParents(string id, string[] parentSchemaTypes, bool normalOnly)
		{
			parentSchemaTypes.NullCheck("parentSchemaTypes");

			var parentRelations = AUCommon.DoDbProcess(() => SchemaRelationObjectAdapter.Instance.LoadByObjectID(id, normalOnly == false, SimpleRequestSoapMessage.Current.TimePoint));

			string[] ids = new string[parentRelations.Count];

			for (int i = 0; i < ids.Length; i++)
				ids[i] = parentRelations[i].ParentID;

			return GetObjectByIDInner(ids, parentSchemaTypes, normalOnly);
		}

		/// <summary>
		/// 查找某个对象的子对象
		/// </summary>
		/// <param name="id"></param>
		/// <param name="childSchemaTypes">用于过滤的Schema类型，为空数组时不过滤。</param>
		/// <param name="normalOnly">为true时，仅含正常对象。</param>
		/// <returns></returns>
		[WebMethod(Description = "根据ID和子级Schema类型获取所有子对象")]
		public ClientSchemaObjectBase[] GetChildren(string id, string[] childSchemaTypes, bool normalOnly)
		{
			childSchemaTypes.NullCheck("childSchemaTypes");

			string[] childrenIds = AUCommon.DoDbProcess(() =>
				SchemaRelationObjectAdapter.Instance.LoadByParentID(id, normalOnly, SimpleRequestSoapMessage.Current.TimePoint).ToIDArray());

			return GetObjectByIDInner(childrenIds, childSchemaTypes, normalOnly);
		}

		/// <summary>
		/// 查找某个对象的成员
		/// </summary>
		/// <param name="id"></param>
		/// <param name="memberSchemaTypes">成员的Schema类型，如果为空数组，则表示所有</param>
		/// <param name="normalOnly">为true时，仅含正常对象。</param>
		/// <returns></returns>
		[WebMethod(Description = "根据ID和成员Schema类型获取所有成员对象")]
		public ClientSchemaObjectBase[] GetMembers(string id, string[] memberSchemaTypes, bool normalOnly)
		{
			id.NullCheck("id");
			memberSchemaTypes.NullCheck("memberSchemaTypes");

			string[] childrenIds = AUCommon.DoDbProcess(() =>
				SCMemberRelationAdapter.Instance.LoadByContainerID(id, memberSchemaTypes, normalOnly, SimpleRequestSoapMessage.Current.TimePoint).ToIDArray());

			return GetObjectByIDInner(childrenIds, memberSchemaTypes, normalOnly);
		}

		/// <summary>
		/// 查找某个对象的成员关系
		/// </summary>
		/// <param name="containerID"></param>
		/// <param name="memberSchemaTypes">成员的Schema类型，如果没有，则表示所有</param>
		/// <param name="normalOnly">为true时，仅含正常对象。</param>
		/// <returns></returns>
		[WebMethod(Description = "根据ID和成员Schema类型获取所有成员对象")]
		public ClientSchemaMember[] GetMemberships(string containerID, string[] memberSchemaTypes, bool normalOnly)
		{
			containerID.NullCheck("id");
			memberSchemaTypes.NullCheck("memberSchemaTypes");

			var memberships = AUCommon.DoDbProcess(() =>
				SCMemberRelationAdapter.Instance.LoadByContainerID(containerID, memberSchemaTypes, normalOnly, SimpleRequestSoapMessage.Current.TimePoint));

			return memberships.ToClientSchemaObjectBaseObjectArray<SCSimpleRelationBase, ClientSchemaMember>();
		}

		/// <summary>
		/// 根据ID和成员Schema类型获取所有容器对象
		/// </summary>
		/// <param name="id"></param>
		/// <param name="memberSchemaTypes"></param>
		/// <param name="normalOnly">为true时，仅含正常对象。</param>
		/// <returns></returns>
		[WebMethod(Description = "根据ID和成员Schema类型获取所有容器对象")]
		public ClientSchemaObjectBase[] GetContainers(string id, string[] containerSchemaTypes, bool normalOnly)
		{
			id.NullCheck("id");

			string[] containers = AUCommon.DoDbProcess(() =>
				SCMemberRelationAdapter.Instance.LoadByMemberID(id, containerSchemaTypes, normalOnly, SimpleRequestSoapMessage.Current.TimePoint).ToContainerIDArray());

			return GetObjectByIDInner(containers, containerSchemaTypes, normalOnly);
		}


		[WebMethod(Description = "根据ID获取对象所有ACL")]
		public ClientAclItem[] GetAcls(string id)
		{
			var acls = AUAclAdapter.Instance.LoadByContainerID(id, SimpleRequestSoapMessage.Current.TimePoint);

			ClientAclItem[] result = new ClientAclItem[acls.Count];

			for (int i = 0; i < result.Length; i++)
				result[i] = acls[i].ToClientObject();

			return result;
		}

		[WebMethod(Description = "根据ID和获取所有条件")]
		public ClientConditionItem[] GetConditions(string id)
		{
			var owner = AUConditionAdapter.Instance.Load(id, AUCommon.ConditionType, SimpleRequestSoapMessage.Current.TimePoint);

			return owner.ToClientObjectsArray();
		}

		[WebMethod(Description = "获取指定Schema的属性定义")]
		public ClientPropertyDefine[] GetSchemaPropertyDefinition(string schemaType)
		{
			var definitions = SchemaDefine.GetSchema(schemaType).Properties;

			ClientPropertyDefine[] results = new ClientPropertyDefine[definitions.Count];

			for (int i = 0; i < results.Length; i++)
			{
				results[i] = definitions[i].ToClientPropertyDefine();
			}

			return results;
		}

		[WebMethod(Description = "获取扩展的属性定义<br/>schemaType:对象的类型，目前只支持AdminUnits<br/>sourceID:扩展属性关联的键。对于管理单元，是其管理架构ID。")]
		public ClientPropertyDefine[] GetExtendedPropertyDefinition(string schemaType, string sourceID)
		{
			ClientPropertyDefine[] result = null;

			var ext = SchemaPropertyExtensionAdapter.Instance.Load(schemaType, sourceID);
			if (ext != null)
			{
				var definitions = ext.Properties;
				result = new ClientPropertyDefine[definitions.Count];
				for (int i = 0; i < result.Length; i++)
				{
					result[i] = definitions[i].ToClientPropertyDefine();
				}
			}
			else
			{
				result = new ClientPropertyDefine[0];
			}

			return result;

		}

		[WebMethod(Description = "获取管理范围")]
		public ClientAUAdminScope GetAUAdminScope(string unitID, string scopeType, bool normalOnly)
		{
			var scope = AUSnapshotAdapter.Instance.LoadAUScope(unitID, scopeType, normalOnly, SimpleRequestSoapMessage.Current.TimePoint).FirstOrDefault();

			return scope != null ? (ClientAUAdminScope)scope.ToClientSchemaObjectBaseObject() : null;
		}

		[WebMethod(Description = "获取管理单元角色")]
		public ClientAURole GetAURole(string unitID, string codeName, bool normalOnly)
		{
			AURole result = null;
			var unit = AUCommon.DoDbProcess(() =>
				(AdminUnit)SchemaObjectAdapter.Instance.Load(unitID));
			if (unit != null && normalOnly)
				unit = unit.Status == SchemaObjectStatus.Normal ? unit : null;

			if (unit != null)
			{
				var schemaRole = AUSnapshotAdapter.Instance.LoadAUSchemaRoleByCodeName(unit.AUSchemaID, codeName, normalOnly, SimpleRequestSoapMessage.Current.TimePoint);
				if (schemaRole != null && normalOnly)
					schemaRole = schemaRole.Status == SchemaObjectStatus.Normal ? schemaRole : null;

				if (schemaRole != null)
					result = AUSnapshotAdapter.Instance.LoadAURole(schemaRole.ID, unitID, normalOnly, SimpleRequestSoapMessage.Current.TimePoint);
			}

			return result != null ? (ClientAURole)result.ToClientSchemaObject() : null;
		}

		[WebMethod(Description = "获取管理单元角色")]
		public ClientAURole GetAURoleBySchemaRoleID(string unitID, string schemaRoleID, bool normalOnly)
		{
			unitID.NullCheck("unitID"); schemaRoleID.NullCheck("schemaRoleID");
			AURole result = null;
			var unit = AUCommon.DoDbProcess(() =>
				(AdminUnit)SchemaObjectAdapter.Instance.Load(unitID));
			if (unit != null && normalOnly)
				unit = unit.Status == SchemaObjectStatus.Normal ? unit : null;

			if (unit != null)
			{
				var schemaRole = (AUSchemaRole)AUCommon.DoDbProcess(() => SchemaObjectAdapter.Instance.Load(schemaRoleID));
				if (schemaRole != null && normalOnly)
					schemaRole = schemaRole.Status == SchemaObjectStatus.Normal ? schemaRole : null;

				if (schemaRole != null)
					result = AUSnapshotAdapter.Instance.LoadAURole(schemaRole.ID, unitID, normalOnly, SimpleRequestSoapMessage.Current.TimePoint);
			}

			return result != null ? (ClientAURole)result.ToClientSchemaObject() : null;
		}

		[WebMethod(Description = "根据分类ID获取其中的管理架构")]
		public ClientAUSchema[] GetAUSchemaByCategory(string categoryID, bool normalOnly)
		{
			return AUSnapshotAdapter.Instance.LoadAUSchemaByCategory(categoryID, normalOnly, SimpleRequestSoapMessage.Current.TimePoint).ToClientSchemaObjectBaseObjectArray<ClientAUSchema>();
		}

		[WebMethod(Description = "获取管理架构的角色")]
		public ClientAUSchemaRole[] GetAUSchemaRoles(string schemaID, string[] codeNames, bool normalOnly)
		{
			if (codeNames != null && codeNames.Length == 0)
				return new ClientAUSchemaRole[0];
			else
			{
				var roles = AUSnapshotAdapter.Instance.LoadAUSchemaRolesByCodeNames(schemaID, codeNames ?? new string[0], normalOnly, SimpleRequestSoapMessage.Current.TimePoint);

				return roles.ToClientSchemaObjectBaseObjectArray<AUSchemaRole, ClientAUSchemaRole>();
			}
		}

		[WebMethod(Description = "获取下级分类")]
		public ClientAUSchemaCategory[] GetSubCategories(string parentID, bool normalOnly)
		{
			var categories = SchemaCategoryAdapter.Instance.LoadSubCategories(parentID, normalOnly, SimpleRequestSoapMessage.Current.TimePoint);
			ClientAUSchemaCategory[] result = new ClientAUSchemaCategory[categories.Count];
			for (int i = categories.Count - 1; i >= 0; i--)
				result[i] = categories[i].ToClientCategory();

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xQuery"></param>
		/// <param name="objectSchemaTypes">要过滤的schema类型，为空数组时不过滤。</param>
		/// <param name="normalOnly"></param>
		/// <returns></returns>
		[WebMethod(Description = "根据XPath查询")]
		public ClientSchemaObjectBase[] GetObjectsByXQuery(string xQuery, string[] objectSchemaTypes, bool normalOnly)
		{
			xQuery.NullCheck("xQuery");
			objectSchemaTypes.NullCheck("objectSchemaTypes");

			var objs = SchemaObjectAdapter.Instance.LoadByXPath(xQuery, objectSchemaTypes, normalOnly == false, DateTime.MinValue);
			return objs.ToClientSchemaObjectBaseObjectArray();
		}
	}
}
