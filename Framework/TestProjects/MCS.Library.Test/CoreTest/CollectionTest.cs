using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Core;

namespace MCS.Library.Test
{
	/// <summary>
	/// Summary description for CollectionTest
	/// </summary>
	[TestClass]
	public class CollectionTest
	{
		private class IntegerComparer : IComparer<int>
		{
			#region IComparer<int> Members

			public int Compare(int x, int y)
			{
				return x - y;
			}

			#endregion
		}

		public CollectionTest()
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
		public void BinarySearch()
		{
			int[] dataArray = { 1, 3, 5, 7, 9, 11 };

			Comparison<int> comparison = (target, item) => target - item;

			Assert.AreEqual(-1, dataArray.BinarySearch(0, comparison));
			Assert.AreEqual(0, dataArray.BinarySearch(1, comparison));
			Assert.AreEqual(1, dataArray.BinarySearch(3, comparison));
			Assert.AreEqual(2, dataArray.BinarySearch(5, comparison));
			Assert.AreEqual(3, dataArray.BinarySearch(7, comparison));
			Assert.AreEqual(4, dataArray.BinarySearch(9, comparison));
			Assert.AreEqual(5, dataArray.BinarySearch(11, comparison));
			Assert.AreEqual(-1, dataArray.BinarySearch(13, comparison));

			dataArray = new int[0];

			Assert.AreEqual(-1, dataArray.BinarySearch(13, comparison));
		}

		[TestMethod]
		public void BinaryNearestSearch()
		{
			int[] dataArray = { 1, 3, 5, 7, 9, 11 };

			Comparison<int> comparison = (target, item) => target - item;

			Assert.AreEqual(0, dataArray.BinaryNearestSearch(0, comparison));
			Assert.AreEqual(0, dataArray.BinaryNearestSearch(1, comparison));
			Assert.AreEqual(1, dataArray.BinaryNearestSearch(2, comparison));
			Assert.AreEqual(1, dataArray.BinaryNearestSearch(3, comparison));
			Assert.AreEqual(4, dataArray.BinaryNearestSearch(9, comparison));
			Assert.AreEqual(5, dataArray.BinaryNearestSearch(10, comparison));
			Assert.AreEqual(5, dataArray.BinaryNearestSearch(11, comparison));
			Assert.AreEqual(6, dataArray.BinaryNearestSearch(12, comparison));

			dataArray = new int[0];

			Assert.AreEqual(0, dataArray.BinaryNearestSearch(13, comparison));
		}

		[TestMethod]
		public void QuickSort()
		{
			int[] array = new int[] { 43, 23, 80, 15, 789, 27, 90, 69, 66, 158, 45, 32, 1, 22, 77, 66, 44 };

			int[] qSortArray = new int[array.Length];
			int[] normalSortArray = new int[array.Length];

			array.CopyTo(qSortArray, 0);
			array.CopyTo(normalSortArray, 0);

			qSortArray.QuickSort();
			Array.Sort(normalSortArray);

			CompareList(qSortArray, normalSortArray);
		}

		[TestMethod]
		public void QuickSortWithComparer()
		{
			int[] array = new int[] { 43, 23, 80, 15, 789, 27, 90, 69, 66, 158, 45, 32, 1, 22, 77, 66, 44 };

			int[] qSortArray = new int[array.Length];
			int[] normalSortArray = new int[array.Length];

			array.CopyTo(qSortArray, 0);
			array.CopyTo(normalSortArray, 0);

			IComparer<int> comparer = new IntegerComparer();

			qSortArray.QuickSort(comparer);
			Array.Sort(normalSortArray, comparer);

			CompareList(qSortArray, normalSortArray);
		}

		[TestMethod]
		public void QuickSortUsingComparison()
		{
			int[] array = new int[] { 43, 23, 80, 15, 789, 27, 90, 69, 66, 158, 45, 32, 1, 22, 77, 66, 44 };

			int[] qSortArray = new int[array.Length];
			int[] normalSortArray = new int[array.Length];

			array.CopyTo(qSortArray, 0);
			array.CopyTo(normalSortArray, 0);

			Comparison<int> comparision = (left, right) => left - right;

			qSortArray.QuickSort(comparision);
			Array.Sort(normalSortArray, comparision);

			CompareList(qSortArray, normalSortArray);
		}

		[TestMethod]
		public void NullArrayQuickSortUsingComparison()
		{
			int[] array = new int[] { };

			int[] qSortArray = new int[array.Length];
			int[] normalSortArray = new int[array.Length];

			array.CopyTo(qSortArray, 0);
			array.CopyTo(normalSortArray, 0);

			Comparison<int> comparision = (left, right) => left - right;

			qSortArray.QuickSort(comparision);
			Array.Sort(normalSortArray, comparision);

			CompareList(qSortArray, normalSortArray);
		}

		[TestMethod]
		public void OneItemArrayQuickSortUsingComparison()
		{
			int[] array = new int[] { 1 };

			int[] qSortArray = new int[array.Length];
			int[] normalSortArray = new int[array.Length];

			array.CopyTo(qSortArray, 0);
			array.CopyTo(normalSortArray, 0);

			Comparison<int> comparision = (left, right) => left - right;

			qSortArray.QuickSort(comparision);
			Array.Sort(normalSortArray, comparision);

			CompareList(qSortArray, normalSortArray);
		}

		[TestMethod]
		public void TwoItemArrayQuickSortUsingComparison()
		{
			int[] array = new int[] { 5, 1 };

			int[] qSortArray = new int[array.Length];
			int[] normalSortArray = new int[array.Length];

			array.CopyTo(qSortArray, 0);
			array.CopyTo(normalSortArray, 0);

			Comparison<int> comparision = (left, right) => left - right;

			qSortArray.QuickSort(comparision);
			Array.Sort(normalSortArray, comparision);

			CompareList(qSortArray, normalSortArray);
		}

		private static void CompareList<T>(IList<T> expected, IList<T> actual)
		{
			Assert.AreEqual(expected.Count, actual.Count);

			for (int i = 0; i < expected.Count; i++)
			{
				Assert.AreEqual(expected[i], actual[i]);
			}
		}
	}
}
