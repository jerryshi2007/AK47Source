using CIIC.HSR.TSP.WF.Bizlet.Contract;
using CIIC.HSR.TSP.WF.BizObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract
{
    /// <summary>
    /// 权限接口
    /// </summary>
    public interface IARPService:IRuntime
    {
        /// <summary>
        /// 获取应用程序
        /// </summary>
        /// <returns>应用程序列表</returns>
        DataSet GetApplications();
        /// <summary>
        /// 获取角色
        /// </summary>
        /// <returns>角色列表</returns>
        DataSet GetRoles();
        /// <summary>
        /// 获取所有资源
        /// </summary>
        /// <returns>资源列表</returns>
        DataSet GetResource();
        /// <summary>
        /// 获取资源角色
        /// </summary>
        /// <returns>角色</returns>
        DataSet GetResourceRoles(string roleCodes);
        /// <summary>
        /// 获取角色中的资源
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <returns>资源列表</returns>
        DataSet GetResourceInRole(string roleCode);
        /// <summary>
        /// 角色中的用户
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <returns></returns>
        DataSet GetUsersInRole(string roleCode);
        /// <summary>
        /// 角色中的用户
        /// </summary>
        /// <param name="roleCodes">角色编码</param>
        /// <returns></returns>
        DataSet GetUsersInRoles(string roleCodes);
        /// <summary>
        /// 角色中的用户
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>用户列表</returns>
        DataSet GetRolesByUserId(string userId);
        /// <summary>
        /// 用户拥有的资源
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>资源列表</returns>
        DataSet GetResourcesByUserId(string userId);
        /// <summary>
        /// 根据雇员号获取用户
        /// </summary>
        /// <param name="EmpCode">雇员号</param>
        /// <returns>用户</returns>
        TranslatorUser GetUserByEmpCode(string EmpCode);
        /// <summary>
        /// 根据账号获取用户
        /// </summary>
        /// <param name="logonName">登录账号</param>
        /// <returns>用户</returns>
        TranslatorUser GetUserByLogonName(string logonName);
        /// <summary>
        /// 获取用户的角色实例
        /// </summary>
        /// <param name="userIds">用户Id列表</param>
        /// <returns>角色</returns>
        DataSet GetGroupsOfUsers(List<string> userIds);
        /// <summary>
        /// 获取组中的用户
        /// </summary>
        /// <param name="groupIds">组Id列表</param>
        /// <returns>用户列表</returns>
        DataSet GetUsersOfGroups(List<string> groupIds);
        /// <summary>
        /// 清除所有的缓存
        /// </summary>
        void RemoveAllCache();
        /// <summary>
        /// 获取组织根据Path
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>组织</returns>
        TranslatorOrg GetDomainByPath(string path);
        /// <summary>
        /// 获取根节点
        /// </summary>
        /// <returns></returns>
        DataSet GetRoot();
    }
}
