#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.OGUPermission
// FileName	：	AppAdminMechanism.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    沈峥	    20070430		创建
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.OGUPermission.Properties;

namespace MCS.Library.OGUPermission
{
	internal sealed class AppAdminMechanism : IPermissionMechanism, IPermissionImplInterface
	{
		public static readonly AppAdminMechanism Instance = new AppAdminMechanism();

		private AppAdminMechanism()
		{
		}

		#region IPermissionMechanism 成员

		public ApplicationCollection GetAllApplications()
		{
			DataTable table = AppAdminServiceBroker.Instance.GetApplications().Tables[0];

			return new ApplicationCollection(BuildObjectsFromTable<IApplication>(table));
		}

		public ApplicationCollection GetApplications(params string[] codeNames)
		{
			if (codeNames == null)
				throw new ArgumentNullException("codeNames");

			List<IApplication> result = new List<IApplication>();

			if (codeNames.Length > 0)
			{
				DataTable table = AppAdminServiceBroker.Instance.GetApplications().Tables[0];
				List<IApplication> listApp = BuildObjectsFromTable<IApplication>(table);

				foreach (IApplication app in listApp)
				{
					if (Array.Exists<string>(codeNames, delegate(string name)
					{
						return string.Compare(app.CodeName, name, true) == 0;
					}))
					{
						result.Add(app);
					}
				}
			}

			return new ApplicationCollection(result);
		}

		public OguObjectCollection<IOguObject> GetRolesObjects(RoleCollection roles, OguObjectCollection<IOrganization> depts, bool recursively)
		{
			OguObjectCollection<IOguObject> result = null;

			if (roles.Count > 0)
			{
				string roleIDs = BuildRoleObjectIDs(roles);
				string deptFullPath = BuildOguObjectFullPath(depts);

				DataTable table = null;

				if (recursively)
					table = AppAdminServiceBroker.Instance.GetRolesUsers(
									 deptFullPath,
									 roles[0].Application.CodeName,
									 roleIDs,
									 DelegationMaskType.All,
									 SidelineMaskType.All,
									 Common.DefaultAttrs).Tables[0];
				else
					table = AppAdminServiceBroker.Instance.GetChildrenInRoles(
									deptFullPath,
									roles[0].Application.CodeName,
									roleIDs,
									false,
									true,
									true).Tables[0];

				result = new OguObjectCollection<IOguObject>(Common.BuildObjectsFromTable<IOguObject>(table));
			}
			else
				result = new OguObjectCollection<IOguObject>(new List<IOguObject>());

			return result;
		}

		/// <summary>
		/// 清除所有的缓存
		/// </summary>
		public void RemoveAllCache()
		{
			AppAdminServiceBroker.Instance.RemoveAllCache();
		}
		#endregion

		#region IPermissionImplInterface
		public RoleCollection GetUserRoles(IApplication application, IUser user)
		{
			application.NullCheck("application");
			user.NullCheck("user");

			//原来是按照ID进行权限判断，现在改成根据配置文件决定（沈峥）
			string userID = user.ID;
			UserValueType valueType = UserValueType.Guid;

            if (OguPermissionSettings.GetConfig().RoleRelatedUserParentDept && user.FullPath.IsNotEmpty())
			{
				userID = user.FullPath;
				valueType = UserValueType.AllPath;
			}

			DataTable table = AppAdminServiceBroker.Instance.GetUserRoles(
										userID,
                                        application.CodeName,
										valueType,
										RightMaskType.App,
										DelegationMaskType.All).Tables[0];

			RoleCollection roles = new RoleCollection(BuildObjectsFromTable<IRole>(table));

            if (application != null)
                roles.ForEach(r => ((RoleImpl)r).Application = application);

			return roles;
		}

        public List<IRole> GetAllUserRoles(IUser user)
        {
            user.NullCheck("user");

            //原来是按照ID进行权限判断，现在改成根据配置文件决定（沈峥）
            string userID = user.ID;
            UserValueType valueType = UserValueType.Guid;

            if (OguPermissionSettings.GetConfig().RoleRelatedUserParentDept)
            {
                userID = user.FullPath;
                valueType = UserValueType.AllPath;
            }

            DataTable table = AppAdminServiceBroker.Instance.GetUserRoles(
                                        userID,
                                        string.Empty,
                                        valueType,
                                        RightMaskType.App,
                                        DelegationMaskType.All).Tables[0];

            return BuildObjectsFromTable<IRole>(table);
        }

		public PermissionCollection GetUserPermissions(IApplication application, IUser user)
		{
			application.NullCheck("application");
			user.NullCheck("user");

			DataTable table = AppAdminServiceBroker.Instance.GetUserPermissions(
										user.ID,
										application.CodeName,
										UserValueType.Guid,
										RightMaskType.App,
										DelegationMaskType.All).Tables[0];

			PermissionCollection permissions = new PermissionCollection(BuildObjectsFromTable<IPermission>(table));

			foreach (PermissionImpl permission in permissions)
				permission.Application = application;

			return permissions;
		}

		public RoleCollection GetRoles(IApplication application)
		{
			application.NullCheck("application");

			DataTable table = AppAdminServiceBroker.Instance.GetRoles(application.CodeName, RightMaskType.App).Tables[0];

			RoleCollection roles = new RoleCollection(BuildObjectsFromTable<IRole>(table));

			foreach (RoleImpl role in roles)
				role.Application = application;

			return roles;
		}

		public PermissionCollection GetPermissions(IApplication application)
		{
			application.NullCheck("application");

			DataTable table = AppAdminServiceBroker.Instance.GetFunctions(application.CodeName, RightMaskType.App).Tables[0];

			PermissionCollection permissions = new PermissionCollection(BuildObjectsFromTable<IPermission>(table));

			foreach (PermissionImpl permission in permissions)
				permission.Application = application;

			return permissions;
		}
		#endregion IPermissionImplInterface

		private static string BuildRoleObjectIDs(IEnumerable<IRole> objs)
		{
			StringBuilder strB = new StringBuilder();

			foreach (IRole obj in objs)
			{
				if (strB.Length > 0)
					strB.Append(",");

				strB.Append(obj.CodeName);
			}

			return strB.ToString();
		}

		private static string BuildOguObjectFullPath(IEnumerable<IOrganization> objs)
		{
			StringBuilder strB = new StringBuilder();

			foreach (IOrganization obj in objs)
			{
				if (strB.Length > 0)
					strB.Append(",");

				strB.Append(obj.FullPath);
			}

			return strB.ToString();
		}

		private static List<T> BuildObjectsFromTable<T>(DataTable table) where T : IPermissionObject
		{
			List<T> list = new List<T>();

			foreach (DataRow row in table.Rows)
			{
				IPermissionObject baseObject = OguPermissionSettings.GetConfig().PermissionObjectFactory.CreateObject(typeof(T));

				if (baseObject is PermissionObjBaseImpl)
				{
					PermissionObjBaseImpl oBase = (PermissionObjBaseImpl)baseObject;
					oBase.InitProperties(row);

					list.Add((T)(oBase as object));
				}
			}

			return list;
		}

		private static PermissionObjBaseImpl CreateObjectByType(System.Type type)
		{
			PermissionObjBaseImpl result = null;

			if (type == typeof(IApplication))
				result = new ApplicationImpl();
			else
				if (type == typeof(IRole))
					result = new RoleImpl();
				else
					if (type == typeof(IPermission))
						result = new PermissionImpl();
					else
						throw new InvalidOperationException(string.Format(Resource.InvalidObjectTypeCreation, type.ToString()));

			return result;
		}
	}
}
