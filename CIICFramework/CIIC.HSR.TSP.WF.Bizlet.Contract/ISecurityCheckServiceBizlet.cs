using CIIC.HSR.TSP.WF.Bizlet.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract
{
    public interface ISecurityCheckServiceBizlet : IRuntime
    {
        /// <summary>
        /// 得到所有的应用
        /// </summary>
        /// <returns></returns>
        DataSet GetApplications();
        /// <summary>
        /// 得到某个应用下的角色
        /// </summary>
        /// <param name="appCodeName">应用编码（废弃）</param>
        /// <param name="rightMask">废弃</param>
        /// <returns>返回上面提到的角色的集合</returns>
        DataSet GetRoles(string appCodeName, RightMaskType rightMask);
        /// <summary>
        /// 应用的代码名称。在中智，可以使用对象GUID来表示
        /// </summary>
        /// <param name="appCodeName">废弃</param>
        /// <param name="rightMask">废弃</param>
        /// <returns></returns>
        DataSet GetFunctions(string appCodeName, RightMaskType rightMask);
        /// <summary>
        /// 应用的代码名称。在中智，可以使用对象GUID来表示
        /// </summary>
        /// <param name="appCodeName">废弃</param>
        /// <param name="funcCodeNames">权限编码列表</param>
        /// <returns>返回上面提到的角色的集合</returns>
        DataSet GetFunctionsRoles(string appCodeName, string funcCodeNames);
        /// <summary>
        /// 返回某个角色下的所有人员
        /// </summary>
        /// <param name="orgRoot">废弃</param>
        /// <param name="appCodeName">废弃</param>
        /// <param name="roleCodeNames">角色的代码名称。多个代码名称使用逗号分隔</param>
        /// <param name="delegationMask">废弃</param>
        /// <param name="sidelineMask">废弃</param>
        /// <param name="extAttr">废弃</param>
        /// <returns>返回之前提到的人员的DataTable</returns>
        DataSet GetRolesUsers(string orgRoot, string appCodeName, string roleCodeNames, DelegationMaskType delegationMask, SidelineMaskType sidelineMask, string extAttr);
        /// <summary>
        /// 返回某个角色下的所有人员
        /// </summary>
        /// <param name="userValue">用户的ID，ID的类型取决于userValueType参数</param>
        /// <param name="appCodeName">废弃</param>
        /// <param name="userValueType">用户Id类型，参见userValueType中的的定义</param>
        /// <param name="rightMask">废弃</param>
        /// <param name="delegationMask">废弃</param>
        /// <returns>返回上面提到的角色的集合</returns>
        DataSet GetUserRoles(string userValue, string appCodeName, UserValueType userValueType, RightMaskType rightMask, DelegationMaskType delegationMask);
        /// <summary>
        /// 返回用户在某个应用下所拥有的权限
        /// </summary>
        /// <param name="userValue">用户的ID，ID的类型取决于userValueType参数</param>
        /// <param name="appCodeName">废弃</param>
        /// <param name="userValueType">用户Id类型，参见userValueType中的的定义</param>
        /// <param name="rightMask">废弃</param>
        /// <param name="delegationMask">废弃</param>
        /// <returns>返回上面提到的权限的集合。在中智，返回空的DataTable</returns>
        DataSet GetUserPermissions(string userValue, string appCodeName, UserValueType userValueType, RightMaskType rightMask, DelegationMaskType delegationMask);
        /// <summary>
        /// 返回某个应用下角色中直接包含的对象
        /// </summary>
        /// <param name="orgRoot">废弃</param>
        /// <param name="appCodeName">废弃</param>
        /// <param name="roleCodeNames">角色的代码名称。多个角色使用逗号分隔</param>
        /// <param name="doesMixSort">废弃</param>
        /// <param name="doesSortRank">废弃</param>
        /// <param name="includeDelegate">废弃</param>
        /// <returns>返回上面提到的通用的组织和人员的DataTable，字段按照通用结构来定义</returns>
        DataSet GetChildrenInRoles(string orgRoot, string appCodeName, string roleCodeNames, bool doesMixSort, bool doesSortRank, bool includeDelegate);
    }
}
