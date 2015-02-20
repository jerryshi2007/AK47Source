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
    public class WeChatImageIncomeMessage : WeChatIncomeMessageBase
    {
        public override void FromXmlElement(XmlElement root)
        {
            base.FromXmlElement(root);

            this.Content = root.GetSingleNodeText("MediaId") + "," + root.GetSingleNodeText("PicUrl");
        }

        public override void ToXmlElement(XmlElement root)
        {
            base.ToXmlElement(root);

            var strs = this.Content.Split(',');
            if (strs.Length == 2)
            {
                root.AppendCDataNode("MediaId", strs[0]);
                root.AppendCDataNode("PicUrl", strs[1]);
            }
        }
    }

    public class WeChatImageIncomeMessageCollection : EditableDataObjectCollectionBase<WeChatImageIncomeMessage>
    {

    }
}
