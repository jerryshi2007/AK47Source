using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Services;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Client;
using PermissionCenter.Clients;
using PermissionCenter.Extensions;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.Client;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Schemas.Client.ServiceBroker;

namespace PermissionCenter.Services
{
	/// <summary>
	/// Summary description for PermissionCenterQueryService
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	// [System.Web.Script.Services.ScriptService]
	[XmlInclude(typeof(ClientSchemaObjectBase))]
	[XmlInclude(typeof(ClientSCBase))]
	[XmlInclude(typeof(ClientSCUser))]
	[XmlInclude(typeof(ClientSCOrganization))]
	[XmlInclude(typeof(ClientSCGroup))]
	[XmlInclude(typeof(ClientSCApplication))]
	[XmlInclude(typeof(ClientSCRole))]
	[XmlInclude(typeof(ClientSCPermission))]
	[XmlInclude(typeof(ClientConditionItem))]
	[XmlInclude(typeof(ClientAclItem))]
	[XmlInclude(typeof(ClientRoleDisplayItem))]
	[ScriptService]
	public class PermissionCenterQueryService : System.Web.Services.WebService, ISchemaQueryService
	{
		[WebMethod(Description = "获取根组织")]
		public ClientSCOrganization GetRoot()
		{
			SCOrganization root = SCOrganization.GetRoot();

			return (ClientSCOrganization)root.ToClientSCBaseObject();
		}

		/// <summary>
		/// 根据ID和Schema类型获取对象
		/// </summary>
		/// <param name="ids">ID的列表，为 null 时将返回符合SchemaType的任何数据，但空数组除外。</param>
		/// <param name="objectSchemaTypes">用于过滤的Schema类型，为空数组时表示不过滤</param>
		/// <param name="normalOnly">为true时，仅查询正常对象，否则包含删除的对象</param>
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
			objectSchemaTypes.NullCheck("objectSchemaTypes");

			ClientSchemaObjectBase[] result;

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
					result = SchemaObjectAdapter.Instance.Load(builder, SimpleRequestSoapMessage.Current.TimePoint).ToClientSCBaseObjectArray();
				else
					result = new ClientSchemaObjectBase[0];
			}

