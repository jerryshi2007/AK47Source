using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using MCS.Library.Caching;
using MCS.Library.Core;

namespace MCS.Library.Services
{
    /// <summary>
    /// 简单的Soap请求信息
    /// </summary>
    public class SimpleRequestSoapMessage
    {
        private XmlDocument _Document = null;
        private bool _useServerCache = true;
        private DateTime _timePoint = DateTime.MinValue;
        private readonly Dictionary<string, string> _ConnectionMappings = new Dictionary<string, string>();
        private readonly Dictionary<string, object> _Context = new Dictionary<string, object>();

        /// <summary>
        /// 
        /// </summary>
        protected SimpleRequestSoapMessage()
        {
        }

        /// <summary>
        /// 创建一个消息
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        public static SimpleRequestSoapMessage CreateMessage(Stream inputStream)
        {
            SimpleRequestSoapMessage message = new SimpleRequestSoapMessage();

            message.InitFromStream(inputStream);

            return message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        public void InitFromStream(Stream inputStream)
        {
            inputStream.NullCheck("inputStream");

            this._Document = new XmlDocument();

            this._Document.Load(inputStream);

            InitProperties();
        }

        /// <summary>
        /// 当前实例
        /// </summary>
        public static SimpleRequestSoapMessage Current
        {
            get
            {
                SimpleRequestSoapMessage result = null;

                if (ObjectContextCache.Instance.ContainsKey("SimpleRequestSoapMessage"))
                    result = (SimpleRequestSoapMessage)ObjectContextCache.Instance["SimpleRequestSoapMessage"];
                else
                    result = new SimpleRequestSoapMessage();

                return result;
            }
            set
            {
                ObjectContextCache.Instance["SimpleRequestSoapMessage"] = value;
            }
        }

        /// <summary>
        /// 文档对象
        /// </summary>
        public XmlDocument Document
        {
            get
            {
                return this._Document;
            }
        }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName
        {
            get;
            set;
        }

        /// <summary>
        /// 方法（动作）名称
        /// </summary>
        public string Action
        {
            get;
            private set;
        }

        /// <summary>
        /// 是否使用服务器端的缓存
        /// </summary>
        public bool UseServerCache
        {
            get
            {
                return this._useServerCache;
            }
            set
            {
                this._useServerCache = value;
            }
        }

        /// <summary>
        /// 调用查询方法时的TimePoint参数
        /// </summary>
        public DateTime TimePoint
        {
            get
            {
                return this._timePoint;
            }
            set
            {
                this._timePoint = value;
            }
        }

        /// <summary>
        /// 方法缓存的Key
        /// </summary>
        public string MethodCacheKey
        {
            get
            {
                string result = this.ServiceName + "~" + this.Action;

                if (TenantContext.Current.Enabled)
                    result += "~" + TenantContext.Current.TenantCode;

                return result;
            }
        }

        /// <summary>
        /// 连接映射
        /// </summary>
        public Dictionary<string, string> ConnectionMappings
        {
            get
            {
                return this._ConnectionMappings;
            }
        }

        /// <summary>
        /// 上下文参数
        /// </summary>
        public Dictionary<string, object> Context
        {
            get
            {
                return this._Context;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitProperties()
        {
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(this._Document.NameTable);

            namespaceManager.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            namespaceManager.AddNamespace("t", "http://tempuri.org/");

            XmlNode headerNode = this._Document.DocumentElement.SelectSingleNode("soap:Header", namespaceManager);

            if (headerNode != null)
            {
                this.ReadHeaderNode(namespaceManager, headerNode);
            }

            XmlNode bodyNode = this._Document.DocumentElement.SelectSingleNode("soap:Body", namespaceManager);

            if (bodyNode != null && bodyNode.FirstChild != null)
            {
                this.Action = bodyNode.FirstChild.Name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namespaceManager"></param>
        /// <param name="headerNode"></param>
        protected virtual void ReadHeaderNode(XmlNamespaceManager namespaceManager, XmlNode headerNode)
        {
            this._useServerCache = XmlHelper.GetSingleNodeValue(headerNode, "t:ServiceBrokerSoapHeader/t:UseServerCache", namespaceManager, true);
            this._timePoint = XmlHelper.GetSingleNodeValue(headerNode, "t:ServiceBrokerSoapHeader/t:TimePoint", namespaceManager, DateTime.MinValue);

            XmlNodeList connectionMappingNodes = headerNode.SelectNodes("t:ServiceBrokerSoapHeader/t:ConnectionMappings/t:SoapHeaderConnectionMappingItem", namespaceManager);

            foreach (XmlNode mappingNode in connectionMappingNodes)
            {
                this.ConnectionMappings[XmlHelper.GetSingleNodeText(mappingNode, "t:Source", namespaceManager)] =
                    XmlHelper.GetSingleNodeText(mappingNode, "t:Destination", namespaceManager);
            }

            XmlNodeList contextNodes = headerNode.SelectNodes("t:ServiceBrokerSoapHeader/t:Context/t:SoapHeaderContextItem", namespaceManager);

            foreach (XmlNode contextNode in contextNodes)
            {
                this.Context[XmlHelper.GetSingleNodeText(contextNode, "t:Key", namespaceManager)] =
                   XmlHelper.GetSingleNodeText(contextNode, "t:Value", namespaceManager);
            }
        }
    }
}