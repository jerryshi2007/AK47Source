using System;
using System.Text;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using MCS.Library.Core;

namespace MCS.Library.Workflow.Descriptors
{
    /// <summary>
    /// 工作流中按照键值进行索引的集合类的基类
    /// </summary>
    [Serializable]
	[DebuggerDisplay("Count = {Count}")]
    public abstract class WfContextDictionaryBase<TKey>: IEnumerable
    {
        private Hashtable _Dictionary = new Hashtable(StringComparer.OrdinalIgnoreCase);

		[NonSerialized]
		private ReaderWriterLock rwLock = null;

		[NonSerialized]
		private static readonly TimeSpan lockTimeout = TimeSpan.FromSeconds(100);

        /// <summary>
        /// 按照key访问字典元素
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>字典中的元素</returns>
        public object this[TKey key]
        {
            get
            {
				AcquireReaderLock();
				try
				{
					return this._Dictionary[key];
				}
				finally
				{
					ReleaseReaderLock();
				}
            }
            set
            {
                Validate(key, value);

				AcquireWriterLock();
				try
				{
					this._Dictionary[key] = value;
				}
				finally
				{
					ReleaseWriterLock();
				}
            }
        }

        /// <summary>
        /// 字典中是否包含某个键值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>是否包含某个键值</returns>
        public bool Contains(TKey key)
        {
			AcquireReaderLock();
			try
			{
				return this._Dictionary.ContainsKey(key);
			}
			finally
			{
				ReleaseReaderLock();
			}
        }

        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">数据</param>
        public void Add(TKey key, object data)
        {
            Validate(key, data);

			AcquireWriterLock();
			try
			{
				this._Dictionary.Add(key, data);
			}
			finally
			{
				ReleaseWriterLock();
			}
        }

        /// <summary>
        /// 删除字典项
        /// </summary>
        /// <param name="key"></param>
        public void Remove(TKey key)
        {
			AcquireWriterLock();
			try
			{
				this._Dictionary.Remove(key);
			}
			finally
			{
				ReleaseWriterLock();
			}
        }

        /// <summary>
        /// 得到集合中的值，如果该值不存在，提供缺省值
        /// </summary>
        /// <typeparam name="T">缺省值的数据类型</typeparam>
        /// <param name="key">键值</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>集合中的值或缺省值</returns>
        public T GetValue<T>(TKey key, T defaultValue)
        {
            T result = defaultValue;

			object data = this[key];

			if (data != null)
				result = (T)DataConverter.ChangeType<object>(data, typeof(T));

			return result;
        }

		/// <summary>
		/// 字典项的个数
		/// </summary>
		public int Count
		{
			get
			{
				AcquireReaderLock();
				try
				{
					return _Dictionary.Count;
				}
				finally
				{
					ReleaseReaderLock();
				}
			}
		}

		protected void AcquireReaderLock()
		{
			RWLock.AcquireReaderLock(lockTimeout);
		}

		protected void AcquireWriterLock()
		{
			RWLock.AcquireWriterLock(lockTimeout);
		}

		protected void ReleaseReaderLock()
		{
			RWLock.ReleaseReaderLock();
		}

		protected void ReleaseWriterLock()
		{
			RWLock.ReleaseWriterLock();
		}

		/// <summary>
		/// 读写锁
		/// </summary>
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

        private void Validate(TKey key, object value)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(key != null, "key");
            ExceptionHelper.FalseThrow<ArgumentNullException>(value != null, "value");
		}

		#region 和Dictionary之间的交互
		/// <summary>
		/// Copy到Dictionary中
		/// </summary>
		/// <param name="dict"></param>
		public void CopyTo(IDictionary<TKey, object> dict)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(dict != null, "dict");

			dict.Clear();

			AcquireReaderLock();
			try
			{
				foreach (KeyValuePair<TKey, object> kp in this)
					dict.Add(kp.Key, kp.Value);
			}
			finally
			{
				ReleaseReaderLock();
			}
		}

		/// <summary>
		/// 从Dictionary中复制
		/// </summary>
		/// <param name="dict"></param>
		public void CopyFrom(IDictionary<TKey, object> dict)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(dict != null, "dict");

			AcquireWriterLock();
			try
			{
				this._Dictionary.Clear();

				//foreach (KeyValuePair<TKey, object> kp in this)//这个地方错了，应该是遍历目标字典，而不是自身。
				foreach (KeyValuePair<TKey, object> kp in dict)
					this.Add(kp.Key, kp.Value);
			}
			finally
			{
				ReleaseWriterLock();
			}
		}
		#endregion

		#region IEnumerable 成员

		public IEnumerator GetEnumerator()
		{
			AcquireReaderLock();
			try
			{
				foreach (DictionaryEntry entry in _Dictionary)
					yield return new KeyValuePair<TKey, object>((TKey)entry.Key, entry.Value);
			}
			finally
			{
				ReleaseReaderLock();
			}
		}

		#endregion
	}
}
