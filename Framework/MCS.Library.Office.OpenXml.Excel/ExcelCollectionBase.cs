using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MCS.Library.Office.OpenXml.Excel
{
    public abstract class ExcelCollectionBase<TKey, TItem> : ICollection<TItem>, IEnumerable<TItem>, ICollection, IEnumerable
    {
        private readonly object _SyncObject = new object();

        protected Dictionary<TKey, TItem> _InnerDict = null;
        protected IEqualityComparer<TKey> _Comparer = null;
        protected int _Capacity;

        protected ExcelCollectionBase()
        {

        }

        protected ExcelCollectionBase(int capacity)
        {
            this._Capacity = capacity;
        }

        protected ExcelCollectionBase(IEqualityComparer<TKey> comparer)
        {
            this._Comparer = comparer;
        }

        protected ExcelCollectionBase(IEnumerable<TItem> collection)
        {
            int repeatedItemCount = collection.GroupBy(p => this.GetKeyForItem(p)).Count(p => p.Count() > 1);

            if (repeatedItemCount > 0)
            {
                throw new ArgumentException("初始化数据错误，包含Key重复的项");
            }

            foreach (var item in collection)
            {
                this.InnerDict.Add(GetKeyForItem(item), item);
            }
        }

        protected Dictionary<TKey, TItem> InnerDict
        {
            get
            {
                if (this._InnerDict == null)
                {
                    if (this._Comparer == null)
                        this._InnerDict = new Dictionary<TKey, TItem>(this._Capacity);
                    else
                        this._InnerDict = new Dictionary<TKey, TItem>(this._Capacity, this._Comparer);
                }

                return this._InnerDict;
            }
            set
            {
                this._InnerDict = value;
            }
        }

        protected abstract TKey GetKeyForItem(TItem item);

        public TItem this[TKey key]
        {
            get
            {
                return (TItem)InnerDict[key];
            }
        }

        public bool ContainsKey(TKey key)
        {
            return InnerDict.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return this.InnerDict.Remove(key);
        }

        #region ICollection<TItem>
        public void Add(TItem item)
        {
            TKey key = GetKeyForItem(item);
            this.InnerDict.Add(key, item);
        }

        public void Clear()
        {
            this.InnerDict.Clear();
        }

        public bool Contains(TItem item)
        {
            TKey key = GetKeyForItem(item);
            return this.InnerDict.ContainsKey(key);
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            int i = arrayIndex;
            foreach (var item in this.InnerDict.Values)
            {
                if (i >= array.Length)
                {
                    return;
                }
                array[i] = item;
                i++;
            }
        }

        public int Count
        {
            get
            {
                return this.InnerDict.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(TItem item)
        {
            TKey key = GetKeyForItem(item);
            return this.InnerDict.Remove(key);
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return this.InnerDict.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.InnerDict.Values.GetEnumerator();
        }
        #endregion

        #region ICollection
        void ICollection.CopyTo(Array array, int index)
        {
            int i = index;
            foreach (var item in this.InnerDict.Values)
            {
                if (i >= array.Length)
                {
                    return;
                }
                array.SetValue(item, i);
                i++;
            }
        }

        int ICollection.Count
        {
            get
            {
                return this.InnerDict.Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this._SyncObject;
            }
        }
        #endregion
    }
}
