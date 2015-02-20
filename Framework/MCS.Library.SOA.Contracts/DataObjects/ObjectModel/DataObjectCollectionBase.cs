using System;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Xml.Serialization;
namespace MCS.Library.SOA.Contracts.DataObjects
{
	/// <summary>
	/// 集合中的数据添加后
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="sender"></param>
	/// <param name="index"></param>
	/// <param name="data"></param>

    
	public delegate void DataObjectCollectionDataInsertedHandler<T>(ReadOnlyDataObjectCollectionBase<T> sender, int index, T data);

	/// <summary>
	/// 去除重复数据的比较器
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="srcData"></param>
	/// <param name="destData"></param>
	/// <returns></returns>
   
	public delegate bool DistinctComparison<T>(T srcData, T destData);

	/// <summary>
	/// 只读数据对象集合类的虚基类
	/// </summary>
	/// <typeparam name="T"></typeparam>
  
	public abstract class ReadOnlyDataObjectCollectionBase<T> : ClientCollectionBase<T>
	{
		/// <summary>
		/// 集合中数据添加后的事件
		/// </summary>
		public event DataObjectCollectionDataInsertedHandler<T> DataInserted;

		/// <summary>
		/// 迭代处理每一个元素
		/// </summary>
		/// <param name="action"></param>
		public virtual void ForEach(Action<T> action)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(action != null, "action");

			foreach (T item in List)
				action(item);
            
		}

		/// <summary>
		/// 判断集合中是否存在某元素
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public virtual bool Exists(Predicate<T> match)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(match != null, "match");

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

		/// <summary>
		/// 判断集合中每个元素是否都满足某条件
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public virtual bool TrueForAll(Predicate<T> match)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(match != null, "match");

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

		/// <summary>
		/// 在集合中查找满足匹配条件的第一个元素
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public virtual T Find(Predicate<T> match)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(match != null, "match");

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

		/// <summary>
		/// 从后向前查找，找到第一个匹配的元素
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public virtual T FindLast(Predicate<T> match)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(match != null, "match");

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

		/// <summary>
		/// 找到满足匹配条件的所有元素
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public virtual IList<T> FindAll(Predicate<T> match)
		{
			IList<T> result = new List<T>();

			foreach (T item in List)
			{
				if (match(item))
					result.Add(item);
			}

			return result;
		}

		/// <summary>
		/// 是否包含某个元素
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public override bool Contains(T item)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(item != null, "item");

