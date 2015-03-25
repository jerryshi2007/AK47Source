using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace MCS.Library.Caching
{
	/// <summary>
	/// �ڵ����������л����Cache���еĻ��࣬��Cache�������ڣ������ڵ�ǰ�̣߳�WinForm����һ��Http��Web��)���������
	/// </summary>
	public abstract class ContextCacheQueueBase
	{
	}

	/// <summary>
	/// �ڵ����������л���ķ���Cache���еĻ��࣬��Cache�������ڣ������ڵ�ǰ�̣߳�WinForm����һ��Http��Web��)���������
	/// </summary>
	public abstract class ContextCacheQueueBase<TKey, TValue> : ContextCacheQueueBase
	{
		/// <summary>
		/// Cache�����ʱ��ί�ж���
		/// </summary>
		/// <param name="cache">Cache����</param>
		/// <param name="key">��ֵ</param>
		/// <returns>�µ�Cache��</returns>
		public delegate TValue ContextCacheItemNotExistsAction(ContextCacheQueueBase<TKey, TValue> cache, TKey key);

		private Dictionary<TKey, TValue> innerDictionary = new Dictionary<TKey, TValue>();

		/// <summary>
		/// ��Cach���������һ��
		/// </summary>
		/// <param name="key">��</param>
		/// <param name="data">ֵ</param>
		/// <returns>ֵ</returns>
		public TValue Add(TKey key, TValue data)
		{
			this.innerDictionary[key] = data;
			return data;
		}

		/// <summary>
		/// ���ռ�ֵ��ȡ������Cache��ֵ
		/// </summary>
		/// <param name="key">��</param>
		/// <returns>ֵ</returns>
		public TValue this[TKey key]
		{
			get
			{
				return this.innerDictionary[key];
			}
			set
			{
				this.innerDictionary[key] = value;
			}
		}

		/// <summary>
		/// ͨ��key����ȡCache���value�������Ӧ��cache����ڵĻ�
		/// ��cache���value��Ϊ������������ظ��ͻ��˴��� 
		/// </summary>
		/// <param name="key">��</param>
		/// <param name="data">ֵ</param>
		/// <returns></returns>
		public bool TryGetValue(TKey key, out TValue data)
		{
			return this.innerDictionary.TryGetValue(key, out data);
		}

		/// <summary>
		/// ��Cache�ж�ȡCache���������ڣ������action
		/// </summary>
		/// <param name="key">��ֵ</param>
		/// <param name="action">������ʱ�Ļص�</param>
		/// <returns>Cache���ֵ</returns>
		public TValue GetOrAddNewValue(TKey key, ContextCacheItemNotExistsAction action)
		{
			TValue result = default(TValue);

			if (TryGetValue(key, out result) == false)
				result = action(this, key);

			return result;
		}

		/// <summary>
		/// ���ռ�ֵ��ɾ��һ��Cache��
		/// </summary>
		/// <param name="key">��</param>
		/// <returns>�Ƿ�ɹ�ɾ��</returns>
		public bool Remove(TKey key)
		{
			return this.innerDictionary.Remove(key);
		}

		/// <summary>
		/// Cache�������
		/// </summary>
		public int Count
		{
			get
			{
				return this.innerDictionary.Count;
			}
		}

		/// <summary>
		/// �������Cache��
		/// </summary>
		public void Clear()
		{
			this.innerDictionary.Clear();
		}

		/// <summary>
		/// �Ƿ����ĳ��ֵ
		/// </summary>
		/// <param name="key">��</param>
		/// <returns>�Ƿ����</returns>
		public bool ContainsKey(TKey key)
		{
			return this.innerDictionary.ContainsKey(key);
		}

		#region IEnumerable ��Ա

		/// <summary>
		/// ö��Cache��ÿһ��
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			foreach (KeyValuePair<TKey, TValue> kp in this.innerDictionary)
				yield return kp;
		}

		#endregion
	}
}
