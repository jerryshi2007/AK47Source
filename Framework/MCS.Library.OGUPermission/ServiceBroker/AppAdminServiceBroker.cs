#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.OGUPermission
// FileName	��	AppAdminServiceBroker.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���	    20070430		����
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
    /// ��Ȩ���͡�
    /// </summary>
    [System.FlagsAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.312")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public enum RightMaskType
    {

        /// <summary>
        /// ����Ȩ��
        /// </summary>
        Self = 1,

        /// <summary>
        /// Ӧ����Ȩ��
        /// </summary>
        App = 2,

        /// <summary>
        /// ȫ�������� ����Ȩ �� Ӧ����Ȩ��
        /// </summary>
        All = 4,
    }

    /// <summary>
    /// Ȩ��ί�����͡�
    /// </summary>
    [System.FlagsAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.312")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public enum DelegationMaskType
    {

        /// <summary>
        /// ԭʼȨ�ޡ�
        /// </summary>
        Original = 1,

        /// <summary>
        /// ��ί��Ȩ�ޡ�
        /// </summary>
        Delegated = 2,

        /// <summary>
        /// ȫ�������� ԭʼȨ�� �� ��ί��Ȩ�ޡ�
        /// </summary>
        All = 4,
    }

    /// <summary>
    /// ��ְ���͡�
    /// </summary>
    [System.FlagsAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.312")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public enum SidelineMaskType
    {

        /// <summary>
        /// ����ְ��
        /// </summary>
        NotSideline = 1,

        /// <summary>
        /// ��ְ��
        /// </summary>
        Sideline = 2,

        /// <summary>
        /// ȫ����
        /// </summary>
        All = 4,
    }

    /// <summary>
    /// ��ݱ�־���͡�
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.312")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public enum UserValueType
    {
        /// <summary>
        /// ��¼��
        /// </summary>
        LogonName = 1,
        /// <summary>
        /// ��Աȫ·��
        /// </summary>
        AllPath = 2,
        /// <summary>
        /// ��Ա���
        /// </summary>
        PersonID = 3,
        /// <summary>
        /// IC����
        /// </summary>
        ICCode = 4,
        /// <summary>
        /// ��ԱGuidֵ
        /// </summary>
        Guid = 8,
        /// <summary>
        /// ����Ψһ������ѯ(Ϊ����Ͼ�����ͳһƽ̨�л����������ֶ�ID[����Ψһ�ֶ�])
        /// </summary>
        Identity = 16
    }

    /// <summary>
    /// Ȩ�޷���Χ���͡�
    /// </summary>
    [System.FlagsAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.312")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public enum ScopeMaskType
    {

        /// <summary>
        /// ������Χ��
        /// </summary>
        OrgScope = 1,

        /// <summary>
        /// ���ݷ�Χ
        /// </summary>
        DataScope = 2,

        /// <summary>
        /// ȫ��
        /// </summary>
        All = 4,
    }

    /// <summary>
    /// ��Ȩ����WebService�Ĵ���
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.312")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "AppSecurityCheckServiceSoap", Namespace = "http://tempuri.org/")]
    public sealed class AppAdminServiceBroker : System.Web.Services.Protocols.SoapHttpClientProtocol
    {
        /// <summary>
        /// ��Ȩ����WebService�Ĵ���ʵ��
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
        /// �������Ӧ��ϵͳ�Ļ�����Ϣ��
        /// </summary>
        /// <returns>����Ӧ��ϵͳ�Ļ�����Ϣ��</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetApplications", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetApplications()
        {
            object[] results = this.Invoke("GetApplications", new object[0]);
            return ((System.Data.DataSet)(results[0]));
        }

        /// <summary>
        /// ��ѯָ��Ӧ��ϵͳ�У�ָ���������н�ɫ
        /// </summary>
        /// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
        /// <param name="rightMask">Ȩ����Ȩ����</param>
        /// <returns>ָ��Ӧ��ϵͳ�У�ָ���������н�ɫ</returns>
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
        /// ��ѯָ��Ӧ��ϵͳ�У�ָ���������й���
        /// </summary>
        /// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
        /// <param name="rightMask">Ȩ����Ȩ����</param>
        /// <returns>ָ��Ӧ��ϵͳ�У�ָ���������й���</returns>
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
        /// ��ѯָ�����ŷ�Χ�£�ָ��Ӧ��ϵͳ�У�ָ����ɫ�µ�������Ա
        /// </summary>
        /// <param name="orgRoot">���ŷ�Χ��ȫ·�����մ�ʱ�������ƣ����ʱ�ö��ŷָ�</param>
        /// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
        /// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
        /// <param name="delegationMask">Ȩ��ί������</param>
        /// <param name="sidelineMask">��Աְλ����</param>
        /// <param name="extAttr">Ҫ���ȡ����չ����</param>
        /// <returns>ָ�����ŷ�Χ�£�ָ��Ӧ��ϵͳ�У�ָ����ɫ�µ�������Ա</returns>
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
        /// ��ѯָ�������£�ָ��Ӧ��ϵͳ�У�ָ����ɫ�����л�������Ա
        /// </summary>
        /// <param name="orgRoot">���ŷ�Χ��ȫ·�����մ�ʱ�������ƣ����ʱ�ö��ŷָ�</param>
        /// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
        /// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
        /// <param name="doesMixSort">�Ƿ���û������true:��������Ա���ţ�false:�Ȼ���������Ա</param>
        /// <param name="doesSortRank">�Ƿ����򼶱�true:�ȼ��𣬺��� false:ֻ�Ų��</param>
        /// <param name="includeDelegate">�Ƿ����ί��Ȩ�޵ı���Ȩ����true:���� false:������</param>
        /// <returns>ָ�������£�ָ��Ӧ��ϵͳ�У�ָ����ɫ�����л�������Ա</returns>
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
        /// ��ѯָ�������£�ָ��Ӧ��ϵͳ�У�ָ����ɫ�����б���Ȩ����
        /// </summary>
        /// <param name="orgRoot">���ŷ�Χ��ȫ·�����մ�ʱ�������ƣ����ʱ�ö��ŷָ�</param>
        /// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
        /// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
        /// <param name="doesMixSort">�Ƿ���û������true:�������顢��Ա���ţ�false:�Ȼ��������飬����Ա</param>
        /// <param name="doesSortRank">�Ƿ����򼶱�true:�ȼ��𣬺��� false:ֻ�Ų��</param>
        /// <param name="includeDelegate">�Ƿ����ί��Ȩ�޵ı���Ȩ����true:���� false:������</param>
        /// <returns>ָ�������£�ָ��Ӧ��ϵͳ�У�ָ����ɫ�����б���Ȩ����</returns>
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
        /// ����ָ��Ӧ��ϵͳ�У�����ָ�����ܵĽ�ɫ
        /// </summary>
        /// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
        /// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
        /// <returns>ָ��Ӧ��ϵͳ�У�����ָ�����ܵĽ�ɫ</returns>
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
        /// ��ѯָ�����ŷ�Χ�£�ָ��Ӧ��ϵͳ�У�ӵ��ָ�����ܵ�������Ա
        /// </summary>
        /// <param name="orgRoot">���ŷ�Χ��ȫ·�����մ�ʱ������</param>
        /// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
        /// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
        /// <param name="delegationMask">Ȩ��ί������</param>
        /// <param name="sidelineMask">��Աְλ����</param>
        /// <param name="extAttr">Ҫ���ȡ����չ����</param>
        /// <returns>ָ�����ŷ�Χ�£�ָ��Ӧ��ϵͳ�У�ӵ��ָ�����ܵ�������Ա</returns>
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
        /// �ж���Ա�Ƿ���ָ��Ӧ��ϵͳ��ָ����ɫ��
        /// </summary>
        /// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
        /// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
        /// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
        /// <param name="userValueType">��Ա��ʶ����</param>
        /// <param name="delegationMask">Ȩ��ί������</param>
        /// <returns>��Ա�Ƿ���ָ��Ӧ��ϵͳ��ָ����ɫ��</returns>
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
        /// �ж���Ա�Ƿ���ָ��Ӧ��ϵͳ��ָ�������н�ɫ��
        /// </summary>
        /// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
        /// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
        /// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
        /// <param name="userValueType">��Ա��ʶ����</param>
        /// <param name="delegationMask">Ȩ��ί������</param>
        /// <returns>��Ա�Ƿ���ָ��Ӧ��ϵͳ��ָ�������н�ɫ��</returns>
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
        /// ��ѯָ����Ա����ָ��Ӧ��ϵͳ�У��Ƿ�ӵ��ָ���Ĺ���Ȩ��(��һ������)
        /// </summary>
        /// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
        /// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
        /// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
        /// <param name="userValueType">��Ա��ʶ����</param>
        /// <param name="delegationMask">Ȩ��ί������</param>
        /// <returns>ָ����Ա����ָ��Ӧ��ϵͳ�У��Ƿ�ӵ��ָ���Ĺ���Ȩ��(��һ������)</returns>
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
        /// ��ѯָ����Ա����ָ��Ӧ��ϵͳ�У��Ƿ�ӵ��ָ���Ĺ���Ȩ��(ӵ��ȫ������)
        /// </summary>
        /// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
        /// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
        /// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
        /// <param name="userValueType">��Ա��ʶ����</param>
        /// <param name="delegationMask">Ȩ��ί������</param>
        /// <returns>ָ����Ա����ָ��Ӧ��ϵͳ�У��Ƿ�ӵ��ָ���Ĺ���Ȩ��(ӵ��ȫ������)</returns>
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
        /// ��ѯָ���û�����ָ��Ӧ��ϵͳ����ӵ�еĽ�ɫ
        /// </summary>
        /// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
        /// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
        /// <param name="userValueType">��Ա��ʶ����</param>
        /// <param name="rightMask">Ȩ����Ȩ����</param>
        /// <param name="delegationMask">Ȩ��ί������</param>
        /// <returns>ָ���û�����ָ��Ӧ��ϵͳ����ӵ�еĽ�ɫ</returns>
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
        /// ��ѯָ����Ա����ָ��Ӧ��ϵͳ���е�Ȩ�ޣ����ܣ�
        /// </summary>
        /// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
        /// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
        /// <param name="userValueType">��Ա��ʶ����</param>
        /// <param name="rightMask">Ȩ����Ȩ����</param>
        /// <param name="delegationMask">Ȩ��ί������</param>
        /// <returns>ָ����Ա����ָ��Ӧ��ϵͳ���е�Ȩ�ޣ����ܣ�</returns>
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
        /// ��ѯָ����Ա���漰������Ӧ��ϵͳ�Ľ�ɫ��Ϣ
        /// </summary>
        /// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
        /// <param name="userValueType">��Ա��ʶ����</param>
        /// <param name="rightMask">Ȩ����Ȩ����</param>
        /// <param name="delegationMask">Ȩ��ί������</param>
        /// <returns>ָ����Ա���漰������Ӧ��ϵͳ�Ľ�ɫ��Ϣ</returns>
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
        /// ��ѯָ����Ա���漰������Ӧ��ϵͳ�Ļ�����Ϣ
        /// </summary>
        /// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
        /// <param name="userValueType">��Ա��ʶ����</param>
        /// <param name="rightMask">Ȩ����Ȩ����</param>
        /// <param name="delegationMask">Ȩ��ί������</param>
        /// <returns>ָ����Ա���漰������Ӧ��ϵͳ�Ļ�����Ϣ</returns>
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
        /// ��ѯָ����Ա����ָ��Ӧ��ϵͳ��ָ����������ӵ�еķ���Χ
        /// </summary>
        /// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
        /// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
        /// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
        /// <param name="userValueType">��Ա��ݱ�ʶ����</param>
        /// <param name="delegationMask">Ȩ��ί������</param>
        /// <param name="scopeMask">����Χ����</param>
        /// <returns>ָ����Ա����ָ��Ӧ��ϵͳ��ָ����������ӵ�еķ���Χ</returns>
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
        /// ��ѯָ����Ա����ָ��Ӧ��ϵͳ��ָ����ɫ����ӵ�еķ���Χ
        /// </summary>
        /// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
        /// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
        /// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
        /// <param name="userValueType">��Ա��ݱ�ʶ����</param>
        /// <param name="delegationMask">Ȩ��ί������</param>
        /// <param name="scopeMask">����Χ����</param>
        /// <returns>ָ����Ա����ָ��Ӧ��ϵͳ��ָ����ɫ����ӵ�еķ���Χ</returns>
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
        /// ��ѯָ����Ա��ָ��Ӧ��ϵͳ�У�ָ����ɫ��ԭ��ί����
        /// </summary>
        /// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
        /// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
        /// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
        /// <param name="userValueType">��Ա��ʶ����</param>
        /// <param name="includeDisabled">�Ƿ������Ч��ί��</param>
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
        /// ��ѯָ����Ա�������漰��Ӧ��ϵͳ�У����б�ί�ɽ�ɫ��ԭ��ί����
        /// </summary>
        /// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
        /// <param name="userValueType">��Ա��ʶ����</param>
        /// <param name="includeDisabled">�Ƿ������Ч��ί��</param>
        /// <returns>ָ����Ա�������漰��Ӧ��ϵͳ�У����б�ί�ɽ�ɫ��ԭ��ί����</returns>
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
        /// ��ѯָ����Ա����ָ��Ӧ��ϵͳ�У�ָ����ɫ�ı�ί����
        /// </summary>
        /// <param name="userValues">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
        /// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
        /// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
        /// <param name="userValueType">��Ա��ʶ����</param>
        /// <param name="includeDisabled">�Ƿ������Ч��ί��</param>
        /// <returns>ָ����Ա����ָ��Ӧ��ϵͳ�У�ָ����ɫ�ı�ί����</returns>
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
        /// ��ѯָ����Ա���������漰��Ӧ��ϵͳ�У����н�ɫ�ı�ί����
        /// </summary>
        /// <param name="userValues">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
        /// <param name="userValueType">��Ա��ʶ����</param>
        /// <param name="includeDisabled">�Ƿ������Ч��ί��</param>
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
        /// ��ѯָ���û�����ָ��Ӧ������ӵ�еģ��ɽ���ί�ɵĽ�ɫ
        /// </summary>
        /// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
        /// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
        /// <param name="userValueType">��Ա��ʶ����</param>
        /// <param name="rightMask">Ȩ����Ȩ����</param>
        /// <returns>ָ���û�����ָ��Ӧ������ӵ�еģ��ɽ���ί�ɵĽ�ɫ</returns>
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
        /// ȥ�����еĻ��档
        /// </summary>
		[ServiceBrokerExtension]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/RemoveAllCache", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void RemoveAllCache()
        {
            this.Invoke("RemoveAllCache", new object[0]);
        }
    }
}
