#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.OGUPermission
// FileName	：	AppAdminServiceBroker.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    沈峥	    20070430		创建
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.OGUPermission
{
    using System.Diagnostics;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.Data;

    /// <summary>
    /// 授权类型。
    /// </summary>
    [System.FlagsAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.312")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public enum RightMaskType
    {

        /// <summary>
        /// 自授权。
        /// </summary>
        Self = 1,

        /// <summary>
        /// 应用授权。
        /// </summary>
        App = 2,

        /// <summary>
        /// 全部，包括 自授权 和 应用授权。
        /// </summary>
        All = 4,
    }

    /// <summary>
    /// 权限委派类型。
    /// </summary>
    [System.FlagsAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.312")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public enum DelegationMaskType
    {

        /// <summary>
        /// 原始权限。
        /// </summary>
        Original = 1,

        /// <summary>
        /// 被委派权限。
        /// </summary>
        Delegated = 2,

        /// <summary>
        /// 全部，包括 原始权限 和 被委派权限。
        /// </summary>
        All = 4,
    }

    /// <summary>
    /// 兼职类型。
    /// </summary>
    [System.FlagsAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.312")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public enum SidelineMaskType
    {

        /// <summary>
        /// 不兼职。
        /// </summary>
        NotSideline = 1,

        /// <summary>
        /// 兼职。
        /// </summary>
        Sideline = 2,

        /// <summary>
        /// 全部。
        /// </summary>
        All = 4,
    }

    /// <summary>
    /// 身份标志类型。
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.312")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public enum UserValueType
    {
        /// <summary>
        /// 登录名
        /// </summary>
        LogonName = 1,
        /// <summary>
        /// 人员全路径
        /// </summary>
        AllPath = 2,
        /// <summary>
        /// 人员编号
        /// </summary>
        PersonID = 3,
        /// <summary>
        /// IC卡号
        /// </summary>
        ICCode = 4,
        /// <summary>
        /// 人员Guid值
        /// </summary>
        Guid = 8,
        /// <summary>
        /// 根据唯一索引查询(为配合南京海关统一平台切换，新增加字段ID[自增唯一字段])
        /// </summary>
        Identity = 16
    }

    /// <summary>
    /// 权限服务范围类型。
    /// </summary>
    [System.FlagsAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.312")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public enum ScopeMaskType
    {

        /// <summary>
        /// 机构范围。
        /// </summary>
        OrgScope = 1,

        /// <summary>
        /// 数据范围
        /// </summary>
        DataScope = 2,

        /// <summary>
        /// 全部
        /// </summary>
        All = 4,
    }

    /// <summary>
    /// 授权管理WebService的代理
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.312")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "AppSecurityCheckServiceSoap", Namespace = "http://tempuri.org/")]
    public sealed class AppAdminServiceBroker : System.Web.Services.Protocols.SoapHttpClientProtocol
    {
        /// <summary>
        /// 授权管理WebService的代理实例
        /// </summary>
		public static AppAdminServiceBroker Instance
		{
			get
			{
				return new AppAdminServiceBroker();
			}
		}

        private AppAdminServiceBroker()
        {
			ServiceBrokerContext.Current.InitWebClientProtocol(this);
            this.Url = OguPermissionSettings.GetConfig().AppAdminServiceAddress.ToString();
        }

        /// <summary>
        /// 获得所有应用系统的基本信息。
        /// </summary>
        /// <returns>所有应用系统的基本信息。</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetApplications", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetApplications()
        {
            object[] results = this.Invoke("GetApplications", new object[0]);
            return ((System.Data.DataSet)(results[0]));
        }

        /// <summary>
        /// 查询指定应用系统中，指定类别的所有角色
        /// </summary>
        /// <param name="appCodeName">应用的英文标识</param>
        /// <param name="rightMask">权限授权类型</param>
        /// <returns>指定应用系统中，指定类别的所有角色</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetRoles", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetRoles(string appCodeName, RightMaskType rightMask)
        {
            object[] results = this.Invoke("GetRoles", new object[] {
                        appCodeName,
                        rightMask});
            return ((System.Data.DataSet)(results[0]));
        }

        /// <summary>
        /// 查询指定应用系统中，指定类别的所有功能
        /// </summary>
        /// <param name="appCodeName">应用的英文标识</param>
        /// <param name="rightMask">权限授权类型</param>
        /// <returns>指定应用系统中，指定类别的所有功能</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetFunctions", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetFunctions(string appCodeName, RightMaskType rightMask)
        {
            object[] results = this.Invoke("GetFunctions", new object[] {
                        appCodeName,
                        rightMask});
            return ((System.Data.DataSet)(results[0]));
        }

        /// <summary>
        /// 查询指定部门范围下，指定应用系统中，指定角色下的所有人员
        /// </summary>
        /// <param name="orgRoot">部门范围的全路径，空串时不做限制，多个时用逗号分隔</param>
        /// <param name="appCodeName">应用的英文标识</param>
        /// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
        /// <param name="delegationMask">权限委派类型</param>
        /// <param name="sidelineMask">人员职位类型</param>
        /// <param name="extAttr">要求获取的扩展属性</param>
        /// <returns>指定部门范围下，指定应用系统中，指定角色下的所有人员</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetRolesUsers", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetRolesUsers(string orgRoot, string appCodeName, string roleCodeNames, DelegationMaskType delegationMask, SidelineMaskType sidelineMask, string extAttr)
        {
            object[] results = this.Invoke("GetRolesUsers", new object[] {
                        orgRoot,
                        appCodeName,
                        roleCodeNames,
                        delegationMask,
                        sidelineMask,
                        extAttr});
            return ((System.Data.DataSet)(results[0]));
        }

        /// <summary>
        /// 查询指定部门下，指定应用系统中，指定角色的所有机构和人员
        /// </summary>
        /// <param name="orgRoot">部门范围的全路径，空串时不做限制，多个时用逗号分隔</param>
        /// <param name="appCodeName">应用的英文标识</param>
        /// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
        /// <param name="doesMixSort">是否采用混合排序，true:机构、人员混排，false:先机构，后人员</param>
        /// <param name="doesSortRank">是否排序级别，true:先级别，后层次 false:只排层次</param>
        /// <param name="includeDelegate">是否包括委派权限的被授权对象，true:包括 false:不包括</param>
        /// <returns>指定部门下，指定应用系统中，指定角色的所有机构和人员</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetDepartmentAndUserInRoles", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetDepartmentAndUserInRoles(string orgRoot, string appCodeName, string roleCodeNames, bool doesMixSort, bool doesSortRank, bool includeDelegate)
        {
            object[] results = this.Invoke("GetDepartmentAndUserInRoles", new object[] {
                        orgRoot,
                        appCodeName,
                        roleCodeNames,
                        doesMixSort,
                        doesSortRank,
                        includeDelegate});
            return ((System.Data.DataSet)(results[0]));
        }

        /// <summary>
        /// 查询指定部门下，指定应用系统中，指定角色的所有被授权对象
        /// </summary>
        /// <param name="orgRoot">部门范围的全路径，空串时不做限制，多个时用逗号分隔</param>
        /// <param name="appCodeName">应用的英文标识</param>
        /// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
        /// <param name="doesMixSort">是否采用混合排序，true:机构、组、人员混排，false:先机构，再组，后人员</param>
        /// <param name="doesSortRank">是否排序级别，true:先级别，后层次 false:只排层次</param>
        /// <param name="includeDelegate">是否包括委派权限的被授权对象，true:包括 false:不包括</param>
        /// <returns>指定部门下，指定应用系统中，指定角色的所有被授权对象</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetChildrenInRoles", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetChildrenInRoles(string orgRoot, string appCodeName, string roleCodeNames, bool doesMixSort, bool doesSortRank, bool includeDelegate)
        {
            object[] results = this.Invoke("GetChildrenInRoles", new object[] {
                        orgRoot,
                        appCodeName,
                        roleCodeNames,
                        doesMixSort,
                        doesSortRank,
                        includeDelegate});
            return ((System.Data.DataSet)(results[0]));
        }

        /// <summary>
        /// 查找指定应用系统中，具有指定功能的角色
        /// </summary>
        /// <param name="appCodeName">应用的英文标识</param>
        /// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
        /// <returns>指定应用系统中，具有指定功能的角色</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetFunctionsRoles", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetFunctionsRoles(string appCodeName, string funcCodeNames)
        {
            object[] results = this.Invoke("GetFunctionsRoles", new object[] {
                        appCodeName,
                        funcCodeNames});
            return ((System.Data.DataSet)(results[0]));
        }

        /// <summary>
        /// 查询指定部门范围下，指定应用系统中，拥有指定功能的所有人员
        /// </summary>
        /// <param name="orgRoot">部门范围的全路径，空串时不限制</param>
        /// <param name="appCodeName">应用的英文标识</param>
        /// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
        /// <param name="delegationMask">权限委派类型</param>
        /// <param name="sidelineMask">人员职位类型</param>
        /// <param name="extAttr">要求获取的扩展属性</param>
        /// <returns>指定部门范围下，指定应用系统中，拥有指定功能的所有人员</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetFunctionsUsers", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetFunctionsUsers(string orgRoot, string appCodeName, string funcCodeNames, DelegationMaskType delegationMask, SidelineMaskType sidelineMask, string extAttr)
        {
            object[] results = this.Invoke("GetFunctionsUsers", new object[] {
                        orgRoot,
                        appCodeName,
                        funcCodeNames,
                        delegationMask,
                        sidelineMask,
                        extAttr});
            return ((System.Data.DataSet)(results[0]));
        }

        /// <summary>
        /// 判断人员是否在指定应用系统的指定角色中
        /// </summary>
        /// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
        /// <param name="appCodeName">应用的英文标识</param>
        /// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
        /// <param name="userValueType">人员标识类型</param>
        /// <param name="delegationMask">权限委派类型</param>
        /// <returns>人员是否在指定应用系统的指定角色中</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/IsUserInRoles", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool IsUserInRoles(string userValue, string appCodeName, string roleCodeNames, UserValueType userValueType, DelegationMaskType delegationMask)
        {
            object[] results = this.Invoke("IsUserInRoles", new object[] {
                        userValue,
                        appCodeName,
                        roleCodeNames,
                        userValueType,
                        delegationMask});
            return ((bool)(results[0]));
        }

        /// <summary>
        /// 判断人员是否在指定应用系统，指定的所有角色中
        /// </summary>
        /// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
        /// <param name="appCodeName">应用的英文标识</param>
        /// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
        /// <param name="userValueType">人员标识类型</param>
        /// <param name="delegationMask">权限委派类型</param>
        /// <returns>人员是否在指定应用系统，指定的所有角色中</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/IsUserInAllRoles", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool IsUserInAllRoles(string userValue, string appCodeName, string roleCodeNames, UserValueType userValueType, DelegationMaskType delegationMask)
        {
            object[] results = this.Invoke("IsUserInAllRoles", new object[] {
                        userValue,
                        appCodeName,
                        roleCodeNames,
                        userValueType,
                        delegationMask});
            return ((bool)(results[0]));
        }

        /// <summary>
        /// 查询指定人员，在指定应用系统中，是否拥有指定的功能权限(有一个即可)
        /// </summary>
        /// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
        /// <param name="appCodeName">应用的英文标识</param>
        /// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
        /// <param name="userValueType">人员标识类型</param>
        /// <param name="delegationMask">权限委派类型</param>
        /// <returns>指定人员，在指定应用系统中，是否拥有指定的功能权限(有一个即可)</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/DoesUserHasPermissions", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool DoesUserHasPermissions(string userValue, string appCodeName, string funcCodeNames, UserValueType userValueType, DelegationMaskType delegationMask)
        {
            object[] results = this.Invoke("DoesUserHasPermissions", new object[] {
                        userValue,
                        appCodeName,
                        funcCodeNames,
                        userValueType,
                        delegationMask});
            return ((bool)(results[0]));
        }

        /// <summary>
        /// 查询指定人员，在指定应用系统中，是否拥有指定的功能权限(拥有全部功能)
        /// </summary>
        /// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
        /// <param name="appCodeName">应用的英文标识</param>
        /// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
        /// <param name="userValueType">人员标识类型</param>
        /// <param name="delegationMask">权限委派类型</param>
        /// <returns>指定人员，在指定应用系统中，是否拥有指定的功能权限(拥有全部功能)</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/DoesUserHasAllPermissions", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool DoesUserHasAllPermissions(string userValue, string appCodeName, string funcCodeNames, UserValueType userValueType, DelegationMaskType delegationMask)
        {
            object[] results = this.Invoke("DoesUserHasAllPermissions", new object[] {
                        userValue,
                        appCodeName,
                        funcCodeNames,
                        userValueType,
                        delegationMask});
            return ((bool)(results[0]));
        }

        ///// <remarks/>
        //[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SignInCheck", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        //public bool SignInCheck(string strLogonName, string strUserPwd)
        //{
        //    object[] results = this.Invoke("SignInCheck", new object[] {
        //                strLogonName,
        //                strUserPwd});
        //    return ((bool)(results[0]));
        //}

        /// <summary>
        /// 查询指定用户，在指定应用系统中所拥有的角色
        /// </summary>
        /// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
        /// <param name="appCodeName">应用的英文标识</param>
        /// <param name="userValueType">人员标识类型</param>
        /// <param name="rightMask">权限授权类型</param>
        /// <param name="delegationMask">权限委派类型</param>
        /// <returns>指定用户，在指定应用系统中所拥有的角色</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetUserRoles", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetUserRoles(string userValue, string appCodeName, UserValueType userValueType, RightMaskType rightMask, DelegationMaskType delegationMask)
        {
            object[] results = this.Invoke("GetUserRoles", new object[] {
                        userValue,
                        appCodeName,
                        userValueType,
                        rightMask,
                        delegationMask});
            return ((System.Data.DataSet)(results[0]));
        }

        /// <summary>
        /// 查询指定人员，在指定应用系统具有的权限（功能）
        /// </summary>
        /// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
        /// <param name="appCodeName">应用的英文标识</param>
        /// <param name="userValueType">人员标识类型</param>
        /// <param name="rightMask">权限授权类型</param>
        /// <param name="delegationMask">权限委派类型</param>
        /// <returns>指定人员，在指定应用系统具有的权限（功能）</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetUserPermissions", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetUserPermissions(string userValue, string appCodeName, UserValueType userValueType, RightMaskType rightMask, DelegationMaskType delegationMask)
        {
            object[] results = this.Invoke("GetUserPermissions", new object[] {
                        userValue,
                        appCodeName,
                        userValueType,
                        rightMask,
                        delegationMask});
            return ((System.Data.DataSet)(results[0]));
        }

        /// <summary>
        /// 查询指定人员所涉及的所有应用系统的角色信息
        /// </summary>
        /// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
        /// <param name="userValueType">人员标识类型</param>
        /// <param name="rightMask">权限授权类型</param>
        /// <param name="delegationMask">权限委派类型</param>
        /// <returns>指定人员所涉及的所有应用系统的角色信息</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetUserApplicationsRoles", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetUserApplicationsRoles(string userValue, UserValueType userValueType, RightMaskType rightMask, DelegationMaskType delegationMask)
        {
            object[] results = this.Invoke("GetUserApplicationsRoles", new object[] {
                        userValue,
                        userValueType,
                        rightMask,
                        delegationMask});
            return ((System.Data.DataSet)(results[0]));
        }

        /// <summary>
        /// 查询指定人员所涉及的所有应用系统的基本信息
        /// </summary>
        /// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
        /// <param name="userValueType">人员标识类型</param>
        /// <param name="rightMask">权限授权类型</param>
        /// <param name="delegationMask">权限委派类型</param>
        /// <returns>指定人员所涉及的所有应用系统的基本信息</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetUserApplications", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetUserApplications(string userValue, UserValueType userValueType, RightMaskType rightMask, DelegationMaskType delegationMask)
        {
            object[] results = this.Invoke("GetUserApplications", new object[] {
                        userValue,
                        userValueType,
                        rightMask,
                        delegationMask});
            return ((System.Data.DataSet)(results[0]));
        }

        /// <summary>
        /// 查询指定人员，在指定应用系统，指定功能下所拥有的服务范围
        /// </summary>
        /// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
        /// <param name="appCodeName">应用的英文标识</param>
        /// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
        /// <param name="userValueType">人员身份标识类型</param>
        /// <param name="delegationMask">权限委派类型</param>
        /// <param name="scopeMask">服务范围类型</param>
        /// <returns>指定人员，在指定应用系统，指定功能下所拥有的服务范围</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetUserRolesScopes", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetUserRolesScopes(string userValue, string appCodeName, string roleCodeNames, UserValueType userValueType, DelegationMaskType delegationMask, ScopeMaskType scopeMask)
        {
            object[] results = this.Invoke("GetUserRolesScopes", new object[] {
                        userValue,
                        appCodeName,
                        roleCodeNames,
                        userValueType,
                        delegationMask,
                        scopeMask});
            return ((System.Data.DataSet)(results[0]));
        }

        /// <summary>
        /// 查询指定人员，在指定应用系统，指定角色下所拥有的服务范围
        /// </summary>
        /// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
        /// <param name="appCodeName">应用的英文标识</param>
        /// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
        /// <param name="userValueType">人员身份标识类型</param>
        /// <param name="delegationMask">权限委派类型</param>
        /// <param name="scopeMask">服务范围类型</param>
        /// <returns>指定人员，在指定应用系统，指定角色下所拥有的服务范围</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetUserFunctionsScopes", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetUserFunctionsScopes(string userValue, string appCodeName, string funcCodeNames, UserValueType userValueType, DelegationMaskType delegationMask, ScopeMaskType scopeMask)
        {
            object[] results = this.Invoke("GetUserFunctionsScopes", new object[] {
                        userValue,
                        appCodeName,
                        funcCodeNames,
                        userValueType,
                        delegationMask,
                        scopeMask});
            return ((System.Data.DataSet)(results[0]));
        }

        /// <summary>
        /// 查询指定人员在指定应用系统中，指定角色的原有委派者
        /// </summary>
        /// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
        /// <param name="appCodeName">应用的英文标识</param>
        /// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
        /// <param name="userValueType">人员标识类型</param>
        /// <param name="includeDisabled">是否包括无效的委派</param>
        /// <returns></returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetOriginalUser", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetOriginalUser(string userValue, string appCodeName, string roleCodeNames, UserValueType userValueType, bool includeDisabled)
        {
            object[] results = this.Invoke("GetOriginalUser", new object[] {
                        userValue,
                        appCodeName,
                        roleCodeNames,
                        userValueType,
                        includeDisabled});
            return ((System.Data.DataSet)(results[0]));
        }

        /// <summary>
        /// 查询指定人员在所有涉及的应用系统中，所有被委派角色的原有委派者
        /// </summary>
        /// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
        /// <param name="userValueType">人员标识类型</param>
        /// <param name="includeDisabled">是否包括无效的委派</param>
        /// <returns>指定人员在所有涉及的应用系统中，所有被委派角色的原有委派者</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetAllOriginalUser", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetAllOriginalUser(string userValue, UserValueType userValueType, bool includeDisabled)
        {
            object[] results = this.Invoke("GetAllOriginalUser", new object[] {
                        userValue,
                        userValueType,
                        includeDisabled});
            return ((System.Data.DataSet)(results[0]));
        }

        /// <summary>
        /// 查询指定人员，在指定应用系统中，指定角色的被委派者
        /// </summary>
        /// <param name="userValues">用户身份标识（由userValueType参数指定类型）</param>
        /// <param name="appCodeName">应用的英文标识</param>
        /// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
        /// <param name="userValueType">人员标识类型</param>
        /// <param name="includeDisabled">是否包括无效的委派</param>
        /// <returns>指定人员，在指定应用系统中，指定角色的被委派者</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetDelegatedUser", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetDelegatedUser(string userValues, string appCodeName, string roleCodeNames, UserValueType userValueType, bool includeDisabled)
        {
            object[] results = this.Invoke("GetDelegatedUser", new object[] {
                        userValues,
                        appCodeName,
                        roleCodeNames,
                        userValueType,
                        includeDisabled});
            return ((System.Data.DataSet)(results[0]));
        }

        /// <summary>
        /// 查询指定人员，在所有涉及的应用系统中，所有角色的被委派者
        /// </summary>
        /// <param name="userValues">用户身份标识（由userValueType参数指定类型）</param>
        /// <param name="userValueType">人员标识类型</param>
        /// <param name="includeDisabled">是否包括无效的委派</param>
        /// <returns></returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetAllDelegatedUser", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetAllDelegatedUser(string userValues, UserValueType userValueType, bool includeDisabled)
        {
            object[] results = this.Invoke("GetAllDelegatedUser", new object[] {
                        userValues,
                        userValueType,
                        includeDisabled});
            return ((System.Data.DataSet)(results[0]));
        }

        /// <summary>
        /// 查询指定用户，在指定应用中所拥有的，可进行委派的角色
        /// </summary>
        /// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
        /// <param name="appCodeName">应用的英文标识</param>
        /// <param name="userValueType">人员标识类型</param>
        /// <param name="rightMask">权限授权类型</param>
        /// <returns>指定用户，在指定应用中所拥有的，可进行委派的角色</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetUserAllowDelegteRoles", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetUserAllowDelegteRoles(string userValue, string appCodeName, UserValueType userValueType, RightMaskType rightMask)
        {
            object[] results = this.Invoke("GetUserAllowDelegteRoles", new object[] {
                        userValue,
                        appCodeName,
                        userValueType,
                        rightMask});
            return ((System.Data.DataSet)(results[0]));
        }

        /// <summary>
        /// 去除所有的缓存。
        /// </summary>
		[ServiceBrokerExtension]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/RemoveAllCache", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void RemoveAllCache()
        {
            this.Invoke("RemoveAllCache", new object[0]);
        }
    }
}
