using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MCS.Library.HtmlParser
{
    /// <summary>
    /// Html净化器
    /// 
    /// </summary>
    public static class HtmlSanitizer
    {
        private static readonly List<HtmlSanitizeNode> _JavascriptRelAttributes = new List<HtmlSanitizeNode>() {
            new HtmlSanitizeNode("A", "href"),
            new HtmlSanitizeNode("area", "href"),
            new HtmlSanitizeNode("iframe", "src"),
            new HtmlSanitizeNode("frame", "src"),
            new HtmlSanitizeNode("img", "src"),
            new HtmlSanitizeNode("embed", "src"),
            new HtmlSanitizeNode("form", "action"),
            new HtmlSanitizeNode("xml", "src"),
            new HtmlSanitizeNode("input", "src"),
            new HtmlSanitizeNode("meta", "url"),
            new HtmlSanitizeNode("audio", "src"),
            new HtmlSanitizeNode("video", "src"),
            new HtmlSanitizeNode("link", "href")
        };

        private static readonly SortedSet<string> _DefaultNodeBlackList = new SortedSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "script"
        };

        private static readonly SortedSet<string> _DefaultAttributeBlackList = new SortedSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "onabort", "onbeforeunload", "onactivate", "onbeforeactivate", "onbeforedeactivate", "ondeactivate",
            "onerror", "onload", "onselect",
            "onclose",
            "ondevicemotion", "ondeviceorientation",
            "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop",
            "onafterprint", "onbeforeprint", "onbounce", "oncandidatewindowhide", "oncandidatewindowshow", "oncandidatewindowupdate",
            "onchange", "oncompassneedscalibration", "onfinish", "onhashchange", "oninput", "onoffline", "ononline", "orientationchange",
            "onreadystatechange", "onreset", "onstart", "onsubmit",
            "onblur", "onfocus", "onfocusin", "onfocusout",
            "onkeydown", "onkeypress",  "onkeyup",
            "ontouchcancel", "ontouchend", "ontouchmove", "ontouchstart",
            "onmessage",
            "onclick", "oncontextmenu", "ondblclick", "onmousedown", "onmousemove", "onmouseout", "onmouseover",
            "onmouseup", "onmousewheel", "onmouseenter", "onmouseleave", 
            "onmssitemodejumplistitemremoved", "onmsthumbnailclick",
            "onpagehide", "onpageshow",
            "ongotpointercapture", "onlostpointercapture", "onpointercancel", "onpointerdown", "onpointerenter",
            "onmspointerhover", "onpointerleave", "onpointermove", "onpointerout", "onpointerover", "onpointerup",
            "onpopstate",
            "onstorage",
            "onbeforecopy", "onbeforecut", "onbeforepaste", "oncopy", "oncut",
            "onresize", "onresizestart", "onresizeend", "onscroll",
            "onrowenter", "onrowexit", "onrowsdelete", "onrowsinserted",
            "oncellchange", "ondataavailable", "ondatasetchanged", "onerrorupdate",
            "onfilterchange", "onfilterchange", "onhelp", "onlayoutcomplete", "onlosecapture",
            "onmove", "onmoveend", "onmovestart",
            "ondatasetcomplete", "onpropertychange"
        };

        /// <summary>
        /// 得到安全的Html
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string GetSafeHtml(string html)
        {
            HtmlDocument doc = new HtmlDocument();

            doc.LoadHtml(html);

            List<HtmlNode> nodesToRemove = new List<HtmlNode>();
            IEnumerator e = doc
                .CreateNavigator()
                .SelectDescendants(System.Xml.XPath.XPathNodeType.All, false)
                .GetEnumerator();

            while (e.MoveNext())
            {
                HtmlNode node =
                    ((HtmlNodeNavigator)e.Current)
                    .CurrentNode;

                if (_DefaultNodeBlackList.Contains(node.Name))
                    nodesToRemove.Add(node);
                else
                {
                    FilterAttributes(node);
                    FilterScriptRelAttributes(node);
                }
            }

            nodesToRemove.ForEach(node => node.ParentNode.RemoveChild(node));

            StringBuilder strB = new StringBuilder();

            using (StringWriter writer = new StringWriter(strB))
                doc.Save(writer);

            return strB.ToString();
        }

        private static void FilterAttributes(HtmlNode node)
        {
            List<HtmlAttribute> attrsNeedToRemove = new List<HtmlAttribute>();

            foreach (HtmlAttribute attr in node.Attributes)
            {
                if (_DefaultAttributeBlackList.Contains(attr.Name))
                    attrsNeedToRemove.Add(attr);
            }

            attrsNeedToRemove.ForEach(attr => node.Attributes.Remove(attr));
        }

        /// <summary>
        /// 过滤有可能包含脚本的属性，如果包含javascript，则过滤掉此属性
        /// </summary>
        /// <param name="node"></param>
        private static void FilterScriptRelAttributes(HtmlNode node)
        {
            List<HtmlAttribute> attrsNeedToRemove = new List<HtmlAttribute>();

            foreach (HtmlSanitizeNode hsn in _JavascriptRelAttributes)
            {
                hsn.MatchAndAction(node, (n, attr) =>
                {
                    if (ContainsScript(attr))
                        attrsNeedToRemove.Add(attr);
                });
            }

            attrsNeedToRemove.ForEach(attr => node.Attributes.Remove(attr));
        }

        private static bool ContainsScript(HtmlAttribute attr)
        {
            bool result = false;

            if (attr != null)
            {
                string attrValue = attr.Value;

                result = ContainsJavaScript(attrValue);

                if (result == false)
                {
                    attrValue = HttpUtility.UrlDecode(attrValue);
                    result = ContainsJavaScript(attrValue);
                }
            }

            return result;
        }

        private static bool ContainsJavaScript(string attrValue)
        {
            attrValue = attrValue.Replace(" ", string.Empty);

            return attrValue.IndexOf("javascript:", 0, StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}
