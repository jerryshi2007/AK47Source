using CIIC.HSR.TSP.WF.Bizlet.Contract;
using CIIC.HSR.TSP.WF.Bizlet.Impl;
using CIIC.HSR.TSP.WF.Bizlet.Impl.Ogu;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace CIIC.HSR.TSP.WF.PermissionManagerService
{
    /// <summary>
    /// Summary description for ReaderService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ReaderService : System.Web.Services.WebService
    {
        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param>
        /// <returns>成功返回true，否则，false</returns>
        [WebMethod]
        public bool SignInCheck(string account, string password)
        {
            ServiceFactory sf = new ServiceFactory();
            var readerService=sf.CreateService<IReaderServiceBizlet>();
            return readerService.SignInCheck(account,password);
        }
        /// <summary>
        /// 得到若干个对象的详细信息，返回多行数据
        /// </summary>
        /// <param name="strObjType">需要返回对象的类型，如果为空，则表示所有类型的对象,参考ObjectType中的定义</param>
        /// <param name="strObjValues">查询Id的值列表，多个ID间使用逗号分隔</param>
        /// <param name="iSoc">Id值的类型，参考IDObjectType中的定义</param>
        /// <param name="strParentValues">废弃</param>
        /// <param name="iSoco">废弃</param>
        /// <param name="strExtAttrs">废弃</param>
        /// <returns>DataSet中包含一个DataTable，每一行包含一个对象。如果人员兼职于两个组织，则返回两个对象。这里需要注意的是，
        /// 如果请求查询的对象类型（strObjValues）是单一的类型，则DataTable的字段是上面描述的具体字段定义；如果是多个类型，则返回的是通用的字段定义</returns>
        [WebMethod]
        public DataSet GetObjectsDetail(string strObjType, string strObjValues, int iSoc, string strParentValues, int iSoco, string strExtAttrs)
        {
            ServiceFactory sf = new ServiceFactory();
            var readerService = sf.CreateService<IReaderServiceBizlet>();
            return readerService.GetObjectsDetail(strObjType, strObjValues,iSoc, strParentValues, iSoco, strExtAttrs);
        }
        /// <summary>
        /// 得到某几个组织下的子对象，返回多行数据
        /// </summary>
        /// <param name="strOrgValues">父对象Id集合，多个ID间使用逗号分隔</param>
        /// <param name="iSoc">Id类型，如果为空，则表示所有类型的对象,见IDObjectType中的定义</param>
        /// <param name="iLot">需要查询的子对象类型，如果为空，则表示所有类型的对象。参考ObjectType中的定义</param>
        /// <param name="iLod">是否查询出删除的对象。1表示正常对象，其它表示包含被删除的对象</param>
        /// <param name="iDepth">0表示递归查询出所有的子对象。1表示仅查询一级</param>
        /// <param name="strOrgRankCodeName">废弃</param>
        /// <param name="strUserRankCodeName">废弃</param>
        /// <param name="strHideType">废弃</param>
        /// <param name="strAttrs">废弃</param>
        /// <returns>DataSet中包含一个DataTable，每一行包含一个对象。根据iLot，如果是单一的类型，则DataTable的字段是上面描述的具体字段定义；如果是多个类型，则返回的是通用的字段定义</returns>
        [WebMethod]
        public DataSet GetOrganizationChildren(string strOrgValues, int iSoc, int iLot, int iLod, int iDepth, string strOrgRankCodeName, string strUserRankCodeName, string strHideType, string strAttrs)
        {
            ServiceFactory sf = new ServiceFactory();
            var readerService = sf.CreateService<IReaderServiceBizlet>();
            return readerService.GetOrganizationChildren(strOrgValues, iSoc, iLot, iLod, iDepth, strOrgRankCodeName, strUserRankCodeName, strHideType, strAttrs);
        }
        /// <summary>
        /// 得到某几个组织下的子对象，返回多行数据
        /// </summary>
        /// <param name="strOrgValues">父对象的ID集合，多个ID间使用逗号分隔</param>
        /// <param name="iSoc">需要查询的子对象类型，如果为空，则表示所有类型的对象。参考ObjectType中的定义</param>
        /// <param name="strLikeName">需要查询字符串，例如“牛小超”</param>
        /// <param name="bLike">是否是Like性质的查询。目前前端传递过来是false</param>
        /// <param name="strAttr">废弃</param>
        /// <param name="iListObjType">要求查询的对象类型，如果为空，则表示所有类型的对象。参考ObjectType中的定义</param>
        /// <param name="iLod">是否查询出删除的对象。1表示正常对象，其它表示包含被删除的对象</param>
        /// <param name="iDep">0表示递归查询出所有的子对象。1表示仅查询一级</param>
        /// <param name="strHideType">废弃</param>
        /// <param name="rtnRowLimit">返回的行数。如果是-1，则返回所有行</param>
        /// <returns>DataSet中包含一个DataTable，每一行包含一个对象。根据iLot，如果是单一的类型，则DataTable的字段是上面描述的具体字段定义；如果是多个类型，则返回的是通用的字段定义</returns>
        [WebMethod]
        public DataSet QueryOGUByCondition3(string strOrgValues, int iSoc, string strLikeName, bool bLike, string strAttr, int iListObjType, int iLod, int iDep, string strHideType, int rtnRowLimit)
        {
            ServiceFactory sf = new ServiceFactory();
            var readerService = sf.CreateService<IReaderServiceBizlet>();
            return readerService.QueryOGUByCondition3(strOrgValues, iSoc, strLikeName, bLike, strAttr, iListObjType, iLod, iDep, strHideType, rtnRowLimit);
        }
        /// <summary>
        /// 得到人员所属的群组。在中智的场景下，应该是查询人员所属的角色实例。
        /// </summary>
        /// <param name="strUserValues">人员的ID，多个ID之间使用逗号分隔</param>
        /// <param name="iSocu">人员对象的ID类型。请参考上面的对象的ID类型</param>
        /// <param name="strParentValue">废弃</param>
        /// <param name="iSoco">废弃</param>
        /// <param name="strAttrs">废弃</param>
        /// <param name="iLod">是否查询出删除的对象。1表示正常对象，其它表示包含被删除的对</param>
        /// <returns>DataSet中包含一个DataTable，每一行包含一个对象，使用通用字段定义</returns>
        [WebMethod]
        public DataSet GetGroupsOfUsers(string strUserValues, int iSocu, string strParentValue, int iSoco, string strAttrs, int iLod)
        {
            ServiceFactory sf = new ServiceFactory();
            var readerService = sf.CreateService<IReaderServiceBizlet>();
            return readerService.GetGroupsOfUsers(strUserValues, iSocu, strParentValue, iSoco, strAttrs, iLod);
        }
        /// <summary>
        /// 查询群组下的人员。在中智的场景下，应该是查询角色实例
        /// </summary>
        /// <param name="strGroupValues">群组的ID，多个ID之间使用逗号分隔</param>
        /// <param name="iSocg">群组对象的ID类型。见IDObjectType中的定义</param>
        /// <param name="strAttrs">废弃</param>
        /// <param name="strOrgValues">废弃</param>
        /// <param name="iSoco">废弃</param>
        /// <param name="strUserRankCodeName">废弃</param>
        /// <param name="iLod">是否查询出删除的对象。1表示正常对象，其它表示包含被删除的对象</param>
        /// <returns>DataSet中包含一个DataTable，每一行包含一个对象，使用用户的字段定义</returns>
        [WebMethod]
        public DataSet GetUsersInGroups(string strGroupValues, int iSocg, string strAttrs, string strOrgValues, int iSoco, string strUserRankCodeName, int iLod)
        {
            ServiceFactory sf = new ServiceFactory();
            var readerService = sf.CreateService<IReaderServiceBizlet>();
            return readerService.GetUsersInGroups(strGroupValues, iSocg, strAttrs, strOrgValues, iSoco, strUserRankCodeName, iLod);
        }
        /// <summary>
        /// 查询领导的秘书
        /// </summary>
        /// <param name="strLeaderValues">领导（人员）的ID，多个ID之间使用逗号分隔</param>
        /// <param name="iSoc">人员对象的ID类型。见IDObjectType中的定义</param>
        /// <param name="strAttrs">废弃</param>
        /// <param name="iLod">是否查询出删除的对象。1表示正常对象，其它表示包含被删除的对象</param>
        /// <returns>DataSet中包含一个DataTable，每一行包含一个对象，使用用户的字段定义</returns>
        [WebMethod]
        public DataSet GetSecretariesOfLeaders(string strLeaderValues, int iSoc, string strAttrs, int iLod)
        {
            ServiceFactory sf = new ServiceFactory();
            var readerService = sf.CreateService<IReaderServiceBizlet>();
            return readerService.GetSecretariesOfLeaders(strLeaderValues, iSoc, strAttrs, iLod);
        }
        /// <summary>
        /// 查询秘书的领导
        /// </summary>
        /// <param name="strSecValues">秘书（人员）的ID，多个ID之间使用逗号分隔</param>
        /// <param name="iSoc">人员对象的ID类型。请参考上面的对象的ID类型</param>
        /// <param name="strAttrs">废弃</param>
        /// <param name="iLod">是否查询出删除的对象。1表示正常对象，其它表示包含被删除的对象</param>
        /// <returns>DataSet中包含一个DataTable，每一行包含一个对象，使用用户的字段定义</returns>
        [WebMethod]
        public DataSet GetLeadersOfSecretaries(string strSecValues, int iSoc, string strAttrs, int iLod)
        {
            ServiceFactory sf = new ServiceFactory();
            var readerService = sf.CreateService<IReaderServiceBizlet>();
            return readerService.GetSecretariesOfLeaders(strSecValues, iSoc, strAttrs, iLod);
        }
        /// <summary>
        /// 清除缓存
        /// </summary>
        [WebMethod]
        public void RemoveAllCache()
        {
            ServiceFactory sf = new ServiceFactory();
            var readerService = sf.CreateService<IReaderServiceBizlet>();
            readerService.RemoveAllCache();
        }
        /// <summary>
        /// 将数据合并
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="dest">目标</param>
        /// <returns>合并结果</returns>
        private DataSet MergeResult(DataSet source, DataSet dest)
        {
            if (source.Tables.Count > 0 && source.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in source.Tables[0].Rows)
                {
                    DataRow newRow = dest.Tables[0].NewRow();
                    foreach (DataColumn column in dest.Tables[0].Columns)
                    {
                        newRow[column.ColumnName] = row[column.ColumnName];
                    }
                    dest.Tables[0].Rows.Add(newRow);
                }
                return dest;
            }

            return new DataSet();
        }
    }
}
