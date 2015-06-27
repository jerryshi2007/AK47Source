using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Properties;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// �����е�������Ӻ�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sender"></param>
    /// <param name="index"></param>
    /// <param name="data"></param>
    public delegate void DataObjectCollectionDataInsertedHandler<T>(ReadOnlyDataObjectCollectionBase<T> sender, int index, T data);

    /// <summary>
    /// ȥ���ظ����ݵıȽ���
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="srcData"></param>
    /// <param name="destData"></param>
    /// <returns></returns>
    public delegate bool DistinctComparison<T>(T srcData, T destData);

    /// <summary>
    /// ֻ�����ݶ��󼯺���������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [ComVisible(true)]
    [DebuggerDisplay("Count = {Count}")]
    [NonXElementSerializedFields(typeof(CollectionBase), "list")]
    public abstract class ReadOnlyDataObjectCollectionBase<T> : CollectionBase, IEnumerable<T>, IXmlSerilizableList
    {
        /// <summary>
        /// ���췽��
        /// </summary>
        protected ReadOnlyDataObjectCollectionBase()
        {
        }

        /// <summary>
        /// ���췽������������ʱ�ķ�������
        /// </summary>
        /// <param name="capacity"></param>
        protected ReadOnlyDataObjectCollectionBase(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// ������������Ӻ���¼�
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event DataObjectCollectionDataInsertedHandler<T> DataInserted;

        /// <summary>
        /// ��������ÿһ��Ԫ��
        /// </summary>
        /// <param name="action"></param>
        public virtual void ForEach(Action<T> action)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(action != null, "action");

            foreach (T item in List)
                action(item);
        }

        /// <summary>
        /// �жϼ������Ƿ����ĳԪ��
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
        /// �жϼ�����ÿ��Ԫ���Ƿ�����ĳ�������������Ϊ�գ�Ҳ����True
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
        /// �жϼ�����ÿ��Ԫ���Ƿ�����ĳ�������Ҽ��ϲ�Ϊ��
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public virtual bool TrueForAllAndNotEmpty(Predicate<T> match)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(match != null, "match");

            bool result = (this.Count > 0);

            if (result)
            {
                foreach (T item in List)
                {
                    if (match(item) == false)
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// �ڼ����в�������ƥ�������ĵ�һ��Ԫ��
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
        /// �Ӻ���ǰ���ң��ҵ���һ��ƥ���Ԫ��
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
        /// �ҵ�����ƥ������������Ԫ��
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
        /// �Ƿ����ĳ��Ԫ��
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Contains(T item)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(item != null, "item");

            return List.Contains(item);
        }

        /// <summary>
        /// �õ�ĳ��Ԫ�ص�λ��
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual int IndexOf(T item)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(item != null, "item");

            return List.IndexOf(item);
        }

        /// <summary>
        /// ���Ƶ���ļ�����
        /// </summary>
        /// <param name="collection"></param>
        public void CopyTo(ICollection<T> collection)
        {
            this.CopyTo(collection, null, null);
        }

        /// <summary>
        /// ���Ƶ���ļ����С���ɸѡ����
        /// </summary>
        /// <param name="collection">����</param>
        /// <param name="predicate">ɸѡ����</param>
        public void CopyTo(ICollection<T> collection, Predicate<T> predicate)
        {
            this.CopyTo(collection, predicate, null);
        }

        /// <summary>
        /// ���Ƶ���ļ����С���ɸѡ����
        /// </summary>
        /// <param name="collection">����</param>
        /// <param name="converter">ת����</param>
        public void CopyTo(ICollection<T> collection, Converter<T, T> converter)
        {
            this.CopyTo(collection, null, converter);
        }

        /// <summary>
        /// ���Ƶ���ļ����С������п��Դ�ɸѡ������ת����
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="predicate">ɸѡ����</param>
        /// <param name="converter">ת����</param>
        public virtual void CopyTo(ICollection<T> collection, Predicate<T> predicate, Converter<T, T> converter)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(collection != null, "collection");

            this.ForEach(delegate(T item)
            {
                if (predicate == null || predicate(item))
                {
                    T newItem = item;

                    if (converter != null)
                        newItem = converter(item);

                    collection.Add(newItem);
                }
            });
        }

        /// <summary>
        /// ת��������
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
        /// ����ToString����Count���
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = base.ToString();

            result += string.Format("({0:#,##0})", this.Count);

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void InnerAdd(T obj)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(obj != null, "obj");

            List.Add(obj);
        }

        #region IEnumerable<T> ��Ա
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public new IEnumerator<T> GetEnumerator()
        {
            foreach (T item in List)
                yield return item;
        }

        #endregion

        #region IXmlSerilizableList Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public virtual void Add(object data)
        {
            this.InnerAdd((T)data);
        }
        #endregion

        /// <summary>
        /// ��������Ӻ�
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        protected override void OnInsertComplete(int index, object value)
        {
            base.OnInsertComplete(index, value);

            if (DataInserted != null)
                DataInserted(this, index, (T)value);
        }
    }

    /// <summary>
    /// ���ݶ��󼯺���������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [ComVisible(true)]
    public abstract class DataObjectCollectionBase<T> : ReadOnlyDataObjectCollectionBase<T>
    {
        /// <summary>
        /// ���췽��
        /// </summary>
        protected DataObjectCollectionBase()
        {
        }

        /// <summary>
        /// ���췽������������ʱ�ķ�������
        /// </summary>
        /// <param name="capacity"></param>
        protected DataObjectCollectionBase(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// �ӱ�ļ����и���(��ӵ����еļ�����)
        /// </summary>
        /// <param name="data"></param>
        public void CopyFrom(IEnumerable<T> data)
        {
            this.CopyFrom(data, null, null);
        }

        /// <summary>
        /// �ӱ�ļ����и���(��ӵ����еļ�����)�����ƵĹ��̿���ɸѡ����
        /// </summary>
        /// <param name="data"></param>
        /// <param name="predicate">ɸѡ����</param>
        public void CopyFrom(IEnumerable<T> data, Predicate<T> predicate)
        {
            this.CopyFrom(data, predicate, null);
        }

        /// <summary>
        /// �ӱ�ļ����и���(��ӵ����еļ�����)�����ƵĹ��̿���ת������
        /// </summary>
        /// <param name="data"></param>
        /// <param name="converter">����ת����</param>
        public void CopyFrom(IEnumerable<T> data, Converter<T, T> converter)
        {
            this.CopyFrom(data, null, converter);
        }

        /// <summary>
        /// �ӱ�ļ����и���(��ӵ����еļ�����)�����ƵĹ��̿���ɸѡ��ת������
        /// </summary>
        /// <param name="data">Դ����</param>
        /// <param name="predicate">ɸѡ����</param>
        /// <param name="converter">ת����</param>
        public virtual void CopyFrom(IEnumerable<T> data, Predicate<T> predicate, Converter<T, T> converter)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(data != null, "data");

            IEnumerator<T> enumerator = data.GetEnumerator();

            while (enumerator.MoveNext())
            {
                T item = (T)enumerator.Current;

                if (predicate == null || predicate(item))
                {
                    T newItem = item;

                    if (converter != null)
                        newItem = converter(item);

                    this.InnerAdd(newItem);
                }
            }
        }

        /// <summary>
        /// ɾ�����������ļ�¼
        /// </summary>
        /// <param name="match"></param>
        public bool Remove(Predicate<T> match)
        {
            int i = 0;
            bool result = false;

            while (i < this.Count)
            {
                T data = (T)List[i];

                if (match(data))
                {
                    this.RemoveAt(i);
                    result = true;
                }
                else
                    i++;
            }

            return result;
        }
    }

    /// <summary>
    /// �ܹ��༭�����ݶ��󼯺���������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [ComVisible(true)]
    public abstract class EditableDataObjectCollectionBase<T> : DataObjectCollectionBase<T>, ICollection<T>
    {
        private bool _IsReadOnly = false;

        /// <summary>
        /// ���췽��
        /// </summary>
        protected EditableDataObjectCollectionBase()
        {
        }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="isReadOnly">�Ƿ�ֻ������</param>
        protected EditableDataObjectCollectionBase(bool isReadOnly)
        {
            this._IsReadOnly = isReadOnly;
        }

        /// <summary>
        /// ���췽������������ʱ�ķ�������
        /// </summary>
        /// <param name="capacity"></param>
        protected EditableDataObjectCollectionBase(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// �Ƿ�ֻ������
        /// </summary>
        public bool IsReadOnly
        {
            get { return this._IsReadOnly; }
            set { this._IsReadOnly = value; }
        }

        /// <summary>
        /// ���һ������
        /// </summary>
        /// <param name="data"></param>
        public virtual void Add(T data)
        {
            InnerAdd(data);
        }

        /// <summary>
        /// ���Ӳ����ڵ����ݣ��Ѿ����ڵĽ������ԣ�
        /// </summary>
        /// <param name="data"></param>
        /// <param name="predicate"></param>
        /// <returns>�Ƿ�����������</returns>
        public virtual bool AddNotExistsItem(T data, Predicate<T> predicate)
        {
            data.NullCheck("data");

            bool needToAdd = predicate == null || this.Exists(predicate) == false;

            if (needToAdd)
                this.Add(data);

            return needToAdd;
        }

        /// <summary>
        /// ��д����
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual T this[int index]
        {
            get { return (T)List[index]; }
            set { List[index] = value; }
        }

        /// <summary>
        /// ����Ĭ�ϵĹ����������(Comparer(T).Default)
        /// </summary>
        public void Sort()
        {
            Comparison<IComparable> comparison = (left, right) => ((IComparable)left).CompareTo(right);

            this.List.QuickSortList(comparison);
        }

        /// <summary>
        /// ����ָ���Ĺ����������
        /// </summary>
        /// <param name="comparison"></param>
        public void Sort(Comparison<T> comparison)
        {
            comparison.NullCheck("comparison");

            this.List.QuickSortList(comparison);
        }

        /// <summary>
        /// ����ָ���Ĺ����������
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(IComparer<T> comparer)
        {
            comparer.NullCheck("comparer");

            this.List.QuickSortList(comparer);
        }

        /// <summary>
        /// ȥ���ظ���Ԫ��
        /// </summary>
        /// <param name="comparison"></param>
        public void Distinct(DistinctComparison<T> comparison)
        {
            comparison.NullCheck("comparer");

            List<T> distinctList = new List<T>();

            foreach (T originalData in this)
            {
                if (distinctList.Exists(targetData => comparison(originalData, targetData)) == false)
                    distinctList.Add(originalData);
            }

            this.Clear();
            this.CopyFrom(distinctList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        protected override void OnInsert(int index, object value)
        {
            CheckReadOnly();
            base.OnInsert(index, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnSet(int index, object oldValue, object newValue)
        {
            CheckReadOnly();
            base.OnSet(index, oldValue, newValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        protected override void OnRemove(int index, object value)
        {
            CheckReadOnly();
            base.OnRemove(index, value);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnClear()
        {
            CheckReadOnly();
            base.OnClear();
        }

        private void CheckReadOnly()
        {
            if (this.IsReadOnly)
                throw new SystemSupportException(Resource.CollectionIsReadOnly);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            int index = 0;

            while (arrayIndex + index < array.Length)
            {
                array[arrayIndex + index] = this[index++];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Remove(T item)
        {
            return base.Remove(obj =>
                        {
                            bool result = false;

                            if (obj != null)
                                result = obj.Equals(item);
                            else
                                if (item == null)
                                    result = true;

                            return result;
                        });
        }
    }

    /// <summary>
    /// ��Key�ļ����࣬���԰���Key��Index���ַ�ʽ����
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TItem"></typeparam>
    [Serializable]
    public abstract class EditableKeyedDataObjectCollectionBase<TKey, TItem> : EditableDataObjectCollectionBase<TItem>
    {
        private static readonly string DuplicateDescriptorKey = Resource.DuplicateDescriptorKey;

        /// <summary>
        /// 
        /// </summary>
        protected IEqualityComparer _Comparer = null;

        /// <summary>
        /// ���췽��
        /// </summary>
        protected EditableKeyedDataObjectCollectionBase()
        {
        }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="comparer"></param>
        protected EditableKeyedDataObjectCollectionBase(IEqualityComparer comparer)
        {
            this._Comparer = comparer;
        }

        /// <summary>
        /// ���췽������������ʱ�ķ�������
        /// </summary>
        /// <param name="capacity"></param>
        protected EditableKeyedDataObjectCollectionBase(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// ���췽������������ʱ�ķ�������
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="comparer"></param>
        protected EditableKeyedDataObjectCollectionBase(int capacity, IEqualityComparer comparer)
            : base(capacity)
        {
            this._Comparer = comparer;
        }

        [NonSerialized]
        private Hashtable _InnerDict = null;

        private Hashtable InnerDict
        {
            get
            {
                if (this._InnerDict == null)
                {
                    if (this._Comparer == null)
                        this._InnerDict = new Hashtable(this.Capacity);
                    else
                        this._InnerDict = new Hashtable(this.Capacity, this._Comparer);
                }

                return this._InnerDict;
            }
        }

        /// <summary>
        /// ȷ��Item��Key
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected abstract TKey GetKeyForItem(TItem item);

        /// <summary>
        /// ����Key��������ȡ����
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TItem this[TKey key]
        {
            get
            {
                return (TItem)InnerDict[key];
            }
        }

        /// <summary>
        /// ���Ӳ����ڵ����ݣ��Ѿ����ڵĽ������ԣ�
        /// </summary>
        /// <param name="data"></param>
        public void AddNotExistsItem(TItem data)
        {
            data.NullCheck("data");

            if (ContainsKey(GetKeyForItem(data)) == false)
                this.Add(data);
        }

        /// <summary>
        /// ָ����key�Ƿ����
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return InnerDict.ContainsKey(key);
        }

        /// <summary>
        /// ���key���ڣ���ִ��Action
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        public void ContainsKey(TKey key, Action<TItem> action)
        {
            if (ContainsKey(key) && action != null)
                action(this[key]);
        }

        /// <summary>
        /// ���Ƶ��ֵ���
        /// </summary>
        /// <param name="dict"></param>
        public void CopyTo(IDictionary<TKey, TItem> dict)
        {
            dict.Clear();

            foreach (DictionaryEntry entry in this.InnerDict)
                dict.Add((TKey)entry.Key, (TItem)entry.Value);
        }

        /// <summary>
        /// ���ֶ��и���
        /// </summary>
        /// <param name="dict"></param>
        public void CopyFrom(IDictionary<TKey, TItem> dict)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(dict != null, "dict");

            this.Clear();
            InnerDict.Clear();

            foreach (var keyValue in dict)
            {
                List.Add(keyValue.Value);
            }
        }

        /// <summary>
        /// �õ����е�Key
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TKey> GetAllKeys()
        {
            foreach (DictionaryEntry entry in this.InnerDict)
            {
                yield return (TKey)entry.Key;
            }
        }

        /// <summary>
        /// ��ɾ��ʱ
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        protected override void OnRemove(int index, object value)
        {
            base.OnRemove(index, value);

            TItem data = (TItem)value;
            InnerDict.Remove(GetKeyForItem(data));
        }

        /// <summary>
        /// ��ɾ�����ʱ
        /// </summary>
        protected override void OnClearComplete()
        {
            base.OnClearComplete();
            InnerDict.Clear();
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        protected override void OnInsert(int index, object value)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(value != null, "value");

            TItem data = (TItem)value;

            TKey key = GetKeyForItem(data);

            if (InnerDict.ContainsKey(key))
            {
                OnDuplicateKey(key, data);
                key = GetKeyForItem(data);
            }

            InnerDict.Add(key, data);

            base.OnInsert(index, data);
        }

        /// <summary>
        /// ����ͼ�����ظ�Keyʱ
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        protected virtual void OnDuplicateKey(TKey key, TItem data)
        {
            throw new ArgumentException(string.Format(DuplicateDescriptorKey, this.GetType().Name, typeof(TItem).Name, key));
        }
    }

    /// <summary>
    /// �����л���EditableKeyedDataObjectCollectionBase�������
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TItem"></typeparam>
    [Serializable]
    public abstract class SerializableEditableKeyedDataObjectCollectionBase<TKey, TItem> : EditableKeyedDataObjectCollectionBase<TKey, TItem>, ISerializable
    {
        /// <summary>
        /// 
        /// </summary>
        public SerializableEditableKeyedDataObjectCollectionBase()
        {
        }

        /// <summary>
        /// ���췽������������ʱ�ķ�������
        /// </summary>
        /// <param name="capacity"></param>
        protected SerializableEditableKeyedDataObjectCollectionBase(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// ���췽����ָ�����ϵ�Key�Ƚ���
        /// </summary>
        /// <param name="comparer"></param>
        protected SerializableEditableKeyedDataObjectCollectionBase(IEqualityComparer comparer)
            : base(comparer)
        {
        }

        /// <summary>
        /// ���췽������������ʱ�ķ�������
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="comparer"></param>
        protected SerializableEditableKeyedDataObjectCollectionBase(int capacity, IEqualityComparer comparer)
            : base(capacity)
        {
            this._Comparer = comparer;
        }
        #region Serializable
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("List", this.InnerList);

            if (this._Comparer != null)
            {
                info.AddValue("ComparerType", this._Comparer.GetType().AssemblyQualifiedName);
                info.AddValue("Comparer", this._Comparer);
            }
            else
                info.AddValue("ComparerType", string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SerializableEditableKeyedDataObjectCollectionBase(SerializationInfo info, StreamingContext context)
        {
            ArrayList list = (ArrayList)info.GetValue("List", typeof(ArrayList));

            string comparerTypeDesp = info.GetString("ComparerType");

            if (comparerTypeDesp.IsNotEmpty())
            {
                Type comparerType = TypeCreator.GetTypeInfo(comparerTypeDesp);
                this._Comparer = (IEqualityComparer)info.GetValue("Comparer", comparerType);
            }

            foreach (TItem obj in list)
                base.Add(obj);
        }
        #endregion Serializable
    }
}
