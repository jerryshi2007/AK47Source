using CIIC.HSR.TSP.DataAccess.Query;
using CIIC.HSR.TSP.IoC;
using CIIC.HSR.TSP.TA.Bizlet.Contract;
using CIIC.HSR.TSP.TA.BizObject;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using CIIC.HSR.TSP.WF.BizObject;
using CIIC.HSR.TSP.WF.BizObject.Exchange;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Ogu
{
	/// <summary>
	/// 权限服务
	/// </summary>
	public class ARPService : IARPService
	{
		/// <summary>
		/// 获取应用程序
		/// </summary>
		/// <returns>应用程序列表</returns>
		public DataSet GetApplications()
		{
			var appFilling = DataFillingFactory.CreateAppFilling();
			AppEntity app = new AppEntity();
			app.ID = Guid.NewGuid().ToString();
			app.Code = "App001";
			app.Name = "所有应用";
			return appFilling.Fill(new List<AppEntity>() { app });
		}

		/// <summary>
		/// 获取角色
		/// </summary>
		/// <returns>角色列表</returns>
		public System.Data.DataSet GetRoles()
		{
            ObjectServiceFactory serviceFactory = new ObjectServiceFactory(this.Context);
            var oguProvider= serviceFactory.CreatOguProvider();
            List<Role> roles = oguProvider.GetAllAARoles();

			var roleFilling = DataFillingFactory.CreateRoleFilling();
			DataSet result = roleFilling.Fill(roles);

			return result;
		}

		/// <summary>
		/// 获取所有资源
		/// </summary>
		/// <returns>资源列表</returns>
		public System.Data.DataSet GetResource()
		{
            ObjectServiceFactory serviceFactory = new ObjectServiceFactory(this.Context);
            var oguProvider = serviceFactory.CreatOguProvider();

            List<CIIC.HSR.TSP.WF.BizObject.Exchange.Resource> resources = oguProvider.GetAllResources();
			var resourceFilling = DataFillingFactory.CreateResourceFilling();
			DataSet result = resourceFilling.Fill(resources);
			return result;
		}

		/// <summary>
		/// 获取角色中的资源
		/// </summary>
		/// <param name="roleCode">角色编码</param>
		/// <returns>资源列表</returns>
		public System.Data.DataSet GetResourceInRole(string roleCode)
		{
            ObjectServiceFactory serviceFactory = new ObjectServiceFactory(this.Context);
            var oguProvider = serviceFactory.CreatOguProvider();

            List<CIIC.HSR.TSP.WF.BizObject.Exchange.Resource> resources = oguProvider.GetAllResourcesByRoleCode(roleCode);
			var resourceFilling = DataFillingFactory.CreateResourceFilling();
			DataSet result = resourceFilling.Fill(resources);
			return result;
		}

		/// <summary>
		/// 角色中的用户
		/// </summary>
		/// <param name="roleCode">角色编码</param>
		/// <returns></returns>
		public System.Data.DataSet GetUsersInRole(string roleCode)
		{
            ObjectServiceFactory serviceFactory = new ObjectServiceFactory(this.Context);
            var oguProvider = serviceFactory.CreatOguProvider();

            List<User> users = oguProvider.GetUsersByRoleCode(roleCode);
			var userFilling = DataFillingFactory.CreateUserFilling();
			DataSet result = userFilling.Fill(users);
			return result;
		}

		/// <summary>
		/// 角色中的用户
		/// </summary>
		/// <param name="roleCodes">角色编码</param>
		/// <returns></returns>
		public System.Data.DataSet GetUsersInRoles(string roleCodes)
		{
			if (string.IsNullOrEmpty(roleCodes))
			{
				return StructureBuilderFactory.CreateUserStructureBuilder().Create();
			}

            ObjectServiceFactory serviceFactory = new ObjectServiceFactory(this.Context);
            var oguProvider = serviceFactory.CreatOguProvider();

			DataSet result = new DataSet();
			var userFilling = DataFillingFactory.CreateUserFilling();
			roleCodes.Split(new[] { ',' }).ToList().ForEach(p =>
			{
                List<User> users = oguProvider.GetUsersByRoleCode(p);
				DataSet temp = userFilling.Fill(users);
				result.Merge(temp);
			});

			return result;
		}

		/// <summary>
		/// 角色中的用户
		/// </summary>
		/// <param name="userId">用户Id</param>
		/// <returns>用户列表</returns>
		public System.Data.DataSet GetRolesByUserId(string userId)
		{
            ObjectServiceFactory serviceFactory = new ObjectServiceFactory(this.Context);
            var oguProvider = serviceFactory.CreatOguProvider();

            List<Role> roles = oguProvider.GetAARoleListByUserID(userId);
			var roleFilling = DataFillingFactory.CreateRoleFilling();
			DataSet result = roleFilling.Fill(roles);

			return result;
		}

		/// <summary>
		/// 用户拥有的资源
		/// </summary>
		/// <param name="userId">用户Id</param>
		/// <returns>资源列表</returns>
		public System.Data.DataSet GetResourcesByUserId(string userId)
		{
            ObjectServiceFactory serviceFactory = new ObjectServiceFactory(this.Context);
            var oguProvider = serviceFactory.CreatOguProvider();

            List<CIIC.HSR.TSP.WF.BizObject.Exchange.Resource> resources = oguProvider.GetResourcesByUserId(userId);

			var resourceFilling = DataFillingFactory.CreateResourceFilling();
			DataSet result = resourceFilling.Fill(resources);

			return result;
		}

		/// <summary>
		/// 获取用户的角色实例
		/// </summary>
		/// <param name="userIds">用户Id列表</param>
		/// <returns>角色</returns>
		public DataSet GetGroupsOfUsers(List<string> userIds)
		{
            ObjectServiceFactory serviceFactory = new ObjectServiceFactory(this.Context);
            var oguProvider = serviceFactory.CreatOguProvider();

            List<Group> groups = oguProvider.GetGroupsOfUsers(userIds);
			var groupFilling = DataFillingFactory.CreateGroupFilling();
            DataSet result = groupFilling.Fill(groups);
			return result;
		}
		/// <summary>
		/// 获取组中的用户
		/// </summary>
		/// <param name="groupIds">组Id列表</param>
		/// <returns>用户列表</returns>
		public DataSet GetUsersOfGroups(List<string> groupIds)
		{
            ObjectServiceFactory serviceFactory = new ObjectServiceFactory(this.Context);
            var oguProvider = serviceFactory.CreatOguProvider();

            List<User> users = new List<User>();
            users = oguProvider.GetUsersOfGroups(groupIds);
			var userFilling = DataFillingFactory.CreateUserFilling();
			DataSet result = userFilling.Fill(users);
			return result;
		}
		/// <summary>
		/// 清除所有的缓存
		/// </summary>
		public void RemoveAllCache()
		{
			//OguPermissionSettings.GetConfig().OguFactory.RemoveAllCache();
		}

		/// <summary>
		/// 获取资源角色
		/// </summary>
		/// <returns>角色列表</returns>
		public DataSet GetResourceRoles(string roleCodes)
		{
            List<Role> roles = new List<Role>();
			if (!string.IsNullOrEmpty(roleCodes))
			{
                ObjectServiceFactory serviceFactory = new ObjectServiceFactory(this.Context);
                var oguProvider = serviceFactory.CreatOguProvider();
                roles = oguProvider.GetResourceRoles(roleCodes.Split(new[] { ',' }).ToList());
			}

			var roleFilling = DataFillingFactory.CreateRoleFilling();
			DataSet result = roleFilling.Fill(roles);
			return result;
		}
		/// <summary>
		/// 根据账号获取用户
		/// </summary>
		/// <param name="logonName">登录账号</param>
		/// <returns>用户</returns>
		public TranslatorUser GetUserByEmpCode(string EmpCode)
		{
			TranslatorUser result = new TranslatorUser();
            ObjectServiceFactory serviceFactory = new ObjectServiceFactory(this.Context);
            var oguProvider = serviceFactory.CreatOguProvider();
            User user = oguProvider.GetUserByEmpCode(EmpCode);
			result.UserId = user.Id.ToString();
			result.UserName = user.DisplayName;
			return result;
		}

		/// <summary>
		/// 根据登录名获取用户
		/// </summary>
		/// <param name="logonName"></param>
		/// <returns></returns>
		public TranslatorUser GetUserByLogonName(string logonName)
		{
			TranslatorUser result = new TranslatorUser();
            ObjectServiceFactory serviceFactory = new ObjectServiceFactory(this.Context);
            var oguProvider = serviceFactory.CreatOguProvider();
            User user = oguProvider.GetUserByLogonName(logonName);
			result.UserId = user.Id.ToString();
			result.UserName = user.DisplayName;
			return result;
		}

		/// <summary>
		/// 根据Path获取组织架构
		/// </summary>
		/// <param name="path">组织路径</param>
		/// <returns></returns>
		public TranslatorOrg GetDomainByPath(string path)
		{
			TranslatorOrg result = null;
            ObjectServiceFactory serviceFactory = new ObjectServiceFactory(this.Context);
            var oguProvider = serviceFactory.CreatOguProvider();
            Org org = oguProvider.GetOrgByPath(path);
			if (null!=org)
			{
				result = new TranslatorOrg();
				result.OrgId = org.Id;
				result.OrgName = org.DisplayName;
			}

            return result;
		}

		public BizObject.ServiceContext Context
		{
			get;
			set;
		}


        public DataSet GetRoot()
        {
            ObjectServiceFactory serviceFactory = new ObjectServiceFactory(this.Context);
            var oguProvider = serviceFactory.CreatOguProvider();
            Org org=oguProvider.GetRoot();
            var orgFilling=DataFillingFactory.CreateOrgFilling();
            return orgFilling.Fill(new List<Org>() { org });
        }
    }
}
