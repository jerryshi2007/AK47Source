using CIIC.HSR.TSP.IoC;
using CIIC.HSR.TSP.TA.Bizlet.Contract;
using CIIC.HSR.TSP.TA.BizObject;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using CIIC.HSR.TSP.WF.BizObject.Exchange;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Ogu
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
        public System.Data.DataSet GetObjectDetail(List<string> objIds)
        {
            ObjectServiceFactory serviceFactory = new ObjectServiceFactory(this.Context);
            var oguProvider = serviceFactory.CreatOguProvider();

            List<Group> group = oguProvider.GetGroupDetail(objIds);
            var groupFilling=DataFillingFactory.CreateGroupFilling();
            DataSet result = groupFilling.Fill(group);
            return result;
        }
        /// <summary>
        /// 某些组织下的所有角色实例
        /// </summary>
        /// <param name="parentIds">组织列表</param>
        /// <param name="recursive">是否深度查询</param>
        /// <returns>角色实例列表</returns>
        public System.Data.DataSet GetObjectChildren(List<string> parentIds, bool recursive)
        {
            ObjectServiceFactory serviceFactory = new ObjectServiceFactory(this.Context);
            var oguProvider = serviceFactory.CreatOguProvider();

            List<Group> groups = oguProvider.GetGroupChildren(parentIds, recursive);
            var groupFilling = DataFillingFactory.CreateGroupFilling();
            DataSet result = groupFilling.Fill(groups);
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
        public System.Data.DataSet SearchObject(List<string> parentIds, string objName, bool fuzzy, bool recursive, int limitRows)
        {
            ObjectServiceFactory serviceFactory = new ObjectServiceFactory(this.Context);
            var oguProvider = serviceFactory.CreatOguProvider();

            List<Group> groups = oguProvider.SearchGroup(parentIds, objName, fuzzy, recursive, limitRows);
            var groupFilling = DataFillingFactory.CreateGroupFilling();
            DataSet result = groupFilling.Fill(groups);
            return result;
        }
        public BizObject.ServiceContext Context
        {
            get;
            set;
        }
    }
}
