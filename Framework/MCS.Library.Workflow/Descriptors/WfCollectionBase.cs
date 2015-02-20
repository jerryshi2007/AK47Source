using System;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MCS.Library.Core;

namespace MCS.Library.Workflow.Descriptors
{
	/// <summary>
	/// 集合类的基类
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable]
	[ComVisible(true)]
	[DebuggerDisplay("Count = {Count}")]
	public abstract class WfCollectionBase<T> : CollectionBase
	{
		[NonSerialized]
		private ReaderWriterLock rwLock = null;

		[NonSerialized]
		protected static readonly TimeSpan lockTimeout = TimeSpan.FromSeconds(100);

		/// <summary>
		/// 迭代处理每一个元素
		/// </summary>
		/// <param name="action"></param>
		public virtual void ForEach(Action<T> action)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(action != null, "action");

			this.RWLock.AcquireReaderLock(lockTimeout);

			try
			{
				foreach (T item in List)
					action(item);
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// 判断集合中是否存在某元素
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public virtual bool Exists(Predicate<T> match)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(match != null, "match");

			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				bool result = false;

				foreach (T item in List)
				{
					if (match(item))
					{
						result = true;
						break;
					}
				}

				return result;
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// 判断集合中每个元素是否都满足某条件
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public virtual bool TrueForAll(Predicate<T> match)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(match != null, "match");

			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				bool result = true;

				foreach (T item in List)
				{
					if (match(item) == false)
					{
						result = false;
						break;
					}
				}

				return result;
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// 在集合中查找满足匹配条件的第一个元素
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public virtual T Find(Predicate<T> match)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(match != null, "match");

			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				T result = default(T);

				foreach (T item in List)
				{
					if (match(item))
					{
						result = item;
						break;
					}
				}

				return result;
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// 从后向前查找，找到第一个匹配的元素
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public virtual T FindLast(Predicate<T> match)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(match != null, "match");

			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				T result = default(T);

				for (int i = this.Count - 1; i >= 0; i--)
				{
					if (match((T)List[i]))
					{
						result = (T)List[i];
						break;
					}
				}

				return result;
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// 找到满足匹配条件的所有元素
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public virtual IList<T> FindAll(Predicate<T> match)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(match != null, "match");

			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				IList<T> result = new List<T>();

				foreach (T item in List)
				{
					if (match(item))
						result.Add(item);
				}

				return result;
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// 是否包含某个元素
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public virtual bool Contains(T item)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(item != null, "item");

			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				return List.Contains(item);
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// 得到某个元素的位置
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public virtual int IndexOf(T item)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(item != null, "item");

			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				return List.IndexOf(item);
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// 复制到别的集合中
		/// </summary>
		/// <param name="collection"></param>
		public virtual void CopyTo(ICollection<T> collection)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(collection != null, "collection");

			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				this.ForEach(delegate(T item) { collection.Add(item); });
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// 转换到数组
		/// </summary>
		/// <returns></returns>
		public virtual T[] ToArray()
		{
			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				T[] result = new T[this.Count];

				for (int i = 0; i < this.Count; i++)
					result[i] = (T)List[i];

				return result;
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
		}

		protected virtual void InnerAdd(T obj)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(obj != null, "obj");

			this.RWLock.AcquireWriterLock(lockTimeout);
			try
			{
				List.Add(obj);
			}
			finally
			{
				this.RWLock.ReleaseWriterLock();
			}
		}

		protected ReaderWriterLock RWLock
		{
			get
			{
				lock (this)
				{
					if (this.rwLock == null)
						this.rwLock = new ReaderWriterLock();

					return this.rwLock;
				}
			}
		}

		protected virtual T InnerGet(int index)
		{
			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				return (T)List[index];
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
		}

		protected virtual void InnerRemove(T obj)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(obj != null, "obj");

			this.RWLock.AcquireWriterLock(lockTimeout);
			try
			{
				List.Remove(obj);
			}
			finally
			{
				this.RWLock.ReleaseWriterLock();
			}
		}

		#region IEnumerable<T> 成员

		public new IEnumerator<T> GetEnumerator()
		{
			this.RWLock.AcquireReaderLock(lockTimeout);
			try
			{
				foreach (T item in List)
					yield return item;
			}
			finally
			{
				this.RWLock.ReleaseReaderLock();
			}
		}

		#endregion
	}
}
