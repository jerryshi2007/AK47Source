#region using

using System;
using System.Xml;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;

using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Accredit.SoapControl;

#endregion

namespace MCS.Applications.AccreditAdmin.Services
{
    /// <summary>
    /// OGUReadServices 的摘要说明。
    /// </summary>
    public class OGUReaderService : System.Web.Services.WebService
    {
        public OGUReaderService()
        {
            InitializeComponent();
        }


        #region webService提供的方法

        #region GetOrganizationChildren
        /// <summary>
        /// 获取指定部门下的所有子对象
        /// </summary>
        /// <param name="strOrgValues">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="iLot">要求查询的数据对象类型（机构、组、人员、兼职对象）</param>
        /// <param name="iLod">是否包含被逻辑删除的成员(1、普通对象；2、被直接删除对象；4、部门级联逻辑删除对象；8、人员级联逻辑删除对象)</param>
        /// <param name="iDepth">要求查询的层次（最少一层）（0代表全部子对象）</param>
        /// <param name="strOrgRankCodeName">查询中要求的机构对象级别</param>
        /// <param name="strUserRankCodeName">查询中要求的人员对象级别</param>
        /// <param name="strHideType">查询中要求屏蔽的数据(对应于配置文件HideTypes.xml中的配置)</param>
        /// <param name="strAttrs">查询中要求获取数据对象的属性类型</param>
        /// <returns>获取指定部门下的所有子对象的查询结果</returns>
        [WebMethod]
        public DataSet GetOrganizationChildren(string strOrgValues, int iSoc, int iLot, int iLod, int iDepth, string strOrgRankCodeName, string strUserRankCodeName, string strHideType, string strAttrs)
        {
            SearchObjectColumn soc = (SearchObjectColumn)iSoc;
            return OGUReader.GetOrganizationChildren(strOrgValues, soc, iLot, iLod, iDepth, strOrgRankCodeName, strUserRankCodeName, strHideType, strAttrs);
        }

        /// <summary>
        /// 获取指定部门下的所有子对象
        /// </summary>
        /// <param name="strOrgValues">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="iLot">要求查询的数据对象类型（机构、组、人员、兼职对象）</param>
        /// <param name="iLod">是否包含被逻辑删除的成员(1、普通对象；2、被直接删除对象；4、部门级联逻辑删除对象；8、人员级联逻辑删除对象)</param>
        /// <param name="iDepth">要求查询的层次（最少一层）（0代表全部子对象）</param>
        /// <param name="strOrgRankCodeName">查询中要求的机构对象级别</param>
        /// <param name="strUserRankCodeName">查询中要求的人员对象级别</param>
        /// <param name="strAttrs">查询中要求获取数据对象的属性类型</param>
        /// <returns>获取指定部门下的所有子对象的查询结果</returns>
        //		[WebMethod]
        //		public DataSet GetOrganizationChildren(string strOrgValues, int iSoc, int iLot, int iLod, int iDepth, string strOrgRankCodeName, string strUserRankCodeName, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetOrganizationChildren(strOrgValues, soc, iLot, iLod, iDepth, strOrgRankCodeName, strUserRankCodeName, strAttrs);
        //		}

        /// <summary>
        /// 获取指定部门下的所有子对象
        /// </summary>
        /// <param name="strOrgValues">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="iLod">是否包含被逻辑删除的成员(1、普通对象；2、被直接删除对象；4、部门级联逻辑删除对象；8、人员级联逻辑删除对象)</param>
        /// <param name="iDepth">要求查询的层次（最少一层）（0代表全部子对象）</param>
        /// <param name="strOrgRankCodeName">查询中要求的机构对象级别</param>
        /// <param name="strUserRankCodeName">查询中要求的人员对象级别</param>
        /// <param name="strAttrs">查询中要求获取数据对象的属性类型</param>
        /// <returns>获取指定部门下的所有子对象的查询结果</returns>
        //		[WebMethod]
        //		public DataSet GetOrganizationChildren(string strOrgValues, int iSoc, int iLod, int iDepth, string strOrgRankCodeName, string strUserRankCodeName, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetOrganizationChildren(strOrgValues, soc, iLod, iDepth, strOrgRankCodeName, strUserRankCodeName, strAttrs);			
        //		}

        /// <summary>
        /// 获取指定部门下的所有子对象
        /// </summary>
        /// <param name="strOrgValues">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="iDepth">要求查询的层次（最少一层）（0代表全部子对象）</param>
        /// <param name="strOrgRankCodeName">查询中要求的机构对象级别</param>
        /// <param name="strUserRankCodeName">查询中要求的人员对象级别</param>
        /// <param name="strAttrs">查询中要求获取数据对象的属性类型</param>
        /// <returns>获取指定部门下的所有子对象的查询结果</returns>
        //		[WebMethod]
        //		public DataSet GetOrganizationChildren(string strOrgValues, int iSoc, int iDepth, string strOrgRankCodeName, string strUserRankCodeName, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetOrganizationChildren(strOrgValues, soc, iDepth, strOrgRankCodeName, strUserRankCodeName, strAttrs);
        //		}

        /// <summary>
        /// 获取指定部门下的所有子对象
        /// </summary>
        /// <param name="strOrgValues">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strOrgRankCodeName">查询中要求的机构对象级别</param>
        /// <param name="strUserRankCodeName">查询中要求的人员对象级别</param>
        /// <param name="strAttrs">查询中要求获取数据对象的属性类型</param>
        /// <returns>获取指定部门下的所有子对象的查询结果</returns>
        //		[WebMethod]
        //		public DataSet GetOrganizationChildren(string strOrgValues, int iSoc, string strOrgRankCodeName, string strUserRankCodeName, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetOrganizationChildren(strOrgValues, soc, strOrgRankCodeName, strUserRankCodeName, strAttrs);
        //		}

        /// <summary>
        /// 获取指定部门下的所有子对象
        /// </summary>
        /// <param name="strOrgValues">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strUserRankCodeName">查询中要求的人员对象级别</param>
        /// <param name="strAttrs">查询中要求获取数据对象的属性类型</param>
        /// <returns>获取指定部门下的所有子对象的查询结果</returns>
        //		[WebMethod]
        //		public DataSet GetOrganizationChildren(string strOrgValues, int iSoc, string strUserRankCodeName, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetOrganizationChildren(strOrgValues, soc, strUserRankCodeName, strAttrs);
        //		}

        /// <summary>
        /// 获取指定部门下的所有子对象
        /// </summary>
        /// <param name="strOrgValues">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strAttrs">查询中要求获取数据对象的属性类型</param>
        /// <returns>获取指定部门下的所有子对象的查询结果</returns>
        //		[WebMethod]
        //		public DataSet GetOrganizationChildren(string strOrgValues, int iSoc, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetOrganizationChildren(strOrgValues, soc, strAttrs);
        //		}

        /// <summary>
        /// 获取指定部门下的所有子对象
        /// </summary>
        /// <param name="strOrgGuids">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
        /// <param name="strAttrs">查询中要求获取数据对象的属性类型</param>
        /// <returns>获取指定部门下的所有子对象的查询结果</returns>
        //		[WebMethod]
        //		public DataSet GetOrganizationChildren(string strOrgGuids, string strAttrs)
        //		{
        //			return OGUReader.GetOrganizationChildren(strOrgGuids, strAttrs);		
        //		}

        /// <summary>
        /// 获取指定部门下的所有子对象
        /// </summary>
        /// <param name="strOrgGuids">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
        /// <returns>获取指定部门下的所有子对象的查询结果</returns>
        //		[WebMethod]
        //		public DataSet GetOrganizationChildren(string strOrgGuids)
        //		{
        //			return OGUReader.GetOrganizationChildren(strOrgGuids);
        //		}

        #endregion

        #region IsUserInObjects
        /// <summary>
        /// 判断一个用户是否存在于指定的多个部门之中
        /// </summary>
        /// <param name="strUserValue">用户的属性数据值</param>
        /// <param name="socu">用户的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strUserParentGuid">指定用户所在的父部门标识（可空）</param>
        /// <param name="objectXmlDoc">判断对象的属性数据值</param>
        /// <param name="soco">机构的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="iLod">是否包含被逻辑删除的成员(1、普通对象；2、被直接删除对象；4、部门级联逻辑删除对象；8、人员级联逻辑删除对象)</param>
        /// <param name="strHideType">查询中要求屏蔽的数据(对应于配置文件HideTypes.xml中的配置)</param>
        /// <param name="bDirect">是否直接从属（无中间部门）</param>
        /// <param name="bFitAll">是否要求完全匹配（存在于指定的每一个部门中）</param>
        /// <returns>判断一个用户是否存在于指定的多个部门之中</returns>
        [WebMethod]
        public bool IsUserInObjects(string strUserValue, int iSocu, string strUserParentGuid, XmlDocument objectXmlDoc, int iSoco, int iLod, string strHideType, bool bDirect, bool bFitAll)
        {
            SearchObjectColumn socu = (SearchObjectColumn)iSocu;
            SearchObjectColumn soco = (SearchObjectColumn)iSoco;
            ListObjectDelete lod = (ListObjectDelete)iLod;
            return OGUReader.IsUserInObjects(strUserValue, socu, strUserParentGuid, objectXmlDoc, soco, lod, strHideType, bDirect, bFitAll);
        }

