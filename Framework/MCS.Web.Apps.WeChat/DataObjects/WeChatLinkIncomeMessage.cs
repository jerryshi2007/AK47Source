using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Web.Apps.WeChat.DataObjects
{
	/// <summary>
	/// 图片类消息
	/// </summary>
	[Serializable]
	public class WeChatLinkIncomeMessage : WeChatIncomeMessageBase
	{
		public override void FromXmlElement(XmlElement root)
		{
			base.FromXmlElement(root);

            this.Content = root.GetSingleNodeText("Url");
		}

		public override void ToXmlElement(XmlElement root)
		{
			base.ToXmlElement(root);

            root.AppendCDataNode("Url", this.Content);
		}
	}

    public class WeChatLinkIncomeMessageCollection : EditableDataObjectCollectionBase<WeChatLinkIncomeMessage>
    {

    }
}
