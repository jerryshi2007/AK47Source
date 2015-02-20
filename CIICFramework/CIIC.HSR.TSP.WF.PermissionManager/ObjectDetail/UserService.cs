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
    /// 用户详细信息
    /// </summary>
    public class UserService : IObjectService
    {
        /// <summary>
        /// 获取对象的详细信息
        /// </summary>
        /// <param name="objIds">对象的Id列表</param>
        /// <param name="recursive">是否</param>
        /// <returns>对象详细信息列表</returns>
        public System.Data.DataSet GetObjectDetail(List<Guid> objIds)
        {
            var userBiz = Containers.Default.Resolve<IWorkflowEngineBizlet>();
            List<AAUserBO> users = userBiz.GetAAUserListByIds(objIds);
            var userFilling = DataFillingFactory.CreateUserFilling();
            DataSet result = userFilling.Fill(users);
            return result;
        }


        public System.Data.DataSet GetObjectChildren(List<Guid> parentIds, bool recursive)
        {
            var userBiz = Containers.Default.Resolve<IWorkflowEngineBizlet>();
            List<AAUserBO> users = userBiz.GetAAUserListByDomainIds(parentIds,recursive,null);
            var userFilling = DataFillingFactory.CreateUserFilling();
            DataSet result = userFilling.Fill(users);
            return result;
        }

        public System.Data.DataSet SearchObject(List<Guid> parentIds, string objName, bool fuzzy, bool recursive, int limitRows)
        {
            var userBiz = Containers.Default.Resolve<IWorkflowEngineBizlet>();
            int? limitCount = null;
            if (limitRows != -1)
            {
                limitCount = limitRows;
            }
            List<AAUserBO> users = userBiz.GetAAUserListByDomainIds(parentIds, recursive, limitCount,objName,fuzzy);
            var userFilling = DataFillingFactory.CreateUserFilling();
            DataSet result = userFilling.Fill(users);
            return result;
        }
        public BizObject.ServiceContext Context
        {
            get;
            set;
        }
    }
}