			return List.Contains(item);
		}

		/// <summary>
		/// 得到某个元素的位置
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public override int IndexOf(T item)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(item != null, "item");

			return List.IndexOf(item);
		}

		/// <summary>
		/// 复制到别的集合中
		/// </summary>
		/// <param name="collection"></param>
		public virtual void CopyTo(ICollection<T> collection)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(collection != null, "collection");

			this.ForEach(delegate(T item) { collection.Add(item); });
		}

		/// <summary>
		/// 转换到数组
		/// </summary>
		/// <returns></returns>
		public virtual T[] ToArray()
		{
			T[] result = new T[this.Count];

			for (int i = 0; i < this.Count; i++)
				result[i] = (T)List[i];

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		protected virtual void InnerAdd(T obj)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(obj != null, "obj");

            base.Add(obj);

		}
		#region IEnumerable<T> 成员

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		

		#endregion

		/// <summary>
		/// 当数据添加后
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		protected override void OnInsertComplete(int index, T value)
		{
			base.OnInsertComplete(index, value);

			if (DataInserted != null)
				DataInserted(this, index, (T)value);
		}
	}

	/// <summary>
	/// 数据对象集合类的虚基类
	/// </summary>
	/// <typeparam name="T"></typeparam>
  
	public abstract class DataObjectCollectionBase<T> : ReadOnlyDataObjectCollectionBase<T>
	{
		/// <summary>
		/// 从别的集合中复制(添加到现有的集合中)
		/// </summary>
		/// <param name="data"></param>
		public void CopyFrom(IEnumerable<T> data)
		{
			CopyFrom(data, null);
		}

		/// <summary>
		/// 从别的集合中复制(添加到现有的集合中)，复制过程可以转换数据
		/// </summary>
		/// <param name="data"></param>
		/// <param name="converter">数据转换器</param>
		public void CopyFrom(IEnumerable<T> data, Converter<T, T> converter)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(data != null, "data");

			IEnumerator<T> enumerator = data.GetEnumerator();

			while (enumerator.MoveNext())
			{
				T item = (T)enumerator.Current;
				T newItem = item;

				if (converter != null)
					newItem = converter(item);

				InnerAdd(newItem);
			}
		}

		/// <summary>
		/// 删除满足条件的记录
		/// </summary>
		/// <param name="match"></param>
		public void Remove(Predicate<T> match)
		{
			int i = 0;

			while (i < this.Count)
			{
				T data = (T)List[i];

				if (match(data))
					this.RemoveAt(i);
				else
					i++;
			}
		}
	}

	/// <summary>
	/// 能够编辑的数据对象集合类的虚基类
	/// </summary>
	/// <typeparam name="T"></typeparam>
 
	public abstract class EditableDataObjectCollectionBase<T> : DataObjectCollectionBase<T>
	{
		/// <summary>
		/// 添加一个对象
		/// </summary>
		/// <param name="data"></param>
		public new void Add(T data)
		{
			InnerAdd(data);
		}

		
		

		/// <summary>
		/// 按照默认的规则进行排序(Comparer(T).Default)
		/// </summary>
		public void Sort()
		{
			InnerSort(list => list.Sort());
		}

		/// <summary>
		/// 按照指定的规则进行排序
		/// </summary>
		/// <param name="comparison"></param>
		public void Sort(Comparison<T> comparison)
		{
			comparison.NullCheck("comparison");

			InnerSort(list => list.Sort(comparison));
		}

		/// <summary>
		/// 按照指定的规则进行排序
		/// </summary>
		/// <param name="comparer"></param>
		public void Sort(IComparer<T> comparer)
		{
            
			comparer.NullCheck("comparer");

			InnerSort(list => list.Sort(comparer));
		}

		/// <summary>
		/// 去除重复的元素
		/// </summary>
		/// <param name="comparerison"></param>
		public void Distinct(DistinctComparison<T> comparerison)
		{
			comparerison.NullCheck("comparer");

			List<T> distinctList = new List<T>();

			foreach (T originalData in this)
			{
				if (distinctList.Exists(targetData => comparerison(originalData, targetData)) == false)
					distinctList.Add(originalData);
			}

			this.Clear();
			this.CopyFrom(distinctList);
		}

		private void InnerSort(Action<List<T>> action)
		{
			List<T> tempList = new List<T>();

			this.CopyTo(tempList);

			action(tempList);

			this.Clear();
			this.CopyFrom(tempList);
		}
	}

	/// <summary>
	/// 带Key的集合类，可以按照Key和Index两种方式索引
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TItem"></typeparam>

    public abstract class EditableKeyedDataObjectCollectionBase<TKey, TItem> : EditableDataObjectCollectionBase<TItem>
    {

        private Hashtable _InnerDict = new Hashtable();

        /// <summary>
        /// 确定Item的Key
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected abstract TKey GetKeyForItem(TItem item);

        /// <summary>
        /// 按照Key的索引获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TItem this[TKey key]
        {
            get
            {
                return (TItem)_InnerDict[key];
            }
        }

        protected override void InnerAdd(TItem obj)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(obj != null, "obj");

            List.Add(obj);
               
        }
        /// <summary>
        /// 指定的key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return _InnerDict.ContainsKey(key);
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        protected override void OnInsert(int index, TItem value)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(value != null, "value");

            TItem data = (TItem)value;

            TKey key = GetKeyForItem(data);
            

            ExceptionHelper.TrueThrow<ArgumentException>(_InnerDict.ContainsKey(key),
                "集合{0}包含重复的键值{1}"+this.GetType().ToString(), typeof(TItem).Name, key);

            _InnerDict.Add(GetKeyForItem(data), data);

            base.OnInsert(index, data);
        }

        /// <summary>
        /// 复制到字典中
        /// </summary>
        /// <param name="dict"></param>
        public void CopyTo(IDictionary<TKey, TItem> dict)
        {
            dict.Clear();

            foreach (DictionaryEntry entry in this._InnerDict)
                dict.Add((TKey)entry.Key, (TItem)entry.Value);
        }

        /// <summary>
        /// 从字段中复制
        /// </summary>
        /// <param name="dict"></param>
        public void CopyFrom(IDictionary<TKey, TItem> dict)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(dict != null, "dict");

            this.Clear();
            _InnerDict.Clear();

            foreach (var keyValue in dict)
            {
                List.Add(keyValue.Value);
            }
        }

        /// <summary>
        /// 当删除时
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        protected override void OnRemove(int index, TItem value)
        {
            base.OnRemove(index, value);

            TItem data = (TItem)value;
            _InnerDict.Remove(GetKeyForItem(data));
        }

        /// <summary>
        /// 当删除完成时
        /// </summary>
        protected override void OnClearComplete()
        {
            base.OnClearComplete();
            _InnerDict.Clear();
        }
    }
}
