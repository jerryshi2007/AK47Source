using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MCS.Library.Core
{
	/// <summary>
	/// 带分区的字典，里面会根据Hash Code分散在不同的子Hash表中
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public class PartitionedDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		private const int DefaultPartitions = 10;

		private object syncRoot = new object();
		private Hashtable[] partitions = null;

		/// <summary>
		/// 
		/// </summary>
		public PartitionedDictionary()
			: this(10)
		{

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="count"></param>
		public PartitionedDictionary(int count)
		{
			InitPartitionArray(count);

			for (int i = 0; i < count; i++)
				this.partitions[i] = Hashtable.Synchronized(new Hashtable());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="count"></param>
		/// <param name="capacity"></param>
		public PartitionedDictionary(int count, int capacity)
		{
			InitPartitionArray(count);

			for (int i = 0; i < count; i++)
				this.partitions[i] = Hashtable.Synchronized(new Hashtable(capacity));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="count"></param>
		/// <param name="capacity"></param>
		/// <param name="comparer"></param>
		public PartitionedDictionary(int count, int capacity, IEqualityComparer comparer)
		{
			InitPartitionArray(count);

			for (int i = 0; i < count; i++)
				this.partitions[i] = Hashtable.Synchronized(new Hashtable(capacity, comparer));
		}

		/// <summary>
		/// 分区的个数
		/// </summary>
		public int ParitionCount
		{
			get
			{
				return this.partitions.Length;
			}
		}

		/// <summary>
		/// 字典的总数
		/// </summary>
		public int Count
		{
			get
			{
				int count = 0;

				foreach (Hashtable table in AllPartitions)
					count += table.Count;

				return count;
			}
		}

		/// <summary>
		/// 所有的分区信息
		/// </summary>
		public IEnumerable<Hashtable> AllPartitions
		{
			get
			{
				return this.partitions;
			}
		}

		/// <summary>
		/// 同步对象
		/// </summary>
		public object SyncRoot
		{
			get
			{
				return this.syncRoot;
			}
		}

		/// <summary>
		/// 在字典中添加一项
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void Add(TKey key, TValue value)
		{
			Hashtable partition = GetPartitionByKey(key);

			partition.Add(key, value);
		}

		/// <summary>
		/// 在字典中删除一项
		/// </summary>
		/// <param name="key"></param>
		public void Remove(TKey key)
		{
			Hashtable partition = GetPartitionByKey(key);

			partition.Remove(key);
		}

		/// <summary>
		/// 清除所有数据
		/// </summary>
		public void Clear()
		{
			lock (this.SyncRoot)
			{
				this.partitions.ForEach(p => p.Clear());
			}
		}

		/// <summary>
		/// 是否包含Key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool ContainsKey(TKey key)
		{
			Hashtable partition = GetPartitionByKey(key);

			return partition.ContainsKey(key);
		}

		/// <summary>
		/// 根据Key访问数据
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public TValue this[TKey key]
		{
			get
			{
				Hashtable partition = GetPartitionByKey(key);

				return (TValue)partition[key];
			}
			set
			{
				Hashtable partition = GetPartitionByKey(key);

				partition[key] = value;
			}
		}

		/// <summary>
		/// 试图去获取Value，如果不为null，则返回true。这里和Dictionary(Key, Value)不一样。Value为null就表示不存在
		/// </summary>
		/// <param name="key">需要查找的Key</param>
		/// <param name="value">Key对应的Value</param>
		/// <returns></returns>
		public bool TryGetValue(TKey key, out TValue value)
		{
			value = (TValue)this[key];

			return value != null;
		}

		/// <summary>
		/// 得到枚举器
		/// </summary>
		/// <returns></returns>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			for (int i = 0; i < this.partitions.Length; i++)
			{
				foreach (DictionaryEntry entry in this.partitions[i])
				{
					KeyValuePair<TKey, TValue> kp = new KeyValuePair<TKey, TValue>((TKey)entry.Key, (TValue)entry.Value);

					yield return kp;
				}
			}
		}

		/// <summary>
		/// 得到所有的Key
		/// </summary>
		public ICollection<TKey> Keys
		{
			get
			{
				List<TKey> result = new List<TKey>();

				foreach (Hashtable table in AllPartitions)
				{
					foreach (TKey key in table.Keys)
						result.Add(key);
				}

				return result;
			}
		}

		/// <summary>
		/// 得到所有的Value
		/// </summary>
		public ICollection<TValue> Values
		{
			get
			{
				List<TValue> result = new List<TValue>();

				foreach (Hashtable table in AllPartitions)
				{
					foreach (TValue value in table.Values)
						result.Add(value);
				}

				return result;
			}
		}

		/// <summary>
		/// 得到每一个分区的信息
		/// </summary>
		/// <returns></returns>
		public string GetAllPartitionsInfo()
		{
			StringBuilder strB = new StringBuilder();

			int index = 0;

			using (StringWriter writer = new StringWriter(strB))
			{
				foreach (Hashtable table in this.AllPartitions)
					Console.WriteLine("Table {0}, Count={1}", index++, table.Count);
			}

			return strB.ToString();
		}

		private Hashtable GetPartitionByKey(TKey key)
		{
			if (key == null)
				throw new ArgumentNullException("字典的Key不能为null");

			int hashCode = key.GetHashCode();

			return GetPartitionByHashCode(hashCode);
		}

		private Hashtable GetPartitionByHashCode(int hashCode)
		{
			int index = Math.Abs(hashCode) % this.ParitionCount;

			return this.partitions[index];
		}

		private void InitPartitionArray(int count)
		{
			this.partitions = new Hashtable[count];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