        /// <summary>
        /// 判断一个用户是否存在于指定的多个部门之中
        /// </summary>
        /// <param name="strUserValue">用户的属性数据值</param>
        /// <param name="socu">用户的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strUserParentGuid">指定用户所在的父部门标识（可空）</param>
        /// <param name="objectXmlDoc">判断对象的属性数据值</param>
        /// <param name="soco">机构的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="iLod">是否包含被逻辑删除的成员(1、普通对象；2、被直接删除对象；4、部门级联逻辑删除对象；8、人员级联逻辑删除对象)</param>
        /// <param name="bDirect">是否直接从属（无中间部门）</param>
        /// <param name="bFitAll">是否要求完全匹配（存在于指定的每一个部门中）</param>
        /// <returns>判断一个用户是否存在于指定的多个部门之中</returns>
        //		[WebMethod]
        //		public bool IsUserInObjects(string strUserValue, int iSocu, string strUserParentGuid, XmlDocument objectXmlDoc, int iSoco, int iLod, bool bDirect, bool bFitAll)
        //		{
        //			SearchObjectColumn socu = (SearchObjectColumn)iSocu;
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			ListObjectDelete lod = (ListObjectDelete)iLod;
        //			return OGUReader.IsUserInObjects(strUserValue, socu, strUserParentGuid, objectXmlDoc, soco, lod, bDirect, bFitAll);
        //		}

        /// <summary>
        /// 判断一个用户是否存在于指定的多个部门之中
        /// </summary>
        /// <param name="strUserValue">用户的属性数据值</param>
        /// <param name="socu">用户的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strUserParentGuid">指定用户所在的父部门标识（可空）</param>
        /// <param name="objectXmlDoc">判断对象的属性数据值</param>
        /// <param name="soco">机构的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="bDirect">是否直接从属（无中间部门）</param>
        /// <param name="bFitAll">是否要求完全匹配（存在于指定的每一个部门中）</param>
        /// <returns>判断一个用户是否存在于指定的多个部门之中</returns>
        //		[WebMethod]
        //		public bool IsUserInObjects(string strUserValue, int iSocu, string strUserParentGuid, XmlDocument objectXmlDoc, int iSoco, bool bDirect, bool bFitAll)
        //		{
        //			SearchObjectColumn socu = (SearchObjectColumn)iSocu;
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			return OGUReader.IsUserInObjects(strUserValue, socu, strUserParentGuid, objectXmlDoc, soco, bDirect, bFitAll);
        //		}

        /// <summary>
        /// 判断一个用户是否存在于指定的多个部门之中
        /// </summary>
        /// <param name="strUserValue">用户的属性数据值</param>
        /// <param name="socu">用户的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strUserParentGuid">指定用户所在的父部门标识（可空）</param>
        /// <param name="objectXmlDoc">判断对象的属性数据值</param>
        /// <param name="soco">机构的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="bFitAll">是否要求完全匹配（存在于指定的每一个部门中）</param>
        /// <returns>判断一个用户是否存在于指定的多个部门之中</returns>
        //		[WebMethod]
        //		public bool IsUserInObjects(string strUserValue, int iSocu, string strUserParentGuid, XmlDocument objectXmlDoc, int iSoco, bool bFitAll)
        //		{
        //			SearchObjectColumn socu = (SearchObjectColumn)iSocu;
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			return OGUReader.IsUserInObjects(strUserValue, socu, strUserParentGuid, objectXmlDoc, soco, bFitAll);
        //		}

        /// <summary>
        /// 判断一个用户是否存在于指定的多个部门之中
        /// </summary>
        /// <param name="strUserValue">用户的属性数据值</param>
        /// <param name="socu">用户的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strUserParentGuid">指定用户所在的父部门标识（可空）</param>
        /// <param name="objectXmlDoc">判断对象的属性数据值</param>
        /// <param name="soco">机构的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <returns>判断一个用户是否存在于指定的多个部门之中</returns>
        //		[WebMethod]
        //		public bool IsUserInObjects(string strUserValue, int iSocu, string strUserParentGuid, XmlDocument objectXmlDoc, int iSoco)
        //		{
        //			SearchObjectColumn socu = (SearchObjectColumn)iSocu;
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			return OGUReader.IsUserInObjects(strUserValue, socu, strUserParentGuid, objectXmlDoc, soco);
        //		}

        /// <summary>
        /// 判断一个用户是否存在于指定的多个部门之中
        /// </summary>
        /// <param name="strUserGuid">用户的属性数据值</param>
        /// <param name="strUserParentGuid">指定用户所在的父部门标识（可空）</param>
        /// <param name="objectXmlDoc">判断对象的属性数据值</param>
        /// <param name="soco">机构的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <returns>判断一个用户是否存在于指定的多个部门之中</returns>
        //		[WebMethod]
        //		public bool IsUserInObjects(string strUserGuid, string strUserParentGuid, XmlDocument objectXmlDoc, int iSoco)
        //		{
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			return OGUReader.IsUserInObjects(strUserGuid, strUserParentGuid, objectXmlDoc, soco);
        //		}

        /// <summary>
        /// 判断一个用户是否存在于指定的多个部门之中
        /// </summary>
        /// <param name="strUserGuid">用户的属性数据值</param>
        /// <param name="strUserParentGuid">指定用户所在的父部门标识（可空）</param>
        /// <param name="objectXmlDoc">判断对象的属性数据值</param>
        /// <returns>判断一个用户是否存在于指定的多个部门之中</returns>
        //		[WebMethod]
        //		public bool IsUserInObjects(string strUserGuid, string strUserParentGuid, XmlDocument objectXmlDoc)
        //		{
        //			return OGUReader.IsUserInObjects(strUserGuid, strUserParentGuid, objectXmlDoc);
        //		}

        /// <summary>
        /// 判断一个用户是否存在于指定的多个部门之中
        /// </summary>
        /// <param name="strUserGuid">用户的属性数据值</param>
        /// <param name="objectXmlDoc">判断对象的属性数据值</param>
        /// <returns>判断一个用户是否存在于指定的多个部门之中</returns>
        //		[WebMethod]
        //		public bool IsUserInObjects(string strUserGuid, XmlDocument objectXmlDoc)
        //		{
        //			return OGUReader.IsUserInObjects(strUserGuid, objectXmlDoc);
        //		}

        #endregion

        #region CheckUserInObjects
        /// <summary>
        /// 判断用户群是否存在于指定的多个部门之中
        /// </summary>
        /// <param name="xmlUserDoc">用户群标识（采用XML方式）</param>
        /// <param name="iSocu">用户的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="xmlObjDoc">机构群（采用XML方式）</param>
        /// <param name="iSoc">机构的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="iLod">是否包含被逻辑删除的成员(1、普通对象；2、被直接删除对象；4、部门级联逻辑删除对象；8、人员级联逻辑删除对象)</param>
        /// <param name="strHideType">查询中要求屏蔽的数据(对应于配置文件HideTypes.xml中的配置)</param>
        /// <param name="bDirect">是否直接从属（无中间部门）</param>
        /// <remarks>
        /// <code>
        /// xmlUserDoc的结构如下（说明oValue必填，与socu配合使用；parentGuid可不填）：
        ///		<USERS>
        ///			<USERS oValue="" parentGuid="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</USERS>
        /// xmlObjDoc的结构如下（说明oValue必填，与soc配合使用；parentGuid可不填；rankCode可不填）：
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// xmlObjDoc的返回结果（字节点方式嵌入返回）：
        /// <OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</ORGANIZATIONS>
        ///			<GROUPS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</GROUPS>
        ///			<USERS oValue="" parentGuid="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///			</USERS>
        ///		</OBJECTS>
        /// </remarks>
        [WebMethod]
        public XmlDocument CheckUserInObjects(XmlDocument xmlUserDoc, int iSocu, XmlDocument xmlObjDoc, int iSoc, int iLod, string strHideType, bool bDirect)
        {
            SearchObjectColumn socu = (SearchObjectColumn)iSocu;
            SearchObjectColumn soc = (SearchObjectColumn)iSoc;
            ListObjectDelete lod = (ListObjectDelete)iLod;
            OGUReader.CheckUserInObjects(xmlUserDoc, socu, xmlObjDoc, soc, lod, strHideType, bDirect);

            return xmlObjDoc;
        }

