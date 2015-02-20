using CIIC.HSR.TSP.WF.Bizlet.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Ogu
{
    public class ReaderServiceBizlet : IReaderServiceBizlet
    {
        public bool SignInCheck(string account, string password)
        {
            return true;
        }

        public System.Data.DataSet GetObjectsDetail(string strObjType, string strObjValues, int iSoc, string strParentValues, int iSoco, string strExtAttrs)
        {
            DataSet result = new DataSet();
            List<string> ids = new List<string>();
            ObjectServiceFactory objectServiceFactory = new ObjectServiceFactory(this.Context);
            IARPService arpService = objectServiceFactory.CreateARPService();
            if (!string.IsNullOrEmpty(strObjValues))
            {
                if (strObjType.Equals(ObjectType.USERSName, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (iSoc.ToString().Equals(IDObjectType.GUID, StringComparison.CurrentCultureIgnoreCase))
                    {
                        strObjValues.Split(new[] { ',' }).ToList().ForEach(p => ids.Add(p));
                    }
                    else if (iSoc.ToString().Equals(IDObjectType.LogonName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        strObjValues.Split(new[] { ',' }).ToList().ForEach(p =>
                        {
                            var userInfo = arpService.GetUserByLogonName(p);
                            ids.Add(userInfo.UserId);
                        });
                    }
                }
                else if (strObjType.Equals(ObjectType.ORGANIZATIONSName, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (iSoc.ToString().Equals(IDObjectType.GUID, StringComparison.CurrentCultureIgnoreCase))
                    {
                        strObjValues.Split(new[] { ',' }).ToList().ForEach(p => ids.Add(p));
                    }
                    else if (iSoc.ToString().Equals(IDObjectType.FullPath, StringComparison.CurrentCultureIgnoreCase))
                    {
                        strObjValues.Split(new[] { ',' }).ToList().ForEach(p =>
                        {
                            var orgInfo = arpService.GetDomainByPath(p);
                            ids.Add(orgInfo.OrgId);
                        });
                    }
                }
                else
                {
                    strObjValues.Split(new[] { ',' }).ToList().ForEach(p => ids.Add(p));
                }
            }
            

            if (!string.IsNullOrEmpty(strObjType))
            {
                IObjectService objectService = objectServiceFactory.Create(strObjType);
                result = objectService.GetObjectDetail(ids);
            }
            else
            {
                IObjectService groupService = objectServiceFactory.Create(ObjectType.GROUPS);
                DataSet resultGroup = groupService.GetObjectDetail(ids);
                IObjectService userService = objectServiceFactory.Create(ObjectType.USERS);
                DataSet resultUser = userService.GetObjectDetail(ids);
                IObjectService orgService = objectServiceFactory.Create(ObjectType.ORGANIZATIONS);
                DataSet resultOrg = orgService.GetObjectDetail(ids);

                IStructureBuilder structureCommon = StructureBuilderFactory.CreateOGUCommonStructureBuilder();
                DataSet common = structureCommon.Create();

                MergeResult(resultGroup, common);
                MergeResult(resultUser, common);
                MergeResult(resultOrg, common);

                result = common;
            }
            return result;
        }

        public System.Data.DataSet GetOrganizationChildren(string strOrgValues, int iSoc, int iLot, int iLod, int iDepth, string strOrgRankCodeName, string strUserRankCodeName, string strHideType, string strAttrs)
        {
            DataSet result = new DataSet();
            List<string> ids = new List<string>();
            strOrgValues.Split(new[] { ',' }).ToList().ForEach(p => ids.Add(p));
            ObjectServiceFactory objectServiceFactory = new ObjectServiceFactory(this.Context);
            if (iLot != 65535)
            {
                IObjectService objectService = objectServiceFactory.Create(iLot.ToString());
                result = objectService.GetObjectChildren(ids, iDepth == 0);
            }
            else
            {
                IObjectService groupService = objectServiceFactory.Create(ObjectType.GROUPS);
                DataSet resultGroup = groupService.GetObjectChildren(ids, iDepth == 0);
                IObjectService userService = objectServiceFactory.Create(ObjectType.USERS);
                DataSet resultUser = userService.GetObjectChildren(ids, iDepth == 0);
                IObjectService orgService = objectServiceFactory.Create(ObjectType.ORGANIZATIONS);
                DataSet resultOrg = orgService.GetObjectChildren(ids, iDepth == 0);

                IStructureBuilder structureCommon = StructureBuilderFactory.CreateOGUCommonStructureBuilder();
                DataSet common = structureCommon.Create();

                MergeResult(resultGroup, common);
                MergeResult(resultUser, common);
                MergeResult(resultOrg, common);
                result = common;
            }
            return result;
        }

        public System.Data.DataSet QueryOGUByCondition3(string strOrgValues, int iSoc, string strLikeName, bool bLike, string strAttr, int iListObjType, int iLod, int iDep, string strHideType, int rtnRowLimit)
        {
            DataSet result = new DataSet();
            List<string> ids = new List<string>();
            strOrgValues.Split(new[] { ',' }).ToList().ForEach(p => ids.Add(p));
            ObjectServiceFactory objectServiceFactory = new ObjectServiceFactory(this.Context);
            if (!string.IsNullOrEmpty(iSoc.ToString()))
            {
                IObjectService objectService = objectServiceFactory.Create(iSoc.ToString());
                result = objectService.SearchObject(ids, strLikeName, bLike, iDep == 0, rtnRowLimit);
            }
            else
            {
                IObjectService groupService = objectServiceFactory.Create(ObjectType.GROUPS);
                DataSet resultGroup = groupService.SearchObject(ids, strLikeName, bLike, iDep == 0, rtnRowLimit);
                IObjectService userService = objectServiceFactory.Create(ObjectType.USERS);
                DataSet resultUser = userService.SearchObject(ids, strLikeName, bLike, iDep == 0, rtnRowLimit);
                IObjectService orgService = objectServiceFactory.Create(ObjectType.ORGANIZATIONS);
                DataSet resultOrg = orgService.SearchObject(ids, strLikeName, bLike, iDep == 0, rtnRowLimit);

                IStructureBuilder structureCommon = StructureBuilderFactory.CreateOGUCommonStructureBuilder();
                DataSet common = structureCommon.Create();

                MergeResult(resultGroup, common);
                MergeResult(resultUser, common);
                MergeResult(resultOrg, common);
                result = common;
            }
            return result;
        }

        public System.Data.DataSet GetGroupsOfUsers(string strUserValues, int iSocu, string strParentValue, int iSoco, string strAttrs, int iLod)
        {
            ObjectServiceFactory objectServiceFactory = new ObjectServiceFactory(this.Context);
            IARPService arpService = objectServiceFactory.CreateARPService();
            List<string> ids = new List<string>();
            strUserValues.Split(new[] { ',' }).ToList().ForEach(p => ids.Add(p));
            return arpService.GetGroupsOfUsers(ids);
        }

        public System.Data.DataSet GetUsersInGroups(string strGroupValues, int iSocg, string strAttrs, string strOrgValues, int iSoco, string strUserRankCodeName, int iLod)
        {
            ObjectServiceFactory objectServiceFactory = new ObjectServiceFactory(this.Context);
            IARPService arpService = objectServiceFactory.CreateARPService();
            List<string> ids = new List<string>();
            strGroupValues.Split(new[] { ',' }).ToList().ForEach(p => ids.Add(p));
            return arpService.GetUsersOfGroups(ids);
        }

        public System.Data.DataSet GetSecretariesOfLeaders(string strLeaderValues, int iSoc, string strAttrs, int iLod)
        {
            //仅有结构，无数据
            return StructureBuilderFactory.CreateUserStructureBuilder().Create();
        }

        public System.Data.DataSet GetLeadersOfSecretaries(string strSecValues, int iSoc, string strAttrs, int iLod)
        {
            //仅有结构，无数据
            return StructureBuilderFactory.CreateUserStructureBuilder().Create();
        }

        public void RemoveAllCache()
        {
            ObjectServiceFactory objectServiceFactory = new ObjectServiceFactory(this.Context);
            objectServiceFactory.CreateARPService().RemoveAllCache();
        }
        public DataSet GetRoot()
        {
            ObjectServiceFactory objectServiceFactory = new ObjectServiceFactory(this.Context);
            var factory=objectServiceFactory.CreateARPService();
            return factory.GetRoot();
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

        public BizObject.ServiceContext Context
        {
            get;
            set;
        }


        
    }
}
