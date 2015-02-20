using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
namespace MCS.Library.SOA.Contracts.DataObjects
{

    public abstract class ClientCollectionBase<T> : IList<T>
    {

        private List<T> _InnerList=new List<T>();
        private object _Object = new object();
        private bool _IsSynchronized = false;
        protected ClientCollectionBase()
        {
           
        }

        #region  CollectionBase Member


        public int Count
        { get { return _InnerList.Count; } }

        protected List<T> InnerList
        { get { return _InnerList; } }

        protected IList<T> List
        { get { return this; } }



        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (T item in _InnerList)
                yield return item;
        }
        public IEnumerator<T> GetEnumerator()
        {
            foreach (T item in _InnerList)
                yield return item;
        }

        protected virtual void OnClear()
        {

        }

        protected virtual void OnClearComplete()
        {


        }

        protected virtual void OnInsert(int index, T value)
        {

        }

        protected virtual void OnInsertComplete(int index, T value)
        { }

        protected virtual void OnRemove(int index, T value)
        {

        }

        protected virtual void OnRemoveComplete(int index, T value)
        { }

        protected virtual void OnSet(int index, object oldValue, T newValue)
        { }

        protected virtual void OnSetComplete(int index, object toldValue, T newValue)
        { }

        protected virtual void OnValidate(T value)
        { }


        #endregion

        #region  IList Member



        public bool IsReadOnly
        {
            get;
            set;
        }
        public bool IsSynchronized
        {
            get { return _IsSynchronized; }
        }
        public object SyncRoot
        {
            get { return _Object; }
        }

        public void CopyTo(T[] array, int index)
        {
            _InnerList.CopyTo(array, index);
        }




        public virtual bool Contains(T value)
        {
            return _InnerList.Contains((T)value);
        }

        public virtual int IndexOf(T value)
        {
            return _InnerList.IndexOf((T)value);
        }




        public virtual T this[int index]
        {
            get
            {
                return _InnerList[index];
            }
            set
            {
                object obj = _InnerList[index];
                OnValidate(value);
                OnSet(index, obj, value);
                _InnerList[index] = value;
                OnSetComplete(index, obj, value);
            }
        }
        public virtual void Add(T value)
        {
            OnValidate(value);
            OnInsert(Count, value);
            _InnerList.Add(value);
            OnInsertComplete(Count, value);
        }
        public virtual void Insert(int index,T value)
        {
            OnValidate(value);
            OnInsert(index, value);
            _InnerList.Insert(index,value);
            OnInsertComplete(index, value);
        }
        public virtual void Clear()
        {
            OnClear();
            _InnerList.Clear();
            OnClearComplete();
        }

        public virtual bool Remove(T value)
        {
            bool result = false;
            int index = this.IndexOf(value);
            OnRemove(index, value);
            result = _InnerList.Remove(value);
            OnRemoveComplete(index, value);
            return result;
        }
        public virtual void RemoveAt(int index)
        {
            T value = this[index];
            OnRemove(index, value);
            _InnerList.RemoveAt(index);
            OnRemoveComplete(index, value);
        }

        #endregion  IList Member

    }
}


