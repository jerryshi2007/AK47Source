using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.PermissionManager.Storage
{
    /// <summary>
    /// 创建数据填充器
    /// </summary>
    public class DataFillingFactory
    {
        /// <summary>
        /// 角色实例填充器
        /// </summary>
        /// <returns>角色实例填充器</returns>
        public static IDataFilling CreateGroupFilling()
        {
            return new GroupDataFilling();
        }
        /// <summary>
        /// 组织填充器
        /// </summary>
        /// <returns>组织填充器</returns>
        public static IDataFilling CreateOrgFilling()
        {
            return new OrgDataFilling();
        }
        /// <summary>
        /// 用户填充器
        /// </summary>
        /// <returns>用户填充器</returns>
        public static IDataFilling CreateUserFilling()
        {
            return new UserDataFilling();
        }
        /// <summary>
        /// 用户填充器
        /// </summary>
        /// <returns>用户填充器</returns>
        public static IDataFilling CreateAppFilling()
        {
            return new AppFilling();
        }
        /// <summary>
        /// 用户填充器
        /// </summary>
        /// <returns>用户填充器</returns>
        public static IDataFilling CreateRoleFilling()
        {
            return new RoleFilling();
        }
        /// <summary>
        /// 用户填充器
        /// </summary>
        /// <returns>用户填充器</returns>
        public static IDataFilling CreateResourceFilling()
        {
            return new ResourceFilling();
        }
    }
}
