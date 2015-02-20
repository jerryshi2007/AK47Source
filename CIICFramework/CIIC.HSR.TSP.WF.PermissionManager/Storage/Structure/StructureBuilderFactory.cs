using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.PermissionManager.Storage
{
    /// <summary>
    /// 创建构建器
    /// </summary>
    public class StructureBuilderFactory
    {
        /// <summary>
        /// 权限、应用和角色通用构建器
        /// </summary>
        /// <returns>构建器</returns>
        public static IStructureBuilder CreateARPCommonStructureBuilder()
        {
            return new ARPCommonStructureBuilder();
        }
        /// <summary>
        /// 应用结构构件器
        /// </summary>
        /// <returns>应用结构构件器</returns>
        public static IStructureBuilder CreateAppStructureBuilder()
        {
            return new AppStructureBuilder();
        }
        /// <summary>
        /// 角色构建起器
        /// </summary>
        /// <returns>/// 角色构建起器</returns>
        public static IStructureBuilder CreateRoleStructureBuilder()
        {
            return new RoleStructureBuilder();
        }
        /// <summary>
        /// 获取资源结构构建器
        /// </summary>
        /// <returns>资源结构构建器</returns>
        public static IStructureBuilder CreatePermissionStructureBuilder()
        {
            return new PermissionStructureBuilder();
        }
        /// <summary>
        /// 创建组织、人员和组的通用结构器
        /// </summary>
        /// <returns>组织、人员和组的通用结构器</returns>
        public static IStructureBuilder CreateOGUCommonStructureBuilder()
        {
            return new OGUCommonStructureBuilder();
        }
        /// <summary>
        /// 创建组织结构构建器
        /// </summary>
        /// <returns>组织结构构建器</returns>
        public static IStructureBuilder CreateOrgStructureBuilder()
        {
            return new OrgStructureBuilder();
        }
        /// <summary>
        /// 创建组构建器
        /// </summary>
        /// <returns>组构建器</returns>
        public static IStructureBuilder CreateGroupStructureBuilder()
        {
            return new GroupStructureBuilder();
        }
        /// <summary>
        /// 用户结构构建器
        /// </summary>
        /// <returns>用户结构构建器</returns>
        public static IStructureBuilder CreateUserStructureBuilder()
        {
            return new UserStructureBuilder();
        }
    }
}
