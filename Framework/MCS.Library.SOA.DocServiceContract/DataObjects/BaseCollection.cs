using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DocServiceContract
{
    /// <summary>
    /// 集合类型基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CollectionDataContract(Name = "{0}Collection",IsReference=true)]
	[Serializable]
    
    public class BaseCollection<T> : IList<T>
    {
        List<T> container = new List<T>();


        public int IndexOf(T item)
        {
            return container.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            container.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            container.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return container[index];
            }
            set
            {
                container[index] = value;
            }
        }

        public void Add(T item)
        {
            container.Add(item);
        }

        public void Clear()
        {
            container.Clear();
        }

        public bool Contains(T item)
        {
            return container.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            container.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return container.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return container.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return container.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return container.GetEnumerator();
        }
    }
}
