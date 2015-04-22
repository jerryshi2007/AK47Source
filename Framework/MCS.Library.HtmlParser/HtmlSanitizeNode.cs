using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.HtmlParser
{
    /// <summary>
    /// 需要
    /// </summary>
    [Serializable]
    public class HtmlSanitizeNode
    {
        private IEnumerable<string> _AttributesName = null;

        /// <summary>
        /// 构造方法
        /// </summary>
        public HtmlSanitizeNode(string tagName, params string[] attrsName)
        {
            this.TagName = tagName;

            if (attrsName != null)
                this._AttributesName = attrsName;
        }

        /// <summary>
        /// 
        /// </summary>
        public string TagName
        {
            get;
            set;
        }

        /// <summary>
        /// 标签的名称
        /// </summary>
        public IEnumerable<string> AttributesName
        {
            get
            {
                if (this._AttributesName == null)
                    this._AttributesName = StringExtension.EmptyStringArray;

                return this._AttributesName;
            }
        }

        /// <summary>
        /// 是否匹配，匹配到了则执行操作
        /// </summary>
        /// <param name="node"></param>
        /// <param name="action"></param>
        public void MatchAndAction(HtmlNode node, Action<HtmlNode, HtmlAttribute> action)
        {
            if (node != null && action != null)
            {
                if (string.Compare(node.Name, this.TagName, true) == 0)
                {
                    foreach (string attrName in this.AttributesName)
                    {
                        HtmlAttribute attr = node.Attributes[attrName];

                        if (attr != null)
                            action(node, attr);
                    }
                }
            }
        }
    }
}