        /// <summary>
        /// 判断用户群是否存在于指定的多个部门之中
        /// </summary>
        /// <param name="xmlUserDoc">用户群标识（采用XML方式）</param>
        /// <param name="iSocu">用户的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="xmlObjDoc">机构群（采用XML方式）</param>
        /// <param name="iSoc">机构的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="iLod">是否包含被逻辑删除的成员(1、普通对象；2、被直接删除对象；4、部门级联逻辑删除对象；8、人员级联逻辑删除对象)</param>
        /// <param name="bDirect">是否直接从属（无中间部门）</param>
        /// <remarks>
        /// <code>
        /// xmlUserDoc的结构如下（说明oValue必填，与socu配合使用；parentGuid可不填）：
        ///		<USERS>
        ///			<USERS oValue="" parentGuid="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</USERS>
        /// xmlObjDoc的结构如下（说明oValue必填，与soc配合使用；parentGuid可不填；rankCode可不填）：
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// xmlObjDoc的返回结果（字节点方式嵌入返回）：
        /// <OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</ORGANIZATIONS>
        ///			<GROUPS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</GROUPS>
        ///			<USERS oValue="" parentGuid="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///			</USERS>
        ///		</OBJECTS>
        /// </remarks>
        //		[WebMethod]
        //		public void CheckUserInObjects(XmlDocument xmlUserDoc, int iSocu, XmlDocument xmlObjDoc, int iSoc, int iLod, bool bDirect)
        //		{
        //			SearchObjectColumn socu = (SearchObjectColumn)iSocu;
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			ListObjectDelete lod = (ListObjectDelete)iLod;
        //			OGUReader.CheckUserInObjects(xmlUserDoc, socu, xmlObjDoc, soc, lod, bDirect);
        //		}

        /// <summary>
        /// 判断用户群是否存在于指定的多个部门之中
        /// </summary>
        /// <param name="xmlUserDoc">用户群标识（采用XML方式）</param>
        /// <param name="iSocu">用户的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="xmlObjDoc">机构群（采用XML方式）</param>
        /// <param name="iSoc">机构的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="bDirect">是否直接从属（无中间部门）</param>
        /// <remarks>
        /// <code>
        /// xmlUserDoc的结构如下（说明oValue必填，与socu配合使用；parentGuid可不填）：
        ///		<USERS>
        ///			<USERS oValue="" parentGuid="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</USERS>
        /// xmlObjDoc的结构如下（说明oValue必填，与soc配合使用；parentGuid可不填；rankCode可不填）：
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// xmlObjDoc的返回结果（字节点方式嵌入返回）：
        /// <OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</ORGANIZATIONS>
        ///			<GROUPS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</GROUPS>
        ///			<USERS oValue="" parentGuid="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///			</USERS>
        ///		</OBJECTS>
        /// </remarks>
        //		[WebMethod]
        //		public void CheckUserInObjects(XmlDocument xmlUserDoc, int iSocu, XmlDocument xmlObjDoc, int iSoc, bool bDirect)
        //		{
        //			SearchObjectColumn socu = (SearchObjectColumn)iSocu;
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			OGUReader.CheckUserInObjects(xmlUserDoc, socu, xmlObjDoc, soc, bDirect);
        //		}

        /// <summary>
        /// 判断用户群是否存在于指定的多个部门之中
        /// </summary>
        /// <param name="xmlUserDoc">用户群标识（采用XML方式）</param>
        /// <param name="iSocu">用户的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="xmlObjDoc">机构群（采用XML方式）</param>
        /// <param name="iSoc">机构的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <remarks>
        /// <code>
        /// xmlUserDoc的结构如下（说明oValue必填，与socu配合使用；parentGuid可不填）：
        ///		<USERS>
        ///			<USERS oValue="" parentGuid="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</USERS>
        /// xmlObjDoc的结构如下（说明oValue必填，与soc配合使用；parentGuid可不填；rankCode可不填）：
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// xmlObjDoc的返回结果（字节点方式嵌入返回）：
        /// <OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</ORGANIZATIONS>
        ///			<GROUPS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</GROUPS>
        ///			<USERS oValue="" parentGuid="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///			</USERS>
        ///		</OBJECTS>
        /// </remarks>
        //		[WebMethod]
        //		public void CheckUserInObjects(XmlDocument xmlUserDoc, int iSocu, XmlDocument xmlObjDoc, int iSoc)
        //		{
        //			SearchObjectColumn socu = (SearchObjectColumn)iSocu;
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			OGUReader.CheckUserInObjects(xmlUserDoc, socu, xmlObjDoc, soc);
        //		}

        /// <summary>
        /// 判断用户群是否存在于指定的多个部门之中
        /// </summary>
        /// <param name="xmlUserDoc">用户群标识（采用XML方式）</param>
        /// <param name="iSocu">用户的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="xmlObjDoc">机构群（采用XML方式）</param>
        /// <remarks>
        /// <code>
        /// xmlUserDoc的结构如下（说明oValue必填，与socu配合使用；parentGuid可不填）：
        ///		<USERS>
        ///			<USERS oValue="" parentGuid="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</USERS>
        /// xmlObjDoc的结构如下（说明oValue必填，与soc配合使用；parentGuid可不填；rankCode可不填）：
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// xmlObjDoc的返回结果（字节点方式嵌入返回）：
        /// <OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</ORGANIZATIONS>
        ///			<GROUPS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</GROUPS>
        ///			<USERS oValue="" parentGuid="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///			</USERS>
        ///		</OBJECTS>
        /// </remarks>
        //		[WebMethod]
        //		public void CheckUserInObjects(XmlDocument xmlUserDoc, int iSocu, XmlDocument xmlObjDoc)
        //		{
        //			SearchObjectColumn socu = (SearchObjectColumn)iSocu;
        //			OGUReader.CheckUserInObjects(xmlUserDoc, socu, xmlObjDoc);
        //		}

        /// <summary>
        /// 判断用户群是否存在于指定的多个部门之中
        /// </summary>
        /// <param name="xmlUserDoc">用户群标识（采用XML方式）</param>
        /// <param name="xmlObjDoc">机构群（采用XML方式）</param>
        /// <remarks>
        /// <code>
        /// xmlUserDoc的结构如下（说明oValue必填，与socu配合使用；parentGuid可不填）：
        ///		<USERS>
        ///			<USERS oValue="" parentGuid="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</USERS>
        /// xmlObjDoc的结构如下（说明oValue必填，与soc配合使用；parentGuid可不填；rankCode可不填）：
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// xmlObjDoc的返回结果（字节点方式嵌入返回）：
        /// <OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</ORGANIZATIONS>
        ///			<GROUPS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</GROUPS>
        ///			<USERS oValue="" parentGuid="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///			</USERS>
        ///		</OBJECTS>
        /// </remarks>
        //		[WebMethod]
        //		public void CheckUserInObjects(XmlDocument xmlUserDoc, XmlDocument xmlObjDoc)
        //		{
        //			OGUReader.CheckUserInObjects(xmlUserDoc, xmlObjDoc);
        //		}

        #endregion

        #region GetAllUsersInAllObjects
        /// <summary>
        /// 获取指定对象中的所有用户对象
        /// </summary>
        /// <param name="xmlObjDoc">要求被查询的数据对象</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strOrgLimitValues">要求对象所在机构的范围内（如果没有将采用系统配置数据，可空）</param>
        /// <param name="iSoco">要求所在机构的范围的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="iLot">要求被查询的数据对象类型（主要是用于辨别是否要求查询兼职人员1、机构；2、人员组；4、用户；8、兼职用户）</param>
        /// <param name="iLod">是否包含被逻辑删除的成员(1、普通对象；2、被直接删除对象；4、部门级联逻辑删除对象；8、人员级联逻辑删除对象)</param>
        /// <param name="strHideType">要求隐藏的设置类型</param>
        /// <param name="strAttrs">要求获得的数据属性</param>
        /// <returns>获取指定对象中的所有用户对象</returns>
        /// <remarks>
        /// <code>
        /// xmlObjDoc的结构：
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// </remarks>
        [WebMethod]
        public DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc, int iSoc, string strOrgLimitValues, int iSoco, int iLot, int iLod, string strHideType, string strAttrs)
        {
            SearchObjectColumn soco = (SearchObjectColumn)iSoco;
            SearchObjectColumn soc = (SearchObjectColumn)iSoc;
            ListObjectType lot = (ListObjectType)iLot;
            ListObjectDelete lod = (ListObjectDelete)iLod;
            return OGUReader.GetAllUsersInAllObjects(xmlObjDoc, soc, strOrgLimitValues, soco, lot, lod, strHideType, strAttrs);
        }

