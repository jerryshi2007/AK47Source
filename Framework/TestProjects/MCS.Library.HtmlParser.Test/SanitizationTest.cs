using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Core;
using System.Reflection;
using System.Collections.Generic;
using System.Web;

namespace MCS.Library.HtmlParser.Test
{
    [TestClass]
    public class SanitizationTest
    {
        [TestMethod]
        public void RemoveScriptTagTest()
        {
            string content = LoadHtmlFromResource("basicWithScript.html");

            string safeHtml = HtmlSanitizer.GetSafeHtml(content);

            Console.WriteLine(safeHtml);
        }

        [TestMethod]
        public void RemoveEventAttributesTest()
        {
            string content = LoadHtmlFromResource("basicWithEvents.html");

            string safeHtml = HtmlSanitizer.GetSafeHtml(content);

            Console.WriteLine(safeHtml);
        }

        [TestMethod]
        public void RemoveJavaScriptAttributesTest()
        {
            string content = LoadHtmlFromResource("basicAnchorScript.html");

            string safeHtml = HtmlSanitizer.GetSafeHtml(content);

            Console.WriteLine(safeHtml);
        }

        private static string LoadHtmlFromResource(string resName)
        {
            string path = string.Format("MCS.Library.HtmlParser.Test.Resources.{0}", resName);

            return ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), path);
        }
    }
}
