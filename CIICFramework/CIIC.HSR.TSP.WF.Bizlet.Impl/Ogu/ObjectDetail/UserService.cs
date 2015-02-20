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
        public System.Data.DataSet GetObjectDetail(List<string> objIds)
        {
            ObjectServiceFactory serviceFactory = new ObjectServiceFactory(this.Context);
            var oguProvider = serviceFactory.CreatOguProvider();

            List<User> users = oguProvider.GetUserDetail(objIds);
            var userFilling = DataFillingFactory.CreateUserFilling();
            DataSet result = userFilling.Fill(users);
            return result;
        }


        public System.Data.DataSet GetObjectChildren(List<string> parentIds, bool recursive)
        {
            ObjectServiceFactory serviceFactory = new ObjectServiceFactory(this.Context);
            var oguProvider = serviceFactory.CreatOguProvider();

            List<User> users = oguProvider.GetUserChildren(parentIds, recursive);
            var userFilling = DataFillingFactory.CreateUserFilling();
            DataSet result = userFilling.Fill(users);
            return result;
        }

        public System.Data.DataSet SearchObject(List<string> parentIds, string objName, bool fuzzy, bool recursive, int limitRows)
        {
            ObjectServiceFactory serviceFactory = new ObjectServiceFactory(this.Context);
            var oguProvider = serviceFactory.CreatOguProvider();

            List<User> users = oguProvider.SearchUser(parentIds, objName,fuzzy,recursive,limitRows);
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
