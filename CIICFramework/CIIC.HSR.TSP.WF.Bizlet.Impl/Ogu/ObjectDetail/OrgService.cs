﻿using CIIC.HSR.TSP.IoC;
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
    public class OrgService : IObjectService
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
            List<Org> orgs = oguProvider.GetOrgDetail(objIds);
            var orgFilling = DataFillingFactory.CreateOrgFilling();
            DataSet result = orgFilling.Fill(orgs);
            return result;
        }

        /// <summary>
        /// 获取某个节点下的所有子组织
        /// </summary>
        /// <param name="parentIds">节点列表</param>
        /// <param name="recursive">是否深度查询</param>
        /// <returns>所有子组织</returns>
        public System.Data.DataSet GetObjectChildren(List<string> parentIds, bool recursive)
        {
            ObjectServiceFactory serviceFactory = new ObjectServiceFactory(this.Context);
            var oguProvider = serviceFactory.CreatOguProvider();
            List<Org> doumainRoles = oguProvider.GetOrgChildren(parentIds, recursive);
            var orgFilling = DataFillingFactory.CreateOrgFilling();
            DataSet result = orgFilling.Fill(doumainRoles);
            return result;
        }
        /// <summary>
        /// 搜索子组织
        /// </summary>
        /// <param name="parentIds">节点列表</param>
        /// <param name="objName">子组织名称</param>
        /// <param name="fuzzy">是否模糊查询</param>
        /// <param name="recursive">是否深度查询</param>
        /// <param name="limitRows">行数</param>
        /// <returns>所有子组织</returns>
        public System.Data.DataSet SearchObject(List<string> parentIds, string objName, bool fuzzy, bool recursive, int limitRows)
        {
            ObjectServiceFactory serviceFactory = new ObjectServiceFactory(this.Context);
            var oguProvider = serviceFactory.CreatOguProvider();

            List<Org> orgs = oguProvider.SearchOrg(parentIds, objName,fuzzy,recursive,limitRows);
            var orgFilling = DataFillingFactory.CreateOrgFilling();
            DataSet result = orgFilling.Fill(orgs);
            return result;
        }

        public BizObject.ServiceContext Context
        {
            get;
            set;
        }
    }
}
