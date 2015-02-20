using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Schemas.Client;
using System.Xml.Serialization;
using MCS.Library.SOA.DataObjects.Security.AUClient.Configuration;
using System.Web.Services.Protocols;
using System.Web.Services.Description;

namespace MCS.Library.SOA.DataObjects.Security.AUClient.ServiceBroker
{
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
	[System.Diagnostics.DebuggerStepThrough()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Web.Services.WebServiceBindingAttribute(Name = "PermissionCenterQueryServiceSoap", Namespace = "http://tempuri.org/")]
	[XmlInclude(typeof(ClientSchemaObjectBase))]
	[XmlInclude(typeof(ClientGenericObject))]
	[XmlInclude(typeof(ClientAdminUnit))]
	[XmlInclude(typeof(ClientAUAdminScope))]
	[XmlInclude(typeof(ClientAUAdminScopeItem))]
	[XmlInclude(typeof(ClientAURole))]
	[XmlInclude(typeof(ClientAURoleDisplayItem))]
	[XmlInclude(typeof(ClientAUSchema))]
	[XmlInclude(typeof(ClientAUSchemaRole))]
	[XmlInclude(typeof(ClientNamedObject))]
	[XmlInclude(typeof(ClientConditionItem))]
	[XmlInclude(typeof(ClientAclItem))]
	public class AUCenterQueryService : System.Web.Services.Protocols.SoapHttpClientProtocol, IAUCenterQueryService
	{
		public static readonly AUCenterQueryService Instance = new AUCenterQueryService();

		#region Internal

		private bool useDefaultCredentialsSetExplicitly;

