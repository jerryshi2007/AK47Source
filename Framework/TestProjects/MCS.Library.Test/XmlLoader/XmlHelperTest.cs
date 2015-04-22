using MCS.Library.Core;
using MCS.Library.Test.ResourceLoader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MCS.Library.Test.XmlLoader
{
    [TestClass]
    public class XmlHelperTest
    {
        [TestMethod]
        public void LoadXmlDocumentAsyncTest()
        {
            string content = PrepareTestFile();

            XmlDocument xmlAsync = XmlHelper.LoadDocumentAsync(ResourceHelperTest.ResourcePath).Result;

            Console.WriteLine(xmlAsync.OuterXml);

            XmlDocument xmlSync = XmlHelper.CreateDomDocument(content);

            Assert.AreEqual(xmlSync.OuterXml, xmlAsync.OuterXml);
        }

        [TestMethod]
        public void LoadXElementAsyncTest()
        {
            string content = PrepareTestFile();

            XElement xmlSync = XmlHelper.LoadElement(ResourceHelperTest.ResourcePath);
            XElement xmlAsync = XmlHelper.LoadElementAsync(ResourceHelperTest.ResourcePath).Result;

            Console.WriteLine(xmlAsync.ToString());

            Assert.AreEqual(xmlSync.ToString(), xmlAsync.ToString());
        }

        private static string PrepareTestFile()
        {
            string content = Assembly.GetExecutingAssembly().LoadStringFromResource(ResourceHelperTest.ResourcePath);

            File.WriteAllText(ResourceHelperTest.ResourcePath, content);

            return content;
        }
    }
}
