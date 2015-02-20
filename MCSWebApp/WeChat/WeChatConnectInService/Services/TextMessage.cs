using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using MCS.Library.Core;

namespace WeChatConnectInService.Services
{
	[Serializable]
	public class TextMessage
	{
		public TextMessage()
		{
		}

		public TextMessage(XmlDocument xmlDoc)
		{
			this.FromXml(xmlDoc);
		}

		public TextMessage(string xmlString)
		{
			XmlDocument xmlDoc = XmlHelper.CreateDomDocument(xmlString);

			this.FromXml(xmlDoc);
		}

		public long MsgID
		{
			get;
			set;
		}

		public string ToUserName
		{
			get;
			set;
		}

		public string FromUserName
		{
			get;
			set;
		}

		public DateTime CreateTime
		{
			get;
			set;
		}

		public string MsgType
		{
			get;
			set;
		}

		public string Content
		{
			get;
			set;
		}

		public void FromXml(XmlDocument xmlDoc)
		{
			xmlDoc.NullCheck("xmlDoc");

			FromXmlElement(xmlDoc.DocumentElement);
		}

		public void FromXmlElement(XmlElement root)
		{
			this.MsgID = root.GetSingleNodeValue("MsgId", 0L);
			this.ToUserName = root.GetSingleNodeText("ToUserName");
			this.FromUserName = root.GetSingleNodeText("FromUserName");
			
			long time = root.GetSingleNodeValue("CreateTime", 0);
			this.CreateTime = (time * 1000L).JavascriptDateNumberToDateTime();
			
			this.MsgType = root.GetSingleNodeText("MsgType");
			this.Content = root.GetSingleNodeText("Content");
		}

		public XmlDocument ToXml()
		{
			XmlDocument xmlDoc = XmlHelper.CreateDomDocument("<xml/>");

			this.ToXmlElement(xmlDoc.DocumentElement);

			return xmlDoc;
		}

		public void ToXmlElement(XmlElement root)
		{
			root.AppendNotDefaultNode("MsgId", this.MsgID);
			root.AppendCDataNode("ToUserName", this.ToUserName);
			root.AppendCDataNode("FromUserName", this.FromUserName);

			long time = this.CreateTime.ToJavascriptDateNumber() / 1000L;

			root.AppendNode("CreateTime", time);
			root.AppendCDataNode("MsgType", this.MsgType);
			root.AppendCDataNode("Content", this.Content);
		}

		public override string ToString()
		{
			return this.ToXml().OuterXml;
		}
	}
}