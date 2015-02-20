using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.OGUPermission
{
    /// <summary>
    /// 授权系统对象实现需要实现的接口
    /// </summary>
    public interface IPermissionImplInterface
    {
        /// <summary>
        /// 得到用户的角色
        /// </summary>
        /// <param name="application"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        RoleCollection GetUserRoles(IApplication application, IUser user);

        /// <summary>
        /// 得到用户的权限
        /// </summary>
        /// <param name="application"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        PermissionCollection GetUserPermissions(IApplication application, IUser user);

        /// <summary>
        /// 得到应用的角色
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        RoleCollection GetRoles(IApplication application);

        /// <summary>
        /// 得到应用的权限
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        PermissionCollection GetPermissions(IApplication application);
    }
}
