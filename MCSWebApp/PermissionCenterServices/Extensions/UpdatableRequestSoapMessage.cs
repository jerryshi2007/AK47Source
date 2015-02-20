using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.Services;

namespace PermissionCenter.Extensions
{
    [Serializable]
    public class UpdatableRequestSoapMessage : SimpleRequestSoapMessage
    {
        private bool _WithLock;

        private UpdatableRequestSoapMessage()
            : base()
        {
        }

        /// <summary>
        /// 创建一个消息
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        public new static UpdatableRequestSoapMessage CreateMessage(Stream inputStream)
        {
            UpdatableRequestSoapMessage message = new UpdatableRequestSoapMessage();

            message.InitFromStream(inputStream);

            return message;
        }

        public bool WithLock
        {
            get
            {
                return this._WithLock;
            }
            set
            {
                this._WithLock = value;
            }
        }

        protected override void ReadHeaderNode(XmlNamespaceManager namespaceManager, XmlNode headerNode)
        {
            base.ReadHeaderNode(namespaceManager, headerNode);

            this._WithLock = headerNode.GetSingleNodeValue("t:SchemaServiceUpdatableClientSoapHeader/t:WithLock", namespaceManager, true);
        }
    }
}