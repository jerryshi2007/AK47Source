using MCS.Library.Data.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace MCS.Library.SOA.DataObjects.Test
{
    public class MockEditableKeyedDataObjectCollection<T> : EditableKeyedDataObjectCollectionBase<string, T>
        where T : class
    {
        protected override string GetKeyForItem(T item)
        {
            return item.GetHashCode().ToString();
        }
    }

    [TestClass]
    public class EditableKeyedDataObjectCollectionBaseTest
    {
        private TestContext testContextInstance;

        private MockEditableKeyedDataObjectCollection<string> testCollection;

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

        [TestInitialize]
        public void SetUp()
        {
            testCollection = new MockEditableKeyedDataObjectCollection<string>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
		[TestCategory(ProcessTestHelper.Data)]
        public void AddAndRemoveTest()
        {
            string data1 = "data1";
            testCollection.Add(data1);

            Assert.AreEqual(data1, testCollection[data1.GetHashCode().ToString()]);
            Assert.IsTrue(testCollection.Count == 1);

            string data2 = "";
            testCollection.Add(data2);
            Assert.IsTrue(testCollection.Contains(data2));

            testCollection.Remove(p => string.IsNullOrEmpty(p));

            Assert.IsFalse(testCollection.Contains(data2));

            testCollection.Add(data1);  //exception occurs
        }

        [TestMethod]
		[TestCategory(ProcessTestHelper.Data)]
        public void CopyTest()
        {
            testCollection.Add("1");
            testCollection.Add("2");
            testCollection.Add("3");
            testCollection.Add("4");
            testCollection.Add("5");

            List<string> list = new List<string>();
            testCollection.CopyTo(list);
            Assert.AreEqual(testCollection.Count, list.Count);

            Dictionary<string, string> dict = new Dictionary<string, string>();
            testCollection.CopyTo(dict);
            Assert.AreEqual(testCollection.Count, dict.Count);

            foreach (var keyvalue in dict)
            {
                Assert.AreEqual(testCollection[keyvalue.Key], keyvalue.Value);
            }

            dict.Add("6".GetHashCode().ToString(), "6");
            dict.Add("7".GetHashCode().ToString(), "7");

            testCollection.CopyFrom(dict);

            Assert.AreEqual(dict.Count, testCollection.Count);

            foreach (var keyvalue in dict)
            {
                Assert.AreEqual(testCollection[keyvalue.Key], keyvalue.Value);
            }
            Console.WriteLine(testCollection.Count);
        }
    }
}
