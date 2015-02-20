using CIIC.HSR.TSP.IoC;
using CIIC.HSR.TSP.WF.BizObject.Exchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Ogu
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
        public static IDataFilling<Group> CreateGroupFilling()
        {
            return new GroupDataFilling();
        }
        /// <summary>
        /// 组织填充器
        /// </summary>
        /// <returns>组织填充器</returns>
        public static IDataFilling<Org> CreateOrgFilling()
        {
            return new OrgDataFilling();
        }
        /// <summary>
        /// 用户填充器
        /// </summary>
        /// <returns>用户填充器</returns>
        public static IDataFilling<User> CreateUserFilling()
        {
            return new UserDataFilling();
        }
        /// <summary>
        /// 用户填充器
        /// </summary>
        /// <returns>用户填充器</returns>
        public static IDataFilling<AppEntity> CreateAppFilling()
        {
            return new AppFilling();
        }
        /// <summary>
        /// 用户填充器
        /// </summary>
        /// <returns>用户填充器</returns>
        public static IDataFilling<Role> CreateRoleFilling()
        {
            return new RoleFilling();
        }
        /// <summary>
        /// 用户填充器
        /// </summary>
        /// <returns>用户填充器</returns>
        public static IDataFilling<CIIC.HSR.TSP.WF.BizObject.Exchange.Resource> CreateResourceFilling()
        {
            return new ResourceFilling();
        }
    }
}
