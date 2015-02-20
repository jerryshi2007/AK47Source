using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Test.LruTest
{
	[TestClass]
	public class LruDataObjectCollectionTest
	{
		private const string Category = "LruDataObjectCollectionTest";

		[TestMethod]
		[TestCategory(Category)]
		public void AddDataTest()
		{
			TestLruCollection list = new TestLruCollection(4);

			for (int i = 0; i < 5; i++)
				list.Add("S" + i);

			Assert.AreEqual("S4", list[0]);
			Assert.AreEqual("S1", list[3]);
		}

		[TestMethod]
		[TestCategory(Category)]
		public void AdvanceDataTest()
		{
			TestLruCollection list = new TestLruCollection(4);

			for (int i = 0; i < 4; i++)
				list.Add("S" + i);

			list.Advance(3);

			Assert.AreEqual("S0", list[0]);
			Assert.AreEqual("S1", list[3]);
		}

		[TestMethod]
		[TestCategory(Category)]
		public void AdvanceAndExceedMaxLengthTest()
		{
			TestLruCollection list = new TestLruCollection(4);

			for (int i = 0; i < 5; i++)
				list.Add("S" + i);

			list.Advance(3);

			Assert.AreEqual("S1", list[0]);
			Assert.AreEqual("S2", list[3]);
		}

		[TestMethod]
		[TestCategory(Category)]
		public void LruCollectionSerializationText()
		{
			TestLruCollection list = new TestLruCollection(4);

			for (int i = 0; i < 5; i++)
				list.Add("S" + i);

			XElementFormatter formatter = new XElementFormatter();

			XElement root = formatter.Serialize(list);

			Console.WriteLine(root.ToString());

			TestLruCollection deserializedList = (TestLruCollection)formatter.Deserialize(root);

			Assert.AreEqual(list.Count, deserializedList.Count);

			for (int i = 0; i < list.Count; i++)
				Assert.AreEqual(list[i], deserializedList[i]);
		}
	}
}