			return result;
		}

		/// <summary>
		/// 根据CodeName查询对象
		/// </summary>
		/// <param name="codeNames">代码名称</param>
		/// <param name="normalOnly">为true时，仅查询正常对象，否则包含删除的对象</param>
		/// <param name="objectSchemaTypes">对象类型的数组，为空数组时不过滤</param>
		/// <returns></returns>
		[WebMethod(Description = "根据代码名称和Schema类型获取对象")]
		public ClientSchemaObjectBase[] GetObjectsByCodeNames(string[] codeNames, string[] objectSchemaTypes, bool normalOnly)
		{
			codeNames.NullCheck("codeNames");
			objectSchemaTypes.NullCheck("objectSchemaTypes");

			if (codeNames.Length > 0)
			{
				SchemaObjectCollection pcObjects = SchemaObjectAdapter.Instance.LoadByCodeNameAndSchema(objectSchemaTypes, codeNames, normalOnly, false, SimpleRequestSoapMessage.Current.TimePoint);
				return pcObjects.ToClientSCBaseObjectArray();
			}
			else
			{
				return new ClientSCBase[0];
			}
		}

		[WebMethod(Description = "获取对象的成员信息")]
		public ClientSchemaMember[] GetMemberships(string containerID, string[] memberSchemaTypes, bool normalOnly)
		{
			containerID.NullCheck("id");

			memberSchemaTypes.NullCheck("memberSchemaTypes");

			var memberships = SCMemberRelationAdapter.Instance.LoadByContainerID(containerID, memberSchemaTypes, normalOnly, SimpleRequestSoapMessage.Current.TimePoint);

			return memberships.ToClientSchemaObjectBaseObjectArray<SCSimpleRelationBase, ClientSchemaMember>();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xQuery"></param>
		/// <param name="objectSchemaTypes">对象的Schema类型的数组，为空数组时不过滤。</param>
		/// <param name="normalOnly"></param>
		/// <returns></returns>
		[WebMethod(Description = "根据XPath查询对象")]
		public ClientSchemaObjectBase[] GetObjectsByXQuery(string xQuery, string[] objectSchemaTypes, bool normalOnly)
		{
			xQuery.NullCheck("xQuery");
			objectSchemaTypes.NullCheck("objectSchemaTypes");

			var objs = SchemaObjectAdapter.Instance.LoadByXPath(xQuery, objectSchemaTypes, normalOnly == false, DateTime.MinValue);
			return objs.ToClientSchemaObjectBaseObjectArray();
		}

		/// <summary>
		/// 查找某个对象所有父对象
		/// </summary>
		/// <param name="id"></param>
		/// <param name="parentSchemaTypes"></param>
		/// <param name="normalOnly">为true时，仅查询正常对象，否则包含删除的对象</param>
		/// <returns></returns>
		[WebMethod(Description = "根据ID和父级Schema类型获取所有父对象")]
		public ClientSchemaObjectBase[] GetParents(string id, string[] parentSchemaTypes, bool normalOnly)
		{
			id.NullCheck("id");

			parentSchemaTypes.NullCheck("parentSchemaTypes");

			var parentRelations = PC.Adapters.SchemaRelationObjectAdapter.Instance.LoadByObjectID(id, normalOnly == false, DateTime.MinValue);

			string[] ids = new string[parentRelations.Count];

			for (int i = 0; i < ids.Length; i++)
				ids[i] = parentRelations[i].ParentID;

			return GetObjectByIDInner(ids, parentSchemaTypes, normalOnly);
		}

		/// <summary>
		/// 查找某个对象的子对象
		/// </summary>
		/// <param name="id"></param>
		/// <param name="normalOnly">为true时，仅查询正常对象，否则包含删除的对象</param>
		/// <returns></returns>
		[WebMethod(Description = "根据ID和子级Schema类型获取所有子对象")]
		public ClientSchemaObjectBase[] GetChildren(string id, string[] childSchemaTypes, bool normalOnly)
		{
			childSchemaTypes.NullCheck("childSchemaTypes");
			id.NullCheck("id");

			string[] childrenIds = PC.Adapters.SchemaRelationObjectAdapter.Instance.LoadByParentID(id, normalOnly, DateTime.MinValue).ToIDArray();

			return GetObjectByIDInner(childrenIds, childSchemaTypes, normalOnly);
		}


		/// <summary>
		/// 查找某个对象的成员
		/// </summary>
		/// <param name="id"></param>
		/// <param name="memberSchemaTypes">成员的Schema类型，如果没有，则表示所有</param>
		/// <param name="normalOnly">为true时，仅查询正常对象，否则包含删除的对象</param>
		/// <returns></returns>
		[WebMethod(Description = "根据ID和成员Schema类型获取所有成员对象")]
		public ClientSchemaObjectBase[] GetMembers(string id, string[] memberSchemaTypes, bool normalOnly)
		{
			id.NullCheck("id");

			memberSchemaTypes.NullCheck("memberSchemaTypes");

			string[] childrenIds = PC.Adapters.SCMemberRelationAdapter.Instance.LoadByContainerID(id, memberSchemaTypes, normalOnly, DateTime.MinValue).ToIDArray();

			return GetObjectByIDInner(childrenIds, memberSchemaTypes, normalOnly);
		}

		/// <summary>
		/// 根据ID和成员Schema类型获取所有容器对象
		/// </summary>
		/// <param name="id"></param>
		/// <param name="containerSchemaTypes"></param>
		/// <param name="normalOnly">为true时，仅查询正常对象，否则包含删除的对象</param>
		/// <returns></returns>
		[WebMethod(Description = "根据ID和成员Schema类型获取所有容器对象")]
		public ClientSchemaObjectBase[] GetContainers(string id, string[] containerSchemaTypes, bool normalOnly)
		{
			id.NullCheck("id");

			containerSchemaTypes.NullCheck("containerSchemaTypes");

			string[] containerIDs = PC.Adapters.SCMemberRelationAdapter.Instance.LoadByMemberID(id, containerSchemaTypes, normalOnly, DateTime.MinValue).ToContainerIDArray();

			return GetObjectByIDInner(containerIDs, containerSchemaTypes, normalOnly);
		}

		[WebMethod(Description = "根据ID获取对象所有ACL")]
		public ClientAclItem[] GetAcls(string id)
		{
			id.NullCheck("id");

			var acls = PC.Adapters.SCAclAdapter.Instance.LoadByContainerID(id, SimpleRequestSoapMessage.Current.TimePoint);

			ClientAclItem[] result = new ClientAclItem[acls.Count];

			for (int i = 0; i < result.Length; i++)
				result[i] = acls[i].ToClientObject();

			return result;
		}

		[WebMethod(Description = "根据ID和获取所有条件")]
		public ClientConditionItem[] GetConditions(string id)
		{
			id.NullCheck("id");

			var owner = PC.Adapters.SCConditionAdapter.Instance.Load(id, null, SimpleRequestSoapMessage.Current.TimePoint);

			return owner.Conditions.ToClientObjectsArray();
		}

		[WebMethod(Description = "获取指定Schema的属性定义")]
		public ClientPropertyDefine[] GetSchemaPropertyDefinition(string schemaType)
		{
			schemaType.CheckStringIsNullOrEmpty("schemaType");

			var definitions = PC.SchemaDefine.GetSchema(schemaType).Properties;

			ClientPropertyDefine[] results = new ClientPropertyDefine[definitions.Count];

			for (int i = 0; i < results.Length; i++)
				results[i] = definitions[i].ToClientPropertyDefine();

			return results;
		}

		[WebMethod(Description = "根据ID查询角色和应用")]
		public ClientRoleDisplayItem[] GetRoleDisplayItems(string[] roleIds)
		{
			roleIds.NullCheck("roleIds");

			if (roleIds.Length > 0)
			{
				var rs = RoleDisplayItemAdapter.Instance.LoadByRoleIds(roleIds);

				return (from r in rs
						select new ClientRoleDisplayItem()
						{
							ApplicationDisplayName = r.ApplicationDisplayName,
							ApplicationID = r.ApplicationID,
							ApplicationName = r.ApplicationName,
							RoleCodeName = r.RoleCodeName,
							RoleDisplayName = r.RoleDisplayName,
							RoleID = r.RoleID,
							RoleName = r.RoleName
						}).ToArray();
			}
			else
			{
				return new ClientRoleDisplayItem[0];
			}
		}
	}
}
