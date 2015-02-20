using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MCS.Library.Core
{
	/// <summary>
	/// List的扩展方法
	/// </summary>
	public static class IListExtension
	{
		/// <summary>
		/// 在一个有序的List中进行二分查找
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="target">查找目标</param>
		/// <param name="comparison"></param>
		/// <returns></returns>
		public static int BinarySearch<T>(this IList<T> list, T target, Comparison<T> comparison)
		{
			list.NullCheck("list");
			target.NullCheck("target");
			comparison.NullCheck("comparison");

			return InnerBinarySearch(list, target, 0, list.Count - 1, comparison);
		}

		private static int InnerBinarySearch<T>(IList<T> list, T target, int startIndex, int endIndex, Comparison<T> comparison)
		{
			int result = -1;

			if (endIndex >= startIndex)
			{
				int midIndex = (endIndex + startIndex) / 2;

				T data = list[midIndex];

				int compareResult = comparison(target, data);

				if (compareResult == 0)
					result = midIndex;
				else
					if (compareResult < 0)
						result = InnerBinarySearch(list, target, startIndex, midIndex - 1, comparison);
					else
						result = InnerBinarySearch(list, target, midIndex + 1, endIndex, comparison);
			}

			return result;
		}

		/// <summary>
		/// 查找最接近的项，返回最接近的项（小于等于）的下标。如果比第一项小，则返回0，如果比最后一项大，则返回集合的下标+1。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="target"></param>
		/// <param name="comparison"></param>
		/// <returns></returns>
		public static int BinaryNearestSearch<T>(this IList<T> list, T target, Comparison<T> comparison)
		{
			list.NullCheck("list");
			target.NullCheck("target");
			comparison.NullCheck("comparison");

			return InnerBinaryNearestSearch(list, target, 0, list.Count - 1, comparison);
		}

		#region QuickSort<T>
		/// <summary>
		/// 快速排序，使用对象默认的比较规则
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		public static void QuickSort<T>(this IList<T> list) where T : IComparable<T>
		{
			list.NullCheck("list");

			Comparison<T> comparison = (left, right) => left.CompareTo(right);

			InnerQuickSort(list, 0, list.Count - 1, comparison);
		}

		/// <summary>
		/// 按照一定的规则进行快速排序
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="comparison"></param>
		public static void QuickSort<T>(this IList<T> list, Comparison<T> comparison)
		{
			list.NullCheck("list");
			comparison.NullCheck("comparison");

			InnerQuickSort(list, 0, list.Count - 1, comparison);
		}

		/// <summary>
		/// 按照一定的规则进行快速排序
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="comparer"></param>
		public static void QuickSort<T>(this IList<T> list, IComparer<T> comparer)
		{
			list.NullCheck("list");
			comparer.NullCheck("comparer");

			Comparison<T> comparison = (left, right) => comparer.Compare(left, right);

			InnerQuickSort(list, 0, list.Count - 1, comparison);
		}

		private static int InnerBinaryNearestSearch<T>(IList<T> list, T target, int startIndex, int endIndex, Comparison<T> comparison)
		{
			int result = -1;

			if (endIndex >= startIndex)
			{
				int midIndex = (endIndex + startIndex) / 2;

				T data = list[midIndex];

				int compareResult = comparison(target, data);

				if (compareResult == 0)
					result = midIndex;
				else
					if (compareResult < 0)
						result = InnerBinaryNearestSearch(list, target, startIndex, midIndex - 1, comparison);
					else
						result = InnerBinaryNearestSearch(list, target, midIndex + 1, endIndex, comparison);
			}
			else
				result = endIndex + 1;

			return result;
		}

		private static void InnerQuickSort<T>(IList<T> list, int left, int right, Comparison<T> comparison)
		{
			if (left < right)
			{
				int middle = GetMiddleFromQuickSort(list, left, right, comparison);

				InnerQuickSort(list, left, middle - 1, comparison);
				InnerQuickSort(list, middle + 1, right, comparison);
			}
		}

		private static int GetMiddleFromQuickSort<T>(IList<T> list, int left, int right, Comparison<T> comparison)
		{
			T key = list[left];

			while (left < right)
			{
				while (left < right && comparison(key, list[right]) < 0)
					right--;

				if (left < right)
				{
					T temp = list[left];
					list[left] = list[right];
					list[right] = temp;
					left++;
				}

				while (left < right && comparison(key, list[left]) > 0)
					left++;

				if (left < right)
				{
					T temp = list[right];
					list[right] = list[left];
					list[left] = temp;
					right--;
				}

				list[left] = key;
			}

			return left;
		}
		#endregion QuickSort<T>

		#region QuickSortList
		/// <summary>
		/// 快速排序，使用对象默认的比较规则
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		public static void QuickSortList<T>(this IList list) where T : IComparable<T>
		{
			list.NullCheck("list");

			Comparison<T> comparison = (left, right) => left.CompareTo(right);

			InnerQuickSort(list, 0, list.Count - 1, comparison);
		}

		/// <summary>
		/// 按照一定的规则进行快速排序
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="comparison"></param>
		public static void QuickSortList<T>(this IList list, Comparison<T> comparison)
		{
			list.NullCheck("list");
			comparison.NullCheck("comparison");

			InnerQuickSort(list, 0, list.Count - 1, comparison);
		}

		/// <summary>
		/// 按照一定的规则进行快速排序
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="comparer"></param>
		public static void QuickSortList<T>(this IList list, IComparer<T> comparer)
		{
			list.NullCheck("list");
			comparer.NullCheck("comparer");

			Comparison<T> comparison = (left, right) => comparer.Compare(left, right);

			InnerQuickSort(list, 0, list.Count - 1, comparison);
		}

		private static void InnerQuickSort<T>(IList list, int left, int right, Comparison<T> comparison)
		{
			if (left < right)
			{
				int middle = GetMiddleFromQuickSort(list, left, right, comparison);

				InnerQuickSort(list, left, middle - 1, comparison);
				InnerQuickSort(list, middle + 1, right, comparison);
			}
		}

		private static int GetMiddleFromQuickSort<T>(IList list, int left, int right, Comparison<T> comparison)
		{
			T key = (T)list[left];

			while (left < right)
			{
				while (left < right && comparison(key, (T)list[right]) < 0)
					right--;

				if (left < right)
				{
					T temp = (T)list[left];
					list[left] = list[right];
					list[right] = temp;
					left++;
				}

				while (left < right && comparison(key, (T)list[left]) > 0)
					left++;

				if (left < right)
				{
					T temp = (T)list[right];
					list[right] = list[left];
					list[left] = temp;
					right--;
				}

				list[left] = key;
			}

			return left;
		}
		#endregion QuickSortList
	}
}
