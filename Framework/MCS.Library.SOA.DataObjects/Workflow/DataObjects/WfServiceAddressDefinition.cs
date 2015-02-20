using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public enum WfServiceRequestMethod
	{
		Get,
		Post,
		Soap
	}

	public enum WfServiceContentType
	{
		Form,
		Json
	}

	[Serializable]
	[XElementSerializable]
	public class WfServiceAddressDefinition : ISimpleXmlSerializer
	{
		public WfServiceAddressDefinition()
		{

		}

		public WfServiceAddressDefinition(
			WfServiceRequestMethod method,
			string address,
			WfServiceContentType contentType)
		{
			this.RequestMethod = method;
			this.Address = address;
			this.ContentType = contentType;
		}

		public WfServiceAddressDefinition(
			WfServiceRequestMethod method,
			WfNetworkCredential credential,
			string address)
		{
			this.RequestMethod = method;
			this.Credential = credential;
			this.Address = address;
			this.ContentType = WfServiceContentType.Form;
		}

		public WfServiceAddressDefinition(WfServiceRequestMethod method,
			WfNetworkCredential credential, string address, WfServiceContentType contentType)
		{
			this.RequestMethod = method;
			this.Credential = credential;
			this.Address = address;
			this.ContentType = contentType;
		}

		public WfServiceRequestMethod RequestMethod;
		public WfNetworkCredential Credential;
		public WfServiceContentType ContentType;

		private string _Key;
		public string Key
		{
			get
			{
				return _Key;
			}
			set
			{
				_Key = value;
			}
		}

		private string _Address;
		/// <summary>
		/// 服务地址，末尾带/字符
		/// </summary>
		public string Address
		{
			get
			{
				return _Address;
			}
			set
			{
				_Address = value;
			}
		}

		private string _ServiceNS;
		/// <summary>
		/// 服务命名空间，末尾带/字符
		/// 即wsdl中的targetNamespace
		/// </summary>
		public string ServiceNS
		{
			get
			{
				return _ServiceNS;
			}
			set
			{
				_ServiceNS = value;
			}
		}

		#region ISimpleXmlSerializer Members

		void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
		{
			if (refNodeName.IsNotEmpty())
				element = element.AddChildElement(refNodeName);

			element.SetAttributeValue("key", this.Key);
			element.SetAttributeValue("serviceNS", this.ServiceNS);
			element.SetAttributeValue("address", this.Address);
		}

		#endregion
	}

	[Serializable]
	[XElementSerializable]
	public class WfServiceAddressDefinitionCollection : SerializableEditableKeyedDataObjectCollectionBase<string, WfServiceAddressDefinition>
	{
		protected override string GetKeyForItem(WfServiceAddressDefinition item)
		{
			return item.Key;
		}
	}
}
