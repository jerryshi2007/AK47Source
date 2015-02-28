using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Script.Serialization;

namespace MCS.Web.Library.Script.Json.Test
{
    /// <summary>
    /// Summary description for VoucherJsonTest
    /// </summary>
    [TestClass]
    public class VoucherJsonTest
    {
        public VoucherJsonTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void VoucherObjectJsonTest()
        {
            JSONSerializerExecute.RegisterConverter(typeof(VoucherConverter));
            JSONSerializerExecute.RegisterConverter(typeof(VoucherItemCollectionConverter));

            VoucherEntity source = VoucherEntity.PrepareData();

            string json = JSONSerializerExecute.SerializeWithType(source);

            Console.WriteLine(json);

            VoucherEntity deserialized = JSONSerializerExecute.Deserialize<VoucherEntity>(json);

            AssertVoucherEntity(source, deserialized);
        }

        [TestMethod]
        public void VoucherItemCollectionWithConverterJsonTest()
        {
            JSONSerializerExecute.RegisterConverter(typeof(VoucherConverter));
            JSONSerializerExecute.RegisterConverter(typeof(VoucherItemCollectionConverter));

            VoucherEntity source = VoucherEntity.PrepareData();

            string json = JSONSerializerExecute.SerializeWithType(source.Items);

            Console.WriteLine(json);

            JavaScriptSerializer serializer = JSONSerializerFactory.GetJavaScriptSerializer(typeof(VoucherItemCollection));

            VoucherItemCollection deserialized = JSONSerializerExecute.DeserializeString<VoucherItemCollection>(json);

            AssertVoucherItemCollection(source.Items, deserialized);
        }

        [TestMethod]
        public void VoucherWithoutItemCollectionConverterJsonTest()
        {
            VoucherEntity source = VoucherEntity.PrepareData();
            JSONSerializerExecute.RegisterConverter(typeof(VoucherConverter));

            string json = JSONSerializerExecute.Serialize(source);

            Console.WriteLine(json);

            VoucherEntity deserialized = JSONSerializerExecute.DeserializeString<VoucherEntity>(json);

            //不校验CollectionName
            AssertVoucherEntity(source, deserialized, false);
        }

        private static void AssertVoucherEntity(VoucherEntity source, VoucherEntity dest, bool withCollectionName = true)
        {
            Assert.AreEqual(source.Name, dest.Name);

            AssertVoucherItemCollection(source.Items, dest.Items, withCollectionName);
        }

        private static void AssertVoucherItemCollection(VoucherItemCollection source, VoucherItemCollection dest, bool withCollectionName = true)
        {
            Assert.AreEqual(source.Count, dest.Count);

            if (withCollectionName)
                Assert.AreEqual(source.CollectioName, dest.CollectioName);

            for (int i = 0; i < source.Count; i++)
                AssertVoucherItem(source[i], dest[i]);
        }

        private static void AssertVoucherItem(VoucherItem source, VoucherItem dest)
        {
            Assert.AreEqual(source.VoucherCode, dest.VoucherCode);
            Assert.AreEqual(source.Code, dest.Code);
            Assert.AreEqual(source.CreateTime, dest.CreateTime);
        }
    }
}
