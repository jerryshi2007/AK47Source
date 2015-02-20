using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MCS.Library.Core;
using MCS.Web.Apps.WeChat.Adapters;
using MCS.Web.Apps.WeChat.DataObjects;
using MCS.Web.Library.MVC;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Xml;
using System.IO;

namespace WeChatConnectInService.Services
{
    /// <summary>
    /// Summary description for ConnectIn
    /// </summary>
    public class ConnectIn : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                object result = ControllerHelper.ExecuteMethodByRequest(this);

                context.Response.Write(result);
            }
            catch (System.Exception ex)
            {
                context.Response.Write(ex.GetRealException().ToString());
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        [ControllerMethod]
        protected string ReceiveMessage(string signature, string timestamp, string nonce, string echostr)
        {
            StringBuilder strB = new StringBuilder();
            VerifySignature(signature, timestamp, nonce);
            string result = echostr;

            strB.AppendLine(GetRequestParams());

            string postedData = GetPostedData();

            if (postedData != null)
                strB.AppendLine(postedData);

            WriteEventLog(strB.ToString());

            if (postedData.IsNotEmpty())
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(postedData);
                var receivedMessage = WeChatIncomeMessageCreator.Create(xmlDoc);

                switch (receivedMessage.MessageType)
                {
                    case WeChatMessageType.Text:
                        WeChatTextIncomeMessageAdapter.Instance.Update(receivedMessage as WeChatTextIncomeMessage);
                        break;
                    case WeChatMessageType.Location:
                        WeChatLocationIncomeMessageAdapter.Instance.Update(receivedMessage as WeChatLocationIncomeMessage);
                        break;
                    case WeChatMessageType.Image:
                        WeChatImageIncomeMessageAdapter.Instance.Update(receivedMessage as WeChatImageIncomeMessage);
                        break;
                    case WeChatMessageType.Video:
                        WeChatVideoIncomeMessageAdapter.Instance.Update(receivedMessage as WeChatVideoIncomeMessage);
                        break;
                    case WeChatMessageType.Voice:
                        WeChatVoiceIncomeMessageAdapter.Instance.Update(receivedMessage as WeChatVoiceIncomeMessage);
                        break;
                    case WeChatMessageType.Link:
                        WeChatLinkIncomeMessageAdapter.Instance.Update(receivedMessage as WeChatLinkIncomeMessage);
                        break;
                }

                TextMessage responseMessage = new TextMessage();

                responseMessage.FromUserName = receivedMessage.ToOpenID;
                responseMessage.ToUserName = receivedMessage.FromOpenID;
                responseMessage.MsgType = receivedMessage.MessageType.ToString();
                responseMessage.CreateTime = DateTime.Now;
                responseMessage.Content = string.Format("I received from {0} at {1}", receivedMessage.FromOpenID, receivedMessage.SentTime.ToLocalTime());

                result = responseMessage.ToString();
            }

            return result;
        }

        [ControllerMethod(true)]
        protected string DefaultEntry()
        {
            return "WeChat Connect In Service";
        }

        private static string GetRequestParams()
        {
            return HttpContext.Current.Request.QueryString.ToUrlParameters(false);
        }

        private static void VerifySignature(string signature, string timestamp, string nonce)
        {
            string[] parts = new string[] { "mayong", timestamp, nonce };

            Array.Sort(parts);

            string source = string.Join("", parts);

            SHA1 sha = new SHA1CryptoServiceProvider();

            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(source));

            string hashString = hash.ToBase16String();

            if (hashString != signature)
                throw new ApplicationException("Signature verify error!");
        }

        private static void WriteEventLog(string message)
        {
            try
            {
                EventLog.WriteEntry("WeChatConnectIn", message);
            }
            catch (System.Exception)
            {
            }
        }

        private static string GetPostedData()
        {
            using (StreamReader reader = new StreamReader(HttpContext.Current.Request.InputStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}