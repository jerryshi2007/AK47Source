using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Core;
using System.Collections;

namespace MCS.Library.Test
{
	/// <summary>
	/// Summary description for PartitionedDictionaryTest
	/// </summary>
	[TestClass]
	public class PartitionedDictionaryTest
	{
		public PartitionedDictionaryTest()
		{
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
		public void SimpleAddTest()
		{
			PartitionedDictionary<string, string> dict = new PartitionedDictionary<string, string>();

			dict.Add("Hero", "Shen Zheng");

			OutputDictionaryInfo(dict);

			Assert.AreEqual("Shen Zheng", dict["Hero"]);
		}

		[TestMethod]
		public void SimpleRemoveTest()
		{
			PartitionedDictionary<string, string> dict = new PartitionedDictionary<string, string>();

			dict.Add("Hero", "Shen Zheng");
			dict.Remove("Hero");
			dict.Remove("Fake");	//这句是空删除

			OutputDictionaryInfo(dict);

			Assert.IsNull(dict["Hero"]);
		}

		[TestMethod]
		public void MoreItemsAddTest()
		{
			PartitionedDictionary<string, string> dict = new PartitionedDictionary<string, string>(20);

			for (int i = 0; i < 1000; i++)
			{
				string key = Guid.NewGuid().ToString();
				dict.Add(key, key);
			}

			OutputDictionaryInfo(dict);

			Assert.AreEqual(1000, dict.Count);
		}

		[TestMethod]
		public void ClearItemsTest()
		{
			PartitionedDictionary<string, string> dict = new PartitionedDictionary<string, string>(20);

			for (int i = 0; i < 1000; i++)
			{
				string key = Guid.NewGuid().ToString();
				dict.Add(key, key);
			}

			dict.Clear();

			OutputDictionaryInfo(dict);

			Assert.AreEqual(0, dict.Count);
		}

		[TestMethod]
		public void AllKeysTest()
		{
			PartitionedDictionary<string, string> dict = new PartitionedDictionary<string, string>();

			for (int i = 0; i < 20; i++)
			{
				string key = Guid.NewGuid().ToString();
				dict.Add(key, key);
			}

			OutputDictionaryInfo(dict);

			dict.Keys.ForEach(key => Console.WriteLine(key));

			Assert.AreEqual(20, dict.Keys.Count);
		}

		[TestMethod]
		public void TryGetValueTest()
		{
			PartitionedDictionary<string, string> dict = new PartitionedDictionary<string, string>();

			dict.Add("Hero", "Shen Zheng");

			OutputDictionaryInfo(dict);

			string result;

			Assert.IsTrue(dict.TryGetValue("Hero", out result));
			Assert.AreEqual("Shen Zheng", result);

			Assert.IsFalse(dict.TryGetValue("HeroNull", out result));
			Assert.IsNull(result);
		}

		private static void OutputDictionaryInfo(PartitionedDictionary<string, string> dict)
		{
			Console.WriteLine(dict.GetAllPartitionsInfo());

			Console.WriteLine("Total Count={0}", dict.Count);

			foreach (KeyValuePair<string, string> kp in dict)
			{
				Console.WriteLine("Key={0} Value={1}", kp.Key, kp.Value);
			}
		}
	}
}
