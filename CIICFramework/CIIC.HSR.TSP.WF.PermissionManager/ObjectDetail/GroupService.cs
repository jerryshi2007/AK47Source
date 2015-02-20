using CIIC.HSR.TSP.IoC;
using CIIC.HSR.TSP.TA.Bizlet.Contract;
using CIIC.HSR.TSP.TA.BizObject;
using CIIC.HSR.TSP.WF.PermissionManager.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.PermissionManager.ObjectDetail
{
    /// <summary>
    /// 角色实例
    /// </summary>
    public class GroupService : IObjectService
    {
        /// <summary>
        /// 角色实例详细信息
        /// </summary>
        /// <param name="objIds">角色实例Id列表</param>
        /// <returns></returns>
        public System.Data.DataSet GetObjectDetail(List<Guid> objIds)
        {
            var iaaDomainRole = Containers.Default.Resolve<IWorkflowEngineBizlet>();
            List<AADomainRoleBO> doumainRoles= iaaDomainRole.GetAADomainRoleListByIds(objIds);
            var groupFilling=DataFillingFactory.CreateGroupFilling();
            DataSet result= groupFilling.Fill(doumainRoles);
            return result;
        }
        /// <summary>
        /// 某些组织下的所有角色实例
        /// </summary>
        /// <param name="parentIds">组织列表</param>
        /// <param name="recursive">是否深度查询</param>
        /// <returns>角色实例列表</returns>
        public System.Data.DataSet GetObjectChildren(List<Guid> parentIds, bool recursive)
        {
            var iaaDomainRole = Containers.Default.Resolve<IWorkflowEngineBizlet>();
            List<AADomainRoleBO> doumainRoles = iaaDomainRole.GetAADomainRoleListByDomainIds(parentIds, recursive, null);
            var groupFilling = DataFillingFactory.CreateGroupFilling();
            DataSet result = groupFilling.Fill(doumainRoles);
            return result;
        }
        /// <summary>
        /// 角色实例搜索
        /// </summary>
        /// <param name="parentIds">组织Id列表</param>
        /// <param name="objName">组织名称</param>
        /// <param name="fuzzy">是否模糊查询</param>
        /// <param name="recursive">是否深度查询</param>
        /// <returns>角色实例列表</returns>
        public System.Data.DataSet SearchObject(List<Guid> parentIds, string objName, bool fuzzy, bool recursive, int limitRows)
        {
            var iaaDomainRole = Containers.Default.Resolve<IWorkflowEngineBizlet>();
            int? limitCount = null;
            if (-1 != limitRows)
            {
                limitCount = limitRows;
            }
            List<AADomainRoleBO> doumainRoles = iaaDomainRole.GetAADomainRoleListByDomainIds(parentIds, recursive, limitCount, objName, fuzzy);
            var groupFilling = DataFillingFactory.CreateGroupFilling();
            DataSet result = groupFilling.Fill(doumainRoles);
            return result;
        }
        public BizObject.ServiceContext Context
        {
            get;
            set;
        }
    }
}
