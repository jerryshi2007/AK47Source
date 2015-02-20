using CIIC.HSR.TSP.IoC;
using CIIC.HSR.TSP.TA.Bizlet.Contract;
using CIIC.HSR.TSP.TA.BizObject;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using CIIC.HSR.TSP.WF.BizObject.Exchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Ogu
{
    public class OguOmpProvider : IOguProvider
    {
        #region 权限管理
        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns>角色列表</returns>
        public List<Role> GetAllAARoles()
        {
            List<Role> result = new List<Role>();
            var iRole = Containers.Default.Resolve<IWorkflowEngineBizlet>();
            List<AARoleBO> roles = iRole.GetAllAARoles();
            if (null != roles && roles.Count > 0)
            {
                roles.ForEach(p => result.Add(new Role()
                {
                    Code = p.RoleCode,
                    Description = p.LabelDescription,
                    ID = p.RoleId.ToString(),
                    Name = p.LabelNameCn
                }));
            }
            return result;
        }
        /// <summary>
        /// 获取所有资源
        /// </summary>
        /// <returns>资源列表</returns>
        public List<CIIC.HSR.TSP.WF.BizObject.Exchange.Resource> GetAllResources()
        {
            List<CIIC.HSR.TSP.WF.BizObject.Exchange.Resource> result = new List<CIIC.HSR.TSP.WF.BizObject.Exchange.Resource>();
            var iResource = Containers.Default.Resolve<IWorkflowEngineBizlet>();
            List<AAResourceBO> resources = iResource.GetAllResources();
            if (null != resources && resources.Count > 0)
            {
                resources.ForEach(p => result.Add(TranslateResource(p)));
            }
            return result;
        }
        /// <summary>
        /// 根据角色获取所有的资源
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <returns>资源列表</returns>
        public List<CIIC.HSR.TSP.WF.BizObject.Exchange.Resource> GetAllResourcesByRoleCode(string roleCode)
        {
            List<CIIC.HSR.TSP.WF.BizObject.Exchange.Resource> result = new List<CIIC.HSR.TSP.WF.BizObject.Exchange.Resource>();
            var iResource = Containers.Default.Resolve<IWorkflowEngineBizlet>();
            List<AAResourceBO> resources = iResource.GetAllResourcesByRoleCode(roleCode);
            if (null != resources && resources.Count > 0)
            {
                resources.ForEach(p => result.Add(TranslateResource(p)));
            }
            return result;
        }
        /// <summary>
        /// 获取角色中所有的用户
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <returns>用户列表</returns>
        public List<User> GetUsersByRoleCode(string roleCode)
        {
            List<User> result = new List<User>();
            var iUser = Containers.Default.Resolve<IWorkflowEngineBizlet>();
            List<AAUserBO> users = iUser.GetAAUserListByRoleCode(roleCode);
            if (null != users && users.Count > 0)
            {
                users.ForEach(p =>
                {
                    if (null != p.AADomainUserBOes && p.AADomainUserBOes.Count > 0)
                    {
                        p.AADomainUserBOes.ToList().ForEach(d =>
                            result.Add(TranslateUser(p, d.DomainId.ToString()))
                        );
                    }
                    else
                    {
                        result.Add(TranslateUser(p));
                    }
                });
            }
            return result;
        }
        /// <summary>
        /// 获取用户所有的角色
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>角色列表</returns>
        public List<Role> GetAARoleListByUserID(string userId)
        {
            List<Role> result = new List<Role>();
            var iRole = Containers.Default.Resolve<IWorkflowEngineBizlet>();
            List<AARoleBO> roles = iRole.GetAARoleListByUserID(new Guid(userId));
            if (null != roles && roles.Count > 0)
            {
                roles.ForEach(p => result.Add(TranslateRole(p)));
            }
            return result;
        }
        /// <summary>
        /// 获取用户拥有的资源
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>资源列表</returns>
        public List<CIIC.HSR.TSP.WF.BizObject.Exchange.Resource> GetResourcesByUserId(string userId)
        {
            List<CIIC.HSR.TSP.WF.BizObject.Exchange.Resource> result = new List<CIIC.HSR.TSP.WF.BizObject.Exchange.Resource>();
            var iResource = Containers.Default.Resolve<IResourcePermissionBizlet>();
            List<AAResourceBO> resources = iResource.GetAuthorizedReourcesByType(ResourceTypes.All, new Guid(userId));
            if (null != resources && resources.Count > 0)
            {
                resources.ForEach(p => result.Add(TranslateResource(p)));
            }
            return result;
        }
        /// <summary>
        /// 获取一组用户所属的组列表
        /// </summary>
        /// <param name="userIds">用户Id列表</param>
        /// <returns>组列表</returns>
        public List<Group> GetGroupsOfUsers(List<string> userIds)
        {
            List<Group> result = new List<Group>();
            var iDomainRole = Containers.Default.Resolve<IWorkflowEngineBizlet>();
            userIds.ForEach(p =>
            {
                List<AADomainRoleBO> userDomainRoles = iDomainRole.GetDomainRoles(new Guid(p));
                if (null != userDomainRoles)
                {
                    userDomainRoles.ForEach(d => result.Add(TranlateGroup(d)));
                }
            });

            return result;
        }
        /// <summary>
        /// 获取一些列组中的所有用户
        /// </summary>
        /// <param name="groupIds">组Id列表</param>
        /// <returns>用户列表</returns>
        public List<User> GetUsersOfGroups(List<string> groupIds)
        {
            List<User> result = new List<User>();
            var iUserBizlet = Containers.Default.Resolve<IWorkflowEngineBizlet>();
            groupIds.ForEach(p =>
            {
                List<AAUserBO> domainUsers = iUserBizlet.GetAAUserListByDomainRoleId(new Guid(p));
                if (null != domainUsers)
                {
                    domainUsers.ForEach(g =>
                    {
                        var domainRoles = iUserBizlet.GetAADomainRoleListByIds(new List<Guid> { new Guid(p) });
                        if (domainRoles != null && domainRoles.Count == 1)
                        {
                            result.Add(TranslateUser(g, domainRoles[0].DomainId.ToString()));
                        }
                    });
                }
            });

            return result;
        }
        /// <summary>
        /// 获取一组资源对应的角色
        /// </summary>
        /// <param name="resourceCodes">资源编码</param>
        /// <returns>角色列表</returns>
        public List<Role> GetResourceRoles(List<string> resourceCodes)
        {
            List<Role> result = new List<Role>();
            if (null != resourceCodes && resourceCodes.Count > 0)
            {
                var iRoleBiz = Containers.Default.Resolve<IWorkflowEngineBizlet>();
                var roles = iRoleBiz.GetAARoleListByResourceCodes(resourceCodes);

                if (null != roles)
                {
                    roles.ForEach(p => result.Add(TranslateRole(p)));
                }
            }
            return result;
        }
        /// <summary>
        /// 根据雇员编码获取用户
        /// </summary>
        /// <param name="EmpCode">雇员号</param>
        /// <returns>用户</returns>
        public User GetUserByEmpCode(string EmpCode)
        {
            User result = new User();
            var iResource = Containers.Default.Resolve<IWorkflowEngineBizlet>();
            AAUserBO user = iResource.GetUserByEmpCode(EmpCode);
            result = TranslateUser(user);
            return result;
        }
        /// <summary>
        /// 根据登陆名获取用户
        /// </summary>
        /// <param name="logonName"></param>
        /// <returns></returns>
        public User GetUserByLogonName(string logonName)
        {
            User result = new User();
            var iResource = Containers.Default.Resolve<IPermissionManagementBizlet>();
            AAUserBO user = iResource.GetAAUserByAccount(logonName);
            result = TranslateUser(user);
            return result;
        }
        /// <summary>
        /// 根据路经获取组织
        /// </summary>
        /// <param name="path">路径，以","号分割的OrgId</param>
        public Org GetOrgByPath(string path)
        {
            Org result = null;
            var iRoleBiz = Containers.Default.Resolve<IWorkflowEngineBizlet>();
            List<AADomainBO> domains = iRoleBiz.GetAADomainListByPaths(new List<string>() { path });
            if (null != domains && domains.Count > 0)
            {
                result = TranslateOrg(domains[0]);
            }

            return result;
        }
        #endregion
        #region 组织
        /// <summary>
        /// 获取对象的详细信息
        /// </summary>
        /// <param name="objIds">对象的Id列表</param>
        /// <returns>对象详细信息列表</returns>
        public List<Org> GetOrgDetail(List<string> objIds)
        {
            return GetData<Org, AADomainBO, IWorkflowEngineBizlet>(objIds, (p, t) =>
            {
                return t.GetAADomainListByIds(p);
            }
            , t => TranslateOrg(t));
        }
        /// <summary>
        /// 获取某个节点下的所有子组织
        /// </summary>
        /// <param name="parentIds">节点列表</param>
        /// <param name="recursive">是否深度查询</param>
        /// <returns>所有子组织</returns>
        public List<Org> GetOrgChildren(List<string> parentIds, bool recursive)
        {
            return GetData<Org, AADomainBO, IWorkflowEngineBizlet>(parentIds, (p, t) =>
            {
                return t.GetDeepAADomainListByParentIds(p, recursive);
            }
            , t => TranslateOrg(t));
        }
        /// <summary>
        /// 搜索子组织
        /// </summary>
        /// <param name="parentIds">节点列表</param>
        /// <param name="objName">子组织名称</param>
        /// <param name="fuzzy">是否模糊查询</param>
        /// <param name="recursive">是否深度查询</param>
        /// <param name="limitRows">行数</param>
        /// <returns>所有子组织</returns>
        public List<BizObject.Exchange.Org> SearchOrg(List<string> parentIds, string objName, bool fuzzy, bool recursive, int limitRows)
        {
            return GetData<Org, AADomainBO, IWorkflowEngineBizlet>(parentIds, (p, t) =>
            {
                return t.GetDeepAADomainListByParentIds(p, recursive, limitRows, objName, fuzzy);
            }
            , t => TranslateOrg(t));
        }
        #endregion
        #region 用户
        /// <summary>
        /// 获取用户的详细信息
        /// </summary>
        /// <param name="objIds">用户的Id列表</param>
        /// <param name="recursive">是否</param>
        /// <returns>用户详细信息列表</returns>
        public List<User> GetUserDetail(List<string> objIds)
        {
            return GetData<User, AAUserBO, IWorkflowEngineBizlet>(objIds
                , (p, t) => t.GetAAUserListByIds(p)
                , t => TranslateUser(t));
        }
        /// <summary>
        /// 获取某个节点下的所有子用户
        /// </summary>
        /// <param name="parentIds">组织节点列表</param>
        /// <param name="recursive">是否深度
        public List<User> GetUserChildren(List<string> parentIds, bool recursive)
        {
            return GetData<User, AAUserBO, IWorkflowEngineBizlet>(parentIds
                , (p, t) => t.GetAAUserListByDomainIds(p, recursive, null)
                , t => TranslateUser(t));
        }
        //// <summary>
        /// 搜索子用户
        /// </summary>
        /// <param name="parentIds">组织节点列表</param>
        /// <param name="objName">用户名称</param>
        /// <param name="fuzzy">是否模糊查询</param>
        /// <param name="recursive">是否深度查询</param>
        /// <param name="limitRows">行数</param>
        /// <returns>所有子用户</returns>
        public List<User> SearchUser(List<string> parentIds, string objName, bool fuzzy, bool recursive, int limitRows)
        {
            return GetData<User, AAUserBO, IWorkflowEngineBizlet>(parentIds
                , (p, t) => t.GetAAUserListByDomainIds(p, recursive, limitRows, objName, fuzzy)
                , t => TranslateUser(t));
        }
        #endregion
        #region 组
        /// <summary>
        /// 获取用户的详细信息
        /// </summary>
        /// <param name="objIds">用户的Id列表</param>
        /// <param name="recursive">是否</param>
        /// <returns>用户详细信息列表</returns>
        public List<Group> GetGroupDetail(List<string> objIds)
        {
            return GetData<Group, AADomainRoleBO, IWorkflowEngineBizlet>(objIds
                , (p, t) => t.GetAADomainRoleListByIds(p)
                , t => TranlateGroup(t));
        }
        // <summary>
        /// 获取某个节点下的所有子用户
        /// </summary>
        /// <param name="parentIds">组织节点列表</param>
        /// <param name="recursive">是否深度查询</param>
        /// <returns>所有子用户</returns>
        public List<Group> GetGroupChildren(List<string> parentIds, bool recursive)
        {
            return GetData<Group, AADomainRoleBO, IWorkflowEngineBizlet>(parentIds
                , (p, t) => t.GetAADomainRoleListByDomainIds(p, recursive, null)
                , t => TranlateGroup(t));
        }
        /// <summary>
        /// 搜索子用户
        /// </summary>
        /// <param name="parentIds">组织节点列表</param>
        /// <param name="objName">用户名称</param>
        /// <param name="fuzzy">是否模糊查询</param>
        /// <param name="recursive">是否深度查询</param>
        /// <param name="limitRows">行数</param>
        /// <returns>所有子用户</returns>
        public List<Group> SearchGroup(List<string> parentIds, string objName, bool fuzzy, bool recursive, int limitRows)
        {
            int? limitCount = null;
            if (-1 != limitRows)
            {
                limitCount = limitRows;
            }

            return GetData<Group, AADomainRoleBO, IWorkflowEngineBizlet>(parentIds
                , (p, t) => t.GetAADomainRoleListByDomainIds(p, recursive, limitCount, objName, fuzzy)
                , t => TranlateGroup(t));
        }
        #endregion
        public BizObject.ServiceContext Context
        {
            get;
            set;
        }
        #region 私有
        /// <summary>
        /// 将字符串转换为GUID
        /// </summary>
        /// <param name="guids">字符串ID列表</param>
        /// <returns>Guid列表</returns>
        private List<Guid> TransformStringToGuid(List<string> guids)
        {
            List<Guid> transformedGuids = new List<Guid>();
            if (null != guids && guids.Count > 0)
            {
                guids.ForEach(p => transformedGuids.Add(new Guid(p)));
            }

            return transformedGuids;
        }
        /// <summary>
        /// 将权限管理中的用户转换为内部User对象
        /// </summary>
        /// <param name="aaUserBo">管理中的用户</param>
        /// <returns>内部User对象</returns>
        private User TranslateUser(AAUserBO aaUserBo)
        {
            User result = new User();
            result.Code = aaUserBo.Account;
            result.Description = aaUserBo.Description;
            result.DisplayName = aaUserBo.UserName;
            result.Email = aaUserBo.Email;
            result.Id = aaUserBo.UserId.ToString();
            result.LogonName = aaUserBo.Account;
            result.Mobile = aaUserBo.Mobile;
            return result;
        }
        /// <summary>
        /// 将权限管理中的用户转换为内部User对象,并同时设置所属的组织
        /// </summary>
        /// <param name="aaUserBo">管理中的用户</param>
        /// <param name="parentOrgId">组织ID</param>
        /// <returns>内部User对象</returns>
        private User TranslateUser(AAUserBO aaUserBo, string parentOrgId)
        {
            var translatedUser = TranslateUser(aaUserBo);
            translatedUser.ParentOrgId = parentOrgId;
            return translatedUser;
        }
        /// <summary>
        /// 将权限管理中心的资源转换为内部资源对象
        /// </summary>
        /// <param name="aaResourceBo">权限管理中心的资源</param>
        /// <returns>内部资源对象</returns>
        private CIIC.HSR.TSP.WF.BizObject.Exchange.Resource TranslateResource(AAResourceBO aaResourceBo)
        {
            CIIC.HSR.TSP.WF.BizObject.Exchange.Resource resource = new CIIC.HSR.TSP.WF.BizObject.Exchange.Resource()
            {
                Code = aaResourceBo.ResourceCode,
                Description = aaResourceBo.LabelDescription,
                Id = aaResourceBo.ResourceID.ToString(),
                Name = aaResourceBo.LabelNameCn
            };

            return resource;
        }
        /// <summary>
        /// 将权限管理中心的角色转换为内部角色对象
        /// </summary>
        /// <param name="aaRoleBo">权限管理中心的角色</param>
        /// <returns>内部角色对象</returns>
        private Role TranslateRole(AARoleBO aaRoleBo)
        {
            return new Role()
            {
                Code = aaRoleBo.RoleCode,
                Description = aaRoleBo.LabelDescription,
                ID = aaRoleBo.RoleId.ToString(),
                Name = aaRoleBo.LabelNameCn
            };
        }
        /// <summary>
        /// 将权限管理中心的角色实例转换为内部的组对象
        /// </summary>
        /// <param name="aaDomainRoleBo">权限管理中心的角色实例</param>
        /// <returns>内部的组对象</returns>
        private Group TranlateGroup(AADomainRoleBO aaDomainRoleBo)
        {
            return new Group()
            {
                CodeName = string.Format("{0}_{1}",aaDomainRoleBo.AADomain.DomainCode, aaDomainRoleBo.AARole.RoleCode),
                Description = string.Format("{0}", aaDomainRoleBo.AARole.LabelNameCn),
                DisplayName = string.Format("{0}", aaDomainRoleBo.AARole.LabelNameCn),
                Id = aaDomainRoleBo.DomainRoleId.ToString(),
                ParentOrgId = aaDomainRoleBo.DomainId.ToString()
            };
        }
        /// <summary>
        /// 将管理中心的组织转换为内部组织对象
        /// </summary>
        /// <param name="aaDomainBo">管理中心的组织</param>
        /// <returns>内部组织对象</returns>
        private Org TranslateOrg(AADomainBO aaDomainBo)
        {
            Org result = new Org();
            result.AllPath = aaDomainBo.Path;
            result.Code = aaDomainBo.DomainCode;
            result.Description = aaDomainBo.LabelDescription;
            result.DisplayName = aaDomainBo.DomainName;
            result.Id = aaDomainBo.DomainId.ToString();
            result.ParentOrgId = aaDomainBo.ParentId.ToString();
            return result;
        }
        /// <summary>
        /// 获取指定的对象，并转换为内部对象
        /// </summary>
        /// <typeparam name="Result">内部对象类型</typeparam>
        /// <typeparam name="ExtBo">外部Bo类型</typeparam>
        /// <param name="paras">外部Bo方法的参数列表</param>
        /// <param name="execFunc">外部Bo的查询方法</param>
        /// <param name="entityTrasformition">外部对象转换为内部对象方法签名</param>
        /// <returns>查询到的数据</returns>
        private List<Result> GetData<Result, ExtBo, Bizlet>(List<string> paras, Func<List<Guid>, Bizlet, List<ExtBo>> execFunc, Func<ExtBo, Result> entityTrasformition) where Bizlet : class
        {
            List<Result> orgs = new List<Result>();
            List<Guid> guids = TransformStringToGuid(paras);

            Bizlet iAABizlet = Containers.Default.Resolve<Bizlet>();
            List<ExtBo> bos = execFunc(guids, iAABizlet);

            if (null != bos && bos.Count > 0)
            {
                bos.ForEach(p => orgs.Add(entityTrasformition(p)));
            }

            return orgs;
        }
        #endregion

        /// <summary>
        /// 获取根节点
        /// </summary>
        /// <returns>返回获取到的根节点</returns>
        public Org GetRoot()
        {
            var iAABizlet = Containers.Default.Resolve<IWorkflowEngineBizlet>();
            var org=iAABizlet.GetAADomainListByParentId(null).FirstOrDefault();
            if (null != org)
            {
                return new Org() {
                    Id=org.DomainId.ToString(),
                    AllPath=org.Path,
                    Code=org.DomainCode,
                    Description=org.LabelDescription,
                    DisplayName=org.LabelNameCn,
                    ParentOrgId=org.ParentId.ToString()
                };
            }
            throw new ArgumentNullException("未找到组织的根节点,利用GetRoot().");
        }
    }
}
