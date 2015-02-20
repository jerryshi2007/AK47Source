using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Core;

namespace MCS.Library.Data.DataObjects
{
	/// <summary>
	/// 符合LRU算法的List，非线程安全
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable]
	[XElementSerializable]
	public class LruDataObjectCollectionBase<T> : DataObjectCollectionBase<T>
	{
		private int _MaxLength = 100;

		/// <summary>
		/// 构造方法
		/// </summary>
		public LruDataObjectCollectionBase()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="maxLength"></param>
		public LruDataObjectCollectionBase(int maxLength)
		{
			this._MaxLength = maxLength;
		}

		/// <summary>
		/// 队列的最大长度
		/// </summary>
		public int MaxLength
		{
			get
			{
				return this._MaxLength;
			}
		}

		/// <summary>
		/// 将对象添加到List中的第一项，如果集合超过了MaxLength，则删除最后一项
		/// </summary>
		/// <param name="obj"></param>
		public virtual void Add(T obj)
		{
			if (List.Count == this._MaxLength)
				List.RemoveAt(List.Count - 1);

			List.Insert(0, obj);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public virtual T this[int index]
		{
			get
			{
				return (T)List[index];
			}
		}

		/// <summary>
		/// 将某个下标的对象提取到第一项
		/// </summary>
		/// <param name="index"></param>
		public virtual void Advance(int index)
		{
			if (index >= 0 && index < List.Count)
			{
				T obj = (T)List[index];

				List.RemoveAt(index);

				List.Insert(0, obj);
			}
		}
	}
}
