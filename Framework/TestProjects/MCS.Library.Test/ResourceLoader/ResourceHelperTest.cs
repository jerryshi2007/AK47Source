using System.Xml.Linq;
using MCS.Library.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;

namespace MCS.Library.Test.ResourceLoader
{
    [TestClass]
    public class ResourceHelperTest
    {
        internal const string ResourcePath = "MCS.Library.Test.ResourceLoader.Data.xml";

        [TestMethod]
        public void LoadResourceStringAsyncTest()
        {
            string xmlSync = Assembly.GetExecutingAssembly().LoadStringFromResource(ResourcePath);

            string xmlAsync = Assembly.GetExecutingAssembly().LoadStringFromResourceAsync(ResourcePath).Result;

            Assert.AreEqual(xmlSync, xmlAsync);
        }

        [TestMethod]
        public void LoadResourceXmlDocumentAsyncTest()
        {
            XmlDocument xmlDocSync = Assembly.GetExecutingAssembly().LoadXmlFromResource(ResourcePath);

            XmlDocument xmlDocAsync = Assembly.GetExecutingAssembly().LoadXmlFromResourceAsync(ResourcePath).Result;

            Assert.AreEqual(xmlDocSync.DocumentElement.Name, xmlDocAsync.DocumentElement.Name);
        }

        [TestMethod]
        public void LoadResourceXElementAsyncTest()
        {
            XElement xElementSync = Assembly.GetExecutingAssembly().LoadXElementFromResource(ResourcePath);

            XElement xElementAsync = Assembly.GetExecutingAssembly().LoadXElementFromResourceAsync(ResourcePath).Result;

            Assert.AreEqual(xElementSync.Name, xElementAsync.Name);
        }
    }
}
