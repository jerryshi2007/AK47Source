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
    /// 文本类消息
    /// </summary>
    [Serializable]
    public class WeChatTextIncomeMessage : WeChatIncomeMessageBase
    {
        public override void FromXmlElement(XmlElement root)
        {
            base.FromXmlElement(root);

            this.Content = root.GetSingleNodeText("Content");
        }

        public override void ToXmlElement(XmlElement root)
        {
            base.ToXmlElement(root);

            root.AppendCDataNode("Content", this.Content);
        }
    }

    public class WeChatTextIncomeMessageCollection : EditableDataObjectCollectionBase<WeChatTextIncomeMessage>
    {

    }
}