        /// <summary>
        /// 获取指定对象中的所有用户对象
        /// </summary>
        /// <param name="xmlObjDoc">要求被查询的数据对象</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strOrgLimitValues">要求对象所在机构的范围内（如果没有将采用系统配置数据，可空）</param>
        /// <param name="iSoco">要求所在机构的范围的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="iLot">要求被查询的数据对象类型（主要是用于辨别是否要求查询兼职人员1、机构；2、人员组；4、用户；8、兼职用户）</param>
        /// <param name="iLod">是否包含被逻辑删除的成员(1、普通对象；2、被直接删除对象；4、部门级联逻辑删除对象；8、人员级联逻辑删除对象)</param>
        /// <param name="strAttrs">要求获得的数据属性</param>
        /// <returns>获取指定对象中的所有用户对象</returns>
        /// <remarks>
        /// <code>
        /// xmlObjDoc的结构：
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// </remarks>
        //		[WebMethod]
        //		public DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc, int iSoc, string strOrgLimitValues, int iSoco, int iLot, int iLod, string strAttrs)
        //		{
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			ListObjectType lot = (ListObjectType)iLot;
        //			ListObjectDelete lod = (ListObjectDelete)iLod;
        //			return OGUReader.GetAllUsersInAllObjects(xmlObjDoc, soc, strOrgLimitValues, soco, lot, lod, strAttrs);
        //		}

        /// <summary>
        /// 获取指定对象中的所有用户对象
        /// </summary>
        /// <param name="xmlObjDoc">要求被查询的数据对象</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strOrgLimitValues">要求对象所在机构的范围内（如果没有将采用系统配置数据，可空）</param>
        /// <param name="iSoco">要求所在机构的范围的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="iLod">是否包含被逻辑删除的成员(1、普通对象；2、被直接删除对象；4、部门级联逻辑删除对象；8、人员级联逻辑删除对象)</param>
        /// <param name="strAttrs">要求获得的数据属性</param>
        /// <returns>获取指定对象中的所有用户对象</returns>
        /// <remarks>
        /// <code>
        /// xmlObjDoc的结构：
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// </remarks>
        //		[WebMethod]
        //		public DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc, int iSoc, string strOrgLimitValues, int iSoco, int iLod, string strAttrs)
        //		{
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			ListObjectDelete lod = (ListObjectDelete)iLod;
        //			return OGUReader.GetAllUsersInAllObjects(xmlObjDoc, soc, strOrgLimitValues, soco, lod, strAttrs);
        //		}

        /// <summary>
        /// 获取指定对象中的所有用户对象
        /// </summary>
        /// <param name="xmlObjDoc">要求被查询的数据对象</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strOrgLimitValues">要求对象所在机构的范围内（如果没有将采用系统配置数据，可空）</param>
        /// <param name="iSoco">要求所在机构的范围的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strAttrs">要求获得的数据属性</param>
        /// <returns>获取指定对象中的所有用户对象</returns>
        /// <remarks>
        /// <code>
        /// xmlObjDoc的结构：
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// </remarks>
        //		[WebMethod]
        //		public DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc, int iSoc, string strOrgLimitValues, int iSoco, string strAttrs)
        //		{
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetAllUsersInAllObjects(xmlObjDoc, soc, strOrgLimitValues, soco, strAttrs);
        //		}

        /// <summary>
        /// 获取指定对象中的所有用户对象
        /// </summary>
        /// <param name="xmlObjDoc">要求被查询的数据对象</param>
        /// <param name="strOrgLimitValues">要求对象所在机构的范围内（如果没有将采用系统配置数据，可空）</param>
        /// <param name="iSoco">要求所在机构的范围的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strAttrs">要求获得的数据属性</param>
        /// <returns>获取指定对象中的所有用户对象</returns>
        /// <remarks>
        /// <code>
        /// xmlObjDoc的结构：
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// </remarks>
        //		[WebMethod]
        //		public DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc, string strOrgLimitValues, int iSoco, string strAttrs)
        //		{
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			return OGUReader.GetAllUsersInAllObjects(xmlObjDoc, strOrgLimitValues, soco, strAttrs);
        //		}

        /// <summary>
        /// 获取指定对象中的所有用户对象
        /// </summary>
        /// <param name="xmlObjDoc">要求被查询的数据对象</param>
        /// <param name="strOrgLimitGuids">要求对象所在机构的范围内（如果没有将采用系统配置数据，可空）</param>
        /// <param name="strAttrs">要求获得的数据属性</param>
        /// <returns>获取指定对象中的所有用户对象</returns>
        /// <remarks>
        /// <code>
        /// xmlObjDoc的结构：
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// </remarks>
        //		[WebMethod]
        //		public DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc, string strOrgLimitGuids, string strAttrs)
        //		{
        //			return OGUReader.GetAllUsersInAllObjects(xmlObjDoc, strOrgLimitGuids, strAttrs);
        //		}

        /// <summary>
        /// 获取指定对象中的所有用户对象
        /// </summary>
        /// <param name="xmlObjDoc">要求被查询的数据对象</param>
        /// <param name="strAttrs">要求获得的数据属性</param>
        /// <returns>获取指定对象中的所有用户对象</returns>
        /// <remarks>
        /// <code>
        /// xmlObjDoc的结构：
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// </remarks>
        //		[WebMethod]
        //		public DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc, string strAttrs)
        //		{
        //			return OGUReader.GetAllUsersInAllObjects(xmlObjDoc, strAttrs);
        //		}

        /// <summary>
        /// 获取指定对象中的所有用户对象
        /// </summary>
        /// <param name="xmlObjDoc">要求被查询的数据对象</param>
        /// <returns>获取指定对象中的所有用户对象</returns>
        /// <remarks>
        /// <code>
        /// xmlObjDoc的结构：
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// </remarks>
        //		[WebMethod]
        //		public DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc)
        //		{
        //			return OGUReader.GetAllUsersInAllObjects(xmlObjDoc);
        //		}

        #endregion

