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
    /// �������а��ռ�ֵ���������ļ�����Ļ���
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
        /// ����key�����ֵ�Ԫ��
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>�ֵ��е�Ԫ��</returns>
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
        /// �ֵ����Ƿ����ĳ����ֵ
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>�Ƿ����ĳ����ֵ</returns>
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
        /// ���Ԫ��
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">����</param>
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
        /// ɾ���ֵ���
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
        /// �õ������е�ֵ�������ֵ�����ڣ��ṩȱʡֵ
        /// </summary>
        /// <typeparam name="T">ȱʡֵ����������</typeparam>
        /// <param name="key">��ֵ</param>
        /// <param name="defaultValue">ȱʡֵ</param>
        /// <returns>�����е�ֵ��ȱʡֵ</returns>
        public T GetValue<T>(TKey key, T defaultValue)
        {
            T result = defaultValue;

			object data = this[key];

			if (data != null)
				result = (T)DataConverter.ChangeType<object>(data, typeof(T));

			return result;
        }

		/// <summary>
		/// �ֵ���ĸ���
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
		/// ��д��
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

		#region ��Dictionary֮��Ľ���
		/// <summary>
		/// Copy��Dictionary��
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
		/// ��Dictionary�и���
		/// </summary>
		/// <param name="dict"></param>
		public void CopyFrom(IDictionary<TKey, object> dict)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(dict != null, "dict");

			AcquireWriterLock();
			try
			{
				this._Dictionary.Clear();

				//foreach (KeyValuePair<TKey, object> kp in this)//����ط����ˣ�Ӧ���Ǳ���Ŀ���ֵ䣬����������
				foreach (KeyValuePair<TKey, object> kp in dict)
					this.Add(kp.Key, kp.Value);
			}
			finally
			{
				ReleaseWriterLock();
			}
		}
		#endregion

		#region IEnumerable ��Ա

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
