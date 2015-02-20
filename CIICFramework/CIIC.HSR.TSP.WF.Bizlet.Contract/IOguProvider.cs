using CIIC.HSR.TSP.WF.BizObject.Exchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract
{
    /// <summary>
    /// 组织、组、人员、资源数据提供接口    
    /// </summary>
    public interface IOguProvider:IRuntime
    {
        #region 权限管理
        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns>角色列表</returns>
        List<Role> GetAllAARoles();
        /// <summary>
        /// 获取所有资源
        /// </summary>
        /// <returns>资源列表</returns>
        List<Resource> GetAllResources();
        /// <summary>
        /// 根据角色获取所有的资源
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <returns>资源列表</returns>
        List<Resource> GetAllResourcesByRoleCode(string roleCode);
        /// <summary>
        /// 获取角色中所有的用户
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <returns>用户列表</returns>
        List<User> GetUsersByRoleCode(string roleCode);
        /// <summary>
        /// 获取用户所有的角色
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>角色列表</returns>
        List<Role> GetAARoleListByUserID(string userId);
        /// <summary>
        /// 获取用户拥有的资源
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>资源列表</returns>
        List<Resource> GetResourcesByUserId(string userId);
        /// <summary>
        /// 获取一组用户所属的组列表
        /// </summary>
        /// <param name="userIds">用户Id列表</param>
        /// <returns>组列表</returns>
        List<Group> GetGroupsOfUsers(List<string> userIds);
        /// <summary>
        /// 获取一些列组中的所有用户
        /// </summary>
        /// <param name="groupIds">组Id列表</param>
        /// <returns>用户列表</returns>
        List<User> GetUsersOfGroups(List<string> groupIds);
        /// <summary>
        /// 获取一组资源对应的角色
        /// </summary>
        /// <param name="resourceCodes">资源编码</param>
        /// <returns>角色列表</returns>
        List<Role> GetResourceRoles(List<string> resourceCodes);
        /// <summary>
        /// 根据雇员编码获取用户
        /// </summary>
        /// <param name="EmpCode">雇员号</param>
        /// <returns>用户</returns>
        User GetUserByEmpCode(string EmpCode);
        /// <summary>
        /// 根据登陆名获取用户
        /// </summary>
        /// <param name="logonName"></param>
        /// <returns></returns>
        User GetUserByLogonName(string logonName);
        /// <summary>
        /// 根据路经获取组织
        /// </summary>
        /// <param name="path">路径，以","号分割的OrgId</param>
        Org GetOrgByPath(string path);
        /// <summary>
        /// 获取组织代码
        /// </summary>
        /// <returns></returns>
        Org GetRoot();
        #endregion
        #region 组织
        /// <summary>
        /// 获取对象的详细信息
        /// </summary>
        /// <param name="objIds">对象的Id列表</param>
        /// <returns>对象详细信息列表</returns>
        List<Org> GetOrgDetail(List<string> objIds);
        /// <summary>
        /// 获取某个节点下的所有子组织
        /// </summary>
        /// <param name="parentIds">节点列表</param>
        /// <param name="recursive">是否深度查询</param>
        /// <returns>所有子组织</returns>
        List<Org> GetOrgChildren(List<string> parentIds, bool recursive);
        /// <summary>
        /// 搜索子组织
        /// </summary>
        /// <param name="parentIds">节点列表</param>
        /// <param name="objName">子组织名称</param>
        /// <param name="fuzzy">是否模糊查询</param>
        /// <param name="recursive">是否深度查询</param>
        /// <param name="limitRows">行数</param>
        /// <returns>所有子组织</returns>
        List<Org> SearchOrg(List<string> parentIds, string objName, bool fuzzy, bool recursive, int limitRows);
        #endregion
        #region 用户
        /// <summary>
        /// 获取用户的详细信息
        /// </summary>
        /// <param name="objIds">组织ID</param>
        /// <param name="recursive">是否</param>
        /// <returns>用户详细信息列表</returns>
        List<User> GetUserDetail(List<string> objIds);
        /// <summary>
        /// 获取某个节点下的所有子用户
        /// </summary>
        /// <param name="parentIds">组织节点列表</param>
        /// <param name="recursive">是否深度查询</param>
        /// <returns>所有子用户</returns>
        List<User> GetUserChildren(List<string> parentIds, bool recursive);
        /// <summary>
        /// 搜索子用户
        /// </summary>
        /// <param name="parentIds">组织节点列表</param>
        /// <param name="objName">用户名称</param>
        /// <param name="fuzzy">是否模糊查询</param>
        /// <param name="recursive">是否深度查询</param>
        /// <param name="limitRows">行数</param>
        /// <returns>所有子用户</returns>
        List<User> SearchUser(List<string> parentIds, string objName, bool fuzzy, bool recursive, int limitRows);
        #endregion
        #region 组
        /// <summary>
        /// 获取组的详细信息
        /// </summary>
        /// <param name="objIds">组的Id列表</param>
        /// <param name="recursive">是否</param>
        /// <returns>组详细信息列表</returns>
        List<Group> GetGroupDetail(List<string> objIds);
        /// <summary>
        /// 获取某个节点下的所有子组
        /// </summary>
        /// <param name="parentIds">组织节点列表</param>
        /// <param name="recursive">是否深度查询</param>
        /// <returns>所有组</returns>
        List<Group> GetGroupChildren(List<string> parentIds, bool recursive);
        /// <summary>
        /// 搜索组
        /// </summary>
        /// <param name="parentIds">组织节点列表</param>
        /// <param name="objName">组名称</param>
        /// <param name="fuzzy">是否模糊查询</param>
        /// <param name="recursive">是否深度查询</param>
        /// <param name="limitRows">行数</param>
        /// <returns>所有组</returns>
        List<Group> SearchGroup(List<string> parentIds, string objName, bool fuzzy, bool recursive, int limitRows);
        #endregion
    }
}
