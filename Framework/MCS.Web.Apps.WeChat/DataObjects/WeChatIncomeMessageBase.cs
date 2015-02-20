using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;

namespace MCS.Web.Apps.WeChat.DataObjects
{
    /// <summary>
    /// 微信公众号（机器人）接收到的原始消息的基类
    /// </summary>
    [Serializable]
    [ORTableMapping("WeChat.IncomeMessages")]
    public abstract class WeChatIncomeMessageBase
    {
        [ORFieldMapping("MessageID", PrimaryKey = true)]
        public long MessageID
        {
            get;
            set;
        }

        /// <summary>
        /// 发送到的OpenID
        /// </summary>
        [ORFieldMapping("ToOpenID")]
        public string ToOpenID
        {
            get;
            set;
        }

        /// <summary>
        /// 从哪个OpenID发送
        /// </summary>
        [ORFieldMapping("FromOpenID")]
        public string FromOpenID
        {
            get;
            set;
        }

        [ORFieldMapping("SentTime")]
        public DateTime SentTime
        {
            get;
            set;
        }

        [ORFieldMapping("MessageType")]
        [SqlBehavior(EnumUsageTypes.UseEnumString)]
        public WeChatMessageType MessageType
        {
            get;
            set;
        }

        /// <summary>
        /// 不是所有的消息类型都有Content，这里的Content起到摘要信息的作用
        /// </summary>
        [ORFieldMapping("Content")]
        public string Content
        {
            get;
            set;
        }

        [ORFieldMapping("RawXml")]
        public string RawXml
        {
            get;
            set;
        }

        public void FromXml(XmlDocument xmlDoc)
        {
            xmlDoc.NullCheck("xmlDoc");

            FromXmlElement(xmlDoc.DocumentElement);
        }

        public virtual void FromXmlElement(XmlElement root)
        {
            this.MessageID = root.GetSingleNodeValue("MsgId", 0L);
            this.ToOpenID = root.GetSingleNodeText("ToUserName");
            this.FromOpenID = root.GetSingleNodeText("FromUserName");

            long time = root.GetSingleNodeValue("CreateTime", 0);
            this.SentTime = (time * 1000L).JavascriptDateNumberToDateTime();

            var msgTypeText = root.GetSingleNodeText("MsgType");
            msgTypeText = msgTypeText[0].ToString().ToUpper() + msgTypeText.Substring(1);
            this.MessageType = (WeChatMessageType)Enum.Parse(typeof(WeChatMessageType), msgTypeText);

            this.RawXml = root.OuterXml;
        }

        public XmlDocument ToXml()
        {
            XmlDocument xmlDoc = XmlHelper.CreateDomDocument("<xml/>");

            this.ToXmlElement(xmlDoc.DocumentElement);

            return xmlDoc;
        }

        public virtual void ToXmlElement(XmlElement root)
        {
            root.AppendNotDefaultNode("MsgId", this.MessageID);
            root.AppendCDataNode("ToUserName", this.ToOpenID);
            root.AppendCDataNode("FromUserName", this.FromOpenID);

            long time = this.SentTime.ToJavascriptDateNumber() / 1000L;

            root.AppendNode("CreateTime", time);
            root.AppendCDataNode("MsgType", this.MessageType.ToString().ToLower());
        }
    }
}
