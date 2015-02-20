using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using MCS.Library.SOA.DataObjects.Schemas.Client;

namespace MCS.Library.SOA.DataObjects.Security.Client
{
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Web.Services.WebServiceBindingAttribute(Name = "PermissionCenterUpdateServiceSoap", Namespace = "http://tempuri.org/")]
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
	[XmlInclude(typeof(ClientSchemaMember))]
	[XmlInclude(typeof(ClientSchemaRelation))]
	[XmlInclude(typeof(ClientSchemaObjectCollection))]
	public class PermissionCenterUpdateService : System.Web.Services.Protocols.SoapHttpClientProtocol
	{
		public static readonly PermissionCenterUpdateService Instance = new PermissionCenterUpdateService();

		private bool useDefaultCredentialsSetExplicitly;

		public PermissionCenterUpdateService()
		{
			PCUpdateServiceBrokerContext.Current.InitWebClientProtocol(this);

			this.Url = PCServiceClientSettings.GetConfig().UpdateServiceAddress.ToString();

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

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/GetFacadeType", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string GetFacadeType()
		{
			return (string)this.Invoke("GetFacadeType", new object[0])[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/NewID", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string NewID()
		{
			return this.Invoke("NewID", new object[0])[0] as string;
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/Echo", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase Echo(ClientSchemaObjectBase clientObj, bool viaSCObject)
		{
			return this.Invoke("Echo", new object[] { clientObj, viaSCObject })[0] as ClientSchemaObjectBase;
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/AddApplication", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSCBase AddApplication(ClientSCApplication clientApp)
		{
			return (ClientSCBase)this.Invoke("AddApplication", new object[] { clientApp })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/UpdateApplication", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase UpdateApplication(ClientSCApplication clientApp)
		{
			return (ClientSchemaObjectBase)this.Invoke("UpdateApplication", new object[] { clientApp })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/DeleteApplication", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase DeleteApplication(ClientSCApplication clientApp)
		{
			return (ClientSchemaObjectBase)this.Invoke("DeleteApplication", new object[] { clientApp })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/AddRole", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase AddRole(ClientSCRole clientRole, ClientSCApplication clientApp)
		{
			return (ClientSchemaObjectBase)this.Invoke("AddRole", new object[] { clientRole, clientApp })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/UpdateRole", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase UpdateRole(ClientSCRole clientRole)
		{
			return (ClientSchemaObjectBase)this.Invoke("UpdateRole", new object[] { clientRole })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/DeleteRole", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase DeleteRole(ClientSCRole clientRole)
		{
			return (ClientSchemaObjectBase)this.Invoke("DeleteRole", new object[] { clientRole })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/AddPermission", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase AddPermission(ClientSCPermission clientPermission, ClientSCApplication clientApp)
		{
			return (ClientSchemaObjectBase)this.Invoke("AddPermission", new object[] { clientPermission, clientApp })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/UpdatePermission", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase UpdatePermission(ClientSCPermission clientPermission)
		{
			return (ClientSchemaObjectBase)this.Invoke("UpdatePermission", new object[] { clientPermission })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/DeletePermission", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase DeletePermission(ClientSCPermission clientPermission)
		{
			return (ClientSchemaObjectBase)this.Invoke("DeletePermission", new object[] { clientPermission })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/JoinRoleAndPermission", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaRelation JoinRoleAndPermission(ClientSCRole clientRole, ClientSCPermission clientPermission)
		{
			return (ClientSchemaRelation)this.Invoke("JoinRoleAndPermission", new object[] { clientRole, clientPermission })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/DisjoinRoleAndPermission", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaRelation DisjoinRoleAndPermission(ClientSCRole clientRole, ClientSCPermission clientPermission)
		{
			return (ClientSchemaRelation)this.Invoke("DisjoinRoleAndPermission", new object[] { clientRole, clientPermission })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/AddOrganization", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase AddOrganization(ClientSCOrganization clientOrganization, ClientSCOrganization clientParent)
		{
			return (ClientSchemaObjectBase)this.Invoke("AddOrganization", new object[] { clientOrganization, clientParent })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/UpdateOrganization", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase UpdateOrganization(ClientSCOrganization clientOrganization)
		{
			return (ClientSchemaObjectBase)this.Invoke("UpdateOrganization", new object[] { clientOrganization })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/DeleteOrganization", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase DeleteOrganization(ClientSCOrganization clientOrganization, ClientSCOrganization clientParent)
		{
			return (ClientSchemaObjectBase)this.Invoke("DeleteOrganization", new object[] { clientOrganization, clientParent })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/AddUser", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase AddUser(ClientSCUser clientUser, ClientSCOrganization clientParent)
		{
			return (ClientSchemaObjectBase)this.Invoke("AddUser", new object[] { clientUser, clientParent })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/UpdateUser", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase UpdateUser(ClientSCUser clientUser)
		{
			return (ClientSchemaObjectBase)this.Invoke("UpdateUser", new object[] { clientUser })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/DeleteUser", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase DeleteUser(ClientSCUser clientUser, ClientSCOrganization clientParent)
		{
			return (ClientSchemaObjectBase)this.Invoke("DeleteUser", new object[] { clientUser, clientParent })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/AddGroup", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase AddGroup(ClientSCGroup clientGroup, ClientSCOrganization clientParent)
		{
			return (ClientSchemaObjectBase)this.Invoke("AddGroup", new object[] { clientGroup, clientParent })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/UpdateGroup", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase UpdateGroup(ClientSCGroup clientGroup)
		{
			return (ClientSchemaObjectBase)this.Invoke("UpdateGroup", new object[] { clientGroup })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/DeleteGroup", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase DeleteGroup(ClientSCGroup clientGroup, ClientSCOrganization clientParent)
		{
			return (ClientSchemaObjectBase)this.Invoke("DeleteGroup", new object[] { clientGroup, clientParent })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/ChangeOwner", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase ChangeOwner(ClientSCBase clientObject, ClientSCOrganization clientParent)
		{
			return (ClientSchemaObjectBase)this.Invoke("ChangeOwner", new object[] { clientObject, clientParent })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/SetUserDefaultOrganization", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaRelation SetUserDefaultOrganization(ClientSCUser clientUser, ClientSCOrganization clientParent)
		{
			return (ClientSchemaRelation)this.Invoke("SetUserDefaultOrganization", new object[] { clientUser, clientParent })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/DeleteObjectsRecursively", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSCOrganization DeleteObjectsRecursively(ClientSchemaObjectCollection clientObjects, ClientSCOrganization clientParent)
		{
			return (ClientSCOrganization)this.Invoke("DeleteObjectsRecursively", new object[] { clientObjects, clientParent })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/MoveObjectToOrganization", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaRelation MoveObjectToOrganization(ClientSCOrganization clientSource, ClientSCBase clientScObject, ClientSCOrganization clientTarget)
		{
			return (ClientSchemaRelation)this.Invoke("MoveObjectToOrganization", new object[] { clientSource, clientScObject, clientTarget })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/AddUserToGroup", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaMember AddUserToGroup(ClientSCUser clientUser, ClientSCGroup clientGroup)
		{
			return (ClientSchemaMember)this.Invoke("AddUserToGroup", new object[] { clientUser, clientGroup })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/AddSecretaryToUser", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaMember AddSecretaryToUser(ClientSCUser clientSecretary, ClientSCUser clientUser)
		{
			return (ClientSchemaMember)this.Invoke("AddSecretaryToUser", new object[] { clientSecretary, clientUser })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/AddMemberToRole", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaMember AddMemberToRole(ClientSCBase clientObject, ClientSCRole clientRole)
		{
			return (ClientSchemaMember)this.Invoke("AddMemberToRole", new object[] { clientObject, clientRole })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/RemoveUserFromGroup", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaMember RemoveUserFromGroup(ClientSCUser clientUser, ClientSCGroup clientGroup)
		{
			return (ClientSchemaMember)this.Invoke("RemoveUserFromGroup", new object[] { clientUser, clientGroup })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/RemoveSecretaryFromUser", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaMember RemoveSecretaryFromUser(ClientSCUser clientSecretary, ClientSCUser clientUser)
		{
			return (ClientSchemaMember)this.Invoke("RemoveSecretaryFromUser", new object[] { clientSecretary, clientUser })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/RemoveMemberFromRole", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaMember RemoveMemberFromRole(ClientSCBase clientObject, ClientSCRole clientRole)
		{
			return (ClientSchemaMember)this.Invoke("RemoveMemberFromRole", new object[] { clientObject, clientRole })[0];
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/UpdateObjectAcl", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void UpdateObjectAcl(string ownerID, ClientAclItem[] clientAcls)
		{
			this.Invoke("UpdateObjectAcl", new object[] { ownerID, clientAcls });
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/ReplaceAclRecursively", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void ReplaceAclRecursively(string ownerID, bool force)
		{
			this.Invoke("ReplaceAclRecursively", new object[] { ownerID, force });
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/UpdateGroupConditions", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void UpdateGroupConditions(string ownerID, string conditionType, ClientConditionItem[] items)
		{
			this.Invoke("UpdateGroupConditions", new object[] { ownerID, conditionType, items });
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/UpdateRoleConditions", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void UpdateRoleConditions(string ownerID, string conditionType, ClientConditionItem[] items)
		{
			this.Invoke("UpdateRoleConditions", new object[] { ownerID, conditionType, items });
		}

		[PCUpdateServiceBrokerExtension]
		[SoapDocumentMethodAttribute("http://tempuri.org/UpdateObjectImageProperty", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ClientSchemaObjectBase UpdateObjectImageProperty(ClientSchemaObjectBase obj, string propertyName, string imageID)
		{
			return (ClientSchemaObjectBase)this.Invoke("UpdateObjectImageProperty", new object[] { obj, propertyName, imageID })[0];
		}
	}
}
