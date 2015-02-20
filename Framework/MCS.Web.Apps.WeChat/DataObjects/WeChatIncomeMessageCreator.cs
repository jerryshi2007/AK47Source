using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Web.Apps.WeChat.DataObjects
{
    public static class WeChatIncomeMessageCreator
    {
        public static WeChatIncomeMessageBase Create(XmlDocument xmlDoc)
        {
            WeChatIncomeMessageBase result = null;

            var messageType = xmlDoc.DocumentElement.GetSingleNodeText("MsgType");
            switch (messageType)
            {
                case "text":
                    result = new WeChatTextIncomeMessage();
                    break;
                case "location":
                    result = new WeChatLocationIncomeMessage();
                    break;
                case "image":
                    result = new WeChatImageIncomeMessage();
                    break;
                case "video":
                    result = new WeChatVideoIncomeMessage();
                    break;
                case "voice":
                    result = new WeChatVoiceIncomeMessage();
                    break;
                case "link":
                    result = new WeChatLinkIncomeMessage();
                    break;
                default:
                    throw new Exception("未知 messageType");
            }

            result.FromXml(xmlDoc);
            
            return result;
        }
    }
}
