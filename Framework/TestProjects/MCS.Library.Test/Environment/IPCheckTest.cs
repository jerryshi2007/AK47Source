using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using MCS.Library.Core;

namespace MCS.Library.Test.Environment
{
    [TestClass]
    public class IPCheckTest
    {
        [TestMethod]
        public void IPv4ParseTest()
        {
            IPAddress ip = "192.168.0.1, 192.168.0.2".GetFirstIPAddress();

            Assert.AreEqual("192.168.0.1", ip.ToString());
        }

        [TestMethod]
        public void OneIPv4ParseTest()
        {
            IPAddress ip = "192.168.0.1".GetFirstIPAddress();

            Assert.AreEqual("192.168.0.1", ip.ToString());
        }

        [TestMethod]
        public void EmptyIPParseTest()
        {
            IPAddress ip = " ".GetFirstIPAddress();

            Assert.IsNull(ip);
        }

        [TestMethod]
        public void IPv6ParseTest()
        {
            IPAddress ip = "fe80::a5ce:30e1:1e08:1038%28, 192.168.0.2".GetFirstIPAddress();

            Assert.AreEqual("fe80::a5ce:30e1:1e08:1038%28", ip.ToString());
        }
    }
}
