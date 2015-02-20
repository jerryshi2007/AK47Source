#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.OGUPermission
// FileName	：	OguServiceBroker.cs
// Remark	：	机构人员服务的WebService代理
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    沈峥	    20070430		创建
// -------------------------------------------------
#endregion
namespace MCS.Library.OGUPermission
{
	using System.Diagnostics;
	using System.Web.Services;
	using System.ComponentModel;
	using System.Web.Services.Protocols;
	using System;
	using System.Xml.Serialization;
	using System.Data;

	/// <summary>
	/// 机构人员服务的WebService代理
	/// </summary>
	//[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Web.Services.WebServiceBindingAttribute(Name = "OGUReaderServiceSoap", Namespace = "http://tempuri.org/")]
	public sealed class OguReaderServiceBroker : System.Web.Services.Protocols.SoapHttpClientProtocol
	{
		/// <summary>
		/// 机构人员服务的WebService代理实例
		/// </summary>
		public static OguReaderServiceBroker Instance
		{
			get
			{
				return new OguReaderServiceBroker();
			}
		}

		private OguReaderServiceBroker()
		{
			ServiceBrokerContext.Current.InitWebClientProtocol(this);
			this.Url = OguPermissionSettings.GetConfig().OguServiceAddress.ToString();
		}

		/// <summary>
		/// Web Service 地址。
		/// </summary>
		public new string Url
		{
			get
			{
				return base.Url;
			}
			set
			{
				base.Url = value;
			}
		}
		/// <summary>
		/// 按照一定的查询条件查询系统中的数据对象
		/// </summary>
		/// <param name="strOrgValues">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
		/// <param name="iSoc">查询要求的查询列名称（0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）［该参数配合strOrgValues使用，用于指定strOrgValues对应数据库的中字段名称］</param>
		/// <param name="iLot">要求查询的数据对象类型（机构、组、人员、兼职对象）</param>
		/// <param name="iLod">是否包含被逻辑删除的成员</param>
		/// <param name="iDepth">要求查询的层次（最少一层）（0代表全部子对象）</param>
		/// <param name="strOrgRankCodeName">查询中要求的机构对象级别</param>
		/// <param name="strUserRankCodeName">查询中要求的人员对象级别</param>
		/// <param name="strHideType">查询中要求屏蔽的数据(对应于配置文件HideTypes.xml中的配置)</param>
		/// <param name="strAttrs">查询中要求获取数据对象的属性类型</param>
		/// <returns>查询结果。</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetOrganizationChildren", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet GetOrganizationChildren(string strOrgValues, int iSoc, int iLot, int iLod, int iDepth, string strOrgRankCodeName, string strUserRankCodeName, string strHideType, string strAttrs)
		{
			object[] results = this.Invoke("GetOrganizationChildren", new object[] {
                        strOrgValues,
                        iSoc,
                        iLot,
                        iLod,
                        iDepth,
                        strOrgRankCodeName,
                        strUserRankCodeName,
                        strHideType,
                        strAttrs});
			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// 判断一个用户是否存在于指定的多个部门之中
		/// </summary>
		/// <param name="strUserValue">用户的属性数据值</param>
		/// <param name="iSocu">用户的属性名称（ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="strUserParentGuid">指定用户所在的父部门标识（可空）</param>
		/// <param name="objectXmlDoc">判断对象的属性数据值</param>
		/// <param name="iSoco">机构的属性名称（ＧＵＩＤ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="iLod">是否包含被逻辑删除的成员</param>
		/// <param name="strHideType">查询中要求屏蔽的数据(对应于配置文件HideTypes.xml中的配置)</param>
		/// <param name="bDirect">是否直接从属（无中间部门）</param>
		/// <param name="bFitAll">是否要求完全匹配（存在于指定的每一个部门中）</param>
		/// <returns>用户是否存在于指定的多个部门之中</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/IsUserInObjects", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool IsUserInObjects(string strUserValue, int iSocu, string strUserParentGuid, System.Xml.XmlNode objectXmlDoc, int iSoco, int iLod, string strHideType, bool bDirect, bool bFitAll)
		{
			object[] results = this.Invoke("IsUserInObjects", new object[] {
                        strUserValue,
                        iSocu,
                        strUserParentGuid,
                        objectXmlDoc,
                        iSoco,
                        iLod,
                        strHideType,
                        bDirect,
                        bFitAll});
			return ((bool)(results[0]));
		}

		/// <summary>
		/// 判断用户群是否存在于指定的多个部门之中
		/// </summary>
		/// <param name="xmlUserDoc">用户群标识（多个之间采用","分隔）</param>
		/// <param name="iSocu">用户的属性名称（ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="xmlObjDoc">机构群（采用XML方式）</param>
		/// <param name="iSoc">机构的属性名称（ＧＵＩＤ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="iLod">是否包含被逻辑删除的成员</param>
		/// <param name="strHideType">查询中要求屏蔽的数据(对应于配置文件HideTypes.xml中的配置)</param>
		/// <param name="bDirect">是否直接从属（无中间部门）</param>
		/// <returns>用户群是否存在于指定的多个部门之中</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/CheckUserInObjects", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Xml.XmlNode CheckUserInObjects(System.Xml.XmlNode xmlUserDoc, int iSocu, System.Xml.XmlNode xmlObjDoc, int iSoc, int iLod, string strHideType, bool bDirect)
		{
			object[] results = this.Invoke("CheckUserInObjects", new object[] {
                        xmlUserDoc,
                        iSocu,
                        xmlObjDoc,
                        iSoc,
                        iLod,
                        strHideType,
                        bDirect});
			return ((System.Xml.XmlNode)(results[0]));
		}

		/// <summary>
		/// 获取指定对象中的所有用户对象
		/// </summary>
		/// <param name="xmlObjDoc">要求被查询的数据对象</param>
		/// <param name="iSoc">查询要求的查询列名称（ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="strOrgLimitValues">要求对象所在机构的范围内（如果没有将采用系统配置数据，可空）</param>
		/// <param name="iSoco">要求所在机构的范围的查询列名称（ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="iLot">要求被查询的数据对象类型（主要是用于辨别是否要求查询兼职人员）</param>
		/// <param name="iLod">系统中被逻辑删除对象是否查询取出</param>
		/// <param name="strHideType">要求隐藏的设置类型</param>
		/// <param name="strAttrs">要求获得的数据属性</param>
		/// <returns>指定对象中的所有用户对象</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetAllUsersInAllObjects", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet GetAllUsersInAllObjects(System.Xml.XmlNode xmlObjDoc, int iSoc, string strOrgLimitValues, int iSoco, int iLot, int iLod, string strHideType, string strAttrs)
		{
			object[] results = this.Invoke("GetAllUsersInAllObjects", new object[] {
                        xmlObjDoc,
                        iSoc,
                        strOrgLimitValues,
                        iSoco,
                        iLot,
                        iLod,
                        strHideType,
                        strAttrs});
			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// 获取指定对象的详细属性数据
		/// </summary>
		/// <param name="strObjType">要求查询对象的类型(可以为空，空则采用混合查询)</param>
		/// <param name="strObjValues">要求查询对象数据的标识(多个之间用","分隔开)</param>
		/// <param name="iSoc">查询要求的查询列名称（ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="strParentValues">对象所在的机构标识（在对象为人员对象的时候有效，一般都为空）</param>
		/// <param name="iSoco">查询要求的查询列名称（ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="strExtAttrs">所需要的扩展属性（仅仅用于strObjType为空时）</param>
		/// <returns>指定对象的详细属性数据</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetObjectsDetail", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet GetObjectsDetail(string strObjType, string strObjValues, int iSoc, string strParentValues, int iSoco, string strExtAttrs)
		{
			object[] results = this.Invoke("GetObjectsDetail", new object[] {
                        strObjType,
                        strObjValues,
                        iSoc,
                        strParentValues,
                        iSoco,
                        strExtAttrs});
			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// 获取行政级别定义的所有数据
		/// </summary>
		/// <param name="iObjType">查询行政级别信息上的类别(1、机构级别；2、人员级别)</param>
		/// <param name="iShowHidden">是否展现系统中的隐藏个人级别信息（有些级别信息是不能做展现的，默认情况下为0）</param>
		/// <returns>行政级别定义的所有数据</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetRankDefine", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet GetRankDefine(int iObjType, int iShowHidden)
		{
			object[] results = this.Invoke("GetRankDefine", new object[] {
                        iObjType,
                        iShowHidden});
			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// 按照不同的要求查询系统中的所有符合条件的数据
		/// </summary>
		/// <param name="strOrgValues">指定父机构（多个之间采用","分隔,空就采用默认）</param>
		/// <param name="iSoc">查询要求的查询列名称（ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="strLikeName">名称中的（模糊匹配对象）</param>
		/// <param name="bLike">是否采用模糊匹配</param>
		/// <param name="bFirstPerson">要求一把手与否</param>
		/// <param name="strOrgRankCodeName">要求查询的机构上的行政级别设置</param>
		/// <param name="strUserRankCodeName">要求查询上的用户的行政级别设置</param>
		/// <param name="strAttr">要求获取的字段</param>
		/// <param name="iListObjType">要求查询的对象类型</param>
		/// <param name="iDep">查询深度</param>
		/// <param name="strHideType">要求屏蔽的类型设置</param>
		/// <returns>查询结果</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/QueryOGUByCondition", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet QueryOGUByCondition(string strOrgValues, int iSoc, string strLikeName, bool bLike, bool bFirstPerson, string strOrgRankCodeName, string strUserRankCodeName, string strAttr, int iListObjType, int iDep, string strHideType)
		{
			object[] results = this.Invoke("QueryOGUByCondition", new object[] {
                        strOrgValues,
                        iSoc,
                        strLikeName,
                        bLike,
                        bFirstPerson,
                        strOrgRankCodeName,
                        strUserRankCodeName,
                        strAttr,
                        iListObjType,
                        iDep,
                        strHideType});
			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// 按照不同的要求查询系统中的所有符合条件的数据
		/// </summary>
		/// <param name="strOrgValues">指定父机构（多个之间采用","分隔,空就采用默认）</param>
		/// <param name="iSoc">查询要求的查询列名称（ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="strLikeName">名称中的（模糊匹配对象）</param>
		/// <param name="bLike">是否采用模糊匹配</param>
		/// <param name="strAttr">要求获取的字段</param>
		/// <param name="iListObjType">要求查询的对象类型</param>
		/// <param name="iDep">查询深度</param>
		/// <param name="strHideType">要求屏蔽的类型设置</param>
		/// <param name="rtnRowLimit">返回的记录数</param>
		/// <returns>查询结果</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/QueryOGUByCondition2", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet QueryOGUByCondition2(string strOrgValues, int iSoc, string strLikeName, bool bLike, string strAttr, int iListObjType, int iDep, string strHideType, int rtnRowLimit)
		{
			object[] results = this.Invoke("QueryOGUByCondition2", new object[] {
                        strOrgValues,
                        iSoc,
                        strLikeName,
                        bLike,
                        strAttr,
                        iListObjType,
                        iDep,
                        strHideType,
						rtnRowLimit});

			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strOrgValues"></param>
		/// <param name="iSoc"></param>
		/// <param name="strLikeName"></param>
		/// <param name="bLike"></param>
		/// <param name="strAttr"></param>
		/// <param name="iListObjType"></param>
		/// <param name="iLod"></param>
		/// <param name="iDep"></param>
		/// <param name="strHideType"></param>
		/// <param name="rtnRowLimit"></param>
		/// <returns></returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/QueryOGUByCondition3", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet QueryOGUByCondition3(string strOrgValues, int iSoc, string strLikeName, bool bLike, string strAttr, int iListObjType, int iLod, int iDep, string strHideType, int rtnRowLimit)
		{
			object[] results = this.Invoke("QueryOGUByCondition3", new object[] {
                        strOrgValues,
                        iSoc,
                        strLikeName,
                        bLike,
                        strAttr,
                        iListObjType,
						iLod,
                        iDep,
                        strHideType,
						rtnRowLimit});

			return ((System.Data.DataSet)(results[0]));
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
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetUsersInGroups", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet GetUsersInGroups(string strGroupValues, int iSocg, string strAttrs, string strOrgValues, int iSoco, string strUserRankCodeName, int iLod)
		{
			object[] results = this.Invoke("GetUsersInGroups", new object[] {
                        strGroupValues,
                        iSocg,
                        strAttrs,
                        strOrgValues,
                        iSoco,
                        strUserRankCodeName,
                        iLod});
			return ((System.Data.DataSet)(results[0]));
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
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetGroupsOfUsers", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet GetGroupsOfUsers(string strUserValues, int iSocu, string strParentValue, int iSoco, string strAttrs, int iLod)
		{
			object[] results = this.Invoke("GetGroupsOfUsers", new object[] {
                        strUserValues,
                        iSocu,
                        strParentValue,
                        iSoco,
                        strAttrs,
                        iLod});
			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// 获取指定领导的所有秘书人成员
		/// </summary>
		/// <param name="strLeaderValues">指定领导的标识（多个之间采用","分隔）</param>
		/// <param name="iSoc">查询要求的查询列名称（ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
		/// <param name="iLod">本次查询中要求查询对象的状态信息数据（是否包含逻辑删除对象）</param>
		/// <returns>指定领导的所有秘书人成员</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetSecretariesOfLeaders", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet GetSecretariesOfLeaders(string strLeaderValues, int iSoc, string strAttrs, int iLod)
		{
			object[] results = this.Invoke("GetSecretariesOfLeaders", new object[] {
                        strLeaderValues,
                        iSoc,
                        strAttrs,
                        iLod});
			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// 获取指定秘书的所有领导人成员
		/// </summary>
		/// <param name="strSecValues">指定秘书的标识（多个之间采用","分隔）</param>
		/// <param name="iSoc">查询要求的查询列名称（ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
		/// <param name="iLod">本次查询中要求查询对象的状态信息数据（是否包含逻辑删除对象）</param>
		/// <returns>指定秘书的所有领导人成员</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetLeadersOfSecretaries", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet GetLeadersOfSecretaries(string strSecValues, int iSoc, string strAttrs, int iLod)
		{
			object[] results = this.Invoke("GetLeadersOfSecretaries", new object[] {
                        strSecValues,
                        iSoc,
                        strAttrs,
                        iLod});
			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门）
		/// </summary>
		/// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
		/// <param name="strObjValues">自身所具有的数据值</param>
		/// <param name="iSoc">查询要求的查询列名称（ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="bOnlyDirectly">是否仅仅获取最接近的机构对象</param>
		/// <param name="bWithVisiual">是否要求忽略虚拟部门</param>
		/// <param name="strOrgRankCodeName">要求最低的机构行政级别</param>
		/// <param name="strAttrs">要求获取的数据字段</param>
		/// <returns>指定对象的父机构对象</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetObjectParentOrgs", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet GetObjectParentOrgs(string strObjType, string strObjValues, int iSoc, bool bOnlyDirectly, bool bWithVisiual, string strOrgRankCodeName, string strAttrs)
		{
			object[] results = this.Invoke("GetObjectParentOrgs", new object[] {
                        strObjType,
                        strObjValues,
                        iSoc,
                        bOnlyDirectly,
                        bWithVisiual,
                        strOrgRankCodeName,
                        strAttrs});
			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门(指定层次)）
		/// </summary>
		/// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
		/// <param name="strObjValues">自身所具有的数据值</param>
		/// <param name="iSoc">查询要求的查询列名称（ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="iDep">要求获取的深度</param>
		/// <param name="strAttrs">要求获取的数据字段</param>
		/// <returns>指定对象的父机构对象</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetObjectDepOrgs", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet GetObjectDepOrgs(string strObjType, string strObjValues, int iSoc, int iDep, string strAttrs)
		{
			object[] results = this.Invoke("GetObjectDepOrgs", new object[] {
                        strObjType,
                        strObjValues,
                        iSoc,
                        iDep,
                        strAttrs});
			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// 获取系统指定的根部门对象
		/// </summary>
		/// <returns>系统指定的根部门对象</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetRootDSE", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet GetRootDSE()
		{
			object[] results = this.Invoke("GetRootDSE", new object[0]);
			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// 登录。
		/// </summary>
		/// <param name="strLogonName">登录名称。</param>
		/// <param name="strUserPwd">密码。</param>
		/// <returns>登录是否成功。</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SignInCheck", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool SignInCheck(string strLogonName, string strUserPwd)
		{
			object[] results = this.Invoke("SignInCheck", new object[] {
                        strLogonName,
                        strUserPwd});
			return ((bool)(results[0]));
		}

		/// <summary>
		/// 修改密码。
		/// </summary>
		/// <param name="strUserValue">指定的用户标识</param>
		/// <param name="iSoc">查询要求的查询列名称（ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="strOldPwd">旧密码</param>
		/// <param name="strNewPwd">新密码</param>
		/// <param name="strConfirmPwd">确认密码</param>
		/// <returns>是否修改成功</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/UpdateUserPwd", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool UpdateUserPwd(string strUserValue, int iSoc, string strOldPwd, string strNewPwd, string strConfirmPwd)
		{
			object[] results = this.Invoke("UpdateUserPwd", new object[] {
                        strUserValue,
                        iSoc,
                        strOldPwd,
                        strNewPwd,
                        strConfirmPwd});
			return ((bool)(results[0]));
		}

		/// <summary>
		/// 去除所有缓存。
		/// </summary>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/RemoveAllCache", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public void RemoveAllCache()
		{
			this.Invoke("RemoveAllCache", new object[0]);
		}
	}
}
