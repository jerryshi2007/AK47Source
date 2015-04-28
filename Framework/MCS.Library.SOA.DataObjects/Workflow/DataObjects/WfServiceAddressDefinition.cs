using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 服务的地址定义
    /// </summary>
    [Serializable]
    [XElementSerializable]
    public class WfServiceAddressDefinition : ISimpleXmlSerializer
    {
        private string _Key;
        private string _Address;
        private string _ServiceNS;

        public WfServiceRequestMethod RequestMethod = WfServiceRequestMethod.Get;

        public WfNetworkCredential Credential;

        public WfServiceContentType ContentType = WfServiceContentType.Form;

        /// <summary>
        /// 构造方法
        /// </summary>
        public WfServiceAddressDefinition()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="key"></param>
        public WfServiceAddressDefinition(string key)
        {
            this.Key = key;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="method"></param>
        /// <param name="address"></param>
        /// <param name="contentType"></param>
        public WfServiceAddressDefinition(
            WfServiceRequestMethod method,
            string address,
            WfServiceContentType contentType)
        {
            this.RequestMethod = method;
            this.Address = address;
            this.ContentType = contentType;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="method"></param>
        /// <param name="credential"></param>
        /// <param name="address"></param>
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

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="method"></param>
        /// <param name="credential"></param>
        /// <param name="address"></param>
        /// <param name="contentType"></param>
        public WfServiceAddressDefinition(WfServiceRequestMethod method,
            WfNetworkCredential credential, string address, WfServiceContentType contentType)
        {
            this.RequestMethod = method;
            this.Credential = credential;
            this.Address = address;
            this.ContentType = contentType;
        }

        /// <summary>
        /// 从配置项初始化
        /// </summary>
        /// <param name="element"></param>
        public WfServiceAddressDefinition(WfServiceAddressDefinitionConfigurationElement element)
        {
            this.FromConfigurationElement(element);
        }

        /// <summary>
        /// 从配置项初始化
        /// </summary>
        /// <param name="element"></param>
        public void FromConfigurationElement(WfServiceAddressDefinitionConfigurationElement element)
        {
            element.NullCheck("element");

            this.Key = element.Name;
            this.RequestMethod = element.RequestMethod;
            this.ContentType = element.ContentType;
            this.ServiceNS = element.ServiceNS;

            this.Address = element.Uri.ToUriString();

            if (element.Identity != null)
                this.Credential = new WfNetworkCredential(element.Identity.LogOnNameWithoutDomain, element.Identity.Password, element.Identity.Domain);
        }

        public string Key
        {
            get
            {
                return this._Key;
            }
            set
            {
                this._Key = value;
            }
        }

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

        /// <summary>
        /// 服务命名空间，末尾带/字符
        /// 即wsdl中的targetNamespace
        /// </summary>
        public string ServiceNS
        {
            get
            {
                return this._ServiceNS;
            }
            set
            {
                this._ServiceNS = value;
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
