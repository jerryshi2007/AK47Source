using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary
{
    public class NameObjectCollection : NameObjectCollectionBase
    {
        public NameObjectCollection()
        {
        }

        public NameObjectCollection(int capacity)
            : base(capacity)
        {

        }

        public NameObjectCollection(NameObjectCollection source)
            : base(source.Count)
        {
            for (int i = 0; i < source.Count; i++)
            {
                this.BaseAdd(source.BaseGetKey(i), source.BaseGet(i));
            }
        }

        public DictionaryEntry this[int index]
        {
            get
            {
                return (new DictionaryEntry(
                    this.BaseGetKey(index), this.BaseGet(index)));
            }
        }

        // Gets or sets the value associated with the specified key.
        public object this[string key]
        {
            get
            {
                return (object)(this.BaseGet(key));
            }
            set
            {
                this.BaseSet(key, value);
            }
        }

        // Gets a String array that contains all the keys in the collection.
        public string[] AllKeys
        {
            get
            {
                return (this.BaseGetAllKeys());
            }
        }

        // Gets an Object array that contains all the values in the collection.
        public Array AllValues
        {
            get
            {
                return (this.BaseGetAllValues());
            }
        }

        // Gets a String array that contains all the values in the collection.
        public object[] AllObjectValues
        {
            get
            {
                return ((object[])this.BaseGetAllValues(typeof(object)));
            }
        }

        // Gets a value indicating if the collection contains keys that are not null.
        public Boolean HasKeys
        {
            get
            {
                return (this.BaseHasKeys());
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Add(String key, object value)
        {
            this.BaseAdd(key, value);
        }

        // Removes an entry with the specified key from the collection.
        public void Remove(string key)
        {
            this.BaseRemove(key);
        }

        // Removes an entry in the specified index from the collection.
        public void Remove(int index)
        {
            this.BaseRemoveAt(index);
        }

        // Clears all the elements in the collection.
        public void Clear()
        {
            this.BaseClear();
        }
    }
}
