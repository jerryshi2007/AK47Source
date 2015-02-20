using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using MCS.Library.SOA.DataObjects.Schemas.Client;
using MCS.Library.SOA.DataObjects.Schemas.Client.ServiceBroker;

namespace MCS.Library.SOA.DataObjects.Security.Client
{
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Web.Services.WebServiceBindingAttribute(Name = "PermissionCenterQueryServiceSoap", Namespace = "http://tempuri.org/")]
	[XmlInclude(typeof(ClientSchemaObjectBase))]
	[XmlInclude(typeof(ClientSCBase))]
	[XmlInclude(typeof(ClientSCUser))]
	[XmlInclude(typeof(ClientSCOrganization))]
	[XmlInclude(typeof(ClientSCGroup))]
	[XmlInclude(typeof(ClientSCApplication))]
	[XmlInclude(typeof(ClientSCRole))]
	[XmlInclude(typeof(ClientSCPermission))]
	[XmlInclude(typeof(ClientObjectBase))]
	[XmlInclude(typeof(ClientAclItem))]
	[XmlInclude(typeof(ClientConditionItem))]
	[XmlInclude(typeof(ClientPropertyDefine))]
	[XmlInclude(typeof(ClientRoleDisplayItem))]
	public partial class PermissionCenterQueryService : System.Web.Services.Protocols.SoapHttpClientProtocol
	{
		public static readonly PermissionCenterQueryService Instance = new PermissionCenterQueryService();

		private bool useDefaultCredentialsSetExplicitly;

		/// <remarks/>
		private PermissionCenterQueryService()
		{
			PCServiceBrokerContext.Current.InitWebClientProtocol(this);

			this.Url = PCServiceClientSettings.GetConfig().QueryServiceAddress.ToString();

			if ((this.IsLocalFileSystemWebService(this.Url) == true))
			{
				this.UseDefaultCredentials = true;
				this.useDefaultCredentialsSetExplicitly = false;
			}
			else
			{
				this.useDefaultCredentialsSetExplicitly = true;
			}
		}

		public new string Url
		{
			get
			{
				return base.Url;
			}
			set
			{
				if ((((this.IsLocalFileSystemWebService(base.Url) == true)
							&& (this.useDefaultCredentialsSetExplicitly == false))
							&& (this.IsLocalFileSystemWebService(value) == false)))
				{
					base.UseDefaultCredentials = false;
				}
				base.Url = value;
			}
		}

		public new bool UseDefaultCredentials
		{
			get
			{
				return base.UseDefaultCredentials;
			}
			set
			{
				base.UseDefaultCredentials = value;
				this.useDefaultCredentialsSetExplicitly = true;
			}
		}

		#region WebMethods

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[PCServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/GetRoot", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSCOrganization GetRoot()
		{
			object[] results = this.Invoke("GetRoot", new object[0]);

			return ((ClientSCOrganization)(results[0]));
		}

		/// <summary>
		/// 根据ID获取对象
		/// </summary>
		/// <param name="ids">对象的ID的集合</param>
		/// <param name="objectSchemaTypes">要获取的对象的schema类型</param>
		/// <returns></returns>
		[PCServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/GetObjectsByIDs", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectCollection GetObjectsByIDs(string[] ids, string[] objectSchemaTypes, bool normalOnly)
		{
			object[] results = this.Invoke("GetObjectsByIDs", new object[] {
                        ids, objectSchemaTypes,normalOnly });

			return new ClientSchemaObjectCollection(((IEnumerable<ClientSchemaObjectBase>)(results[0])));
		}

		/// <summary>
		/// 根据代码名称获取对象
		/// </summary>
		/// <param name="codeNames">对象的代码名称</param>
		/// <param name="objectSchemaTypes">对象的Schema类型</param>
		/// <returns></returns>
		[PCServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/GetObjectsByCodeNames", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectCollection GetObjectsByCodeNames(string[] codeNames, string[] objectSchemaTypes, bool normalOnly)
		{
			object[] results = this.Invoke("GetObjectsByCodeNames", new object[] {
                        codeNames, objectSchemaTypes,normalOnly});

			return new ClientSchemaObjectCollection(((IEnumerable<ClientSchemaObjectBase>)(results[0])));
		}

		[PCServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/GetMemberships", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaMember[] GetMemberships(string containerID, string[] memberSchemaTypes, bool normalOnly)
		{
			object[] results = this.Invoke("GetMemberships", new object[] {
                        containerID, memberSchemaTypes,normalOnly});

			return (ClientSchemaMember[])results[0];
		}

		[PCServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/GetObjectsByXQuery", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectCollection GetObjectsByXQuery(string xQuery, string[] objectSchemaTypes, bool normalOnly)
		{
			object[] results = this.Invoke("GetObjectsByXQuery", new object[] {
                        xQuery, objectSchemaTypes,normalOnly});

			return new ClientSchemaObjectCollection(((IEnumerable<ClientSchemaObjectBase>)(results[0])));
		}

		/// <summary>
		/// 获取对象的上级对象
		/// </summary>
		/// <param name="id">下级对象的ID</param>
		/// <param name="parentSchemaTypes">上级对象的Schema类型</param>
		/// <returns></returns>
		[PCServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/GetParents", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectCollection GetParents(string id, string[] parentSchemaTypes, bool normalOnly)
		{
			object[] results = this.Invoke("GetParents", new object[] {
                        id, parentSchemaTypes ,normalOnly });

			return new ClientSchemaObjectCollection(((IEnumerable<ClientSchemaObjectBase>)(results[0])));
		}


		/// <summary>
		/// 获取对象的下级对象
		/// </summary>
		/// <param name="id">上级对象ID</param>
		/// <param name="childSchemaTypes">下级对象的Schema类型</param>
		/// <returns></returns>
		[PCServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/GetChildren", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectCollection GetChildren(string id, string[] childSchemaTypes, bool normalOnly)
		{
			object[] results = this.Invoke("GetChildren", new object[] {
                        id, childSchemaTypes ,normalOnly });

			return new ClientSchemaObjectCollection(((IEnumerable<ClientSchemaObjectBase>)(results[0])));
		}

		/// <summary>
		/// 获取对象的成员对象的集合
		/// </summary>
		/// <param name="id">对象ID</param>
		/// <param name="memberSchemaTypes">成员对象的Schema类型</param>
		/// <returns></returns>
		[PCServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/GetMembers", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectCollection GetMembers(string id, string[] memberSchemaTypes, bool normalOnly)
		{
			object[] results = this.Invoke("GetMembers", new object[] {
                        id, memberSchemaTypes, normalOnly });

			return new ClientSchemaObjectCollection(((IEnumerable<ClientSchemaObjectBase>)(results[0])));
		}

		/// <summary>
		/// 获取对象的容器对象
		/// </summary>
		/// <param name="id">对象id</param>
		/// <param name="containerSchemaTypes">容器对象的SchemaType的集合</param>
		/// <returns></returns>
		[PCServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/GetContainers", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectCollection GetContainers(string id, string[] containerSchemaTypes, bool normalOnly)
		{
			object[] results = this.Invoke("GetContainers", new object[] {
                        id, containerSchemaTypes, normalOnly });

			return new ClientSchemaObjectCollection(((IEnumerable<ClientSchemaObjectBase>)(results[0])));
		}

		/// <summary>
		/// 获取对象的正常容器对象
		/// </summary>
		/// <param name="id">对象id</param>
		/// <param name="containerSchemaTypes">容器对象的SchemaType的集合</param>
		/// <returns></returns>
		[PCServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/GetCurrentContainers", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectCollection GetCurrentContainers(string id, string[] containerSchemaTypes, bool normalOnly)
		{
			object[] results = this.Invoke("GetCurrentContainers", new object[] {
                        id, containerSchemaTypes, normalOnly });

			return new ClientSchemaObjectCollection(((IEnumerable<ClientSchemaObjectBase>)(results[0])));
		}

		/// <summary>
		/// 获取对象上定义的访问控制列表
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[PCServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/GetAcls", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientAclItemCollection GetAcls(string id)
		{
			object[] results = this.Invoke("GetAcls", new object[] {
                        id});

			return (ClientAclItemCollection)results[0];
		}

		/// <summary>
		/// 获取对象的条件集合
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[PCServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/GetConditions", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientConditionItem[] GetConditions(string id)
		{
			object[] results = this.Invoke("GetConditions", new object[] {
                        id});

			return (ClientConditionItem[])results[0];
		}

		/// <summary>
		/// 获取Schema属性定义
		/// </summary>
		/// <param name="schemaType">要获取属性定义的Schema</param>
		/// <returns></returns>
		[PCServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/GetSchemaPropertyDefinition", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientPropertyDefine[] GetSchemaPropertyDefinition(string schemaType)
		{
			object[] results = this.Invoke("GetSchemaPropertyDefinition", new object[] {
                        schemaType});

			return (ClientPropertyDefine[])results[0];
		}

		/// <summary>
		/// 获取角色的显示信息
		/// </summary>
		/// <param name="roleIds">要显示的角色的ID</param>
		/// <returns></returns>
		[PCServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/GetRoleDisplayItems", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientRoleDisplayItem[] GetRoleDisplayItems(string[] roleIds)
		{
			object[] results = this.Invoke("GetRoleDisplayItems", new object[] {
                        roleIds});

			return (ClientRoleDisplayItem[])results[0];
		}

		#endregion

		private bool IsLocalFileSystemWebService(string url)
		{
			if (((url == null)
						|| (url == string.Empty)))
			{
				return false;
			}
			System.Uri wsUri = new System.Uri(url);
			if (((wsUri.Port >= 1024)
						&& (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0)))
			{
				return true;
			}

			return false;
		}
	}
}
