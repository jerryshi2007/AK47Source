using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace MCS.Library.HtmlParser
{
    public partial class HtmlDocument : IXPathNavigable
    {
        /// <summary>
        /// Creates a new XPathNavigator object for navigating this HTML document.
        /// </summary>
        /// <returns>An XPathNavigator object. The XPathNavigator is positioned on the root of the document.</returns>
        public XPathNavigator CreateNavigator()
        {
            return new HtmlNodeNavigator(this, _documentnode);
        }
    }
}