        #region GetObjectsDetail
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
        [WebMethod]
        public DataSet GetObjectsDetail(string strObjType, string strObjValues, int iSoc, string strParentValues, int iSoco, string strExtAttrs)
        {
            SearchObjectColumn soc = (SearchObjectColumn)iSoc;
            SearchObjectColumn soco = (SearchObjectColumn)iSoco;
            return OGUReader.GetObjectsDetail(strObjType, strObjValues, soc, strParentValues, soco, strExtAttrs);
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
        /// <returns>针对性的查询结果返回</returns>
        //		[WebMethod]
        //		public DataSet GetObjectsDetail(string strObjType, string strObjValues, int iSoc, string strParentValues, int iSoco)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			return OGUReader.GetObjectsDetail(strObjType, strObjValues, soc, strParentValues, soco);
        //		}

        /// <summary>
        /// 获取指定对象的详细属性数据
        /// </summary>
        /// <param name="strObjValues">要求查询对象数据的标识(多个之间用","分隔开)</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strParentValues">对象所在的机构标识（在对象为人员对象的时候有效，一般都为空）</param>
        /// <param name="iSoco">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strExtAttrs">所需要的扩展属性（仅仅用于strObjType为空时）</param>
        /// <returns>针对性的查询结果返回</returns>
        //		[WebMethod]
        //		public DataSet GetObjectsDetail(string strObjValues, int iSoc, string strParentValues, int iSoco)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			return OGUReader.GetObjectsDetail(strObjValues, soc, strParentValues, soco);
        //		}

        /// <summary>
        /// 获取指定对象的详细属性数据
        /// </summary>
        /// <param name="strObjType">要求查询对象的类型(可以为空，涉及多类查询)</param>
        /// <param name="strObjValues">要求查询对象数据的标识(多个之间用","分隔开)</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <returns>针对性的查询结果返回</returns>
        //		[WebMethod]
        //		public DataSet GetObjectsDetail(string strObjType, string strObjValues, int iSoc)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetObjectsDetail(strObjType, strObjValues, soc);
        //		}

        /// <summary>
        /// 获取指定对象的详细属性数据
        /// </summary>
        /// <param name="strObjValues">要求查询对象数据的标识(多个之间用","分隔开)</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <returns>针对性的查询结果返回</returns>
        //		[WebMethod]
        //		public DataSet GetObjectsDetail(string strObjValues, int iSoc)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetObjectsDetail(strObjValues, soc);
        //		}

        /// <summary>
        /// 获取指定对象的详细属性数据
        /// </summary>
        /// <param name="strObjType">要求查询对象的类型(可以为空，涉及多类查询)</param>
        /// <param name="strObjGuids">要求查询对象数据的标识(多个之间用","分隔开)</param>
        /// <returns>针对性的查询结果返回</returns>
        //		[WebMethod]
        //		public DataSet GetObjectsDetail(string strObjType, string strObjGuids)
        //		{
        //			return OGUReader.GetObjectsDetail(strObjType, strObjGuids);
        //		}

        /// <summary>
        /// 获取指定对象的详细属性数据
        /// </summary>
        /// <param name="strObjGuids">要求查询对象数据的标识(多个之间用","分隔开)</param>
        /// <returns>针对性的查询结果返回</returns>
        //		[WebMethod]
        //		public DataSet GetObjectsDetail(string strObjGuids)
        //		{
        //			return OGUReader.GetObjectsDetail(strObjGuids);
        //		}

        #endregion

        #region GetRankDefine
        /// <summary>
        /// 获取行政级别定义的所有数据
        /// </summary>
        /// <param name="iObjType">查询行政级别信息上的类别(1、机构级别；2、人员级别)</param>
        /// <param name="iShowHidden">是否展现系统中的隐藏个人级别信息（有些级别信息是不能做展现的，默认情况下为0）</param>
        /// <returns>获取行政级别定义的所有数据</returns>
        [WebMethod]
        public DataSet GetRankDefine(int iObjType, int iShowHidden)
        {
            return OGUReader.GetRankDefine(iObjType, iShowHidden);
        }

        /// <summary>
        /// 获取行政级别定义的所有数据
        /// </summary>
        /// <param name="iObjType">查询行政级别信息上的类别(1、机构级别；2、人员级别)</param>
        /// <returns>获取行政级别定义的所有数据</returns>
        //		[WebMethod]
        //		public DataSet GetRankDefine(int iObjType)
        //		{
        //			return OGUReader.GetRankDefine(iObjType);
        //		}

        #endregion

        #region QueryOGUByCondition
        /// <summary>
        /// 按照不同的要求查询系统中的所有符合条件的数据
        /// </summary>
        /// <param name="strOrgValues">指定父机构（多个之间采用","分隔,空就采用默认）</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strLikeName">名称中的（模糊匹配对象）</param>
        /// <param name="bLike">是否采用模糊匹配</param>
        /// <param name="bFirstPerson">要求一把手与否</param>
        /// <param name="strOrgRankCodeName">要求查询的机构上的行政级别设置</param>
        /// <param name="strUserRankCodeName">要求查询上的用户的行政级别设置</param>
        /// <param name="strAttr">要求获取的字段</param>
        /// <param name="iListObjType">要求查询的对象类型</param>
        /// <param name="iDep">查询深度</param>
        /// <param name="strHideType">要求屏蔽的类型设置</param>
        /// <returns>按照不同的要求查询系统中的所有符合条件的数据</returns>
        [WebMethod]
        public DataSet QueryOGUByCondition(string strOrgValues, int iSoc, string strLikeName, bool bLike, bool bFirstPerson, string strOrgRankCodeName, string strUserRankCodeName, string strAttr, int iListObjType, int iDep, string strHideType)
        {
            SearchObjectColumn soc = (SearchObjectColumn)iSoc;
            return OGUReader.QueryOGUByCondition(strOrgValues, soc, strLikeName, bLike, bFirstPerson, strOrgRankCodeName, strUserRankCodeName, strAttr, iListObjType, iDep, strHideType);
        }

        //2009-05-06 删除 RANK_DEFINE 约束，调整ORIGINAL_SORT约束，增加返回行数限制
        [WebMethod]
        public DataSet QueryOGUByCondition2(string strOrgValues, int iSoc, string strLikeName, bool bLike, string strAttr, int iListObjType, int iDep, string strHideType, int rtnRowLimit)
        {
            SearchObjectColumn soc = (SearchObjectColumn)iSoc;
            return OGUReader.QueryOGUByCondition2(strOrgValues, soc, strLikeName, bLike, strAttr, iListObjType, ListObjectDelete.COMMON, iDep, strHideType, rtnRowLimit);
        }

		[WebMethod]
		public DataSet QueryOGUByCondition3(string strOrgValues, int iSoc, string strLikeName, bool bLike, string strAttr, int iListObjType, int iLod, int iDep, string strHideType, int rtnRowLimit)
		{
			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
			return OGUReader.QueryOGUByCondition2(strOrgValues, soc, strLikeName, bLike, strAttr, iListObjType, (ListObjectDelete)iLod, iDep, strHideType, rtnRowLimit);
		}

        /// <summary>
        /// 按照不同的要求查询系统中的所有符合条件的数据
        /// </summary>
        /// <param name="strOrgValues">指定父机构（多个之间采用","分隔,空就采用默认）</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strLikeName">名称中的（模糊匹配对象）</param>
        /// <param name="bFirstPerson">要求一把手与否</param>
        /// <param name="strOrgRankCodeName">要求查询的机构上的行政级别设置</param>
        /// <param name="strUserRankCodeName">要求查询上的用户的行政级别设置</param>
        /// <param name="strAttr">要求获取的字段</param>
        /// <param name="iListObjType">要求查询的对象类型</param>
        /// <param name="iDep">查询深度</param>
        /// <param name="strHideType">要求屏蔽的类型设置</param>
        /// <returns>按照不同的要求查询系统中的所有符合条件的数据</returns>
        //		[WebMethod]
        //		public DataSet QueryOGUByCondition(string strOrgValues, int iSoc, string strLikeName, bool bFirstPerson, string strOrgRankCodeName, string strUserRankCodeName, string strAttr, int iListObjType, int iDep, string strHideType)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.QueryOGUByCondition(strOrgValues, soc, strLikeName, bFirstPerson, strOrgRankCodeName, strUserRankCodeName, strAttr, iListObjType, iDep, strHideType);
        //		}

        /// <summary>
        /// 按照不同的要求查询系统中的所有符合条件的数据
        /// </summary>
        /// <param name="strOrgValues">指定父机构（多个之间采用","分隔,空就采用默认）</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strLikeName">名称中的（模糊匹配对象）</param>
        /// <param name="bFirstPerson">要求一把手与否</param>
        /// <param name="strOrgRankCodeName">要求查询的机构上的行政级别设置</param>
        /// <param name="strUserRankCodeName">要求查询上的用户的行政级别设置</param>
        /// <param name="strAttr">要求获取的字段</param>
        /// <param name="iListObjType">要求查询的对象类型</param>
        /// <param name="iDep">查询深度</param>
        /// <returns>按照不同的要求查询系统中的所有符合条件的数据</returns>
        //		[WebMethod]
        //		public DataSet QueryOGUByCondition(string strOrgValues, int iSoc, string strLikeName, bool bFirstPerson, string strOrgRankCodeName, string strUserRankCodeName, string strAttr, int iListObjType, int iDep)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.QueryOGUByCondition(strOrgValues, soc, strLikeName, bFirstPerson, strOrgRankCodeName, strUserRankCodeName, strAttr, iListObjType, iDep);
        //		}

        /// <summary>
        /// 按照不同的要求查询系统中的所有符合条件的数据
        /// </summary>
        /// <param name="strOrgValues">指定父机构（多个之间采用","分隔,空就采用默认）</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）
        /// </param>
        /// <param name="strLikeName">名称中的（模糊匹配对象）</param>
        /// <param name="bFirstPerson">要求一把手与否</param>
        /// <param name="strOrgRankCodeName">要求查询的机构上的行政级别设置</param>
        /// <param name="strUserRankCodeName">要求查询上的用户的行政级别设置</param>
        /// <param name="strAttr">要求获取的字段</param>
        /// <param name="iListObjType">要求查询的对象类型</param>
        /// <returns>按照不同的要求查询系统中的所有符合条件的数据</returns>
        //		[WebMethod]
        //		public DataSet QueryOGUByCondition(string strOrgValues, int iSoc, string strLikeName, bool bFirstPerson, string strOrgRankCodeName, string strUserRankCodeName, string strAttr, int iListObjType)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.QueryOGUByCondition(strOrgValues, soc, strLikeName, bFirstPerson, strOrgRankCodeName, strUserRankCodeName, strAttr, iListObjType);
        //		}

        /// <summary>
        /// 按照不同的要求查询系统中的所有符合条件的数据
        /// </summary>
        /// <param name="strOrgValues">指定父机构（多个之间采用","分隔,空就采用默认）</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）
        /// </param>
        /// <param name="strLikeName">名称中的（模糊匹配对象）</param>
        /// <param name="strOrgRankCodeName">要求查询的机构上的行政级别设置</param>
        /// <param name="strUserRankCodeName">要求查询上的用户的行政级别设置</param>
        /// <param name="strAttr">要求获取的字段</param>
        /// <param name="iListObjType">要求查询的对象类型</param>
        /// <returns>按照不同的要求查询系统中的所有符合条件的数据</returns>
        //		[WebMethod]
        //		public DataSet QueryOGUByCondition(string strOrgValues, int iSoc, string strLikeName, string strOrgRankCodeName, string strUserRankCodeName, string strAttr, int iListObjType)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.QueryOGUByCondition(strOrgValues, soc, strLikeName, strOrgRankCodeName, strUserRankCodeName, strAttr, iListObjType);
        //		}

        /// <summary>
        /// 按照不同的要求查询系统中的所有符合条件的数据
        /// </summary>
        /// <param name="strOrgValues">指定父机构（多个之间采用","分隔,空就采用默认）</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）
        /// </param>
        /// <param name="strLikeName">名称中的（模糊匹配对象）</param>
        /// <param name="strOrgRankCodeName">要求查询的机构上的行政级别设置</param>
        /// <param name="strUserRankCodeName">要求查询上的用户的行政级别设置</param>
        /// <param name="strAttr">要求获取的字段</param>
        /// <returns>按照不同的要求查询系统中的所有符合条件的数据</returns>
        //		[WebMethod]
        //		public DataSet QueryOGUByCondition(string strOrgValues, int iSoc, string strLikeName, string strOrgRankCodeName, string strUserRankCodeName, string strAttr)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.QueryOGUByCondition(strOrgValues, soc, strLikeName, strOrgRankCodeName, strUserRankCodeName, strAttr);
        //		}

        /// <summary>
        /// 按照不同的要求查询系统中的所有符合条件的数据
        /// </summary>
        /// <param name="strOrgValues">指定父机构（多个之间采用","分隔,空就采用默认）</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）
        /// </param>
        /// <param name="strLikeName">名称中的（模糊匹配对象）</param>
        /// <param name="strUserRankCodeName">要求查询上的用户的行政级别设置</param>
        /// <param name="strAttr">要求获取的字段</param>
        /// <returns>按照不同的要求查询系统中的所有符合条件的数据</returns>
        //		[WebMethod]
        //		public DataSet QueryOGUByCondition(string strOrgValues, int iSoc, string strLikeName, string strUserRankCodeName, string strAttr)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.QueryOGUByCondition(strOrgValues, soc, strLikeName, strUserRankCodeName, strAttr);
        //		}

        /// <summary>
        /// 按照不同的要求查询系统中的所有符合条件的数据
        /// </summary>
        /// <param name="strOrgValues">指定父机构（多个之间采用","分隔,空就采用默认）</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）
        /// </param>
        /// <param name="strLikeName">名称中的（模糊匹配对象）</param>
        /// <param name="strAttr">要求获取的字段</param>
        /// <returns>按照不同的要求查询系统中的所有符合条件的数据</returns>
        //		[WebMethod]
        //		public DataSet QueryOGUByCondition(string strOrgValues, int iSoc, string strLikeName, string strAttr)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.QueryOGUByCondition(strOrgValues, soc, strLikeName, strAttr);
        //		}

        /// <summary>
        /// 按照不同的要求查询系统中的所有符合条件的数据
        /// </summary>
        /// <param name="strOrgValues">指定父机构（多个之间采用","分隔,空就采用默认）</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）
        /// </param>
        /// <param name="strLikeName">名称中的（模糊匹配对象）</param>
        /// <returns>按照不同的要求查询系统中的所有符合条件的数据</returns>
        //		[WebMethod]
        //		public DataSet QueryOGUByCondition(string strOrgValues, int iSoc, string strLikeName)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.QueryOGUByCondition(strOrgValues, soc, strLikeName);
        //		}

        /// <summary>
        /// 按照不同的要求查询系统中的所有符合条件的数据
        /// </summary>
        /// <param name="strOrgGuids">指定父机构（多个之间采用","分隔,空就采用默认）</param>
        /// <param name="strLikeName">名称中的（模糊匹配对象）</param>
        /// <returns>按照不同的要求查询系统中的所有符合条件的数据</returns>
        //		[WebMethod]
        //		public DataSet QueryOGUByCondition(string strOrgGuids, string strLikeName)
        //		{
        //			return OGUReader.QueryOGUByCondition(strOrgGuids, strLikeName);
        //		}

        #endregion

        #region GetUsersInGroups
        /// <summary>
        /// 获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）
        /// </summary>
        /// <param name="strGroupValues">要求查询的人员组对象标识（多个之间采用","分隔）</param>
        /// <param name="iSocg">被查询对象所要求对应的数据类型（数据表字段名称）
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
        /// <param name="strOrgValues">要求查询数据的机构范围(多个之间采用","分隔， 空的时候表示无机构要求)</param>
        /// <param name="iSoco">被查询对象所要求对应的数据类型（数据表字段名称）
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strUserRankCodeName">对人员要求的最低行政级别</param>
        /// <param name="iLod">本次查询中要求查询对象的状态信息数据（是否包含逻辑删除对象）</param>
        /// <returns>获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）</returns>
        [WebMethod]
        public DataSet GetUsersInGroups(string strGroupValues, int iSocg, string strAttrs, string strOrgValues, int iSoco, string strUserRankCodeName, int iLod)
        {
            SearchObjectColumn socg = (SearchObjectColumn)iSocg;
            SearchObjectColumn soco = (SearchObjectColumn)iSoco;
            return OGUReader.GetUsersInGroups(strGroupValues, socg, strAttrs, strOrgValues, soco, strUserRankCodeName, iLod);
        }

        /// <summary>
        /// 获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）
        /// </summary>
        /// <param name="strGroupValues">要求查询的人员组对象标识（多个之间采用","分隔）</param>
        /// <param name="iSocg">被查询对象所要求对应的数据类型（数据表字段名称）
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
        /// <param name="strOrgValues">要求查询数据的机构范围(多个之间采用","分隔， 空的时候表示无机构要求)</param>
        /// <param name="iSoco">被查询对象所要求对应的数据类型（数据表字段名称）
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strUserRankCodeName">对人员要求的最低行政级别</param>
        /// <returns>获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）</returns>
        //		[WebMethod]
        //		public DataSet GetUsersInGroups(string strGroupValues, int iSocg, string strAttrs, string strOrgValues, int iSoco, string strUserRankCodeName)
        //		{
        //			SearchObjectColumn socg = (SearchObjectColumn)iSocg;
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			return OGUReader.GetUsersInGroups(strGroupValues, socg, strAttrs, strOrgValues, soco, strUserRankCodeName);
        //		}

        /// <summary>
        /// 获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）
        /// </summary>
        /// <param name="strGroupValues">要求查询的人员组对象标识（多个之间采用","分隔）</param>
        /// <param name="iSocg">被查询对象所要求对应的数据类型（数据表字段名称）
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
        /// <param name="strOrgValues">要求查询数据的机构范围(多个之间采用","分隔， 空的时候表示无机构要求)</param>
        /// <param name="iSoco">被查询对象所要求对应的数据类型（数据表字段名称）
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <returns>获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）</returns>
        //		[WebMethod]
        //		public DataSet GetUsersInGroups(string strGroupValues, int iSocg, string strAttrs, string strOrgValues, int iSoco)
        //		{
        //			SearchObjectColumn socg = (SearchObjectColumn)iSocg;
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			return OGUReader.GetUsersInGroups(strGroupValues, socg, strAttrs, strOrgValues, soco);
        //		}

        /// <summary>
        /// 获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）
        /// </summary>
        /// <param name="strGroupValues">要求查询的人员组对象标识（多个之间采用","分隔）</param>
        /// <param name="iSocg">被查询对象所要求对应的数据类型（数据表字段名称）
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
        /// <returns>获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）</returns>
        //		[WebMethod]
        //		public DataSet GetUsersInGroups(string strGroupValues, int iSocg, string strAttrs)
        //		{
        //			SearchObjectColumn socg = (SearchObjectColumn)iSocg;
        //			return OGUReader.GetUsersInGroups(strGroupValues, socg, strAttrs);
        //		}

        /// <summary>
        /// 获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）
        /// </summary>
        /// <param name="strGroupValues">要求查询的人员组对象标识（多个之间采用","分隔）</param>
        /// <param name="iSocg">被查询对象所要求对应的数据类型（数据表字段名称）
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <returns>获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）</returns>
        //		[WebMethod]
        //		public DataSet GetUsersInGroups(string strGroupValues, int iSocg)
        //		{
        //			SearchObjectColumn socg = (SearchObjectColumn)iSocg;
        //			return OGUReader.GetUsersInGroups(strGroupValues, socg);
        //		}

        /// <summary>
        /// 获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）
        /// </summary>
        /// <param name="strGroupGuids">要求查询的人员组对象标识GUID（多个GUID之间采用","分隔）</param>
        /// <returns>获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）</returns>
        //		[WebMethod]
        //		public DataSet GetUsersInGroups(string strGroupGuids)
        //		{
        //			return OGUReader.GetUsersInGroups(strGroupGuids);
        //		}

        #endregion

        #region GetGroupsOfUsers
        /// <summary>
        /// 获取指定用户所从属的"人员组"集合
        /// </summary>
        /// <param name="strUserValues">指定的用户标识（多个之间采用“,”分隔）</param>
        /// <param name="iSocu">用户的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）
        /// </param>
        /// <param name="strParentValue">指定的用户所在部门（用于区别兼职问题）</param>
        /// <param name="iSoco">机构的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）
        /// </param>
        /// <param name="strAttrs">所要求获取的属性信息</param>
        /// <param name="iLod">本次查询中要求查询对象的状态信息数据（是否包含逻辑删除对象）</param>
        /// <returns>获取指定人员的所有"人员组"集合（注意其中逻辑删除的数据对象）</returns>
        [WebMethod]
        public DataSet GetGroupsOfUsers(string strUserValues, int iSocu, string strParentValue, int iSoco, string strAttrs, int iLod)
        {
            SearchObjectColumn socu = (SearchObjectColumn)iSocu;
            SearchObjectColumn soco = (SearchObjectColumn)iSoco;
            return OGUReader.GetGroupsOfUsers(strUserValues, socu, strParentValue, soco, strAttrs, iLod);
        }

        /// <summary>
        /// 获取指定用户所从属的"人员组"集合
        /// </summary>
        /// <param name="strUserValues">指定的用户标识（多个之间采用“,”分隔）</param>
        /// <param name="iSocu">用户的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）
        /// </param>
        /// <param name="strParentValue">指定的用户所在部门（用于区别兼职问题）</param>
        /// <param name="iSoco">机构的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）
        /// </param>
        /// <param name="strAttrs">所要求获取的属性信息</param>
        /// <returns>获取指定人员的所有"人员组"集合（注意其中逻辑删除的数据对象）</returns>
        //		[WebMethod]
        //		public DataSet GetGroupsOfUsers(string strUserValues, int iSocu, string strParentValue, int iSoco, string strAttrs)
        //		{
        //			SearchObjectColumn socu = (SearchObjectColumn)iSocu;
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			return OGUReader.GetGroupsOfUsers(strUserValues, socu, strParentValue, soco, strAttrs);
        //		}

        /// <summary>
        /// 获取指定用户所从属的"人员组"集合
        /// </summary>
        /// <param name="strUserValues">指定的用户标识（多个之间采用“,”分隔）</param>
        /// <param name="iSocu">用户的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）
        /// </param>
        /// <param name="strAttrs">所要求获取的属性信息</param>
        /// <returns>获取指定人员的所有"人员组"集合（注意其中逻辑删除的数据对象）</returns>
        //		[WebMethod]
        //		public DataSet GetGroupsOfUsers(string strUserValues, int iSocu, string strAttrs)
        //		{
        //			SearchObjectColumn socu = (SearchObjectColumn)iSocu;
        //			return OGUReader.GetGroupsOfUsers(strUserValues, socu, strAttrs);
        //		}

        /// <summary>
        /// 获取指定用户所从属的"人员组"集合
        /// </summary>
        /// <param name="strUserGuids">指定的用户标识GUID（多个GUID之间采用“,”分隔）</param>
        /// <param name="strAttrs">所要求获取的属性信息</param>
        /// <returns>获取指定人员的所有"人员组"集合（注意其中逻辑删除的数据对象）</returns>
        //		[WebMethod]
        //		public DataSet GetGroupsOfUsers(string strUserGuids, string strAttrs)
        //		{
        //			return OGUReader.GetGroupsOfUsers(strUserGuids, strAttrs);
        //		}

        /// <summary>
        /// 获取指定用户所从属的"人员组"集合
        /// </summary>
        /// <param name="strUserGuids">指定的用户标识GUID（多个GUID之间采用“,”分隔）</param>
        /// <returns>获取指定人员的所有"人员组"集合（注意其中逻辑删除的数据对象）</returns>
        //		[WebMethod]
        //		public DataSet GetGroupsOfUsers(string strUserGuids)
        //		{
        //			return OGUReader.GetGroupsOfUsers(strUserGuids);
        //		}

        #endregion

        #region GetSecretariesOfLeaders
        /// <summary>
        /// 获取指定领导的所有秘书人成员
        /// </summary>
        /// <param name="strLeaderValues">指定领导的标识（多个之间采用","分隔）</param>
        /// <param name="iSoc">用户的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）
        /// </param>
        /// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
        /// <param name="iLod">本次查询中要求查询对象的状态信息数据（是否包含逻辑删除对象）</param>
        /// <returns>获取指定领导的所有秘书人成员</returns>
        [WebMethod]
        public DataSet GetSecretariesOfLeaders(string strLeaderValues, int iSoc, string strAttrs, int iLod)
        {
            SearchObjectColumn soc = (SearchObjectColumn)iSoc;
            return OGUReader.GetSecretariesOfLeaders(strLeaderValues, soc, strAttrs, iLod);
        }

        /// <summary>
        /// 获取指定领导的所有秘书人成员
        /// </summary>
        /// <param name="strLeaderValues">指定领导的标识（多个之间采用","分隔）</param>
        /// <param name="iSoc">用户的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）
        /// </param>
        /// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
        /// <returns>获取指定领导的所有秘书人成员</returns>
        //		[WebMethod]
        //		public DataSet GetSecretariesOfLeaders(string strLeaderValues, int iSoc, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetSecretariesOfLeaders(strLeaderValues, soc, strAttrs);
        //		}

        /// <summary>
        /// 获取指定领导的所有秘书人成员
        /// </summary>
        /// <param name="strLeaderGuids">指定领导的标识GUID（多个GUID之间采用","分隔）</param>
        /// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
        /// <returns>获取指定领导的所有秘书人成员</returns>
        //		[WebMethod]
        //		public DataSet GetSecretariesOfLeaders(string strLeaderGuids, string strAttrs)
        //		{
        //			return OGUReader.GetSecretariesOfLeaders(strLeaderGuids, strAttrs);
        //		}

        /// <summary>
        /// 获取指定领导的所有秘书人成员
        /// </summary>
        /// <param name="strLeaderGuids">指定领导的标识GUID（多个GUID之间采用","分隔）</param>
        /// <returns>获取指定领导的所有秘书人成员</returns>
        //		[WebMethod]
        //		public DataSet GetSecretariesOfLeaders(string strLeaderGuids)
        //		{
        //			return OGUReader.GetSecretariesOfLeaders(strLeaderGuids);
        //		}

        #endregion

        #region GetLeadersOfSecretaries
        /// <summary>
        /// 获取指定秘书的所有领导人成员
        /// </summary>
        /// <param name="strSecValues">指定秘书的标识（多个之间采用","分隔）</param>
        /// <param name="iSoc">用户的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）
        /// </param>
        /// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
        /// <param name="iLod">本次查询中要求查询对象的状态信息数据（是否包含逻辑删除对象）</param>
        /// <returns>获取指定秘书的所有领导人成员</returns>
        [WebMethod]
        public DataSet GetLeadersOfSecretaries(string strSecValues, int iSoc, string strAttrs, int iLod)
        {
            SearchObjectColumn soc = (SearchObjectColumn)iSoc;
            return OGUReader.GetLeadersOfSecretaries(strSecValues, soc, strAttrs, iLod);
        }

        /// <summary>
        /// 获取指定秘书的所有领导人成员
        /// </summary>
        /// <param name="strSecValues">指定秘书的标识（多个之间采用","分隔）</param>
        /// <param name="iSoc">用户的属性名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）
        /// </param>
        /// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
        /// <returns>获取指定秘书的所有领导人成员</returns>
        //		[WebMethod]
        //		public DataSet GetLeadersOfSecretaries(string strSecValues, int iSoc, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetLeadersOfSecretaries(strSecValues, soc, strAttrs);
        //		}

        /// <summary>
        /// 获取指定秘书的所有领导人成员
        /// </summary>
        /// <param name="strSecGuids">指定秘书的标识GUID（多个GUID之间采用","分隔）</param>
        /// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
        /// <returns>获取指定秘书的所有领导人成员</returns>
        //		[WebMethod]
        //		public DataSet GetLeadersOfSecretaries(string strSecGuids, string strAttrs)
        //		{
        //			return OGUReader.GetLeadersOfSecretaries(strSecGuids, strAttrs);
        //		}

        /// <summary>
        /// 获取指定秘书的所有领导人成员
        /// </summary>
        /// <param name="strSecGuids">指定秘书的标识GUID（多个GUID之间采用","分隔）</param>
        /// <returns>获取指定秘书的所有领导人成员</returns>
        //		[WebMethod]
        //		public DataSet GetLeadersOfSecretaries(string strSecGuids)
        //		{
        //			return OGUReader.GetLeadersOfSecretaries(strSecGuids);
        //		}

        #endregion

        #region GetObjectParentOrgs
        /// <summary>
        /// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门）
        /// </summary>
        /// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
        /// <param name="strObjValues">自身所具有的数据值(多个之间采用","分隔)</param>
        /// <param name="iSoc">自身所具有的数据类型（0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="bOnlyDirectly">是否仅仅获取最接近的机构对象</param>
        /// <param name="bWithVisiual">是否要求忽略虚拟部门</param>
        /// <param name="strOrgRankCodeName">要求最低的机构行政级别</param>
        /// <param name="strAttrs">要求获取的数据字段</param>
        /// <returns>获取指定对象的父部门对象</returns>
        [WebMethod]
        public DataSet GetObjectParentOrgs(string strObjType, string strObjValues, int iSoc, bool bOnlyDirectly, bool bWithVisiual, string strOrgRankCodeName, string strAttrs)
        {
            SearchObjectColumn soc = (SearchObjectColumn)iSoc;
            return OGUReader.GetObjectParentOrgs(strObjType, strObjValues, soc, bOnlyDirectly, bWithVisiual, strOrgRankCodeName, strAttrs);
        }

        /// <summary>
        /// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门）
        /// </summary>
        /// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
        /// <param name="strObjValues">自身所具有的数据值(多个之间采用","分隔)</param>
        /// <param name="iSoc">自身所具有的数据类型（0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="bWithVisiual">是否要求忽略虚拟部门</param>
        /// <param name="strOrgRankCodeName">要求最低的机构行政级别</param>
        /// <param name="strAttrs">要求获取的数据字段</param>
        /// <returns>获取指定对象的父部门对象</returns>
        //		[WebMethod]
        //		public DataSet GetObjectParentOrgs(string strObjType, string strObjValues, int iSoc, bool bWithVisiual, string strOrgRankCodeName, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetObjectParentOrgs(strObjType, strObjValues, soc, bWithVisiual, strOrgRankCodeName, strAttrs);
        //		}

        /// <summary>
        /// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门）
        /// </summary>
        /// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
        /// <param name="strObjValues">自身所具有的数据值(多个之间采用","分隔)</param>
        /// <param name="iSoc">自身所具有的数据类型（0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strOrgRankCodeName">要求最低的机构行政级别</param>
        /// <param name="strAttrs">要求获取的数据字段</param>
        /// <returns>获取指定对象的父部门对象</returns>
        //		[WebMethod]
        //		public DataSet GetObjectParentOrgs(string strObjType, string strObjValues, int iSoc, string strOrgRankCodeName, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetObjectParentOrgs(strObjType, strObjValues, soc, strOrgRankCodeName, strAttrs);
        //		}

        /// <summary>
        /// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门）
        /// </summary>
        /// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
        /// <param name="strObjValues">自身所具有的数据值(多个之间采用","分隔)</param>
        /// <param name="iSoc">自身所具有的数据类型
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strAttrs">要求获取的数据字段</param>
        /// <returns>获取指定对象的父部门对象</returns>
        //		[WebMethod]
        //		public DataSet GetObjectParentOrgs(string strObjType, string strObjValues, int iSoc, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetObjectParentOrgs(strObjType, strObjValues, soc, strAttrs);
        //		}

        /// <summary>
        /// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门）
        /// </summary>
        /// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
        /// <param name="strObjGuids">自身所具有的数据值GUID(多个GUID之间采用","分隔)</param>
        /// <param name="strAttrs">要求获取的数据字段</param>
        /// <returns>获取指定对象的父部门对象</returns>
        //		[WebMethod]
        //		public DataSet GetObjectParentOrgs(string strObjType, string strObjGuids, string strAttrs)
        //		{
        //			return OGUReader.GetObjectParentOrgs(strObjType, strObjGuids, strAttrs);
        //		}

        /// <summary>
        /// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门）
        /// </summary>
        /// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
        /// <param name="strObjGuids">自身所具有的数据值GUID(多个GUID之间采用","分隔)</param>
        /// <returns>获取指定对象的父部门对象</returns>
        //		[WebMethod]
        //		public DataSet GetObjectParentOrgs(string strObjType, string strObjGuids)
        //		{
        //			return OGUReader.GetObjectParentOrgs(strObjType, strObjGuids);
        //		}

        #endregion

        #region GetObjectDepOrgs
        /// <summary>
        /// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门(指定层次)）
        /// </summary>
        /// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
        /// <param name="strObjValues">自身所具有的数据值(多个之间采用","分隔)</param>
        /// <param name="iSoc">自身所具有的数据类型
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="iDep">要求获取的深度</param>
        /// <param name="strAttrs">要求获取的数据字段</param>
        /// <returns>获取指定对象的父部门对象</returns>
        [WebMethod]
        public DataSet GetObjectDepOrgs(string strObjType, string strObjValues, int iSoc, int iDep, string strAttrs)
        {
            SearchObjectColumn soc = (SearchObjectColumn)iSoc;
            return OGUReader.GetObjectDepOrgs(strObjType, strObjValues, soc, iDep, strAttrs);
        }

        /// <summary>
        /// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门(指定层次)）
        /// </summary>
        /// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
        /// <param name="strObjValues">自身所具有的数据值(多个之间采用","分隔)</param>
        /// <param name="iSoc">自身所具有的数据类型
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strAttrs">要求获取的数据字段</param>
        /// <returns>获取指定对象的父部门对象</returns>
        //		[WebMethod]
        //		public DataSet GetObjectDepOrgs(string strObjType, string strObjValues, int iSoc, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetObjectDepOrgs(strObjType, strObjValues, soc, strAttrs);
        //		}

        /// <summary>
        /// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门(指定层次)）
        /// </summary>
        /// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
        /// <param name="strObjGuids">自身所具有的数据值GUID(多个GUID之间采用","分隔)</param>
        /// <param name="strAttrs">要求获取的数据字段</param>
        /// <returns>获取指定对象的父部门对象</returns>
        //		[WebMethod]
        //		public DataSet GetObjectDepOrgs(string strObjType, string strObjGuids, string strAttrs)
        //		{
        //			return OGUReader.GetObjectDepOrgs(strObjType, strObjGuids, strAttrs);
        //		}

        /// <summary>
        /// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门(指定层次)）
        /// </summary>
        /// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
        /// <param name="strObjGuids">自身所具有的数据值GUID(多个GUID之间采用","分隔)</param>
        /// <returns>获取指定对象的父部门对象</returns>
        //		[WebMethod]
        //		public DataSet GetObjectDepOrgs(string strObjType, string strObjGuids)
        //		{
        //			return OGUReader.GetObjectDepOrgs(strObjType, strObjGuids);
        //		}

        #endregion

        #region GetRootDSE
        /// <summary>
        /// 获取系统指定的根部门
        /// </summary>
        /// <returns>获取系统指定的根部门</returns>
        [WebMethod]
        public DataSet GetRootDSE()
        {
            return OGUReader.GetRootDSE();
        }
        #endregion

        #region SignInCheck
        /// <summary>
        /// 验证人员登录名和密码是否正确
        /// </summary>
        /// <param name="strLogonName">用户登录名</param>
        /// <param name="strUserPwd">用户密码</param>
        /// <returns>登录名和密码是否匹配</returns>
        [WebMethod]
        public bool SignInCheck(string strLogonName, string strUserPwd)
        {
            return OGUReader.SignInCheck(strLogonName, strUserPwd);
        }

        #endregion

        #region UpdateUserPwd
        /// <summary>
        /// 用户修改口令接口
        /// </summary>
        /// <param name="strUserGuid">要求被修改口令的用户</param>
        /// <param name="strOldPwd">用户的旧口令</param>
        /// <param name="strNewPwd">使用的新口令</param>
        /// <param name="strConfirmPwd">新口令的确认</param>
        /// <returns>本次修改是否成功</returns>
        /*[WebMethod]
        public bool UpdateUserPwd(string strUserGuid, string strOldPwd, string strNewPwd, string strConfirmPwd)
        {
            return UpdateUserPwd(strUserGuid, (int)SearchObjectColumn.SEARCH_GUID, strOldPwd, strNewPwd, strConfirmPwd);
        }
        // del by cgac\yuan_yong 2004-11-14
        */
        /// <summary>
        /// 用户修改口令接口
        /// </summary>
        /// <param name="strUserValue">要求被修改口令的用户</param>
        /// <param name="iSoc">查询要求的查询列名称
        /// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
        /// <param name="strOldPwd">用户的旧口令</param>
        /// <param name="strNewPwd">使用的新口令</param>
        /// <param name="strConfirmPwd">新口令的确认</param>
        /// <returns>本次修改是否成功</returns>
        [WebMethod]
        public bool UpdateUserPwd(string strUserValue, int iSoc, string strOldPwd, string strNewPwd, string strConfirmPwd)
        {
            OGUControler oguControl = new OGUControler();

            SearchObjectColumn soc = (SearchObjectColumn)iSoc;

            return oguControl.UpdateUserPwd(strUserValue, soc, strOldPwd, strNewPwd, strConfirmPwd);
        }

        #endregion

        #region RemoveAllCache
        /// <summary>
        /// 清理数据缓存
        /// </summary>
        [WebMethod]
        public void RemoveAllCache()
        {
            OGUReader.RemoveAllCache();
        }
        #endregion
        #endregion

        #region 组件设计器生成的代码

        //Web 服务设计器所必需的
        private IContainer components = null;

        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
        }

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

    }
}