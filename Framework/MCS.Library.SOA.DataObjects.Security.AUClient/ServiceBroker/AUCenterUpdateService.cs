using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using MCS.Library.SOA.DataObjects.Schemas.Client;
using MCS.Library.SOA.DataObjects.Security.AUClient.Configuration;
using System.Web.Services.Protocols;
using System.Web.Services.Description;

namespace MCS.Library.SOA.DataObjects.Security.AUClient.ServiceBroker
{
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Web.Services.WebServiceBindingAttribute(Name = "PermissionCenterUpdateServiceSoap", Namespace = "http://tempuri.org/")]
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
	public class AUCenterUpdateService : System.Web.Services.Protocols.SoapHttpClientProtocol, IAUCenterUpdateService
	{
		public static readonly AUCenterUpdateService Instance = new AUCenterUpdateService();

		#region Internal
		private bool useDefaultCredentialsSetExplicitly;

		public AUCenterUpdateService()
		{
			AUUpdateServiceBrokerContext.Current.InitWebClientProtocol(this);

			this.Url = AUServiceClientSettings.GetConfig().UpdateServiceAddress.ToString();

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

		[AUUpdateServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/AddAdminSchema", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void AddAdminSchema(ClientAUSchema schema)
		{
			this.Invoke("AddAdminSchema", new object[] { schema });
		}

		[AUUpdateServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/AddAdminSchemaRole", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void AddAdminSchemaRole(ClientAUSchemaRole role, ClientAUSchema schema)
		{
			this.Invoke("AddAdminSchemaRole", new object[] { role, schema });
		}

		[AUUpdateServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/AddAdminUnit", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void AddAdminUnit(ClientAdminUnit unit, ClientAdminUnit parent)
		{
			this.Invoke("AddAdminUnit", new object[] { unit, parent });
		}

		[AUUpdateServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/AddAdminUnitWithMembers", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void AddAdminUnitWithMembers(ClientAdminUnit unit, ClientAdminUnit parent, ClientAURole[] roles, ClientAUAdminScope[] scopes)
		{
			this.Invoke("AddAdminUnitWithMembers", new object[] { unit, parent, roles, scopes });
		}

		[AUUpdateServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/AddObjectToScope", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void AddObjectToScope(ClientAUAdminScopeItem item, ClientAUAdminScope scope)
		{
			this.Invoke("AddObjectToScope", new object[] { item, scope });
		}

		[AUUpdateServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/AddUserToRole", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void AddUserToRole(ClientGenericObject user, ClientAdminUnit unit, ClientAUSchemaRole role)
		{
			this.Invoke("AddUserToRole", new object[] { user, unit, role });
		}

		[AUUpdateServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/DeleteAdminSchemaRole", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void DeleteAdminSchemaRole(ClientAUSchemaRole role)
		{
			this.Invoke("DeleteAdminSchemaRole", new object[] { role });
		}

		[AUUpdateServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/DeleteAdminUnit", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void DeleteAdminUnit(ClientAdminUnit unit)
		{
			this.Invoke("DeleteAdminUnit", new object[] { unit });
		}

		[AUUpdateServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/GetFacadeType", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string GetFacadeType()
		{
			return (string)this.Invoke("GetFacadeType", new object[0])[0];
		}

		[AUUpdateServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/MoveAdminUnit", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void MoveAdminUnit(ClientAdminUnit unit, ClientAdminUnit newParent)
		{
			this.Invoke("MoveAdminUnit", new object[] { unit, newParent });
		}

		[AUUpdateServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/NewID", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string NewID()
		{
			return this.Invoke("NewID", new object[0])[0] as string;
		}

		[AUUpdateServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/RemoveObjectFromScope", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void RemoveObjectFromScope(ClientAUAdminScopeItem item, ClientAUAdminScope scope)
		{
			this.Invoke("RemoveObjectFromScope", new object[] { item, scope });
		}

		[AUUpdateServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/RemoveUserFromRole", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void RemoveUserFromRole(ClientGenericObject user, ClientAdminUnit unit, ClientAUSchemaRole role)
		{
			this.Invoke("RemoveUserFromRole", new object[] { user, unit, role });
		}

		[AUUpdateServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/ReplaceAclRecursively", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void ReplaceAclRecursively(string ownerID, bool force)
		{
			this.Invoke("ReplaceAclRecursively", new object[] { ownerID, force });
		}

		[AUUpdateServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/ReplaceUsersInRole", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void ReplaceUsersInRole(ClientGenericObject[] users, ClientAdminUnit unit, ClientAUSchemaRole role)
		{
			this.Invoke("ReplaceUsersInRole", new object[] { users, unit, role });
		}

		[AUUpdateServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/UpdateAdminSchemaRole", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void UpdateAdminSchemaRole(ClientAUSchemaRole role)
		{
			this.Invoke("UpdateAdminSchemaRole", new object[] { role });
		}

		[AUUpdateServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/UpdateAdminUnit", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void UpdateAdminUnit(ClientAdminUnit unit)
		{
			this.Invoke("UpdateAdminUnit", new object[] { unit });
		}

		[AUUpdateServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/UpdateObjectAcl", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void UpdateObjectAcl(string ownerID, Schemas.Client.ClientAclItem[] clientAcls)
		{
			this.Invoke("UpdateObjectAcl", new object[] { ownerID, clientAcls });
		}

		[AUUpdateServiceBrokerExtension]
		[SoapDocumentMethod("http://tempuri.org/UpdateScopeCondition", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void UpdateScopeCondition(ClientAUAdminScope scope, Schemas.Client.ClientConditionItem condition)
		{
			this.Invoke("UpdateScopeCondition", new object[] { scope, condition });
		}
	}
}
