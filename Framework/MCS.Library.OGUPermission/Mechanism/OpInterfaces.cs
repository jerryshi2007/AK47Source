#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.OGUPermission
// FileName	：	IOrganizationMechanism.cs
// Remark	：	机构人员的操作接口
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    沈峥	    20070430		创建
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.OGUPermission
{
    /// <summary>
    /// 机构人员的操作接口
    /// </summary>
    public interface IOrganizationMechanism
    {
        /// <summary>
        /// 得到机构或人员对象
        /// </summary>
        /// <typeparam name="T">期望返回的结果对象的类型，IOrganization、IUser、IGroup或IOguObject</typeparam>
        /// <param name="idType">查询时使用的ID类型，GUID，LogonName或FullPath</param>
        /// <param name="ids">ID数组</param>
        /// <returns>对象集合</returns>
        // <param name="objType">对象的类型</param>
        OguObjectCollection<T> GetObjects<T>(SearchOUIDType idType, params string[] ids) where T : IOguObject;

        /// <summary>
        /// 返回根机构的对象。取决于配置文件
        /// </summary>
        /// <returns>根机构对象</returns>
        IOrganization GetRoot();

		/// <summary>
		/// 用户认证，通常是判断用户的用户名和口令是否正确
		/// </summary>
		/// <param name="identity">用户的登录名、口令和域</param>
		/// <returns>是否认证成功</returns>
		bool AuthenticateUser(LogOnIdentity identity);

		/// <summary>
		/// 清除所有Cache
		/// </summary>
		void RemoveAllCache();
    }

    /// <summary>
    /// 授权系统的操作接口
    /// </summary>
    public interface IPermissionMechanism
    {
        /// <summary>
        /// 得到所有应用对象
        /// </summary>
        /// <returns></returns>
        ApplicationCollection GetAllApplications();

        /// <summary>
        /// 得到指定名称的应用对象
        /// </summary>
        /// <param name="codeNames">应用的名称</param>
        /// <returns>应用对象集合</returns>
        ApplicationCollection GetApplications(params string[] codeNames);

        /// <summary>
        /// 得到指定角色下，某些部门内的所有授权人员
        /// </summary>
        /// <param name="roles">角色集合。</param>
        /// <param name="depts">组织机构集合。</param>
        /// <param name="recursively">是否递归。</param>
        /// <returns></returns>
        OguObjectCollection<IOguObject> GetRolesObjects(RoleCollection roles, OguObjectCollection<IOrganization> depts, bool recursively);

		/// <summary>
		/// 清除所有Cache
		/// </summary>
		void RemoveAllCache();
    }
}
