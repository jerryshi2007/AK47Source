using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ServiceModel.Channels;
using System.Net;
using System.ServiceModel.Description;
using System.Xml;

namespace MCS.Library.WcfExtensions
{
    static class WcfUtils
    {
        public static readonly string JSON_CONTENT_TYPE_STR = @"application/json";

        public static Message SimpleCloneMessage(Message srcMessage, out XmlDocument document)
        {
            Message result = srcMessage;

            document = new XmlDocument();

            if (srcMessage.IsEmpty == false)
            {
                XmlDictionaryReader reader = srcMessage.GetReaderAtBodyContents();

                document.Load(reader);

                XmlNodeReader xmlReader = new XmlNodeReader(document.FirstChild);

                Message newMsg = Message.CreateMessage(srcMessage.Version, null, xmlReader);

                newMsg.Headers.CopyHeadersFrom(srcMessage);

                foreach (string propertyKey in srcMessage.Properties.Keys)
                    newMsg.Properties.Add(propertyKey, srcMessage.Properties[propertyKey]);

                result = newMsg;
            }

            return result;
        }

        /// <summary>
        /// 创建RequestMessage
        /// </summary>
        /// <param name="msgVersion"></param>
        /// <param name="msgAction"></param>
        /// <param name="msgContent"></param>
        /// <returns></returns>
        public static Message CreateJsonFormatRequestMessage(MessageVersion msgVersion, string msgAction, string msgContent)
        {
            var bodyBytes = Encoding.UTF8.GetBytes(msgContent);

            Message requestMessage = Message.CreateMessage(msgVersion, msgAction, new WfRawMessageBodyWriter(bodyBytes));
            requestMessage.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));

            HttpRequestMessageProperty requestProp = new HttpRequestMessageProperty();
            requestProp.Headers[HttpRequestHeader.ContentType] = JSON_CONTENT_TYPE_STR;
            requestMessage.Properties.Add(HttpRequestMessageProperty.Name, requestProp);

            return requestMessage;
        }

        /// <summary>
        /// 创建ReplyMessage
        /// </summary>
        /// <param name="msgVersion"></param>
        /// <param name="msgAction"></param>
        /// <param name="msgContent"></param>
        /// <returns></returns>
        public static Message CreateJsonFormatReplyMessage(MessageVersion msgVersion, string msgAction, string msgContent)
        {
            var bodyBytes = Encoding.UTF8.GetBytes(msgContent);

            Message replyMessage = Message.CreateMessage(msgVersion, msgAction, new WfRawMessageBodyWriter(bodyBytes));
            replyMessage.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));

            HttpResponseMessageProperty respProp = new HttpResponseMessageProperty();
            respProp.Headers[HttpResponseHeader.ContentType] = JSON_CONTENT_TYPE_STR;
            replyMessage.Properties.Add(HttpResponseMessageProperty.Name, respProp);

            return replyMessage;
        }

        public static string GetMessageRawContent(Message message)
        {
            XmlDictionaryReader bodyReader = message.GetReaderAtBodyContents();
            bodyReader.ReadStartElement("Binary");
            byte[] rawBody = bodyReader.ReadContentAsBase64();

            using (MemoryStream ms = new MemoryStream(rawBody))
            {
                using (StreamReader sr = new StreamReader(ms))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public static string GetMessageRawContent(XmlDocument document)
        {
            string result = string.Empty;

            if (document != null)
            {
                if (document.DocumentElement != null && document.DocumentElement.Name == "Binary")
                {
                    string base64Text = document.DocumentElement.InnerText;

                    byte[] rawBody = Convert.FromBase64String(base64Text);

                    using (MemoryStream ms = new MemoryStream(rawBody))
                    {
                        using (StreamReader sr = new StreamReader(ms))
                        {
                            result = sr.ReadToEnd();
                        }
                    }
                }
            }

            return result;
        }

        #region Extended Methods
        /// <summary>
        /// 在Behavior集合中查找指定类型的Behavior，如果没有，则创建一个。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="behaviors"></param>
        /// <returns></returns>
        public static T GetBehavior<T>(this KeyedByTypeCollection<IServiceBehavior> behaviors) where T : IServiceBehavior, new()
        {
            T behavior = default(T);

            if (behaviors.Contains(typeof(T)))
                behavior = (T)behaviors[typeof(T)];
            else
            {
                behavior = new T();
                behaviors.Add(behavior);
            }

            return behavior;
        }

        /// <summary>
        /// 根据Contract的名字查找EndPoint
        /// </summary>
        /// <param name="endpoints"></param>
        /// <param name="contractName"></param>
        /// <returns></returns>
        public static ServiceEndpoint FindByContractName(this ServiceEndpointCollection endpoints, string contractName)
        {
            ServiceEndpoint result = null;

            foreach (ServiceEndpoint endpoint in endpoints)
            {
                if (endpoint.Contract.ConfigurationName == contractName)
                {
                    result = endpoint;
                    break;
                }
            }

            return result;
        }
        #endregion
    }
}
