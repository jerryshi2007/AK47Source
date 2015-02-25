using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OP = MCS.Library.OGUPermission;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.Data.Builder;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.PermissionBridge
{
	public sealed class BridgedPermissionMechanism : OP.IPermissionMechanism, OP.IPermissionImplInterface
	{
		#region 过时
		//public OP.ApplicationCollection GetAllApplications()
		//{

		//    WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
		//    builder.AppendItem("Status", (int)SchemaObjectStatus.Normal);
		//    builder.AppendItem("SchemaType", PC.StandardObjectSchemaType.Applications.ToString());
		//    var dataSource = PC.Adapters.SchemaObjectAdapter.Instance.Load(builder);
		//    var arr = new OP.IApplication[dataSource.Count];
		//    int count = 0;
		//    var factory = Util.GetPermissionObjectFactory();
		//    foreach (var obj in dataSource)
		//    {
		//        var item = (OP.IApplication)factory.CreateObject(typeof(OP.IApplication));
		//        arr[count++] = item;
		//        ((Objects.IBridgedObject)item).InitProperties(obj);
		//    }
		//    return new OP.ApplicationCollection(arr);

		//}

		//public OP.ApplicationCollection GetApplications(params string[] codeNames)
		//{
		//    if (codeNames.Length == 0)
		//        throw new ArgumentOutOfRangeException("至少得包含一个名称");
		//    var objects = PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(DateTime.MinValue, m =>
		//    {
		//        WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
		//        where.AppendItem("O.SchemaType", PC.StandardObjectSchemaType.Applications.ToString());
		//        m.Add(where);
		//    }, codeNames);

		//    var factory = Util.GetPermissionObjectFactory();
		//    OP.IApplication[] apps = new OP.IApplication[objects.Count];
		//    int c = 0;
		//    foreach (var item in objects)
		//    {
		//        apps[c] = (OP.IApplication)factory.CreateObject(typeof(OP.IApplication));
		//        ((PermissionBridge.Objects.IBridgedObject)apps[c]).InitProperties(item);
		//        c++;
		//    }

		//    return new OP.ApplicationCollection(apps);

		//}

		///// <summary>
		///// 得到指定角色下，某些部门内的所有授权人员
		///// </summary>
		///// <param name="roles">角色集合。</param>
		///// <param name="depts">组织机构集合。</param>
		///// <param name="recursively">是否递归。</param>
		///// <returns></returns>
		//public OP.OguObjectCollection<OP.IOguObject> GetRolesObjects(OP.RoleCollection roles, OP.OguObjectCollection<OP.IOrganization> depts, bool recursively)
		//{
		//    throw new NotImplementedException();
		//}

		//public void RemoveAllCache()
		//{

		//}


		//public OP.RoleCollection GetRoles(OP.IApplication application)
		//{
		//    if (application == null)
		//        throw new ArgumentNullException("application");

		//    PC.SchemaObjectCollection roles = PC.Adapters.ReferencedSchemaObjectLoader.Instance.GetMemberObjects(DateTime.MinValue, PC.StandardObjectSchemaType.Roles.ToString(), PC.StandardObjectSchemaType.Applications.ToString(), m =>
		//    {
		//        WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
		//        where.AppendItem("P.ID", application.ID);
		//        m.Add(where);

		//    });
		//    return new OP.RoleCollection((from r in roles select ObjectToRole(r)).ToArray());

		//}

		//public OP.PermissionCollection GetPermissions(OP.IApplication application)
		//{
		//    if (application == null)
		//        throw new ArgumentNullException("application");

		//    var permissions = PC.Adapters.ReferencedSchemaObjectLoader.Instance.GetMemberObjects(DateTime.MinValue, PC.StandardObjectSchemaType.Permissions.ToString(), PC.StandardObjectSchemaType.Applications.ToString(), c =>
		//    {
		//        WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
		//        where.AppendItem("P.ID", application.ID);
		//        where.AppendItem("O.Status", (int)SchemaObjectStatus.Normal);
		//        where.AppendItem("M.Status", (int)SchemaObjectStatus.Normal);
		//        where.AppendItem("P.Status", (int)SchemaObjectStatus.Normal);

		//    });

		//    return new OP.PermissionCollection((from p in permissions select ObjectToPermission(p)).ToArray());
		//}



		//public OP.PermissionCollection GetUserPermissions(OP.IApplication application, OP.IUser user)
		//{
		//    return new OP.PermissionCollection(null);
		//}

		//public OP.RoleCollection GetUserRoles(OP.IApplication application, OP.IUser user)
		//{
		//    return new OP.RoleCollection(null);
		//}


		//public OP.ApplicationCollection GetAllApplications()
		//{
		//    throw new NotImplementedException();
		//}

		//public OP.ApplicationCollection GetApplications(params string[] codeNames)
		//{
		//    throw new NotImplementedException();
		//} 
		#endregion

		#region IPermissionMechanism
		public OP.ApplicationCollection GetAllApplications()
		{
			var timeCondition = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder();

			var where = new WhereSqlClauseBuilder();

			where.AppendItem("SchemaType", PC.StandardObjectSchemaType.Applications.ToString());
			where.AppendItem("Status", (int)SchemaObjectStatus.Normal);
			var queryResult = PC.Adapters.SchemaObjectAdapter.Instance.Load(new ConnectiveSqlClauseCollection(timeCondition, where));
			return new OP.ApplicationCollection((from a in queryResult select this.CastApplication(a)).ToArray());
		}

		public OP.ApplicationCollection GetApplications(params string[] codeNames)
		{
			var queryResult = PC.Adapters.SchemaObjectAdapter.Instance.LoadByCodeName(
				c =>
				{
					WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
					where.AppendItem("O.Status", (int)SchemaObjectStatus.Normal);
					where.AppendItem("S.Status", (int)SchemaObjectStatus.Normal);
					c.Add(where);
				}, DateTime.MinValue, codeNames);
			return new OP.ApplicationCollection((from o in queryResult select this.CastApplication(o)).ToArray());
		}

		private OP.IApplication CastApplication(PC.SchemaObjectBase o)
		{
			if (o == null)
				throw new ArgumentNullException("o");

			if (o.SchemaType != PC.StandardObjectSchemaType.Applications.ToString())
				throw new ArgumentException(string.Format("无法将{0}转换成IApplication", o), "o");

			OP.ApplicationImpl app = new OP.ApplicationImpl();

			var wrapper = (OP.IApplicationPropertyAccessible)app;

			wrapper.CodeName = o.Properties.GetValue<string>("CodeName", string.Empty);
			wrapper.Description = o.Properties.GetValue<string>("Description", string.Empty);
			wrapper.ID = o.ID;
			wrapper.Name = o.Properties.GetValue<string>("Name", string.Empty);
			wrapper.ResourceLevel = o.Properties.GetValue<string>("ResourceLevel", string.Empty);
			return app;
		}

		/// <summary>
		/// 得到指定角色下，某些部门内的所有授权人员
		/// </summary>
		/// <param name="roles">角色集合。</param>
		/// <param name="depts">组织机构集合。</param>
		/// <param name="recursively">是否递归。</param>
		/// <returns></returns>
		public OP.OguObjectCollection<OP.IOguObject> GetRolesObjects(OP.RoleCollection roles, OP.OguObjectCollection<OP.IOrganization> depts, bool recursively)
		{
			var items = PC.Adapters.SCSnapshotAdapter.Instance.QueryRolesContainsUsers(new string[] { "Roles" },(from r in roles select r.ID).ToArray(), Util.GetContextIncludeDeleted(), DateTime.MinValue);

			return new BridgedOrganizationMechanism().GetObjects<IOguObject>(SearchOUIDType.Guid, items.ToIDArray());

			//忽略depts参数
		}

		public void RemoveAllCache()
		{

		}
		#endregion

		#region IPermissionImplInterface
		public OP.RoleCollection GetUserRoles(OP.IApplication application, OP.IUser user)
		{
			bool includeDeleted = Util.GetContextIncludeDeleted();
			//var allRoles = GetRoles(application);
			var roles = PC.Adapters.SCSnapshotAdapter.Instance.QueryUserBelongToRoles(PC.SchemaInfo.FilterByCategory("Roles").ToSchemaNames(), application.CodeName, new string[] { user.ID }, false, DateTime.MinValue);
			////var relations = PC.Adapters.SCSnapshotAdapter.Instance.QueryUserBelongToContainersByIDs(new string[] { "Roles" }, new string[] { user.ID }, includeDeleted, DateTime.MinValue);

			//return new OP.RoleCollection((from role in allRoles join r in relations on role.ID equals r.ID select role).ToArray());
			return new OP.RoleCollection((from role in roles select this.CastRole(role, application)).ToArray());
		}

		public OP.PermissionCollection GetUserPermissions(OP.IApplication application, OP.IUser user)
		{
			//var allFuns = InnerGetPermission(application);

			//var allFunIds = allFuns.ToIDArray();

			bool includeDeleted = Util.GetContextIncludeDeleted();

			var appPermission = application.Permissions;

			var userAllPermissions = PC.Adapters.SCSnapshotAdapter.Instance.QueryPermissionsByUserIDs(new string[] { user.ID }, includeDeleted, DateTime.MinValue);

			return new OP.PermissionCollection((from p in userAllPermissions join appp in appPermission on p.ID equals appp.ID select this.CastPermission(p, application)).ToArray());
		}

		public OP.RoleCollection GetRoles(OP.IApplication application)
		{
			if (application == null)
				throw new ArgumentNullException("application");

			var roles = InnerGetRoles(application);

			return new OP.RoleCollection((from p in roles select this.CastRole(p, application)).ToArray());
		}

		private static PC.SCRoleCollection InnerGetRoles(OP.IApplication application)
		{
			var app = (PC.SCApplication)PC.Adapters.SchemaObjectAdapter.Instance.Load(application.ID);
			var roles = app.CurrentRoles;

			return roles;
		}

		private OP.IRole CastRole(PC.SchemaObjectBase obj, OP.IApplication app)
		{
			OP.RoleImpl role = Util.GetPermissionObjectFactory().CreateObject(typeof(OP.IRole)) as OP.RoleImpl;
			var wrapper = role as OP.IApplicationMemberPropertyAccessible;

			if (wrapper == null)
				throw new InvalidCastException("工厂创建的对象应实现IApplicationMemberPropertyAccessible，否则无法适用此工厂。");

			wrapper.CodeName = obj.Properties.GetValue<string>("CodeName", string.Empty);
			wrapper.Description = obj.Properties.GetValue<string>("Description", string.Empty);
			wrapper.ID = obj.ID;
			wrapper.Name = obj.Properties.GetValue<string>("Name", string.Empty);
			wrapper.Application = app;

			return role;
		}

		public OP.PermissionCollection GetPermissions(OP.IApplication application)
		{
			if (application == null)
				throw new ArgumentNullException("application");

			var permissions = InnerGetPermission(application);

			return new OP.PermissionCollection((from p in permissions select this.CastPermission(p, application)).ToArray());
		}

		private static PC.SCPermissionCollection InnerGetPermission(OP.IApplication application)
		{
			var permissions = ((PC.SCApplication)PC.Adapters.SchemaObjectAdapter.Instance.Load(application.ID)).CurrentPermissions;
			return permissions;
		}

		private OP.IPermission CastPermission(PC.SchemaObjectBase obj, OP.IApplication app)
		{
			OP.IPermission per = (OP.IPermission)Util.GetPermissionObjectFactory().CreateObject(typeof(OP.IPermission));
			if (per is OP.IApplicationMemberPropertyAccessible)
			{
				OP.IApplicationMemberPropertyAccessible wrapper = (OP.IApplicationMemberPropertyAccessible)per;
				wrapper.CodeName = obj.Properties.GetValue<string>("CodeName", string.Empty);
				wrapper.Description = obj.Properties.GetValue<string>("Description", string.Empty);
				wrapper.ID = obj.ID;
				wrapper.Name = obj.Properties.GetValue<string>("Name", string.Empty);
				wrapper.Application = app;
			}
			else
			{
				throw new InvalidCastException("工厂创建的对象应实现IPermissionPropertyAccessible");
			}

			return per;
		}
		#endregion
	}
}