		public AUCenterQueryService()
		{
			AUServiceBrokerContext.Current.InitWebClientProtocol(this);

			this.Url = AUServiceClientSettings.GetConfig().QueryServiceAddress.ToString();

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
		#endregion

		/// <summary>
		/// 获取对象的ACL
		/// </summary>
		/// <param name="id">对象的ID</param>
		/// <returns></returns>
		[AUServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/GetAcls", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientAclItem[] GetAcls(string id)
		{
			object[] results = this.Invoke("GetAcls", new object[] { id });

			return ((ClientAclItem[])(results[0]));
		}

		/// <summary>
		/// 获取对象的子对象
		/// </summary>
		/// <param name="id">对象ID</param>
		/// <param name="childSchemaTypes">子对象Schema类型</param>
		/// <param name="normalOnly">当为true时，仅包含正常对象</param>
		/// <returns></returns>
		[AUServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/GetChildren", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase[] GetChildren(string id, string[] childSchemaTypes, bool normalOnly)
		{
			object[] results = this.Invoke("GetChildren", new object[] { id, childSchemaTypes, normalOnly });

			return ((ClientSchemaObjectBase[])(results[0]));
		}

		/// <summary>
		/// 获取对象上定义的条件集合
		/// </summary>
		/// <param name="id">对象的ID</param>
		/// <returns></returns>
		[AUServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/GetConditions", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientConditionItem[] GetConditions(string id)
		{
			object[] results = this.Invoke("GetConditions", new object[] { id });

			return ((ClientConditionItem[])(results[0]));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="containerSchemaTypes"></param>
		/// <param name="normalOnly"></param>
		/// <returns></returns>
		[AUServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/GetContainers", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase[] GetContainers(string id, string[] containerSchemaTypes, bool normalOnly)
		{
			object[] results = this.Invoke("GetContainers", new object[] { id, containerSchemaTypes, normalOnly });

			return ((ClientSchemaObjectBase[])(results[0]));
		}

		[AUServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/GetMembers", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase[] GetMembers(string id, string[] memberSchemaTypes, bool normalOnly)
		{
			object[] results = this.Invoke("GetMembers", new object[] { id, memberSchemaTypes, normalOnly });

			return ((ClientSchemaObjectBase[])(results[0]));
		}

		[AUServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/GetMemberships", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaMember[] GetMemberships(string containerID, string[] memberSchemaTypes, bool normalOnly)
		{
			object[] results = this.Invoke("GetMemberships", new object[] { containerID, memberSchemaTypes, normalOnly });

			return ((ClientSchemaMember[])(results[0]));
		}

		[AUServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/GetObjectsByCodeNames", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase[] GetObjectsByCodeNames(string[] codeNames, string[] objectSchemaTypes, bool normalOnly)
		{
			object[] results = this.Invoke("GetObjectsByCodeNames", new object[] { codeNames, objectSchemaTypes, normalOnly });

			return ((ClientSchemaObjectBase[])(results[0]));
		}

		[AUServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/GetObjectsByIDs", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase[] GetObjectsByIDs(string[] ids, string[] objectSchemaTypes, bool normalOnly)
		{
			object[] results = this.Invoke("GetObjectsByIDs", new object[] { ids, objectSchemaTypes, normalOnly });

			return ((ClientSchemaObjectBase[])(results[0]));
		}

		[AUServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/GetObjectsByXQuery", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase[] GetObjectsByXQuery(string xQuery, string[] objectSchemaTypes, bool normalOnly)
		{
			object[] results = this.Invoke("GetObjectsByXQuery", new object[] { xQuery, objectSchemaTypes, normalOnly });

			return ((ClientSchemaObjectBase[])(results[0]));
		}

		[AUServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/GetParents", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase[] GetParents(string id, string[] parentSchemaTypes, bool normalOnly)
		{
			object[] results = this.Invoke("GetParents", new object[] { id, parentSchemaTypes, normalOnly });

			return ((ClientSchemaObjectBase[])(results[0]));
		}

		[AUServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/GetSchemaPropertyDefinition", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientPropertyDefine[] GetSchemaPropertyDefinition(string schemaType)
		{
			object[] results = this.Invoke("GetSchemaPropertyDefinition", new object[] { schemaType });

			return ((ClientPropertyDefine[])(results[0]));
		}

		[AUServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/GetExtendedPropertyDefinition", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientPropertyDefine[] GetExtendedPropertyDefinition(string schemaType, string sourceID)
		{
			object[] results = this.Invoke("GetExtendedPropertyDefinition", new object[] { schemaType, sourceID });

			return ((ClientPropertyDefine[])(results[0]));
		}

		[AUServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/GetAURole", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientAURole GetAURole(string unitID, string codeName, bool normalOnly)
		{
			object[] results = this.Invoke("GetAURole", new object[] { unitID, codeName, normalOnly });

			return ((ClientAURole)(results[0]));
		}

		[AUServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/GetAUSchemaRoles", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientAUSchemaRole[] GetAUSchemaRoles(string schemaID, string[] codeNames, bool normalOnly)
		{
			object[] results = this.Invoke("GetAUSchemaRoles", new object[] { schemaID, codeNames, normalOnly });

			return ((ClientAUSchemaRole[])(results[0]));
		}

		[AUServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/GetAUSchemaByCategory", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientAUSchema[] GetAUSchemaByCategory(string categoryID, bool normalOnly)
		{
			object[] results = this.Invoke("GetAUSchemaByCategory", new object[] { categoryID, normalOnly });

			return ((ClientAUSchema[])(results[0]));
		}

		[AUServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/GetSubCategories", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientAUSchemaCategory[] GetSubCategories(string parentID, bool normalOnly)
		{
			object[] results = this.Invoke("GetSubCategories", new object[] { parentID, normalOnly });

			return ((ClientAUSchemaCategory[])(results[0]));
		}

		[AUServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/GetAUAdminScope", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientAUAdminScope GetAUAdminScope(string unitID, string scopeType, bool normalOnly)
		{
			object[] results = this.Invoke("GetAUAdminScope", new object[] { unitID, scopeType, normalOnly });

			return ((ClientAUAdminScope)(results[0]));
		}

		[AUServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/GetAURoleBySchemaRoleID", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientAURole GetAURoleBySchemaRoleID(string unitID, string schemaRoleID, bool normalOnly)
		{
			object[] results = this.Invoke("GetAURoleBySchemaRoleID", new object[] { unitID, schemaRoleID, normalOnly });

			return ((ClientAURole)(results[0]));
		}
	}
}
