using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    [Serializable]
    [XElementSerializable]
    public class WfServiceOperationDefinition : ISimpleXmlSerializer
    {
        private WfServiceAddressDefinition _AddressDef = null;
        private WfServiceOperationParameterCollection _Params;
        private TimeSpan _Timeout = TimeSpan.FromSeconds(30);

        /// <summary>
        /// 构造方法
        /// </summary>
        public WfServiceOperationDefinition()
        {
        }

        public WfServiceOperationDefinition(string addressKey,
            string operationName,
            WfServiceOperationParameterCollection parameters,
            string xmlStoreParaName)
        {
            if (WfGlobalParameters.Default.ServiceAddressDefs[addressKey] == null)
                throw new ArgumentException(string.Format("无法找到Key为{0}的服务地址定义", addressKey));

            this._AddressDef = WfGlobalParameters.Default.ServiceAddressDefs[addressKey];
            this.OperationName = operationName;
            this.Params = parameters;
            this.RtnXmlStoreParamName = xmlStoreParaName;
        }

        public WfServiceOperationDefinition(WfServiceAddressDefinition address,
            string operationName)
        {
            this._AddressDef = address;
            this.OperationName = operationName;
        }

        public WfServiceOperationDefinition(WfServiceAddressDefinition address,
            string operationName, WfServiceOperationParameterCollection parameters,
            string xmlStoreParaName)
        {
            this._AddressDef = address;
            this.OperationName = operationName;
            this.Params = parameters;
            this.RtnXmlStoreParamName = xmlStoreParaName;
        }

        /// <summary>
        /// 从配置信息项初始化
        /// </summary>
        /// <param name="element"></param>
        public WfServiceOperationDefinition(WfServiceOperationDefinitionConfigurationElement element)
        {
            this.FromConfigurationElement(element);
        }

        /// <summary>
        /// 从配置信息项初始化
        /// </summary>
        /// <param name="element"></param>
        public void FromConfigurationElement(WfServiceOperationDefinitionConfigurationElement element)
        {
            element.NullCheck("element");

            this._AddressDef = new WfServiceAddressDefinition(element.AddressKey);

            this.OperationName = element.OperationName;
            this.Params = new WfServiceOperationParameterCollection(element.Parameters);
            this.RtnXmlStoreParamName = element.ReturnParamName;
            this.Timeout = element.Timeout;
            this.InvokeWhenPersist = element.InvokeWhenPersist;
        }

        ///// <summary>
        ///// 兼容2012-02-03之前保存的数据，故保留该属性
        ///// </summary>
        //public string AddressKey
        //{
        //    get;
        //    set;
        //}

        public WfServiceAddressDefinition AddressDef
        {
            get
            {
                //// 如果全局地址表包含Key，那么就是用此Key去全局地址表取地址，否则使用默认的。
                //if (string.IsNullOrEmpty(this.AddressKey) == false)
                //{
                //    this._AddressDef = WfGlobalParameters.Default.ServiceAddressDefs[this.AddressKey];
                //}
                //else
                //{
                //    if (this._AddressDef != null && string.IsNullOrEmpty(this._AddressDef.Key) == false && WfGlobalParameters.Default.ServiceAddressDefs.ContainsKey(this._AddressDef.Key))
                //    {
                //        return WfGlobalParameters.Default.ServiceAddressDefs[this._AddressDef.Key];
                //    }
                //}
                WfServiceAddressDefinition result = this._AddressDef;

                if (this._AddressDef != null && this._AddressDef.Key.IsNotEmpty())
                {
                    if (WfGlobalParameters.Default.ServiceAddressDefs.ContainsKey(this._AddressDef.Key))
                    {
                        result = WfGlobalParameters.Default.ServiceAddressDefs[this._AddressDef.Key];
                    }
                    else
                    {
                        WfServiceAddressDefinitionConfigurationElement addressElement = WfServiceDefinitionSettings.GetSection().Addresses[this._AddressDef.Key];

                        if (addressElement != null)
                            result = new WfServiceAddressDefinition(addressElement);
                    }
                }

                return result;
            }
            internal set
            {
                this._AddressDef = value;
            }
        }

        /// <summary>
        /// 是否在流程持久化时调用。
        /// </summary>
        public bool InvokeWhenPersist
        {
            get;
            set;
        }

        public string OperationName
        {
            get;
            set;
        }

        /// <summary>
        /// 调用服务超时间 
        /// </summary>
        //2012-11-28
        public TimeSpan Timeout
        {
            get
            {
                return this._Timeout;
            }
            set
            {
                this._Timeout = value;
            }
        }

        public WfServiceOperationParameterCollection Params
        {
            get
            {
                if (this._Params == null)
                    this._Params = new WfServiceOperationParameterCollection();

                return _Params;
            }
            set
            {
                this._Params = value;
            }
        }

        /// <summary>
        /// 服务返回的xml存放在流程中的参数名
        /// </summary>
        public string RtnXmlStoreParamName
        {
            get;
            set;
        }

        public string Key
        {
            get;
            set;
        }

        public WfServiceOperationDefinition Clone()
        {
            return (WfServiceOperationDefinition)SerializationHelper.CloneObject(this);
        }

        #region ISimpleXmlSerializer Members

        void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
        {
            if (refNodeName.IsNotEmpty())
                element = element.AddChildElement(refNodeName);

            element.SetAttributeValue("key", this.Key);
            element.SetAttributeValue("rtnXmlStoreParamName", this.RtnXmlStoreParamName);

            if (this._Params != null)
                ((ISimpleXmlSerializer)this._Params).ToXElement(element, "Params");

            if (this.AddressDef != null)
                ((ISimpleXmlSerializer)this.AddressDef).ToXElement(element, "AddressDef");
        }

        #endregion
    }

    [Serializable]
    [XElementSerializable]
    public class WfServiceOperationDefinitionCollection : EditableDataObjectCollectionBase<WfServiceOperationDefinition>, ISimpleXmlSerializer
    {
        #region ISimpleXmlSerializer Members

        void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
        {
            if (refNodeName.IsNotEmpty())
                element = element.AddChildElement(refNodeName);

            foreach (WfServiceOperationDefinition operation in this)
                ((ISimpleXmlSerializer)operation).ToXElement(element, "Operation");
        }

        #endregion

        /// <summary>
        /// 得到在持久化时需要调用的服务
        /// </summary>
        /// <returns></returns>
        public WfServiceOperationDefinitionCollection GetServiceOperationsWhenPersist()
        {
            WfServiceOperationDefinitionCollection result = new WfServiceOperationDefinitionCollection();

            foreach (WfServiceOperationDefinition serviceOpDef in this)
            {
                if (serviceOpDef.InvokeWhenPersist)
                    result.Add(serviceOpDef);
            }

            return result;
        }

        /// <summary>
        /// 得到在持久化之前需要调用的服务
        /// </summary>
        /// <returns></returns>
        public WfServiceOperationDefinitionCollection GetServiceOperationsBeforePersist()
        {
            WfServiceOperationDefinitionCollection result = new WfServiceOperationDefinitionCollection();

            foreach (WfServiceOperationDefinition serviceOpDef in this)
            {
                if (serviceOpDef.InvokeWhenPersist == false)
                    result.Add(serviceOpDef);
            }

            return result;
        }

        public void SyncPropertiesToFields(PropertyValue property)
        {
            if (property != null)
            {
                this.Clear();

                if (property.StringValue.IsNotEmpty())
                {
                    IEnumerable<WfServiceOperationDefinition> deserializedData = (IEnumerable<WfServiceOperationDefinition>)JSONSerializerExecute.DeserializeObject(property.StringValue, this.GetType());

                    this.CopyFrom(deserializedData);
                }
            }
        }
    }
}
