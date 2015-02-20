using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Core
{
	/// <summary>
	/// 比较两个对象是否相等的delegate
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public delegate bool EqualityComparerHandler<T>(T x, T y);

	/// <summary>
	/// 通用对象比较器，用于集合中的Distinct等操作
	/// </summary>
	/// <typeparam name="T">需要比较的数据类型</typeparam>
	public class GeneralEqualityComparer<T> : IEqualityComparer<T>
	{
		private EqualityComparerHandler<T> _Comparer = null;

		/// <summary>
		/// 构造方法。
		/// </summary>
		/// <param name="comparer"></param>
		public GeneralEqualityComparer(EqualityComparerHandler<T> comparer)
		{
			this._Comparer = comparer;
		}

		/// <summary>
		/// 判断x，y是否相等
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool Equals(T x, T y)
		{
			bool result = false;

			if (this._Comparer != null)
				result = this._Comparer(x, y);
			else
				result = object.Equals(x, y);

			return result;
		}

		/// <summary>
		/// 得到对象的Hashcode
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int GetHashCode(T obj)
		{
			return obj.GetHashCode();
		}
	}
}
