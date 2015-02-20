using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using PermissionCenter.Adapters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace PermissionCenter.Services
{
    using MCS.Library.Caching;
    using MCS.Library.OGUPermission;
    using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
    using MCS.Library.SOA.DataObjects.Security;
    using MCS.Library.SOA.DataObjects.Security.Actions;

    /// <summary>
    /// Summary description for OGUReaderService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class OGUReaderService : System.Web.Services.WebService
    {
        private static Dictionary<SearchOUIDType, QueryByIDsAdapterBase<DataTable>> _SearchAdapter = new Dictionary<SearchOUIDType, QueryByIDsAdapterBase<DataTable>>() { };

        #region SchemaParameter
        private class SchemaParameter
        {
            private string[] _SchemaTypes = null;
            private bool _IncludeNonDefault = true;

            public SchemaParameter(int iLot)
            {
                SchemaType schemaType = (SchemaType)iLot;

                List<string> types = new List<string>();

                if ((SchemaType.Users & schemaType) != SchemaType.Unspecified)
                    types.Add(SchemaType.Users.ToString());

                if ((SchemaType.Organizations & schemaType) != SchemaType.Unspecified)
                    types.Add(SchemaType.Organizations.ToString());

                if ((SchemaType.Groups & schemaType) != SchemaType.Unspecified)
                    types.Add(SchemaType.Groups.ToString());

                this._IncludeNonDefault = (SchemaType.Sideline & schemaType) != SchemaType.Unspecified;

                this._SchemaTypes = types.ToArray();
            }

            public string[] SchemaTypes
            {
                get
                {
                    return this._SchemaTypes;
                }
            }

            public bool IncludeNonDefault
            {
                get
                {
                    return this._IncludeNonDefault;
                }
                set
                {
                    this._IncludeNonDefault = value;
                }
            }
        }
        #endregion SchemaParameter

        #region Web methods
        /// <summary>
        /// 验证人员登录名和密码是否正确
        /// </summary>
        /// <param name="strLogonName">用户登录名</param>
        /// <param name="strUserPwd">用户密码</param>
        /// <returns>登录名和密码是否匹配</returns>
        [WebMethod]
        public bool SignInCheck(string strLogonName, string strUserPwd)
        {
            bool result = false;

            if (strLogonName.IsNotEmpty())
            {
                string key = strLogonName + strUserPwd;

                SchemaObjectBase userObj = SchemaObjectAdapter.Instance.LoadByCodeName(StandardObjectSchemaType.Users.ToString(), strLogonName, SchemaObjectStatus.Normal, DateTime.MinValue);

                ExceptionHelper.FalseThrow<ApplicationException>(userObj != null, "用户\"{0}\"不存在", strLogonName);

                result = UserPasswordAdapter.Instance.CheckPassword(userObj.ID, UserPasswordAdapter.GetPasswordType(), strUserPwd);

                (userObj.CurrentParentRelations.Count > 0).FalseThrow("账户\"{0}\"必须属于一个组织", strLogonName);

                if (userObj.Properties.GetValue("PasswordNotRequired", false) == false)
                    result = UserPasswordAdapter.Instance.CheckPassword(userObj.ID, UserPasswordAdapter.GetPasswordType(), strUserPwd);
                else
                    result = true;

                if (result)
                {
                    userObj.Properties.GetValue("AccountDisabled", false).TrueThrow("账户\"{0}\"已经被禁用", strLogonName);
                    (userObj.Properties.GetValue("AccountExpires", DateTime.MaxValue) > DateTime.Now ||
                        userObj.Properties.GetValue("AccountExpires", DateTime.MaxValue) == DateTime.MinValue).FalseThrow("账户\"{0}\"已过期", strLogonName);
                    (userObj.Properties.GetValue("AccountInspires", DateTime.MinValue) < DateTime.Now).FalseThrow("账户\"{0}\"还没有到启用时间", strLogonName);
                }
            }

            return result;
        }

        /// <summary>
        /// 获取指定对象的详细属性数据
        /// </summary>
        /// <param name="strObjType">要求查询对象的类型(可以为空，涉及多类查询)</param>
        /// <param name="strObjValues">要求查询对象数据的标识(多个之间用","分隔开)</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strParentValues">对象所在的机构标识（在对象为人员对象的时候有效，一般都为空）</param>
        /// <param name="iSoco">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strExtAttrs">所需要的扩展属性（仅仅用于strObjType为空时）</param>
        /// <returns>针对性的查询结果返回</returns>
        [WebMethod(Description = "获取指定对象的详细属性数据<br/>strObjType:要求查询对象的类型(可以为空，涉及多类查询)<br/>strObjValues:要求查询对象数据的标识(多个之间用\",\"分隔开)<br/>iSoc:查询要求的查询列名称,（0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）<br/>strParentValues:对象所在的机构标识（在对象为人员对象的时候有效，一般都为空）<br/>iSoco:查询要求的查询列名称，（0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）<br/>strExtAttrs:所需要的扩展属性（仅仅用于strObjType为空时）<br/>")]
        public DataSet GetObjectsDetail(string strObjType, string strObjValues, int iSoc, string strParentValues, int iSoco, string strExtAttrs)
        {
            DataSet ds = new DataSet();

            ds.Tables.Add(GetSearchAdapter(GetSearchOUIDType(iSoc), SplitSchemaTypes(strObjType), SplitObjectValues(strObjValues), false).Query());

            return ds;
        }

        /// <summary>
        /// 按照一定的查询条件查询系统中的数据对象
        /// </summary>
        /// <param name="strOrgValues">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
        /// <param name="iSoc">查询要求的查询列名称（0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）［该参数配合strOrgValues使用，用于指定strOrgValues对应数据库的中字段名称］</param>
        /// <param name="iLot">要求查询的数据对象类型（机构、组、人员、兼职对象）</param>
        /// <param name="iLod">是否包含被逻辑删除的成员</param>
        /// <param name="iDepth">要求查询的层次（最少一层）（0代表全部子对象）</param>
        /// <param name="strOrgRankCodeName">查询中要求的机构对象级别，默认是空串</param>
        /// <param name="strUserRankCodeName">查询中要求的人员对象级别，默认是空串</param>
        /// <param name="strHideType">查询中要求屏蔽的数据(对应于配置文件HideTypes.xml中的配置)，默认是空串</param>
        /// <param name="strAttrs">查询中要求获取数据对象的属性类型</param>
        /// <returns>查询结果。</returns>
        [WebMethod(Description = @"按照一定的查询条件查询系统中的数据对象<br />
strOrgValues:要求查询的部门对象(父部门标识,多个之间采用"",""分隔) <br />
iSoc:查询要求的查询列名称（0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）［该参数配合strOrgValues使用，用于指定strOrgValues对应数据库的中字段名称］<br/>
iLot:要求查询的数据对象类型（机构、组、人员、兼职对象）<br />
iLod:是否包含被逻辑删除的成员<br/>
iDepth:要求查询的层次（最少一层）（0代表全部子对象）<br />
strOrgRankCodeName:查询中要求的机构对象级别，默认是空串.<br />
strUserRankCodeName:查询中要求的人员对象级别，默认是空串<br />
strHideType:查询中要求屏蔽的数据(对应于配置文件HideTypes.xml中的配置)，默认是空串<br />
strAttrs:查询中要求获取数据对象的属性类型
")]
        public DataSet GetOrganizationChildren(string strOrgValues, int iSoc, int iLot, int iLod, int iDepth, string strOrgRankCodeName, string strUserRankCodeName, string strHideType, string strAttrs)
        {
            SchemaParameter sp = new SchemaParameter(iLot);

            DataSet ds = new DataSet();

            ds.Tables.Add(GetQueryChildrenAdapter(GetSearchOUIDType(iSoc), sp.SchemaTypes, SplitObjectValues(strOrgValues), iDepth == 0, sp.IncludeNonDefault, GetListObjectMask(iLod) != ListObjectMask.Common).Query());

            return ds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strOrgValues">指定父机构（多个之间采用","分隔,空就采用默认）</param>
        /// <param name="iSoc">查询要求的查询列名称（ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
        /// <param name="strLikeName">名称中的（模糊匹配对象）</param>
        /// <param name="bLike">是否采用模糊匹配</param>
        /// <param name="strAttr">要求获取的字段</param>
        /// <param name="iListObjType">要求查询的对象类型</param>
        /// <param name="iLod">是否查询逻辑删除的对象</param>
        /// <param name="iDep">查询深度</param>
        /// <param name="strHideType">要求屏蔽的类型设置</param>
        /// <param name="rtnRowLimit">返回的记录数</param>
        /// <returns></returns>
        [WebMethod]
        public DataSet QueryOGUByCondition3(string strOrgValues, int iSoc, string strLikeName, bool bLike, string strAttr, int iListObjType, int iLod, int iDep, string strHideType, int rtnRowLimit)
        {
            SchemaParameter sp = new SchemaParameter(iListObjType);

            DataSet ds = new DataSet();

            ds.Tables.Add(GetSearchChildrenAdapter(GetSearchOUIDType(iSoc), sp.SchemaTypes, SplitObjectValues(strOrgValues), strLikeName, rtnRowLimit, iDep == 0, sp.IncludeNonDefault, GetListObjectMask(iLod) != ListObjectMask.Common).Query());

            return ds;
        }

        /// <summary>
        /// 获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）
        /// </summary>
        /// <param name="strGroupValues">要求查询的人员组对象标识（多个之间采用","分隔）</param>
        /// <param name="iSocg">查询要求的查询列名称（ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
        /// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
        /// <param name="strOrgValues">要求查询数据的机构范围(多个之间采用","分隔， 空的时候表示无机构要求)</param>
        /// <param name="iSoco">查询要求的查询列名称（ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
        /// <param name="strUserRankCodeName">对人员要求的最低行政级别</param>
        /// <param name="iLod">本次查询中要求查询对象的状态信息数据（是否包含逻辑删除对象）</param>
        /// <returns>指定人员组中的所有成员</returns>
        [WebMethod]
        public DataSet GetUsersInGroups(string strGroupValues, int iSocg, string strAttrs, string strOrgValues, int iSoco, string strUserRankCodeName, int iLod)
        {
            DataSet ds = new DataSet();

            ds.Tables.Add(GetContainsUsersAdapter(SchemaInfo.FilterByCategory("Groups").ToSchemaNames(), GetSearchOUIDType(iSocg), SplitObjectValues(strGroupValues), GetListObjectMask(iLod) != ListObjectMask.Common).Query());

            return ds;
        }

        /// <summary>
        /// 获取指定用户所从属的"人员组"集合
        /// </summary>
        /// <param name="strUserValues">指定的用户标识（多个之间采用“,”分隔）</param>
        /// <param name="iSocu">查询要求的查询列名称（ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
        /// <param name="strParentValue">指定的用户所在部门（用于区别兼职问题）</param>
        /// <param name="iSoco">查询要求的查询列名称（ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
        /// <param name="strAttrs">所要求获取的属性信息</param>
        /// <param name="iLod">本次查询中要求查询对象的状态信息数据（是否包含逻辑删除对象）</param>
        /// <returns>指定用户所从属的"人员组"集合</returns>
        [WebMethod]
        public DataSet GetGroupsOfUsers(string strUserValues, int iSocu, string strParentValue, int iSoco, string strAttrs, int iLod)
        {
            DataSet ds = new DataSet();

            ds.Tables.Add(GetUserContainersAdapter(SchemaInfo.FilterByCategory("Groups").ToSchemaNames(), GetSearchOUIDType(iSocu), SplitObjectValues(strUserValues), GetListObjectMask(iLod) != ListObjectMask.Common).Query());

            return ds;
        }

        /// <summary>
        /// 获取指定领导的所有秘书人成员
        /// </summary>
        /// <param name="strLeaderValues">指定领导的标识（多个之间采用","分隔）</param>
        /// <param name="iSoc">查询要求的查询列名称（ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
        /// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
        /// <param name="iLod">本次查询中要求查询对象的状态信息数据（是否包含逻辑删除对象）</param>
        /// <returns>指定领导的所有秘书人成员</returns>
        [WebMethod]
        public DataSet GetSecretariesOfLeaders(string strLeaderValues, int iSoc, string strAttrs, int iLod)
        {
            DataSet ds = new DataSet();

            ds.Tables.Add(GetContainsMembersAdapter(new string[] { "Users" }, GetSearchOUIDType(iSoc), SplitObjectValues(strLeaderValues), GetListObjectMask(iLod) != ListObjectMask.Common).Query());

            return ds;
        }

        /// <summary>
        /// 获取指定秘书的所有领导人成员
        /// </summary>
        /// <param name="strSecValues">指定秘书的标识（多个之间采用","分隔）</param>
        /// <param name="iSoc">查询要求的查询列名称（ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
        /// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
        /// <param name="iLod">本次查询中要求查询对象的状态信息数据（是否包含逻辑删除对象）</param>
        /// <returns>指定秘书的所有领导人成员</returns>
        [WebMethod]
        public DataSet GetLeadersOfSecretaries(string strSecValues, int iSoc, string strAttrs, int iLod)
        {
            DataSet ds = new DataSet();

            ds.Tables.Add(GetMemberContainersAdapter(SchemaInfo.FilterByCategory("Users").ToSchemaNames(), GetSearchOUIDType(iSoc), SplitObjectValues(strSecValues), GetListObjectMask(iLod) != ListObjectMask.Common).Query());

            return ds;
        }

        [WebMethod]
        public void RemoveAllCache()
        {
            SCCacheHelper.InvalidateAllCache();
        }
        #endregion Web Methods

        //internal static void InternalRemoveAllCache()
        //{
        //    string[] cacheQueueType = {
        //                                "MCS.Library.OGUPermission.OguObjectIDCache, MCS.Library.OGUPermission", 
        //                                "MCS.Library.OGUPermission.OguObjectFullPathCache, MCS.Library.OGUPermission", 
        //                                "MCS.Library.OGUPermission.OguObjectLogOnNameCache, MCS.Library.OGUPermission",
        //                                "PermissionCenter.Caching.ServiceMethodCache, PermissionCenterServices"
        //                              };

        //    CacheNotifyData[] data = new CacheNotifyData[cacheQueueType.Length];

        //    for (int i = 0; i < cacheQueueType.Length; i++)
        //    {
        //        data[i] = new CacheNotifyData();
        //        data[i].CacheQueueTypeDesp = cacheQueueType[i];
        //        data[i].NotifyType = CacheNotifyType.Clear;
        //    }

        //    UdpCacheNotifier.Instance.SendNotify(data);
        //}

        internal static QueryByIDsAdapterBase<DataTable> GetSearchAdapter(SearchOUIDType idType, string[] schemaTypes, string[] ids, bool includeDeleted)
        {
            QueryByIDsAdapterBase<DataTable> result = null;

            switch (idType)
            {
                case SearchOUIDType.LogOnName:
                    result = new QueryByCodeNamesReturnTableAdapter(schemaTypes, ids, includeDeleted);
                    break;
                case SearchOUIDType.Guid:
                    result = new QueryByGuidsReturnTableAdapter(schemaTypes, ids, includeDeleted);
                    break;
                case SearchOUIDType.FullPath:
                    result = new QueryByFullPathsReturnTableAdapter(schemaTypes, ids, includeDeleted);
                    break;
            }

            return result;
        }

        private static QueryByIDsAdapterBase<DataTable> GetQueryChildrenAdapter(SearchOUIDType idType, string[] schemaTypes, string[] ids, bool recursively, bool includeNonDefault, bool includeDeleted)
        {
            QueryByIDsAdapterBase<DataTable> result = null;

            switch (idType)
            {
                case SearchOUIDType.LogOnName:
                    result = new QueryChildrenByCodeNamesReturnTableAdapter(schemaTypes, ids, recursively, includeNonDefault, includeDeleted);
                    break;
                case SearchOUIDType.Guid:
                    result = new QueryChildrenByGuidsReturnTableAdapter(schemaTypes, ids, recursively, includeNonDefault, includeDeleted);
                    break;
                case SearchOUIDType.FullPath:
                    result = new QueryChildrenByFullPathsReturnTableAdapter(schemaTypes, ids, recursively, includeNonDefault, includeDeleted);
                    break;
                default:
                    throw new NotSupportedException("不支持此查询方式");
            }

            return result;
        }

        private static QueryByIDsAdapterBase<DataTable> GetSearchChildrenAdapter(SearchOUIDType idType, string[] schemaTypes, string[] ids, string keyword, int maxCount, bool recursively, bool includeNonDefault, bool includeDeleted)
        {
            QueryByIDsAdapterBase<DataTable> result = null;

            switch (idType)
            {
                case SearchOUIDType.LogOnName:
                    result = new SearchChildrenByCodeNamesReturnTableAdapter(schemaTypes, ids, keyword, maxCount, recursively, includeNonDefault, includeDeleted);
                    break;
                case SearchOUIDType.Guid:
                    result = new SearchChildrenByGuidsReturnTableAdapter(schemaTypes, ids, keyword, maxCount, recursively, includeNonDefault, includeDeleted);
                    break;
                case SearchOUIDType.FullPath:
                    result = new SearchChildrenByFullPathsReturnTableAdapter(schemaTypes, ids, keyword, maxCount, recursively, includeNonDefault, includeDeleted);
                    break;
            }

            return result;
        }

        private static QueryByIDsAdapterBase<DataTable> GetContainsUsersAdapter(string[] schemaTypes, SearchOUIDType idType, string[] ids, bool includeDeleted)
        {
            QueryByIDsAdapterBase<DataTable> result = null;

            switch (idType)
            {
                case SearchOUIDType.LogOnName:
                    result = new QueryContainersUsersByCodeNamesReturnTableAdapter(schemaTypes, ids, includeDeleted);
                    break;
                case SearchOUIDType.Guid:
                    result = new QueryContainersUsersByGuidsReturnTableAdapter(schemaTypes, ids, includeDeleted);
                    break;
                case SearchOUIDType.FullPath:
                    result = new QueryContainersUsersByFullPathsReturnTableAdapter(schemaTypes, ids, includeDeleted);
                    break;
            }

            return result;
        }

        internal static QueryByIDsAdapterBase<DataTable> GetUserContainersAdapter(string[] schemaTypes, SearchOUIDType idType, string[] ids, bool includeDeleted)
        {
            QueryByIDsAdapterBase<DataTable> result = null;

            switch (idType)
            {
                case SearchOUIDType.LogOnName:
                    result = new QueryUserBelongToContainersByCodeNamesReturnTableAdapter(schemaTypes, ids, includeDeleted);
                    break;
                case SearchOUIDType.Guid:
                    result = new QueryUserBelongToContainersByGuidsReturnTableAdapter(schemaTypes, ids, includeDeleted);
                    break;
                case SearchOUIDType.FullPath:
                    result = new QueryUserBelongToContainersByFullPathsReturnTableAdapter(schemaTypes, ids, includeDeleted);
                    break;
            }

            return result;
        }

        private static QueryByIDsAdapterBase<DataTable> GetContainsMembersAdapter(string[] schemaTypes, SearchOUIDType idType, string[] ids, bool includeDeleted)
        {
            QueryByIDsAdapterBase<DataTable> result = null;

            switch (idType)
            {
                case SearchOUIDType.LogOnName:
                    result = new QueryContainersMembersByCodeNamesReturnTableAdapter(schemaTypes, ids, includeDeleted);
                    break;
                case SearchOUIDType.Guid:
                    result = new QueryContainersMembersByGuidsReturnTableAdapter(schemaTypes, ids, includeDeleted);
                    break;
                case SearchOUIDType.FullPath:
                    result = new QueryContainersMembersByFullPathsReturnTableAdapter(schemaTypes, ids, includeDeleted);
                    break;
            }

            return result;
        }

        private static QueryByIDsAdapterBase<DataTable> GetMemberContainersAdapter(string[] schemaTypes, SearchOUIDType idType, string[] ids, bool includeDeleted)
        {
            QueryByIDsAdapterBase<DataTable> result = null;

            switch (idType)
            {
                case SearchOUIDType.LogOnName:
                    result = new QueryMemberBelongToContainersByCodeNamesReturnTableAdapter(schemaTypes, ids, includeDeleted);
                    break;
                case SearchOUIDType.Guid:
                    result = new QueryMemberBelongToContainersByGuidsReturnTableAdapter(schemaTypes, ids, includeDeleted);
                    break;
                case SearchOUIDType.FullPath:
                    result = new QueryMemberBelongToContainersByFullPathsReturnTableAdapter(schemaTypes, ids, includeDeleted);
                    break;
            }

            return result;
        }

        private static ListObjectMask GetListObjectMask(int iLod)
        {
            ListObjectMask result = ListObjectMask.Common;

            Enum.TryParse(iLod.ToString(), out result);

            return result;
        }

        private static SearchOUIDType GetSearchOUIDType(int iSoc)
        {
            SearchOUIDType result = SearchOUIDType.None;

            if (Enum.TryParse(iSoc.ToString(), out result) == false)
                throw new ApplicationException(string.Format("\"{0}\"不是能够接受的查询ID类型", iSoc));

            return result;
        }

        private static string[] SplitSchemaTypes(string strObjType)
        {
            string[] result = null;

            if (strObjType.IsNotEmpty())
                result = strObjType.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            else
                result = new string[0];

            return result;
        }

        internal static string[] SplitObjectValues(string strObjValues)
        {
            string[] result = null;

            if (strObjValues.IsNotEmpty())
                result = strObjValues.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            else
                result = new string[0];

            return result;
        }
    }
}
