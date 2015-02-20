using CIIC.HSR.TSP.DataAccess.Query;
using CIIC.HSR.TSP.IoC;
using CIIC.HSR.TSP.TA.Bizlet.Contract;
using CIIC.HSR.TSP.TA.BizObject;
using CIIC.HSR.TSP.WF.BizObject;
using CIIC.HSR.TSP.WF.PermissionManager.Storage;
using MCS.Library.OGUPermission;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.PermissionManager.ObjectDetail
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
			var iRole = Containers.Default.Resolve<IWorkflowEngineBizlet>();
			List<AARoleBO> roles = iRole.GetAllAARoles();
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
			var iResource = Containers.Default.Resolve<IWorkflowEngineBizlet>();
			List<AAResourceBO> resources = iResource.GetAllResources();
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
			var iResource = Containers.Default.Resolve<IWorkflowEngineBizlet>();
			List<AAResourceBO> resources = iResource.GetAllResourcesByRoleCode(roleCode);
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
			var iUser = Containers.Default.Resolve<IWorkflowEngineBizlet>();
			List<AAUserBO> users = iUser.GetAAUserListByRoleCode(roleCode);
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

			DataSet result = new DataSet();
			var userFilling = DataFillingFactory.CreateUserFilling();
			roleCodes.Split(new[] { ',' }).ToList().ForEach(p =>
			{
				var iUser = Containers.Default.Resolve<IWorkflowEngineBizlet>();
				List<AAUserBO> users = iUser.GetAAUserListByRoleCode(p);
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
			var iRole = Containers.Default.Resolve<IWorkflowEngineBizlet>();

			List<AARoleBO> roles = iRole.GetAARoleListByUserID(new Guid(userId));
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
			var iResource = Containers.Default.Resolve<IResourcePermissionBizlet>();

			List<AAResourceBO> resources = iResource.GetAuthorizedReourcesByType(ResourceTypes.All, new Guid(userId));

			var resourceFilling = DataFillingFactory.CreateResourceFilling();
			DataSet result = resourceFilling.Fill(resources);

			return result;
		}

		/// <summary>
		/// 获取用户的角色实例
		/// </summary>
		/// <param name="userIds">用户Id列表</param>
		/// <returns>角色</returns>
		public DataSet GetGroupsOfUsers(List<Guid> userIds)
		{
			List<AADomainRoleBO> domainRoles = new List<AADomainRoleBO>();
			var iDomainRole = Containers.Default.Resolve<IWorkflowEngineBizlet>();
			userIds.ForEach(p =>
			{
				List<AADomainRoleBO> userDomainRoles = iDomainRole.GetDomainRoles(p);
				if (null != userDomainRoles)
				{
					domainRoles.AddRange(userDomainRoles);
				}
			});

			var groupFilling = DataFillingFactory.CreateGroupFilling();
			DataSet result = groupFilling.Fill(domainRoles);
			return result;
		}
		/// <summary>
		/// 获取组中的用户
		/// </summary>
		/// <param name="groupIds">组Id列表</param>
		/// <returns>用户列表</returns>
		public DataSet GetUsersOfGroups(List<Guid> groupIds)
		{
			List<AAUserBO> users = new List<AAUserBO>();
			var iDomainRole = Containers.Default.Resolve<IWorkflowEngineBizlet>();
			groupIds.ForEach(p =>
			{
				List<AAUserBO> domainUsers = iDomainRole.GetAAUserListByDomainRoleId(p);
				if (null != domainUsers)
				{
					users.AddRange(domainUsers);
				}
			});

			var userFilling = DataFillingFactory.CreateUserFilling();
			DataSet result = userFilling.Fill(users);
			return result;
		}
		/// <summary>
		/// 清除所有的缓存
		/// </summary>
		public void RemoveAllCache()
		{
			OguPermissionSettings.GetConfig().OguFactory.RemoveAllCache();
		}

		/// <summary>
		/// 获取资源角色
		/// </summary>
		/// <returns>角色列表</returns>
		public DataSet GetResourceRoles(string roleCodes)
		{
			List<AARoleBO> roles = new List<AARoleBO>();
			if (!string.IsNullOrEmpty(roleCodes))
			{
				var iRoleBiz = Containers.Default.Resolve<IWorkflowEngineBizlet>();
				roles = iRoleBiz.GetAARoleListByResourceCodes(roleCodes.Split(new[] { ',' }).ToList());
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
			var iResource = Containers.Default.Resolve<IWorkflowEngineBizlet>();
			AAUserBO user = iResource.GetUserByEmpCode(EmpCode);
			result.UserId = user.UserId.ToString();
			result.UserName = user.UserName;
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
			var iResource = Containers.Default.Resolve<IPermissionManagementBizlet>();
			AAUserBO user = iResource.GetAAUserByAccount(logonName);
			result.UserId = user.UserId.ToString();
			result.UserName = user.UserName;
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
			var iRoleBiz = Containers.Default.Resolve<IWorkflowEngineBizlet>();
			List<AADomainBO> domains = iRoleBiz.GetAADomainListByPaths(new List<string>() { path });
			if (null != domains && domains.Count > 0)
			{
				result = new TranslatorOrg();
				result.OrgId = domains[0].DomainId.ToString();
				result.OrgName = domains[0].LabelNameCn;

				return result;
			}
			else
			{
				return result;
			}
		}

		public BizObject.ServiceContext Context
		{
			get;
			set;
		}
	}
}
